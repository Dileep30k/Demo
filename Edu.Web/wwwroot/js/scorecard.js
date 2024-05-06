var scorecardBaseUrl = `${$('#websiteBaseUrl').val()}/Scorecard`;
var scorecardData = [];
$(document).ready(function () {
    $("#scorecardDatatable").DataTable({
        "processing": true,
        "serverSide": true,
        "filter": false,
        "bLengthChange": false,
        "bPaginate": true,
        "scrollX": true,
        "bInfo": false,
        "fnDrawCallback": function (oSettings) {
            scorecardData = this.fnGetData();
            renderDecimalOnly();
        },
        "dom": 'Bfrtip',
        "buttons": [
            {
                extend: 'excel',
                customize: function (xlsx) {
                    $('row c', xlsx.xl.worksheets['sheet1.xml']).attr('s', '55');
                },
                exportOptions: {
                    columns: ':not(.notexport)'
                }
            },
        ],
        "ajax": {
            "url": `${scorecardBaseUrl}/GetScorecards`,
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
            { "data": "staffEmail", "name": "staffEmail", "autoWidth": true },
            { "data": "mobileNo", "name": "mobileNo", "autoWidth": true },
            {
                render: function (row, type, data) {
                    return data.instituteNames.map(i => i).join('<br/>\r');
                }
            },
            {
                "data": "examDate", "name": "examDate", "autoWidth": true,
                render: function (row, type, data) {
                    return getFormatDate(data.examDate);
                }
            },
            {
                "data": "score", "name": "score", "autoWidth": true,
                render: function (row, type, data, meta) {
                    return `<input id="score-${data.nominationId}" maxlength="5" onchange="changeScore(this, ${data.nominationId}, ${meta.row})" type="text" class="decimalOnly scoreinput" value="${data.score || ''}" disabled>
                            <lable id="score-label-${data.nominationId}" class="display-none">${data.score || ''}</lable>`;
                }
            },
            {
                render: function (row, type, data) {
                    return data.documents.map(doc => `<a href="${doc.filePath}" target="_blank">${doc.fileName}</a>`).join('<br/>\r');
                }
            },
            {
                render: function (row, type, data, meta) {
                    return data.isEdit && data.isExamTaken ? `
                        <span id="edit-${data.nominationId}" onclick="editScroe(${data.nominationId}, ${meta.row})" class="cursor-pointer score-approve"><i class="fas fa-edit"></i> Edit</span>
                        <span id="save-${data.nominationId}" onclick="saveScore(${data.nominationId}, ${meta.row})" class="cursor-pointer score-save display-none"><i class="fas fa-save"></i> Save</span>
                        ` : ``;
                }
            },
            {
                render: function (row, type, data, meta) {
                    return data.isExamTaken ? `
                        <span id="score-approved-${data.nominationId}" class="score-approved  ${(data.isScoreApprove ? '' : 'display-none')}"><i class="fas fa-check-circle"></i> Approved</span>
                        <span id="score-approve-${data.nominationId}" onclick="approveScore(${data.nominationId}, ${meta.row})" class="cursor-pointer score-approve ${(data.isScoreApprove ? 'display-none' : '')}"><i class="fas fa-check-circle"></i> Approve</span>
                        ` : ``;
                }
            },
        ]
    });

    $("#scorecardInstituteDatatable").DataTable({
        "processing": true,
        "serverSide": true,
        "filter": false,
        "bLengthChange": false,
        "bPaginate": true,
        "scrollX": false,
        "deferLoading": 0,
        "bInfo": false,
        "dom": 'Bfrtip',
        "buttons": [
            {
                extend: 'excel',
                customize: function (xlsx) {
                    $('row c', xlsx.xl.worksheets['sheet1.xml']).attr('s', '55');
                },
                exportOptions: {
                    columns: ':not(.notexport)'
                }
            },
        ],
        "ajax": {
            "url": `${scorecardBaseUrl}/GetScorecards`,
            "type": "POST",
            "datatype": "json",
            "data": function (d) { return getInstututeFilterParams(d); }
        },
        "columnDefs": [{
            "targets": 'no-sort',
            "orderable": false,
        }],
        "columns": [
            { "data": "vertical", "name": "vertical", "autoWidth": true },
            { "data": "msilUserId", "name": "msilUserId", "autoWidth": true },
            { "data": "staffName", "name": "staffName", "autoWidth": true },
            { "data": "staffEmail", "name": "staffEmail", "autoWidth": true },
            { "data": "mobileNo", "name": "mobileNo", "autoWidth": true },
            {
                render: function (row, type, data) {
                    return data.instituteNames.map(i => i).join('<br/>\r');
                }
            },
            {
                "data": "examDate", "name": "examDate", "autoWidth": true,
                render: function (row, type, data) {
                    return getFormatDate(data.examDate);
                }
            },
            {
                "data": "score", "name": "score", "autoWidth": true,
                render: function (row, type, data, meta) {
                    return `<lable>${data.score}</lable>`;
                }
            },
            {
                render: function (row, type, data) {
                    return data.documents.map(doc => doc.fileName).join('<br/>\r');
                }
            },
            {
                render: function (row, type, data, meta) {
                    return ``;
                }
            },
            {
                render: function (row, type, data, meta) {
                    return `
                        <span id="score-approved-${data.nominationId}" class="score-approved  ${(data.isScoreApprove ? '' : 'display-none')}"><i class="fas fa-check-circle"></i> Approved</span>
                        <span id="score-approve-${data.nominationId}" onclick="approveScore(${data.nominationId}, ${meta.row})" class="cursor-pointer score-approve ${(data.isScoreApprove ? 'display-none' : '')}"><i class="fas fa-check-circle"></i> Approve</span>
                        `;
                }
            },
        ]
    });

    $('#instituteId').html($(`<option></option>`).val('').html('Please select the institute'));

    $("#file-upload-scorecard").on('change', function (e) {
        $('#file-upload-scorecard-error').html('');
        var file = e.target.files[0];
        if (!file || !(/\.(xlsx|xls)$/i).test(file.name)) {
            $('#file-upload-scorecard-error').html('Please upload only excel file!');
            $(this).val(null);
        }
    });

    $("#file-upload-documents").on('change', function (e) {
        $('#file-upload-documents-error').html('');
        var file = e.target.files[0];
        if (!file || !(/\.(zip)$/i).test(file.name)) {
            $('#file-upload-documents-error').html('Please upload only zip file!');
            $(this).val(null);
        }
    });
});

