using System;

namespace WaDesktop.Domain.Interfaces
{
    public interface ILimitBillingView : IViewBase
    {
        int? LimitMarketing { get; set; }
        int? LimitUtility { get; set; }
        int? LimitAuthentication { get; set; }
        int? LimitService { get; set; }

        decimal MaxMarketingCost { set; }
        decimal MaxUtilityCost { set; }
        decimal MaxAuthenticationCost { set; }
        decimal MaxServiceCost { set; }
        decimal MaxTotalCost { set; }

        bool IsLoading { set; }
        void ShowError(string message);

        event EventHandler SaveClicked;
        event EventHandler LimitsChanged;

        void CloseDialog(bool success);
    }
}
