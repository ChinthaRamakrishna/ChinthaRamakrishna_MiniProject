// dashboardService.js
// This service calculates all the numbers shown on the dashboard.
// It uses employeeService to get data. It never touches the DOM.

var dashboardService = {

    // Get total, active, inactive, department count
    getSummary: function() {
        var all = employeeService.getAll();
        var active = 0;
        var inactive = 0;
        var deptList = [];

        for (var i = 0; i < all.length; i++) {
            if (all[i].status === "Active") {
                active++;
            } else {
                inactive++;
            }
            if (deptList.indexOf(all[i].department) === -1) {
                deptList.push(all[i].department);
            }
        }

        return {
            total: all.length,
            active: active,
            inactive: inactive,
            departments: deptList.length
        };
    },

    // Get count of employees in each department
    getDepartmentBreakdown: function() {
        var all = employeeService.getAll();
        var counts = {};

        for (var i = 0; i < all.length; i++) {
            var dept = all[i].department;
            if (counts[dept]) {
                counts[dept]++;
            } else {
                counts[dept] = 1;
            }
        }

        var result = [];
        var total = all.length;
        for (var dept in counts) {
            result.push({
                department: dept,
                count: counts[dept],
                percentage: total > 0 ? Math.round((counts[dept] / total) * 100) : 0
            });
        }
        return result;
    },

    // Get last N employees added (by highest id)
    getRecentEmployees: function(n) {
        var all = employeeService.getAll().slice(); // copy
        // Sort by id descending
        all.sort(function(a, b) {
            return b.id - a.id;
        });
        return all.slice(0, n);
    }

};
