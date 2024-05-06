var nominationBaseUrl = `${$('#websiteBaseUrl').val()}/Nomination`;
var nominationForm = null;

$(document).ready(function () {

});

function getSchemeIntakes(obj) {
    $('#divViewButtons').hide();
    $('#instituteDiv').html('');
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
                $('#instituteDiv').html('<div class="col-sm-6 col-md-12 font-t-blue font-bold font-18 text-center">Intake not found.</div>');
            } else {
                $.each(result, function (index, value) {
                    $('#intakeId').append($(`<option ${index == 0 ? 'selected' : ''}></option>`).val
                        (value.id).html(value.text));
                });
                getNominationModel($('#intakeId'));
            }
            hideLoading();
        },
        error: function (error) {
            displayToastr(error.data, "Intake Detail", "Error");
            hideLoading();
        },
    });
}

function getNominationModel(obj) {
    nominationForm = null;
    $('#instituteDiv').html('');
    $('#divButtons').hide();
    $('#divViewButtons').hide();
    var intakeId = $(obj).val();
    if (intakeId == '') {
        return;
    }
    showLoading();
    $.ajax({
        url: `${nominationBaseUrl}/GetNominationModel`,
        type: 'POST',
        data: { intakeId },
        success: function (result) {
            if (result.success) {
                nominationForm = result.data;
                getNominationInstitutes($('#intakeId'));
            } else {
                $('#instituteDiv').html('<div class="col-sm-6 col-md-12 font-t-blue font-bold font-18 text-center">Nomination not found.</div>');
            }
            hideLoading();
        },
        error: function (error) {
            displayToastr(error.data, "Institute Detail", "Error");
            hideLoading();
        },
    });
}

function getNominationInstitutes(obj) {
    var intakeId = $(obj).val();
    showLoading();
    $.ajax({
        url: `${nominationBaseUrl}/GetNominationInstitutes`,
        type: 'POST',
        data: { intakeId },
        success: function (result) {
            if (result.trim() != '') {
                $('#instituteDiv').html(result);
            }
            toggelForm();
            hideLoading();
        },
        error: function (error) {
            displayToastr(error.data, "Institute Detail", "Error");
            hideLoading();
        },
    });
}

function getFilterParams(d) {
    d.schemeId = $('#schemeId').val();
    d.intakeId = $('#intakeId').val();
}

function clearFilters() {
}


function showAcceptNomination() {
    $("#chkTerms").prop('checked', false);
    displayModel('templateModal');
}

function rejectNomination() {
    nominationForm = { ...nominationForm, intakeId: $("#intakeId").val() }
    showLoading();
    $.ajax({
        url: `${nominationBaseUrl}/RejectNomination`,
        type: 'POST',
        data: { nomination: nominationForm },
        success: function (result) {
            if (result.success) {
                nominationForm = result.data;
                hideModel('rejectModal');
                toggelForm();
                displayToastr('Nomination Rejected', "Nomination", "Success");
            } else {
                displayToastr('Nomination not Rejected', "Nomination", "Error");
            }
            hideLoading();
        },
        error: function (error) {
            displayToastr('Nomination not Submitted', "Nomination", "Error");
            hideLoading();
        },
    });
}

function acceptNomination() {
    nominationForm = getFormData($("#formNomination"));
    nominationForm = { ...nominationForm, intakeId: $("#intakeId").val() }
    showLoading();
    $.ajax({
        url: `${nominationBaseUrl}/AcceptNomination`,
        type: 'POST',
        data: { nomination: nominationForm },
        success: function (result) {
            if (result.success) {
                nominationForm = result.data;
                hideModel('approvalModal');
                toggelForm();
                displayToastr('Nomination Submitted', "Nomination", "Success");
            } else {
                displayToastr('Nomination not Submitted', "Nomination", "Error");
            }
            hideLoading();
        },
        error: function (error) {
            displayToastr('Nomination not Submitted', "Nomination", "Error");
            hideLoading();
        },
    });
}

function fillNomination() {
    if (!$("#chkTerms").is(":checked")) {
        displayToastr('Please accept the terms', "Nomination Form", "Error");
        return;
    }
    hideModel('templateModal');
    toggelForm(true);
}


function toggelForm(isForm = false) {
    $('#nominationForm').html('');
    $('#divButtons').hide();
    $('#divViewButtons').hide();
    if (isForm) {
        $('.list-content').hide();
        $('.form-content').show();
        getNominationForm();
    } else {
        $('.form-content').hide();
        $('.list-content').show();
    }
    if (+nominationForm.nominationStatusId > nominationStatuses.Submitted) {
        $('#divViewButtons').show();
        $('#btnStatus').html(nominationForm.nominationStatusName);
    } else {
        $('#divButtons').show();
        $('#btnStatus').html('');
    }
}

function getNominationForm() {
    showLoading();
    $.ajax({
        url: `${nominationBaseUrl}/GetNominationForm`,
        type: 'POST',
        data: { model: nominationForm },
        success: function (result) {
            $("#nominationForm").html(result);
            $.validator.unobtrusive.parse("#formNomination");
            renderControls();
            hideLoading();
        },
        error: function (error) {
            hideLoading();
        },
    });
}

function validateNomination() {
    $("#institutes-error").hide();
    var institutes = $("#institutes").val().reduce((acc, x) => acc.concat(+x), []);
    $('#SelectedInstitutes').val(institutes.join());

    if ($("#formNomination").valid()) {
        if (institutes.length === 0) {
            $("#institutes-error").show();
            return false;
        }
        displayModel('approvalModal');
        return true;
    } else {
        if (institutes.length === 0) {
            $("#institutes-error").show();
        }
    }
    return false;
}
