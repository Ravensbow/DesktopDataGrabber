using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DesktopDataGrabber.Tools
{
    public class StringValueAttribute : Attribute
    {
        public string sValue { get; protected set; }

        public StringValueAttribute(string value)
        {
            sValue = value;
        }
    }
    public static class ExtensionEnum
    {
        public static string GetStringValue(this Enum value)
        {
            Type type = value.GetType();
            FieldInfo fieldInfo = type.GetField(value.ToString());

            StringValueAttribute[] attribs = fieldInfo.GetCustomAttributes(
                typeof(StringValueAttribute), false) as StringValueAttribute[];

            return attribs.Length > 0 ? attribs[0].sValue : null;
        }
    }
}
