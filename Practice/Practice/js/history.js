$(document).ready(function () {
    loadHistoryTable();
});

function convertDate(x,display) {
    var value = new Date(parseInt(x.substr(6)));
    var year = value.getFullYear();
    var month = value.getMonth() + 1;
    var day = value.getDate();

    if (month.toString().length == 1)
        month = "0".concat(month);

    if (day.toString().length == 1)
        day = "0".concat(day);

    if (display == true)
        var date = year + "/" + month + "/" + day;
    else
        // other date format to convert to DateTime in HistoryController
        var date = day + "/" + month + "/" + year;

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
                    return convertDate(data,true);
                }
            },
            {
                "data": "EndDate",
                "render": function (data) {
                    if (data == null)
                        return "Still working";

                    return convertDate(data,true);
                }
            },
            {
                "data": function (data, type, dataToSet) {
                    var deleteButton = '<button type="button" class="btn btn-sm btn-danger ml-1" data-toggle="modal" data-target="#deleteHistoryModal" data-id="' + data.Id + '" data-StartDate="' + convertDate(data.StartDate,false) + '" data-Department="' + data.Department + '"><i class="far fa-trash-alt"></i></button>';
                    return deleteButton;
                }
            }
        ],
        "columnDefs": [
            { "targets": 0 },
            { "targets": 1 },
            { "targets": 2 },
            { "targets": 3 },
            { "targets": 4 },
            { "targets": 5, "sortable": false },
            { "targets": 6, "sortable": false }
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

$('#deleteHistoryModal').on('show.bs.modal', function (event) {
    var button = $(event.relatedTarget);
    var id = button.data('id');
    var department = button.data('department');
    var startDate = button.data('startdate');
    let modal = $(this);

    $.ajax({
        type: "POST",
        url: "/History/GetHistory/",
        data: {
            id: id,
            department: department,
            startDate: startDate
        },
        success: function (result) {
            if (result.status == "success") {
                modal.find('#hiddenId').val(id);
                modal.find('#hiddenDate').val(startDate);
                modal.find('#hiddenDepartment').val(department);
                modal.find('.modal-body').text('Do you want to delete selected row?');
            } else {
                toastr["error"](result.message, "Error");
            }
        },
        error: function (result) {
            toastr["error"]("Oops. Something went wrong. Try again.", "Error");
        }
    });

});

$("#deleteHistoryButton").click(function (e) {
    e.preventDefault();
    let modal = $("#deleteHistoryModal");
    $.ajax({
        type: "POST",
        url: "/History/Delete/",
        data: {
            id: modal.find("#hiddenId").val(),
            department: modal.find("#hiddenDepartment").val(),
            startDate: modal.find("#hiddenDate").val()
        },
        success: function (result) {
            if (result.status == "success") {
                loadHistoryTable();
                modal.modal("hide");
                toastr["success"](result.message, "Success");
            } else {
                toastr["error"](result.message, "Error");
            }
        },
        error: function (result) {
            toastr["error"]("Oops. Something went wrong. Try again.", "Error");
        }
    });
});