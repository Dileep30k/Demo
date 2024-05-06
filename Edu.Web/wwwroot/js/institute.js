var instituteBaseUrl = `${$('#websiteBaseUrl').val()}/Institute`;

$(document).ready(function () {
    $("#instituteDatatable").DataTable({
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
            "url": `${instituteBaseUrl}/GetInstitutes`,
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
                "data": "instituteId", "name": "instituteId",
                render: function (row, type, data, meta) {
                    return meta.row + 1;
                },
            },
            {
                "data": "instituteName", "name": "instituteName",
                render: function (row, type, data) {
                    return `<a href='./Institute/Manage/${data.instituteId}'>${data.instituteName}</a>`;
                }
            },
            {
                render: function (row, type, data) {
                    return data.schemes.join('<br/>\r');
                }
            },
            { "data": "emailAddress", "name": "emailAddress", "autoWidth": true },
            { "data": "contactNo", "name": "contactNo", "autoWidth": true },
            {
                render: function (row, type, data) {
                    return data.intakes.map(i => i.intakeName).join('<br/>\r');
                }
            },
            {
                render: function (row, type, data) {
                    return data.intakes.map(i => i.totalSeats).join('<br/>\r');
                }
            },
            {
                "data": "instituteName", "name": "instituteName",
                render: function (row, type, data) {
                    return `<a href='${$('#websiteBaseUrl').val()}/Batch?id=${data.instituteId}'>View Batch Details</a>`;
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
});

function clearFilters() {
    $('#searchValue').val('');
    $('#schemeId').val('').trigger('change');
}

function filterTable() {
    $(`#instituteDatatable`).DataTable().ajax.reload();
}

function getFilterParams(d) {
    d.searchValue = $('#searchValue').val();
    d.schemeId = $('#schemeId').val();
}

function validateInstitute() {
    if ($("#instituteForm").valid()) {
        showLoading();
        return true;
    }
    return false;
}
