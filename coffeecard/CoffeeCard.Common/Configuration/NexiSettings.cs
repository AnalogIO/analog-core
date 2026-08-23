using System;

namespace CoffeeCard.Common.Configuration;

public class NexiSettings
{
    public required Uri ApiUrl { get; set; }
    public required string ApiKey { get; set; }
}
