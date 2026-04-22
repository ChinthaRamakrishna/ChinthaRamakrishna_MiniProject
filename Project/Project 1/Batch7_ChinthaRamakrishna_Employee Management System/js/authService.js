// authService.js
// This service handles admin login, signup and logout.
// It keeps a list of registered admins and tracks who is logged in.

var registeredAdmins = [
    { username: adminCredentials.username, password: adminCredentials.password }
];

var currentUser = null; // null means no one is logged in

var authService = {

    // Sign up a new admin
    signup: function(username, password) {
        // Check if username already exists
        for (var i = 0; i < registeredAdmins.length; i++) {
            if (registeredAdmins[i].username === username) {
                return { success: false, error: "Username already exists. Please choose another." };
            }
        }
        // Save the new admin
        registeredAdmins.push({ username: username, password: password });
        return { success: true, error: null };
    },

    // Login with username and password
    login: function(username, password) {
        for (var i = 0; i < registeredAdmins.length; i++) {
            if (registeredAdmins[i].username === username && registeredAdmins[i].password === password) {
                currentUser = username;
                return true;
            }
        }
        return false;
    },

    // Logout - clear the session
    logout: function() {
        currentUser = null;
    },

    // Check if someone is logged in
    isLoggedIn: function() {
        return currentUser !== null;
    },

    // Get the current logged in username
    getCurrentUser: function() {
        return currentUser;
    }

};
