var settingBaseUrl = `${$('#websiteBaseUrl').val()}/Setting`;

$(document).ready(function () {
    $("#settingDatatable").DataTable({
        "processing": true,
        "serverSide": true,
        "filter": true,
        "ajax": {
            "url": `${settingBaseUrl}/GetSettings`,
            "type": "POST",
            "datatype": "json"
        },
        "columnDefs": [{
            "targets": 'no-sort',
            "orderable": false,
        }],
        "columns": [
            {
                "data": "settingName", "name": "settingName",
                render: function (row, type, data) {
                    return `<a href='./Setting/Manage/${data.settingId}'>${data.settingName}</a>`;
                }
            },
            { "data": "settingValue", "name": "settingValue", "autoWidth": true },
        ]
    });
});

function validateSetting() {
    if ($("#settingForm").valid()) {
        showLoading();
        return true;
    }
    return false;
}
