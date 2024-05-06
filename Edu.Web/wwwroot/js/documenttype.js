var documentTypeBaseUrl = `${$('#websiteBaseUrl').val()}/DocumentType`;

$(document).ready(function () {
    $("#documentTypeDatatable").DataTable({
        "processing": true,
        "serverSide": true,
        "filter": true,
        "ajax": {
            "url": `${documentTypeBaseUrl}/GetDocumentTypes`,
            "type": "POST",
            "datatype": "json"
        },
        "columnDefs": [{
            "targets": 'no-sort',
            "orderable": false,
        }],
        "columns": [
            {
                "data": "documentTypeName", "name": "documentTypeName",
                render: function (row, type, data) {
                    return `<a href='./DocumentType/Manage/${data.documentTypeId}'>${data.documentTypeName}</a>`;
                }
            },
        ]
    });
});

function validateDocumentType() {
    if ($("#documentTypeForm").valid()) {
        showLoading();
        return true;
    }
    return false;
}
