using NUnit.Framework;
using System;
using System.IO;
using System.Text;
using WaDesktop.Domain.Entities;
using WaDesktop.Infrastructure.Services;

namespace WaDesktop.Tests.Authentication
{
    [TestFixture]
    public class AuthSessionStoreTests
    {
        private sealed class TestDataProtector : ISessionDataProtector
        {
            public byte[] Protect(byte[] data) => Transform(data);
            public byte[] Unprotect(byte[] data) => Transform(data);

            private static byte[] Transform(byte[] data)
            {
                var result = new byte[data.Length];
                for (var index = 0; index < data.Length; index++)
                {
                    result[index] = (byte)(data[index] ^ 0xA5);
                }
                return result;
            }
        }

        [Test]
        public void ClearSession_RemovesAccessAndRefreshTokens()
        {
            var store = new AuthSessionStore();
            store.SetSession("access-token", "refresh-token");

            store.ClearSession();

            Assert.That(store.AccessToken, Is.Null);
            Assert.That(store.RefreshToken, Is.Null);
        }

        [Test]
        public void PersistedSessionStore_RoundTripsEncryptedSession()
        {
            var directory = Path.Combine(TestContext.CurrentContext.WorkDirectory, "session-tests", Guid.NewGuid().ToString("N"));
            var filePath = Path.Combine(directory, "session.dat");

            try
            {
                var store = new PersistedSessionStore(filePath, new TestDataProtector());
                store.Save(new PersistedSession
                {
                    AccessToken = "access-token",
                    RefreshToken = "refresh-token",
                    Role = "admin",
                    DisplayName = "Test User",
                    CompanyName = "Test Company",
                    CompanyId = "company-id"
                });

                var raw = Encoding.UTF8.GetString(File.ReadAllBytes(filePath));
                var restored = store.Load();

                Assert.That(raw, Does.Not.Contain("refresh-token"));
                Assert.That(restored.AccessToken, Is.EqualTo("access-token"));
                Assert.That(restored.RefreshToken, Is.EqualTo("refresh-token"));
                Assert.That(restored.CompanyId, Is.EqualTo("company-id"));
            }
            finally
            {
                if (Directory.Exists(directory)) Directory.Delete(directory, true);
            }
        }

        [Test]
        public void PersistedSessionStore_CorruptFile_ReturnsNullAndDeletesFile()
        {
            var directory = Path.Combine(TestContext.CurrentContext.WorkDirectory, "session-tests", Guid.NewGuid().ToString("N"));
            var filePath = Path.Combine(directory, "session.dat");

            try
            {
                Directory.CreateDirectory(directory);
                File.WriteAllText(filePath, "not-an-encrypted-session");
                var store = new PersistedSessionStore(filePath, new TestDataProtector());

                var restored = store.Load();

                Assert.That(restored, Is.Null);
                Assert.That(File.Exists(filePath), Is.False);
            }
            finally
            {
                if (Directory.Exists(directory)) Directory.Delete(directory, true);
            }
        }
    }
}
