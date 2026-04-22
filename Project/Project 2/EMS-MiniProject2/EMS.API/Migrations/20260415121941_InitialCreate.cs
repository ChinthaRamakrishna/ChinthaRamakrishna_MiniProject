using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EMS.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Department = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Designation = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Salary = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    JoinDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "AppUsers",
                columns: new[] { "Id", "CreatedAt", "PasswordHash", "Role", "Username" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "$2a$11$ZnssI2G1wSwNp.4k5xvqh.svs6tR0HDWxmPHc34l/LTGCtP1l2Ljy", "Admin", "admin" },
                    { 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "$2a$11$D2xo.xPWwmqU0WTTgn2A2.wAh296gRQO5UWPy6AeOksldHTgboRTW", "Viewer", "viewer" }
                });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "CreatedAt", "Department", "Designation", "Email", "FirstName", "JoinDate", "LastName", "Phone", "Salary", "Status", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2021, 3, 15, 0, 0, 0, 0, DateTimeKind.Utc), "Engineering", "Software Engineer", "priya.prabhu@hexacore.com", "Priya", new DateTime(2021, 3, 15, 0, 0, 0, 0, DateTimeKind.Utc), "Prabhu", "9876543210", 850000m, "Active", new DateTime(2021, 3, 15, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, new DateTime(2020, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Marketing", "Marketing Executive", "arjun.sharma@hexacore.com", "Arjun", new DateTime(2020, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Sharma", "9123456780", 620000m, "Active", new DateTime(2020, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3, new DateTime(2019, 11, 20, 0, 0, 0, 0, DateTimeKind.Utc), "HR", "HR Executive", "neha.kapoor@hexacore.com", "Neha", new DateTime(2019, 11, 20, 0, 0, 0, 0, DateTimeKind.Utc), "Kapoor", "9988776655", 550000m, "Active", new DateTime(2019, 11, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4, new DateTime(2022, 1, 10, 0, 0, 0, 0, DateTimeKind.Utc), "Finance", "Financial Analyst", "rahul.verma@hexacore.com", "Rahul", new DateTime(2022, 1, 10, 0, 0, 0, 0, DateTimeKind.Utc), "Verma", "9001234567", 720000m, "Active", new DateTime(2022, 1, 10, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 5, new DateTime(2018, 6, 5, 0, 0, 0, 0, DateTimeKind.Utc), "Operations", "Operations Manager", "sneha.prasad@hexacore.com", "Sneha", new DateTime(2018, 6, 5, 0, 0, 0, 0, DateTimeKind.Utc), "Prasad", "9012345678", 950000m, "Active", new DateTime(2018, 6, 5, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 6, new DateTime(2017, 9, 12, 0, 0, 0, 0, DateTimeKind.Utc), "Engineering", "Senior Developer", "vikram.raj@hexacore.com", "Vikram", new DateTime(2017, 9, 12, 0, 0, 0, 0, DateTimeKind.Utc), "Raj", "9811223344", 1100000m, "Active", new DateTime(2017, 9, 12, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 7, new DateTime(2023, 2, 28, 0, 0, 0, 0, DateTimeKind.Utc), "Marketing", "Content Strategist", "ananya.singh@hexacore.com", "Ananya", new DateTime(2023, 2, 28, 0, 0, 0, 0, DateTimeKind.Utc), "Singh", "9922334455", 580000m, "Inactive", new DateTime(2023, 2, 28, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 8, new DateTime(2020, 4, 17, 0, 0, 0, 0, DateTimeKind.Utc), "Finance", "Accounts Manager", "karthik.rajan@hexacore.com", "Karthik", new DateTime(2020, 4, 17, 0, 0, 0, 0, DateTimeKind.Utc), "Rajan", "9876001234", 800000m, "Active", new DateTime(2020, 4, 17, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 9, new DateTime(2021, 8, 9, 0, 0, 0, 0, DateTimeKind.Utc), "HR", "Talent Acquisition", "divya.nair@hexacore.com", "Divya", new DateTime(2021, 8, 9, 0, 0, 0, 0, DateTimeKind.Utc), "Nair", "9754123456", 530000m, "Inactive", new DateTime(2021, 8, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 10, new DateTime(2019, 3, 22, 0, 0, 0, 0, DateTimeKind.Utc), "Engineering", "DevOps Engineer", "rohan.mehta@hexacore.com", "Rohan", new DateTime(2019, 3, 22, 0, 0, 0, 0, DateTimeKind.Utc), "Mehta", "9654987321", 920000m, "Active", new DateTime(2019, 3, 22, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 11, new DateTime(2022, 5, 14, 0, 0, 0, 0, DateTimeKind.Utc), "Operations", "Supply Chain Analyst", "kavya.iyer@hexacore.com", "Kavya", new DateTime(2022, 5, 14, 0, 0, 0, 0, DateTimeKind.Utc), "Iyer", "9543219876", 670000m, "Active", new DateTime(2022, 5, 14, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 12, new DateTime(2020, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), "Finance", "Tax Consultant", "suresh.babu@hexacore.com", "Suresh", new DateTime(2020, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), "Babu", "9432187654", 750000m, "Inactive", new DateTime(2020, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 13, new DateTime(2018, 12, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Marketing", "Brand Manager", "lakshmi.c@hexacore.com", "Lakshmi", new DateTime(2018, 12, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Chandran", "9321654321", 880000m, "Active", new DateTime(2018, 12, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 14, new DateTime(2023, 7, 10, 0, 0, 0, 0, DateTimeKind.Utc), "Operations", "Supply Chain Analyst", "amit.joshi@hexacore.com", "Amit", new DateTime(2023, 7, 10, 0, 0, 0, 0, DateTimeKind.Utc), "Joshi", "9210543219", 610000m, "Active", new DateTime(2023, 7, 10, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 15, new DateTime(2024, 1, 5, 0, 0, 0, 0, DateTimeKind.Utc), "Engineering", "DevOps Engineer", "pooja.ghosh@hexacore.com", "Pooja", new DateTime(2024, 1, 5, 0, 0, 0, 0, DateTimeKind.Utc), "Ghosh", "9109432108", 990000m, "Active", new DateTime(2024, 1, 5, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppUsers_Username",
                table: "AppUsers",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_Email",
                table: "Employees",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppUsers");

            migrationBuilder.DropTable(
                name: "Employees");
        }
    }
}
