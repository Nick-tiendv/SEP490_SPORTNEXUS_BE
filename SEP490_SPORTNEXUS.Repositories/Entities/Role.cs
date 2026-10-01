using SEP490_SPORTNEXUS_BE.Repositories.Abstraction;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SEP490_SPORTNEXUS_BE.Repositories.Entities
{
    public class Role : Entity<Guid>
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = null!;
        public ICollection<Account> Accounts { get; set; } = new List<Account>();
    }
}
