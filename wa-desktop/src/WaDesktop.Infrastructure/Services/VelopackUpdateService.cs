using System;
using System.Threading.Tasks;
using Velopack;
using Velopack.Sources;
using WaDesktop.Domain.Interfaces;

namespace WaDesktop.Infrastructure.Services
{
    public class VelopackUpdateService : IUpdateService
    {
        private const string RepoUrl = "https://github.com/familypulsa-dev/whatsapp-client";
        private const string AccessToken = "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";
        
        private UpdateInfo _pendingUpdate;

        public async Task<AppUpdateInfo> CheckForUpdatesAsync()
        {
            try
            {
                var mgr = new UpdateManager(new GithubSource(RepoUrl, AccessToken, false));
                _pendingUpdate = await mgr.CheckForUpdatesAsync();
                
                if (_pendingUpdate != null)
                {
                    return new AppUpdateInfo
                    {
                        Version = _pendingUpdate.TargetFullRelease.Version.ToString(),
                        ReleaseNotes = _pendingUpdate.TargetFullRelease.NotesMarkdown
                    };
                }
            }
            catch
            {
                // Ignored
            }
            return null;
        }

        public async Task DownloadAndApplyUpdateAsync(IProgress<(string status, int percent)> progress = null)
        {
            if (_pendingUpdate == null) return;

            var mgr = new UpdateManager(new GithubSource(RepoUrl, AccessToken, false));
            
            progress?.Report(($"Mengunduh pembaruan {_pendingUpdate.TargetFullRelease.Version}...", 10));

            await mgr.DownloadUpdatesAsync(_pendingUpdate, (percent) =>
            {
                progress?.Report(($"Mengunduh pembaruan ({percent}%)...", 10 + (int)(percent * 0.8)));
            });

            progress?.Report(("Pembaruan siap. Menjalankan ulang aplikasi...", 95));
            await Task.Delay(1000);

            mgr.ApplyUpdatesAndRestart(_pendingUpdate);
        }

        public async Task CheckAndDownloadUpdateAsync(IProgress<(string status, int percent)> progress)
        {
            try
            {
                var newVersion = await CheckForUpdatesAsync();
                if (newVersion != null)
                {
                    await DownloadAndApplyUpdateAsync(progress);
                }
                else
                {
                    progress?.Report(("Aplikasi sudah versi terbaru.", 100));
                }
            }
            catch (Exception ex)
            {
                // Jika error (misal offline), kita skip saja dan jalankan aplikasi biasa
                System.Diagnostics.Debug.WriteLine($"Update check failed: {ex.Message}");
                progress?.Report(("Gagal mengecek pembaruan. Melanjutkan...", 100));
            }
        }
    }
}
