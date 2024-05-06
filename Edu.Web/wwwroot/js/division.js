var divisionBaseUrl = `${$('#websiteBaseUrl').val()}/Division`;

$(document).ready(function () {
    $("#divisionDatatable").DataTable({
        "processing": true,
        "serverSide": true,
        "filter": true,
        "ajax": {
            "url": `${divisionBaseUrl}/GetDivisions`,
            "type": "POST",
            "datatype": "json"
        },
        "columnDefs": [{
            "targets": 'no-sort',
            "orderable": false,
        }],
        "columns": [
            {
                "data": "divisionName", "name": "divisionName",
                render: function (row, type, data) {
                    return `<a href='./Division/Manage/${data.divisionId}'>${data.divisionName}</a>`;
                }
            },
        ]
    });
});

function validateDivision() {
    if ($("#divisionForm").valid()) {
        showLoading();
        return true;
    }
    return false;
}
