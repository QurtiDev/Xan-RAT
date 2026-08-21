

using ProtoBuf;
using System;


namespace InvokedCommon.Structs
{
	[ProtoContract]
	public struct GeckoCreditCard
	{
		[ProtoMember(1)]
		public string cardholderName;
		[ProtoMember(2)]
		public string cardType;
		[ProtoMember(3)]
		public string cardNumber;
		[ProtoMember(4)]
		public int expirationMonth;
		[ProtoMember(5)]
		public int expirationYear;

		public GeckoCreditCard(
		    string _cardholderName,
		    string _cardType,
		    string _cardNumber,
		    int _expirationMonth,
		    int _expirationYear)
		{
			this.cardholderName = _cardholderName;
			this.cardType = _cardType;
			this.cardNumber = _cardNumber;
			this.expirationMonth = _expirationMonth;
			this.expirationYear = _expirationYear;
		}

		public override string ToString()
		{
			return "CARDHOLDER_NAME: " + this.cardholderName + Environment.NewLine + "CARD_TYPE: " + this.cardType + Environment.NewLine + "CARD_NUMBER: " + this.cardNumber + Environment.NewLine + "EXPIRATION_MONTH: " + this.expirationMonth.ToString() + Environment.NewLine + "EXPIRATION_YEAR: " + this.expirationYear.ToString();
		}
	}
}