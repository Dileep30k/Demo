var admissionBaseUrl = `${$('#websiteBaseUrl').val()}/Admission`;

var admission = null;
var admissionUserId = null;
var selectedFile = null;
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

    $("#admissionAllDatatable").DataTable({
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
            "url": `${admissionBaseUrl}/GetAdmissionUsers`,
            "type": "POST",
            "datatype": "json",
            "data": function (d) { return getAllUserFilterParams(d); }
        },
        "columnDefs": [{
            "targets": 'no-sort',
            "orderable": false,
        }],
        "columns": [
            {
                "data": "admissionStatusId", "name": "admissionStatusId",
                render: function (row, type, data) {
                    return `<span class="dot tooltipeds"  style="color: ${getAdmissionStatusTextColor(data.admissionStatusId)} ; background-color: ${getAdmissionStatusColor(data.admissionStatusId)}">${getAdmissionStatusText(data.admissionStatusId)} <div> <div class="tooltiptexteds">${getAdmissionStatusTextHover(data.admissionStatusId)}</div></div> </span>  <span class="display-none">${data.status}</span>`;
                }
            },
            { "data": "rank", "name": "rank", "autoWidth": true },
            { "data": "msilUserId", "name": "msilUserId", "autoWidth": true },
            { "data": "staffName", "name": "staffName", "autoWidth": true },
            { "data": "email", "name": "email", "autoWidth": true },
            { "data": "mobileNo", "name": "mobileNo", "autoWidth": true },
            {
                "data": "intakeStartDate", "name": "intakeStartDate",
                render: function (row, type, data) {
                    return getFormatDate(data.intakeStartDate);
                }
            },
            {
                "data": "isConfirmByEmp", "name": "isConfirmByEmp",
                render: function (row, type, data) {
                    return `${data.isConfirmByEmp == null ? 'NA' : (data.isConfirmByEmp ? 'Accepted' : 'Rejected')}`;
                }
            },
            { "data": "employeeRemarks", "name": "employeeRemarks", "autoWidth": true },
            {
                "data": "isConfirmByInstitute", "name": "isConfirmByInstitute",
                render: function (row, type, data) {
                    return `${data.isConfirmByInstitute == null ? 'NA' : (data.isConfirmByInstitute ? 'Yes' : 'No')}`;
                }
            },
            {
                render: function (row, type, data) {
                    return data.documents.map(doc => `<a href="${doc.filePath}" target="_blank">${doc.fileName}</a>`).join('<br/>\r');
                }
            }
        ],
        "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
            if (aData.admissionStatusId == admissionStatuses.Waiting) {
                $(nRow).addClass('row-waiting');
            }
        }
    });

    $("#confirmAdmissionDatatable").DataTable({
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
                },
                exportOptions: {
                    columns: ':not(.notexport)'
                }
            },
        ],
        "ajax": {
            "url": `${admissionBaseUrl}/GetConfirmAdmissionUsers`,
            "type": "POST",
            "datatype": "json",
            "data": function (d) { return getConfirmUserFilterParams(d); }
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
            { "data": "msilUserId", "name": "msilUserId", "autoWidth": true },
            { "data": "staffName", "name": "staffName", "autoWidth": true },
            { "data": "email", "name": "email", "autoWidth": true },
            { "data": "mobileNo", "name": "mobileNo", "autoWidth": true },
            {
                render: function (row, type, data) {
                    var className = '';
                    if (data.admissionStatusId == admissionStatuses.Active) { className = 'confirm-active'; }
                    if (data.admissionStatusId == admissionStatuses.Left) { className = 'confirm-left'; }
                    if (data.admissionStatusId == admissionStatuses.Pause) { className = 'confirm-pause'; }
                    return `<span class='${className}'>${data.status}</span>`;
                }
            },
            { "data": "semester", "name": "semester", "autoWidth": true },
            { "data": "department", "name": "department", "autoWidth": true },
            { "data": "division", "name": "division", "autoWidth": true },
            { "data": "vertical", "name": "vertical", "autoWidth": true },
            { "data": "location", "name": "location", "autoWidth": true },
            { "data": "reportingManager", "name": "reportingManager", "autoWidth": true },
            { "data": "remarks", "name": "remarks", "autoWidth": true },
            {
                render: function (row, type, data) {
                    if (data.admissionStatusId == admissionStatuses.Left || data.admissionStatusId == admissionStatuses.Pause) {
                        return data.documents.map(doc => `<a href="${doc.filePath}" target="_blank">${doc.fileName}</a>`).join('<br/>\r');
                    }

                    return `<a href='javascript:void(0)' onclick="takeAction(${data.admissionUserId})">Take Action</a>`;
                }
            }
        ]
    });
    $('.dropZone').on('dragenter', function (e) {
        e.stopPropagation();
        e.preventDefault();
        $(".dropZone").addClass('highlight-zone');
    }).on('dragover', function (e) {
        e.stopPropagation();
        e.preventDefault();
        $(".dropZone").addClass('highlight-zone');
    }).on('dragleave', function (e) {
        e.stopPropagation();
        e.preventDefault();
        $(".dropZone").removeClass('highlight-zone');
    }).on('drop', function (e) {
        e.stopPropagation();
        e.preventDefault();
        $(".dropZone").removeClass('highlight-zone');
        if (e.originalEvent.dataTransfer.files.length > 1) {
            displayToastr('Please upload only one file!', "Upload File", "Error");
            return;
        }
        if ($(this).attr('id') == 'dropZoneAdmission') {
            uploadData(e.originalEvent.dataTransfer.files[0]);
        }
        if ($(this).attr('id') == 'dropZoneConfirm') {
            uploadData(e.originalEvent.dataTransfer.files[0]);
        }
    });

    $("#uploadFile").on('change', function (e) {
        uploadData(e.target.files[0]);
    });

    $("#uploadFileConfirm").on('change', function (e) {
        uploadConfirmData(e.target.files[0]);
    });

    $("#btnFilter").click(function (e) {
        e.preventDefault();
        filterAllTable();
    });

    $("#btnFilterClear").click(function (e) {
        e.preventDefault();
        clearAllFilters();
        filterAllTable();
    });
    $("#searchValue").blur(function () {
        filterAllTable();
    });

    $("#searchConfirmValue").blur(function () {
        filterConfirmTable();
    });

    $("#updateStatusFile").on('change', function (e) {
        if (e.target.files.length > 0) {
            $('#updateStatusFileError').hide();
        }
    });

    clearAllFilters();
});

