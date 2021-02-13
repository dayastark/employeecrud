$(function () {
    $('#employee-table').DataTable();
    $(".datepicker").datepicker();
    var date = $(".datepicker").val();
    $(".datepicker").val(date.replace(" 00:00:00", ""));
});

function isNumberKey(evt) {
    var charCode = (evt.which) ? evt.which : evt.keyCode
    if (charCode > 31 && (charCode < 48 || charCode > 57))
        return false;
    return true;
}



function changeUser(ele)
{
    var selectUser = $(ele).val();
    
    $.ajax({
        method: 'get',
        url: '/home/changeuser/' + selectUser,
        success: function (e) {
            if (e.status) {
                sessionStorage.setItem('user', e.message);
                window.location.href = "/employee";
            } else {
                sessionStorage.clear();
            }
        },
        error: function (e) {
            console.log(e);
            sessionStorage.clear();
        }
    })
}