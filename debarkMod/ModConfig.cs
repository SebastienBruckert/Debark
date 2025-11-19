using System;
using Vintagestory.API.Common;

public class ModConfig
{
    public static ConfigData configData;

    private static string configFile = "DebarkModConfig.json";

    public static void tryToLoadConfig(ICoreAPI api)
    {
        try {
            configData = api.LoadModConfig<ConfigData>(configFile);
            if (configData == null) {
                configData = new ConfigData();
            }
            api.StoreModConfig<ConfigData>(configData, configFile);
        } catch (Exception e) {
            DebarkMod.CoreAPI.Logger.Error("Could not load config! Loading default settings instead.");
            DebarkMod.CoreAPI.Logger.Error(e.Message);
            configData = new ConfigData();
        }
    }
}
