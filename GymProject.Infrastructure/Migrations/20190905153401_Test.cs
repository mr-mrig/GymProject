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
                    Id = table.Column<long>(nullable: false),
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
                    Id = table.Column<int>(nullable: false, defaultValue: 1),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingPlanType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TrainingHashtag",
                schema: "GymApp",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    EntryStatusId = table.Column<int>(nullable: true),
                    Body = table.Column<string>(type: "TEXT", nullable: false)
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
                name: "IdTypeValue",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TrainingPlanRootId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdTypeValue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IdTypeValue_TrainingPlan_TrainingPlanRootId",
                        column: x => x.TrainingPlanRootId,
                        principalSchema: "GymApp",
                        principalTable: "TrainingPlan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TrainingPlanHashtag",
                schema: "GymApp",
                columns: table => new
                {
                    TrainingPlanId = table.Column<long>(nullable: false),
                    HashtagId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingPlanHashtag", x => new { x.TrainingPlanId, x.HashtagId });
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
                    ParentPlanId = table.Column<long>(nullable: false),
                    ChildPlanId = table.Column<long>(nullable: false),
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
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TrainingPlanRelation_TrainingPlan_ParentPlanId",
                        column: x => x.ParentPlanId,
                        principalSchema: "GymApp",
                        principalTable: "TrainingPlan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IdTypeValue_TrainingPlanRootId",
                table: "IdTypeValue",
                column: "TrainingPlanRootId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingHashtag_EntryStatusId",
                schema: "GymApp",
                table: "TrainingHashtag",
                column: "EntryStatusId");

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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IdTypeValue");

            migrationBuilder.DropTable(
                name: "TrainingHashtag",
                schema: "GymApp");

            migrationBuilder.DropTable(
                name: "TrainingPlanHashtag",
                schema: "GymApp");

            migrationBuilder.DropTable(
                name: "TrainingPlanRelation",
                schema: "GymApp");

            migrationBuilder.DropTable(
                name: "EntryStatusType",
                schema: "GymApp");

            migrationBuilder.DropTable(
                name: "TrainingPlan",
                schema: "GymApp");

            migrationBuilder.DropTable(
                name: "TrainingPlanType",
                schema: "GymApp");
        }
    }
}
