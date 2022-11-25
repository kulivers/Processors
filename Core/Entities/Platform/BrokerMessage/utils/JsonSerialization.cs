using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PlatformEntities.Platform.BrokerMessage.utils
{
    public static class JsonSerialization
    {
        private static readonly JsonSerializerSettings _serializationSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            DateTimeZoneHandling = DateTimeZoneHandling.Local,
            TypeNameHandling = TypeNameHandling.Auto,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            ObjectCreationHandling = ObjectCreationHandling.Replace,
            Error = OnSerializationError,
            Converters = new List<Newtonsoft.Json.JsonConverter> 
            {
                new Newtonsoft.Json.Converters.StringEnumConverter()
            }
        };

        /// <summary>
        /// Doesn`t specify type on deserialization
        /// </summary>
        private static readonly JsonSerializerSettings _nonTypeSerializationSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            DateTimeZoneHandling = DateTimeZoneHandling.Local,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            ObjectCreationHandling = ObjectCreationHandling.Replace,
            Error = OnSerializationError,
            Converters = new List<Newtonsoft.Json.JsonConverter>
            {
                new Newtonsoft.Json.Converters.StringEnumConverter()
            }
        };

        private static JsonSerializer _commonSerializer = Newtonsoft.Json.JsonSerializer.Create(_serializationSettings);
        private static JsonSerializer _specialSerializer = Newtonsoft.Json.JsonSerializer.Create(_nonTypeSerializationSettings);

        private static void OnSerializationError(object sender, Newtonsoft.Json.Serialization.ErrorEventArgs e)
        {
            e.ErrorContext.Handled = true;
        }

        #region TypeConversion
        public static TOutput TransformTo<TOutput>(this object obj)
        {
            var json = Serialize(obj);
            return Deserialize<TOutput>(json);
        }

        public static object TransformTo(this object obj, Type outputType)
        {
            var json = Serialize(obj);
            return JsonConvert.DeserializeObject(json, outputType, _serializationSettings);
        }
        #endregion TypeConversion

        public static byte[] Serialize<T>(this T data, bool isSpecial)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                JsonWriter jsonWriter = new JsonTextWriter(new System.IO.StreamWriter(stream));
                if (!isSpecial)
                {
                    _commonSerializer.Serialize(jsonWriter, data);
                }
                else
                {
                    _specialSerializer.Serialize(jsonWriter, data);
                }
                jsonWriter.Flush();
                stream.Flush();
                return stream.ToArray();
            }
        }

        public static string Serialize(this object obj, Formatting formatting = Formatting.None, bool serializeType = true)
        {
            if (obj == null)
            {
                return string.Empty;
            }

            return JsonConvert.SerializeObject(obj, formatting, serializeType ? _serializationSettings : _nonTypeSerializationSettings);
        }

        public static T Deserialize<T>(this byte[] data)
        {
            if (data == null || data.Length == 0)
            {
                return default(T);
            }
            using (JsonReader reader = new JsonTextReader(new StreamReader(new MemoryStream(data))))
            {
                return _commonSerializer.Deserialize<T>(reader);
            }
        }

        public static T Deserialize<T>(this string obj)
        {
            if (string.IsNullOrEmpty(obj))
            {
                return default(T);
            }

            return JsonConvert.DeserializeObject<T>(obj, _serializationSettings);
        }

        public static bool TryDeserialize<T>(this string jsonString, Type type, out T result)
        {
            result = default(T);
            if (string.IsNullOrEmpty(jsonString))
            {
                return false;
            }

            var obj = JsonConvert.DeserializeObject(jsonString, type, _serializationSettings);
            if (obj is T deserializationResult)
            {
                result = deserializationResult;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Desirialize from json data, where type was not specified
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonString">Input json string</param>
        /// <param name="result">Deserialization result</param>
        /// <param name="type">Deserialization type</param>
        /// <returns></returns>
        public static bool TryDeserialize<T>(this string jsonString, Type type, bool specifiedType, out T result)
        {
            result = default(T);
            if (string.IsNullOrEmpty(jsonString))
            {
                return false;
            }

            var obj = JsonConvert.DeserializeObject(jsonString, type, specifiedType ? _serializationSettings : _nonTypeSerializationSettings);
            if (obj is T deserializationResult)
            {
                result = deserializationResult;
                return true;
            }

            return false;
        }

        public static Dictionary<string, object> Convert(this string jsonString)
        {
            if (string.IsNullOrEmpty(jsonString))
            {
                return default(Dictionary<string, object>);
            }

            var rawObject = JsonConvert.DeserializeObject(jsonString, _serializationSettings);
            return DictionaryJConverter.ToDictionary(rawObject) as Dictionary<string, object>;
        }
    }

    internal static class DictionaryJConverter
    {
        internal static object ToDictionary(object jtoken)
        {
            switch (jtoken)
            {
                case JObject jObject:
                    return ((IEnumerable<KeyValuePair<string, JToken>>)jObject).ToDictionary(j => j.Key, j => ToDictionary(j.Value));
                case JArray jArray:
                    return jArray.Select(ToDictionary).ToList();
                case JValue jValue:
                    return jValue.Value;
                default:
                    return jtoken;
            }
        }
    }
}
