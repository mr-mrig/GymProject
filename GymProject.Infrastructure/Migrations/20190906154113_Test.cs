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
                name: "TrainingPlan",
                schema: "GymApp",
                columns: table => new
                {
                    Id = table.Column<uint>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    IsBookmarked = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingPlan", x => x.Id);
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
                name: "TrainingSchedule",
                schema: "GymApp",
                columns: table => new
                {
                    Id = table.Column<uint>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: false),
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
                        name: "FK_TrainingPlanRelation_TrainingPlan_ParentPlanId",
                        column: x => x.ParentPlanId,
                        principalSchema: "GymApp",
                        principalTable: "TrainingPlan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TrainingScheduleFeedback",
                schema: "GymApp",
                columns: table => new
                {
                    Id = table.Column<uint>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Comment = table.Column<string>(maxLength: 1000, nullable: true),
                    Rating = table.Column<float>(nullable: false),
                    TrainingScheduleId = table.Column<uint>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingScheduleFeedback", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrainingScheduleFeedback_TrainingSchedule_TrainingScheduleId",
                        column: x => x.TrainingScheduleId,
                        principalSchema: "GymApp",
                        principalTable: "TrainingSchedule",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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
                name: "IX_TrainingSchedule_TrainingPlanId",
                schema: "GymApp",
                table: "TrainingSchedule",
                column: "TrainingPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingScheduleFeedback_TrainingScheduleId",
                schema: "GymApp",
                table: "TrainingScheduleFeedback",
                column: "TrainingScheduleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EntryStatusType",
                schema: "GymApp");

            migrationBuilder.DropTable(
                name: "TrainingPlanRelation",
                schema: "GymApp");

            migrationBuilder.DropTable(
                name: "TrainingScheduleFeedback",
                schema: "GymApp");

            migrationBuilder.DropTable(
                name: "TrainingPlanType",
                schema: "GymApp");

            migrationBuilder.DropTable(
                name: "TrainingSchedule",
                schema: "GymApp");

            migrationBuilder.DropTable(
                name: "TrainingPlan",
                schema: "GymApp");
        }
    }
}
