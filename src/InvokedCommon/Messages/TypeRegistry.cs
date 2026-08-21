

using ProtoBuf.Meta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;


namespace InvokedCommon.Messages
{
	public static class TypeRegistry
	{
		private static int _typeIndex;

		public static void AddTypeToSerializer(Type parent, Type type)
		{
			if (type == (Type) null || parent == (Type) null)
				throw new ArgumentNullException();
			if (((IEnumerable<SubType>) RuntimeTypeModel.Default[parent].GetSubtypes()).Any<SubType>((Func<SubType, bool>) (subType => subType.DerivedType.Type == type)))
				return;
			RuntimeTypeModel.Default[parent].AddSubType(++TypeRegistry._typeIndex, type);
		}

		public static void AddTypesToSerializer(Type parent, params Type[] types)
		{
			foreach (Type type in types)
				TypeRegistry.AddTypeToSerializer(parent, type);
		}

		public static IEnumerable<Type> GetPacketTypes(Type type)
		{
			return ((IEnumerable<Assembly>) AppDomain.CurrentDomain.GetAssemblies()).SelectMany<Assembly, Type>((Func<Assembly, IEnumerable<Type>>) (s => (IEnumerable<Type>) s.GetTypes())).Where<Type>((Func<Type, bool>) (p => type.IsAssignableFrom(p) && !p.IsInterface));
		}
	}
}
