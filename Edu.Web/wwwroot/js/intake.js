var intakeBaseUrl = `${$('#websiteBaseUrl').val()}/Intake`;
$(document).ready(function () {
    $("#intakeDatatable").DataTable({
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
            "url": `${intakeBaseUrl}/GetIntakes`,
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
                "data": "intakeId", "name": "intakeId",
                render: function (row, type, data, meta) {
                    return meta.row + 1;
                },
            },
            {
                "data": "intakeName", "name": "intakeName",
                render: function (row, type, data) {
                    return `<a href='./Intake/Manage/${data.intakeId}'>${data.intakeName}</a>`;
                },
            },
            {
                "data": "startDate", "name": "startDate",
                render: function (row, type, data) {

                    return getFormatDate(data.startDate);
                }
            },
            { "data": "schemeName", "name": "schemeName", "autoWidth": true },
            {
                "data": "nominationCutoffDate", "name": "nominationCutoffDate",
                render: function (row, type, data) {
                    return getFormatDate(data.nominationCutoffDate);
                }
            },
            {
                render: function (row, type, data) {
                    return data.institutes.map(i => i.instituteCode).join('<br/>\r');
                }
            },
            {
                render: function (row, type, data) {
                    return data.institutes.map(i => i.totalSeats).join('<br/>\r');
                }
            },
            {
                render: function (row, type, data) {
                    return data.institutes.map(i => getFormatDate(i.admissionCutoffDate)).join('<br/>\r');
                }
            },
            {
                "data": "examDate", "name": "examDate",
                render: function (row, type, data) {
                    return getFormatDate(data.examDate);
                }
            },
            {
                render: function (row, type, data) {
                    return data.documentTypes.join('<br/>\r');
                }
            },
        ]
    });

    if ($("#IntakeId").length && +$("#IntakeId").val() > 0) {
        $(`#intakeIntitutesDiv`).show();
    } else {
        $(`#intakeIntitutesDiv`).hide();
    }

    $("#btnFilter").click(function (e) {
        e.preventDefault();
        filterTable();
    });

    $("#btnFilterClear").click(function (e) {
        e.preventDefault();
        clearFilters();
        filterTable();
    });

    $("#schemeInstitutes").on("select2:select select2:unselect", function (e) {
        e.preventDefault();
        var items = $(this).val();
        var instituteId = e.params.data.id;
        var institueName = e.params.data.text;
        if (items.some(i => i == instituteId)) {
            showLoading();
            $.ajax({
                url: `${intakeBaseUrl}/GetIntakeInstitute`,
                type: 'POST',
                data: { institueName, instituteId },
                success: function (result) {
                    $("#intakeIntituteDetails").append(result);
                    $("#formIntake").removeData("validator");
                    $("#formIntake").removeData("unobtrusiveValidation");
                    $.validator.unobtrusive.parse("#formIntake");
                    renderControls();
                    hideLoading();
                },
                error: function (error) {
                    hideLoading();
                },
            });
        } else {
            removeSchemeInstitute(instituteId);
        }

        if (items.length == 0) {
            $(`#intakeIntitutesDiv`).hide();
            $("#intakeIntituteDetails").html('');
        } else {
            $(`#intakeIntitutesDiv`).show();
        }
    })
});

function clearFilters() {
    $('#searchValue').val('');
    $('#startDate').val('');
    $('#endDate').val('');
    $('#filterSchemeId').val('').trigger('change');
    $('#filterIntakeId').val('').trigger('change');
}

function removeSchemeInstitute(instituteId) {
    $(`#institutes-${instituteId}`).remove();
}

function resetFrom(formId) {
    $(`#${formId}`).trigger("reset");
}

function validateIntake() {
    $("#scoreUpload-error").hide();
    $("#locations-error").hide();
    $("#schemeInstitutes-error").hide();
    $("#documentTypes-error").hide();
    var locations = $("#locations").val().reduce((acc, x) => acc.concat(+x), []);
    var institutes = $("#schemeInstitutes").val().reduce((acc, x) => acc.concat(+x), []);
    var documentTypes = $("#documentTypes").val().reduce((acc, x) => acc.concat(+x), []);
    $('#SelectedLocations').val(locations.join());
    $('#SelectedInstitutes').val(institutes.join());
    $('#SelectedDocumentTypes').val(documentTypes.join());

    if ($("#formIntake").valid()) {
        var isValid = true;
        if (locations.length === 0) {
            $("#locations-error").show();
            isValid = false;
        }
        if (institutes.length === 0) {
            $("#schemeInstitutes-error").show();
            isValid = false;
        }
        if (documentTypes.length === 0) {
            $("#documentTypes-error").show();
            isValid = false;
        }
        if (!$("#IsGTSScoreUpload").is(":checked") && !$("#IsEmplyoeeScoreUpload").is(":checked")) {
            $("#scoreUpload-error").show();
            isValid = false;
        }
        if ($("#scoreCardCutOffDate").val() < $("#examDate").val()) {
            $("#scoreCard-error").show();
            isValid = false;
        }
        if (isValid) {
            showLoading();
        }
        return isValid;
    } else {
        if (locations.length === 0) {
            $("#locations-error").show();
        }
        if (institutes.length === 0) {
            $("#schemeInstitutes-error").show();
        }
        if (documentTypes.length === 0) {
            $("#documentTypes-error").show();
        }
        if (!$("#IsGTSScoreUpload").is(":checked") && !$("#IsEmplyoeeScoreUpload").is(":checked")) {
            $("#scoreUpload-error").show();
        }
        if ($("#scoreCardCutOffDate").val() < $("#examDate").val()) {
            $("#scoreCard-error").show();
        }
    }
    return false;


}

function getSchemeInstitutes(obj) {
    $('#schemeInstitutes').html('');
    $("#intakeIntituteDetails").html('');
    $('#intakeIntitutesDiv').hide();
    $("#schemeInstitutes-error").hide();
    var schemeId = $(obj).val();
    if (schemeId == '') {
        return;
    }
    showLoading();
    $.ajax({
        url: `${intakeBaseUrl}/GetSchemeInstitutes`,
        type: 'POST',
        data: { schemeId },
        success: function (result) {
            if (result) {
                $.each(result, function (key, value) {
                    $('#schemeInstitutes').append($(`<option></option>`).val
                        (value.id).html(value.text));
                });
            }
            hideLoading();
        },
        error: function (error) {
            displayToastr(error.data, "Institues Detail", "Error")
            hideLoading();
        },
    });
}

function getSchemeIntakes(obj) {
    $('#filterIntakeId').html($(`<option></option>`).val('').html('All'));
    var schemeId = $(obj).val();
    showLoading();
    $.ajax({
        url: `${intakeBaseUrl}/GetSchemeIntakes`,
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

function getFilterParams(d) {
    d.schemeId = $('#filterSchemeId').val();
    d.intakeId = $('#filterIntakeId').val();
    d.searchValue = $('#searchValue').val();
    d.startDate = $('#startDate').val();
    d.endDate = $('#endDate').val();
}

function filterTable() {
    $(`#intakeDatatable`).DataTable().ajax.reload();
}

function changeScoreCardCutOffDate(d) {
    var scoreCardCutOffDate = d;
    var examDate = $('#ExamDate').val();
    console.log(examDate);
    console.log(scoreCardCutOffDate);

    if (scoreCardCutOffDate < examDate) {
        $("#scoreCardCutOffDate").val("");
        $("#scoreCard-error").show();
    } else {
        $("#scoreCard-error").hide();
    }

}


