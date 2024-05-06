var eligibilityBaseUrl = `${$('#websiteBaseUrl').val()}/Eligibility`;
var selectedFile = null;

$(document).ready(function () {
    $("#eligibilityDatatable").DataTable({
        "processing": true,
        "serverSide": true,
        "filter": false,
        "bLengthChange": false,
        "bPaginate": true,
        "scrollX": true,
        "bInfo": false,
        "fnDrawCallback": function (oSettings) {
            $('#btn-publish').prop('disabled', !this.fnGetData().some(g => !g.isPublish));
            $('#btn-eligibility').prop('disabled', this.fnGetData().some(g => g.isPublish));
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
            "url": `${eligibilityBaseUrl}/GetEligibilities`,
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
            {
                "data": "intakeStartDate", "name": "intakeStartDate",
                render: function (row, type, data) {
                    return getFormatDate(data.intakeStartDate);
                }
            },
            {
                "data": "doj", "name": "doj",
                render: function (row, type, data) {
                    return getFormatDate(data.doj);
                }
            },
            { "data": "msilTenure", "name": "msilTenure", "autoWidth": true },
            { "data": "designation", "name": "designation", "autoWidth": true },
            { "data": "relevantPrevExp", "name": "relevantPrevExp", "autoWidth": true },
            { "data": "division", "name": "division", "autoWidth": true },
            { "data": "department", "name": "department", "autoWidth": true },
            { "data": "location", "name": "location", "autoWidth": true },
            {
                "data": "isPublish", "name": "isPublish",
                render: function (row, type, data) {
                    return data.isPublish ? 'Yes' : 'No';
                }
            },
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

    $("#uploadFile").on('change', function (e) {
        uploadData(e.target.files[0]);
    });
});

function openUpload() {
    $("#uploadFile").trigger('click');
}

function uploadData(file) {
    var schemeId = $('#schemeId').val();
    if (schemeId == '') {
        displayToastr('Please select Scheme!', "Upload File", "Error");
        $('#uploadFile').val(null);
        return;
    }
    var intakeId = $('#intakeId').val();
    if (intakeId == '') {
        displayToastr('Please select Financial Year!', "Upload File", "Error");
        $('#uploadFile').val(null);
        return;
    }

    if (!file || !(/\.(xlsx|xls)$/i).test(file.name)) {
        displayToastr('Please upload only Excel file!', "Upload File", "Error");
        $('#uploadFile').val(null);
        return;
    }

    var formData = new FormData();
    formData.append('file', file);
    formData.append('schemeId', schemeId);
    formData.append('intakeId', intakeId);
    selectedFile = file;

    $('#uploadError').html('');
    $('#eligibilityUserDiv').hide();
    $('#eligibilityUserDatatable').DataTable().destroy();

    showLoading();
    $.ajax({
        type: 'POST',
        url: `${eligibilityBaseUrl}/UploadEligibilities`,
        data: formData,
        cache: false,
        contentType: false,
        processData: false,
        success: function (response) {
            if (response.success) {
                $('#eligibilityUserDatatable').DataTable({
                    "data": response.data,
                    "bLengthChange": false,
                    "bPaginate": true,
                    "filter": false,
                    "columns": [
                        { "data": "vertical", "name": "vertical", "autoWidth": true },
                        { "data": "msilUserId", "name": "msilUserId", "autoWidth": true },
                        { "data": "staffName", "name": "staffName", "autoWidth": true },
                        {
                            "data": "doj", "name": "doj",
                            render: function (row, type, data) {
                                return getFormatDate(data.doj);
                            }
                        },
                        {
                            "data": "msilTenure", "name": "msilTenure",
                            render: function (row, type, data) {
                                return data.msilTenure.toFixed(2);
                            }
                        },
                        { "data": "designation", "name": "designation", "autoWidth": true },
                        //{ "data": "grade", "name": "grade", "autoWidth": true },
                        { "data": "relevantPrevExp", "name": "relevantPrevExp", "autoWidth": true },
                        { "data": "division", "name": "division", "autoWidth": true },
                        { "data": "department", "name": "department", "autoWidth": true },
                        { "data": "location", "name": "location", "autoWidth": true },
                        { "data": "approver1", "name": "approver1", "autoWidth": true },
                        { "data": "approver2", "name": "approver2", "autoWidth": true },
                    ]
                });
                $('#eligibilityUserDiv').show();
            } else {
                $('#uploadError').html(response.message);
                displayToastr("Eligibility List Upload Error", "Eligibility List Upload", "Error");
            }
            $('#uploadFile').val(null);
            hideLoading();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            $('#uploadFile').val(null);
            displayToastr(textStatus, "Eligibility List Upload", "Error");
            hideLoading();
        }
    });
}

function saveEligibilities() {
    var schemeId = $('#schemeId').val();
    if (schemeId == '') {
        displayToastr('Please select Scheme!', "Upload File", "Error");
        return;
    }
    var intakeId = $('#intakeId').val();
    if (intakeId == '') {
        displayToastr('Please select Financial Year!', "Upload File", "Error");
        return;
    }

    var formData = new FormData();
    formData.append('file', selectedFile);
    formData.append('schemeId', schemeId);
    formData.append('intakeId', intakeId);

    $('#uploadError').html('');
    showLoading();
    $.ajax({
        url: `${eligibilityBaseUrl}/SaveEligibilities`,
        type: 'POST',
        data: formData,
        cache: false,
        contentType: false,
        processData: false,
        success: function (result) {
            if (result.success) {
                selectedFile = null;
                displayToastr('Eligibility List Uploaded', "Eligibility", "Success");
                toggleContent();
                filterTable();
            } else {
                $('#uploadError').html(response.message);
                displayToastr('Eligibility List not Uploaded', "Eligibility", "Error");
            }
            hideLoading();
        },
        error: function (error) {
            displayToastr('Eligibility List not Uploaded', "Eligibility", "Error");
            hideLoading();
        },
    });
}

function publishEligibilities() {
    var schemeId = $('#schemeId').val();
    if (schemeId == '') {
        displayToastr('Please select Scheme!', "Upload File", "Error");
        return;
    }
    var intakeId = $('#intakeId').val();
    if (intakeId == '') {
        displayToastr('Please select Financial Year!', "Upload File", "Error");
        return;
    }
    $('#btn-publish').prop('disabled', true);
    showLoading();
    $.ajax({
        url: `${eligibilityBaseUrl}/PublishEligibilities`,
        type: 'POST',
        data: { schemeId, intakeId },
        success: function (result) {
            if (result.success) {
                displayToastr('Eligibility List Published', "Eligibility", "Success");
                filterTable();
            } else {
                displayToastr('Eligibility List not Published', "Eligibility", "Error");
            }
            hideLoading();
        },
        error: function (error) {
            displayToastr('Eligibility List  not Published', "Eligibility", "Error");
            hideLoading();
            $('#btn-publish').prop('disabled', false);
        },
    });
}

function getSchemeIntakes(obj) {
    $('#intakeId').html('');
    var schemeId = $(obj).val();
    showLoading();
    $.ajax({
        url: `${eligibilityBaseUrl}/GetSchemeIntakes`,
        type: 'POST',
        data: { schemeId },
        success: function (result) {
            if (result) {
                $('#intakeId').append($(`<option></option>`).val('').html('All'));
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

function getFilterParams(d) {
    d.searchValue = $('#searchValue').val();
    d.startDate = $('#startDate').val();
    d.endDate = $('#endDate').val();
    d.schemeId = $('#schemeId').val();
    d.intakeId = $('#intakeId').val();
    d.designationId = $('#designationId').val();
    d.divisionId = $('#divisionId').val();
    d.departmentId = $('#departmentId').val();
    d.locationId = $('#locationId').val();
}

function clearFilters() {
    $('#searchValue').val('');
    $('#startDate').val('');
    $('#endDate').val('');
    $('#designationId').val('').trigger('change');
    $('#divisionId').val('').trigger('change');
    $('#departmentId').val('').trigger('change');
    $('#locationId').val('').trigger('change');
}

function filterTable() {
    $('#uploadError').html('');
    $(`#eligibilityDatatable`).DataTable().ajax.reload();
}

function toggleContent(isUpload = false) {
    $('#uploadError').html('');
    if (isUpload) {
        $('.list-content').hide();
        $('.upload-content').show();
    } else {
        $('#eligibilityUserDatatable').DataTable().destroy();
        $('#eligibilityUserDiv').hide();
        $('.upload-content').hide();
        $('.list-content').show();
    }
}