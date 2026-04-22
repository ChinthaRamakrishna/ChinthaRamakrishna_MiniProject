// uiService.js
// This service handles all DOM updates and rendering.
// It receives plain data and shows it on screen. No business logic here.

var uiService = {

    // Generate initials avatar for an employee
    getAvatar: function(emp) {
        var initials = emp.firstName[0] + emp.lastName[0];
        var colors = ["#2d6cdf", "#28a745", "#dc3545", "#f4b400", "#17a2b8", "#6f42c1", "#fd7e14", "#20c997"];
        var color = colors[emp.id % colors.length];
        return '<div class="emp-avatar" style="background:' + color + '">' + initials.toUpperCase() + '</div>';
    },

    // Generate department badge
    getDeptBadge: function(dept) {
        var classMap = {
            "Engineering": "badge-engineering",
            "Marketing":   "badge-marketing",
            "HR":          "badge-hr",
            "Finance":     "badge-finance",
            "Operations":  "badge-operations"
        };
        var cls = classMap[dept] || "";
        return '<span class="dept-badge ' + cls + '">' + dept + '</span>';
    },

    // Generate status badge
    getStatusBadge: function(status) {
        if (status === "Active") {
            return '<span class="badge-status-active">Active</span>';
        }
        return '<span class="badge-status-inactive">Inactive</span>';
    },

    // Format salary in Indian style  e.g. ₹8,50,000
    formatSalary: function(salary) {
        return "₹" + Number(salary).toLocaleString("en-IN");
    },

    // Format date from YYYY-MM-DD to DD Mon YYYY
    formatDate: function(dateStr) {
        var d = new Date(dateStr);
        var months = ["Jan","Feb","Mar","Apr","May","Jun","Jul","Aug","Sep","Oct","Nov","Dec"];
        return d.getDate() + " " + months[d.getMonth()] + " " + d.getFullYear();
    },

    // Render the employee table
    renderEmployeeTable: function(list) {
        var html = "";

        if (list.length === 0) {
            html = '<tr><td colspan="10" class="text-center py-4 text-muted">' +
                   '<i class="bi bi-search fs-4 d-block mb-2"></i>No employees found.</td></tr>';
            $("#employeeTableBody").html(html);
            $("#employeeCount").text("Showing 0 of " + employeeService.getAll().length + " employees");
            return;
        }

        for (var i = 0; i < list.length; i++) {
            var e = list[i];
            html += '<tr>';
            html += '<td class="text-muted fw-semibold">#' + e.id + '</td>';
            html += '<td>' + this.getAvatar(e) + '</td>';
            html += '<td class="fw-semibold">' + e.firstName + ' ' + e.lastName + '</td>';
            html += '<td class="text-muted small">' + e.email + '</td>';
            html += '<td>' + this.getDeptBadge(e.department) + '</td>';
            html += '<td class="small">' + e.designation + '</td>';
            html += '<td class="fw-semibold">' + this.formatSalary(e.salary) + '</td>';
            html += '<td class="small text-muted">' + this.formatDate(e.joinDate) + '</td>';
            html += '<td>' + this.getStatusBadge(e.status) + '</td>';
            html += '<td>';
            html += '<button class="btn btn-action btn-view viewBtn" data-id="' + e.id + '" title="View"><i class="bi bi-eye"></i></button> ';
            html += '<button class="btn btn-action btn-edit editBtn" data-id="' + e.id + '" title="Edit"><i class="bi bi-pencil"></i></button> ';
            html += '<button class="btn btn-action btn-delete deleteBtn" data-id="' + e.id + '" title="Delete"><i class="bi bi-trash"></i></button>';
            html += '</td>';
            html += '</tr>';
        }

        $("#employeeTableBody").html(html);
        $("#employeeCount").text("Showing " + list.length + " of " + employeeService.getAll().length + " employees");
    },

    // Render dashboard KPI cards
    renderDashboardCards: function(summary) {
        $("#statTotal").text(summary.total);
        $("#statActive").text(summary.active);
        $("#statInactive").text(summary.inactive);
        $("#statDepts").text(summary.departments);
    },

    // Render department breakdown table
    renderDepartmentBreakdown: function(breakdown) {
        var colorMap = {
            "Engineering": "#2d6cdf",
            "Marketing":   "#f4b400",
            "HR":          "#17a2b8",
            "Finance":     "#28a745",
            "Operations":  "#6c757d"
        };
        var html = "";
        for (var i = 0; i < breakdown.length; i++) {
            var d = breakdown[i];
            var color = colorMap[d.department] || "#aaa";
            html += '<tr>';
            html += '<td>' + this.getDeptBadge(d.department) + '</td>';
            html += '<td class="fw-semibold">' + d.count + '</td>';
            html += '<td style="width:160px"><div class="progress" style="height:8px">';
            html += '<div class="progress-bar" style="width:' + d.percentage + '%;background:' + color + '"></div>';
            html += '</div></td>';
            html += '<td class="text-muted small">' + d.percentage + '%</td>';
            html += '</tr>';
        }
        $("#deptBreakdownBody").html(html);
    },

    // Render recent employees panel
    renderRecentEmployees: function(list) {
        var html = "";
        for (var i = 0; i < list.length; i++) {
            var e = list[i];
            html += '<div class="recent-emp-item d-flex align-items-center justify-content-between">';
            html += '<div class="d-flex align-items-center gap-3">';
            html += this.getAvatar(e);
            html += '<div><div class="fw-semibold small">' + e.firstName + ' ' + e.lastName + '</div>';
            html += '<div class="text-muted" style="font-size:12px">' + e.designation + '</div></div>';
            html += '</div>';
            html += '<div class="d-flex gap-1">' + this.getDeptBadge(e.department) + ' ' + this.getStatusBadge(e.status) + '</div>';
            html += '</div>';
        }
        $("#recentEmployeesList").html(html);
    },

    // Fill the add/edit form with employee data (for edit mode)
    populateForm: function(emp) {
        $("#empFirstName").val(emp.firstName);
        $("#empLastName").val(emp.lastName);
        $("#empEmail").val(emp.email);
        $("#empPhone").val(emp.phone);
        $("#empDepartment").val(emp.department);
        $("#empDesignation").val(emp.designation);
        $("#empSalary").val(emp.salary);
        $("#empJoinDate").val(emp.joinDate);
        $("#empStatus").val(emp.status);
    },

    // Clear the add/edit form
    clearForm: function() {
        $("#empForm")[0].reset();
        $("#editEmpId").val("");
    },

    // Show inline error messages on employee form fields
    showFormErrors: function(errors) {
        // Clear old errors first
        this.clearFormErrors();

        var fieldMap = {
            firstName:   "#empFirstName",
            lastName:    "#empLastName",
            email:       "#empEmail",
            phone:       "#empPhone",
            department:  "#empDepartment",
            designation: "#empDesignation",
            salary:      "#empSalary",
            joinDate:    "#empJoinDate",
            status:      "#empStatus"
        };

        for (var field in errors) {
            var $el = $(fieldMap[field]);
            if ($el.length) {
                $el.addClass("is-invalid");
                $el.next(".invalid-feedback").text(errors[field]);
            }
        }
    },

    // Clear all form error states
    clearFormErrors: function() {
        $("#empForm .is-invalid").removeClass("is-invalid");
        $("#empForm .invalid-feedback").text("");
    },

    // Show auth form errors (login/signup)
    showAuthErrors: function(errors, formPrefix) {
        this.clearAuthErrors(formPrefix);
        if (errors.username) {
            $("#" + formPrefix + "Username").addClass("is-invalid").next(".invalid-feedback").text(errors.username);
        }
        if (errors.password) {
            $("#" + formPrefix + "Password").addClass("is-invalid").next(".invalid-feedback").text(errors.password);
        }
        if (errors.confirmPassword) {
            $("#signupConfirmPassword").addClass("is-invalid").next(".invalid-feedback").text(errors.confirmPassword);
        }
    },

    // Clear auth form errors
    clearAuthErrors: function(formPrefix) {
        $("#" + formPrefix + "Form .is-invalid").removeClass("is-invalid");
        $("#" + formPrefix + "Form .invalid-feedback").text("");
        $("#" + formPrefix + "ErrorMsg").addClass("d-none").text("");
    },

    // Show a toast notification
    showToast: function(message, type) {
        type = type || "success";
        var icons = {
            success: "bi-check-circle-fill",
            danger:  "bi-x-circle-fill",
            warning: "bi-exclamation-triangle-fill"
        };
        var $toast = $("#appToast");
        $toast.find(".toast-body").html('<i class="bi ' + icons[type] + ' me-2"></i>' + message);
        $toast.removeClass("text-bg-success text-bg-danger text-bg-warning").addClass("text-bg-" + type);
        var toastEl = new bootstrap.Toast($toast[0]);
        toastEl.show();
    },

    // Fill department filter dropdown
    fillDeptFilter: function() {
        var depts = employeeService.getDepartments();
        $("#deptFilter").find("option:not(:first)").remove();
        for (var i = 0; i < depts.length; i++) {
            $("#deptFilter").append('<option value="' + depts[i] + '">' + depts[i] + '</option>');
        }
    },

    // Show view modal with employee details
    showViewModal: function(emp) {
        var colors = ["#2d6cdf", "#28a745", "#dc3545", "#f4b400", "#17a2b8", "#6f42c1", "#fd7e14", "#20c997"];
        var color = colors[emp.id % colors.length];
        var initials = (emp.firstName[0] + emp.lastName[0]).toUpperCase();

        var html = '<div class="text-center mb-3">';
        html += '<div class="view-avatar mx-auto" style="background:' + color + '">' + initials + '</div>';
        html += '<h5 class="mt-2 mb-1 fw-bold">' + emp.firstName + ' ' + emp.lastName + '</h5>';
        html += this.getDeptBadge(emp.department);
        html += '</div>';
        html += '<div class="row g-3 mt-1">';
        html += '<div class="col-6"><div class="view-label">EMAIL</div><div class="view-value">' + emp.email + '</div></div>';
        html += '<div class="col-6"><div class="view-label">PHONE</div><div class="view-value">' + emp.phone + '</div></div>';
        html += '<div class="col-6"><div class="view-label">DESIGNATION</div><div class="view-value">' + emp.designation + '</div></div>';
        html += '<div class="col-6"><div class="view-label">ANNUAL SALARY</div><div class="view-value text-primary fw-bold">' + this.formatSalary(emp.salary) + '</div></div>';
        html += '<div class="col-6"><div class="view-label">JOIN DATE</div><div class="view-value">' + this.formatDate(emp.joinDate) + '</div></div>';
        html += '<div class="col-6"><div class="view-label">STATUS</div><div class="view-value">' + this.getStatusBadge(emp.status) + '</div></div>';
        html += '</div>';

        $("#viewModalBody").html(html);
        var modal = new bootstrap.Modal(document.getElementById("viewModal"));
        modal.show();
    }

};
