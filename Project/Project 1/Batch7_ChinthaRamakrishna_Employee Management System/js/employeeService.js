// employeeService.js
// This service handles all employee-related business logic.
// It uses storageService to read and write data. It never touches the DOM.

var employeeService = {

    // Get all employees
    getAll: function() {
        return storageService.getAll();
    },

    // Get one employee by id
    getById: function(id) {
        return storageService.getById(id);
    },

    // Add new employee
    add: function(data) {
        data.id = storageService.nextId();
        storageService.add(data);
    },

    // Update existing employee
    update: function(id, data) {
        storageService.update(id, data);
    },

    // Delete employee
    remove: function(id) {
        storageService.remove(id);
    },

    // Search by name or email
    search: function(query) {
        var q = query.toLowerCase();
        var result = [];
        var all = storageService.getAll();
        for (var i = 0; i < all.length; i++) {
            var fullName = (all[i].firstName + " " + all[i].lastName).toLowerCase();
            var email = all[i].email.toLowerCase();
            if (fullName.indexOf(q) !== -1 || email.indexOf(q) !== -1) {
                result.push(all[i]);
            }
        }
        return result;
    },

    // Filter by department
    filterByDepartment: function(dept) {
        if (!dept || dept === "All") return storageService.getAll();
        var result = [];
        var all = storageService.getAll();
        for (var i = 0; i < all.length; i++) {
            if (all[i].department === dept) {
                result.push(all[i]);
            }
        }
        return result;
    },

    // Filter by status
    filterByStatus: function(status) {
        if (!status || status === "All") return storageService.getAll();
        var result = [];
        var all = storageService.getAll();
        for (var i = 0; i < all.length; i++) {
            if (all[i].status === status) {
                result.push(all[i]);
            }
        }
        return result;
    },

    // Apply all filters together (search + department + status)
    applyFilters: function(searchText, dept, status) {
        var all = storageService.getAll();
        var result = [];
        var q = searchText ? searchText.toLowerCase() : "";

        for (var i = 0; i < all.length; i++) {
            var emp = all[i];

            // Check search match
            var fullName = (emp.firstName + " " + emp.lastName).toLowerCase();
            var matchSearch = !q || fullName.indexOf(q) !== -1 || emp.email.toLowerCase().indexOf(q) !== -1;

            // Check department match
            var matchDept = !dept || dept === "All" || emp.department === dept;

            // Check status match
            var matchStatus = !status || status === "All" || emp.status === status;

            // All three must match (AND logic)
            if (matchSearch && matchDept && matchStatus) {
                result.push(emp);
            }
        }
        return result;
    },

    // Sort employees by field and direction
    sortBy: function(list, field, direction) {
        var sorted = list.slice(); // copy array
        sorted.sort(function(a, b) {
            var valA, valB;

            if (field === "name") {
                valA = a.lastName.toLowerCase();
                valB = b.lastName.toLowerCase();
            } else if (field === "salary") {
                valA = a.salary;
                valB = b.salary;
            } else if (field === "joinDate") {
                valA = new Date(a.joinDate);
                valB = new Date(b.joinDate);
            } else {
                return 0;
            }

            if (valA < valB) return direction === "asc" ? -1 : 1;
            if (valA > valB) return direction === "asc" ? 1 : -1;
            return 0;
        });
        return sorted;
    },

    // Get list of unique department names
    getDepartments: function() {
        var all = storageService.getAll();
        var depts = [];
        for (var i = 0; i < all.length; i++) {
            if (depts.indexOf(all[i].department) === -1) {
                depts.push(all[i].department);
            }
        }
        return depts;
    }

};
