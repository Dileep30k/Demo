var departmentBaseUrl = `${$('#websiteBaseUrl').val()}/Department`;

$(document).ready(function () {
    $("#departmentDatatable").DataTable({
        "processing": true,
        "serverSide": true,
        "filter": true,
        "ajax": {
            "url": `${departmentBaseUrl}/GetDepartments`,
            "type": "POST",
            "datatype": "json"
        },
        "columnDefs": [{
            "targets": 'no-sort',
            "orderable": false,
        }],
        "columns": [
            {
                "data": "departmentName", "name": "departmentName",
                render: function (row, type, data) {
                    return `<a href='./Department/Manage/${data.departmentId}'>${data.departmentName}</a>`;
                }
            },
        ]
    });
});

function validateDepartment() {
    if ($("#departmentForm").valid()) {
        showLoading();
        return true;
    }
    return false;
}
