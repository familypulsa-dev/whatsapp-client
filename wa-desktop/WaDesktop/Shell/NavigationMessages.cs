using System;

namespace WaDesktop.Domain.Messages
{
    /// <summary>Immutable message untuk membuka tab di workspace.</summary>
    public class RequestOpenTabMessage
    {
        public string ModuleKey { get; }
        public string Title { get; }
        public RequestOpenTabMessage(string moduleKey, string title)
        {
            ModuleKey = moduleKey;
            Title = title;
        }
    }

    /// <summary>Immutable message untuk menutup tab.</summary>
    public class RequestCloseTabMessage
    {
        public string ModuleKey { get; }
        public RequestCloseTabMessage(string moduleKey) => ModuleKey = moduleKey;
    }

    /// <summary>Refresh Tab</summary>
    public class RequestRefreshTabMessage
    {
        public string ModuleKey { get; }
        public RequestRefreshTabMessage(string moduleKey) => ModuleKey = moduleKey;
    }

    /// <summary>Immutable message: login berhasil.</summary>
    public class LoginCompletedMessage
    {
        public string DisplayName { get; }
        public string Role { get; }
        public LoginCompletedMessage(string displayName, string role)
        {
            DisplayName = displayName;
            Role = role;
        }
    }

    /// <summary>Immutable message: logout.</summary>
    public class LogoutMessage { }

    /// <summary>Immutable message: session expired (401), perlu login ulang.</summary>
    public class SessionExpiredMessage { }

    /// <summary>Published after a successful silent token refresh (no user interaction).</summary>
    public class TokenRefreshedMessage { }
}
