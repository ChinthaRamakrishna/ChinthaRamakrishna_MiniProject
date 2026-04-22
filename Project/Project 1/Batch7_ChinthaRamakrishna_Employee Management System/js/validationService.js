// validationService.js
// This service checks if form data is valid before saving.
// It returns an object with error messages for each invalid field.

var validationService = {

    // Validate employee add/edit form
    validateEmployeeForm: function(data, editId) {
        var errors = {};

        // First name required
        if (!data.firstName || data.firstName.trim() === "") {
            errors.firstName = "First name is required.";
        }

        // Last name required
        if (!data.lastName || data.lastName.trim() === "") {
            errors.lastName = "Last name is required.";
        }

        // Email required and valid format
        if (!data.email || data.email.trim() === "") {
            errors.email = "Email is required.";
        } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(data.email)) {
            errors.email = "Please enter a valid email address.";
        } else {
            // Check for duplicate email
            var allEmps = storageService.getAll();
            for (var i = 0; i < allEmps.length; i++) {
                if (allEmps[i].email.toLowerCase() === data.email.toLowerCase() && allEmps[i].id != editId) {
                    errors.email = "This email is already used by another employee.";
                    break;
                }
            }
        }

        // Phone must be exactly 10 digits
        if (!data.phone || data.phone.trim() === "") {
            errors.phone = "Phone number is required.";
        } else if (!/^\d{10}$/.test(data.phone.trim())) {
            errors.phone = "Phone number must be exactly 10 digits.";
        }

        // Department required
        if (!data.department || data.department === "") {
            errors.department = "Please select a department.";
        }

        // Designation required
        if (!data.designation || data.designation.trim() === "") {
            errors.designation = "Designation is required.";
        }

        // Salary must be a positive number
        if (!data.salary && data.salary !== 0) {
            errors.salary = "Salary is required.";
        } else if (isNaN(data.salary) || Number(data.salary) <= 0) {
            errors.salary = "Salary must be a positive number.";
        }

        // Join date required
        if (!data.joinDate || data.joinDate === "") {
            errors.joinDate = "Join date is required.";
        }

        // Status required
        if (!data.status || data.status === "") {
            errors.status = "Please select a status.";
        }

        return errors;
    },

    // Validate login/signup form
    validateAuthForm: function(data, isSignup) {
        var errors = {};

        if (!data.username || data.username.trim() === "") {
            errors.username = "Username is required.";
        }

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
    }

};
