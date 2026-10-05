using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SEP490_SPORTNEXUS_BE.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class AddTournaments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tournaments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FacilityId = table.Column<Guid>(type: "uuid", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    ParentTournamentId = table.Column<Guid>(type: "uuid", nullable: true),
                    CategoryType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Format = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    MaxParticipants = table.Column<int>(type: "integer", nullable: false),
                    EntryFee = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tournaments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tournaments_Facilities_FacilityId",
                        column: x => x.FacilityId,
                        principalTable: "Facilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tournaments_Tournaments_ParentTournamentId",
                        column: x => x.ParentTournamentId,
                        principalTable: "Tournaments",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TournamentParticipants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TournamentId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    TeamId = table.Column<Guid>(type: "uuid", nullable: true),
                    PaymentStatus = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TournamentParticipants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TournamentParticipants_Accounts_UserId",
                        column: x => x.UserId,
                        principalTable: "Accounts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TournamentParticipants_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TournamentParticipants_Tournaments_TournamentId",
                        column: x => x.TournamentId,
                        principalTable: "Tournaments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TournamentPrizes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TournamentId = table.Column<Guid>(type: "uuid", nullable: false),
                    Position = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    RewardAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TournamentPrizes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TournamentPrizes_Tournaments_TournamentId",
                        column: x => x.TournamentId,
                        principalTable: "Tournaments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BracketMatches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TournamentId = table.Column<Guid>(type: "uuid", nullable: false),
                    CourtSlotId = table.Column<Guid>(type: "uuid", nullable: true),
                    RoundName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    MatchIndex = table.Column<int>(type: "integer", nullable: false),
                    Participant1Id = table.Column<Guid>(type: "uuid", nullable: true),
                    Participant2Id = table.Column<Guid>(type: "uuid", nullable: true),
                    WinnerId = table.Column<Guid>(type: "uuid", nullable: true),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BracketMatches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BracketMatches_CourtSlots_CourtSlotId",
                        column: x => x.CourtSlotId,
                        principalTable: "CourtSlots",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BracketMatches_TournamentParticipants_Participant1Id",
                        column: x => x.Participant1Id,
                        principalTable: "TournamentParticipants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BracketMatches_TournamentParticipants_Participant2Id",
                        column: x => x.Participant2Id,
                        principalTable: "TournamentParticipants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BracketMatches_TournamentParticipants_WinnerId",
                        column: x => x.WinnerId,
                        principalTable: "TournamentParticipants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BracketMatches_Tournaments_TournamentId",
                        column: x => x.TournamentId,
                        principalTable: "Tournaments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MatchScoreDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BracketMatchId = table.Column<Guid>(type: "uuid", nullable: false),
                    SetNumber = table.Column<int>(type: "integer", nullable: false),
                    ScoreP1 = table.Column<int>(type: "integer", nullable: false),
                    ScoreP2 = table.Column<int>(type: "integer", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchScoreDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MatchScoreDetails_BracketMatches_BracketMatchId",
                        column: x => x.BracketMatchId,
                        principalTable: "BracketMatches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BracketMatches_CourtSlotId",
                table: "BracketMatches",
                column: "CourtSlotId");

            migrationBuilder.CreateIndex(
                name: "IX_BracketMatches_Participant1Id",
                table: "BracketMatches",
                column: "Participant1Id");

            migrationBuilder.CreateIndex(
                name: "IX_BracketMatches_Participant2Id",
                table: "BracketMatches",
                column: "Participant2Id");

            migrationBuilder.CreateIndex(
                name: "IX_BracketMatches_TournamentId",
                table: "BracketMatches",
                column: "TournamentId");

            migrationBuilder.CreateIndex(
                name: "IX_BracketMatches_WinnerId",
                table: "BracketMatches",
                column: "WinnerId");

            migrationBuilder.CreateIndex(
                name: "IX_MatchScoreDetails_BracketMatchId",
                table: "MatchScoreDetails",
                column: "BracketMatchId");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentParticipants_TeamId",
                table: "TournamentParticipants",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentParticipants_TournamentId",
                table: "TournamentParticipants",
                column: "TournamentId");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentParticipants_UserId",
                table: "TournamentParticipants",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentPrizes_TournamentId",
                table: "TournamentPrizes",
                column: "TournamentId");

            migrationBuilder.CreateIndex(
                name: "IX_Tournaments_FacilityId",
                table: "Tournaments",
                column: "FacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_Tournaments_ParentTournamentId",
                table: "Tournaments",
                column: "ParentTournamentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MatchScoreDetails");

            migrationBuilder.DropTable(
                name: "TournamentPrizes");

            migrationBuilder.DropTable(
                name: "BracketMatches");

            migrationBuilder.DropTable(
                name: "TournamentParticipants");

            migrationBuilder.DropTable(
                name: "Tournaments");
        }
    }
}
