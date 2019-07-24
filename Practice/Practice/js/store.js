$(document).ready(function () {
    loadSubscriptionsTable();
});

function loadSubscriptionsTable() {
    $('#table').DataTable().destroy();
    var tab = $('#table').DataTable({
        "bServerSide": true,
        "bProcessing": true,
        "searching": false,
        "ajax": {
            url: '/Store/GetAllStoresAsync',
            type: 'POST'
        },
        paging: true,
        ordering: true,
        bAutoWidth: false,
        "searching": true,
        "pageLength": 25,
        "order": [[1, "asc"]],
        "columns": [
            { "data": "Name" },
            { "data": "Address" },
            { "data": "PostalCode" },
            { "data": "City" },
            { "data": "Country" },
            {
                "data": "Id",
                "className": "text-center",
                "render": function (data) {
                    var editButton = '<button type="button" class="btn btn-sm btn-warning" data-toggle="modal" data-target="#editStoreModal" data-id="' + data + '"><i class="fas fa-edit"></i></button>';
                    var deleteButton = '<button type="button" class="btn btn-sm btn-danger ml-1" data-toggle="modal" data-target="#deleteStoreModal" data-id="' + data + '"><i class="far fa-trash-alt"></i></button>';
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
            { "targets": 5, "sortable": false }
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


$('#editStoreModal').on('show.bs.modal', function (event) {
    let button = $(event.relatedTarget);
    let id = button.data('id');
    let modal = $(this);

    $.ajax({
        type: "POST",
        url: "/Store/GetStore/",
        data: {
            id: id
        },
        success: function (result) {
            if (result.status == "success") {
                modal.find('#Id').val(result.store.Id);
                modal.find('#Name').val(result.store.Name);
                modal.find('#Address').val(result.store.Address);
                modal.find('#PostalCode').val(result.store.PostalCode);
                modal.find('#City').val(result.store.City);
                modal.find('#Country').val(result.store.Country);
            } else {
                toastr["error"](result.message, "Error");
            }
        },
        error: function (result) {
            toastr["error"]("Oops. Something went wrong. Try again.", "Error");
        }
    });
});


$("#saveStoreButton").click(function (e) {
    e.preventDefault();
    let modal = $("#editStoreModal");

    let validationSucceed = $("#editStoreForm").validate().form();

    if (validationSucceed == true) {
        $.ajax({
            type: "POST",
            url: "/Store/Edit/",
            data: {
                id: modal.find('#Id').val(),
                Name: modal.find('#Name').val(),
                Address: modal.find('#Address').val(),
                PostalCode: modal.find('#PostalCode').val(),
                City: modal.find('#City').val(),
                Country: modal.find('#Country').val()
            },
            success: function (result) {
                if (result.status == "success") {
                    loadSubscriptionsTable();
                    modal.modal("hide");
                    toastr["success"](result.message, "Success");

                    modal.find('#Id').val("");
                    modal.find('#Name').val("");
                    modal.find('#Address').val("");
                    modal.find('#PostalCode').val("");
                    modal.find('#City').val("");
                    modal.find('#Country').val("");
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


$('#deleteStoreModal').on('show.bs.modal', function (event) {
    var button = $(event.relatedTarget);
    var id = button.data('id');
    let modal = $(this);

    $.ajax({
        type: "POST",
        url: "/Store/GetStore/",
        data: {
            id: id
        },
        success: function (result) {
            if (result.status == "success") {
                modal.find('#hiddenId').val(result.store.Id);
                modal.find('.modal-body').text('Do you want to delete store: `' + result.store.Name + '`?');
            } else {
                toastr["error"](result.message, "Error");
            }
        },
        error: function (result) {
            toastr["error"]("Oops. Something went wrong. Try again.", "Error");
        }
    });

});


$("#deleteStoreButton").click(function (e) {
    e.preventDefault();
    let modal = $("#deleteStoreModal");

    $.ajax({
        type: "POST",
        url: "/Store/Delete/",
        data: {
            id: modal.find("#hiddenId").val()
        },
        success: function (result) {
            if (result.status == "success") {
                loadSubscriptionsTable();
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


$("#createStoreButton").click(function (e) {
    e.preventDefault();
    let modal = $("#createStoreModal");

    let validationSucceed = $("#createStoreForm").validate().form();
    if (validationSucceed == true) {
        $.ajax({
            type: "POST",
            url: "/Store/Create/",
            data: {
                ShopName : modal.find("#ShopName").val(),
                Address : modal.find("#Address").val(),
                PostalCode : modal.find("#PostalCode").val(),
                City : modal.find("#City").val(),
                Country : modal.find("#Country").val(),
            },
            success: function (result) {
                if (result.status == "success") {
                    loadSubscriptionsTable();
                    $("#createStoreModal").modal("hide");
                    toastr["success"](result.message, "Success");

                    modal.find("#ShopName").val("");
                    modal.find("#Address").val("");
                    modal.find("#PostalCode").val("");
                    modal.find("#City").val("");
                    modal.find("#Country").val("");
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