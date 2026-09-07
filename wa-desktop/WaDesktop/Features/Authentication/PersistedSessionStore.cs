using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using WaDesktop.Domain.Entities;
using WaDesktop.Domain.Interfaces;

namespace WaDesktop.Infrastructure.Services
{
    public interface ISessionDataProtector
    {
        byte[] Protect(byte[] data);
        byte[] Unprotect(byte[] data);
    }

    public sealed class WindowsSessionDataProtector : ISessionDataProtector
    {
        private static readonly byte[] Entropy = Encoding.UTF8.GetBytes("WaDesktop.Session.v1");

        public byte[] Protect(byte[] data)
            => ProtectedData.Protect(data, Entropy, DataProtectionScope.CurrentUser);

        public byte[] Unprotect(byte[] data)
            => ProtectedData.Unprotect(data, Entropy, DataProtectionScope.CurrentUser);
    }

    public class PersistedSessionStore : IPersistedSessionStore
    {
        private readonly string _filePath;
        private readonly ISessionDataProtector _protector;

        public PersistedSessionStore()
            : this(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "WhatsAppClient",
                "session.dat"),
                new WindowsSessionDataProtector())
        {
        }

        public PersistedSessionStore(string filePath, ISessionDataProtector protector)
        {
            _filePath = filePath;
            _protector = protector ?? throw new ArgumentNullException(nameof(protector));
        }

        public PersistedSession Load()
        {
            if (!File.Exists(_filePath)) return null;

            try
            {
                var encrypted = File.ReadAllBytes(_filePath);
                var plain = _protector.Unprotect(encrypted);
                var session = JsonConvert.DeserializeObject<PersistedSession>(Encoding.UTF8.GetString(plain));

                if (session == null || string.IsNullOrEmpty(session.RefreshToken))
                {
                    Delete();
                    return null;
                }

                return session;
            }
            catch
            {
                Delete();
                return null;
            }
        }

        public void Save(PersistedSession session)
        {
            if (session == null || string.IsNullOrEmpty(session.RefreshToken))
                throw new ArgumentException("A refresh token is required.", nameof(session));

            try
            {
                var directory = Path.GetDirectoryName(_filePath);
                if (!string.IsNullOrEmpty(directory)) Directory.CreateDirectory(directory);

                var json = JsonConvert.SerializeObject(session);
                var plain = Encoding.UTF8.GetBytes(json);
                var encrypted = _protector.Protect(plain);
                File.WriteAllBytes(_filePath, encrypted);
            }
            catch (IOException)
            {
                // Persistence failure must not invalidate an otherwise valid login.
            }
            catch (UnauthorizedAccessException)
            {
                // Persistence failure must not invalidate an otherwise valid login.
            }
            catch (CryptographicException)
            {
                // Persistence failure must not invalidate an otherwise valid login.
            }
        }

        public void Delete()
        {
            try
            {
                if (File.Exists(_filePath)) File.Delete(_filePath);
            }
            catch (IOException)
            {
                // A locked session file must not prevent logout or startup.
            }
            catch (UnauthorizedAccessException)
            {
                // Treat an inaccessible session as unavailable.
            }
        }
    }
}
