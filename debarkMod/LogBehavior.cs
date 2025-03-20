using Vintagestory.API.Common;
using Vintagestory.API.Server;
using Vintagestory.API.Client;
using Vintagestory.API.MathTools;
using System;

class LogBehavior : BlockBehavior
{
    public LogBehavior(Block block) : base(block)
    {

    }

    private bool isValidLog(string name)
    {
        return name.StartsWith("log-");
    }

    private string getDebarkedLog(string variant)
    {
        return "debarkedlog-" + variant;
    }

    public override bool OnBlockInteractStart(IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel, ref EnumHandling handling)
    {
        DebarkMod.CoreAPI.Logger.Chat("DebarkMod: Onblockinteractstart");
        Block block = world.BlockAccessor.GetBlock(blockSel.Position);
        DebarkMod.CoreAPI.Logger.Chat("DebarkMod: " + block.Code.ToString());
        DebarkMod.CoreAPI.Logger.Chat("DebarkMod: " + block.Code.GetName());
        DebarkMod.CoreAPI.Logger.Chat("DebarkMod: " + block.Code.EndVariant());
        DebarkMod.CoreAPI.Logger.Chat("DebarkMod: " + block.Code.CodePartsAfterSecond());
        DebarkMod.CoreAPI.Logger.Chat("DebarkMod: " + block.Code.Domain);
        if (isValidLog(block.Code.GetName()))
        {
            Block debarkedBlock = world.GetBlock(new AssetLocation(block.Code.Domain, getDebarkedLog(block.Code.CodePartsAfterSecond())));
            world.BlockAccessor.SetBlock(debarkedBlock.BlockId, blockSel.Position);
            handling = EnumHandling.Handled;
            return true;
        }
        handling = EnumHandling.PassThrough;
        return true;
    }
}