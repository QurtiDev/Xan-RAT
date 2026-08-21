

using InvokedCommon.Utilities;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace InvokedServer.Build
{
	public class Renamer
	{
		private Renamer.MemberOverloader _typeOverloader;
		private Dictionary<TypeDefinition, Renamer.MemberOverloader> _methodOverloaders;
		private Dictionary<TypeDefinition, Renamer.MemberOverloader> _fieldOverloaders;
		private Dictionary<TypeDefinition, Renamer.MemberOverloader> _eventOverloaders;

		public AssemblyDefinition AsmDef { get; set; }

		private int Length { get; set; }

		public Renamer(AssemblyDefinition asmDef)
		  : this(asmDef, 20)
		{
		}

		public Renamer(AssemblyDefinition asmDef, int length)
		{
			this.AsmDef = asmDef;
			this.Length = length;
			this._typeOverloader = new Renamer.MemberOverloader(this.Length);
			this._methodOverloaders = new Dictionary<TypeDefinition, Renamer.MemberOverloader>();
			this._fieldOverloaders = new Dictionary<TypeDefinition, Renamer.MemberOverloader>();
			this._eventOverloaders = new Dictionary<TypeDefinition, Renamer.MemberOverloader>();
		}

		public bool Perform()
		{
			try
			{
				foreach (TypeDefinition typeDef in this.AsmDef.Modules.SelectMany<ModuleDefinition, TypeDefinition>((Func<ModuleDefinition, IEnumerable<TypeDefinition>>)(module => (IEnumerable<TypeDefinition>)module.Types)))
					this.RenameInType(typeDef);
				return true;
			}
			catch
			{
				return false;
			}
		}

		private void RenameInType(TypeDefinition typeDef)
		{
			if (!typeDef.Namespace.StartsWith("Invoked") || typeDef.Namespace.StartsWith("InvokedCommon.Messages") || typeDef.IsEnum)
				return;
			this._typeOverloader.GiveName((MemberReference)typeDef);
			typeDef.Namespace = string.Empty;
			Renamer.MemberOverloader methodOverloader = this.GetMethodOverloader(typeDef);
			Renamer.MemberOverloader fieldOverloader = this.GetFieldOverloader(typeDef);
			Renamer.MemberOverloader eventOverloader = this.GetEventOverloader(typeDef);
			if (typeDef.HasNestedTypes)
			{
				foreach (TypeDefinition nestedType in typeDef.NestedTypes)
					this.RenameInType(nestedType);
			}
			if (typeDef.HasMethods)
			{
				foreach (MethodDefinition member in typeDef.Methods.Where<MethodDefinition>((Func<MethodDefinition, bool>)(methodDef => !methodDef.IsConstructor && !methodDef.HasCustomAttributes && !methodDef.IsAbstract && !methodDef.IsVirtual)))
					methodOverloader.GiveName((MemberReference)member);
			}
			if (typeDef.HasFields)
			{
				foreach (FieldDefinition field in typeDef.Fields)
					fieldOverloader.GiveName((MemberReference)field);
			}
			if (!typeDef.HasEvents)
				return;
			foreach (EventDefinition member in typeDef.Events)
				eventOverloader.GiveName((MemberReference)member);
		}

		private Renamer.MemberOverloader GetMethodOverloader(TypeDefinition typeDef)
		{
			return this.GetOverloader(this._methodOverloaders, typeDef);
		}

		private Renamer.MemberOverloader GetFieldOverloader(TypeDefinition typeDef)
		{
			return this.GetOverloader(this._fieldOverloaders, typeDef);
		}

		private Renamer.MemberOverloader GetEventOverloader(TypeDefinition typeDef)
		{
			return this.GetOverloader(this._eventOverloaders, typeDef);
		}

		private Renamer.MemberOverloader GetOverloader(
		  Dictionary<TypeDefinition, Renamer.MemberOverloader> overloaderDictionary,
		  TypeDefinition targetTypeDef)
		{
			Renamer.MemberOverloader overloader;
			if (!overloaderDictionary.TryGetValue(targetTypeDef, out overloader))
			{
				overloader = new Renamer.MemberOverloader(this.Length);
				overloaderDictionary.Add(targetTypeDef, overloader);
			}
			return overloader;
		}

		private class MemberOverloader
		{
			private readonly Dictionary<string, string> _renamedMembers = new Dictionary<string, string>();
			private readonly char[] _charMap;
			private readonly SafeRandom _random = new SafeRandom();
			private int[] _indices;

			private bool DoRandom { get; set; }

			private int StartingLength { get; set; }

			public MemberOverloader(int startingLength, bool doRandom = true)
			  : this(startingLength, doRandom, "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToLower().ToCharArray())
			{
			}

			private MemberOverloader(int startingLength, bool doRandom, char[] chars)
			{
				this._charMap = chars;
				this.DoRandom = doRandom;
				this.StartingLength = startingLength;
				this._indices = new int[startingLength];
			}

			public void GiveName(MemberReference member)
			{
				string currentName = this.GetCurrentName();
				string key = member.ToString();
				member.Name = currentName;
				while (this._renamedMembers.ContainsValue(member.ToString()))
					member.Name = this.GetCurrentName();
				this._renamedMembers.Add(key, member.ToString());
			}

			private string GetCurrentName()
			{
				return !this.DoRandom ? this.GetOverloadedName() : this.GetRandomName();
			}

			private string GetRandomName()
			{
				StringBuilder stringBuilder = new StringBuilder();
				for (int index = 0; index < this.StartingLength; ++index)
					stringBuilder.Append((char)this._random.Next(int.MinValue, int.MaxValue));
				return stringBuilder.ToString();
			}

			private string GetOverloadedName()
			{
				this.IncrementIndices();
				char[] chArray = new char[this._indices.Length];
				for (int index = 0; index < this._indices.Length; ++index)
					chArray[index] = this._charMap[this._indices[index]];
				return new string(chArray);
			}

			private void IncrementIndices()
			{
				for (int index = this._indices.Length - 1; index >= 0; --index)
				{
					++this._indices[index];
					if (this._indices[index] < this._charMap.Length)
						break;
					if (index == 0)
						Array.Resize<int>(ref this._indices, this._indices.Length + 1);
					this._indices[index] = 0;
				}
			}
		}
	}
}