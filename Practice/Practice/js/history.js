$(document).ready(function () {
    loadHistoryTable();
});

function convertDate(x) {
    var value = new Date(parseInt(x.substr(6)));
    var year = value.getFullYear();
    var month = value.getMonth() + 1;
    var day = value.getDate();

    if (month.toString().length == 1)
        month = "0".concat(month);

    if (day.toString().length == 1)
        day = "0".concat(day);

    var date = year + "/" + month + "/" + day;

    return date;
}

function loadHistoryTable() {
    $('#table').DataTable().destroy();
    var tab = $('#table').DataTable({
        "bServerSide": true,
        "bProcessing": true,
        "searching": false,
        "ajax": {
            url: '/History/GetAllHistoriesAsync',
            type: 'POST'
        },
        paging: true,
        ordering: true,
        bAutoWidth: false,
        "searching": true,
        "pageLength": 25,
        "order": [[0, "asc"]],
        "columns": [
            { "data": "Id" },
            { "data": "FirstName" },
            { "data": "LastName" },
            { "data": "Department" },
            {
                "data": "StartDate",
                "render": function (data) {
                    return convertDate(data);
                }
            },
            {
                "data": "EndDate",
                "render": function (data) {
                    if (data == null)
                        return "Still working";

                    return convertDate(data);
                }
            },
            {
                "data": "Actions",
                "className": "text-center",
                "render": function (data) {
                    var editButton = '<button type="button" class="btn btn-sm btn-warning" data-toggle="modal" data-target="#" data-id="' + data + '"><i class="fas fa-edit"></i></button>';
                    var deleteButton = '<button type="button" class="btn btn-sm btn-danger ml-1" data-toggle="modal" data-target="#" data-id="' + data + '"><i class="far fa-trash-alt"></i></button>';
                    var actionButtons = editButton + deleteButton;
                    return actionButtons;

                }
            }
        ],
        "columnDefs": [
            { "targets": 0 },
            { "targets": 1 },
            { "targets": 2 },
            { "targets": 3 },
            { "targets": 4 },
            { "targets": 5, "sortable": false},
            { "targets": 6, "sortable": false}
        ],
        "bDestroy": true,
        "iDisplayLength": 25,
        "lengthMenu": [[5, 10, 25, 50, 100], [5, 10, 25, 50, 100]],
    });
    $('#table_filter input').unbind();
    $('#table_filter input').bind('keyup', function (e) {
        if (e.keyCode == 13) {
            tab.search(this.value).draw();
        }
    });
}