using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using System;


namespace IMMIWeb.Infrastructure
{

    public static class SessionFactory
    {
        private static IHttpContextAccessor _httpContextAccessor;

        public static void Configure(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public static int CurrentUserId
        {
            get
            {
                var sessionValue = _httpContextAccessor?.HttpContext?.Session?.GetInt32("CurrentUserId");
                return sessionValue ?? default(int);
            }
            set
            {
                _httpContextAccessor?.HttpContext?.Session?.SetInt32("CurrentUserId", value);
            }
        }
        public static string CurrentUserMobile
        {
            get
            {
                var sessionValue = _httpContextAccessor?.HttpContext?.Session?.GetString("CurrentUserMobile");
                return sessionValue ?? default(string);
            }
            set
            {
                _httpContextAccessor?.HttpContext?.Session?.SetString("CurrentUserMobile", value);
            }
        }
       
        public static string CurrentUserName
        {
            get
            {
                var sessionValue = _httpContextAccessor?.HttpContext?.Session?.GetString("CurrentUserName");
                return sessionValue ?? default(string);
            }
            set
            {
                _httpContextAccessor?.HttpContext?.Session?.SetString("CurrentUserName", value);
            }
        }
        public static string CurrentUserEmail
        {
            get
            {
                var sessionValue = _httpContextAccessor?.HttpContext?.Session?.GetString("CurrentUserEmail");
                return sessionValue ?? default(string);
            }
            set
            {
                _httpContextAccessor?.HttpContext?.Session?.SetString("CurrentUserEmail", value);
            }
        }

        public static int CurrentUserCountryId
        {
            get
            {
                var sessionValue = _httpContextAccessor?.HttpContext?.Session?.GetInt32("CurrentUserCountryId");
                return sessionValue ?? default(int);
            }
            set
            {
                _httpContextAccessor?.HttpContext?.Session?.SetInt32("CurrentUserCountryId", value);
            }
        }

        public static int CurrentUserCountryName
        {
            get
            {
                var sessionValue = _httpContextAccessor?.HttpContext?.Session?.GetInt32("CurrentUserCountryName");
                return sessionValue ?? default(int);
            }
            set
            {
                _httpContextAccessor?.HttpContext?.Session?.SetInt32("CurrentUserCountryName", value);
            }
        }

        public static int CurrentUserMobileCountryCode
        {
            get
            {
                var sessionValue = _httpContextAccessor?.HttpContext?.Session?.GetInt32("CurrentUserMobileCountryCode");
                return sessionValue ?? default(int);
            }
            set
            {
                _httpContextAccessor?.HttpContext?.Session?.SetInt32("CurrentUserMobileCountryCode", value);
            }
        }

        public static string CurrentUserProfilePic
        {
            get
            {
                var sessionValue = _httpContextAccessor?.HttpContext?.Session?.GetString("CurrentUserProfilePic");
                return sessionValue ?? default(string);
            }
            set
            {
                _httpContextAccessor?.HttpContext?.Session?.SetString("CurrentUserProfilePic", value);
            }
        }

        public static Dispatcher DispatcherTimer { get; set; }

        public static string CometChatUid
        {
            get
            {
                var sessionValue = _httpContextAccessor?.HttpContext?.Session?.GetString("CometChatUid");
                return sessionValue ?? default(string);
            }
            set
            {
                _httpContextAccessor?.HttpContext?.Session?.SetString("CometChatUid", value);
            }
        }
        public static string SessionCommetChatConsultnatList
        {
            get
            {
                var sessionValue = _httpContextAccessor?.HttpContext?.Session?.GetString("SessionCommetChatConsultnatList");
                return sessionValue ?? default(string);
            }
            set
            {
                _httpContextAccessor?.HttpContext?.Session?.SetString("SessionCommetChatConsultnatList", value);
            }
        }

        public static string SessionCommetChatUserList
        {
            get
            {
                var sessionValue = _httpContextAccessor?.HttpContext?.Session?.GetString("SessionCommetChatUserList");
                return sessionValue ?? default(string);
            }
            set
            {
                _httpContextAccessor?.HttpContext?.Session?.SetString("SessionCommetChatUserList", value);
            }
        }

        public static string TimeZone
        {
            get
            {
                var sessionValue = _httpContextAccessor?.HttpContext?.Session?.GetString("TimeZone");
                return sessionValue ?? default(string);
            }
            set
            {
                _httpContextAccessor?.HttpContext?.Session?.SetString("TimeZone", value);
            }
        }        
    }
}
