using System;
using System.Collections.Generic;
using System.Text;

namespace SEP490_SPORTNEXUS_BE.Repositories.Abstraction
{
    public abstract class Entity<T>
    {
        public T Id { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
