using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using WaDesktop.Client.Presenters;
using WaDesktop.Domain.Common;
using WaDesktop.Domain.Entities;
using WaDesktop.Domain.Interfaces;

namespace WaDesktop.Tests.PresenterTests
{
    [TestFixture]
    public class WabaOnboardingPresenterTests
    {
        private sealed class View : IWabaOnboardingView
        {
            public bool Busy;
            public string Status;
            public bool IsBusy { set { Busy = value; } }
            public void ShowStatus(string message) { Status = message; }
        }

        private sealed class Repository : IWabaRepository
        {
            public int SyncCalls;
            public bool FailSync;
            public bool FailList;
            public TaskCompletionSource<Result<bool>> PendingSync;
            public List<Waba> Wabas = new List<Waba>();
            public Task<Result<List<Waba>>> GetAllAsync() => Task.FromResult(FailList
                ? Result<List<Waba>>.Failure(Error.Unknown("refresh failed"))
                : Result<List<Waba>>.Success(Wabas));
            public Task<Result<bool>> UpdateCompanyAsync(string wabaId, string companyId) => throw new NotSupportedException();
            public Task<Result<bool>> SyncFromMetaAsync()
            {
                SyncCalls++;
                return PendingSync != null ? PendingSync.Task : Task.FromResult(FailSync
                    ? Result<bool>.Failure(Error.Unknown("Meta failed")) : Result<bool>.Success(true));
            }
        }

        [Test]
        public async Task SyncFindsAllNewAccountsAgainstOriginalBaseline()
        {
            var repo = new Repository { Wabas = new List<Waba> { new Waba { WabaId = "old" }, new Waba { WabaId = "new", Name = "New" }, new Waba { WabaId = "second", Name = "Second" } } };
            var view = new View();
            var presenter = new WabaOnboardingPresenter(repo, view, new[] { new Waba { WabaId = "old" } });
            await presenter.SyncAsync();
            Assert.That(presenter.NewWabas.Count, Is.EqualTo(2));
            Assert.That(view.Status, Does.Contain("New (new)").And.Contain("Second (second)"));
            Assert.That(view.Busy, Is.False);
            await presenter.SyncAsync();
            Assert.That(presenter.NewWabas.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task SyncWithoutNewAccountsAllowsRetry()
        {
            var repo = new Repository();
            var view = new View();
            var presenter = new WabaOnboardingPresenter(repo, view, new Waba[0]);
            await presenter.SyncAsync();
            Assert.That(view.Status, Does.Contain("belum ada WABA baru"));
            repo.Wabas.Add(new Waba { WabaId = "new" });
            await presenter.SyncAsync();
            Assert.That(presenter.NewWabas.Count, Is.EqualTo(1));
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task FailurePreservesBaselineAndAllowsRecovery(bool failSync)
        {
            var repo = new Repository { FailSync = failSync, FailList = !failSync };
            var view = new View();
            var presenter = new WabaOnboardingPresenter(repo, view, new Waba[0]);
            await presenter.SyncAsync();
            Assert.That(view.Status, Does.Contain("gagal"));
            Assert.That(view.Busy, Is.False);
            repo.FailSync = repo.FailList = false;
            repo.Wabas.Add(new Waba { WabaId = "new" });
            await presenter.SyncAsync();
            Assert.That(presenter.NewWabas.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task ConcurrentSubmitOnlySendsOneRequest()
        {
            var repo = new Repository { PendingSync = new TaskCompletionSource<Result<bool>>() };
            var view = new View();
            var presenter = new WabaOnboardingPresenter(repo, view, new Waba[0]);
            var first = presenter.SyncAsync();
            await presenter.SyncAsync();
            Assert.That(repo.SyncCalls, Is.EqualTo(1));
            Assert.That(view.Busy, Is.True);
            repo.PendingSync.SetResult(Result<bool>.Success(true));
            await first;
            Assert.That(view.Busy, Is.False);
        }
    }
}
