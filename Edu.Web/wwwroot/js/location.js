var locationBaseUrl = `${$('#websiteBaseUrl').val()}/Location`;

$(document).ready(function () {
    $("#locationDatatable").DataTable({
        "processing": true,
        "serverSide": true,
        "filter": true,
        "ajax": {
            "url": `${locationBaseUrl}/GetLocations`,
            "type": "POST",
            "datatype": "json"
        },
        "columnDefs": [{
            "targets": 'no-sort',
            "orderable": false,
        }],
        "columns": [
            {
                "data": "locationName", "name": "locationName",
                render: function (row, type, data) {
                    return `<a href='./Location/Manage/${data.locationId}'>${data.locationName}</a>`;
                }
            },
        ]
    });
});

function validateLocation() {
    if ($("#locationForm").valid()) {
        showLoading();
        return true;
    }
    return false;
}
