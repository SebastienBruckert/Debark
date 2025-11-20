using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.API.Client;
using Vintagestory.API.Util;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Server;

public class DebarkMod : ModSystem
{
    public static ICoreAPI CoreAPI;
    public static ICoreClientAPI ClientAPI;
    public Harmony harmony;
    public static bool jacksadzeModLoaded;


    public override void Start(ICoreAPI api)
    {
        CoreAPI = api;
        if (!Harmony.HasAnyPatches(Mod.Info.ModID))
        {
            harmony = new Harmony(Mod.Info.ModID);
            harmony.PatchAll();
        }
        CoreAPI.RegisterBlockBehaviorClass("DebarkMod_Behavior", typeof(LogBehavior));
        CoreAPI.Logger.Notification("DebarkMod: Started.");
        jacksadzeModLoaded = api.ModLoader.IsModEnabled("jacksadze");
    }

    public override void StartServerSide(ICoreServerAPI api)
    {
        ModConfig.tryToLoadConfig(api);
    }
    public override void StartClientSide(ICoreClientAPI api)
    {
        // This will do the job for now.
        ModConfig.tryToLoadConfig(api);
    }
}

[HarmonyPatch(typeof(Block), nameof(Block.GetPlacedBlockInteractionHelp))]
internal class BlockLog_GetPlacedBlockInteractionHelp_Patch
{
    private static List<ItemStack> toolItems = new List<ItemStack>();
    public static void Postfix(ref WorldInteraction[] __result, IWorldAccessor world, BlockSelection selection, IPlayer forPlayer)
    {
        if (toolItems.Count == 0) {
            if (DebarkMod.jacksadzeModLoaded == false)
            {
                Item[] axes = world.SearchItems(new AssetLocation("axe-*"));
                foreach(Item item in axes) toolItems.Add(new ItemStack(item));
            } else
            {
                Item[] hoes = world.SearchItems(new AssetLocation("hoe-*"));
                foreach(Item item in hoes) toolItems.Add(new ItemStack(item));
            }
        }
        string ActionLangCode = "Debark log";
        if (DebarkMod.jacksadzeModLoaded == false && ModConfig.configData.needHammerInOffhand == true) {
            ActionLangCode += " (Requires hammer in offhand)";
        }

        //if (world.Config.GetBool("needHammerInOffhand", false)) {
        Block block = world.BlockAccessor.GetBlock(selection.Position);
        //WorldInteraction[] interactions = block.GetPlacedBlockInteractionHelp(world, selection, forPlayer);
        if (block != null && block.Code != null && LogBehavior.isValidLog(block.Code.GetName()))
        {
            __result = __result.Append(new WorldInteraction()
            {
                ActionLangCode = ActionLangCode,
                MouseButton = EnumMouseButton.Right,
                Itemstacks = toolItems.ToArray(),
            });
        }
    }
}