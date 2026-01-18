using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bizonet.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddReferralTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FollowupStatuses",
                columns: table => new
                {
                    FollowupStatusId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FollowupStatusName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    So = table.Column<byte>(type: "tinyint", nullable: false, defaultValue: (byte)0),
                    Active = table.Column<byte>(type: "tinyint", nullable: false, defaultValue: (byte)1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FollowupStatuses", x => x.FollowupStatusId);
                });

            migrationBuilder.CreateTable(
                name: "Referrals",
                columns: table => new
                {
                    ReferralId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReferredByUserId = table.Column<int>(type: "int", nullable: true),
                    ReferredToUserId = table.Column<int>(type: "int", nullable: true),
                    ReferralType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GroupId = table.Column<int>(type: "int", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Referrals", x => x.ReferralId);
                    table.ForeignKey(
                        name: "FK_Referrals_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "GroupId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Referrals_Users_ReferredByUserId",
                        column: x => x.ReferredByUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Referrals_Users_ReferredToUserId",
                        column: x => x.ReferredToUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FollowupComments",
                columns: table => new
                {
                    FollowupCommentsId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FollowupStatusId = table.Column<int>(type: "int", nullable: true),
                    FollowupCommentsName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    So = table.Column<byte>(type: "tinyint", nullable: false, defaultValue: (byte)0),
                    Active = table.Column<byte>(type: "tinyint", nullable: false, defaultValue: (byte)1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FollowupComments", x => x.FollowupCommentsId);
                    table.ForeignKey(
                        name: "FK_FollowupComments_FollowupStatuses_FollowupStatusId",
                        column: x => x.FollowupStatusId,
                        principalTable: "FollowupStatuses",
                        principalColumn: "FollowupStatusId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReferralOutsiders",
                columns: table => new
                {
                    OutsiderId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReferralId = table.Column<int>(type: "int", nullable: true),
                    ReferredTo = table.Column<int>(type: "int", nullable: true),
                    OutsiderName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OutsiderMobileNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OutsiderEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OutsiderAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReferralOutsiders", x => x.OutsiderId);
                    table.ForeignKey(
                        name: "FK_ReferralOutsiders_Referrals_ReferralId",
                        column: x => x.ReferralId,
                        principalTable: "Referrals",
                        principalColumn: "ReferralId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReferralStatuses",
                columns: table => new
                {
                    ReferralStatusId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReferralId = table.Column<int>(type: "int", nullable: true),
                    ReferredByUserId = table.Column<int>(type: "int", nullable: true),
                    ReferredToUserId = table.Column<int>(type: "int", nullable: true),
                    GroupId = table.Column<int>(type: "int", nullable: true),
                    ReferralReceivedStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RejectReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FollowupStatusId = table.Column<int>(type: "int", nullable: true),
                    FollowupCommentsId = table.Column<int>(type: "int", nullable: true),
                    FollowupNextDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReferralStatuses", x => x.ReferralStatusId);
                    table.ForeignKey(
                        name: "FK_ReferralStatuses_FollowupComments_FollowupCommentsId",
                        column: x => x.FollowupCommentsId,
                        principalTable: "FollowupComments",
                        principalColumn: "FollowupCommentsId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReferralStatuses_FollowupStatuses_FollowupStatusId",
                        column: x => x.FollowupStatusId,
                        principalTable: "FollowupStatuses",
                        principalColumn: "FollowupStatusId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReferralStatuses_Referrals_ReferralId",
                        column: x => x.ReferralId,
                        principalTable: "Referrals",
                        principalColumn: "ReferralId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FollowupComments_FollowupStatusId",
                table: "FollowupComments",
                column: "FollowupStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_ReferralOutsiders_ReferralId",
                table: "ReferralOutsiders",
                column: "ReferralId",
                unique: true,
                filter: "[ReferralId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Referrals_GroupId",
                table: "Referrals",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Referrals_ReferredByUserId",
                table: "Referrals",
                column: "ReferredByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Referrals_ReferredToUserId",
                table: "Referrals",
                column: "ReferredToUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ReferralStatuses_FollowupCommentsId",
                table: "ReferralStatuses",
                column: "FollowupCommentsId");

            migrationBuilder.CreateIndex(
                name: "IX_ReferralStatuses_FollowupStatusId",
                table: "ReferralStatuses",
                column: "FollowupStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_ReferralStatuses_ReferralId",
                table: "ReferralStatuses",
                column: "ReferralId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReferralOutsiders");

            migrationBuilder.DropTable(
                name: "ReferralStatuses");

            migrationBuilder.DropTable(
                name: "FollowupComments");

            migrationBuilder.DropTable(
                name: "Referrals");

            migrationBuilder.DropTable(
                name: "FollowupStatuses");
        }
    }
}
