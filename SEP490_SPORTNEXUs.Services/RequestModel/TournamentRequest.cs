using SEP490_SPORTNEXUS_BE.Repositories.Enums;
using System;
using System.Collections.Generic;

namespace SEP490_SPORTNEXUS_BE.Services.RequestModel
{
    public class CreateTournamentRequest
    {
        public Guid FacilityId { get; set; }
        public Guid CategoryId { get; set; }
        public string Name { get; set; } = null!;
        public List<TournamentChildDto> Children { get; set; } = new List<TournamentChildDto>();
    }

    public class TournamentChildDto
    {
        public string Name { get; set; } = null!;
        public TournamentCategoryType CategoryType { get; set; }
        public int MaxParticipants { get; set; }
        public decimal EntryFee { get; set; }
        public List<PrizeDto> Prizes { get; set; } = new List<PrizeDto>();
    }

    public class PrizeDto
    {
        public string Position { get; set; } = null!;
        public decimal RewardAmount { get; set; }
    }
}

