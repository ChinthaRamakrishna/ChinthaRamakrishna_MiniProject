// dashboardService.js
// getSummary() makes a single call to /api/employees/dashboard.
// All KPIs, department breakdown and recent employees come back together.

var dashboardService = {

    getSummary: async function () {
        return await storageService.getDashboard();
    }

};