function openUpload() {
    $("#uploadFile").trigger('click');
}

function openUploadConfirm() {
    $("#uploadFileConfirm").trigger('click');
}

function uploadData(file) {
    if (!file || !(/\.(xlsx|xls)$/i).test(file.name)) {
        displayToastr('Please upload only excel file!', "Upload File", "Error");
        return;
    }

    var formData = new FormData();
    formData.append('file', file);
    formData.append('schemeId', $('#schemeId').val());
    formData.append('intakeId', $('#intakeId').val());
    formData.append('instituteId', $('#instituteId').val());
    selectedFile = file;

    $('#uploadError').html('');
    $('#admissionUserDiv').hide();
    $('#admissionUserDatatable').DataTable().destroy();

    showLoading();
    $.ajax({
        type: 'POST',
        url: `${admissionBaseUrl}/UploadAdmissions`,
        data: formData,
        cache: false,
        contentType: false,
        processData: false,
        success: function (response) {
            if (response.success) {
                $('#admissionUserDatatable').DataTable({
                    "data": response.data,
                    "bLengthChange": false,
                    "bPaginate": true,
                    "filter": false,
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
                $('#admissionUserDiv').show();
            } else {
                $('#uploadError').html(response.message);
                displayToastr("Nomination List Upload Error", "Admissions Upload", "Error");
            }
            hideLoading();
            $('#uploadFile').val(null);
        },
        error: function (jqXHR, textStatus, errorThrown) {
            $('#uploadFile').val(null);
            displayToastr(textStatus, "Admissions Upload", "Error");
            hideLoading();
        }
    });
}

function getSchemeIntakes(obj) {
    $('.grid-content').hide();
    $('#intakeId').html('');
    $('#instituteId').html('');
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

function getBatches() {
    $.ajax({
        url: `${admissionBaseUrl}/GetBatches`,
        type: 'POST',
        data: { schemeId: $('#schemeId').val(), intakeId: $('#intakeId').val(), instituteId: $('#instituteId').val() },
        success: function (result) {
            if (result.length == 0) {
                $('#batchId').html($(`<option></option>`).val('').html('N/A'));
            } else {
                $('#batchId').html('');
                $.each(result, function (index, value) {
                    $('#batchId').append($(`<option></option>`).val(value.id).html(value.text));
                });
            }
        },
        error: function (error) {
            displayToastr(error.data, "Batch Detail", "Error");
        },
    });
}

function getAdmission(obj) {
    admission = null;
    $('.grid-content').hide();
    $('.no-selected-institute').hide();
    $('.selected-institute').hide();
    $('#approver1').removeClass('selected rejected lineSelected');
    $('#approver2').removeClass('selected rejected lineSelected');
    toggleContent();
    var instituteId = $(obj).val();
    if (instituteId == '') {
        $('.no-selected-institute').show();
        return;
    }
    $('#uploadTitle').html(`Scheme: ${$('#schemeId').find("option:selected").text()} - Financial Year: ${$('#intakeId').find("option:selected").text()} - Institue: ${$('#instituteId').find("option:selected").text()}`);
    showLoading();
    $.ajax({
        url: `${admissionBaseUrl}/GetAdmission`,
        type: 'POST',
        data: { schemeId: $('#schemeId').val(), intakeId: $('#intakeId').val(), instituteId: $('#instituteId').val(), isGts: true },
        success: function (result) {
            if (result.success) {
                admission = result.data;
                if (admission.isPublish) {
                    displayAllAdmission();
                    if (admission.isConfirmUpload) {
                        filterConfirmTable();
                    } else {
                        $('.confirm-upload-content').show();
                        $('.confirm-list-content').hide();
                    }
                } else {
                    if (admission.approved1) {
                        $('#approver1').addClass('selected ');
                        $('#approver1').addClass('lineSelected');

                        $('#lineApprover').addClass('lineSelected');
                        $('#lineApprover1').addClass('lineSelected');

                    }
                    else if (admission.approved1 == false) {
                        $('#approver1').addClass('rejected ');
                        $('#approver1').addClass('lineSelected');
                        $('#lineApprover1').addClass('lineSelected');
                    }
                    if (admission.approved2) {
                        $('#approver2').addClass('selected');
                        $('#approver2').addClass('lineSelected');
                        $('#lineApprover2').addClass('lineSelected');
                    } else if (admission.approved2 == false) {
                        $('#approver2').addClass('rejected ');
                        $('#approver2').addClass('lineSelected');
                        $('#lineApprover2').addClass('lineSelected');
                    }
                    if (admission.approved1 && admission.approved2) {
                        $('#btnPublish').attr('disabled', false);
                    } else {
                        $('#btnPublish').attr('disabled', true);
                    }
                    if (admission.approved1 == false || admission.approved2 == false) {
                        $('#btnPublish').hide();
                        $('#btnRePublish').show();
                    } else {
                        $('#btnPublish').show();
                        $('#btnRePublish').hide();
                    }
                    filterTable();
                }
            } else {
                $('.selected-institute').show();
            }
            hideLoading();
        },
        error: function (error) {
            displayToastr(error.data, "Admission Detail", "Error");
            hideLoading();
        },
    });

    getBatches();
}

function saveAdmissions() {
    var formData = new FormData();
    formData.append('file', selectedFile);
    formData.append('schemeId', $('#schemeId').val());
    formData.append('intakeId', $('#intakeId').val());
    formData.append('instituteId', $('#instituteId').val());
    if (admission && (admission.approved1 == false || admission.approved2 == false)) {
        formData.append('admissionId', admission.admissionId);
    } else {
        formData.append('admissionId', 0);
    }

    showLoading();
    $.ajax({
        url: `${admissionBaseUrl}/SaveAdmissions`,
        type: 'POST',
        data: formData,
        cache: false,
        contentType: false,
        processData: false,
        success: function (result) {
            if (result.success) {
                selectedFile = null;
                displayToastr('Nomination List Uploaded', "Admission", "Success");
                getAdmission();
            } else {
                displayToastr('Nomination List  not Uploaded', "Admission", "Error");
            }
            hideLoading();
        },
        error: function (error) {
            displayToastr('Nomination List not Uploaded', "Admission", "Error");
            hideLoading();
        },
    });
}

function publishAdmissions() {
    showLoading();
    $.ajax({
        url: `${admissionBaseUrl}/PublishAdmissions`,
        type: 'POST',
        data: admission,
        success: function (result) {
            if (result.success) {
                displayToastr('Nomination List published', "Admission", "Success");
                displayAllAdmission();
                $('.confirm-upload-content').show();
                $('.confirm-list-content').hide();
            } else {
                displayToastr('Nomination List not published', "Admission", "Error");
            }
            hideLoading();
        },
        error: function (error) {
            displayToastr('Nomination List not published', "Admission", "Error");
            hideLoading();
        },
    });
}

function rePublishAdmissions() {
    toggleContent(true);
}

function getFilterParams(d) {
    d.schemeId = $('#schemeId').val();
    d.intakeId = $('#intakeId').val();
    d.instituteId = $('#instituteId').val();
    d.isGts = true;
}

function filterTable() {
    $('.list-content').hide();
    $('.grid-content').show();
    $(`#admissionDatatable`).DataTable().ajax.reload();
    resizeDataTable();
}

function toggleContent(isUpload = false) {
    $('.grid-content').hide();
    $('#uploadTitle').html();
    $('.publish-content').hide();
    if (isUpload) {
        $('.list-content').hide();
        $('.upload-content').show();
    } else {
        $('#admissionUserDatatable').DataTable().destroy();
        $('#admissionUserDiv').hide();
        $('.upload-content').hide();
        $('.list-content').show();
        if (admission && (admission.approved1 == false || admission.approved2 == false)) {
            $('.grid-content').show();
        }
    }
}

function displayAllAdmission() {
    $('#btnPublish').attr('disabled', true);
    $('.grid-content').hide();
    $('.upload-content').hide();
    $('.list-content').hide();
    $('.publish-content').show();
    filterAllTable();
}

function filterAllTable() {
    $(`#admissionAllDatatable`).DataTable().ajax.reload();
    resizeDataTable();
}

function getAllUserFilterParams(d) {
    d.searchValue = $('#searchValue').val();
    d.startDate = $('#startDate').val();
    d.endDate = $('#endDate').val();
    d.schemeId = $('#schemeId').val();
    d.intakeId = $('#intakeId').val();
    d.instituteId = $('#instituteId').val();
    d.admissionStatusId = $('#admissionStatusId').val();
    d.serviceBondAccepted = $('#serviceBondAccepted').val();
}

function clearAllFilters() {
    $('#searchValue').val('');
    $('#startDate').val('');
    $('#endDate').val('');
    $('#admissionStatusId').val('').trigger('change');
    $('#serviceBondAccepted').val('').trigger('change');
}

function exportAllAdmission() {
    $('#admissionAllDatatable_wrapper .buttons-excel').click()
}

function uploadConfirmData(file) {
    if (!file || !(/\.(xlsx|xls)$/i).test(file.name)) {
        displayToastr('Please upload only excel file!', "Upload File", "Error");
        return;
    }

    var formData = new FormData();
    formData.append('file', file);
    formData.append('admissionId', admission.admissionId);
    selectedFile = file;

    $('#uploadErrorConfirm').html('');
    $('#confirmUserDiv').hide();
    $('#confirmUserDatatable').DataTable().destroy();

    showLoading();
    $.ajax({
        type: 'POST',
        url: `${admissionBaseUrl}/UploadConfirmAdmissions`,
        data: formData,
        cache: false,
        contentType: false,
        processData: false,
        success: function (response) {
            if (response.success) {
                $('#confirmUserDatatable').DataTable({
                    "data": response.data,
                    "bLengthChange": false,
                    "bPaginate": true,
                    "filter": false,
                    "columns": [
                        {
                            render: function (row, type, data, meta) {
                                return meta.row + 1;
                            },
                        },
                        { "data": "msilUserId", "name": "msilUserId", "autoWidth": true },
                        { "data": "staffName", "name": "staffName", "autoWidth": true },
                        { "data": "email", "name": "email", "autoWidth": true },
                        { "data": "mobileNo", "name": "mobileNo", "autoWidth": true },
                    ]
                });
                $('#confirmUserDiv').show();
            } else {
                $('#uploadErrorConfirm').html(response.message);
                displayToastr("Confirm Admissions Upload Error", "Admissions Upload", "Error");
            }
            $('#uploadFileConfirm').val(null);
            hideLoading();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            $('#uploadFileConfirm').val(null);
            displayToastr(textStatus, "Confirm Admissions Upload", "Error");
            hideLoading();
        }
    });
}

function saveConfirmAdmissions() {
    var formData = new FormData();
    formData.append('file', selectedFile);
    formData.append('admissionId', admission.admissionId);
    showLoading();
    $.ajax({
        url: `${admissionBaseUrl}/SaveConfirmAdmissions`,
        type: 'POST',
        data: formData,
        cache: false,
        contentType: false,
        processData: false,
        success: function (result) {
            if (result.success) {
                selectedFile = null;
                displayToastr('Admissions Confirmed', "Admission", "Success");
                filterConfirmTable();
                getBatches();
            } else {
                displayToastr(result.message, "Admission", "Error");
            }
            hideLoading();
        },
        error: function (error) {
            displayToastr('Admissions not confirmed', "Admission", "Error");
            hideLoading();
        },
    });
}

function getConfirmUserFilterParams(d) {
    d.schemeId = $('#schemeId').val();
    d.intakeId = $('#intakeId').val();
    d.instituteId = $('#instituteId').val();
    d.searchValue = $('#searchConfirmValue').val();
}

function filterConfirmTable() {
    $('#confirmUserDiv').hide();
    $('#confirmUserDatatable').DataTable().destroy();
    $('.confirm-upload-content').hide();
    $('.confirm-list-content').show();
    $(`#confirmAdmissionDatatable`).DataTable().ajax.reload();
}

function exportConfirmAdmission() {
    $('#confirmAdmissionDatatable_wrapper .buttons-excel').click()
}

function takeAction(id) {
    admissionUserId = id;
    $('#updateAdmissionStatusId').val('').trigger('change');
    $('#updateStatusRemarks').val('');
    $('#updateStatusFile').val(null);
    $('#updateStatusError').hide();
    $('#updateStatusRemarksError').hide();
    $('#updateStatusFileError').hide();
    displayModel('actionModal');
}

function updateAdmissionUserStatus() {
    if (validateUpdateAdmissionUserStatus()) {

        var formData = new FormData();
        formData.append('file', $('#updateStatusFile').get(0).files[0]);
        formData.append('admissionUserId', admissionUserId);
        formData.append('admissionStatusId', $('#updateAdmissionStatusId').val());
        formData.append('remarks', $('#updateStatusRemarks').val());

        showLoading();
        $.ajax({
            url: `${admissionBaseUrl}/UpdateAdmissionUserStatus`,
            type: 'POST',
            data: formData,
            cache: false,
            contentType: false,
            processData: false,
            success: function (result) {
                if (result.success) {
                    admissionUserId = null;
                    displayToastr('Admissions Updated', "Admission", "Success");
                    hideModel('actionModal');
                    filterAllTable();
                    filterConfirmTable();
                } else {
                    displayToastr('Admissions not updated', "Admission", "Error");
                }
                hideLoading();
            },
            error: function (error) {
                displayToastr('Admissions not updated', "Admission", "Error");
                hideLoading();
            },
        });
    }
}

function validateUpdateAdmissionUserStatus() {
    $('#updateStatusError').hide();
    $('#updateStatusRemarksError').hide();
    $('#updateStatusFileError').hide();
    var isValid = true;
    if ($('#updateAdmissionStatusId').val() == '') {
        $('#updateStatusError').show();
        isValid = false;
    }
    if ($('#updateStatusRemarks').val() == '') {
        $('#updateStatusRemarksError').show();
        isValid = false;
    }
    if ($('#updateStatusFile').get(0).files.length == 0) {
        $('#updateStatusFileError').show();
        isValid = false;
    }
    return isValid;
}