// employeeService.js
// Async delegates to storageService. All filtering/sorting/pagination
// now happens on the server — applyFilters() is removed.

var employeeService = {

    // Get paged employee list with server-side filters/sort/pagination
    getAll: async function (params) {
        return await storageService.getAll(params);
    },

    // Get one employee by id
    getById: async function (id) {
        return await storageService.getById(id);
    },

    // Add new employee (Admin only)
    add: async function (data) {
        return await storageService.add(data);
    },

    // Update existing employee (Admin only)
    update: async function (id, data) {
        return await storageService.update(id, data);
    },

    // Delete employee (Admin only)
    remove: async function (id) {
        return await storageService.remove(id);
    },

    // Get list of available departments (static — same as DB allowed values)
    getDepartments: function () {
        return ["Engineering", "Marketing", "HR", "Finance", "Operations"];
    }

};
