var schemeBaseUrl = `${$('#websiteBaseUrl').val()}/Scheme`;

$(document).ready(function () {
    $("#schemeDatatable").DataTable({
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
            "url": `${schemeBaseUrl}/GetSchemes`,
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
                "data": "schemeId", "name": "schemeId",
                render: function (row, type, data, meta) {
                    return meta.row + 1;
                },
            },
            {
                render: function (row, type, data) {
                    return `<a href='./Scheme/Manage/${data.schemeId}'>${data.schemeName}</a>`;
                },
            },
            { "data": "schemeCode", "name": "schemeCode", "autoWidth": true },
            { "data": "duration", "name": "duration", "autoWidth": true },
            {
                render: function (row, type, data) {
                    return data.institutes.map(i => i).join('<br/>\r');
                }
            },
        ]
    });
    if ($("#SchemeId").length && +$("#SchemeId").val() > 0) {
        loadInstitute($("#SchemeId").val());
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
});

function clearFilters() {
    $('#searchValue').val('');
    $('#instituteId').val('').trigger('change');
}

function filterTable() {
    $(`#schemeDatatable`).DataTable().ajax.reload();
}

function getFilterParams(d) {
    d.searchValue = $('#searchValue').val();
    d.instituteId = $('#instituteId').val();
}

function resetFrom(formId) {
    $(`#${formId}`).trigger("reset");
}
function addInstitute() {
    var instituteId = $("#InstituteId").find("option:selected").val();
    if (instituteId == '') {
        displayToastr('Please select institute', 'Institute', 'Error');
        return;
    }

    showLoading();
    $.ajax({
        url: `${schemeBaseUrl}/AddSchemeInstitute`,
        type: 'POST',
        data: { schemeId: $("#SchemeId").val(), instituteId },
        success: function (result) {
            hideLoading();
            if (result.success) {
                displayToastr(result.message, 'Institute', 'Success');
                loadInstitute($("#SchemeId").val());
            } else {
                displayToastr(result.message, 'Institute', 'Error');
            }
        },
        error: function (error) {
            hideLoading();
        },
    });
}

function removeInstitute(schemeInstituteId) {
    if (confirm("Are you sure to delete institute?")) {
        showLoading();
        $.ajax({
            url: `${schemeBaseUrl}/RemoveSchemeInstitute`,
            type: 'POST',
            data: { schemeInstituteId },
            success: function (result) {
                hideLoading();
                if (result.success) {
                    displayToastr(result.message, 'Institute', 'Success');
                    loadInstitute($("#SchemeId").val());
                } else {
                    displayToastr(result.message, 'Institute', 'Error');
                }
            },
            error: function (error) {
                hideLoading();
            },
        });
    }
}

function loadInstitute(schemeId) {
    showLoading();
    $.ajax({
        url: `${schemeBaseUrl}/GetSchemeInstitutes`,
        type: 'POST',
        data: { schemeId },
        success: function (result) {
            hideLoading();
            $("#institute-list").html(result);
            $("#InstituteId").val("").trigger("change")

        },
        error: function (error) {
            hideLoading();
        },
    });
}



function validateScheme() {
    $("#institutes-error").hide();
    var institutes = $("#institutes").val().reduce((acc, x) => acc.concat(+x), []);
    $('#SelectedInstitutes').val(institutes.join());

    if ($("#formScheme").valid()) {
        if (institutes.length === 0) {
            $("#institutes-error").show();
            return false;
        }
        showLoading();
        return true;
    } else {
        if (institutes.length === 0) {
            $("#institutes-error").show();
        }
    }
    return false;
}
