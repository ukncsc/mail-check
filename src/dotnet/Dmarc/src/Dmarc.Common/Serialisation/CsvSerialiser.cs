using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Dmarc.Common.Util;

namespace Dmarc.Common.Serialisation
{
    public class CsvSerialiser
    {
        private const char Quote = '\"';
        private const char CarriageReturn = '\r';
        private const char LineFeed = '\n';
        private const char Eqls = '=';
        private const char Plus = '+';
        private const char Minus = '-';
        private const char At = '@';
        private const char Apostrophe = '\'';

        public static async Task<Stream> SerialiseAsync<T>(IEnumerable<T> ts, bool includeHeaders = true, char delimiter = ',')
        {
            Stream stream = new MemoryStream();

            Type type = ts.GetType().GetGenericArguments()[0];

            PropertyInfo[] propertyInfos = type.GetProperties();

            using (StreamWriter streamWriter = new StreamWriter(stream, Encoding.UTF8, 1024, true))
            {
                if (includeHeaders)
                {
                    await streamWriter.WriteLineAsync(string.Join(new string(new [] {delimiter}), propertyInfos.Select(_ => _.Name.PascalToSnakeCase())));
                }

                foreach (T record in ts)
                {
                    await streamWriter.WriteLineAsync(GetRecord(record, delimiter));
                }

                await streamWriter.FlushAsync();
                stream.Position = 0;
                return stream;
            }
        }

        public static Stream Serialise<T>(IEnumerable<T> ts, bool includeHeaders = true, char delimiter = ',')
        {
            Stream stream = new MemoryStream();

            Type type = ts.GetType().GetGenericArguments()[0];

            PropertyInfo[] propertyInfos = type.GetProperties();

            using (StreamWriter streamWriter = new StreamWriter(stream, Encoding.UTF8, 1024, true))
            {
                if (includeHeaders)
                {
                    streamWriter.WriteLine(string.Join(new string(new[] { delimiter }), propertyInfos.Select(_ => _.Name.PascalToSnakeCase())));
                }

                foreach (T record in ts)
                {
                    streamWriter.WriteLine(GetRecord(record, delimiter));
                }

                streamWriter.Flush();
                stream.Position = 0;
                return stream;
            }
        }

        private static string GetRecord<T>(T t, char delimiter)
        {
            PropertyInfo[] propertyInfos = t.GetType().GetProperties();
            return string.Join(new string(new[] { delimiter }), propertyInfos.Select(_ => GetValue(t, _, delimiter)));
        }

        private static string GetValue<T>(T t, PropertyInfo propertyInfo, char delimiter)
        {
            object value = propertyInfo.GetValue(t);

            return value == null 
                ? null
                : EncodeString(value.ToString(), delimiter);
        }

        private static string EncodeString(string inputString, char delimiter)
        {
            List<char> outputChars = new List<char>();
            bool needsEncoding = false;
            char[] inputCharArray = inputString.ToCharArray();

            for (int i = 0; i < inputCharArray.Length; i++)
            {
                char chr = inputCharArray[i];

                if (i == 0 && (chr == Eqls || chr == Plus || chr == Minus || chr == At))
                {
                    outputChars.Add(Apostrophe);
                }

                if (chr == delimiter || chr == CarriageReturn || chr == LineFeed)
                {
                    needsEncoding = true;
                }
                else if (chr == Quote)
                {
                    outputChars.Add(Quote);
                    needsEncoding = true;
                }
                outputChars.Add(chr);
            }

            return needsEncoding
                ? $"\"{new string(outputChars.ToArray())}\""
                : new string(outputChars.ToArray());
        }
    }
}
