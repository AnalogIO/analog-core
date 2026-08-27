using System;
using System.Threading.Tasks;
using CoffeeCard.Models.DataTransferObjects.v2.Receipts;

namespace CoffeeCard.Library.Services.v2;

public interface IReceiptService
{
    Task<ReceiptResponse> GetReceipts(DateTime from, ReceiptType type, int userId, int batchSize);
}
