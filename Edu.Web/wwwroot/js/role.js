var roleBaseUrl = `${$('#websiteBaseUrl').val()}/Role`;

$(document).ready(function () {
    $("#roleDatatable").DataTable({
        "processing": true,
        "serverSide": true,
        "filter": true,
        "ajax": {
            "url": `${roleBaseUrl}/GetRoles`,
            "type": "POST",
            "datatype": "json"
        },
        "columnDefs": [{
            "targets": 'no-sort',
            "orderable": false,
        }],
        "columns": [
            {
                render: function (row, type, data) {
                    return `<a href='./Role/Manage/${data.roleId}'>${data.roleName}</a>`;
                },
            },
            { "data": "designations", "name": "designations", "autoWidth": true },
        ]
    });
    if ($("#RoleId").length && +$("#RoleId").val() > 0) {
        loadDesignation($("#RoleId").val());
    }
});

function resetFrom(formId) {
    $(`#${formId}`).trigger("reset");
}
function addDesignation() {
    var designationId = $("#DesignationId").find("option:selected").val();
    if (designationId == '') {
        displayToastr('Please select designation', 'Designation', 'Error');
        return;
    }

    showLoading();
    $.ajax({
        url: `${roleBaseUrl}/AddRoleDesignation`,
        type: 'POST',
        data: { roleId: $("#RoleId").val(), designationId },
        success: function (result) {
            if (result.success) {
                displayToastr(result.message, 'Designation', 'Success');
                loadDesignation($("#RoleId").val());
            } else {
                displayToastr(result.message, 'Designation', 'Error');
            }
            hideLoading();
        },
        error: function (error) {
            hideLoading();
        },
    });
}

function removeDesignation(roleDesignationId) {
    if (confirm("Are you sure to delete designation?")) {
        showLoading();
        $.ajax({
            url: `${roleBaseUrl}/RemoveRoleDesignation`,
            type: 'POST',
            data: { roleDesignationId },
            success: function (result) {
                if (result.success) {
                    displayToastr(result.message, 'Designation', 'Success');
                    loadDesignation($("#RoleId").val());
                } else {
                    displayToastr(result.message, 'Designation', 'Error');
                }
                hideLoading();
            },
            error: function (error) {
                hideLoading();
            },
        });
    }
}

function loadDesignation(roleId) {
    showLoading();
    $.ajax({
        url: `${roleBaseUrl}/GetRoleDesignations`,
        type: 'POST',
        data: { roleId },
        success: function (result) {
            $("#designation-list").html(result);
            $("#DesignationId").val("").trigger("change")
            hideLoading();
        },
        error: function (error) {
            hideLoading();
        },
    });
}
