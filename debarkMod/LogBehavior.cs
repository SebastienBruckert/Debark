using System.Linq;
using Vintagestory.API.Common;

class LogBehavior : BlockBehavior
{
    public LogBehavior(Block block) : base(block)
    {

    }

    static public bool isValidLog(string name)
    {
        DebarkMod.CoreAPI.Logger.Debug(name);
        bool ret = false;
        if (name.StartsWith("log-")) {
            ret = true;
        }
        if (ModConfig.configData.canDebarkTree == false && name.Contains("-grown-")) {
            ret = false;
        }
        return ret;
    }

    static public string getDebarkedLog(string variant)
    {
        return "debarkedlog-" + variant;
    }

    private bool IsHoldingHammerInOffhand(IPlayer player)
    {
        IInventory characterInventory = player.InventoryManager.GetHotbarInventory();
        if (characterInventory == null) {
            DebarkMod.CoreAPI.Logger.Warning("No hotbar inventory");
            return false;
        }

        // Debug to know where offhand item is.
        // for (int i = 0; i < characterInventory.Count; i++) {
        //     ItemSlot slot = characterInventory[i];
        //     if (slot?.Itemstack != null) {
        //         string itemCode = slot.Itemstack.Collectible.Code.ToString();
        //         DebarkMod.CoreAPI.Logger.Debug("Item " + i + ": " + slot.GetStackName());
        //     } else {
        //         DebarkMod.CoreAPI.Logger.Debug("No hotbar " + i + " item");
        //     }
        // }

        ItemSlot offHandSlot = characterInventory.Last();
        if (offHandSlot?.Itemstack?.Item == null) {
            //DebarkMod.CoreAPI.Logger.Debug("No hotbar offhand item");
            return false;
        }
        //DebarkMod.CoreAPI.Logger.Debug("Item in offhand " + offHandSlot.GetStackName());
        return offHandSlot.Itemstack.Collectible.Tool == EnumTool.Hammer;
    }

    public override bool OnBlockInteractStart(IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel, ref EnumHandling handling)
    {
        Block block = world.BlockAccessor.GetBlock(blockSel.Position);
        //DebarkMod.CoreAPI.Logger.Debug("Block interacted: " + block.Code.GetName());
        if (ModConfig.configData.needHammerInOffhand == true && ! IsHoldingHammerInOffhand(byPlayer)) {
            //DebarkMod.CoreAPI.Logger.Debug("Need hammer in off-hand!");
            return false;
        }
        if (isValidLog(block.Code.GetName())
         && byPlayer.InventoryManager.ActiveTool == EnumTool.Axe)
        {
            Block debarkedBlock = world.GetBlock(new AssetLocation(block.Code.Domain, getDebarkedLog(block.Code.CodePartsAfterSecond())));
            world.BlockAccessor.SetBlock(debarkedBlock.BlockId, blockSel.Position);
            long randomsound = world.Rand.Next(1, 3);
            world.PlaySoundAt(new AssetLocation("sounds/block/chop" + randomsound), byPlayer, byPlayer, true, 8);
            byPlayer.InventoryManager.ActiveHotbarSlot.Itemstack.Item.DamageItem(world, byPlayer.Entity, byPlayer.InventoryManager.ActiveHotbarSlot);
            ItemSlot offHandSlot = byPlayer.InventoryManager.GetHotbarInventory().Last();
            if (ModConfig.configData.needHammerInOffhand == true && offHandSlot?.Itemstack?.Item != null) {
                offHandSlot.Itemstack.Item.DamageItem(world, byPlayer.Entity, offHandSlot);
            }
            handling = EnumHandling.Handled;
            return true;
        }
        handling = EnumHandling.PassThrough;
        return true;
    }
}