var nominationBaseUrl = `${$('#websiteBaseUrl').val()}/Nomination`;
var selectedNominations = [];

$(document).ready(function () {

    $("#nominationDatatable").DataTable({
        "processing": true,
        "serverSide": true,
        "filter": false,
        "bLengthChange": false,
        "bPaginate": true,
        "bInfo": true,
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
            "url": `${nominationBaseUrl}/GetNominationApprover`,
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
                render: function (row, type, data) {
                    return `<input id="chk-nom-${data.nominationId}" type="checkbox" class="chk-nom" value="${data.nominationId}" onclick="selectCheckbox(this);" />`;
                }
            },
            { "data": "msilUserId", "name": "msilUserId", "autoWidth": true },
            { "data": "staffName", "name": "staffName", "autoWidth": true },
            { "data": "schemeName", "name": "schemeName", "autoWidth": true },
            {
                render: function (row, type, data) {
                    return data.instituteNames.map(i => i).join('<br/>\r');
                }
            },
            { "data": "staffEmail", "name": "staffEmail", "autoWidth": true },
            { "data": "mobileNo", "name": "mobileNo", "autoWidth": true },
            {
                "data": "doj", "name": "doj",
                render: function (row, type, data) {
                    return getFormatDate(data.doj);
                }
            },
            { "data": "msilTenure", "name": "msilTenure", "autoWidth": true },
            { "data": "department", "name": "department", "autoWidth": true },
            { "data": "division", "name": "division", "autoWidth": true },
            
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

function selectAll(obj) {
    selectedNominations = [];
    $(".chk-nom").each(function () {
        var nominamtionId = +$(this).val();
        if ($(obj).is(':checked')) {
            selectedNominations.push(nominamtionId);
        }
        $(this).prop('checked', $(obj).is(':checked'));
    });
}

function selectCheckbox(obj) {
    var nominamtionId = +$(obj).val();
    if ($(obj).is(':checked')) {
        selectedNominations.push(nominamtionId);
    } else {
        selectedNominations = selectedNominations.filter(n => n != nominamtionId);
    }
    $('.select-all').prop('checked', $('.chk-nom').length == selectedNominations.length);
}

function takeAction(id) {
    var ele = $(`#chk-nom-${id}`);
    ele.prop('checked', !selectedNominations.some(n => n == id));
    selectCheckbox(ele);
}

function getSchemeIntakes(obj) {
    $('#intakeId').html('');
    var schemeId = $(obj).val();
    if (schemeId == '') {
        $('#intakeId').append($(`<option></option>`).val('').html('N/A'));
        return;
    }
    showLoading();
    $.ajax({
        url: `${nominationBaseUrl}/GetSchemeIntakes`,
        type: 'POST',
        data: { schemeId },
        success: function (result) {
            if (result.length == 0) {
                $('#intakeId').append($(`<option></option>`).val('').html('N/A'));
                filterTable();
            } else {
                $.each(result, function (index, value) {
                    $('#intakeId').append($(`<option ${index == 0 ? 'selected' : ''}></option>`).val
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
    d.divisionId = $('#divisionId').val();
    d.departmentId = $('#departmentId').val();
    d.instituteId = $('#instituteId').val();
}

function clearFilters() {
    $('#searchValue').val('');
    $('#startDate').val('');
    $('#endDate').val('');
    $('#divisionId').val('').trigger('change');
    $('#departmentId').val('').trigger('change');
    $('#instituteId').val('').trigger('change');
    $('#chk-all').prop('checked', false);
}

function filterTable() {
    $(`#nominationDatatable`).DataTable().ajax.reload();
    selectedNominations = [];
    $('#chk-all').prop('checked', false);
}

function viewModal(modal) {
    if (selectedNominations.length == 0) {
        displayToastr("Please select Nomination", "Nomination", "Error");
        return;
    }
    displayModel(modal);
}

function updateNomination(type, modal) {

    if ((type == 2 && $("#remarkReview").val() == '') ||
        (type == 3 && $("#remarkReject").val() == '')) {
        displayToastr("Please enter Remarks", "Nomination", "Error");
        return;
    }

    var status = 'Approved';

    var remarks = '';
    if (type == 2) {
        remarks = $("#remarkReview").val();
        status = 'Reviewed';
    }
    if (type == 3) {
        remarks = $("#remarkReject").val();
        status = 'Rejected';
    }

    showLoading();
    $.ajax({
        url: `${nominationBaseUrl}/updateNomination`,
        type: 'POST',
        data: { ids: selectedNominations, type, remarks },
        success: function (result) {
            if (result.success) {
                hideModel(modal);
                displayToastr(`Nomination ${status}`, "Nomination", "Success");
                filterTable();
            } else {
                displayToastr(`Nomination not ${status}`, "Nomination", "Error");
            }
            hideLoading();
        },
        error: function (error) {
            displayToastr("Nomination not updated", "Nomination", "Error");
            hideLoading();
        },
    });
}

function onApprove() {
    viewModal('approvalModal');
}

function onReview() {
    $('#remarkReview').val('');
    viewModal('reviewModal');
}

function onReject() {
    $('#remarkReject').val('');
    viewModal('rejectModal');
}