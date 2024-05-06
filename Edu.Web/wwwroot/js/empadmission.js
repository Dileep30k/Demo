var admissionBaseUrl = `${$('#websiteBaseUrl').val()}/Admission`;
var admissionUserId = null;

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
        "fnDrawCallback": function (oSettings) {
            var admissionUser = this.fnGetData().find(a => a.isConfirmByEmp);
            if (admissionUser) {
                admissionUserId = admissionUser.admissionUserId;
                if (admissionUser.isBondAccepted) {
                    displayLegalAccepted(admissionUser.bondAcceptedDate);
                }
                else {
                    displayLegalContent();
                }
            }
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
            "url": `${admissionBaseUrl}/GetAdmissionInstitutes`,
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
                render: function (row, type, data, meta) {
                    return meta.row + 1;
                },
            },
            { "data": "instituteName", "name": "instituteName", "autoWidth": true },
            {
                render: function (row, type, data, meta) {
                    var html = '';
                    if (data.isConfirmByEmp == null) {
                        if (data.admissionStatusId == admissionStatuses.Confirm) {
                            html += `<span class="confirm-seat">Merit List</span>`;
                        }
                        else if (data.admissionStatusId == admissionStatuses.Waiting) {
                            html += `<span class="waiting-seat">Waitlisted - ${data.rank}</span>`;
                        }
                    } else {
                        if (data.isConfirmByEmp) {
                            html += `<span class="confirm-seat">Accepted</span>`;
                        } else {
                            html += `<span class="waiting-seat">Rejected</span>`;
                        }
                    }
                    return html;
                },
            },
            {
                "data": "admissionCutoffDate", "name": "admissionCutoffDate",
                render: function (row, type, data) {
                    return getFormatDate(data.admissionCutoffDate);
                }
            },
            {
                render: function (row, type, data, meta) {
                    return data.admissionStatusId == admissionStatuses.Confirm && data.isConfirmByEmp == null ? `
                    <button onclick="confirmSeat(${data.admissionUserId})" class="btn btn-success">Confirm Seat</button>&nbsp;&nbsp;
                    <button onclick="rejectAdmission(${data.admissionUserId})" class="btn btn-danger">Reject Admission</button>
                    ` : null;
                },
            },
        ],
    });
    $('#uploadTitle').html(`No data found.`);
    $('#file-input-label').click(function () { $('#uploadServiceAgreement').trigger('click'); });
    $('#file-input-bond').click(function () { $('#uploadSuretyBond').trigger('click'); });

    $("#uploadServiceAgreement").on('change', function (e) {
        if (e.target.files) {
            $('#uploadServiceAgreementError').hide();
        }
    });

    $("#uploadSuretyBond").on('change', function (e) {
        if (e.target.files) {
            $('#uploadSuretyBondError').hide();
        }
    });

});

function getSchemeIntakes(obj) {
    $('.grid-content').hide();
    $('.legal-content').hide();
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
    $('.legal-content').hide();
    $('#uploadTitle').html(`No data found.`);
    $('#instituteId').html('');
    var intakeId = $(obj).val();
    if (intakeId == '') {
        $('#instituteId').append($(`<option></option>`).val('').html('N/A'));
        return;
    }
    $('#uploadTitle').html(`Scheme: ${$('#schemeId').find("option:selected").text()} - Financial Year: ${$('#intakeId').find("option:selected").text()}`);
    filterTable();
    admissionUserId = null;
}

