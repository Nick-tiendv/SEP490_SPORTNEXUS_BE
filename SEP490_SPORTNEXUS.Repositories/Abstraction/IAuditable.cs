using System;
using System.Collections.Generic;
using System.Text;

namespace SEP490_SPORTNEXUS_BE.Repositories.Abstraction
{
    public interface IAuditable
    {
        DateTimeOffset CreateAt { get; set; }
        DateTimeOffset? ModifiedAt { get; set; }
    }
}
