using System;
using System.Collections.Generic;
using System.Text;

namespace MasTicket
{
    public static class Constantes
    {
        //Animated views
        #if __IOS__
            public static bool animated = false;
        #else
            public static bool animated = true;
		#endif

		//VESTA
		//demo
		//public static string FingerprintAPI = "";
		//prod
		public static string FingerprintAPI = "https://fingerprint.ecustomerpayments.com";
		public static string PaymentAPI = "https://vsafe1.ecustomerpayments.com/GatewayV3Proxy/Service";
		public static string TokenizationAPI = "https://vsafe1token.ecustomerpayments.com/GatewayV3ProxyJSON/Service";

		//Facebook
		public static string FBclientId = "1051990704849367";
        public static string FBscope = "email";
        public static string FBauthorizeUrl = "https://m.facebook.com/dialog/oauth/";
        public static string FBredirectUrl = "http://asicompras.com/ayudaapp/redes.htm";
        public static string FBgraph = "https://graph.facebook.com/me?fields=email,first_name,last_name,gender,picture";

		//Google
		//public static string GGClientId = "575858484188-compute@developer.gserviceaccount.com"; 
		public static string GGClientId = "575858484188-fu504sovv3e4nfo87af9152t9jpcvks5.apps.googleusercontent.com";
		public static string GGClientSecret = "H_86lz8ytei6OqpZAZwD63g4";
		public static string GGScope = "email profile"; //"https://www.googleapis.com/auth/userinfo.email";
		public static string GGAuthorizeUrl = "https://accounts.google.com/o/oauth2/auth";
		//public static string GGAccessTokenUrl = "https://accounts.google.com/o/oauth2/token";
		public static string GGAccessTokenUrl = "https://www.googleapis.com/oauth2/v4/token?grant_type=authorization_code";
		public static string GGUserInfoUrl = "https://www.googleapis.com/oauth2/v2/userinfo";
        public static string GGRedirectUrl = "http://asicompras.com/ayudaapp/redes.htm";

    }
}