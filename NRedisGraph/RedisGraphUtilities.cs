﻿using System.Text;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace NRedisGraph
{
    internal static class RedisGraphUtilities
    {
        internal static string PrepareQuery(string query, IDictionary<string, object> parms)
        {
            var preparedQuery = new StringBuilder();

            preparedQuery.Append("CYPHER ");

            foreach (var param in parms)
            {
                preparedQuery.Append($"{param.Key}={ValueToString(param.Value)} ");
            }

            preparedQuery.Append(query);

            return preparedQuery.ToString();
        }
        
        internal static string ValueToString(object value)
        {
            if (value == null)
            {
                return "null";
            }

            if (value is string stringValue)
            {
                return QuoteString(stringValue);
            }

            if (value is char charValue)
            {
                return QuoteCharacter(charValue);
            }

            if (value.GetType().IsArray)
            {
                if (value is IEnumerable arrayValue)
                {
                    var values = new List<object>();

                    foreach (var v in arrayValue)
                    {
                        values.Add(v);
                    }

                    return ArrayToString(values.ToArray());
                }
            }

            if ((value is System.Collections.IList valueList) && value.GetType().IsGenericType)
            {
                var objectValueList = new List<object>();

                foreach (var val in valueList)
                {
                    objectValueList.Add((object) val);
                }

                return ArrayToString(objectValueList.ToArray());
            }

            if (value is bool boolValue)
            {
                return boolValue.ToString().ToLowerInvariant();
            }

            if (value is float floatValue)
            {
                return floatValue.ToString(CultureInfo.InvariantCulture);
            } 

            if (value is decimal decimalValue)
            {
                return decimalValue.ToString(CultureInfo.InvariantCulture);
            }

            if (value is double doubleValue)
            {
                return doubleValue.ToString(CultureInfo.InvariantCulture);
            }

            return value.ToString();
        }
        
        private static string ArrayToString(object[] array)
        {
            var arrayElements = array.Select(x =>
            {
                if (x.GetType().IsArray)
                {
                    return ArrayToString((object[]) x);
                }
                else
                {
                    return ValueToString(x);
                }
            });

            var arrayToString = new StringBuilder();

            arrayToString.Append('[');
            arrayToString.Append(string.Join(", ", arrayElements));
            arrayToString.Append(']');

            return arrayToString.ToString();
        }
        
        internal static string QuoteCharacter(char character) =>
            $"\"{character}\"";

        internal static string QuoteString(string unquotedString)
        {
            var quotedString = new StringBuilder(unquotedString.Length + 12);

            quotedString.Append('"');
            quotedString.Append(unquotedString.Replace("\"", "\\\""));
            quotedString.Append('"');

            return quotedString.ToString();
        }
    }
}