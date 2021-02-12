$(document).ready(function () {

    $('#employee-table').DataTable();


});


function changeUser(ele)
{
    var selectUser = $(ele).val();
    
    $.ajax({
        method: 'get',
        url: '/home/changeuser/' + selectUser,
        success: function (e) {
            if (e.status) {
                sessionStorage.setItem('user', e.message);
                window.location.href = "/empform";
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