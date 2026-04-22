// employeeService.test.js
// Unit tests for employeeService
// Tests: add, update, remove, search, filter, sort, applyFilters

// ── Mock data ──────────────────────────────────────────────────
var testData = [
    { id: 1, firstName: "Priya",  lastName: "Prabhu",  email: "priya@test.com",  phone: "9876543210", department: "Engineering", designation: "Engineer",  salary: 850000,  joinDate: "2021-03-15", status: "Active"   },
    { id: 2, firstName: "Arjun",  lastName: "Sharma",  email: "arjun@test.com",  phone: "9123456780", department: "Marketing",   designation: "Executive", salary: 620000,  joinDate: "2020-07-01", status: "Active"   },
    { id: 3, firstName: "Neha",   lastName: "Kapoor",  email: "neha@test.com",   phone: "9988776655", department: "HR",          designation: "HR Exec",   salary: 550000,  joinDate: "2019-11-20", status: "Inactive" },
    { id: 4, firstName: "Rahul",  lastName: "Verma",   email: "rahul@test.com",  phone: "9001234567", department: "Finance",     designation: "Analyst",   salary: 720000,  joinDate: "2022-01-10", status: "Active"   },
    { id: 5, firstName: "Sneha",  lastName: "Prasad",  email: "sneha@test.com",  phone: "9012345678", department: "Operations",  designation: "Manager",   salary: 950000,  joinDate: "2018-06-05", status: "Active"   }
];

// ── Mock storageService ────────────────────────────────────────
var _store;

var storageService = {
    getAll: function() { return _store; },
    getById: function(id) {
        for (var i = 0; i < _store.length; i++) {
            if (_store[i].id == id) return _store[i];
        }
        return null;
    },
    add: function(emp) { _store.push(emp); },
    update: function(id, data) {
        for (var i = 0; i < _store.length; i++) {
            if (_store[i].id == id) {
                _store[i] = Object.assign({}, _store[i], data);
                break;
            }
        }
    },
    remove: function(id) {
        _store = _store.filter(function(e) { return e.id != id; });
    },
    nextId: function() {
        if (_store.length === 0) return 1;
        var max = 0;
        for (var i = 0; i < _store.length; i++) {
            if (_store[i].id > max) max = _store[i].id;
        }
        return max + 1;
    }
};

// ── employeeService (same logic as the real file) ──────────────
var employeeService = {
    getAll: function() { return storageService.getAll(); },
    getById: function(id) { return storageService.getById(id); },
    add: function(data) { data.id = storageService.nextId(); storageService.add(data); },
    update: function(id, data) { storageService.update(id, data); },
    remove: function(id) { storageService.remove(id); },
    search: function(query) {
        var q = query.toLowerCase();
        var result = [];
        var all = storageService.getAll();
        for (var i = 0; i < all.length; i++) {
            var fullName = (all[i].firstName + " " + all[i].lastName).toLowerCase();
            if (fullName.indexOf(q) !== -1 || all[i].email.toLowerCase().indexOf(q) !== -1) {
                result.push(all[i]);
            }
        }
        return result;
    },
    filterByDepartment: function(dept) {
        if (!dept || dept === "All") return storageService.getAll();
        return storageService.getAll().filter(function(e) { return e.department === dept; });
    },
    filterByStatus: function(status) {
        if (!status || status === "All") return storageService.getAll();
        return storageService.getAll().filter(function(e) { return e.status === status; });
    },
    applyFilters: function(search, dept, status) {
        var all = storageService.getAll();
        var result = [];
        var q = search ? search.toLowerCase() : "";
        for (var i = 0; i < all.length; i++) {
            var emp = all[i];
            var fullName = (emp.firstName + " " + emp.lastName).toLowerCase();
            var matchSearch = !q || fullName.indexOf(q) !== -1 || emp.email.toLowerCase().indexOf(q) !== -1;
            var matchDept = !dept || dept === "All" || emp.department === dept;
            var matchStatus = !status || status === "All" || emp.status === status;
            if (matchSearch && matchDept && matchStatus) result.push(emp);
        }
        return result;
    },
    sortBy: function(list, field, direction) {
        var sorted = list.slice();
        sorted.sort(function(a, b) {
            var valA, valB;
            if (field === "name")     { valA = a.lastName.toLowerCase(); valB = b.lastName.toLowerCase(); }
            else if (field === "salary")   { valA = a.salary; valB = b.salary; }
            else if (field === "joinDate") { valA = new Date(a.joinDate); valB = new Date(b.joinDate); }
            else return 0;
            if (valA < valB) return direction === "asc" ? -1 : 1;
            if (valA > valB) return direction === "asc" ? 1 : -1;
            return 0;
        });
        return sorted;
    }
};

// ── Reset store before each test ───────────────────────────────
beforeEach(function() {
    _store = JSON.parse(JSON.stringify(testData));
});

// ── Tests ──────────────────────────────────────────────────────

