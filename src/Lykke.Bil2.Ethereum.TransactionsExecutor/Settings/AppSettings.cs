using JetBrains.Annotations;
using Lykke.Bil2.Sdk.TransactionsExecutor.Settings;

namespace Lykke.Bil2.Ethereum.TransactionsExecutor.Settings
{
    /// <summary>
    /// Specific blockchain settings
    /// </summary>
    [UsedImplicitly]
    public class AppSettings : BaseTransactionsExecutorSettings<DbSettings>
    {
        // Implement specific blockchain settings, if necessary.
        // Mark sensitive data with SecureSettingsAttribute to prevent leaks.
        //
        // For example:
        //
        // public string NodeUrl { get; set; }
        //
        // public string NodeRpcUsername { get; set; }
        //
        // [SecureSettings]
        // public string NodeRpcPassword { get; set; }
    }
}
