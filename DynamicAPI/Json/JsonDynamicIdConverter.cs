﻿using System;
using System.Collections.Generic;
using System.Linq;
using Igorious.StardewValley.DynamicAPI.Data;
using Newtonsoft.Json;

namespace Igorious.StardewValley.DynamicAPI.Json
{
    public sealed class JsonDynamicIdConverter<TEnum> : JsonConverter
    {
        private static Dictionary<string, int> NameToInt { get; } = new Dictionary<string, int>();
        private static Dictionary<int, string> IntToName { get; } = new Dictionary<int, string>();

        static JsonDynamicIdConverter()
        {
            var names = Enum.GetNames(typeof(TEnum));
            var values = (TEnum[])Enum.GetValues(typeof(TEnum));
            for (var i = 0; i < values.Length; ++i)
            {
                IntToName.Add(Convert.ToInt32(values[i]), names[i]);
                NameToInt.Add(names[i].ToLower(), Convert.ToInt32(values[i]));
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            var intValue = Convert.ToInt32(value);
            string name;
            if (IntToName.TryGetValue(intValue, out name))
            {
                if (writer.WriteState == WriteState.Object)
                {
                    writer.WritePropertyName(name);
                }
                else
                {
                    writer.WriteValue(name);
                }
            }
            else
            {
                if (writer.WriteState == WriteState.Object)
                {
                    writer.WritePropertyName(intValue.ToString());
                }
                else
                {
                    writer.WriteValue(intValue);
                }
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var converter = objectType.GetMethod("op_Implicit", new[] { typeof(int) });

            switch (reader.TokenType)
            {
                case JsonToken.Integer:
                {
                    var intValue = Convert.ToInt32(reader.Value);
                    return converter.Invoke(null, new object[] { intValue });
                }
                case JsonToken.PropertyName:
                case JsonToken.String:
                {
                    int intValue;
                    if (!int.TryParse(reader.Value.ToString(), out intValue))
                    {
                        intValue = NameToInt[reader.Value.ToString().ToLower()];
                    }
                    return converter.Invoke(null, new object[] { intValue });
                }
                default: return null;
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType.IsGenericType 
                && objectType.GetGenericTypeDefinition() == typeof(DynamicID<>)
                && objectType.GenericTypeArguments.Single() == typeof(TEnum);
        }
    }
}
