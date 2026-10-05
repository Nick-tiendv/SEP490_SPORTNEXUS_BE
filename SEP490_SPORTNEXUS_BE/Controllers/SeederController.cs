using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SEP490_SPORTNEXUS_BE.Repositories;
using SEP490_SPORTNEXUS_BE.Repositories.Entities.Identities;
using SEP490_SPORTNEXUS_BE.Repositories.Entities.Facilities;
using SEP490_SPORTNEXUS_BE.Repositories.Entities.Finances;
using SEP490_SPORTNEXUS_BE.Repositories.Entities.Bookings;
using SEP490_SPORTNEXUS_BE.Repositories.Entities.Socials;
using SEP490_SPORTNEXUS_BE.Repositories.Entities.Tournaments;
using SEP490_SPORTNEXUS_BE.Repositories.Entities.AiAssistant;
using SEP490_SPORTNEXUS_BE.Repositories.Entities.System;
using SEP490_SPORTNEXUS_BE.Repositories.Entities.MasterData;
using SEP490_SPORTNEXUS_BE.Repositories.Enums;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SEP490_SPORTNEXUS_BE.API.Controllers
{
    [Route("api/v1/seeder")]
    [ApiController]
    public class SeederController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SeederController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("run")]
        public async Task<IActionResult> RunSeeder()
        {
            try
            {
                var rand = new Random();
                var roles = await _context.Roles.ToListAsync();
                if (roles.Count == 0) return BadRequest("No roles.");
                var playerRole = roles.First(r => r.Name == "Player");
                var ownerRole = roles.First(r => r.Name == "Owner");

                // 1. ACCOUNTS & WALLETS
                var accounts = new List<Account>();
                var wallets = new List<Wallet>();
                for (int i = 0; i < 10; i++)
                {
                    var acc = new Account {
                        Id = Guid.NewGuid(),
                        Username = "user" + i,
                        Phone = "090000000" + i,
                        Email = $"user{i}@test.com",
                        PasswordHash = "hash",
                        FullName = "Test User " + i,
                        RoleId = i < 3 ? ownerRole.Id : playerRole.Id,
                        FairPlayScore = 5.0m
                    };
                    accounts.Add(acc);
                    wallets.Add(new Wallet { Id = Guid.NewGuid(), UserId = acc.Id, Balance = 1000000m });
                }
                await _context.Accounts.AddRangeAsync(accounts);
                await _context.Wallets.AddRangeAsync(wallets);

                // 3. MASTER DATA CATEGORIES
                var categories = new List<SportCategory>();
                string[] sports = { "Cầu Lông", "Bóng Đá", "Tennis", "Bóng Rổ", "Pickleball" };
                for (int i = 0; i < 10; i++)
                {
                    categories.Add(new SportCategory { Id = Guid.NewGuid(), Name = sports[i % 5] + " " + i });
                }
                await _context.Set<SportCategory>().AddRangeAsync(categories);

                // 4. FACILITIES & IMAGES & REVIEWS
                var facilities = new List<Facility>();
                var images = new List<FacilityImage>();
                var reviews = new List<FacilityReview>();
                var owners = accounts.Where(a => a.RoleId == ownerRole.Id).ToList();
                if (owners.Count == 0) owners = accounts;

                for (int i = 0; i < 10; i++)
                {
                    var fac = new Facility {
                        Id = Guid.NewGuid(),
                        OwnerId = owners[i % owners.Count].Id,
                        Name = "Sân " + i,
                        Address = "Hồ Chí Minh",
                        Location = new Point(106.6 + rand.NextDouble(), 10.7 + rand.NextDouble()) { SRID = 4326 },
                        Status = "Active"
                    };
                    facilities.Add(fac);
                    images.Add(new FacilityImage { Id = Guid.NewGuid(), FacilityId = fac.Id, ImageUrl = "url" });
                    reviews.Add(new FacilityReview { Id = Guid.NewGuid(), FacilityId = fac.Id, UserId = accounts[i].Id, Rating = 5, Comment = "Good" });
                }
                await _context.Facilities.AddRangeAsync(facilities);
                await _context.Set<FacilityImage>().AddRangeAsync(images);
                await _context.Set<FacilityReview>().AddRangeAsync(reviews);

                // 7. COURTS & SLOTS
                var courts = new List<Court>();
                var slots = new List<CourtSlot>();
                for (int i = 0; i < 10; i++)
                {
                    var c = new Court { Id = Guid.NewGuid(), FacilityId = facilities[i].Id, CategoryId = categories[i].Id, Name = "Cụm " + i, DefaultPrice = 100000m };
                    courts.Add(c);
                    slots.Add(new CourtSlot { Id = Guid.NewGuid(), CourtId = c.Id, StartTime = DateTime.UtcNow.AddHours(i), EndTime = DateTime.UtcNow.AddHours(i+1), Price = 100000m, Status = CourtSlotStatus.Booked });
                }
                await _context.Courts.AddRangeAsync(courts);
                await _context.CourtSlots.AddRangeAsync(slots);

                // 9. BOOKINGS & PARTICIPANTS & CANCELLATIONS
                var bookings = new List<Booking>();
                var bps = new List<BookingParticipant>();
                var cancellations = new List<BookingCancellation>();
                var players = accounts.Where(a => a.RoleId == playerRole.Id).ToList();
                for (int i = 0; i < 10; i++)
                {
                    var b = new Booking { Id = Guid.NewGuid(), HostId = players[i % players.Count].Id, CourtSlotId = slots[i].Id, TotalAmount = 100000m, Status = BookingStatus.PAID, DynamicQRCode = "qr" };
                    bookings.Add(b);
                    bps.Add(new BookingParticipant { Id = Guid.NewGuid(), BookingId = b.Id, UserId = players[i % players.Count].Id, AmountContributed = 100000m, PaymentStatus = PaymentStatus.PAID, QrCode = "qr" });
                    cancellations.Add(new BookingCancellation { Id = Guid.NewGuid(), BookingId = b.Id, CancelledBy = players[i % players.Count].Id, Reason = "Bận", RefundAmount = 0, PenaltyAmount = 100000m });
                }
                await _context.Bookings.AddRangeAsync(bookings);
                await _context.BookingParticipants.AddRangeAsync(bps);
                await _context.BookingCancellations.AddRangeAsync(cancellations);

                // TRANSACTIONS
                var txs = new List<Transaction>();
                var pgLogs = new List<PaymentGatewayLog>();
                for (int i = 0; i < 10; i++)
                {
                    var tx = new Transaction { Id = Guid.NewGuid(), WalletId = wallets[i].Id, Amount = 100000m, Type = TransactionType.DEPOSIT, Status = "SUCCESS", ReferenceId = bookings[i].Id };
                    txs.Add(tx);
                    pgLogs.Add(new PaymentGatewayLog { Id = Guid.NewGuid(), TransactionId = tx.Id, Gateway = "VNPay", RawPayload = "{}" });
                }
                await _context.Transactions.AddRangeAsync(txs);
                await _context.PaymentGatewayLogs.AddRangeAsync(pgLogs);

                // TEAMS
                var teams = new List<Team>();
                var tms = new List<TeamMember>();
                for (int i = 0; i < 10; i++)
                {
                    var t = new Team { Id = Guid.NewGuid(), Name = "Team " + i, CreatedById = players[i % players.Count].Id };
                    teams.Add(t);
                    tms.Add(new TeamMember { Id = Guid.NewGuid(), TeamId = t.Id, AccountId = players[i % players.Count].Id });
                }
                await _context.Teams.AddRangeAsync(teams);
                await _context.TeamMembers.AddRangeAsync(tms);

                // LFG
                var lfgs = new List<LfgCard>();
                var lfgPs = new List<LfgParticipant>();
                for (int i = 0; i < 10; i++)
                {
                    var lfg = new LfgCard { Id = Guid.NewGuid(), BookingId = bookings[i].Id, SlotsNeeded = 1, SkillLevelTag = "PRO", Status = LfgCardStatus.PENDING };
                    lfgs.Add(lfg);
                    lfgPs.Add(new LfgParticipant { Id = Guid.NewGuid(), LfgCardId = lfg.Id, UserId = players[i % players.Count].Id, Status = LfgParticipantStatus.ACCEPTED, QrCode = "qr" });
                }
                await _context.LfgCards.AddRangeAsync(lfgs);
                await _context.LfgParticipants.AddRangeAsync(lfgPs);

                // COMMUNITY
                var posts = new List<CommunityPost>();
                var cmts = new List<PostComment>();
                for (int i = 0; i < 10; i++)
                {
                    var p = new CommunityPost { Id = Guid.NewGuid(), AuthorId = players[i % players.Count].Id, Content = "Hello" };
                    posts.Add(p);
                    cmts.Add(new PostComment { Id = Guid.NewGuid(), PostId = p.Id, UserId = players[i % players.Count].Id, Content = "Hi" });
                }
                await _context.CommunityPosts.AddRangeAsync(posts);
                await _context.PostComments.AddRangeAsync(cmts);

                // TOURNAMENTS
                var tours = new List<Tournament>();
                var tPrizes = new List<TournamentPrize>();
                var tParts = new List<TournamentParticipant>();
                var matches = new List<BracketMatch>();
                var scores = new List<MatchScoreDetail>();
                for (int i = 0; i < 10; i++)
                {
                    var tour = new Tournament { Id = Guid.NewGuid(), FacilityId = facilities[i].Id, CategoryId = categories[i].Id, Name = "Cup " + i, CategoryType = TournamentCategoryType.SINGLES, Status = TournamentStatus.OPEN };
                    tours.Add(tour);
                    tPrizes.Add(new TournamentPrize { Id = Guid.NewGuid(), TournamentId = tour.Id, Position = "1st", RewardAmount = 1000000m });
                    
                    var p1 = new TournamentParticipant { Id = Guid.NewGuid(), TournamentId = tour.Id, UserId = players[i % players.Count].Id, PaymentStatus = PaymentStatus.PAID, QrCode = "qr" };
                    var p2 = new TournamentParticipant { Id = Guid.NewGuid(), TournamentId = tour.Id, UserId = players[(i+1) % players.Count].Id, PaymentStatus = PaymentStatus.PAID, QrCode = "qr2" };
                    tParts.Add(p1); tParts.Add(p2);

                    var m = new BracketMatch { Id = Guid.NewGuid(), TournamentId = tour.Id, MatchIndex = i+1, RoundName = "Chung Kết", Participant1Id = p1.Id, Participant2Id = p2.Id, Status = "PENDING" };
                    matches.Add(m);
                    scores.Add(new MatchScoreDetail { Id = Guid.NewGuid(), BracketMatchId = m.Id, SetNumber = 1, ScoreP1 = 21, ScoreP2 = 19 });
                }
                await _context.Tournaments.AddRangeAsync(tours);
                await _context.TournamentPrizes.AddRangeAsync(tPrizes);
                await _context.TournamentParticipants.AddRangeAsync(tParts);
                await _context.BracketMatches.AddRangeAsync(matches);
                await _context.MatchScoreDetails.AddRangeAsync(scores);

                // AI CHAT
                var sessions = new List<AiChatSession>();
                var aiLogs = new List<AiPromptLog>();
                for (int i = 0; i < 10; i++)
                {
                    var s = new AiChatSession { Id = Guid.NewGuid(), UserId = players[i % players.Count].Id, SessionTitle = "Chat " + i };
                    sessions.Add(s);
                    aiLogs.Add(new AiPromptLog { Id = Guid.NewGuid(), SessionId = s.Id, Sender = "USER", Message = "Hi" });
                }
                await _context.AiChatSessions.AddRangeAsync(sessions);
                await _context.AiPromptLogs.AddRangeAsync(aiLogs);

                // LOGS & SYSTEM
                var ciLogs = new List<CheckInLog>();
                var fpRatings = new List<FairPlayRating>();
                var nfLogs = new List<NotificationLog>();
                for (int i = 0; i < 10; i++)
                {
                    ciLogs.Add(new CheckInLog { Id = Guid.NewGuid(), FacilityId = facilities[i].Id, BookingParticipantId = bps[i].Id });
                    fpRatings.Add(new FairPlayRating { Id = Guid.NewGuid(), ReviewerId = players[i % players.Count].Id, RevieweeId = players[(i+1) % players.Count].Id, ContextId = bookings[i].Id, Score = 5, Comment = "Good" });
                    nfLogs.Add(new NotificationLog { Id = Guid.NewGuid(), UserId = players[i % players.Count].Id, Title = "Thong bao", Body = "Noi dung" });
                }
                await _context.CheckInLogs.AddRangeAsync(ciLogs);
                await _context.FairPlayRatings.AddRangeAsync(fpRatings);
                await _context.NotificationLogs.AddRangeAsync(nfLogs);

                await _context.SaveChangesAsync();

                return Ok(new { message = "Seeded successfully. 29 tables populated with 10 rows each!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message, inner = ex.InnerException?.Message });
            }
        }
    }
}
