using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GymProject.Infrastructure.Migrations
{
    public partial class Test : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "GymApp");

            migrationBuilder.CreateTable(
                name: "AccountStatusType",
                schema: "GymApp",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountStatusType", x => x.Id);
                    table.UniqueConstraint("AK_AccountStatusType_Name", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "EntryStatusType",
                schema: "GymApp",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntryStatusType", x => x.Id);
                    table.UniqueConstraint("AK_EntryStatusType_Name", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "MuscleGroup",
                schema: "GymApp",
                columns: table => new
                {
                    Id = table.Column<uint>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Abbreviation = table.Column<string>(type: "TEXT", maxLength: 5, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MuscleGroup", x => x.Id);
                    table.UniqueConstraint("AK_MuscleGroup_Abbreviation", x => x.Abbreviation);
                    table.UniqueConstraint("AK_MuscleGroup_Name", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "TrainingEffortType",
                schema: "GymApp",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Abbreviation = table.Column<string>(type: "TEXT", maxLength: 4, nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingEffortType", x => x.Id);
                    table.UniqueConstraint("AK_TrainingEffortType_Abbreviation", x => x.Abbreviation);
                    table.UniqueConstraint("AK_TrainingEffortType_Name", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "TrainingPlanMessage",
                schema: "GymApp",
                columns: table => new
                {
                    Id = table.Column<uint>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Body = table.Column<string>(maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingPlanMessage", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TrainingPlanNote",
                schema: "GymApp",
                columns: table => new
                {
                    Id = table.Column<uint>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Body = table.Column<string>(maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingPlanNote", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TrainingPlanType",
                schema: "GymApp",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingPlanType", x => x.Id);
                    table.UniqueConstraint("AK_TrainingPlanType_Name", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "TrainingWeekType",
                schema: "GymApp",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingWeekType", x => x.Id);
                    table.UniqueConstraint("AK_TrainingWeekType_Name", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "WorkUnitTemplateNote",
                schema: "GymApp",
                columns: table => new
                {
                    Id = table.Column<uint>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Body = table.Column<string>(maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkUnitTemplateNote", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                schema: "GymApp",
                columns: table => new
                {
                    Id = table.Column<uint>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    Password = table.Column<string>(type: "TEXT", nullable: false),
                    Salt = table.Column<string>(type: "TEXT", nullable: false),
                    Username = table.Column<string>(type: "TEXT", nullable: false),
                    SubscriptionDate = table.Column<long>(type: "INTEGER", nullable: false, defaultValueSql: "strftime('%s', 'now')"),
                    AccountStatusTypeId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                    table.UniqueConstraint("AK_User_Email", x => x.Email);
                    table.UniqueConstraint("AK_User_Salt", x => x.Salt);
                    table.UniqueConstraint("AK_User_Username", x => x.Username);
                    table.ForeignKey(
                        name: "FK_User_AccountStatusType_AccountStatusTypeId",
                        column: x => x.AccountStatusTypeId,
                        principalSchema: "GymApp",
                        principalTable: "AccountStatusType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Excercise",
                schema: "GymApp",
                columns: table => new
                {
                    Id = table.Column<uint>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EntryStatusId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Excercise", x => x.Id);
                    table.UniqueConstraint("AK_Excercise_Name", x => x.Name);
                    table.ForeignKey(
                        name: "FK_Excercise_EntryStatusType_EntryStatusId",
                        column: x => x.EntryStatusId,
                        principalSchema: "GymApp",
                        principalTable: "EntryStatusType",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TrainingHashtag",
                schema: "GymApp",
                columns: table => new
                {
                    Id = table.Column<uint>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EntryStatusId = table.Column<int>(nullable: false),
                    Body = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingHashtag", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrainingHashtag_EntryStatusType_EntryStatusId",
                        column: x => x.EntryStatusId,
                        principalSchema: "GymApp",
                        principalTable: "EntryStatusType",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TrainingPhase",
                schema: "GymApp",
                columns: table => new
                {
                    Id = table.Column<uint>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EntryStatusId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingPhase", x => x.Id);
                    table.UniqueConstraint("AK_TrainingPhase_Name", x => x.Name);
                    table.ForeignKey(
                        name: "FK_TrainingPhase_EntryStatusType_EntryStatusId",
                        column: x => x.EntryStatusId,
                        principalSchema: "GymApp",
                        principalTable: "EntryStatusType",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TrainingProficiency",
                schema: "GymApp",
                columns: table => new
                {
                    Id = table.Column<uint>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EntryStatusId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingProficiency", x => x.Id);
                    table.UniqueConstraint("AK_TrainingProficiency_Name", x => x.Name);
                    table.ForeignKey(
                        name: "FK_TrainingProficiency_EntryStatusType_EntryStatusId",
                        column: x => x.EntryStatusId,
                        principalSchema: "GymApp",
                        principalTable: "EntryStatusType",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TrainingPlan",
                schema: "GymApp",
                columns: table => new
                {
                    Id = table.Column<uint>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    IsBookmarked = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0),
                    TrainingPlanNoteId = table.Column<uint>(nullable: true),
                    OwnerId = table.Column<uint>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingPlan", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrainingPlan_User_OwnerId",
                        column: x => x.OwnerId,
                        principalSchema: "GymApp",
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TrainingPlan_TrainingPlanNote_TrainingPlanNoteId",
                        column: x => x.TrainingPlanNoteId,
                        principalSchema: "GymApp",
                        principalTable: "TrainingPlanNote",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "TrainingPlanHashtag",
                schema: "GymApp",
                columns: table => new
                {
                    TrainingPlanId = table.Column<uint>(nullable: false),
                    HashtagId = table.Column<uint>(nullable: false),
                    ProgressiveNumber = table.Column<uint>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingPlanHashtag", x => new { x.TrainingPlanId, x.HashtagId });
                    table.ForeignKey(
                        name: "FK_TrainingPlanHashtag_TrainingHashtag_HashtagId",
                        column: x => x.HashtagId,
                        principalSchema: "GymApp",
                        principalTable: "TrainingHashtag",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TrainingPlanHashtag_TrainingPlan_TrainingPlanId",
                        column: x => x.TrainingPlanId,
                        principalSchema: "GymApp",
                        principalTable: "TrainingPlan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TrainingPlanMuscleFocus",
                schema: "GymApp",
                columns: table => new
                {
                    TrainingPlanId = table.Column<uint>(nullable: false),
                    MuscleGroupId = table.Column<uint>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingPlanMuscleFocus", x => new { x.TrainingPlanId, x.MuscleGroupId });
                    table.ForeignKey(
                        name: "FK_TrainingPlanMuscleFocus_TrainingHashtag_MuscleGroupId",
                        column: x => x.MuscleGroupId,
                        principalSchema: "GymApp",
                        principalTable: "TrainingHashtag",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TrainingPlanMuscleFocus_TrainingPlan_TrainingPlanId",
                        column: x => x.TrainingPlanId,
                        principalSchema: "GymApp",
                        principalTable: "TrainingPlan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TrainingPlanPhase",
                schema: "GymApp",
                columns: table => new
                {
                    TrainingPlanId = table.Column<uint>(nullable: false),
                    TrainingPhaseId = table.Column<uint>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingPlanPhase", x => new { x.TrainingPlanId, x.TrainingPhaseId });
                    table.ForeignKey(
                        name: "FK_TrainingPlanPhase_TrainingHashtag_TrainingPhaseId",
                        column: x => x.TrainingPhaseId,
                        principalSchema: "GymApp",
                        principalTable: "TrainingHashtag",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TrainingPlanPhase_TrainingPlan_TrainingPlanId",
                        column: x => x.TrainingPlanId,
                        principalSchema: "GymApp",
                        principalTable: "TrainingPlan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TrainingPlanProficiency",
                schema: "GymApp",
                columns: table => new
                {
                    TrainingPlanId = table.Column<uint>(nullable: false),
                    TrainingProficiencyId = table.Column<uint>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingPlanProficiency", x => new { x.TrainingPlanId, x.TrainingProficiencyId });
                    table.ForeignKey(
                        name: "FK_TrainingPlanProficiency_TrainingPlan_TrainingPlanId",
                        column: x => x.TrainingPlanId,
                        principalSchema: "GymApp",
                        principalTable: "TrainingPlan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TrainingPlanProficiency_TrainingProficiency_TrainingProficiencyId",
                        column: x => x.TrainingProficiencyId,
                        principalSchema: "GymApp",
                        principalTable: "TrainingProficiency",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TrainingPlanRelation",
                schema: "GymApp",
                columns: table => new
                {
                    ParentPlanId = table.Column<uint>(nullable: false),
                    ChildPlanId = table.Column<uint>(nullable: false),
                    MessageId = table.Column<uint>(nullable: true),
                    ChildPlanTypeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingPlanRelation", x => new { x.ParentPlanId, x.ChildPlanId });
                    table.ForeignKey(
                        name: "FK_TrainingPlanRelation_TrainingPlan_ChildPlanId",
                        column: x => x.ChildPlanId,
                        principalSchema: "GymApp",
                        principalTable: "TrainingPlan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TrainingPlanRelation_TrainingPlanType_ChildPlanTypeId",
                        column: x => x.ChildPlanTypeId,
                        principalSchema: "GymApp",
                        principalTable: "TrainingPlanType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TrainingPlanRelation_TrainingPlanMessage_MessageId",
                        column: x => x.MessageId,
                        principalSchema: "GymApp",
                        principalTable: "TrainingPlanMessage",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TrainingPlanRelation_TrainingPlan_ParentPlanId",
                        column: x => x.ParentPlanId,
                        principalSchema: "GymApp",
                        principalTable: "TrainingPlan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TrainingSchedule",
                schema: "GymApp",
                columns: table => new
                {
                    Id = table.Column<uint>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StartDate = table.Column<DateTime>(type: "INTEGER", nullable: true),
                    EndDate = table.Column<DateTime>(type: "INTEGER", nullable: true),
                    TrainingPlanId = table.Column<uint>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingSchedule", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrainingSchedule_TrainingPlan_TrainingPlanId",
                        column: x => x.TrainingPlanId,
                        principalSchema: "GymApp",
                        principalTable: "TrainingPlan",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TrainingWeek",
                schema: "GymApp",
                columns: table => new
                {
                    Id = table.Column<uint>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProgressiveNumber = table.Column<uint>(type: "INTEGER", nullable: false),
                    TrainingWeekTypeId = table.Column<int>(nullable: true),
                    TrainingPlanId = table.Column<uint>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingWeek", x => x.Id);
                    table.UniqueConstraint("AK_TrainingWeek_TrainingPlanId_ProgressiveNumber", x => new { x.TrainingPlanId, x.ProgressiveNumber });
                    table.ForeignKey(
                        name: "FK_TrainingWeek_TrainingPlan_TrainingPlanId",
                        column: x => x.TrainingPlanId,
                        principalSchema: "GymApp",
                        principalTable: "TrainingPlan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TrainingWeek_TrainingWeekType_TrainingWeekTypeId",
                        column: x => x.TrainingWeekTypeId,
                        principalSchema: "GymApp",
                        principalTable: "TrainingWeekType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "TrainingScheduleFeedback",
                schema: "GymApp",
                columns: table => new
                {
                    Id = table.Column<uint>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Comment = table.Column<string>(maxLength: 1000, nullable: true),
                    Rating = table.Column<float>(nullable: true),
                    UserId = table.Column<uint>(nullable: false),
                    TrainingScheduleId = table.Column<uint>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingScheduleFeedback", x => x.Id);
                    table.UniqueConstraint("AK_TrainingScheduleFeedback_TrainingScheduleId_UserId", x => new { x.TrainingScheduleId, x.UserId });
                    table.ForeignKey(
                        name: "FK_TrainingScheduleFeedback_TrainingSchedule_TrainingScheduleId",
                        column: x => x.TrainingScheduleId,
                        principalSchema: "GymApp",
                        principalTable: "TrainingSchedule",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TrainingScheduleFeedback_User_UserId",
                        column: x => x.UserId,
                        principalSchema: "GymApp",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkoutTemplate",
                schema: "GymApp",
                columns: table => new
                {
                    Id = table.Column<uint>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProgressiveNumber = table.Column<uint>(nullable: false),
                    TrainingWeekId = table.Column<uint>(nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    SpecificWeekday = table.Column<int>(nullable: true, defaultValue: 0)
                        .Annotation("Sqlite:Autoincrement", true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkoutTemplate", x => x.Id);
                    table.UniqueConstraint("AK_WorkoutTemplate_TrainingWeekId_ProgressiveNumber", x => new { x.TrainingWeekId, x.ProgressiveNumber });
                    table.ForeignKey(
                        name: "FK_WorkoutTemplate_TrainingWeek_TrainingWeekId",
                        column: x => x.TrainingWeekId,
                        principalSchema: "GymApp",
                        principalTable: "TrainingWeek",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkUnitTemplate",
                schema: "GymApp",
                columns: table => new
                {
                    Id = table.Column<uint>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProgressiveNumber = table.Column<uint>(type: "INTEGER", nullable: false),
                    WorkUnitNoteId = table.Column<uint>(nullable: true),
                    ExcerciseId = table.Column<uint>(nullable: false),
                    WorkoutTemplateId = table.Column<uint>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkUnitTemplate", x => x.Id);
                    table.UniqueConstraint("AK_WorkUnitTemplate_WorkoutTemplateId_ProgressiveNumber", x => new { x.WorkoutTemplateId, x.ProgressiveNumber });
                    table.ForeignKey(
                        name: "FK_WorkUnitTemplate_Excercise_ExcerciseId",
                        column: x => x.ExcerciseId,
                        principalSchema: "GymApp",
                        principalTable: "Excercise",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_WorkUnitTemplate_WorkUnitTemplateNote_WorkUnitNoteId",
                        column: x => x.WorkUnitNoteId,
                        principalSchema: "GymApp",
                        principalTable: "WorkUnitTemplateNote",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_WorkUnitTemplate_WorkoutTemplate_WorkoutTemplateId",
                        column: x => x.WorkoutTemplateId,
                        principalSchema: "GymApp",
                        principalTable: "WorkoutTemplate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkingSetTemplate",
                schema: "GymApp",
                columns: table => new
                {
                    Id = table.Column<uint>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProgressiveNumber = table.Column<uint>(type: "INTEGER", nullable: false),
                    TargetRepetitions = table.Column<int>(type: "INTEGER", nullable: true),
                    Rest = table.Column<int>(type: "INTEGER", nullable: true),
                    Cadence = table.Column<string>(type: "TEXT", nullable: true),
                    Effort = table.Column<float>(type: "INTEGER", nullable: true),
                    Effort_EffortTypeId = table.Column<int>(nullable: true),
                    WorkUnitTemplateId = table.Column<uint>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkingSetTemplate", x => x.Id);
                    table.UniqueConstraint("AK_WorkingSetTemplate_WorkUnitTemplateId_ProgressiveNumber", x => new { x.WorkUnitTemplateId, x.ProgressiveNumber });
                    table.ForeignKey(
                        name: "FK_WorkingSetTemplate_TrainingEffortType_Effort_EffortTypeId",
                        column: x => x.Effort_EffortTypeId,
                        principalSchema: "GymApp",
                        principalTable: "TrainingEffortType",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_WorkingSetTemplate_WorkUnitTemplate_WorkUnitTemplateId",
                        column: x => x.WorkUnitTemplateId,
                        principalSchema: "GymApp",
                        principalTable: "WorkUnitTemplate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "GymApp",
                table: "AccountStatusType",
                columns: new[] { "Id", "Name" },
                values: new object[] { 4, "Super" });

            migrationBuilder.InsertData(
                schema: "GymApp",
                table: "AccountStatusType",
                columns: new[] { "Id", "Name" },
                values: new object[] { 2, "Inactive" });

            migrationBuilder.InsertData(
                schema: "GymApp",
                table: "AccountStatusType",
                columns: new[] { "Id", "Name" },
                values: new object[] { 1, "Active" });

            migrationBuilder.InsertData(
                schema: "GymApp",
                table: "AccountStatusType",
                columns: new[] { "Id", "Name" },
                values: new object[] { 3, "Banned" });

            migrationBuilder.InsertData(
                schema: "GymApp",
                table: "EntryStatusType",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[] { 5, "Entry belonging to the DB release", "Native" });

            migrationBuilder.InsertData(
                schema: "GymApp",
                table: "EntryStatusType",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[] { 4, "Banned entry, visible to nobody", "Banned" });

            migrationBuilder.InsertData(
                schema: "GymApp",
                table: "EntryStatusType",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[] { 3, "Public entry visible to everyone", "Approved" });

            migrationBuilder.InsertData(
                schema: "GymApp",
                table: "EntryStatusType",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[] { 1, "The entry is visible only to the Owner", "Private" });

            migrationBuilder.InsertData(
                schema: "GymApp",
                table: "EntryStatusType",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[] { 2, "Public entry waiting for approval", "Pending" });

            migrationBuilder.InsertData(
                schema: "GymApp",
                table: "MuscleGroup",
                columns: new[] { "Id", "Abbreviation", "Name" },
                values: new object[] { 10u, "Quad", "Quadriceps" });

            migrationBuilder.InsertData(
                schema: "GymApp",
                table: "MuscleGroup",
                columns: new[] { "Id", "Abbreviation", "Name" },
                values: new object[] { 2u, "Delt", "Shoulders" });

            migrationBuilder.InsertData(
                schema: "GymApp",
                table: "MuscleGroup",
                columns: new[] { "Id", "Abbreviation", "Name" },
                values: new object[] { 3u, "Bis", "Biceps" });

            migrationBuilder.InsertData(
                schema: "GymApp",
                table: "MuscleGroup",
                columns: new[] { "Id", "Abbreviation", "Name" },
                values: new object[] { 9u, "Glute", "Glutes" });

            migrationBuilder.InsertData(
                schema: "GymApp",
                table: "MuscleGroup",
                columns: new[] { "Id", "Abbreviation", "Name" },
                values: new object[] { 4u, "Tris", "Triceps" });

            migrationBuilder.InsertData(
                schema: "GymApp",
                table: "MuscleGroup",
                columns: new[] { "Id", "Abbreviation", "Name" },
                values: new object[] { 5u, "FArm", "Forearms" });

            migrationBuilder.InsertData(
                schema: "GymApp",
                table: "MuscleGroup",
                columns: new[] { "Id", "Abbreviation", "Name" },
                values: new object[] { 1u, "Chest", "Chest" });

            migrationBuilder.InsertData(
                schema: "GymApp",
                table: "MuscleGroup",
                columns: new[] { "Id", "Abbreviation", "Name" },
                values: new object[] { 7u, "Back", "Back" });

            migrationBuilder.InsertData(
                schema: "GymApp",
                table: "MuscleGroup",
                columns: new[] { "Id", "Abbreviation", "Name" },
                values: new object[] { 8u, "Abs", "Abdomen" });

            migrationBuilder.InsertData(
                schema: "GymApp",
                table: "MuscleGroup",
                columns: new[] { "Id", "Abbreviation", "Name" },
                values: new object[] { 12u, "Calf", "Calves" });

            migrationBuilder.InsertData(
                schema: "GymApp",
                table: "MuscleGroup",
                columns: new[] { "Id", "Abbreviation", "Name" },
                values: new object[] { 11u, "Hams", "Hamstrings" });

            migrationBuilder.InsertData(
                schema: "GymApp",
                table: "MuscleGroup",
                columns: new[] { "Id", "Abbreviation", "Name" },
                values: new object[] { 6u, "Trap", "Trapezius" });

            migrationBuilder.InsertData(
                schema: "GymApp",
                table: "TrainingEffortType",
                columns: new[] { "Id", "Abbreviation", "Description", "Name" },
                values: new object[] { 1, "%", "Percentage of 1RM", "Intensity" });

            migrationBuilder.InsertData(
                schema: "GymApp",
                table: "TrainingEffortType",
                columns: new[] { "Id", "Abbreviation", "Description", "Name" },
                values: new object[] { 3, "RPE", "Self-assessed measure of the difficulty of a training set", "RPE" });

            migrationBuilder.InsertData(
                schema: "GymApp",
                table: "TrainingEffortType",
                columns: new[] { "Id", "Abbreviation", "Description", "Name" },
                values: new object[] { 2, "RM", "The most weight you can lift for a defined number of exercise movements", "RM" });

            migrationBuilder.InsertData(
                schema: "GymApp",
                table: "TrainingPlanType",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[] { 1, "Variant of another plan", "Variant" });

            migrationBuilder.InsertData(
                schema: "GymApp",
                table: "TrainingPlanType",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[] { 2, "Received by another user", "Inherited" });

            migrationBuilder.InsertData(
                schema: "GymApp",
                table: "TrainingWeekType",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[] { 1, "Variant of another plan", "Variant" });

            migrationBuilder.InsertData(
                schema: "GymApp",
                table: "TrainingWeekType",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[] { 2, "Received by another user", "Inherited" });

            migrationBuilder.CreateIndex(
                name: "IX_Excercise_EntryStatusId",
                schema: "GymApp",
                table: "Excercise",
                column: "EntryStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingHashtag_EntryStatusId",
                schema: "GymApp",
                table: "TrainingHashtag",
                column: "EntryStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingPhase_EntryStatusId",
                schema: "GymApp",
                table: "TrainingPhase",
                column: "EntryStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingPlan_TrainingPlanNoteId",
                schema: "GymApp",
                table: "TrainingPlan",
                column: "TrainingPlanNoteId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingPlan_OwnerId_TrainingPlanNoteId",
                schema: "GymApp",
                table: "TrainingPlan",
                columns: new[] { "OwnerId", "TrainingPlanNoteId" });

            migrationBuilder.CreateIndex(
                name: "IX_TrainingPlanHashtag_HashtagId",
                schema: "GymApp",
                table: "TrainingPlanHashtag",
                column: "HashtagId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingPlanMuscleFocus_MuscleGroupId",
                schema: "GymApp",
                table: "TrainingPlanMuscleFocus",
                column: "MuscleGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingPlanPhase_TrainingPhaseId",
                schema: "GymApp",
                table: "TrainingPlanPhase",
                column: "TrainingPhaseId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingPlanProficiency_TrainingProficiencyId",
                schema: "GymApp",
                table: "TrainingPlanProficiency",
                column: "TrainingProficiencyId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingPlanRelation_ChildPlanId",
                schema: "GymApp",
                table: "TrainingPlanRelation",
                column: "ChildPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingPlanRelation_ChildPlanTypeId",
                schema: "GymApp",
                table: "TrainingPlanRelation",
                column: "ChildPlanTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingPlanRelation_MessageId",
                schema: "GymApp",
                table: "TrainingPlanRelation",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingProficiency_EntryStatusId",
                schema: "GymApp",
                table: "TrainingProficiency",
                column: "EntryStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingSchedule_TrainingPlanId",
                schema: "GymApp",
                table: "TrainingSchedule",
                column: "TrainingPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingScheduleFeedback_UserId",
                schema: "GymApp",
                table: "TrainingScheduleFeedback",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingWeek_TrainingWeekTypeId",
                schema: "GymApp",
                table: "TrainingWeek",
                column: "TrainingWeekTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_User_AccountStatusTypeId",
                schema: "GymApp",
                table: "User",
                column: "AccountStatusTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkingSetTemplate_Effort_EffortTypeId",
                schema: "GymApp",
                table: "WorkingSetTemplate",
                column: "Effort_EffortTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkUnitTemplate_ExcerciseId",
                schema: "GymApp",
                table: "WorkUnitTemplate",
                column: "ExcerciseId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkUnitTemplate_WorkUnitNoteId",
                schema: "GymApp",
                table: "WorkUnitTemplate",
                column: "WorkUnitNoteId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MuscleGroup",
                schema: "GymApp");

            migrationBuilder.DropTable(
                name: "TrainingPhase",
                schema: "GymApp");

            migrationBuilder.DropTable(
                name: "TrainingPlanHashtag",
                schema: "GymApp");

            migrationBuilder.DropTable(
                name: "TrainingPlanMuscleFocus",
                schema: "GymApp");

            migrationBuilder.DropTable(
                name: "TrainingPlanPhase",
                schema: "GymApp");

            migrationBuilder.DropTable(
                name: "TrainingPlanProficiency",
                schema: "GymApp");

            migrationBuilder.DropTable(
                name: "TrainingPlanRelation",
                schema: "GymApp");

            migrationBuilder.DropTable(
                name: "TrainingScheduleFeedback",
                schema: "GymApp");

            migrationBuilder.DropTable(
                name: "WorkingSetTemplate",
                schema: "GymApp");

            migrationBuilder.DropTable(
                name: "TrainingHashtag",
                schema: "GymApp");

            migrationBuilder.DropTable(
                name: "TrainingProficiency",
                schema: "GymApp");

            migrationBuilder.DropTable(
                name: "TrainingPlanType",
                schema: "GymApp");

            migrationBuilder.DropTable(
                name: "TrainingPlanMessage",
                schema: "GymApp");

            migrationBuilder.DropTable(
                name: "TrainingSchedule",
                schema: "GymApp");

            migrationBuilder.DropTable(
                name: "TrainingEffortType",
                schema: "GymApp");

            migrationBuilder.DropTable(
                name: "WorkUnitTemplate",
                schema: "GymApp");

            migrationBuilder.DropTable(
                name: "Excercise",
                schema: "GymApp");

            migrationBuilder.DropTable(
                name: "WorkUnitTemplateNote",
                schema: "GymApp");

            migrationBuilder.DropTable(
                name: "WorkoutTemplate",
                schema: "GymApp");

            migrationBuilder.DropTable(
                name: "EntryStatusType",
                schema: "GymApp");

            migrationBuilder.DropTable(
                name: "TrainingWeek",
                schema: "GymApp");

            migrationBuilder.DropTable(
                name: "TrainingPlan",
                schema: "GymApp");

            migrationBuilder.DropTable(
                name: "TrainingWeekType",
                schema: "GymApp");

            migrationBuilder.DropTable(
                name: "User",
                schema: "GymApp");

            migrationBuilder.DropTable(
                name: "TrainingPlanNote",
                schema: "GymApp");

            migrationBuilder.DropTable(
                name: "AccountStatusType",
                schema: "GymApp");
        }
    }
}
