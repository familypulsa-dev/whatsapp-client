using System;
using System.Threading.Tasks;
using WaDesktop.Domain.Entities;
using WaDesktop.Domain.Interfaces;

namespace WaDesktop.Client.Presenters
{
    public class LimitBillingPresenter : IDisposable
    {
        private readonly ILimitBillingView _view;
        private readonly ICompanyRepository _companies;
        private readonly Company _company;
        private bool _disposed;

        // Pricing Constants
        private const decimal PriceMarketing = 586.33m;
        private const decimal PriceUtility = 356.65m;
        private const decimal PriceAuthentication = 356.65m;
        private const decimal PriceService = 0m;

        public LimitBillingPresenter(ILimitBillingView view, ICompanyRepository companies, Company company)
        {
            _view = view;
            _companies = companies;
            _company = company;

            _view.SaveClicked += OnSaveClicked;
            _view.LimitsChanged += OnLimitsChanged;

            InitializeView();
        }

        private void InitializeView()
        {
            _view.LimitMarketing = _company.LimitMarketing;
            _view.LimitUtility = _company.LimitUtility;
            _view.LimitAuthentication = _company.LimitAuthentication;
            _view.LimitService = _company.LimitService;

            CalculateCosts();
        }

        private void OnLimitsChanged(object sender, EventArgs e)
        {
            CalculateCosts();
        }

        private void CalculateCosts()
        {
            decimal costMkt = (_view.LimitMarketing ?? 0) * PriceMarketing;
            decimal costUtl = (_view.LimitUtility ?? 0) * PriceUtility;
            decimal costAuth = (_view.LimitAuthentication ?? 0) * PriceAuthentication;
            decimal costSvc = (_view.LimitService ?? 0) * PriceService;

            _view.MaxMarketingCost = costMkt;
            _view.MaxUtilityCost = costUtl;
            _view.MaxAuthenticationCost = costAuth;
            _view.MaxServiceCost = costSvc;
            _view.MaxTotalCost = costMkt + costUtl + costAuth + costSvc;
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            _view.IsLoading = true;
            try
            {
                var result = await Task.Run(() => _companies.UpdateAsync(
                    _company.Id,
                    _company.Name,
                    _view.LimitMarketing,
                    _view.LimitUtility,
                    _view.LimitAuthentication,
                    _view.LimitService));

                if (result.IsFailure)
                {
                    _view.ShowError($"Gagal menyimpan pengaturan limit: {result.Error.Message}");
                    return;
                }

                _view.CloseDialog(true);
            }
            finally
            {
                _view.IsLoading = false;
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _view.SaveClicked -= OnSaveClicked;
                _view.LimitsChanged -= OnLimitsChanged;
                _disposed = true;
            }
        }
    }
}
