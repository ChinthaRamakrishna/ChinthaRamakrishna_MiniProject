// app.js
// This is the main file. It sets up all event listeners and
// connects the services together. No business logic here.

// These variables track current filter and sort state
var currentSearch = "";
var currentDept = "All";
var currentStatus = "All";
var currentSortField = "name";
var currentSortDir = "asc";
var currentSection = "login";

// Show or hide sections
function showSection(section) {
    currentSection = section;

    // Hide all sections first
    $("#loginSection").addClass("d-none");
    $("#signupSection").addClass("d-none");
    $("#dashboardSection").addClass("d-none");
    $("#employeeSection").addClass("d-none");

    if (section === "login" || section === "signup") {
        $("#mainNavbar").addClass("d-none");
    } else {
        $("#mainNavbar").removeClass("d-none");
    }

    if (section === "login") {
        $("#loginSection").removeClass("d-none");
    } else if (section === "signup") {
        $("#signupSection").removeClass("d-none");
    } else if (section === "dashboard") {
        $("#dashboardSection").removeClass("d-none");
        refreshDashboard();
        $("#navDashboard").addClass("active");
        $("#navEmployees").removeClass("active");
    } else if (section === "employees") {
        $("#employeeSection").removeClass("d-none");
        uiService.fillDeptFilter();
        refreshEmployeeList();
        $("#navEmployees").addClass("active");
        $("#navDashboard").removeClass("active");
    }
}

// Refresh dashboard data
function refreshDashboard() {
    var summary = dashboardService.getSummary();
    uiService.renderDashboardCards(summary);

    var breakdown = dashboardService.getDepartmentBreakdown();
    uiService.renderDepartmentBreakdown(breakdown);

    var recent = dashboardService.getRecentEmployees(5);
    uiService.renderRecentEmployees(recent);
}

// Refresh employee list with current filters and sort
function refreshEmployeeList() {
    var list = employeeService.applyFilters(currentSearch, currentDept, currentStatus);
    list = employeeService.sortBy(list, currentSortField, currentSortDir);
    uiService.renderEmployeeTable(list);
}

// Refresh both dashboard and employee list
function refreshAll() {
    refreshDashboard();
    if (currentSection === "employees") {
        refreshEmployeeList();
    }
}

