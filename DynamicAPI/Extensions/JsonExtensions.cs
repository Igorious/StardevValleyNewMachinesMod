using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Igorious.StardewValley.DynamicAPI.Extensions
{
    public static class JsonExtensions
    {
        public static string ToJson(this object o)
        {
            var buffer = new StringBuilder();
            using (var sw = new StringWriter(buffer))
            using (var writer = new JsonTextWriter(sw))
            {
                writer.QuoteName = false;
                var ser = new JsonSerializer { DefaultValueHandling = DefaultValueHandling.Ignore, Formatting = Formatting.Indented };
                ser.Converters.Add(new StringEnumConverter { AllowIntegerValues = true });
                ser.Serialize(writer, o);
            }
            return buffer.ToString();
        }
    }
}
