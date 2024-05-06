var dashboardBaseUrl = `${$('#websiteBaseUrl').val()}/Dashboard`;

$(document).ready(function () {
    getAllData();
});

function getSchemeIntakes(obj) {
    $('#intakeId').html($(`<option></option>`).val('').html('All'));
    var schemeId = $(obj).val();
    showLoading();
    $.ajax({
        url: `${dashboardBaseUrl}/GetSchemeIntakes`,
        type: 'POST',
        data: { schemeId },
        success: function (result) {
            $.each(result, function (index, value) {
                $('#intakeId').append($(`<option></option>`).val(value.id).html(value.text));
            });
            getAllData();
            hideLoading();
        },
        error: function (error) {
            displayToastr(error.data, "Intake Detail", "Error");
            hideLoading();
        },
    });
}

function getAllData() {
    getTotalEligible();
    getTotalNomination();
    getTotalInstitute();
    getTotalAdmission();
    getTotalWaillist();
    getPendingServiceAgreement();
    getPendingNominationAcceptance();
    getPendingAdmissionConfirmation();
    getAdmissionPagedByDesg();
    getAdmissionPagedByDiv();
}

function getFilterParams() {
    return {
        schemeId: $('#schemeId').val(),
        intakeId: $('#intakeId').val(),
        startDate: null,
        endDate: null,
    };
}

function getTotalEligible() {
    $('#total-eligible').html('0');
    $.ajax({
        url: `${dashboardBaseUrl}/GetTotalEligible`,
        type: 'POST',
        data: getFilterParams(),
        success: function (result) {
            $('#total-eligible').html(result);
        },
        error: function (error) {
            displayToastr(error.data, "Total Eligible", "Error");
        },
    });
}

function getTotalNomination() {
    $('#total-nominations').html('0');
    $.ajax({
        url: `${dashboardBaseUrl}/GetTotalNomination`,
        type: 'POST',
        data: getFilterParams(),
        success: function (result) {
            $('#total-nominations').html(result);
        },
        error: function (error) {
            displayToastr(error.data, "Total Nomination", "Error");
        },
    });
}

function getTotalInstitute() {
    $('#total-institute').html('0');
    $.ajax({
        url: `${dashboardBaseUrl}/GetTotalInstitute`,
        type: 'POST',
        data: getFilterParams(),
        success: function (result) {
            $('#total-institute').html(result);
        },
        error: function (error) {
            displayToastr(error.data, "Total Institute", "Error");
        },
    });
}

function getTotalAdmission() {
    $('#total-admissions').html('0');
    $.ajax({
        url: `${dashboardBaseUrl}/GetTotalAdmission`,
        type: 'POST',
        data: getFilterParams(),
        success: function (result) {
            $('#total-admissions').html(result);
        },
        error: function (error) {
            displayToastr(error.data, "Total Admission", "Error");
        },
    });
}

function getTotalWaillist() {
    $('#total-waillist').html('0');
    $.ajax({
        url: `${dashboardBaseUrl}/GetTotalWaillist`,
        type: 'POST',
        data: getFilterParams(),
        success: function (result) {
            $('#total-waillist').html(result);
        },
        error: function (error) {
            displayToastr(error.data, "Total Waillist", "Error");
        },
    });
}

function getPendingServiceAgreement() {
    $('#pending-sa').html('0');
    $.ajax({
        url: `${dashboardBaseUrl}/GetPendingServiceAgreement`,
        type: 'POST',
        data: getFilterParams(),
        success: function (result) {
            $('#pending-sa').html(result);
        },
        error: function (error) {
            displayToastr(error.data, "Pending Service Agreement", "Error");
        },
    });
}

function getPendingNominationAcceptance() {
    $('#pending-ac').html('0');
    $.ajax({
        url: `${dashboardBaseUrl}/GetPendingNominationAcceptance`,
        type: 'POST',
        data: getFilterParams(),
        success: function (result) {
            $('#pending-ac').html(result);
        },
        error: function (error) {
            displayToastr(error.data, "Pending Nomination Acceptance", "Error");
        },
    });
}

function getPendingAdmissionConfirmation() {
    $('#pending-confirm').html('0');
    $.ajax({
        url: `${dashboardBaseUrl}/GetPendingAdmissionConfirmation`,
        type: 'POST',
        data: getFilterParams(),
        success: function (result) {
            $('#pending-confirm').html(result);
        },
        error: function (error) {
            displayToastr(error.data, "Pending Admission Confirmation", "Error");
        },
    });
}

function getAdmissionPagedByDesg() {
    $.ajax({
        url: `${dashboardBaseUrl}/GetAdmissionPagedByDesg`,
        type: 'POST',
        data: getFilterParams(),
        success: function (result) {
            const ctx = document.getElementById('pieChart');
            new Chart(ctx, {
                type: 'pie',
                data: {
                    labels: result.map(r => r.name),
                    datasets: [{
                        data: result.map(r => r.count),
                        //backgroundColor: result.map(r => getRandomColor()),
                        backgroundColor: ["#9ADCFF", "#EDB7ED", "#FFF89A", "#FFB2A6", "#FF8AAE", "#CBFFA9", "#FF8DC7"],
                        borderWidth: 1
                    }]
                },
                options: {
                    plugins: {
                        legend: {
                            display: true,
                            position: 'right',
                            labels: {
                                font: {
                                    size: 10
                                },
                                usePointStyle: true,
                                boxHeight: 8
                            }
                        }
                    },
                    gridLines: {
                        display: false
                    }
                }
            });
        },
        error: function (error) {
            displayToastr(error.data, "Enrollment Levels", "Error");
        },
    });
}

function getAdmissionPagedByDiv() {
    $.ajax({
        url: `${dashboardBaseUrl}/GetAdmissionPagedByDiv`,
        type: 'POST',
        data: getFilterParams(),
        success: function (result) {
            const ctx = document.getElementById('barChart');
            new Chart(ctx, {
                type: 'bar',
                data: {
                    labels: result.map(r => r.name),
                    datasets: [{
                        data: result.map(r => r.count),
                        backgroundColor: ["#A3D6BE"],
                    }]


                },
                options: {
                    plugins: {
                        legend: {
                            display: false,
                        }
                    },
                    scales: {
                        y: {
                            beginAtZero: true,
                            grid: {
                                display: false
                            }
                        },
                        x: {
                            barThickness: 6,  // number (pixels) or 'flex'
                            maxBarThickness: 8,
                            grid: {
                                display: false
                            }
                        }
                    }
                }


            });
        },
        error: function (error) {
            displayToastr(error.data, "Division Analytics", "Error");
        },
    });
}

function getRandomColor() {
    color = "hsl(" + Math.random() * 360 + ", 100%, 75%)";
    return color;
}