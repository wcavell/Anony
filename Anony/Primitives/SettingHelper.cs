using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Anony.Primitives
{
    class SettingHelper
    {
        public const string DefChannelName = "DefChannelName";
        public static object GetValue(string key, object def = null)
        {
            var result = ApplicationData.Current.LocalSettings.Values[key];
            if (result == null)
            {
                if (def != null)
                {
                    ApplicationData.Current.LocalSettings.Values[key] = def;
                    return def;
                }
                else return 0;
            }
            return result;
        }

        public static void SetValue(string key, object value)
        {
            ApplicationData.Current.LocalSettings.Values[key] = value;
        }
    }
}
