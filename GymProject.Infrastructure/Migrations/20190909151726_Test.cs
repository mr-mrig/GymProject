﻿using System;
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
                name: "User",
                schema: "GymApp",
                columns: table => new
                {
                    Id = table.Column<uint>(nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    Password = table.Column<string>(type: "TEXT", nullable: false),
                    Salt = table.Column<string>(type: "TEXT", nullable: false),
                    Username = table.Column<string>(type: "TEXT", nullable: false),
                    SubscriptionDate = table.Column<DateTime>(type: "INTEGER", nullable: false, defaultValueSql: "strftime('%s', 'now')"),
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
                name: "TrainingHashtag",
                schema: "GymApp",
                columns: table => new
                {
                    Id = table.Column<uint>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EntryStatusId = table.Column<int>(nullable: true),
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
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                        name: "FK_TrainingPlan_TrainingPlanMessage_TrainingPlanNoteId",
                        column: x => x.TrainingPlanNoteId,
                        principalSchema: "GymApp",
                        principalTable: "TrainingPlanMessage",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TrainingPlanHashtag",
                schema: "GymApp",
                columns: table => new
                {
                    TrainingPlanId = table.Column<uint>(nullable: false),
                    HashtagId = table.Column<uint>(nullable: false)
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

            migrationBuilder.InsertData(
                schema: "GymApp",
                table: "AccountStatusType",
                columns: new[] { "Id", "Name" },
                values: new object[] { 1, "Active" });

            migrationBuilder.InsertData(
                schema: "GymApp",
                table: "AccountStatusType",
                columns: new[] { "Id", "Name" },
                values: new object[] { 2, "Inactive" });

            migrationBuilder.InsertData(
                schema: "GymApp",
                table: "AccountStatusType",
                columns: new[] { "Id", "Name" },
                values: new object[] { 3, "Banned" });

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
                table: "EntryStatusType",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[] { 3, "Public entry visible to everyone", "Approved" });

            migrationBuilder.InsertData(
                schema: "GymApp",
                table: "EntryStatusType",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[] { 4, "Banned entry, visible to nobody", "Banned" });

            migrationBuilder.InsertData(
                schema: "GymApp",
                table: "EntryStatusType",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[] { 5, "Entry belonging to the DB release", "Native" });

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
                name: "IX_TrainingHashtag_EntryStatusId",
                schema: "GymApp",
                table: "TrainingHashtag",
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
                name: "IX_User_AccountStatusTypeId",
                schema: "GymApp",
                table: "User",
                column: "AccountStatusTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TrainingPlanHashtag",
                schema: "GymApp");

            migrationBuilder.DropTable(
                name: "TrainingPlanNote",
                schema: "GymApp");

            migrationBuilder.DropTable(
                name: "TrainingPlanRelation",
                schema: "GymApp");

            migrationBuilder.DropTable(
                name: "TrainingScheduleFeedback",
                schema: "GymApp");

            migrationBuilder.DropTable(
                name: "TrainingHashtag",
                schema: "GymApp");

            migrationBuilder.DropTable(
                name: "TrainingPlanType",
                schema: "GymApp");

            migrationBuilder.DropTable(
                name: "TrainingSchedule",
                schema: "GymApp");

            migrationBuilder.DropTable(
                name: "EntryStatusType",
                schema: "GymApp");

            migrationBuilder.DropTable(
                name: "TrainingPlan",
                schema: "GymApp");

            migrationBuilder.DropTable(
                name: "User",
                schema: "GymApp");

            migrationBuilder.DropTable(
                name: "TrainingPlanMessage",
                schema: "GymApp");

            migrationBuilder.DropTable(
                name: "AccountStatusType",
                schema: "GymApp");
        }
    }
}
