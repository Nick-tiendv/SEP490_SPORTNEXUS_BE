using System;
using System.Collections.Generic;
using System.Text;

namespace SEP490_SPORTNEXUS_BE.Services.ResponseModel
{
    public class LoginResponse
    {
        public string Token { get; set; } = null!;
        public string FullName { get; set; } = null!;
    }
}
