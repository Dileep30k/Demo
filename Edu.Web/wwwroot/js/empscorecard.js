var scorecardBaseUrl = `${$('#websiteBaseUrl').val()}/Scorecard`;
var nominationModel = null;
var selectedFile = null;

$(document).ready(function () {
});

function getSchemeIntakes(obj) {
    $('#resultDiv').html('');
    $('#formScorecardDiv').html('');
    $('#optionDiv').hide();

    $('#intakeId').html('');
    var schemeId = $(obj).val();
    if (schemeId == '') {
        $('#intakeId').append($(`<option></option>`).val('').html('N/A'));
        return;
    }
    showLoading();
    $.ajax({
        url: `${scorecardBaseUrl}/GetSchemeIntakes`,
        type: 'POST',
        data: { schemeId },
        success: function (result) {
            if (result.length == 0) {
                $('#intakeId').append($(`<option></option>`).val('').html('N/A'));
                $('#resultDiv').html('<div class="col-sm-6 col-md-12 font-t-blue font-bold font-18 text-center">Intake not found.</div>');
            } else {
                $.each(result, function (index, value) {
                    $('#intakeId').append($(`<option ${index == 0 ? 'selected' : ''}></option>`).val
                        (value.id).html(value.text));
                });
                getScorecardModel($('#intakeId'));
            }
            hideLoading();
        },
        error: function (error) {
            displayToastr(error.data, "Intake Detail", "Error");
            hideLoading();
        },
    });
}

function getScorecardModel(obj) {
    nominationModel = null;
    $('#resultDiv').html('');
    $('#formScorecardDiv').html('');
    $('#optionDiv').hide();
    clearOptions();
    var intakeId = $(obj).val();
    selectedFile = null;
    if (intakeId == '') {
        return;
    }
    showLoading();
    $.ajax({
        url: `${scorecardBaseUrl}/GetScorecardModel`,
        type: 'POST',
        data: { intakeId },
        success: function (result) {
            if (result.success) {
                nominationModel = result.data;
                if (nominationModel.isExamTaken == null) {
                    $('#optionDiv').show();
                }
                else if (nominationModel.isExamTaken == true) {
                    $('#resultDiv').html(`<div class="col-sm-6 font-t-blue font-bold font-18 text-center">
                        Score already uploaded. Score: ${nominationModel.score}
                    </div>`);
                }
            } else {
                $('#resultDiv').html('<div class="col-sm-6 col-md-12 font-t-blue font-bold font-18 text-center">Nomination not found.</div>');
            }
            hideLoading();
        },
        error: function (error) {
            displayToastr(error.data, "Institute Detail", "Error");
            hideLoading();
        },
    });
}

function getScorecardForm() {
    showLoading();
    $.ajax({
        url: `${scorecardBaseUrl}/GetScorecardForm`,
        type: 'POST',
        data: { model: nominationModel },
        success: function (result) {
            $("#formScorecardDiv").html(result);
            $.validator.unobtrusive.parse("#formScorecard");
            renderControls();
            renderDropZone();
            hideLoading();
        },
        error: function (error) {
            hideLoading();
        },
    });
}

function getFilterParams(d) {
    d.schemeId = $('#schemeId').val();
    d.intakeId = $('#intakeId').val();
}

function clearOptions() {
    $('.score-option').each(function () { $(this).prop('checked', false); });
}

function optionSelected(type) {
    $("#formScorecardDiv").html('');
    if (type == 1) {
        getScorecardForm();
    } else if (type == 2) {
        displayModel('notTakenModal');
    } else if (type == 3) {
        displayModel('rejectModal');
    }
    selectedFile = null;
}

function cancelForm() {
    clearOptions();
    $("#formScorecardDiv").html('');
}

function notTakenExam() {
    showLoading();
    $.ajax({
        url: `${scorecardBaseUrl}/UpdateScorecardExam`,
        type: 'POST',
        data: { nominationId: nominationModel.nominationId },
        success: function (result) {
            if (result.success) {
                clearForm();
                displayToastr('Nomination updated exam status', "Nomination", "Success");
            } else {
                displayToastr('Nomination not updated', "Nomination", "Error");
            }
            hideLoading();
        },
        error: function (error) {
            displayToastr('Nomination not updated', "Nomination", "Error");
            hideLoading();
        },
    });
}

