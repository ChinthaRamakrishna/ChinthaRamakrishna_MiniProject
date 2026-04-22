// authService.test.js
// Unit tests for authService
// Tests: signup, login, logout, session state

// ── Create a fresh authService for each test ───────────────────
function makeAuthService() {
    var admins = [{ username: "admin", password: "admin123" }];
    var loggedInUser = null;

    return {
        signup: function(username, password) {
            if (!username || username.trim() === "") {
                return { success: false, error: "Username is required." };
            }
            if (!password || password.length < 6) {
                return { success: false, error: "Password must be at least 6 characters." };
            }
            for (var i = 0; i < admins.length; i++) {
                if (admins[i].username === username) {
                    return { success: false, error: "Username already exists. Please choose another." };
                }
            }
            admins.push({ username: username, password: password });
            return { success: true, error: null };
        },
        login: function(username, password) {
            for (var i = 0; i < admins.length; i++) {
                if (admins[i].username === username && admins[i].password === password) {
                    loggedInUser = username;
                    return true;
                }
            }
            return false;
        },
        logout: function() { loggedInUser = null; },
        isLoggedIn: function() { return loggedInUser !== null; },
        getCurrentUser: function() { return loggedInUser; }
    };
}

var authService;

beforeEach(function() {
    authService = makeAuthService();
});

// ── signup tests ───────────────────────────────────────────────

describe("authService - signup", function() {
    test("should create a new admin successfully", function() {
        var result = authService.signup("newadmin", "pass123");
        expect(result.success).toBe(true);
        expect(result.error).toBeNull();
    });

    test("should reject duplicate username", function() {
        var result = authService.signup("admin", "anypass123");
        expect(result.success).toBe(false);
        expect(result.error).toMatch(/already exists/i);
    });

    test("should reject password shorter than 6 characters", function() {
        var result = authService.signup("bob", "123");
        expect(result.success).toBe(false);
        expect(result.error).toMatch(/6 characters/i);
    });

    test("newly signed up user can login", function() {
        authService.signup("charlie", "charlie123");
        var ok = authService.login("charlie", "charlie123");
        expect(ok).toBe(true);
    });
});

// ── login tests ────────────────────────────────────────────────

describe("authService - login", function() {
    test("should return true for correct credentials", function() {
        expect(authService.login("admin", "admin123")).toBe(true);
    });

    test("should return false for wrong password", function() {
        expect(authService.login("admin", "wrongpass")).toBe(false);
    });

    test("should return false for unknown username", function() {
        expect(authService.login("nobody", "admin123")).toBe(false);
    });

    test("should set session after successful login", function() {
        authService.login("admin", "admin123");
        expect(authService.isLoggedIn()).toBe(true);
        expect(authService.getCurrentUser()).toBe("admin");
    });

    test("should not set session after failed login", function() {
        authService.login("admin", "wrong");
        expect(authService.isLoggedIn()).toBe(false);
    });
});

// ── logout tests ───────────────────────────────────────────────

describe("authService - logout", function() {
    test("should clear session after logout", function() {
        authService.login("admin", "admin123");
        authService.logout();
        expect(authService.isLoggedIn()).toBe(false);
        expect(authService.getCurrentUser()).toBeNull();
    });

    test("isLoggedIn should be false before any login", function() {
        expect(authService.isLoggedIn()).toBe(false);
    });
});
