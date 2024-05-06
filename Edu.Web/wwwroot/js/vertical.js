var verticalBaseUrl = `${$('#websiteBaseUrl').val()}/Vertical`;

$(document).ready(function () {
    $("#verticalDatatable").DataTable({
        "processing": true,
        "serverSide": true,
        "filter": true,
        "ajax": {
            "url": `${verticalBaseUrl}/GetVerticals`,
            "type": "POST",
            "datatype": "json"
        },
        "columnDefs": [{
            "targets": 'no-sort',
            "orderable": false,
        }],
        "columns": [
            {
                "data": "verticalName", "name": "verticalName",
                render: function (row, type, data) {
                    return `<a href='./Vertical/Manage/${data.verticalId}'>${data.verticalName}</a>`;
                }
            },
        ]
    });
});

function validateVertical() {
    if ($("#verticalForm").valid()) {
        showLoading();
        return true;
    }
    return false;
}
