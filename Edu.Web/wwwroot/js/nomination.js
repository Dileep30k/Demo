var nominationBaseUrl = `${$('#websiteBaseUrl').val()}/Nomination`;
var nominationForm = null;

$(document).ready(function () {

    $("#nominationDatatable").DataTable({
        "processing": true,
        "serverSide": true,
        "filter": false,
        "bLengthChange": false,
        "bPaginate": true,
        "bInfo": false,
        "scrollX": true,
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
            "url": `${nominationBaseUrl}/GetNominations`,
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
                "data": "nominationStatusId", "name": "nominationStatusId",
                render: function (row, type, data) {
                    return `<span class="dot tooltipeds" alt="${getNominationStatusText(data.nominationStatusId)}" style="background-color: ${getNominationStatusColor(data.nominationStatusId)}">${getNominationStatusText(data.nominationStatusId)} <div> <div class="tooltiptexteds">${getNominationStatusTextHover(data.nominationStatusId)}</div></div> </span>`; 
                }
            },
            {
                render: function (row, type, data) {
                    return data.instituteNames.map(i => i).join('<br/>\r');
                }
            },
            { "data": "vertical", "name": "vertical", "autoWidth": true },
            { "data": "msilUserId", "name": "msilUserId", "autoWidth": true },
            { "data": "staffName", "name": "staffName", "autoWidth": true },
            { "data": "staffEmail", "name": "staffEmail", "autoWidth": true },
            { "data": "mobileNo", "name": "mobileNo", "autoWidth": true },
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
            { "data": "division", "name": "division", "autoWidth": true },
            { "data": "department", "name": "department", "autoWidth": true },
            { "data": "grade", "name": "grade", "autoWidth": true },
            { "data": "approver1", "name": "approver1", "autoWidth": true },
            { "data": "approver1StaffId", "name": "approver1StaffId", "autoWidth": true },
            { "data": "approver2", "name": "approver2", "autoWidth": true },
            { "data": "approver2StaffId", "name": "approver2StaffId", "autoWidth": true },
            {
                render: function (row, type, data) {
                    return data.remarks.map(i => i).join('<br/>\r');
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
    $('#intakeId').html($(`<option></option>`).val('').html('All'));
    $('#instituteId').html($(`<option></option>`).val('').html('All'));
    var schemeId = $(obj).val();
    showLoading();
    $.ajax({
        url: `${nominationBaseUrl}/GetSchemeIntakes`,
        type: 'POST',
        data: { schemeId },
        success: function (result) {
            $.each(result, function (index, value) {
                $('#intakeId').append($(`<option></option>`).val(value.id).html(value.text));
            });
            filterTable();
            hideLoading();
        },
        error: function (error) {
            displayToastr(error.data, "Intake Detail", "Error");
            hideLoading();
        },
    });
}

function getIntakeInstitutes(obj) {
    $('#instituteId').html($(`<option></option>`).val('').html('All'));
    var intakeId = $(obj).val();
    showLoading();
    $.ajax({
        url: `${nominationBaseUrl}/GetIntakeInstitutes`,
        type: 'POST',
        data: { intakeId },
        success: function (result) {
            $.each(result, function (index, value) {
                $('#instituteId').append($(`<option></option>`).val(value.id).html(value.text));
            });
            filterTable();
            hideLoading();
        },
        error: function (error) {
            displayToastr(error.data, "Institute Detail", "Error");
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
    d.departmentId = $('#departmentId').val();
    d.instituteId = $('#instituteId').val();
    d.nominationStatusId = $('#nominationStatusId').val();
}

function clearFilters() {
    $('#searchValue').val('');
    $('#startDate').val('');
    $('#endDate').val('');
    $('#designationId').val('').trigger('change');
    $('#departmentId').val('').trigger('change');
    $('#instituteId').val('').trigger('change');
    $('#nominationStatusId').val('').trigger('change');
}

function filterTable() {
    $(`#nominationDatatable`).DataTable().ajax.reload();
}

