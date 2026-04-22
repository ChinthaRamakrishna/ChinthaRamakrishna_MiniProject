// uiService.js
// Handles all DOM updates and rendering.
// UPDATED: renderEmployeeTable() accepts paged result + renders pagination bar.
//          applyRoleUI() shows/hides write buttons based on Admin/Viewer role.

var uiService = {

    // Generate initials avatar
    getAvatar: function (emp) {
        var initials = (emp.firstName || emp.FirstName || "?")[0] +
                       (emp.lastName  || emp.LastName  || "?")[0];
        var colors = ["#2d6cdf","#28a745","#dc3545","#f4b400","#17a2b8","#6f42c1","#fd7e14","#20c997"];
        var color  = colors[emp.id % colors.length];
        return '<div class="emp-avatar" style="background:' + color + '">' +
               initials.toUpperCase() + '</div>';
    },

    // Generate department badge
    getDeptBadge: function (dept) {
        var classMap = {
            "Engineering": "badge-engineering",
            "Marketing":   "badge-marketing",
            "HR":          "badge-hr",
            "Finance":     "badge-finance",
            "Operations":  "badge-operations"
        };
        return '<span class="dept-badge ' + (classMap[dept] || "") + '">' + dept + '</span>';
    },

    // Generate status badge
    getStatusBadge: function (status) {
        if (status === "Active")
            return '<span class="badge-status-active">Active</span>';
        return '<span class="badge-status-inactive">Inactive</span>';
    },

    // Format salary in Indian style
    formatSalary: function (salary) {
        return "₹" + Number(salary).toLocaleString("en-IN");
    },

    // Format ISO date string to DD Mon YYYY
    formatDate: function (dateStr) {
        var d = new Date(dateStr);
        var months = ["Jan","Feb","Mar","Apr","May","Jun","Jul","Aug","Sep","Oct","Nov","Dec"];
        return d.getDate() + " " + months[d.getMonth()] + " " + d.getFullYear();
    },

    // ── Render employee table with paged result ───────────────────────────────
    renderEmployeeTable: function (pagedResult) {
        var list       = pagedResult.data        || [];
        var totalCount = pagedResult.totalCount  || 0;
        var page       = pagedResult.page        || 1;
        var pageSize   = pagedResult.pageSize    || PAGE_SIZE;
        var totalPages = pagedResult.totalPages  || 1;

        var isAdmin = authService.isAdmin();

        if (list.length === 0) {
            $("#employeeTableBody").html(
                '<tr><td colspan="10" class="text-center py-4 text-muted">' +
                '<i class="bi bi-search fs-4 d-block mb-2"></i>No employees found.</td></tr>'
            );
            var from = (page - 1) * pageSize + 1;
            $("#employeeCount").text("Showing 0 of " + totalCount + " employees");
            this.renderPagination(page, totalPages);
            return;
        }

        var html = "";
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
            if (isAdmin) {
                html += '<button class="btn btn-action btn-edit editBtn" data-id="' + e.id + '" title="Edit"><i class="bi bi-pencil"></i></button> ';
                html += '<button class="btn btn-action btn-delete deleteBtn" data-id="' + e.id + '" title="Delete"><i class="bi bi-trash"></i></button>';
            }
            html += '</td>';
            html += '</tr>';
        }

        $("#employeeTableBody").html(html);

        // Record count label
        var from = (page - 1) * pageSize + 1;
        var to   = Math.min(page * pageSize, totalCount);
        $("#employeeCount").text("Showing " + from + "–" + to + " of " + totalCount + " employees");

        this.renderPagination(page, totalPages);
    },

    // ── Render Bootstrap pagination bar ──────────────────────────────────────
    renderPagination: function (currentPage, totalPages) {
        if (totalPages <= 1) {
            $("#paginationBar").html("");
            return;
        }

        var html = '<ul class="pagination pagination-sm mb-0 justify-content-end">';

        // Prev button
        html += '<li class="page-item' + (currentPage === 1 ? ' disabled' : '') + '">';
        html += '<button class="page-link pageBtn" data-page="' + (currentPage - 1) + '">&laquo; Prev</button></li>';

        // Page number buttons (show max 5 around current)
        var start = Math.max(1, currentPage - 2);
        var end   = Math.min(totalPages, currentPage + 2);

        if (start > 1) {
            html += '<li class="page-item"><button class="page-link pageBtn" data-page="1">1</button></li>';
            if (start > 2) html += '<li class="page-item disabled"><span class="page-link">…</span></li>';
        }

        for (var p = start; p <= end; p++) {
            html += '<li class="page-item' + (p === currentPage ? ' active' : '') + '">';
            html += '<button class="page-link pageBtn" data-page="' + p + '">' + p + '</button></li>';
        }

        if (end < totalPages) {
            if (end < totalPages - 1) html += '<li class="page-item disabled"><span class="page-link">…</span></li>';
            html += '<li class="page-item"><button class="page-link pageBtn" data-page="' + totalPages + '">' + totalPages + '</button></li>';
        }

        // Next button
        html += '<li class="page-item' + (currentPage === totalPages ? ' disabled' : '') + '">';
        html += '<button class="page-link pageBtn" data-page="' + (currentPage + 1) + '">Next &raquo;</button></li>';

        html += '</ul>';
        $("#paginationBar").html(html);
    },

    // ── Role-based UI ─────────────────────────────────────────────────────────
    applyRoleUI: function () {
        var isAdmin = authService.isAdmin();
        var role    = authService.getRole();

        // Show role badge in navbar
        $("#navbarRoleBadge")
            .removeClass("badge-admin badge-viewer")
            .addClass(isAdmin ? "badge-admin" : "badge-viewer")
            .text(role);

        if (isAdmin) {
            // Show write buttons, hide viewer notice
            $(".admin-only").removeClass("d-none");
            $("#viewerNotice").addClass("d-none");
        } else {
            // Hide write buttons, show viewer notice
            $(".admin-only").addClass("d-none");
            $("#viewerNotice").removeClass("d-none");
        }
    },

    // ── Dashboard rendering ───────────────────────────────────────────────────
    renderDashboardCards: function (summary) {
        $("#statTotal")   .text(summary.total);
        $("#statActive")  .text(summary.active);
        $("#statInactive").text(summary.inactive);
        $("#statDepts")   .text(summary.departments);
    },

    renderDepartmentBreakdown: function (breakdown) {
        var colorMap = {
            "Engineering": "#2d6cdf",
            "Marketing":   "#f4b400",
            "HR":          "#17a2b8",
            "Finance":     "#28a745",
            "Operations":  "#6c757d"
        };
        var html = "";
        for (var i = 0; i < breakdown.length; i++) {
            var d     = breakdown[i];
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

    renderRecentEmployees: function (list) {
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

    // ── Form helpers ──────────────────────────────────────────────────────────
    populateForm: function (emp) {
        // Normalize field names (API returns camelCase)
        $("#empFirstName") .val(emp.firstName   || emp.FirstName);
        $("#empLastName")  .val(emp.lastName    || emp.LastName);
        $("#empEmail")     .val(emp.email       || emp.Email);
        $("#empPhone")     .val(emp.phone       || emp.Phone);
        $("#empDepartment").val(emp.department  || emp.Department);
        $("#empDesignation").val(emp.designation|| emp.Designation);
        $("#empSalary")    .val(emp.salary      || emp.Salary);
        // Strip time portion for date input
        var jd = (emp.joinDate || emp.JoinDate || "").toString().substring(0, 10);
        $("#empJoinDate")  .val(jd);
        $("#empStatus")    .val(emp.status      || emp.Status);
    },

    clearForm: function () {
        $("#empForm")[0].reset();
        $("#editEmpId").val("");
    },

    showFormErrors: function (errors) {
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

    clearFormErrors: function () {
        $("#empForm .is-invalid").removeClass("is-invalid");
        $("#empForm .invalid-feedback").text("");
    },

    showAuthErrors: function (errors, formPrefix) {
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

    clearAuthErrors: function (formPrefix) {
        $("#" + formPrefix + "Form .is-invalid").removeClass("is-invalid");
        $("#" + formPrefix + "Form .invalid-feedback").text("");
        $("#" + formPrefix + "ErrorMsg").addClass("d-none").text("");
    },

    showToast: function (message, type) {
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

    fillDeptFilter: function () {
        var depts = employeeService.getDepartments();
        $("#deptFilter").find("option:not(:first)").remove();
        for (var i = 0; i < depts.length; i++) {
            $("#deptFilter").append('<option value="' + depts[i] + '">' + depts[i] + '</option>');
        }
    },

    showViewModal: function (emp) {
        var colors  = ["#2d6cdf","#28a745","#dc3545","#f4b400","#17a2b8","#6f42c1","#fd7e14","#20c997"];
        var color   = colors[emp.id % colors.length];
        var initials = ((emp.firstName || "?")[0] + (emp.lastName || "?")[0]).toUpperCase();

        var html  = '<div class="text-center mb-3">';
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
