var dashboardBaseUrl = `${$('#websiteBaseUrl').val()}/Dashboard`;

$(document).ready(function () {

});

function getSchemeIntakes(obj) {
    getEmployeeSchemeIntakes(obj);
}

function getEmployeeSchemeIntakes(obj) {
    $('.content-data').hide();
    $('#intakeId').html('');
    var schemeId = $(obj).val();
    if (schemeId == '') {
        $('#intakeId').append($(`<option></option>`).val('').html('N/A'));
        return;
    }
    showLoading();
    $.ajax({
        url: `${dashboardBaseUrl}/GetSchemeIntakes`,
        type: 'POST',
        data: { schemeId },
        success: function (result) {
            if (result.length == 0) {
                $('#intakeId').append($(`<option></option>`).val('').html('N/A'));
            } else {
                $.each(result, function (index, value) {
                    $('#intakeId').append($(`<option ${index == 0 ? 'selected' : ''}></option>`).val
                        (value.id).html(value.text));
                });
                getAllData();
            }
            hideLoading();
        },
        error: function (error) {
            displayToastr(error.data, "Intake Detail", "Error");
            hideLoading();
        },
    });
}

function getAllData() {
    $('.content-data').show();
    getIntakeInstitutes();
    getEmployeeNomination();
}

function getFilterParams() {
    return {
        schemeId: $('#schemeId').val(),
        intakeId: $('#intakeId').val(),
        startDate: null,
        endDate: null,
    };
}

function getIntakeInstitutes() {
    $('#intake-institute').html('');
    var intakeId = $($('#intakeId')).val();
    $.ajax({
        url: `${dashboardBaseUrl}/GetIntakeInstitutes`,
        type: 'POST',
        data: { intakeId },
        success: function (result) {
            $.each(result, function (index, value) {
                $('#intake-institute').append(`
                <div class="col-xxl-12   col-md-12">
                    <h6 class="card-value" style="font-size:13px">${value.text}</h6>
                </div>`);
            });
        },
        error: function (error) {
            displayToastr(error.data, "Institute", "Error");
        },
    });
}

function getEmployeeNomination() {
    $('#empapprove').removeClass('selected rejected lineSelected');
    $('#approver1').removeClass('selected rejected lineSelected');
    $('#approver2').removeClass('selected rejected lineSelected');
    $('#nominationApprover1').removeClass('lineSelected');
    $('#nominationApprover2').removeClass('lineSelected');
    var schemeId = $($('#schemeId')).val();
    var intakeId = $($('#intakeId')).val();
    $.ajax({
        url: `${dashboardBaseUrl}/GetEmployeeNomination`,
        type: 'POST',
        data: { schemeId, intakeId },
        success: function (result) {
            if (result.success) {
                var nomination = result.data;
                if (nomination.nominationStatusId == nominationStatuses.Accepted) {
                    $('#empapprove').addClass('selected ');
                }
                if (nomination.nominationStatusId == nominationStatuses.Rejected) {
                    $('#empapprove').addClass('rejected ');
                }
                if (nomination.nominationStatusId == nominationStatuses.DepApprove) {
                    $('#empapprove').addClass('selected ');
                    $('#approver1').addClass('selected ');
                    $('#approver1').addClass('lineSelected');
                    $('#nominationApprover1').addClass('lineSelected');
                }
                if (nomination.nominationStatusId == nominationStatuses.DepRejected) {
                    $('#empapprove').addClass('selected ');
                    $('#approver1').addClass('rejected ');
                    $('#approver1').addClass('lineSelected');
                    $('#nominationApprover1').addClass('lineSelected');
                }
                if (nomination.nominationStatusId == nominationStatuses.DivApprove) {
                    $('#empapprove').addClass('selected ');
                    $('#approver1').addClass('selected ');
                    $('#approver1').addClass('lineSelected');
                    $('#nominationApprover1').addClass('lineSelected');
                    $('#approver2').addClass('selected');
                    $('#approver2').addClass('lineSelected');
                    $('#nominationApprover2').addClass('lineSelected');
                }
                if (nomination.nominationStatusId == nominationStatuses.DivRejected) {
                    $('#empapprove').addClass('selected ');
                    $('#approver1').addClass('selected ');
                    $('#approver1').addClass('lineSelected');
                    $('#nominationApprover1').addClass('lineSelected');
                    $('#approver2').addClass('rejected ');
                    $('#approver2').addClass('lineSelected');
                    $('#nominationApprover2').addClass('lineSelected');
                }
            }
        },
        error: function (error) {
            displayToastr(error.data, "Admission", "Error");
        },
    });
}
