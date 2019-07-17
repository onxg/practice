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
            { "data": "City", "name": "City"}
        ],
        "serverSide": "true",
        "order": [0, "asc"],
        "processing":"true"
    })
})