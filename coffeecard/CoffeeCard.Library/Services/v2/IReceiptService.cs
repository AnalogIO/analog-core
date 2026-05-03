using System.Threading.Tasks;
using CoffeeCard.Models.DataTransferObjects.v2.Receipts;

namespace CoffeeCard.Library.Services.v2;

/// <summary>
/// Service for retrieving receipts for a specific user.
/// </summary>
public interface IReceiptService
{
    /// <summary>
    /// Returns a flat, unsegmented list of receipts for the given user, optionally filtered by
    /// <paramref name="type"/>. The returned list is sorted by event date descending.
    /// </summary>
    /// <param name="type">
    /// The receipt type to include. Use <see cref="ReceiptTypeFilter.All"/> to return every type.
    /// </param>
    /// <param name="userId">The database primary key of the user whose receipts are fetched.</param>
    /// <returns>A <see cref="ReceiptsResponse"/> containing the matching receipt items.</returns>
    Task<ReceiptsResponse> GetReceipts(ReceiptTypeFilter type, int userId);
}