function rejectNomination() {
    showLoading();
    $.ajax({
        url: `${scorecardBaseUrl}/RejectNomination`,
        type: 'POST',
        data: { nominationId: nominationModel.nominationId },
        success: function (result) {
            if (result.success) {
                clearForm();
                displayToastr('Nomination rejected', "Nomination", "Success");
            } else {
                displayToastr('Nomination not rejected', "Nomination", "Error");
            }
            hideLoading();
        },
        error: function (error) {
            displayToastr('Nomination not rejected', "Nomination", "Error");
            hideLoading();
        },
    });
}

function uploadScorecard() {
    var scorecardForm = getFormData($("#formScorecard"));

    var formData = new FormData();
    formData.append('file', selectedFile);
    formData.append('nominationId', scorecardForm.NominationId);
    formData.append('score', scorecardForm.Score);

    showLoading();
    $.ajax({
        type: 'POST',
        url: `${scorecardBaseUrl}/UploadEmpScorecard`,
        data: formData,
        cache: false,
        contentType: false,
        processData: false,
        success: function (response) {
            if (response.success) {
                clearForm();
                displayToastr('Scorecard uploaded', "Scorecard", "Success");
            } else {
                displayToastr('Scorecard not uploaded', "Scorecard", "Error");
            }
            $('#uploadFile').val(null);
            hideLoading();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            $('#uploadFile').val(null);
            displayToastr('Scorecard not uploaded', "Scorecard", "Error");
            hideLoading();
        }
    });
}

function clearForm() {
    $('#schemeId').val('').trigger('change');
    $('#intakeId').append($(`<option></option>`).val('').html('N/A'));
    $('#resultDiv').html('');
    $('#formScorecardDiv').html('');
    $('#optionDiv').hide();
    hideModel('notTakenModal');
    hideModel('rejectModal');
}

function saveScorecard() {
    if (validateScorecard()) {
        uploadScorecard();
    }
}

function validateScorecard() {
    $('#scoreError').hide();
    $('#uploadError').hide();
    $('#uploadFileName').html('No file uploaded.');
    var isValid = true;
    if ($('#Score').val() == '' || +$('#Score').val() > 100) {
        $('#scoreError').show();
        isValid = false;
    }
    if (selectedFile == null) {
        $('#uploadError').show();
        isValid = false;
    }
    return isValid;
}

function renderDropZone() {
    $('#dropZone').on('dragenter', function (e) {
        e.stopPropagation();
        e.preventDefault();
        $("#dropZone").addClass('highlight-zone');
    }).on('dragover', function (e) {
        e.stopPropagation();
        e.preventDefault();
        $("#dropZone").addClass('highlight-zone');
    }).on('dragleave', function (e) {
        e.stopPropagation();
        e.preventDefault();
        $("#dropZone").removeClass('highlight-zone');
    }).on('drop', function (e) {
        e.stopPropagation();
        e.preventDefault();
        $("#dropZone").removeClass('highlight-zone');
        if (e.originalEvent.dataTransfer.files.length > 1) {
            displayToastr('Please upload only one file!', "Upload File", "Error");
            return;
        }
        uploadData(e.originalEvent.dataTransfer.files[0]);
    });

    $("#Score").on('change', function (e) {
        $('#scoreError').hide();
        if ($(this).val() == '' || +$(this).val() > 100) {
            $('#scoreError').show();
        }
    });

    $("#uploadFile").on('change', function (e) {
        uploadData(e.target.files[0]);
    });
}

function openUpload() {
    $("#uploadFile").trigger('click');
}

function uploadData(file) {
    $('#uploadFileName').html('No file uploaded.');
    var validExtentions = ['gif', 'png', 'jpg', 'jpeg', 'pdf'];
    var ext = file.name.split('.').pop().toLowerCase();
    if (!validExtentions.find(e => e == ext)) {
        displayToastr('Please upload only image/pdf file!', "Upload File", "Error");
        return;
    }
    $('#uploadFileName').html(file.name);

    selectedFile = file;
    $('#uploadError').hide();
}