describe("employeeService - getAll", function() {
    test("should return all 5 employees", function() {
        expect(employeeService.getAll().length).toBe(5);
    });
});

describe("employeeService - add", function() {
    test("should add a new employee with auto id", function() {
        var newEmp = { firstName: "Test", lastName: "User", email: "test@test.com", phone: "9999999999", department: "HR", designation: "Intern", salary: 300000, joinDate: "2024-01-01", status: "Active" };
        employeeService.add(newEmp);
        expect(employeeService.getAll().length).toBe(6);
        expect(employeeService.getAll()[5].id).toBe(6);
    });

    test("should not create duplicate IDs after deletion", function() {
        employeeService.remove(5);
        var newEmp = { firstName: "New", lastName: "Guy", email: "new@test.com", phone: "8888888888", department: "Finance", designation: "CFO", salary: 200000, joinDate: "2024-01-01", status: "Active" };
        employeeService.add(newEmp);
        var ids = employeeService.getAll().map(function(e) { return e.id; });
        var uniqueIds = ids.filter(function(id, index) { return ids.indexOf(id) === index; });
        expect(uniqueIds.length).toBe(ids.length);
    });
});

describe("employeeService - update", function() {
    test("should update employee fields correctly", function() {
        employeeService.update(1, { designation: "Senior Engineer", salary: 1200000 });
        var emp = employeeService.getById(1);
        expect(emp.designation).toBe("Senior Engineer");
        expect(emp.salary).toBe(1200000);
    });

    test("should not change other employees", function() {
        employeeService.update(1, { salary: 100 });
        expect(employeeService.getById(2).salary).toBe(620000);
    });
});

describe("employeeService - remove", function() {
    test("should remove employee by id", function() {
        employeeService.remove(2);
        expect(employeeService.getAll().length).toBe(4);
        expect(employeeService.getById(2)).toBeNull();
    });

    test("deleted employee should not appear again", function() {
        employeeService.remove(3);
        var found = employeeService.getAll().filter(function(e) { return e.id === 3; });
        expect(found.length).toBe(0);
    });
});

describe("employeeService - search", function() {
    test("should find employee by first name", function() {
        var result = employeeService.search("priya");
        expect(result.length).toBe(1);
        expect(result[0].firstName).toBe("Priya");
    });

    test("should find employee by email", function() {
        var result = employeeService.search("arjun@test.com");
        expect(result.length).toBe(1);
        expect(result[0].firstName).toBe("Arjun");
    });

    test("should return empty array when no match", function() {
        var result = employeeService.search("notfound");
        expect(result.length).toBe(0);
    });
});

describe("employeeService - filterByDepartment", function() {
    test("should filter by Engineering", function() {
        var result = employeeService.filterByDepartment("Engineering");
        expect(result.every(function(e) { return e.department === "Engineering"; })).toBe(true);
    });

    test("should return all when dept is All", function() {
        expect(employeeService.filterByDepartment("All").length).toBe(5);
    });
});

describe("employeeService - filterByStatus", function() {
    test("should filter Active employees", function() {
        var result = employeeService.filterByStatus("Active");
        expect(result.every(function(e) { return e.status === "Active"; })).toBe(true);
    });

    test("should filter Inactive employees", function() {
        var result = employeeService.filterByStatus("Inactive");
        expect(result.length).toBe(1);
        expect(result[0].firstName).toBe("Neha");
    });
});

describe("employeeService - applyFilters (AND logic)", function() {
    test("should apply search + dept + status together", function() {
        var result = employeeService.applyFilters("priya", "Engineering", "Active");
        expect(result.length).toBe(1);
        expect(result[0].firstName).toBe("Priya");
    });

    test("should return empty if filters do not match", function() {
        var result = employeeService.applyFilters("priya", "HR", "Active");
        expect(result.length).toBe(0);
    });

    test("should return all when no filters applied", function() {
        var result = employeeService.applyFilters("", "All", "All");
        expect(result.length).toBe(5);
    });
});

describe("employeeService - sortBy", function() {
    test("should sort by name A to Z", function() {
        var sorted = employeeService.sortBy(employeeService.getAll(), "name", "asc");
        var names = sorted.map(function(e) { return e.lastName; });
        var copy = names.slice().sort();
        expect(names).toEqual(copy);
    });

    test("should sort by salary high to low", function() {
        var sorted = employeeService.sortBy(employeeService.getAll(), "salary", "desc");
        for (var i = 0; i < sorted.length - 1; i++) {
            expect(sorted[i].salary).toBeGreaterThanOrEqual(sorted[i + 1].salary);
        }
    });

    test("should sort by joinDate newest first", function() {
        var sorted = employeeService.sortBy(employeeService.getAll(), "joinDate", "desc");
        for (var i = 0; i < sorted.length - 1; i++) {
            expect(new Date(sorted[i].joinDate).getTime()).toBeGreaterThanOrEqual(new Date(sorted[i + 1].joinDate).getTime());
        }
    });
});
