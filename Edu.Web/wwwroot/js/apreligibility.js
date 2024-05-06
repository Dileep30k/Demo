var eligibilityBaseUrl = `${$('#websiteBaseUrl').val()}/Eligibility`;
var selectedFile = null;

$(document).ready(function () {
    $("#eligibilityDatatable").DataTable({
        "processing": true,
        "serverSide": true,
        "filter": false,
        "bLengthChange": false,
        "bPaginate": true,
        "scrollX": true,
        "bInfo": false,
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
            "url": `${eligibilityBaseUrl}/GetEligibilities`,
            "type": "POST",
            "datatype": "json",
            "data": function (d) { return getFilterParams(d); }
        },
        "columnDefs": [{
            "targets": 'no-sort',
            "orderable": false,
        }],
        "columns": [
            { "data": "vertical", "name": "vertical", "autoWidth": true },
            { "data": "msilUserId", "name": "msilUserId", "autoWidth": true },
            { "data": "staffName", "name": "staffName", "autoWidth": true },
            {
                "data": "intakeStartDate", "name": "intakeStartDate",
                render: function (row, type, data) {
                    return getFormatDate(data.intakeStartDate);
                }
            },
            {
                "data": "doj", "name": "doj",
                render: function (row, type, data) {
                    return getFormatDate(data.doj);
                }
            },
            { "data": "msilTenure", "name": "msilTenure", "autoWidth": true },
            { "data": "designation", "name": "designation", "autoWidth": true },
            { "data": "relevantPrevExp", "name": "relevantPrevExp", "autoWidth": true },
            { "data": "division", "name": "division", "autoWidth": true },
            { "data": "department", "name": "department", "autoWidth": true },
            { "data": "location", "name": "location", "autoWidth": true },
            {
                "data": "isPublish", "name": "isPublish",
                render: function (row, type, data) {
                    return data.isPublish ? 'Yes' : 'No';
                }
            },
        ]
    });

    $("#btnFilter").click(function (e) {
        e.preventDefault();
        filterTable();
    });

    $("#btnFilterClear").click(function (e) {
        e.preventDefault();
        clearFilters();
        filterTable();
    });
    $("#searchValue").blur(function () {
        filterTable();
    });
});
function getSchemeIntakes(obj) {
    $('#intakeId').html('');
    var schemeId = $(obj).val();
    showLoading();
    $.ajax({
        url: `${eligibilityBaseUrl}/GetSchemeIntakes`,
        type: 'POST',
        data: { schemeId },
        success: function (result) {
            if (result) {
                $('#intakeId').append($(`<option></option>`).val('').html('All'));
                $.each(result, function (index, value) {
                    $('#intakeId').append($(`<option></option>`).val
                        (value.id).html(value.text));
                });
                filterTable();
            }
            hideLoading();
        },
        error: function (error) {
            displayToastr(error.data, "Intake Detail", "Error");
            hideLoading();
        },
    });
}

function getFilterParams(d) {
    d.searchValue = $('#searchValue').val();
    d.startDate = $('#startDate').val();
    d.endDate = $('#endDate').val();
    d.schemeId = $('#schemeId').val();
    d.intakeId = $('#intakeId').val();
    d.designationId = $('#designationId').val();
    d.divisionId = $('#divisionId').val();
    d.departmentId = $('#departmentId').val();
    d.locationId = $('#locationId').val();
}

function clearFilters() {
    $('#searchValue').val('');
    $('#startDate').val('');
    $('#endDate').val('');
    $('#designationId').val('').trigger('change');
    $('#divisionId').val('').trigger('change');
    $('#departmentId').val('').trigger('change');
    $('#locationId').val('').trigger('change');
}

function filterTable() {
    $('#uploadError').html('');
    $(`#eligibilityDatatable`).DataTable().ajax.reload();
}
