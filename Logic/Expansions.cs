using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Models.Entities;
using Models.Enums;
using Models.Requests;

namespace Logic
{
    public static class Expansions
    {
        public static IEnumerable<string> GetDescriptions<T>(this T enumValue) where T : Type
        {
            var arr = enumValue.GetEnumValues();
            var result = new List<string>();

            foreach (var o in arr)
            {
                var fieldInfo = enumValue.GetField(o.ToString() ?? enumValue.ToString());

                if (fieldInfo is null) return new[] {enumValue.ToString()};

                var attrs = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), true)
                    .Where(x => x.GetType() == typeof(DescriptionAttribute))
                    .Select(x => (DescriptionAttribute) x);

                result.Add(attrs.First().Description);
            }

            return result;
        }

        public static string DescriptionAttr<T>(this T source)
        {
            var field = source.GetType().GetField(source.ToString() ?? string.Empty);

            if (field is null) return string.Empty;

            var attributes = (DescriptionAttribute[]) field.GetCustomAttributes(
                typeof(DescriptionAttribute), false);

            return attributes.Length > 0 ? attributes[0].Description : source.ToString();
        }

        public static Dictionary<string, int> GetForDropList<T>(this T enumValue) where T : Type
        {
            var arr = enumValue.GetEnumValues();
            var result = new Dictionary<string, int>();

            foreach (var o in arr)
            {
                var fieldInfo = enumValue.GetField(o.ToString() ?? enumValue.ToString());
                
                if (fieldInfo is null) return new Dictionary<string, int>();

                var attrs = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), true)
                    .Where(x => x.GetType() == typeof(DescriptionAttribute))
                    .Select(x => (DescriptionAttribute) x);
                
                result.Add(attrs.First().Description, (int)o);
            }

            return result;
        }

        public static string ToStringCurse(this CurseRequest request)
        {
            return $"{request.From.DescriptionAttr()}_{request.To.DescriptionAttr()}";
        }
        
        public static string ToStringCurse(this Curse request)
        {
            return $"{request.CurrenciesFrom.DescriptionAttr()}_{request.CurrenciesTo.DescriptionAttr()}";
        }

        public static (CurrenciesEnum from, CurrenciesEnum to) ParseStr(this string responseKey)
        {
            var result = responseKey.Split('_');

            return (Enum.Parse<CurrenciesEnum>(result[0]), Enum.Parse<CurrenciesEnum>(result[1]));
        }
    }
}