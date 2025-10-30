namespace CoffeeCard.Models.DataTransferObjects.v2;

/// <summary>
/// Service Health
/// </summary>
/// <example>
/// {
///     "mobilePay": true,
///     "database": true
/// }
/// </example>
public class ServiceHealthResponse
{
    /// <summary>
    /// MobilePay connected
    /// </summary>
    /// <value>Health</value>
    /// <example>true</example>
    public bool MobilePay { get; set; }

    /// <summary>
    /// Database connected
    /// </summary>
    /// <value>Health</value>
    /// <example>true</example>
    public bool Database { get; set; }
}
