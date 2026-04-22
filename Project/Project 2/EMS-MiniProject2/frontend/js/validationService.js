// validationService.js
// Client-side validation (runs before API calls to prevent unnecessary requests).
// mapServerErrors() translates API 409 Conflict responses into field-level errors.

var validationService = {

    // Validate employee add/edit form
    validateEmployeeForm: function (data, editId) {
        var errors = {};

        if (!data.firstName || data.firstName.trim() === "")
            errors.firstName = "First name is required.";

        if (!data.lastName || data.lastName.trim() === "")
            errors.lastName = "Last name is required.";

        if (!data.email || data.email.trim() === "") {
            errors.email = "Email is required.";
        } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(data.email)) {
            errors.email = "Please enter a valid email address.";
        }

        if (!data.phone || data.phone.trim() === "") {
            errors.phone = "Phone number is required.";
        } else if (!/^\d{10}$/.test(data.phone.trim())) {
            errors.phone = "Phone number must be exactly 10 digits.";
        }

        if (!data.department || data.department === "")
            errors.department = "Please select a department.";

        if (!data.designation || data.designation.trim() === "")
            errors.designation = "Designation is required.";

        if (!data.salary && data.salary !== 0) {
            errors.salary = "Salary is required.";
        } else if (isNaN(data.salary) || Number(data.salary) <= 0) {
            errors.salary = "Salary must be a positive number.";
        }

        if (!data.joinDate || data.joinDate === "")
            errors.joinDate = "Join date is required.";

        if (!data.status || data.status === "")
            errors.status = "Please select a status.";

        return errors;
    },

    // Validate login/signup form
    validateAuthForm: function (data, isSignup) {
        var errors = {};

        if (!data.username || data.username.trim() === "")
            errors.username = "Username is required.";

        if (!data.password || data.password.trim() === "") {
            errors.password = "Password is required.";
        } else if (isSignup && data.password.length < 6) {
            errors.password = "Password must be at least 6 characters.";
        }

        if (isSignup) {
            if (!data.confirmPassword || data.confirmPassword.trim() === "") {
                errors.confirmPassword = "Please confirm your password.";
            } else if (data.password !== data.confirmPassword) {
                errors.confirmPassword = "Passwords do not match.";
            }
        }

        return errors;
    },

    // Map API 409 Conflict or 400 error messages → field-level errors object
    mapServerErrors: function (message) {
        var errors = {};
        if (!message) return errors;

        var lower = message.toLowerCase();
        if (lower.indexOf("email") !== -1 && lower.indexOf("already") !== -1) {
            errors.email = "This email is already used by another employee.";
        } else if (lower.indexOf("username") !== -1 && lower.indexOf("already") !== -1) {
            errors.username = "Username already exists. Please choose another.";
        }
        return errors;
    }

};
