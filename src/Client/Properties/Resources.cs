

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;


namespace InvokedClient.Properties
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  internal class Resources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal Resources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (InvokedClient.Properties.Resources.resourceMan == null)
          InvokedClient.Properties.Resources.resourceMan = new ResourceManager("InvokedClient.Properties.Resources", typeof (InvokedClient.Properties.Resources).Assembly);
        return InvokedClient.Properties.Resources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get => InvokedClient.Properties.Resources.resourceCulture;
      set => InvokedClient.Properties.Resources.resourceCulture = value;
    }
  }
}
