// storageService.js
// This service handles all data operations on the employee array.
// All other services must use this to read or write employee data.

// We copy the data from data.js into a working array
var employees = JSON.parse(JSON.stringify(employeesData));

var storageService = {

    // Get all employees
    getAll: function() {
        return employees;
    },

    // Get one employee by id
    getById: function(id) {
        for (var i = 0; i < employees.length; i++) {
            if (employees[i].id == id) {
                return employees[i];
            }
        }
        return null;
    },

    // Add a new employee
    add: function(emp) {
        employees.push(emp);
    },

    // Update an existing employee
    update: function(id, newData) {
        for (var i = 0; i < employees.length; i++) {
            if (employees[i].id == id) {
                employees[i] = Object.assign({}, employees[i], newData);
                break;
            }
        }
    },

    // Remove an employee by id
    remove: function(id) {
        var newList = [];
        for (var i = 0; i < employees.length; i++) {
            if (employees[i].id != id) {
                newList.push(employees[i]);
            }
        }
        employees = newList;
    },

    // Get next available id (max id + 1)
    nextId: function() {
        if (employees.length === 0) return 1;
        var maxId = 0;
        for (var i = 0; i < employees.length; i++) {
            if (employees[i].id > maxId) {
                maxId = employees[i].id;
            }
        }
        return maxId + 1;
    }

};
