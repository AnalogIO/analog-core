﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace coffeecard.Models.DataTransferObjects.Ticket
{
    public class UseMultipleTicketDTO
    {
        public IEnumerable<int> ticketIds { get; set; }
    }
}