using EmployeeCrud.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace kendoCrudMvc.Models
{
    public class Datalayer
    {
        SqlConnection con = new SqlConnection();

        public Datalayer()
        {
            con.ConnectionString = ConfigurationManager.ConnectionStrings["KendoContext"].ConnectionString;
        }

        public void query(string sql)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = sql;
            cmd.CommandType = CommandType.Text;
            con.Open();
            cmd.Connection = con;
            cmd.ExecuteNonQuery();
            cmd.Dispose();
            con.Close();
        }

        public List<User> GetUser()
        {
            try
            {
                DataTable dt = new DataTable();
                DataSet ds = new DataSet();
                var userlist = new List<User>();
                SqlDataAdapter da = new SqlDataAdapter("select * from Employee", con);
                ds.Reset();
                da.Fill(ds);
                dt = ds.Tables[0];
                if (dt.Rows.Count > 0)
                {
                    int indexRow = 0;
                    foreach (DataRow row in dt.Rows)
                    {
                        var user = new User();
                        user.Id = indexRow;
                        user.FirstName = row["first_name"].ToString();
                        user.LastName = row["last_name"].ToString();
                        userlist.Add(user);
                        indexRow++;
                    }
                }
                return userlist;
            }
            catch (Exception e)
            {
                if (e.Source != null)
                    Console.WriteLine("source: {0}", e.Source);
                throw;
            }
        }

        public List<UserModel> getusers()
        {
            try
            {
                DataTable dt = new DataTable();
                DataSet ds = new DataSet();
                List<UserModel> userlist = new List<UserModel>();
                string sql = "SELECT * FROM [Cred].[EmplServiceDeliveryHistorical] order by employee";
                SqlDataAdapter da = new SqlDataAdapter(sql, con);
                ds.Reset();
                da.Fill(ds);
                dt = ds.Tables[0];

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        try
                        {
                            UserModel user = new UserModel();
                            user.userid = Convert.ToInt32(row["userid"].ToString());
                            user.employee = row["employee"].ToString();
                            user.Empl_external_id = row["Empl_external_id"].ToString();
                            user.svc_delivery_amount = row["svc_delivery_amount"].ToString();
                            //DateTime frmdt = Convert.ToDateTime(row["svc_del_effectivedate"]);
                            //string frmdtString = frmdt.ToString("yyyy-MM-dd");

                            //var date = dateAndTime.Date;
                            //var effectivedate = row["svc_del_effectivedate"];
                            //var effectivedate = row["svc_del_effectivedate"] != null ? (DateTime)row["svc_del_effectivedate"] : DateTime.MinValue;
                            //var enddate = row["svc_del_enddate"] != null ? (DateTime)row["svc_del_enddate"] : DateTime.MinValue;
                            user.svc_del_effectivedate = Convert.ToDateTime(row["svc_del_effectivedate"]).ToString("yyyy-MM-dd");
                            //user.svc_del_effectivedate = DateTime.ParseExact(effectivedate.ToString(), "MM/dd/yyyy", CultureInfo.InvariantCulture);
                            if (Convert.ToDateTime(row["svc_del_enddate"]).ToString("yyyy-MM-dd") == "2199-12-31")
                            {
                                user.svc_del_enddate = Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd");
                            }
                            else
                            {
                                user.svc_del_enddate = Convert.ToDateTime(row["svc_del_enddate"]).ToString("yyyy-MM-dd");
                            }
                            //user.svc_del_enddate=DateTime.ParseExact(enddate.ToString(), "MM/dd/yyyy", CultureInfo.InvariantCulture);   
                            user.program_desc = row["program_desc"].ToString();
                            user.program_group = row["program_group"].ToString();
                            user.emp_status = row["emp_status"].ToString();
                            userlist.Add(user);
                        }
                        catch (Exception ex)
                        {
                            continue;
                        }
                    }
                }
                
                return userlist;
            }
            catch (Exception e)
            {
                if (e.Source != null)
                    Console.WriteLine("source: {0}", e.Source);
                throw;
            }
        }

        public DataTable getData(string sql)
        {
            DataTable dt = new DataTable();
            DataSet ds = new DataSet();            
            SqlDataAdapter da = new SqlDataAdapter(sql, con);
            ds.Reset();
            da.Fill(ds);
            dt = ds.Tables[0];
            return dt;
        }

        public List<EmpModel> getEmpHistusersToBeDeleted(UserModel users, DateTime effectivedate, DateTime enddate)
        {
            List<EmpModel> userlist = new List<EmpModel>();
            string sql = "";
            DataTable dtEmp = new DataTable();
            DataTable dtOverLap = new DataTable();
            DataTable dtGetEmpRowID = new DataTable();
            sql = "select * from [dbo].[View_EmployeePrimaryTeam] where EmpName ='" + users.employee + "'";
            dtEmp = getData(sql);
            if (dtEmp.Rows.Count > 0)
            {
                foreach (DataRow row in dtEmp.Rows)
                {
                    EmpModel user = new EmpModel();
                    if (users.userid > 0)
                    {
                        sql = "SELECT userid FROM [cred].[EmplServiceDeliveryHistorical] " +
                                         " WHERE employee = '" + users.employee + "' and svc_delivery_amount ='" + users.svc_delivery_amount + "' and " +
                                         " convert(date, svc_del_effectivedate) = '" + effectivedate + "' and convert(date, svc_del_enddate) ='" + enddate + "' and program_desc ='" + row["Program"].ToString() + "'";
                        dtGetEmpRowID = getData(sql);
                        if (dtGetEmpRowID.Rows.Count > 0)
                        {
                            user.userid = Convert.ToInt32(dtGetEmpRowID.Rows[0]["userid"].ToString());
                        }
                        else
                        {
                            user.userid = 0;
                        }
                    }
                    user.EmpName = row["EmpName"].ToString();
                    user.EmpExtID = Convert.ToInt32(row["EmpExtID"].ToString());
                    user.Amount = users.svc_delivery_amount.ToString();
                    user.Effectivedate = effectivedate.ToString();
                    user.Enddate = enddate.ToString();
                    user.Program = row["Program"].ToString();
                    user.ServiceLine = row["ServiceLine"].ToString();
                    user.EmpStatus = row["EmpStatus"].ToString();
                    userlist.Add(user);
                    users.errorStr = "";
                }
            }
                return userlist;
        }

        public List<EmpModel> getEmpHistusers(UserModel users, DateTime effectivedate, DateTime enddate)
        {
            try
            {
               
                UserModel presentUser = new UserModel();
                DataTable dtEmp = new DataTable();
                DataTable dtOverLap = new DataTable();
                DataTable dtGetEmpRowID = new DataTable();
                List<EmpModel> userlist = new List<EmpModel>();


                DateTime date1 = effectivedate;
                DateTime date2 = enddate;
                int result = DateTime.Compare(date1, date2);

                if (result == 1)
                {
                    users.errorStr = "Start date should be greater than end date";
                    return userlist;
                }
                string sql = "";

                if (users.userid > 0)
                {
                    sql = "select * FROM [cred].[EmplServiceDeliveryHistorical] where userid ='" + users.userid + "'";                                  
                    if (getData(sql).Rows.Count > 0)
                    {
                        foreach (DataRow row in getData(sql).Rows)
                        {
                            presentUser.employee = row["employee"].ToString();
                            presentUser.employee = row["employee"].ToString();
                            presentUser.svc_delivery_amount = row["svc_delivery_amount"].ToString();
                            presentUser.svc_del_effectivedate  = Convert.ToDateTime(row["svc_del_effectivedate"]).ToString("yyyy-MM-dd");                           
                            presentUser.svc_del_enddate = Convert.ToDateTime(row["svc_del_enddate"]).ToString("yyyy-MM-dd");                            
                        }
                    }
                }
                if (users.errorStr=="delete")
                {
                    users = presentUser;
                }
                              
                sql = "select * from [dbo].[View_EmployeePrimaryTeam] where EmpName ='" + users.employee + "'";               
                dtEmp = getData(sql);

                if (dtEmp.Rows.Count > 0)
                {                  
                    sql = "SELECT * FROM [cred].[EmplServiceDeliveryHistorical] " +
                        " WHERE employee = '" + users.employee + "' and svc_delivery_amount ='" + users.svc_delivery_amount + "' and " +                       
                        " (('" + effectivedate + "' BETWEEN convert(date, svc_del_effectivedate) AND convert(date, svc_del_enddate) or " +
                        " '" + enddate + "' BETWEEN convert(date, svc_del_effectivedate) AND convert(date, svc_del_enddate)) or" +
                        " (convert(date, svc_del_effectivedate) BETWEEN '" + effectivedate + "' AND '" + enddate + "' or " +
                        " convert(date, svc_del_enddate) BETWEEN '" + effectivedate + "' AND '" + enddate + "')) ";

                    dtOverLap = getData(sql);                   
                    if (dtOverLap.Rows.Count == 0)
                    {
                        foreach (DataRow row in dtEmp.Rows)
                        {
                            EmpModel user = new EmpModel();
                            if (users.userid > 0)
                            {                                
                               sql = "SELECT userid FROM [cred].[EmplServiceDeliveryHistorical] " +
                                                " WHERE employee = '" + presentUser.employee + "' and svc_delivery_amount ='" + presentUser.svc_delivery_amount + "' and " +
                                                " convert(date, svc_del_effectivedate) = '" + presentUser.svc_del_effectivedate  + "' and convert(date, svc_del_enddate) ='" + presentUser.svc_del_enddate + "' and program_desc ='" + row["Program"].ToString() + "'";
                                dtGetEmpRowID = getData(sql);
                                if (dtGetEmpRowID.Rows.Count > 0)
                                {
                                    user.userid = Convert.ToInt32(dtGetEmpRowID.Rows[0]["userid"].ToString());
                                }
                                else
                                {
                                    user.userid = 0;
                                }
                            }                                
                            user.EmpName = row["EmpName"].ToString();
                            user.EmpExtID = Convert.ToInt32(row["EmpExtID"].ToString());
                            user.Amount = users.svc_delivery_amount.ToString();
                            user.Effectivedate  = effectivedate.ToString();
                            user.Enddate = enddate.ToString();
                            user.Program = row["Program"].ToString();
                            user.ServiceLine = row["ServiceLine"].ToString();
                            user.EmpStatus = row["EmpStatus"].ToString();
                            userlist.Add(user);
                            users.errorStr = "";
                        }
                    }
                    else
                    {
                        users.errorStr = "There is an overlap please check !";
                    }

                }
                else
                {
                    users.errorStr = "Employee \""+ users.employee+"\" does not match with Credible!";
                }
                return userlist;
            }
            catch (Exception e)
            {
                if (e.Source != null)
                    Console.WriteLine("source: {0}", e.Source);
                throw;
            }
        }
    }
}