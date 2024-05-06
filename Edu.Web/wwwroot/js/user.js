var userBaseUrl = `${$('#websiteBaseUrl').val()}/User`;

$(document).ready(function () {
    $("#userDatatable").DataTable({
        "processing": true,
        "serverSide": true,
        "filter": true,
        "ajax": {
            "url": `${userBaseUrl}/GetUsers`,
            "type": "POST",
            "datatype": "json"
        },
        "columnDefs": [{
            "targets": 'no-sort',
            "orderable": false,
        }],
        "columns": [
            {
                "data": "msilUserId", "name": "msilUserId",
                render: function (row, type, data) {
                    return `<a href='./User/Manage/${data.userId}'>${data.msilUserId}</a>`;
                },
            },
            { "data": "firstName", "name": "firstName", "autoWidth": true },
            { "data": "lastName", "name": "lastName", "autoWidth": true },
            { "data": "email", "name": "email", "autoWidth": true },
            { "data": "mobileNo", "name": "mobileNo", "autoWidth": true },
        ]
    });

    if (+$("#UserId").val() == 0) {
        $("#MsilUserId").val('');
    }
});

function validateUser() {
    if ($("#userForm").valid()) {
        showLoading();
        return true;
    }
    return false;
}
