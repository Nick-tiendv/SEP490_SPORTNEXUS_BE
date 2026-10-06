using System;

namespace SEP490_SPORTNEXUS_BE.Services.ResponseModel
{
    public class SportCategoryResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? RulesDescription { get; set; }
    }

    public class AmenityResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? IconCode { get; set; }
    }
}
