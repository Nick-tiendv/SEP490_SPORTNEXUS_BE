using System;
using System.Collections.Generic;
using System.Text;

namespace SEP490_SPORTNEXUS_BE.Services.ResponseModel
{
    public class ApiResponse<T>
    {
        public int StatusCode { get; set; }
        public string Message { get; set; } = null!;
        public T? Data { get; set; }
    }
}
