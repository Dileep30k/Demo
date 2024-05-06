var designationBaseUrl = `${$('#websiteBaseUrl').val()}/Designation`;

$(document).ready(function () {
    $("#designationDatatable").DataTable({
        "processing": true,
        "serverSide": true,
        "filter": true,
        "ajax": {
            "url": `${designationBaseUrl}/GetDesignations`,
            "type": "POST",
            "datatype": "json"
        },
        "columnDefs": [{
            "targets": 'no-sort',
            "orderable": false,
        }],
        "columns": [
            {
                "data": "designationName", "name": "designationName",
                render: function (row, type, data) {
                    return `<a href='./Designation/Manage/${data.designationId}'>${data.designationName}</a>`;
                }
            },
        ]
    });
});

function validateDesignation() {
    if ($("#designationForm").valid()) {
        showLoading();
        return true;
    }
    return false;
}
