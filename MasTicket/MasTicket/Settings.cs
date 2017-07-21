using Plugin.Settings;
using Plugin.Settings.Abstractions;

using System;

namespace MasTicket
{
    /// <summary>
    /// This is the Settings static class that can be used in your Core solution or in any
    /// of your client applications. All settings are laid out the same exact way with getters
    /// and setters. 
    /// </summary>
    public static class Settings
    {
        private static ISettings AppSettings
        {
            get
            {
                return CrossSettings.Current;
            }
        }

        #region Setting Constants
        private const string SettingsKey = "settings_key";
        private static readonly string SettingsDefault = string.Empty;
        // Tuto Visto 1a Vez
        private const string TutoVisto1aVezKey = "TutoVisto1aVez_key";
        private static readonly bool TutoVisto1aVezDefault = false;
		// salt p crypto
		private const string SaltCryptoKey = "SaltCrypto_key";
		private static readonly string SaltCryptoDefault = "35365464678680084688712";
		// pwd p crypto
		private const string PwdCryptoKey = "PwdCrypto_key";
		private static readonly string PwdCryptoDefault = "@#$%65tygsyd656*()_)(*";
        // ws
        private const string WebServiceKey = "WebService_key";
        private static readonly string WebServiceDefault = "https://asicompras.com/wsac/sac.svc";
		//private static readonly string WebServiceDefault = "http://192.168.15.113/wsac/sac.svc";
        // ws
        private const string WebServiceCatKey = "WebServiceCat_key";
        private static readonly string WebServiceCatDefault = "https://asicompras.com/wsac/wscatalogos.asmx";
		//private static readonly string WebServiceCatDefault = "http://192.168.15.113/wsac/wscatalogos.asmx";
		// usr p ws
		private const string WebServiceUsrKey = "WebServiceUsr_key";
		private static readonly string WebServiceUsrDefault = "wsac";
		// pwd p ws
		private const string WebServicePwdKey = "WebServicePwd_key";
		private static readonly string WebServicePwdDefault = "C0r1t2016";
        // ws chat
        private const string WebServChatKey = "WebServChat_key";
		private static readonly string WebServChatDefault = "http://192.168.15.113/wsac/schat.svc";
		// API user vesta
		private const string APIUsernameKey = "APIUsername_Key";
		private static readonly string APIUsernameDefault = "Recargasell";
		// API pwd vesta
		private const string APIPasswordKey = "APIPassword_Key";
		private static readonly string APIPasswordDefault = "vend-@RhJN$C5e&amp;96ZXZzj+iBdY3B-mN";
		// API payment vesta
		private const string MerchantRoutingIDKey = "MerchantRoutingID_Key";
		private static readonly string MerchantRoutingIDDefault = "18000000000004475000";

		#endregion


		public static string GeneralSettings
        {
            get
            {
                return AppSettings.GetValueOrDefault(SettingsKey, SettingsDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(SettingsKey, value);
            }
        }
        public static bool TutoVisto1aVez
        {
            get
            {
                return AppSettings.GetValueOrDefault(TutoVisto1aVezKey, TutoVisto1aVezDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(TutoVisto1aVezKey, value);
            }
        }
		public static string SaltCrypto
		{
			get
			{
				return AppSettings.GetValueOrDefault(SaltCryptoKey, SaltCryptoDefault);
			}
			set
			{
				AppSettings.AddOrUpdateValue(SaltCryptoKey, value);
			}
		}
		public static string PwdCrypto
		{
			get
			{
				return AppSettings.GetValueOrDefault(PwdCryptoKey, PwdCryptoDefault);
			}
			set
			{
				AppSettings.AddOrUpdateValue(PwdCryptoKey, value);
			}
		}
        public static string WebService
        {
            get
            {
                return AppSettings.GetValueOrDefault(WebServiceKey, WebServiceDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(WebServiceKey, value);
            }
        }
        public static string WebServiceCat
        {
            get
            {
                return AppSettings.GetValueOrDefault(WebServiceCatKey, WebServiceCatDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(WebServiceCatKey, value);
            }
        }
		public static string WebServiceUsr
		{
			get
			{
				return AppSettings.GetValueOrDefault(WebServiceUsrKey, WebServiceUsrDefault);
			}
			set
			{
				AppSettings.AddOrUpdateValue(WebServiceUsrKey, value);
			}
		}
		public static string WebServicePwd
		{
			get
			{
				return AppSettings.GetValueOrDefault(WebServicePwdKey, WebServicePwdDefault);
			}
			set
			{
				AppSettings.AddOrUpdateValue(WebServicePwdKey, value);
			}
		}
		public static string WebServChat
		{
			get
			{
				return AppSettings.GetValueOrDefault(WebServChatKey, WebServChatDefault);
			}
			set
			{
				AppSettings.AddOrUpdateValue(WebServChatKey, value);
			}
		}
		public static string APIUsername
		{
			get
			{
				return AppSettings.GetValueOrDefault(APIUsernameKey, APIUsernameDefault);
			}
			set
			{
				AppSettings.AddOrUpdateValue(APIUsernameKey, value);
			}
		}
		public static string APIPassword
		{
			get
			{
				return AppSettings.GetValueOrDefault(APIPasswordKey, APIPasswordDefault);
			}
			set
			{
				AppSettings.AddOrUpdateValue(APIPasswordKey, value);
			}
		}
		public static string MerchantRoutingID
		{
			get
			{
				return AppSettings.GetValueOrDefault(MerchantRoutingIDKey, MerchantRoutingIDDefault);
			}
			set
			{
				AppSettings.AddOrUpdateValue(MerchantRoutingIDKey, value);
			}
		}

	}
}