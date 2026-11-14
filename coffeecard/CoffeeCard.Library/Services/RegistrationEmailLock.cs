using System;
using System.Threading.Tasks;
using AsyncKeyedLock;

namespace CoffeeCard.Library.Services;

/// <summary>
/// Coordinates account creation for the same email within this application process.
/// </summary>
internal static class RegistrationEmailLock
{
    private static readonly AsyncKeyedLocker<string> Locks = new(StringComparer.OrdinalIgnoreCase);

    public static async ValueTask<IDisposable> LockAsync(string email)
    {
        return await Locks.LockAsync(email);
    }
}
