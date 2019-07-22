$(document).ready(function () {
    loadEmployeesTable();
    $('#data-table-employees thead th').removeClass('text-center');
});

function loadEmployeesTable() {
    $('#data-table-employees').DataTable().destroy();
    var empTab = $('#data-table-employees').DataTable({
        "bServerSide": true,
        "bProcessing": true,
        "searching": true,
        "ajax": {
            url: '/Employee/GetAllEmployees',
            type: 'POST'
        },
        paging: true,
        ordering: true,
        bAutoWidth: false,
        "pageLength": 25,
        "order": [[1, "asc"]],
        "columns": [
            { "data": "FirstName" },
            { "data": "LastName" },
            { "data": "PhoneNumber" },
            { "data": "Address" },
            { "data": "PostalCode" },
            { "data": "City" },
            {
                "data": "Id",
                "className": "text-center",
                "render": function (data) {
                    var editButton = '<button type="button" class="btn btn-sm btn-warning" data-toggle="modal" data-target="#editEmployeeModal" data-id="' + data + '"><i class="fas fa-edit"></i></button>';
                    var deleteButton = '<button type="button" class="btn btn-sm btn-danger ml-1" data-toggle="modal" data-target="#deleteEmployeeModal" data-id="' + data + '"><i class="far fa-trash-alt"></i></button>';
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
            { "targets": 5 },
            { "targets": 6, "sortable": false }
        ],
        "bDestroy": true,
        "iDisplayLength": 25,
        "lengthMenu": [[5, 10, 25, 50, 100], [5, 10, 25, 50, 100]],
    });

    $('#data-table-employees_filter input').unbind();
    $('#data-table-employees_filter input').bind('keyup', function (e) {
        if (e.keyCode == 13) {
            empTab.search(this.value).draw();
        }
    });
}

$("#createEmployeeButton").click(function (e) {
    e.preventDefault();
    let modal = $("#createEmployeeModal");

    let validationSucceed = $("#createEmployeeForm").validate().form();
    if (validationSucceed == true) {
        $.ajax({
            type: "POST",
            url: "/Employee/Create/",
            data: {
                firstName: modal.find("#FirstName").val(),
                lastName: modal.find("#LastName").val(),
                phoneNumber: modal.find("#PhoneNumber").val(),
                address: modal.find("#Address").val(),
                postalCode: modal.find("#PostalCode").val(),
                city: modal.find("#City").val()
            },
            success: function (result) {
                if (result.status == "success") {
                    loadEmployeesTable();
                    $("#createEmployeeModal").modal("hide");
                    toastr["success"](result.message, "Success");

                    modal.find("#FirstName").val("");
                    modal.find("#LastName").val("");
                    modal.find("#PhoneNumber").val("");
                    modal.find("#Address").val("");
                    modal.find("#PostalCode").val("");
                    modal.find("#City").val("");
                } else {
                    toastr["error"](result.message, "Error");
                }
            },
            error: function (result) {
                toastr["error"]("Oops. Something went wrong. Try again.", "Error");
            }
        });
    }
});

$('#editEmployeeModal').on('show.bs.modal', function (event) {
    let button = $(event.relatedTarget);
    let id = button.data('id');
    let modal = $(this);

    $.ajax({
        type: "POST",
        url: "/Employee/GetEmployee/",
        data: {
            id: id
        },
        success: function (result) {
            if (result.status == "success") {
                modal.find('#Id').val(result.employee.Id);
                modal.find('#FirstName').val(result.employee.FirstName);
                modal.find('#LastName').val(result.employee.LastName);
                modal.find('#PhoneNumber').val(result.employee.PhoneNumber);
                modal.find('#Address').val(result.employee.Address);
                modal.find('#PostalCode').val(result.employee.PostalCode);
                modal.find('#City').val(result.employee.City);
            } else {
                toastr["error"](result.message, "Error");
            }
        },
        error: function (result) {
            toastr["error"]("Oops. Something went wrong. Try again.", "Error");
        }
    });
});

$("#saveEmployeeButton").click(function (e) {
    e.preventDefault();
    let modal = $("#editEmployeeModal");

    let validationSucceed = $("#editEmployeeForm").validate().form();
    if (validationSucceed == true) {
        $.ajax({
            type: "POST",
            url: "/Employee/Edit/",
            data: {
                id: modal.find("#Id").val(),
                firstName: modal.find("#FirstName").val(),
                lastName: modal.find("#LastName").val(),
                phoneNumber: modal.find("#PhoneNumber").val(),
                address: modal.find("#Address").val(),
                postalCode: modal.find("#PostalCode").val(),
                city: modal.find("#City").val()
            },
            success: function (result) {
                if (result.status == "success") {
                    loadEmployeesTable();
                    modal.modal("hide");
                    toastr["success"](result.message, "Success");

                    modal.find("#FirstName").val("");
                    modal.find("#LastName").val("");
                    modal.find("#PhoneNumber").val("");
                    modal.find("#Address").val("");
                    modal.find("#PostalCode").val("");
                    modal.find("#City").val("");
                } else {
                    toastr["error"](result.message, "Error");
                }
            },
            error: function (result) {
                toastr["error"]("Oops. Something went wrong. Try again.", "Error");
            }
        });
    }
});

$('#deleteEmployeeModal').on('show.bs.modal', function (event) {
    var button = $(event.relatedTarget);
    var id = button.data('id');
    let modal = $(this);

    $.ajax({
        type: "POST",
        url: "/Employee/GetEmployee/",
        data: {
            id: id
        },
        success: function (result) {
            if (result.status == "success") {
                modal.find('#hiddenId').val(result.employee.Id);
                modal.find('.modal-body').text('Do you want to delete employee: `' + result.employee.FirstName + ' ' + result.employee.LastName + '`?');
            } else {
                toastr["error"](result.message, "Error");
            }
        },
        error: function (result) {
            toastr["error"]("Oops. Something went wrong. Try again.", "Error");
        }
    });

});

$("#deleteEmployeeButton").click(function (e) {
    e.preventDefault();
    let modal = $("#deleteEmployeeModal");

    $.ajax({
        type: "POST",
        url: "/Employee/Delete/",
        data: {
            id: modal.find("#hiddenId").val()
        },
        success: function (result) {
            if (result.status == "success") {
                loadEmployeesTable();
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