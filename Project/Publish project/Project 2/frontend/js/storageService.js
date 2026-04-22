// storageService.js
// ALL methods now make real fetch() calls to the .NET 8 Web API.
// Public method names are identical to Mini Project 1 — only the
// implementation inside changed. This is the ONLY data-layer change.

var storageService = (function () {

    // Build Authorization header for every authenticated request
    function _headers(withAuth) {
        if (withAuth === undefined) withAuth = true;
        var headers = { "Content-Type": "application/json" };
        if (withAuth) {
            var token = authService.getToken();
            if (token) headers["Authorization"] = "Bearer " + token;
        }
        return headers;
    }

    // Generic error handler — returns { ok, status, data }
    async function _request(method, url, body) {
        try {
            var opts = {
                method:  method,
                headers: _headers()
            };
            if (body !== undefined) opts.body = JSON.stringify(body);

            var response = await fetch(url, opts);
            var data = null;
            try { data = await response.json(); } catch (_) {}
            return { ok: response.ok, status: response.status, data: data };
        } catch (err) {
            return { ok: false, status: 0, data: { message: "Network error: " + err.message } };
        }
    }

    return {

        // ── Employee CRUD ─────────────────────────────────────────────────────

        // GET /api/employees with query params (paged result)
        getAll: async function (params) {
            params = params || {};
            var qs = new URLSearchParams();
            if (params.search)     qs.set("search",     params.search);
            if (params.department) qs.set("department",  params.department);
            if (params.status)     qs.set("status",      params.status);
            if (params.sortBy)     qs.set("sortBy",      params.sortBy);
            if (params.sortDir)    qs.set("sortDir",     params.sortDir);
            qs.set("page",     params.page     || 1);
            qs.set("pageSize", params.pageSize || PAGE_SIZE);

            var res = await _request("GET", API_BASE_URL + "/employees?" + qs.toString());
            return res; // { ok, status, data: PagedResult }
        },

        // GET /api/employees/:id
        getById: async function (id) {
            return await _request("GET", API_BASE_URL + "/employees/" + id);
        },

        // POST /api/employees
        add: async function (employeeData) {
            return await _request("POST", API_BASE_URL + "/employees", employeeData);
        },

        // PUT /api/employees/:id
        update: async function (id, employeeData) {
            return await _request("PUT", API_BASE_URL + "/employees/" + id, employeeData);
        },

        // DELETE /api/employees/:id
        remove: async function (id) {
            return await _request("DELETE", API_BASE_URL + "/employees/" + id);
        },

        // GET /api/employees/dashboard
        getDashboard: async function () {
            return await _request("GET", API_BASE_URL + "/employees/dashboard");
        }
    };

})();
