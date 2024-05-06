var batchBaseUrl = `${$('#websiteBaseUrl').val()}/Batch`;

$(document).ready(function () {
    $("#batchDatatable").DataTable({
        "processing": true,
        "serverSide": true,
        "filter": false,
        "bLengthChange": false,
        "bPaginate": true,
        "scrollX": true,
        "bInfo": false,
        "ajax": {
            "url": `${batchBaseUrl}/GetBatches`,
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
                "data": "batchId", "name": "batchId",
                render: function (row, type, data, meta) {
                    return meta.row + 1;
                },
            },
            {
                render: function (row, type, data) {
                    return `${data.batchCode}`;
                },
            },
            {
                "data": "startDate", "name": "startDate",
                render: function (row, type, data) {
                    return getFormatDate(data.startDate);
                }
            },
            { "data": "academicYears", "name": "academicYears", "autoWidth": true },
            { "data": "schemeName", "name": "schemeName", "autoWidth": true },
            { "data": "intakeName", "name": "intakeName", "autoWidth": true },
            { "data": "instituteName", "name": "instituteName", "autoWidth": true },
            { "data": "totalSeats", "name": "totalSeats", "autoWidth": true },
            { "data": "totalFee", "name": "totalFee", "autoWidth": true },
            {
                render: function (row, type, data) {
                    return `<a href='javascript:void(0)'>EmployeeList.pdf</a>`;
                },
            },
            {
                render: function (row, type, data) {
                    return `<a href='javascript:void(0)'>Add Payment</a>`;
                },
            },

        ]
    });
});

function validateBatch() {
    if ($("#batchForm").valid()) {
        showLoading();
        return true;
    }
    return false;
}

function getFilterParams(d) {
    d.instituteId = getUrlParameter('id');
}