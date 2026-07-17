using System;
using System.Threading.Tasks;

namespace WaDesktop.Domain.Interfaces
{
    public class AppUpdateInfo
    {
        public string Version { get; set; }
        public string ReleaseNotes { get; set; }
    }

    public interface IUpdateService
    {
        Task CheckAndDownloadUpdateAsync(IProgress<(string status, int percent)> progress);
        Task<AppUpdateInfo> CheckForUpdatesAsync();
        Task DownloadAndApplyUpdateAsync(IProgress<(string status, int percent)> progress = null);
    }
}