function getSchemeIntakes(obj) {
    $('#btn-upload-scorecard').hide();
    $('#intakeId').html($(`<option></option>`).val('').html('All'));
    var schemeId = $(obj).val();
    showLoading();
    $.ajax({
        url: `${scorecardBaseUrl}/GetSchemeIntakes`,
        type: 'POST',
        data: { schemeId },
        success: function (result) {
            if (result.length == 0) {
            } else {
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

function getIntakeInstitutes(obj) {
    var intakeId = $(obj).val();
    $('#instituteId').html($(`<option></option>`).val('').html('Please select the institute'));
    showLoading();
    $.ajax({
        url: `${scorecardBaseUrl}/GetIntakeInstitutes`,
        type: 'POST',
        data: { intakeId },
        success: function (result) {
            $.each(result, function (index, value) {
                $('#instituteId').append($(`<option></option>`).val(value.id).html(value.text));
            });
            hideLoading();
        },
        error: function (error) {
            displayToastr(error.data, "Institute Detail", "Error");
            hideLoading();
        },
    });
}

function filterTable() {
    $(`#scorecardDatatable`).DataTable().ajax.reload();
    allowGtsScorecard();
}

function getFilterParams(d) {
    d.searchValue = $('#searchValue').val();
    d.schemeId = $('#schemeId').val();
    d.intakeId = $('#intakeId').val();
    d.isScoreApprove = $('#isScoreApprove').val();
}

function changeScore(obj, id, index) {
    $(`#score-label-${id}`).val($(obj).val());
}

function approveScore(id, index) {
    if ($(`#save-${id}`).is(":visible")) { return; }
    scorecardData[index].isScoreApprove = true;
    $(`#score-approved-${id}`).show();
    $(`#score-approve-${id}`).hide();
    $(`#edit-${id}`).show();
    updateNominationScoreApproval(index);
}

function editScroe(id, index) {
    $(`#score-${id}`).attr('disabled', false);
    $(`#save-${id}`).show();
    $(`#edit-${id}`).hide();
}

function saveScore(id, index) {
    if ($(`#score-${id}`).val() == '' || +$(`#score-${id}`).val() > 100) {
        displayToastr('Please enter valid score', "Score", "Error");
        return;
    }
    if (scorecardData[index].score != $(`#score-${id}`).val()) {
        scorecardData[index].score = $(`#score-${id}`).val();
        scorecardData[index].isScoreApprove = false;
        updateNominationScore(index);
    }
    $(`#save-${id}`).hide();
    $(`#edit-${id}`).show();
    $(`#score-${id}`).attr('disabled', true);
    $(`#score-approved-${id}`).hide();
    $(`#score-approve-${id}`).show();
}

function updateNominationScore(index) {
    showLoading();
    $.ajax({
        url: `${scorecardBaseUrl}/UpdateNominationScore`,
        type: 'POST',
        data: { nominationId: scorecardData[index].nominationId, score: scorecardData[index].score },
        success: function (result) {
            if (result.success) {
                displayToastr('Exam percentile updated', "Score Card", "Success");
            } else {
                displayToastr('Exam percentile not updated', "Score Card", "Error");
            }
            hideLoading();
        },
        error: function (error) {
            displayToastr('Exam percentile not updated', "Score Card", "Error");
            hideLoading();
        },
    });
}

function updateNominationScoreApproval(index) {
    showLoading();
    $.ajax({
        url: `${scorecardBaseUrl}/UpdateNominationScoreApproval`,
        type: 'POST',
        data: { nominationId: scorecardData[index].nominationId, isScoreApprove: scorecardData[index].isScoreApprove },
        success: function (result) {
            if (result.success) {
                displayToastr('Score Card Approved', "Score Card", "Success");
            } else {
                displayToastr('Score Card not Approved', "Score Card", "Error");
            }
            hideLoading();
        },
        error: function (error) {
            displayToastr('Score Card not Approved', "Score Card", "Error");
            hideLoading();
        },
    });
}

function filterInsituteTable(obj) {
    var instituteId = $(obj).val();
    if (instituteId == '') {
        return;
    }
    $(`#scorecardInstituteDatatable`).DataTable().ajax.reload();
    $('#scorecardInstituteDatatable_wrapper .dataTables_paginate').hide();
}

function exportInstituteExcel() {
    var instituteId = $('#instituteId').val();
    if (instituteId == '') {
        displayToastr('Please select institute', "Score card", "Error");
        return;
    }
    $('#scorecardInstituteDatatable_wrapper .buttons-excel').click()
    hideModel('exportModal');
}

function exportInstitute() {
    var intakeId = $($('#intakeId')).val();
    if (intakeId == '') {
        displayToastr('Please select Financial Year', "Institute Detail", "Error");
        return;
    }
    getIntakeInstitutes($('#intakeId'));
    displayModel('exportModal');
}

function downloadAllScoreCard() {
    window.location = `${scorecardBaseUrl}/DownloadAllScoreCard?schemeId=${$('#schemeId').val()}&intakeId=${$('#intakeId').val()}`;
}

function getInstututeFilterParams(d) {
    d.searchValue = $('#searchValue').val();
    d.schemeId = $('#schemeId').val();
    d.intakeId = $('#intakeId').val();
    d.instituteId = $('#instituteId').val();
    d.isScoreApprove = $('#isScoreApprove').val();
}

function allowGtsScorecard() {
    $('#btn-upload-scorecard').hide();
    var intakeId = $('#intakeId').val();
    if (+intakeId > 0) {
        $.ajax({
            url: `${scorecardBaseUrl}/AllowGtsScorecard`,
            type: 'POST',
            data: { intakeId },
            success: function (result) {
                if (result) {
                    $('#btn-upload-scorecard').show();
                }
            },
            error: function (error) {
            },
        });
    }
}

function onUploadScorecard() {
    $('#uploadError').html('');
    $('#file-upload-scorecard-error').html('');
    $('#file-upload-documents-error').html('');
    $('#file-upload-scorecard').val(null);
    $('#file-upload-documents').val(null);
    displayModel('uploadModal');
}

function uploadScorecards() {
    $('#uploadError').html('');
    if (validateuploadScorecard()) {
        var formData = new FormData();
        formData.append('scorecardFile', $('#file-upload-scorecard').get(0).files[0]);
        formData.append('documentFile', $('#file-upload-documents').get(0).files[0]);
        formData.append('schemeId', $('#schemeId').val());
        formData.append('intakeId', $('#intakeId').val());
        showLoading();
        $.ajax({
            type: 'POST',
            url: `${scorecardBaseUrl}/UploadScorecards`,
            data: formData,
            cache: false,
            contentType: false,
            processData: false,
            success: function (response) {
                if (response.success) {
                    displayToastr('Scorecards Uploaded', "Upload Scorecards", "Success");
                    hideModel('uploadModal');
                    $(`#scorecardDatatable`).DataTable().ajax.reload();
                } else {
                    $('#uploadError').html(response.message);
                    displayToastr('Scorecards not uploaded', "Upload Scorecards", "Error");
                }
                hideLoading();
            },
            error: function (jqXHR, textStatus, errorThrown) {
                displayToastr('Scorecards not uploaded', "Upload Scorecards", "Error");
                hideLoading();
            }
        });
    }
}

function validateuploadScorecard() {
    $('#file-upload-scorecard-error').html('');
    $('#file-upload-documents-error').html('');

    var isValid = true;
    if ($('#file-upload-scorecard').get(0).files.length == 0) {
        $('#file-upload-scorecard-error').html('Please upload scorecard file');
        isValid = false;
    }
    if ($('#file-upload-documents').get(0).files.length == 0) {
        $('#file-upload-documents-error').html('Please upload document file');
        isValid = false;
    }
    return isValid;
}