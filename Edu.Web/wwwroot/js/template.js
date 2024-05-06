var templateBaseUrl = `${$('#websiteBaseUrl').val()}/Template`;

$(document).ready(function () {
    $("#templateDatatable").DataTable({
        "processing": true,
        "serverSide": true,
        "filter": false,
        "bLengthChange": false,
        "bPaginate": true,
        "scrollX": true,
        "bInfo": false,
        "fnDrawCallback": function (oSettings) {
        },
        "dom": 'Bfrtip',
        "buttons": [
            {
                extend: 'excel',
                customize: function (xlsx) {
                    $('row c', xlsx.xl.worksheets['sheet1.xml']).attr('s', '55');
                }
            },
        ],
        "ajax": {
            "url": `${templateBaseUrl}/GetTemplates`,
            "type": "POST",
            "datatype": "json",
            "data": function (d) { return getFilterParams(d); }
        },
        "columnDefs": [{
            "targets": 'no-sort',
            "orderable": false,
        }],
        "columns": [
            {
                "data": "intakeTemplateId", "name": "intakeTemplateId",
                render: function (row, type, data, meta) {
                    return meta.row + 1;
                },
            },
            { "data": "intakeName", "name": "intakeName", "autoWidth": true },
            {
                "data": "templateName", "name": "templateName",
                render: function (row, type, data) {
                    return `<a href='./Template/Manage/${data.intakeTemplateId}'>${data.templateName}</a>`;
                },
            },
            { "data": "templateSubject", "name": "templateSubject", "autoWidth": true },
        ]
    });

    $('#TemplateContent').summernote()
});


function getSchemeIntakes(obj) {
    $('#filterIntakeId').html($(`<option></option>`).val('').html('All'));
    var schemeId = $(obj).val();
    showLoading();
    $.ajax({
        url: `${templateBaseUrl}/GetSchemeIntakes`,
        type: 'POST',
        data: { schemeId },
        success: function (result) {
            if (result) {
                $.each(result, function (index, value) {
                    $('#filterIntakeId').append($(`<option></option>`).val(value.id).html(value.text));
                });
                filterTable();
            }
            hideLoading();
        },
        error: function (error) {
            displayToastr(error.data, "Intake Detail", "Error")
            hideLoading();
        },
    });
}

function filterTable() {
    $(`#templateDatatable`).DataTable().ajax.reload();
}


function validateTemplate() {
    if ($("#templateForm").valid()) {
        showLoading();
        return true;
    }
    return false;
}

function getFilterParams(d) {
    d.schemeId = $('#filterSchemeId').val();
    d.intakeId = $('#filterIntakeId').val();
}
