// app.js
// Main controller: event listeners + async/await + pagination state.
// All data operations are async. Pagination state tracked in _state object.

var _state = {
    search:    "",
    dept:      "All",
    status:    "All",
    sortBy:    "name",
    sortDir:   "asc",
    page:      1,
    pageSize:  PAGE_SIZE,
    section:   "login"
};

var _searchTimer = null; // debounce handle

// ── Section navigation ────────────────────────────────────────────────────────
function showSection(section) {
    _state.section = section;

    $("#loginSection, #signupSection, #dashboardSection, #employeeSection").addClass("d-none");

    if (section === "login" || section === "signup") {
        $("#mainNavbar").addClass("d-none");
    } else {
        $("#mainNavbar").removeClass("d-none");
        uiService.applyRoleUI();
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

// ── Dashboard refresh ─────────────────────────────────────────────────────────
async function refreshDashboard() {
    var res = await dashboardService.getSummary();
    if (!res.ok) {
        uiService.showToast("Failed to load dashboard data.", "danger");
        return;
    }
    var d = res.data;
    uiService.renderDashboardCards(d);
    uiService.renderDepartmentBreakdown(d.departmentBreakdown);
    uiService.renderRecentEmployees(d.recentEmployees);
}

// ── Employee list refresh ─────────────────────────────────────────────────────
async function refreshEmployeeList() {
    var params = {
        search:    _state.search,
        department: _state.dept !== "All" ? _state.dept : "",
        status:    _state.status !== "All" ? _state.status : "",
        sortBy:    _state.sortBy,
        sortDir:   _state.sortDir,
        page:      _state.page,
        pageSize:  _state.pageSize
    };

    var res = await employeeService.getAll(params);
    if (!res.ok) {
        uiService.showToast("Failed to load employees.", "danger");
        return;
    }
    uiService.renderEmployeeTable(res.data);
}

// ── Refresh both dashboard + employee list ────────────────────────────────────
async function refreshAll() {
    await refreshDashboard();
    if (_state.section === "employees") {
        await refreshEmployeeList();
    }
}

// ── Document ready ────────────────────────────────────────────────────────────
$(document).ready(function () {

    showSection("login");

    // ── NAVBAR ────────────────────────────────────────────────────────────────
    $("#navDashboard").on("click", function () { showSection("dashboard"); });
    $("#navEmployees").on("click", function () { showSection("employees"); });

    $("#navAddEmployee, #dashAddBtn").on("click", function () {
        uiService.clearForm();
        uiService.clearFormErrors();
        $("#empModalLabel").text("Add Employee");
        $("#saveEmpBtn").text("Save Employee");
        $("#editEmpId").val("");
        new bootstrap.Modal(document.getElementById("empModal")).show();
    });

    $("#logoutBtn").on("click", function () {
        authService.logout();
        $("#loginForm")[0].reset();
        uiService.clearAuthErrors("login");
        showSection("login");
    });

    // ── LOGIN ─────────────────────────────────────────────────────────────────
    $("#loginForm").on("submit", async function (e) {
        e.preventDefault();

        var username = $("#loginUsername").val().trim();
        var password = $("#loginPassword").val().trim();

        var errors = validationService.validateAuthForm({ username: username, password: password }, false);
        uiService.clearAuthErrors("login");

        if (Object.keys(errors).length > 0) {
            uiService.showAuthErrors(errors, "login");
            return;
        }

        var result = await authService.login(username, password);
        if (result.success) {
            $("#navbarUsername").text(authService.getCurrentUser());
            showSection("dashboard");
        } else {
            $("#loginErrorMsg").removeClass("d-none").text(result.message || "Invalid username or password.");
        }
    });

    $("#goToSignup").on("click", function () {
        $("#signupForm")[0].reset();
        uiService.clearAuthErrors("signup");
        showSection("signup");
    });

    // ── SIGNUP ────────────────────────────────────────────────────────────────
    $("#signupForm").on("submit", async function (e) {
        e.preventDefault();

        var username        = $("#signupUsername").val().trim();
        var password        = $("#signupPassword").val().trim();
        var confirmPassword = $("#signupConfirmPassword").val().trim();
        var role            = $("#signupRole").val() || "Viewer";

        var errors = validationService.validateAuthForm(
            { username: username, password: password, confirmPassword: confirmPassword }, true
        );
        uiService.clearAuthErrors("signup");

        if (Object.keys(errors).length > 0) {
            uiService.showAuthErrors(errors, "signup");
            return;
        }

        var result = await authService.signup(username, password, role);
        if (result.success) {
            uiService.showToast("Account created! Please login.", "success");
            setTimeout(function () { showSection("login"); }, 1200);
        } else {
            var serverErrors = validationService.mapServerErrors(result.error);
            if (Object.keys(serverErrors).length > 0) {
                uiService.showAuthErrors(serverErrors, "signup");
            } else {
                $("#signupErrorMsg").removeClass("d-none").text(result.error);
            }
        }
    });

    $("#goToLogin").on("click", function () {
        $("#loginForm")[0].reset();
        uiService.clearAuthErrors("login");
        showSection("login");
    });

    // ── SEARCH (debounced 350ms) ───────────────────────────────────────────────
    $("#searchInput").on("input", function () {
        clearTimeout(_searchTimer);
        var val = $(this).val();
        _searchTimer = setTimeout(async function () {
            _state.search = val;
            _state.page   = 1;
            await refreshEmployeeList();
        }, 350);
    });

    // ── DEPARTMENT FILTER ─────────────────────────────────────────────────────
    $("#deptFilter").on("change", async function () {
        _state.dept = $(this).val();
        _state.page = 1;
        await refreshEmployeeList();
    });

    // ── STATUS FILTER ─────────────────────────────────────────────────────────
    $("#statusAll, #statusActive, #statusInactive").on("click", async function () {
        $("#statusAll, #statusActive, #statusInactive").removeClass("active");
        $(this).addClass("active");
        _state.status = $(this).data("status");
        _state.page   = 1;
        await refreshEmployeeList();
    });

    // ── SORT ──────────────────────────────────────────────────────────────────
    $(document).on("click", ".sortable", async function () {
        var field = $(this).data("sort");
        if (_state.sortBy === field) {
            _state.sortDir = _state.sortDir === "asc" ? "desc" : "asc";
        } else {
            _state.sortBy  = field;
            _state.sortDir = "asc";
        }
        $(".sortable .sort-icon").html('<i class="bi bi-arrow-down-up text-muted"></i>');
        $(this).find(".sort-icon").html(
            _state.sortDir === "asc"
                ? '<i class="bi bi-arrow-up text-primary"></i>'
                : '<i class="bi bi-arrow-down text-primary"></i>'
        );
        _state.page = 1;
        await refreshEmployeeList();
    });

    // ── PAGINATION ────────────────────────────────────────────────────────────
    $(document).on("click", ".pageBtn", async function () {
        var page = parseInt($(this).data("page"));
        if (!isNaN(page) && page > 0) {
            _state.page = page;
            await refreshEmployeeList();
        }
    });

    // ── ADD EMPLOYEE (list page button) ───────────────────────────────────────
    $("#addEmpBtn").on("click", function () {
        uiService.clearForm();
        uiService.clearFormErrors();
        $("#empModalLabel").text("Add Employee");
        $("#saveEmpBtn").text("Save Employee");
        $("#editEmpId").val("");
        new bootstrap.Modal(document.getElementById("empModal")).show();
    });

    // ── SAVE EMPLOYEE ─────────────────────────────────────────────────────────
    $("#saveEmpBtn").on("click", async function () {
        var editId = $("#editEmpId").val() || null;

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

        // Client-side validation first
        var errors = validationService.validateEmployeeForm(data, editId);
        if (Object.keys(errors).length > 0) {
            uiService.showFormErrors(errors);
            return;
        }

        var res;
        if (editId) {
            res = await employeeService.update(Number(editId), data);
        } else {
            res = await employeeService.add(data);
        }

        if (res.ok) {
            bootstrap.Modal.getInstance(document.getElementById("empModal")).hide();
            uiService.showToast(
                data.firstName + " " + data.lastName + (editId ? " updated" : " added") + " successfully.",
                "success"
            );
            if (!editId) _state.page = 1;
            await refreshAll();
        } else if (res.status === 409) {
            // Email conflict
            var serverErrors = validationService.mapServerErrors(res.data && res.data.message);
            if (Object.keys(serverErrors).length > 0) {
                uiService.showFormErrors(serverErrors);
            } else {
                uiService.showToast(res.data && res.data.message || "Conflict error.", "danger");
            }
        } else if (res.status === 401 || res.status === 403) {
            uiService.showToast("Access denied. Admin role required.", "danger");
        } else {
            uiService.showToast("Failed to save employee. Please try again.", "danger");
        }
    });

    // Clear errors as user types
    $("#empForm").on("input change", ".form-control, .form-select", function () {
        $(this).removeClass("is-invalid");
    });

    // ── VIEW EMPLOYEE ─────────────────────────────────────────────────────────
    $(document).on("click", ".viewBtn", async function () {
        var id  = $(this).data("id");
        var res = await employeeService.getById(id);
        if (res.ok) {
            uiService.showViewModal(res.data);
        } else {
            uiService.showToast("Could not load employee details.", "danger");
        }
    });

    // ── EDIT EMPLOYEE ─────────────────────────────────────────────────────────
    $(document).on("click", ".editBtn", async function () {
        var id  = $(this).data("id");
        var res = await employeeService.getById(id);
        if (res.ok) {
            uiService.clearFormErrors();
            uiService.populateForm(res.data);
            $("#editEmpId").val(res.data.id);
            $("#empModalLabel").text("Edit Employee");
            $("#saveEmpBtn").text("Update Employee");
            new bootstrap.Modal(document.getElementById("empModal")).show();
        } else {
            uiService.showToast("Could not load employee for editing.", "danger");
        }
    });

    // ── DELETE EMPLOYEE ───────────────────────────────────────────────────────
    $(document).on("click", ".deleteBtn", async function () {
        var id  = $(this).data("id");
        var res = await employeeService.getById(id);
        if (res.ok) {
            var emp = res.data;
            $("#deleteEmpName").text(emp.firstName + " " + emp.lastName);
            $("#confirmDeleteBtn").data("id", emp.id);
            new bootstrap.Modal(document.getElementById("deleteModal")).show();
        }
    });

    $("#confirmDeleteBtn").on("click", async function () {
        var id  = $(this).data("id");
        var res = await employeeService.remove(id);

        bootstrap.Modal.getInstance(document.getElementById("deleteModal")).hide();

        if (res.ok) {
            uiService.showToast("Employee deleted successfully.", "danger");
            // If we deleted the last record on a non-first page, go back one page
            _state.page = Math.max(1, _state.page - 0); // re-check after refresh
            await refreshAll();
        } else if (res.status === 403) {
            uiService.showToast("Access denied. Admin role required.", "danger");
        } else {
            uiService.showToast("Failed to delete employee.", "danger");
        }
    });

}); // end document.ready
