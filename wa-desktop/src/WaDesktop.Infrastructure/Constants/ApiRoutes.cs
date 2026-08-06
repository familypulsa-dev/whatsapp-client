namespace WaDesktop.Infrastructure.Constants
{
    internal static class ApiRoutes
    {
        public static class Auth
        {
            public const string Login = "/api/v1/auth/login";
            public const string Refresh = "/api/v1/auth/refresh";
        }
        
        public static class Companies
        {
            public const string Base = "/api/v1/companies";
        }
        
        public static class Users
        {
            public const string Base = "/api/v1/users";
        }
        
        public static class PhoneNumbers
        {
            public const string Base = "/api/v1/phone-numbers";
        }
        
        public static class Templates
        {
            public const string Base = "/api/v1/templates";
        }
        
        public static class Settings
        {
            public const string Base = "/api/v1/settings";
        }
        
        public static class Waba
        {
            public const string Base = "/api/v1/waba";
            public const string Usage = "/api/v1/waba/usage";
        }
        
        public static class Wabas
        {
            public const string Base = "/api/v1/wabas";
        }
        
        public static class Analytics
        {
            public const string Billing = "/api/v1/analytics/billing";
        }
        
        public static class Webhook
        {
            public const string Setup = "/api/v1/webhook/setup";
            public const string Health = "/api/v1/health";
        }
    }
}
