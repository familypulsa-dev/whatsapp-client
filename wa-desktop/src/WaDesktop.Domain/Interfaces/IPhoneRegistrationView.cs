using System;

namespace WaDesktop.Domain.Interfaces
{
    public interface IPhoneRegistrationView : IViewBase
    {
        string WabaId { get; set; }
        string PhoneNumberId { get; set; }
        
        string Cc { get; }
        string PhoneNumber { get; }
        string VerifiedName { get; }
        string CodeMethod { get; }
        string VerificationCode { get; }
        string Pin { get; }
        string ConfirmPin { get; }
        bool IsVerified { get; set; }

        event EventHandler ProcessStepClicked;
        event EventHandler BackStepClicked;
        event EventHandler CancelClicked;

        void ShowStep(int step);
        void SetLoading(bool isLoading);
        void ShowMessage(string title, string message);
        void ShowError(string title, string error);
        void CloseDialog(bool isSuccess);
    }
}