function getIntakeDocuments(obj) {
    $('#serviceAgreement').hide();
    $('#suretyBond').hide();
    var intakeId = $('#intakeId').val();
    showLoading();
    $.ajax({
        url: `${admissionBaseUrl}/GetIntakeDocuments`,
        type: 'POST',
        data: { intakeId },
        success: function (result) {
            if (result.find(d => d.documentTypeId == 1)) {
                $('#serviceAgreement').show();
            }
            if (result.find(d => d.documentTypeId == 2)) {
                $('#suretyBond').show();
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
    d.schemeId = $('#schemeId').val();
    d.intakeId = $('#intakeId').val();
}

function filterTable() {
    $('.grid-content').show();
    $('.legal-content').hide();
    $(`#admissionDatatable`).DataTable().ajax.reload();
}

function confirmSeat(id) {
    admissionUserId = id;
    displayModel('approvalModal');
}

function rejectAdmission(id) {
    $("#remarkReject").val('');
    admissionUserId = id;
    displayModel('rejectModal');
}

function updateAdmissionUser(isApprove, modal) {
    var remarks = $("#remarkReject").val();
    if (!isApprove && remarks == '') {
        displayToastr("Please enter Remarks", "Admission", "Error");
        return;
    }
    if (isApprove && !$("#chkTerms").is(":checked")) {
        displayToastr('Please accept the terms', "Admission Approval", "Error");
        return;
    }
    showLoading();
    $.ajax({
        url: `${admissionBaseUrl}/ApproveAdmissionUser`,
        type: 'POST',
        data: { admissionUserId, isConfirmByEmp: isApprove, employeeRemarks: remarks },
        success: function (result) {
            if (result.success) {
                displayToastr(`Admission ${isApprove ? 'Confirmed' : 'Rejected'}`, "Admission", "Success");
                hideModel(modal);
                if (isApprove) {
                    displayLegalContent();
                } else {
                    filterTable();
                }
            } else {
                displayToastr(`Admissions not ${isApprove ? 'Approved' : 'Rejected'}`, "Admission", "Error");
            }
            hideLoading();
        },
        error: function (error) {
            displayToastr(`Admission not ${isApprove ? 'Approved' : 'Rejected'}`, "Admission", "Error");
            hideLoading();
        },
    });
}

function displayLegalContent() {
    $('#uploadTitle').html('');
    $('.grid-content').hide();
    $('.legal-content').show();
    $('#schemeSelected').html(`Scheme: ${$('#schemeId').find("option:selected").text()} - Financial Year: ${$('#intakeId').find("option:selected").text()}`);
    getIntakeDocuments();
}

function displayLegalAccepted(bondAcceptedDate) {
    $('.grid-content').hide();
    $('.legal-content').hide();
    $('#uploadTitle').html(`Legal documents are uploaded on ${getFormatDate(bondAcceptedDate)}`);
}

function saveLegalDocument() {
    if (validateLegalDocument()) {
        var formData = new FormData();
        if ($("#serviceAgreement").is(":visible")) {
            formData.append('serviceAgreement', $('#uploadServiceAgreement').get(0).files[0]);
        }
        if ($("#suretyBond").is(":visible")) {
            formData.append('suretyBond', $('#uploadSuretyBond').get(0).files[0]);
        }
        formData.append('admissionUserId', admissionUserId);

        showLoading();
        $.ajax({
            url: `${admissionBaseUrl}/UpdateAdmissionUserLegal`,
            type: 'POST',
            data: formData,
            cache: false,
            contentType: false,
            processData: false,
            success: function (result) {
                if (result.success) {
                    displayToastr('Document Submitted', "Admission", "Success");
                    displayLegalAccepted(new Date());
                } else {
                    displayToastr('Document not Submitted', "Admission", "Error");
                }
                hideLoading();
            },
            error: function (error) {
                displayToastr('Document not Submitted', "Admission", "Error");
                hideLoading();
            },
        });
    }
}

function validateLegalDocument() {
    $('#uploadPolicyError').hide();
    $('#uploadServiceAgreementError').hide();
    $('#uploadSuretyBondError').hide();
    var isValid = true;
    if (!$('#chkPolicy').prop('checked')) {
        $('#uploadPolicyError').show();
        isValid = false;
    }
    if ($("#serviceAgreement").is(":visible") && $('#uploadServiceAgreement').get(0).files.length == 0) {
        $('#uploadServiceAgreementError').show();
        isValid = false;
    }
    if ($("#suretyBond").is(":visible") && $('#uploadSuretyBond').get(0).files.length == 0) {
        $('#uploadSuretyBondError').show();
        isValid = false;
    }
    return isValid;
}

function cancelAdmission() {
    $("#chkPolicy").prop('checked', false);
    $("#uploadServiceAgreement").val(null);
    $("#uploadSuretyBond").val(null);
    $("#uploadSuretyBondError").hide();
    $("#uploadServiceAgreementError").hide();
    $("#uploadPolicyError").hide();
}