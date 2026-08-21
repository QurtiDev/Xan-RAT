

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.X509.Extension;
using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;


namespace InvokedServer.Helper
{
	public static class CertificateHelper
	{
		public static X509Certificate2 CreateCertificateAuthority(string caName, int keyStrength)
		{
			SecureRandom random = new SecureRandom((IRandomGenerator)new CryptoApiRandomGenerator());
			RsaKeyPairGenerator keyPairGenerator = new RsaKeyPairGenerator();
			keyPairGenerator.Init(new KeyGenerationParameters(random, keyStrength));
			AsymmetricCipherKeyPair keyPair = keyPairGenerator.GenerateKeyPair();
			X509V3CertificateGenerator certificateGenerator = new X509V3CertificateGenerator();
			X509Name x509Name = new X509Name("CN=" + caName + "O=hjkdashjksdajkh");
			certificateGenerator.SetSerialNumber(BigInteger.ProbablePrime(190, (Random)random));
			certificateGenerator.SetSubjectDN(x509Name);
			certificateGenerator.SetIssuerDN(x509Name);
			certificateGenerator.SetNotAfter(DateTime.UtcNow.AddYears(50));
			certificateGenerator.SetNotBefore(DateTime.UtcNow.Subtract(new TimeSpan(1, 1, 7, 6)));
			certificateGenerator.SetPublicKey(keyPair.Public);
			certificateGenerator.AddExtension(X509Extensions.SubjectKeyIdentifier, false, (Asn1Encodable)new SubjectKeyIdentifierStructure(keyPair.Public));
			certificateGenerator.AddExtension(X509Extensions.BasicConstraints, true, (Asn1Encodable)new BasicConstraints(true));
			certificateGenerator.AddExtension(X509Extensions.KeyUsage, true, (Asn1Encodable)new KeyUsage(6));
			certificateGenerator.AddExtension(X509Extensions.AuthorityKeyIdentifier, false, (Asn1Encodable)new AuthorityKeyIdentifierStructure(keyPair.Public));
			return new X509Certificate2(DotNetUtilities.ToX509Certificate(certificateGenerator.Generate((ISignatureFactory)new Asn1SignatureFactory("SHA256WITHRSA", keyPair.Private, random))))
			{
				PrivateKey = (AsymmetricAlgorithm)DotNetUtilities.ToRSA(keyPair.Private as RsaPrivateCrtKeyParameters)
			};
		}
	}
}