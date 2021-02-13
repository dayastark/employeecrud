using kendoCrudMvc.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace EmployeeCrud.Models
{
    public class DataAccessLayer
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["EmployeeContext"].ConnectionString;

        private SqlConnection connection;
        private SqlCommand sqlCommand;

        public List<User> GetViewUsers()
        {
            try
            {
                var users = new List<User>();

                connection = new SqlConnection(connectionString);
                connection.Open();

                string sql = "select first_name, last_name from Employee";
                sqlCommand = new SqlCommand(sql, connection);

                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();

                while (sqlDataReader.Read())
                {
                    var user = new User
                    {
                        FirstName = sqlDataReader["first_name"].ToString(),
                        LastName = sqlDataReader["last_name"].ToString()
                    };
                    users.Add(user);
                }
                sqlDataReader.Close();
                connection.Close();
                return users;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public Dictionary<int, string> GetEmployeeList()
        {
            try
            {
                var users = new Dictionary<int, string>();
                connection = new SqlConnection(connectionString);
                connection.Open();

                var sql = "select distinct emp_id, EmpName from View_EmployeePrimaryTeam;";

                sqlCommand = new SqlCommand(sql, connection);

                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();

                while (sqlDataReader.Read())
                {
                    int UserId = Convert.ToInt32(sqlDataReader["emp_id"].ToString());
                    if (!users.ContainsKey(UserId))
                        users.Add(UserId, sqlDataReader["EmpName"].ToString());
                }
                sqlDataReader.Close();
                connection.Close();
                return users;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public List<UserViewModel> GetUsers()
        {
            try
            {
                var users = new List<UserViewModel>();

                connection = new SqlConnection(connectionString);
                connection.Open();

                string sql = "SELECT * FROM [EmpServiceDeliveryHistorical] order by employee";

                sqlCommand = new SqlCommand(sql, connection);

                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();

                while (sqlDataReader.Read())
                {
                    try
                    {
                        var user = new UserViewModel()
                        {
                            UserId = Convert.ToInt32(sqlDataReader["id"].ToString()),
                            Employee = sqlDataReader["employee"].ToString(),
                            ExternalId = sqlDataReader["Empl_external_id"].ToString(),
                            DeliveryAmount = sqlDataReader["svc_delivery_amount"].ToString(),
                            EffectiveDate = sqlDataReader["svc_del_effectivedate"].ToString(),
                            EndDate = sqlDataReader["svc_del_enddate"].ToString(),
                            Description = sqlDataReader["program_desc"].ToString(),
                            Group = sqlDataReader["program_group"].ToString(),
                            Status = sqlDataReader["emp_status"].ToString()
                        };
                        users.Add(user);
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
                sqlDataReader.Close();
                connection.Close();
                return users;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public UserViewModel GetUserById(int Id)
        {
            connection = new SqlConnection(connectionString);
            connection.Open();

            var userViewModel = new UserViewModel();
            var sql = new StringBuilder();

            sql.Append("SELECT DISTINCT dbo.View_EmployeePrimaryTeam.emp_id,  EmpServiceDeliveryHistorical.*");
            sql.Append(" FROM  EmpServiceDeliveryHistorical INNER JOIN");
            sql.Append(" dbo.View_EmployeePrimaryTeam ON  EmpServiceDeliveryHistorical.employee = dbo.View_EmployeePrimaryTeam.EmpName");
            sql.Append($" where id = {Id}");


            sqlCommand = new SqlCommand(sql.ToString(), connection);

            SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();

            while (sqlDataReader.Read())
            {
                try
                {
                    userViewModel = new UserViewModel()
                    {
                        UserId = Convert.ToInt32(sqlDataReader["id"].ToString()),
                        Employee = sqlDataReader["emp_id"].ToString(),
                        ExternalId = sqlDataReader["Empl_external_id"].ToString(),
                        DeliveryAmount = sqlDataReader["svc_delivery_amount"].ToString(),
                        EffectiveDate = sqlDataReader["svc_del_effectivedate"].ToString(),
                        EndDate =sqlDataReader["svc_del_enddate"].ToString(),
                        Description = sqlDataReader["program_desc"].ToString(),
                        Group = sqlDataReader["program_group"].ToString(),
                        Status = sqlDataReader["emp_status"].ToString()
                    };
                }
                catch (Exception ex)
                {
                    continue;
                }
            }
            sqlDataReader.Close();
            connection.Close();
            return userViewModel;
        }

        public List<EmployeeViewDetail> GetEmployeeViewModels(string EmpId)
        {
            connection = new SqlConnection(connectionString);
            connection.Open();

            var employeeDetails = new List<EmployeeViewDetail>();
            var sql = new StringBuilder();
            sql.Append("SELECT [ServiceLine], [EmpExtID] ,[EmpStatus], [EmpName],");
            sql.Append("[Program] FROM [Demo].[dbo].[View_EmployeePrimaryTeam]");
            sql.Append($" where emp_id = '{EmpId}'");

            sqlCommand = new SqlCommand(sql.ToString(), connection);

            SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();

            while (sqlDataReader.Read())
            {
                try
                {
                    var employeeDetail = new EmployeeViewDetail()
                    {
                        ServiceLine = sqlDataReader["ServiceLine"].ToString(),
                        Status = sqlDataReader["EmpStatus"].ToString(),
                        EmpId = EmpId,
                        ExternalId = sqlDataReader["EmpExtID"].ToString(),
                        EmployeeName = sqlDataReader["EmpName"].ToString(),
                        Program = sqlDataReader["Program"].ToString(),
                    };
                    employeeDetails.Add(employeeDetail);
                }
                catch (Exception ex)
                {
                    continue;
                }
            }
            sqlDataReader.Close();
            connection.Close();
            return employeeDetails;
        }

        private List<string> CheckOverlap(UserViewModel model)
        {
            var overLapping = new List<string>();

            var query = new StringBuilder();
            query.Append("SELECT count(*) FROM EmpServiceDeliveryHistorical ");
            query.Append($" WHERE employee='{model.Employee}' and  svc_delivery_amount = '{model.DeliveryAmount}' and");
            query.Append($" (( '{model.EffectiveDate}' BETWEEN convert(date, '{model.EffectiveDate}')");
            query.Append($" and convert(date, '{model.EndDate}') or");
            query.Append($" '{model.EndDate}' BETWEEN convert(date, '{model.EffectiveDate}')");
            query.Append($" AND convert(date, '{model.EndDate}')) or");
            query.Append($" (convert(date, '{model.EffectiveDate}') BETWEEN ");
            query.Append($" '{model.EffectiveDate}' AND '{model.EndDate}' or");
            query.Append($" convert(date, '{model.EndDate}') BETWEEN ");
            query.Append($" '{model.EffectiveDate}' AND '{model.EndDate}'))");

            connection = new SqlConnection(connectionString);
            connection.Open();

            sqlCommand = new SqlCommand(query.ToString(), connection);

            SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();

            while (sqlDataReader.Read())
            {
                try
                {
                    overLapping.Add(sqlDataReader["id"].ToString());
                }
                catch (Exception ex)
                {
                    continue;
                }
            }
            sqlDataReader.Close();
            connection.Close();
            return overLapping;
        }

        public string CreateUser(UserViewModel model)
        {
            string userName = HttpContext.Current.Session["User"] as string;
            string result;
            try
            {
                var users = new List<UserViewModel>();
                var query = new StringBuilder();

                if(model.UserId > 0)
                {
                    query.AppendLine("UPDATE EmpServiceDeliveryHistorical ");

                    query.AppendLine(" SET");
                    query.AppendLine($" employee = {model.Employee}");
                    query.AppendLine($" Empl_external_id = {model.ExternalId}");
                    query.AppendLine($" svc_delivery_amount = {model.DeliveryAmount}");
                    query.AppendLine($" svc_del_effectivedate = {model.EffectiveDate}");
                    query.AppendLine($" program_desc = {model.Description}");
                    query.AppendLine($" program_group = {model.Group}");
                    query.AppendLine($" emp_status = {model.Status}");
                    query.AppendLine($" svc_del_enddate = {model.EndDate}");

                    query.AppendLine($" WHERE id = {model.UserId}");

                    connection = new SqlConnection(connectionString);
                    connection.Open();
                    sqlCommand = new SqlCommand(query.ToString(), connection);

                    int updated = sqlCommand.ExecuteNonQuery();

                    if (updated > 0)
                        result = "Successfully Updated Record";
                    else
                        result = "Something went wrong";
                    connection.Close();
                }

                var overLapping = CheckOverlap(model);

                if (overLapping.Count == 0)
                {
                    var employeeViewModels = GetEmployeeViewModels(model.Employee);

                    var newUserViewModels = new List<UserViewModel>();

                    foreach (var employee in employeeViewModels)
                    {
                        var userViewModel = new UserViewModel()
                        {
                            DeliveryAmount = model.DeliveryAmount,
                            EndDate = model.EndDate,
                            UserId = model.UserId,
                            EffectiveDate = model.EffectiveDate,
                            Status = employee.Status,
                            ExternalId = employee.ExternalId,
                            Group = employee.ServiceLine,
                            Description = employee.Program,
                            Employee = employee.EmployeeName
                        };
                        newUserViewModels.Add(userViewModel);
                    }

                    foreach (var viewModel in newUserViewModels)
                    {
                        query.AppendLine("Insert into EmpServiceDeliveryHistorical ");
                        string[] columns = new string[] {
                        "employee", "Empl_external_id", "svc_delivery_amount","svc_del_effectivedate",
                        "program_desc" , "program_group", "emp_status" , "svc_del_enddate" };

                        query.Append("(");
                        for (int i = 0; i < columns.Length; i++)
                            if (i == 0)
                                query.Append($"\"{columns[i]}\"");
                            else
                                query.Append($", \"{columns[i]}\"");
                        query.Append(") values ");

                        query.Append($"('{viewModel.Employee}','{viewModel.ExternalId}','{viewModel.DeliveryAmount}','{viewModel.EffectiveDate}',");
                        query.Append($"'{viewModel.Description}','{viewModel.Group}','{viewModel.Status}','{viewModel.EndDate}')");
                        query.Append("\n");
                        query.Append("\n");

                        LogError($"{userName} Created: {Environment.NewLine} {viewModel.Employee}| {viewModel.DeliveryAmount} | {viewModel.EffectiveDate} | {viewModel.EndDate}");
                    }

                    connection = new SqlConnection(connectionString);
                    connection.Open();
                    sqlCommand = new SqlCommand(query.ToString(), connection);

                    int added = sqlCommand.ExecuteNonQuery();
                    if (added > 0)
                        result = "Successfully Added Record";
                    else
                        result = "Something went wrong";
                    connection.Close();
                }
                else
                {
                    result = "Following Id are overlapped: " + string.Join(", ", overLapping);
                }
            }
            catch (Exception e)
            {
                result = e.Message;
            }
            return result;
        }

        public bool DeleteUser(int id, string employeeName)
        {
            string userName = HttpContext.Current.Session["User"] as string;
            
            var users = new List<UserViewModel>();
            connection = new SqlConnection(connectionString);
            connection.Open();

            string sql = $"SELECT * FROM [EmpServiceDeliveryHistorical] where employee = '{employeeName}' order by employee";

            sqlCommand = new SqlCommand(sql, connection);

            SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();

            while (sqlDataReader.Read())
            {
                try
                {
                    var user = new UserViewModel()
                    {
                        UserId = Convert.ToInt32(sqlDataReader["id"].ToString()),
                        Employee = sqlDataReader["employee"].ToString(),
                        ExternalId = sqlDataReader["Empl_external_id"].ToString(),
                        DeliveryAmount = sqlDataReader["svc_delivery_amount"].ToString(),
                        EffectiveDate = sqlDataReader["svc_del_effectivedate"].ToString(),
                        EndDate = sqlDataReader["svc_del_enddate"].ToString(),
                        Description = sqlDataReader["program_desc"].ToString(),
                        Group = sqlDataReader["program_group"].ToString(),
                        Status = sqlDataReader["emp_status"].ToString()
                    };
                    users.Add(user);
                }
                catch (Exception)
                {
                    continue;
                }
            }
            sqlDataReader.Close();
            connection.Close();

            var deleteQuery = new StringBuilder();

            var newUserList = new List<UserViewModel>();

            foreach (var user in users.GroupBy(m => new { m.DeliveryAmount, m.EffectiveDate, m.EndDate }))
                if(user.Any(m => m.UserId == id))
                    foreach (var each in user)
                        newUserList.Add(each);

            foreach (var user in newUserList)
            {
                deleteQuery.Append($"Delete from EmpServiceDeliveryHistorical Where id='{user.UserId}'  ");
                LogError($"Username : {userName} Deleted : {Environment.NewLine + user.Employee} | {user.DeliveryAmount} | {user.EffectiveDate} | {user.EndDate} ");
            }

            if (!string.IsNullOrEmpty(deleteQuery.ToString()))
            {
                connection = new SqlConnection(connectionString);
                connection.Open();
                sqlCommand = new SqlCommand(deleteQuery.ToString(), connection);
                sqlCommand.ExecuteNonQuery();
                sqlDataReader.Close();
                connection.Close();
            }
            
            return true;
        }

        private void LogError(string inputMessage)
        {
            string message = string.Format("{0}", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"));
            message += Environment.NewLine;
            message += "-----------------------------------------------------------";
            message += Environment.NewLine;
            message += string.Format("{0}", inputMessage);
            message += Environment.NewLine;
            message += "-----------------------------------------------------------";
            message += Environment.NewLine;

            string path = ConfigurationManager.AppSettings["LISpath"].ToString();
            string relativepath =  HttpContext.Current.Server.MapPath(path);
            using (StreamWriter writer = new StreamWriter(relativepath, true))
            {
                writer.WriteLine(message);
                writer.Close();
            }
        }
    }
}