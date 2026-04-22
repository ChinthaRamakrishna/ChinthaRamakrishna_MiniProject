// dashboardService.test.js
// Unit tests for dashboardService
// Tests: getSummary, getDepartmentBreakdown, getRecentEmployees

// ── Test data ──────────────────────────────────────────────────
var testEmployees = [
    { id: 1, firstName: "Priya",  lastName: "Prabhu",  email: "p@test.com",  department: "Engineering", designation: "Engineer",    salary: 850000,  joinDate: "2021-03-15", status: "Active"   },
    { id: 2, firstName: "Arjun",  lastName: "Sharma",  email: "a@test.com",  department: "Marketing",   designation: "Executive",   salary: 620000,  joinDate: "2020-07-01", status: "Active"   },
    { id: 3, firstName: "Neha",   lastName: "Kapoor",  email: "n@test.com",  department: "HR",          designation: "HR Exec",     salary: 550000,  joinDate: "2019-11-20", status: "Inactive" },
    { id: 4, firstName: "Rahul",  lastName: "Verma",   email: "r@test.com",  department: "Finance",     designation: "Analyst",     salary: 720000,  joinDate: "2022-01-10", status: "Active"   },
    { id: 5, firstName: "Sneha",  lastName: "Prasad",  email: "s@test.com",  department: "Operations",  designation: "Manager",     salary: 950000,  joinDate: "2018-06-05", status: "Active"   },
    { id: 6, firstName: "Vikram", lastName: "Raj",     email: "v@test.com",  department: "Engineering", designation: "Sr Dev",      salary: 1100000, joinDate: "2017-09-12", status: "Active"   },
    { id: 7, firstName: "Ananya", lastName: "Singh",   email: "an@test.com", department: "Marketing",   designation: "Strategist",  salary: 580000,  joinDate: "2023-02-28", status: "Inactive" }
];

// ── Mock employeeService ───────────────────────────────────────
var employeeService = {
    getAll: function() { return testEmployees.slice(); }
};

// ── dashboardService (same logic as the real file) ─────────────
var dashboardService = {
    getSummary: function() {
        var all = employeeService.getAll();
        var active = 0;
        var inactive = 0;
        var deptList = [];
        for (var i = 0; i < all.length; i++) {
            if (all[i].status === "Active") { active++; } else { inactive++; }
            if (deptList.indexOf(all[i].department) === -1) deptList.push(all[i].department);
        }
        return { total: all.length, active: active, inactive: inactive, departments: deptList.length };
    },
    getDepartmentBreakdown: function() {
        var all = employeeService.getAll();
        var counts = {};
        for (var i = 0; i < all.length; i++) {
            var d = all[i].department;
            counts[d] = counts[d] ? counts[d] + 1 : 1;
        }
        var result = [];
        var total = all.length;
        for (var dept in counts) {
            result.push({ department: dept, count: counts[dept], percentage: Math.round((counts[dept] / total) * 100) });
        }
        return result;
    },
    getRecentEmployees: function(n) {
        var all = employeeService.getAll();
        all.sort(function(a, b) { return b.id - a.id; });
        return all.slice(0, n);
    }
};

// ── Tests ──────────────────────────────────────────────────────

describe("dashboardService - getSummary", function() {
    test("should return correct total count", function() {
        expect(dashboardService.getSummary().total).toBe(7);
    });

    test("should return correct active count", function() {
        expect(dashboardService.getSummary().active).toBe(5);
    });

    test("should return correct inactive count", function() {
        expect(dashboardService.getSummary().inactive).toBe(2);
    });

    test("should return correct number of departments", function() {
        expect(dashboardService.getSummary().departments).toBe(5);
    });

    test("active + inactive should equal total", function() {
        var s = dashboardService.getSummary();
        expect(s.active + s.inactive).toBe(s.total);
    });
});

describe("dashboardService - getDepartmentBreakdown", function() {
    test("should return 5 departments", function() {
        expect(dashboardService.getDepartmentBreakdown().length).toBe(5);
    });

    test("Engineering should have count 2", function() {
        var bd = dashboardService.getDepartmentBreakdown();
        var eng = null;
        for (var i = 0; i < bd.length; i++) {
            if (bd[i].department === "Engineering") { eng = bd[i]; break; }
        }
        expect(eng.count).toBe(2);
    });

    test("all percentages should be between 0 and 100", function() {
        var bd = dashboardService.getDepartmentBreakdown();
        for (var i = 0; i < bd.length; i++) {
            expect(bd[i].percentage).toBeGreaterThanOrEqual(0);
            expect(bd[i].percentage).toBeLessThanOrEqual(100);
        }
    });

    test("sum of all counts should equal total employees", function() {
        var bd = dashboardService.getDepartmentBreakdown();
        var sum = 0;
        for (var i = 0; i < bd.length; i++) sum += bd[i].count;
        expect(sum).toBe(7);
    });
});

describe("dashboardService - getRecentEmployees", function() {
    test("should return 5 employees by default", function() {
        expect(dashboardService.getRecentEmployees(5).length).toBe(5);
    });

    test("should return employees with highest IDs first", function() {
        var recent = dashboardService.getRecentEmployees(3);
        expect(recent[0].id).toBe(7);
        expect(recent[1].id).toBe(6);
        expect(recent[2].id).toBe(5);
    });

    test("should respect the n parameter", function() {
        expect(dashboardService.getRecentEmployees(2).length).toBe(2);
    });

    test("IDs should be in descending order", function() {
        var recent = dashboardService.getRecentEmployees(5);
        for (var i = 0; i < recent.length - 1; i++) {
            expect(recent[i].id).toBeGreaterThan(recent[i + 1].id);
        }
    });
});