// ─── App Start ───────────────────────────────────────────────
$(document).ready(function() {

    // Start at login page
    showSection("login");

    // ─── NAVBAR EVENTS ───────────────────────────
    $("#navDashboard").on("click", function() {
        showSection("dashboard");
    });

    $("#navEmployees").on("click", function() {
        showSection("employees");
    });

    // Add Employee button (navbar and dashboard)
    $("#navAddEmployee, #dashAddBtn").on("click", function() {
        uiService.clearForm();
        uiService.clearFormErrors();
        $("#empModalLabel").text("Add Employee");
        $("#saveEmpBtn").text("Save Employee");
        $("#editEmpId").val("");
        var modal = new bootstrap.Modal(document.getElementById("empModal"));
        modal.show();
    });

    // Logout
    $("#logoutBtn").on("click", function() {
        authService.logout();
        $("#loginForm")[0].reset();
        uiService.clearAuthErrors("login");
        showSection("login");
    });


    // ─── LOGIN ───────────────────────────────────
    $("#loginForm").on("submit", function(e) {
        e.preventDefault();

        var username = $("#loginUsername").val();
        var password = $("#loginPassword").val();

        // Validate
        var errors = validationService.validateAuthForm({ username: username, password: password }, false);
        uiService.clearAuthErrors("login");

        if (Object.keys(errors).length > 0) {
            uiService.showAuthErrors(errors, "login");
            return;
        }

        // Try login
        var success = authService.login(username, password);
        if (success) {
            $("#navbarUsername").text(authService.getCurrentUser());
            showSection("dashboard");
        } else {
            $("#loginErrorMsg").removeClass("d-none").text("Invalid username or password. Please try again.");
        }
    });

    // Go to signup page
    $("#goToSignup").on("click", function() {
        $("#signupForm")[0].reset();
        uiService.clearAuthErrors("signup");
        showSection("signup");
    });

    // ─── SIGNUP ──────────────────────────────────
    $("#signupForm").on("submit", function(e) {
        e.preventDefault();

        var username = $("#signupUsername").val();
        var password = $("#signupPassword").val();
        var confirmPassword = $("#signupConfirmPassword").val();

        // Validate
        var errors = validationService.validateAuthForm(
            { username: username, password: password, confirmPassword: confirmPassword },
            true
        );
        uiService.clearAuthErrors("signup");

        if (Object.keys(errors).length > 0) {
            uiService.showAuthErrors(errors, "signup");
            return;
        }

        // Try signup
        var result = authService.signup(username, password);
        if (result.success) {
            uiService.showToast("Account created! Please login.", "success");
            setTimeout(function() {
                showSection("login");
            }, 1200);
        } else {
            $("#signupErrorMsg").removeClass("d-none").text(result.error);
        }
    });

    // Go back to login from signup
    $("#goToLogin").on("click", function() {
        $("#loginForm")[0].reset();
        uiService.clearAuthErrors("login");
        showSection("login");
    });

    // ─── SEARCH ──────────────────────────────────
    $("#searchInput").on("input", function() {
        currentSearch = $(this).val();
        refreshEmployeeList();
    });

    // ─── DEPARTMENT FILTER ───────────────────────
    $("#deptFilter").on("change", function() {
        currentDept = $(this).val();
        refreshEmployeeList();
    });

    // ─── STATUS FILTER ───────────────────────────
    $("#statusAll").on("click", function() {
        currentStatus = "All";
        $("#statusAll, #statusActive, #statusInactive").removeClass("active");
        $(this).addClass("active");
        refreshEmployeeList();
    });

    $("#statusActive").on("click", function() {
        currentStatus = "Active";
        $("#statusAll, #statusActive, #statusInactive").removeClass("active");
        $(this).addClass("active");
        refreshEmployeeList();
    });

    $("#statusInactive").on("click", function() {
        currentStatus = "Inactive";
        $("#statusAll, #statusActive, #statusInactive").removeClass("active");
        $(this).addClass("active");
        refreshEmployeeList();
    });

    // ─── SORT BY COLUMN ──────────────────────────
    $(document).on("click", ".sortable", function() {
        var field = $(this).data("sort");

        // Toggle direction if same field clicked again
        if (currentSortField === field) {
            currentSortDir = currentSortDir === "asc" ? "desc" : "asc";
        } else {
            currentSortField = field;
            currentSortDir = "asc";
        }

        // Update sort icons
        $(".sortable .sort-icon").html('<i class="bi bi-arrow-down-up text-muted"></i>');
        if (currentSortDir === "asc") {
            $(this).find(".sort-icon").html('<i class="bi bi-arrow-up text-primary"></i>');
        } else {
            $(this).find(".sort-icon").html('<i class="bi bi-arrow-down text-primary"></i>');
        }

        refreshEmployeeList();
    });

    // ─── ADD EMPLOYEE BUTTON (list page) ─────────
    $("#addEmpBtn").on("click", function() {
        uiService.clearForm();
        uiService.clearFormErrors();
        $("#empModalLabel").text("Add Employee");
        $("#saveEmpBtn").text("Save Employee");
        $("#editEmpId").val("");
        var modal = new bootstrap.Modal(document.getElementById("empModal"));
        modal.show();
    });

    // ─── SAVE EMPLOYEE (add or edit) ─────────────
    $("#saveEmpBtn").on("click", function() {
        var editId = $("#editEmpId").val() || null;

        // Collect form data
        var data = {
            firstName:   $("#empFirstName").val().trim(),
            lastName:    $("#empLastName").val().trim(),
            email:       $("#empEmail").val().trim(),
            phone:       $("#empPhone").val().trim(),
            department:  $("#empDepartment").val(),
            designation: $("#empDesignation").val().trim(),
            salary:      Number($("#empSalary").val()),
            joinDate:    $("#empJoinDate").val(),
            status:      $("#empStatus").val()
        };

        // Validate
        var errors = validationService.validateEmployeeForm(data, editId);
        if (Object.keys(errors).length > 0) {
            uiService.showFormErrors(errors);
            return;
        }

        if (editId) {
            // Update existing
            employeeService.update(Number(editId), data);
            uiService.showToast(data.firstName + " " + data.lastName + " updated successfully.", "success");
        } else {
            // Add new
            employeeService.add(data);
            uiService.showToast(data.firstName + " " + data.lastName + " added successfully.", "success");
        }

        // Close modal and refresh
        bootstrap.Modal.getInstance(document.getElementById("empModal")).hide();
        uiService.fillDeptFilter();
        refreshAll();
    });

    // Clear errors when user types in form fields
    $("#empForm").on("input change", ".form-control, .form-select", function() {
        $(this).removeClass("is-invalid");
    });

    // ─── VIEW EMPLOYEE ────────────────────────────
    $(document).on("click", ".viewBtn", function() {
        var id = $(this).data("id");
        var emp = employeeService.getById(id);
        if (emp) {
            uiService.showViewModal(emp);
        }
    });

    // ─── EDIT EMPLOYEE ────────────────────────────
    $(document).on("click", ".editBtn", function() {
        var id = $(this).data("id");
        var emp = employeeService.getById(id);
        if (emp) {
            uiService.clearFormErrors();
            uiService.populateForm(emp);
            $("#editEmpId").val(emp.id);
            $("#empModalLabel").text("Edit Employee");
            $("#saveEmpBtn").text("Update Employee");
            var modal = new bootstrap.Modal(document.getElementById("empModal"));
            modal.show();
        }
    });

    // ─── DELETE EMPLOYEE ──────────────────────────
    $(document).on("click", ".deleteBtn", function() {
        var id = $(this).data("id");
        var emp = employeeService.getById(id);
        if (emp) {
            $("#deleteEmpName").text(emp.firstName + " " + emp.lastName);
            $("#confirmDeleteBtn").data("id", emp.id);
            var modal = new bootstrap.Modal(document.getElementById("deleteModal"));
            modal.show();
        }
    });

    $("#confirmDeleteBtn").on("click", function() {
        var id = $(this).data("id");
        var emp = employeeService.getById(id);
        var name = emp ? emp.firstName + " " + emp.lastName : "Employee";

        employeeService.remove(id);
        bootstrap.Modal.getInstance(document.getElementById("deleteModal")).hide();
        uiService.showToast(name + " deleted successfully.", "danger");
        uiService.fillDeptFilter();
        refreshAll();
    });

}); // end document.ready
