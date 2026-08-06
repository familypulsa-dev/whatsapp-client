using System;
using WaDesktop.Client.Extensions;
using WaDesktop.Domain.Interfaces;

namespace WaDesktop.Client.Presenters
{
    public class PhoneRegistrationPresenter : IDisposable
    {
        private readonly IPhoneRegistrationView _view;
        private readonly IPhoneRegistrationUseCase _useCase;
        private int _currentStep;
        private bool _disposed;

        public PhoneRegistrationPresenter(IPhoneRegistrationView view, IPhoneRegistrationUseCase useCase)
        {
            _view = view;
            _useCase = useCase;

            _view.ProcessStepClicked += OnProcessStepClicked;
            _view.BackStepClicked += OnBackStepClicked;
            _view.CancelClicked += OnCancelClicked;
        }

        public void Initialize()
        {
            if (!string.IsNullOrEmpty(_view.PhoneNumberId))
            {
                _view.ShowStep(2);
                _currentStep = 2;
            }
            else
            {
                _view.ShowStep(1);
                _currentStep = 1;
            }
        }

        private async void OnProcessStepClicked(object sender, EventArgs e)
        {
            _view.SetLoading(true);
            try
            {
                switch (_currentStep)
                {
                    case 1:
                        var phoneId = await _useCase.CreatePhoneNumberAsync(
                            _view.WabaId, _view.Cc, _view.PhoneNumber, _view.VerifiedName);
                        _view.PhoneNumberId = phoneId;
                        break;
                    case 2:
                        await _useCase.RequestVerificationCodeAsync(_view.PhoneNumberId, _view.CodeMethod);
                        _view.ShowMessage("Sukses", "Kode verifikasi berhasil dikirim.");
                        break;
                    case 3:
                        await _useCase.VerifyCodeAsync(_view.PhoneNumberId, _view.VerificationCode);
                        _view.ShowMessage("Sukses", "Kode verifikasi berhasil dikonfirmasi.");
                        break;
                    case 4:
                        await _useCase.RegisterPhoneAsync(_view.PhoneNumberId, _view.Pin, _view.ConfirmPin);
                        _view.ShowMessage("Sukses", "Nomor telepon berhasil didaftarkan!");
                        _view.CloseDialog(true);
                        return;
                }

                if (_currentStep < 4)
                {
                    _currentStep++;
                    _view.ShowStep(_currentStep);
                }
            }
            catch (Exception ex)
            {
                _view.ShowError("Error", ex.Message);
            }
            finally
            {
                _view.SetLoading(false);
            }
        }

        private void OnBackStepClicked(object sender, EventArgs e)
        {
            if (_currentStep > 1)
            {
                _currentStep--;
                _view.ShowStep(_currentStep);
            }
        }

        private void OnCancelClicked(object sender, EventArgs e)
        {
            _view.CloseDialog(false);
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _view.ProcessStepClicked -= OnProcessStepClicked;
                _view.BackStepClicked -= OnBackStepClicked;
                _view.CancelClicked -= OnCancelClicked;
                _disposed = true;
            }
        }
    }
}
