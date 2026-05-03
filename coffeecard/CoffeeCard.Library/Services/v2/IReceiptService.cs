using System.Threading.Tasks;
using CoffeeCard.Models.DataTransferObjects.v2.Receipts;

namespace CoffeeCard.Library.Services.v2;

/// <summary>
/// Service for retrieving receipts for a specific user.
/// </summary>
public interface IReceiptService
{
    /// <summary>
    /// Returns a list of receipts for the given user,
    /// sorted by most recent event first.
    /// </summary>
    /// <param name="userId">The database primary key of the user whose receipts are fetched.</param>
    /// <returns>A <see cref="ReceiptsResponse"/> containing the receipt items.</returns>
    Task<ReceiptsResponse> GetReceipts(int userId);
}
