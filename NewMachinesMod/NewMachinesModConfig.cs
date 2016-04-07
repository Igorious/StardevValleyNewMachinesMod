using System;
using System.IO;
using Newtonsoft.Json;
using StardewModdingAPI;

namespace Igorious.StardewValley.NewMachinesMod
{
    public partial class NewMachinesModConfig
    {
        public NewMachinesModConfig Load(string basePath)
        {
            var defaultSetting = new NewMachinesModConfig().FillWithDefaults();
            var jsonSettings = new JsonSerializerSettings {DefaultValueHandling = DefaultValueHandling.Ignore};

            var properties = GetType().GetProperties();
            foreach (var property in properties)
            {
                var value = property.GetValue(defaultSetting);

                var filePath = Path.Combine(basePath, $"{property.Name}.json");
                if (File.Exists(filePath))
                {
                    try
                    {
                        value = JsonConvert.DeserializeObject(File.ReadAllText(filePath), property.PropertyType, jsonSettings);
                    }
                    catch (Exception e)
                    {
                        Log.Error($"Exception during reading {property.Name}. Cause: {e}");
                    }
                }
                else
                {
                    File.WriteAllText(filePath, JsonConvert.SerializeObject(value));
                    Log.Info($"Created default configuration for {property.Name}");
                }
                property.SetValue(this, value);
            }

            return this;
        }
    }
}
