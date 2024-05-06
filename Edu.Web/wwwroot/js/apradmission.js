var admissionBaseUrl = `${$('#websiteBaseUrl').val()}/Admission`;

var admission = null;

$(document).ready(function () {
    $("#admissionDatatable").DataTable({
        "processing": true,
        "serverSide": true,
        "filter": false,
        "bLengthChange": false,
        "bPaginate": true,
        "scrollX": true,
        "deferLoading": 0,
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
            "url": `${admissionBaseUrl}/GetAdmissions`,
            "type": "POST",
            "datatype": "json",
            "data": function (d) { return getFilterParams(d); }
        },
        "columnDefs": [{
            "targets": 'no-sort',
            "orderable": false,
        }],
        "columns": [
            { "data": "rank", "name": "rank", "autoWidth": true },
            { "data": "msilUserId", "name": "msilUserId", "autoWidth": true },
            { "data": "staffName", "name": "staffName", "autoWidth": true },
            { "data": "email", "name": "email", "autoWidth": true },
            { "data": "mobileNo", "name": "mobileNo", "autoWidth": true },
        ],
        "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
            if (aData.admissionStatusId == admissionStatuses.Waiting) {
                $(nRow).addClass('row-waiting');
            }
        }
    });
    $('#uploadTitle').html(`No data found.`);
});

function getSchemeIntakes(obj) {
    $('.grid-content').hide();
    $('#intakeId').html('');
    $('#instituteId').html('');
    $('#uploadTitle').html(`No data found.`);
    var schemeId = $(obj).val();
    if (schemeId == '') {
        $('#intakeId').append($(`<option></option>`).val('').html('N/A'));
        $('#instituteId').append($(`<option></option>`).val('').html('N/A'));
        return;
    }
    showLoading();
    $.ajax({
        url: `${admissionBaseUrl}/GetSchemeIntakes`,
        type: 'POST',
        data: { schemeId },
        success: function (result) {
            if (result.length == 0) {
                $('#intakeId').append($(`<option></option>`).val('').html('N/A'));
                $('#instituteId').append($(`<option></option>`).val('').html('N/A'));
            } else {
                $.each(result, function (index, value) {
                    $('#intakeId').append($(`<option ${index == 0 ? 'selected' : ''}></option>`).val
                        (value.id).html(value.text));
                });
                getIntakeInstitutes($('#intakeId'));
            }
            hideLoading();
        },
        error: function (error) {
            displayToastr(error.data, "Intake Detail", "Error");
            hideLoading();
        },
    });
}

function getIntakeInstitutes(obj) {
    $('.grid-content').hide();
    $('#uploadTitle').html(`No data found.`);
    $('#instituteId').html('');
    var intakeId = $(obj).val();
    if (intakeId == '') {
        $('#instituteId').append($(`<option></option>`).val('').html('N/A'));
        return;
    }
    showLoading();
    $.ajax({
        url: `${admissionBaseUrl}/GetIntakeInstitutes`,
        type: 'POST',
        data: { intakeId },
        success: function (result) {
            $('#instituteId').append($(`<option></option>`).val('').html('Select'));
            $.each(result, function (index, value) {
                $('#instituteId').append($(`<option ${index == 0 ? 'selected' : ''}></option>`).val
                    (value.id).html(value.text));
            });
            getAdmission($('#instituteId'));
            hideLoading();
        },
        error: function (error) {
            displayToastr(error.data, "Institute Detail", "Error");
            hideLoading();
        },
    });
}

function getAdmission(obj) {
    admission = null;
    $('.grid-content').hide();
    $('#uploadTitle').html(`No data found.`);
    var instituteId = $(obj).val();
    if (instituteId == '') {
        return;
    }
    showLoading();
    $.ajax({
        url: `${admissionBaseUrl}/GetAdmission`,
        type: 'POST',
        data: { schemeId: $('#schemeId').val(), intakeId: $('#intakeId').val(), instituteId: $('#instituteId').val(), isGts: false },
        success: function (result) {
            if (result.success) {
                admission = result.data;
                filterTable();
                if (admission.isApprovedByUser == null) {
                    $('.buttons-div').show();
                    $('#uploadTitle').html(`Scheme: ${$('#schemeId').find("option:selected").text()} - Financial Year: ${$('#intakeId').find("option:selected").text()} - Institue: ${$('#instituteId').find("option:selected").text()}`);
                } else {
                    $('.buttons-div').hide();
                    $('#uploadTitle').html(`Nomination List ${admission.isApprovedByUser ? 'Approved' : 'Rejected'}`);
                }
            }
            hideLoading();
        },
        error: function (error) {
            displayToastr(error.data, "Admission Detail", "Error");
            hideLoading();
        },
    });
}

function updateAdmissions(isApprove, modal) {
    var remarks = $("#remarkReject").val();
    if (!isApprove && remarks == '') {
        displayToastr("Please enter Remarks", "Admission", "Error");
        return;
    }
    showLoading();
    $.ajax({
        url: `${admissionBaseUrl}/ApproveAdmissions`,
        type: 'POST',
        data: { ...admission, approved1: isApprove, approved2: isApprove, approvalRemarks1: remarks, approvalRemarks2: remarks },
        success: function (result) {
            if (result.success) {
                displayToastr(`Nomination List ${isApprove ? 'Approved' : 'Rejected'}`, "Admission", "Success");
                hideModel(modal);
                $('.buttons-div').hide();
                $('#uploadTitle').html(`Nomination List ${isApprove ? 'Approved' : 'Rejected'}`);
            } else {
                displayToastr(`Nomination List not ${isApprove ? 'Approved' : 'Rejected'}`, "Admission", "Error");
            }
            hideLoading();
        },
        error: function (error) {
            displayToastr(`Nomination List not ${isApprove ? 'Approved' : 'Rejected'}`, "Admission", "Error");
            hideLoading();
        },
    });
}

function getFilterParams(d) {
    d.schemeId = $('#schemeId').val();
    d.intakeId = $('#intakeId').val();
    d.instituteId = $('#instituteId').val();
    d.isGts = false;
}

function filterTable() {
    $('.grid-content').show();
    $(`#admissionDatatable`).DataTable().ajax.reload();
}
