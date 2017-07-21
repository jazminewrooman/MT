using System;
using System.Collections.Generic;
using System.Text;
#if __ANDROID__
using Card.IO;
#endif

namespace MasTicket
{
    public interface ICardReader
    {
#if __ANDROID__
        Action<string, int, int, string, CardType> CardFound { get; set; }
        void ReadCard();
#endif
#if __IOS__
		//Action<string, int, int, string, CreditCardType> CardFound { get; set; }
        //void ReadCard();
#endif

	}
}
