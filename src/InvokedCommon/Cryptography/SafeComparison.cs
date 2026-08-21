

using System.Runtime.CompilerServices;


namespace InvokedCommon.Cryptography
{
	public class SafeComparison
	{
		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
		public static bool AreEqual(byte[] a1, byte[] a2)
		{
			bool flag = true;
			for (int index = 0; index < a1.Length; ++index)
			{
				if ((int) a1[index] != (int) a2[index])
					flag = false;
			}
			return flag;
		}
	}
}
