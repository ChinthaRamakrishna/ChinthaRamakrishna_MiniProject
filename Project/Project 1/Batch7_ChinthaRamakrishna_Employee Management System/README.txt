Employee Management System
==========================
Name   : Chintha Ramakrishna
Batch  : Batch 7

HOW TO RUN THE APP:
  1. Unzip the folder
  2. Open index.html directly in Chrome or any browser
  3. Default login: username = admin  |  password = admin123

HOW TO RUN TESTS:
  1. Make sure Node.js is installed on your computer
  2. Open terminal / command prompt in the project folder
  3. Run: npm install
  4. Run: npm test

PROJECT STRUCTURE:
  index.html               - Main HTML file (all pages in one file)
  css/styles.css           - All custom CSS styles
  js/data.js               - Employee data and admin credentials
  js/storageService.js     - Handles reading and writing employee data
  js/authService.js        - Handles login, signup and logout
  js/employeeService.js    - Employee business logic (search, filter, sort)
  js/validationService.js  - Form validation logic
  js/dashboardService.js   - Dashboard summary calculations
  js/uiService.js          - All DOM rendering functions
  js/app.js                - Event listeners and page navigation
  tests/                   - Jest unit test files
  package.json             - Node project config
  jest.config.js           - Jest test configuration
