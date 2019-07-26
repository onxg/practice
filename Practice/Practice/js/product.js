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
            url: '/Product/GetAllProductsAsync',
            type: 'POST'
        },
        paging: true,
        ordering: true,
        bAutoWidth: false,
        "searching": true,
        "pageLength": 25,
        "order": [[1, "asc"]],
        "columns": [
            { "data": "ProductID" },
            { "data": "Name" },
            { "data": "ProductModel" },
            { "data": "CultureID" },
            {
                "data": function (data) {
                    var editButton = '<button type="button" class="btn btn-sm btn-warning" data-toggle="modal" data-target="#editProductModal" data-ProductID="' +data.ProductID + '"data-CultureID="' +data.CultureID+ '"><i class="fas fa-edit"></i></button>';
                    var deleteButton = '<button type="button" class="btn btn-sm btn-danger ml-1" data-toggle="modal" data-target="#deleteProductModal" data-ProductID="' + data.ProductID + '"data-CultureID="' + data.CultureID + '"><i class="far fa-trash-alt"></i></button>';
                    var descriptionButton = '<button type="button" class="btn btn-sm btn-info ml-1" data-toggle="modal" data-target="#descriptionModal" data-ProductID="' + data.ProductID + '"data-CultureID="' + data.CultureID + '"><i class="fas fa-info"></i></button>';
                    var actionButtons = editButton + deleteButton + descriptionButton;
                    return actionButtons;
                }
            }

                //"data": "ProductID",
                //"className": "text-center",
                //"render": function (data) {
                //    var editButton = '<button type="button" class="btn btn-sm btn-warning" data-toggle="modal" data-target="#editProductModal" data-id="' +  data + '"><i class="fas fa-edit"></i></button>';
                //    var deleteButton = '<button type="button" class="btn btn-sm btn-danger ml-1" data-toggle="modal" data-target="#deleteProductModal" data-id="' + data + '"><i class="far fa-trash-alt"></i></button>';
                //    var descriptionButton = '<button type="button" class="btn btn-sm btn-info ml-1" data-toggle="modal" data-target="#descriptionModal" data-id="' + /*ProductID/* data*/ /*+ CultureID*/ data + '"><i class="fas fa-info"></i></button>';
                //    var actionButtons = editButton + deleteButton + descriptionButton;
                //    return actionButtons;
                  
                //}
            
        ],
        "columnDefs": [
            { "targets": 0 },
            { "targets": 1 },
            { "targets": 2 },
            { "targets": 3 },
            { "targets": 4, "sortable": false }
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


$('#editProductModal').on('show.bs.modal', function (event) {
    let button = $(event.relatedTarget);
    let ProductID = button.data('productid');
    let CultureID = button.data('cultureid');
    let modal = $(this);
    $.ajax({
        type: "POST",
        url: "/Product/GetProduct/",
        data: {
            id: ProductID,
            culture: CultureID

        },
        success: function (result) {
            if (result.status == "success") {
                modal.find('#ProductID').val(result.product.ProductID);
                modal.find('#Name').val(result.product.Name);
                modal.find('#ProductModel').val(result.product.ProductModel);
                modal.find('#Description').val(result.product.Description);
                
                let cultures = result.product.Cultures;
                let htmlCultures = "";
                let textToPrint = "";
                for (i = 0; i < cultures.length; i++) {
                    if (cultures[i] == CultureID)
                        textToPrint = "<option selected>" + cultures[i] + "</option>"
                    else
                        textToPrint = "<option>" + cultures[i] + "</option>";

                    htmlCultures += textToPrint;
                }
                document.getElementById('cultureid').innerHTML = htmlCultures;

            } else {
                toastr["error"](result.message, "Error");
            }
        },
        error: function (result) {
            toastr["error"]("Oops. Something went wrong. Try again.", "Error");
        }
    });
});


$("#saveProductButton").click(function (e) {
    e.preventDefault();
    let modal = $("#editProductModal");

    let validationSucceed = $("#editProductForm").validate().form();

    if (validationSucceed == true) {
        $.ajax({
            type: "POST",
            url: "/Product/Edit/",
            data: {
                id: modal.find('#ProductID').val(),
                Name: modal.find('#Name').val(),
                ProductModel: modal.find('#ProductModel').val(),
                culture: modal.find('#cultureid').val(),
                Description: modal.find('#Description').val()
            },
            success: function (result) {
                if (result.status == "success") {
                    loadSubscriptionsTable();
                    modal.modal("hide");
                    toastr["success"](result.message, "Success");

                    modal.find('#ProductID').val("");
                    modal.find('#Name').val("");
                    modal.find('#ProductModel').val("");
                    modal.find('#cultureid').val("");
                    modal.find('#Description').val("");
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


$('#deleteProductModal').on('show.bs.modal', function (event) {
    var button = $(event.relatedTarget);
    let ProductID = button.data('productid');
    let CultureID = button.data('cultureid');
    let modal = $(this);

    $.ajax({
        type: "POST",
        url: "/Product/GetProduct/",
        data: {
            id: ProductID,
            culture: CultureID
        },
        success: function (result) {
            if (result.status == "success") {
                modal.find('#hiddenId').val(result.product.ProductID);
                modal.find('#hiddenCultureID').val(result.product.CultureID);
                modal.find('.modal-body').text('Do you want to delete product: `' + result.product.ProductID + '`?');
            } else {
                toastr["error"](result.message, "Error");
            }
        },
        error: function (result) {
            toastr["error"]("Oops. Something went wrong. Try again.", "Error");
        }
    });

});


$("#deleteProductButton").click(function (e) {
    e.preventDefault();
    let modal = $("#deleteProductModal");
    console.log(modal.find("#hiddenId").val());
    $.ajax({
        type: "POST",
        url: "/Product/Delete/",
        data: {
            id: modal.find("#hiddenId").val(),
            culture: modal.find('#hiddenCultureID').val()
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

$('#descriptionModal').on('show.bs.modal', function (event) {
    var button = $(event.relatedTarget);
    let ProductID = button.data('productid');
    let CultureID = button.data('cultureid');
    let modal = $(this);

    $.ajax({
        type: "POST",
        url: "/Product/GetProduct/",
        data: {
            id: ProductID,
            culture: CultureID
        },
        success: function (result) {
            if (result.status == "success") {
                modal.find('.modal-body').text(result.product.Description);
            } else {
                toastr["error"](result.message, "Error");
            }
        },
        error: function (result) {
            toastr["error"]("Oops. Something went wrong. Try again.", "Error");
        }
    });

});