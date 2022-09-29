using Xamarin.Forms;

#if NET6_0_OR_GREATER
using XmlnsPrefixAttribute = Microsoft.Maui.Controls.XmlnsPrefixAttribute;

[assembly: XmlnsDefinition("http://sharpnado.com", "XamEffects")]
#endif

[assembly: XmlnsDefinition("http://sharpnado.com", "Sharpnado.Tabs")]
[assembly: XmlnsDefinition("http://sharpnado.com", "Sharpnado.Tabs.Effects")]
[assembly: XmlnsPrefix("http://sharpnado.com", "sho")]