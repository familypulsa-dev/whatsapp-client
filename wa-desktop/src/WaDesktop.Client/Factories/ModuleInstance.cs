using System;
using Microsoft.Extensions.DependencyInjection;
using WaDesktop.Domain.Interfaces;

namespace WaDesktop.Client.Factories
{
    /// <summary>
    /// Hasil perakitan satu modul tab. Menjaga View + Presenter + DI Scope
    /// tetap hidup bersama dan membuang semuanya saat tab ditutup.
    /// </summary>
    public sealed class ModuleInstance : IDisposable
    {
        /// <summary>View siap tampil di Workspace.</summary>
        public IViewBase View { get; }

        /// <summary>
        /// Presenter yang mendukung refresh (LoadData). Null jika modul tidak
        /// membutuhkan refresh (mis. messages / template detail).
        /// </summary>
        public IPresenterBase Refreshable { get; }

        private readonly IDisposable _disposable; // Presenter mentah (jika IDisposable)
        private readonly IServiceScope _scope;
        private bool _disposed;

        public ModuleInstance(IViewBase view, object presenter, IServiceScope scope)
        {
            View = view;
            Refreshable = presenter as IPresenterBase;
            _disposable = presenter as IDisposable;
            _scope = scope;
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            // Presenter di-dispose eksplisit karena ActivatorUtilities.CreateInstance
            // TIDAK melacak instance yang dibuatnya ke dalam scope (hanya View via GetRequiredService).
            _disposable?.Dispose();
            _scope?.Dispose();
        }
    }
}
