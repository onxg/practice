// Call the dataTables jQuery plugin
//$(document).ready(function() {
//    $('#dataTable').DataTable();
//});
$(document).ready(function () {
    $('#data-table-employees').DataTable({
        "ajax": {
            "url": "/Employee/GetEmployeesData",
            "type": "POST",
            "datatype": "json"
        },
        "columns": [
            { "data": "FirstName","name":"FirstName" },
            { "data": "LastName", "name": "LastName"  },
            { "data": "PhoneNumber", "name": "PhoneNumber" },
            { "data": "Address", "name": "Address"  },
            { "data": "PostalCode", "name": "PostalCode" },
            { "data": "City", "name": "City" },
            {
                "data": "Id", "render": function (id) {
                    return "<a class='btn btn-primary' href=/Employee/Edit/" + id + ">Edit</a> <a class='btn btn-info' href=/Employee/Details/" + id + ">Details</a> <a class='btn btn-danger' href=/Employee/Delete/" + id + ">Delete</a>"

                },
                "orderable": false,
                "width":"210px"
            }
        ],
        "serverSide": "true",
        "order": [0, "asc"],
        "processing":"true"
    })
})