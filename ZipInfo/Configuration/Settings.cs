using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Sitecore;
using Sitecore.Configuration;
using Sitecore.Shell.Applications.Install.Commands;
using Sitecore.StringExtensions;
using Sitecore.Xml;
using ZipInfo.Providers;

namespace ZipInfo.Configuration
{
    public partial class Settings
    {
        private static string settingsNodeName = "/sitecore/zipInfo";

        public static class Cache
        {
            public static long MaxSize = StringUtil.ParseSizeString(XmlUtil.GetAttribute("maxSize", Factory.GetConfigNode("{0}/cache".FormatWith(settingsNodeName)), "1MB"));
            public static int ObjectSize = Int32.Parse(XmlUtil.GetAttribute("objectSize", Factory.GetConfigNode("{0}/cache".FormatWith(settingsNodeName)), "200"));
        }

        public static XmlNode ProviderConfig = Factory.GetConfigNode("{0}/provider".FormatWith(settingsNodeName));
        public static string ProviderName = XmlUtil.GetAttribute("type", ProviderConfig);

        public static IZipInfoProvider GetProvider()
        {
            return (IZipInfoProvider)Sitecore.Reflection.ReflectionUtil.CreateObject(ProviderName, new object[] {});
        }
    }
}
