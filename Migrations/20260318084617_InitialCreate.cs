using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ProgrammeTracker.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppSettings",
                columns: table => new
                {
                    Key = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppSettings", x => x.Key);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Role = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProgrammeTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DefaultPeriodMonths = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgrammeTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Programmes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DeliveryType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ClientName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ContactPerson = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ContactDetails = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Stipend = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Bursary = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Funding = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    EEServices = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    WSPServices = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NumberOfLearners = table.Column<int>(type: "int", nullable: true),
                    AbsorptionAssistance = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DisabilityRatio = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    TypeOfDisability = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MaleCapacity = table.Column<int>(type: "int", nullable: true),
                    FemaleCapacity = table.Column<int>(type: "int", nullable: true),
                    OtherGenderCapacity = table.Column<int>(type: "int", nullable: true),
                    EEComposition = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreferredProvince = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AccountManagerId = table.Column<int>(type: "int", nullable: true),
                    ProgrammeTypeId = table.Column<int>(type: "int", nullable: false),
                    ProgrammeName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TopUpSkillsTraining = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    SoftSkillsTraining = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    ReportingFrequency = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LearnerRequiredAttributes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TrainingPeriodMonths = table.Column<int>(type: "int", nullable: true),
                    HostingPeriodMonths = table.Column<int>(type: "int", nullable: true),
                    WorkplacePeriodMonths = table.Column<int>(type: "int", nullable: true),
                    TotalPeriodMonths = table.Column<int>(type: "int", nullable: true),
                    AbsenteeismManagedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ReplacementPolicy = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    ReplacementTimeframe = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CoordinatorId = table.Column<int>(type: "int", nullable: true),
                    FacilitatorId = table.Column<int>(type: "int", nullable: true),
                    TalentManagerId = table.Column<int>(type: "int", nullable: true),
                    PayrollId = table.Column<int>(type: "int", nullable: true),
                    SETAAdvisorName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SETAAdvisorContact = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LearnerPresentation1Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LearnerPresentation2Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LearnerRegistrationDeadline = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PlacementDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SoftSkillsStartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModerationReportDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VerificationTriggerDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReportingIntervals = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Mentorship = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    SoftSkillsYN = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    AnyAddOn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AddOnComment = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Phase1Complete = table.Column<bool>(type: "bit", nullable: false),
                    Phase2Complete = table.Column<bool>(type: "bit", nullable: false),
                    IsLegacyImport = table.Column<bool>(type: "bit", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Programmes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Programmes_AspNetUsers_AccountManagerId",
                        column: x => x.AccountManagerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Programmes_AspNetUsers_CoordinatorId",
                        column: x => x.CoordinatorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Programmes_AspNetUsers_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Programmes_AspNetUsers_FacilitatorId",
                        column: x => x.FacilitatorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Programmes_AspNetUsers_PayrollId",
                        column: x => x.PayrollId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Programmes_AspNetUsers_TalentManagerId",
                        column: x => x.TalentManagerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Programmes_ProgrammeTypes_ProgrammeTypeId",
                        column: x => x.ProgrammeTypeId,
                        principalTable: "ProgrammeTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Deliverables",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProgrammeId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AssignedToId = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NotifyDaysBefore = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Deliverables", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Deliverables_AspNetUsers_AssignedToId",
                        column: x => x.AssignedToId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Deliverables_Programmes_ProgrammeId",
                        column: x => x.ProgrammeId,
                        principalTable: "Programmes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NotificationLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DeliverableId = table.Column<int>(type: "int", nullable: false),
                    SentToUserId = table.Column<int>(type: "int", nullable: true),
                    SentAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificationLogs_AspNetUsers_SentToUserId",
                        column: x => x.SentToUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_NotificationLogs_Deliverables_DeliverableId",
                        column: x => x.DeliverableId,
                        principalTable: "Deliverables",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AppSettings",
                columns: new[] { "Key", "Value" },
                values: new object[,]
                {
                    { "CheckIntervalSeconds", "3600" },
                    { "NotifyEnabled", "true" },
                    { "SmtpFromAddress", "" },
                    { "SmtpHost", "" },
                    { "SmtpPassword", "" },
                    { "SmtpPort", "587" },
                    { "SmtpUsername", "" }
                });

            migrationBuilder.InsertData(
                table: "ProgrammeTypes",
                columns: new[] { "Id", "DefaultPeriodMonths", "IsActive", "Name" },
                values: new object[,]
                {
                    { 1, 12, true, "Learnership" },
                    { 2, 18, true, "QTCO" },
                    { 3, 3, true, "Skills Programme" },
                    { 4, 12, true, "Internship" },
                    { 5, null, true, "Customised" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_Email",
                table: "AspNetUsers",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Deliverables_AssignedToId",
                table: "Deliverables",
                column: "AssignedToId");

            migrationBuilder.CreateIndex(
                name: "IX_Deliverables_ProgrammeId",
                table: "Deliverables",
                column: "ProgrammeId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationLogs_DeliverableId",
                table: "NotificationLogs",
                column: "DeliverableId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationLogs_SentToUserId",
                table: "NotificationLogs",
                column: "SentToUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Programmes_AccountManagerId",
                table: "Programmes",
                column: "AccountManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_Programmes_CoordinatorId",
                table: "Programmes",
                column: "CoordinatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Programmes_CreatedByUserId",
                table: "Programmes",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Programmes_FacilitatorId",
                table: "Programmes",
                column: "FacilitatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Programmes_PayrollId",
                table: "Programmes",
                column: "PayrollId");

            migrationBuilder.CreateIndex(
                name: "IX_Programmes_ProgrammeTypeId",
                table: "Programmes",
                column: "ProgrammeTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Programmes_TalentManagerId",
                table: "Programmes",
                column: "TalentManagerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppSettings");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "NotificationLogs");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Deliverables");

            migrationBuilder.DropTable(
                name: "Programmes");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "ProgrammeTypes");
        }
    }
}
