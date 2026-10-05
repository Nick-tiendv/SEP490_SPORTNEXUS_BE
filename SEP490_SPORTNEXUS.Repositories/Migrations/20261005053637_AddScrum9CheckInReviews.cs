using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SEP490_SPORTNEXUS_BE.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class AddScrum9CheckInReviews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCheckedIn",
                table: "TournamentParticipants",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "QrCode",
                table: "TournamentParticipants",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCheckedIn",
                table: "LfgParticipants",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "QrCode",
                table: "LfgParticipants",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCheckedIn",
                table: "BookingParticipants",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "QrCode",
                table: "BookingParticipants",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CheckInLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FacilityId = table.Column<Guid>(type: "uuid", nullable: false),
                    BookingParticipantId = table.Column<Guid>(type: "uuid", nullable: true),
                    LfgParticipantId = table.Column<Guid>(type: "uuid", nullable: true),
                    TournamentParticipantId = table.Column<Guid>(type: "uuid", nullable: true),
                    CheckedInAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CheckInLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CheckInLogs_BookingParticipants_BookingParticipantId",
                        column: x => x.BookingParticipantId,
                        principalTable: "BookingParticipants",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CheckInLogs_Facilities_FacilityId",
                        column: x => x.FacilityId,
                        principalTable: "Facilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CheckInLogs_LfgParticipants_LfgParticipantId",
                        column: x => x.LfgParticipantId,
                        principalTable: "LfgParticipants",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CheckInLogs_TournamentParticipants_TournamentParticipantId",
                        column: x => x.TournamentParticipantId,
                        principalTable: "TournamentParticipants",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "FairPlayRatings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ReviewerId = table.Column<Guid>(type: "uuid", nullable: false),
                    RevieweeId = table.Column<Guid>(type: "uuid", nullable: false),
                    ContextId = table.Column<Guid>(type: "uuid", nullable: false),
                    Score = table.Column<int>(type: "integer", nullable: false),
                    Comment = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FairPlayRatings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FairPlayRatings_Accounts_RevieweeId",
                        column: x => x.RevieweeId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FairPlayRatings_Accounts_ReviewerId",
                        column: x => x.ReviewerId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CheckInLogs_BookingParticipantId",
                table: "CheckInLogs",
                column: "BookingParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_CheckInLogs_FacilityId",
                table: "CheckInLogs",
                column: "FacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_CheckInLogs_LfgParticipantId",
                table: "CheckInLogs",
                column: "LfgParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_CheckInLogs_TournamentParticipantId",
                table: "CheckInLogs",
                column: "TournamentParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_FairPlayRatings_RevieweeId",
                table: "FairPlayRatings",
                column: "RevieweeId");

            migrationBuilder.CreateIndex(
                name: "IX_FairPlayRatings_ReviewerId",
                table: "FairPlayRatings",
                column: "ReviewerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CheckInLogs");

            migrationBuilder.DropTable(
                name: "FairPlayRatings");

            migrationBuilder.DropColumn(
                name: "IsCheckedIn",
                table: "TournamentParticipants");

            migrationBuilder.DropColumn(
                name: "QrCode",
                table: "TournamentParticipants");

            migrationBuilder.DropColumn(
                name: "IsCheckedIn",
                table: "LfgParticipants");

            migrationBuilder.DropColumn(
                name: "QrCode",
                table: "LfgParticipants");

            migrationBuilder.DropColumn(
                name: "IsCheckedIn",
                table: "BookingParticipants");

            migrationBuilder.DropColumn(
                name: "QrCode",
                table: "BookingParticipants");
        }
    }
}
