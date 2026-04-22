// authService.js
// Handles login, signup and logout against the real /api/auth/* endpoints.
// Stores the JWT token and user role in-memory (never localStorage — XSS risk).

var authService = (function () {

    // In-memory session — cleared on page refresh (correct secure behaviour)
    var _session = null; // { username, role, token }

    return {

        // Login — POST /api/auth/login
        login: async function (username, password) {
            try {
                var response = await fetch(API_BASE_URL + "/auth/login", {
                    method:  "POST",
                    headers: { "Content-Type": "application/json" },
                    body:    JSON.stringify({ username: username, password: password })
                });

                var data = await response.json();

                if (response.ok && data.success) {
                    _session = {
                        username: data.username,
                        role:     data.role,
                        token:    data.token
                    };
                    return { success: true, role: data.role };
                } else {
                    return { success: false, message: data.message || "Invalid username or password." };
                }
            } catch (err) {
                return { success: false, message: "Cannot connect to server. Is the API running?" };
            }
        },

        // Signup — POST /api/auth/register
        signup: async function (username, password, role) {
            try {
                var response = await fetch(API_BASE_URL + "/auth/register", {
                    method:  "POST",
                    headers: { "Content-Type": "application/json" },
                    body:    JSON.stringify({ username: username, password: password, role: role || "Viewer" })
                });

                var data = await response.json();

                if (response.ok && data.success) {
                    return { success: true, error: null };
                } else {
                    var msg = data.message || "Registration failed.";
                    return { success: false, error: msg };
                }
            } catch (err) {
                return { success: false, error: "Cannot connect to server. Is the API running?" };
            }
        },

        // Logout — clear session
        logout: function () {
            _session = null;
        },

        // Check if someone is logged in
        isLoggedIn: function () {
            return _session !== null && _session.token != null;
        },

        // Is the logged-in user an Admin?
        isAdmin: function () {
            return _session !== null && _session.role === "Admin";
        },

        // Get current username
        getCurrentUser: function () {
            return _session ? _session.username : null;
        },

        // Get current role
        getRole: function () {
            return _session ? _session.role : null;
        },

        // Get the JWT token (used by storageService to attach Authorization header)
        getToken: function () {
            return _session ? _session.token : null;
        }
    };

})();
