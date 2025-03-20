using System;
using Vintagestory.API.Common;
using Vintagestory.API.Server;
using Vintagestory.API.MathTools;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Client;
using System.Runtime.InteropServices;

public class DebarkMod : ModSystem
{
    public static ICoreAPI CoreAPI;

    public override void Start(ICoreAPI api)
    {
        CoreAPI = api;
        CoreAPI.RegisterBlockBehaviorClass("DebarkMod_Behavior", typeof(LogBehavior));
        CoreAPI.Logger.Notification("DebarkMod: Started.");
    }
}