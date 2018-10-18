using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace coffeecard.Helpers
{
    public class ApiError
    {
        public string message { get; set; }

        public ApiError(string message)
        {
            this.message = message;
        }
    }
}
