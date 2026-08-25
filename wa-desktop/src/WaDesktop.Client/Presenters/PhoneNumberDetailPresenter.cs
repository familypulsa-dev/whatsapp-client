using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WaDesktop.Domain.Common;
using WaDesktop.Domain.Interfaces;

namespace WaDesktop.Client.Presenters
{
    public class PhoneNumberDetailPresenter : IDisposable
    {
        private readonly IPhoneNumberDetailView _view;
        private readonly IPhoneNumberRepository _phones;
        private readonly string _phoneNumberId;
        private bool _disposed;

        public PhoneNumberDetailPresenter(IPhoneNumberDetailView view, IPhoneNumberRepository phones, string phoneNumberId)
        {
            _view = view;
            _phones = phones;
            _phoneNumberId = phoneNumberId;

            _view.SaveClicked += OnSave;
            _view.FetchFromMetaClicked += OnFetchFromMeta;
            _view.UploadPhotoClicked += OnUploadPhoto;
            _view.RefreshClicked += async (s, e) => await LoadDataAsync();
        }

        public async void LoadData() => await LoadDataAsync();

        private async Task LoadPictureAsync(string profilePictureUrl)
        {
            if (string.IsNullOrEmpty(profilePictureUrl)) return;
            var picture = await Task.Run(() => _phones.GetProfilePictureAsync(profilePictureUrl));
            if (picture.IsSuccess)
                _view.LoadProfilePicture(picture.Value);
        }

        private async Task LoadDataAsync()
        {
            _view.IsSaving = true;
            try
            {
                var result = await Task.Run(() => _phones.GetDetailAsync(_phoneNumberId));
                if (result.IsFailure)
                    throw new Exception(result.Error.Message);
                _view.LoadDetail(result.Value);

                await LoadPictureAsync(result.Value.ProfilePictureUrl);
            }
            catch (Exception ex)
            {
                _view.ShowError($"Failed to load: {ex.Message}");
            }
            finally
            {
                _view.IsSaving = false;
            }
        }

        private async void OnSave(object sender, EventArgs e)
        {
            _view.IsSaving = true;
            try
            {
                var websites = new List<string>();
                if (!string.IsNullOrEmpty(_view.Website1)) websites.Add(_view.Website1);
                if (!string.IsNullOrEmpty(_view.Website2)) websites.Add(_view.Website2);

                var result = await Task.Run(() =>
                    _phones.SaveDetailAsync(
                        _phoneNumberId,
                        _view.DisplayName,
                        _view.Description,
                        _view.Email,
                        _view.About,
                        _view.Address,
                        _view.Vertical,
                        websites));
                if (result.IsFailure)
                    throw new Exception(result.Error.Message);

                _view.LoadDetail(result.Value.Detail);

                foreach (var w in result.Value.Warnings ?? new List<string>())
                    _view.ShowWarning(w);

                await LoadPictureAsync(result.Value.Detail?.ProfilePictureUrl);
                _view.ShowSuccess("Phone number updated.");
            }
            catch (Exception ex)
            {
                _view.ShowError($"Save failed: {ex.Message}");
            }
            finally
            {
                _view.IsSaving = false;
            }
        }

        private async void OnFetchFromMeta(object sender, EventArgs e)
        {
            _view.IsSaving = true;
            try
            {
                var result = await Task.Run(() => _phones.SyncProfileAsync(_phoneNumberId));
                if (result.IsFailure)
                    throw new Exception(result.Error.Message);
                _view.LoadDetail(result.Value);

                await LoadPictureAsync(result.Value.ProfilePictureUrl);
                _view.ShowSuccess("Profile synced from Meta.");
            }
            catch (Exception ex)
            {
                _view.ShowError($"Sync failed: {ex.Message}");
            }
            finally
            {
                _view.IsSaving = false;
            }
        }

        private async void OnUploadPhoto(object sender, EventArgs e)
        {
            var filePath = _view.PendingUploadPath;
            if (string.IsNullOrEmpty(filePath))
            {
                _view.ShowError("No file selected.");
                return;
            }

            _view.IsSaving = true;
            try
            {
                var result = await Task.Run(() => _phones.UploadPictureAsync(_phoneNumberId, filePath));
                if (result.IsFailure)
                    throw new Exception(result.Error.Message);
                _view.LoadDetail(result.Value);

                await LoadPictureAsync(result.Value.ProfilePictureUrl);
                _view.ShowSuccess("Profile picture updated.");
            }
            catch (Exception ex)
            {
                _view.ShowError($"Upload failed: {ex.Message}");
            }
            finally
            {
                _view.IsSaving = false;
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _view.SaveClicked -= OnSave;
                _view.FetchFromMetaClicked -= OnFetchFromMeta;
                _view.UploadPhotoClicked -= OnUploadPhoto;
                _disposed = true;
            }
        }
    }
}
