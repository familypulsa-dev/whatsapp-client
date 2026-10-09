using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WaDesktop.Domain.Entities;
using WaDesktop.Domain.Interfaces;

namespace WaDesktop.Client.Presenters
{
    public interface IWabaOnboardingView
    {
        bool IsBusy { set; }
        void ShowStatus(string message);
    }

    public sealed class WabaOnboardingPresenter
    {
        private readonly IWabaRepository _repository;
        private readonly IWabaOnboardingView _view;
        private readonly HashSet<string> _baseline;
        private bool _busy;

        public List<Waba> LatestWabas { get; private set; }
        public List<Waba> NewWabas { get; private set; } = new List<Waba>();

        public WabaOnboardingPresenter(IWabaRepository repository, IWabaOnboardingView view, IEnumerable<Waba> baseline)
        {
            _repository = repository;
            _view = view;
            _baseline = new HashSet<string>(baseline.Select(w => w.WabaId), StringComparer.Ordinal);
        }

        public async Task SyncAsync()
        {
            if (_busy) return;
            _busy = true;
            _view.IsBusy = true;
            _view.ShowStatus("Menyinkronkan WABA dari Meta…");
            try
            {
                var sync = await _repository.SyncFromMetaAsync();
                if (sync.IsFailure) throw new Exception(sync.Error.Message);
                var list = await _repository.GetAllAsync();
                if (list.IsFailure) throw new Exception(list.Error.Message);
                LatestWabas = list.Value;
                NewWabas = LatestWabas.Where(w => !_baseline.Contains(w.WabaId)).ToList();
                _view.ShowStatus(NewWabas.Count == 0
                    ? "Sinkronisasi selesai; belum ada WABA baru yang ditemukan. Anda dapat mencoba lagi."
                    : "WABA baru ditemukan: " + string.Join("; ", NewWabas.Select(w => w.Name + " (" + w.WabaId + ")")));
            }
            catch (Exception ex)
            {
                _view.ShowStatus("Sinkronisasi gagal: " + ex.Message);
            }
            finally
            {
                _busy = false;
                _view.IsBusy = false;
            }
        }
    }
}
