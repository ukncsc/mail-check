using System;
using System.Linq;
using System.Reflection;

namespace Dmarc.ForensicReport.Parser.Lambda.Dao.Entities
{
    [AttributeUsage(AttributeTargets.Field)]
    public class EnumDbNameAttribute : Attribute
    {
        public EnumDbNameAttribute(string displayName)
        {
            DisplayName = displayName;
        }

        public string DisplayName { get; set; }
    }

    public static class EnumExtensionMethods
    {
        public static string GetDbName(this Enum enumType)
        {
            var displayNameAttribute = enumType.GetType()
                                               .GetField(enumType.ToString())
                                               .GetCustomAttributes(typeof(EnumDbNameAttribute), false)
                                               .FirstOrDefault() as EnumDbNameAttribute;

            return displayNameAttribute != null ? displayNameAttribute.DisplayName : Enum.GetName(enumType.GetType(), enumType);
        }
    }
}