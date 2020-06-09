using System.Collections.Generic;
using System.Windows.Forms;

namespace TGC.Group.Model
{
    internal static class GameCraftingManager
    {
        private struct Constants
        {
            public static int WEAPON_COUNT_ORE_SILVER = 1;
            public static int WEAPON_COUNT_CORAL_NORMAL = 3;
            public static int WEAPON_COUNT_CORAL_TREE = 2;
            public static int WEAPON_COUNT_FISH_NORMAL = 4;
            public static int WEAPON_COUNT_FISH_YELLOW = 6;

            public static int DIVING_HELMET_COUNT_ORE_GOLD = 2;
            public static int DIVING_HELMET_COUNT_ORE_IRON = 3;
            public static int DIVING_HELMET_COUNT_CORAL_SPIRAL = 1;
            public static int DIVING_HELMET_COUNT_CORAL_TREE = 2;
            public static int DIVING_HELMET_COUNT_FISH_NORMAL = 4;
            public static int DIVING_HELMET_COUNT_FISH_YELLOW = 1;

            public static int CATCH_FISH_COUNT_ORE_IRON = 1;
            public static int CATCH_FISH_COUNT_ORE_SILVER = 2;
            public static int CATCH_FISH_COUNT_CORAL_NORMAL = 3;
            public static int CATCH_FISH_COUNT_CORAL_TREE = 1;
        }

        public static bool HasWeapon { get; set; }
        public static bool HasDivingHelmet { get; set; }
        public static bool CanFish { get; set; }

        public static bool CanCraftWeapon(Dictionary<string, List<string>> items)
        {
            if (HasWeapon) return false;

            if (items["SILVER"].Count >= Constants.WEAPON_COUNT_ORE_SILVER) //&&
                 //items["NORMALCORAL"].Count >= Constants.WEAPON_COUNT_CORAL_NORMAL &&
                 //items["TREECORAL"].Count >= Constants.WEAPON_COUNT_CORAL_TREE &&
                 //items["NORMALFISH"].Count >= Constants.WEAPON_COUNT_FISH_NORMAL &&
                 //items["YELLOWFISH"].Count >= Constants.WEAPON_COUNT_FISH_YELLOW)
            {
                items["SILVER"].RemoveRange(0, Constants.WEAPON_COUNT_ORE_SILVER);
                //items["NORMALCORAL"].RemoveRange(0, Constants.WEAPON_COUNT_CORAL_NORMAL);
                //items["TREECORAL"].RemoveRange(0, Constants.WEAPON_COUNT_CORAL_TREE);
                //items["NORMALFISH"].RemoveRange(0, Constants.WEAPON_COUNT_FISH_NORMAL);
                //items["YELLOWFISH"].RemoveRange(0, Constants.WEAPON_COUNT_FISH_YELLOW);
                MessageBox.Show("Weapon crafted!");
                HasWeapon = true;
            }

            return HasWeapon;
        }

        public static bool CanCraftDivingHelmet(Dictionary<string, List<string>> items)
        {
            if (HasDivingHelmet) return false;

            if (items["GOLD"].Count >= Constants.DIVING_HELMET_COUNT_ORE_GOLD &&
                 items["IRON"].Count >= Constants.DIVING_HELMET_COUNT_ORE_IRON &&
                 items["SPIRALCORAL"].Count >= Constants.DIVING_HELMET_COUNT_CORAL_SPIRAL &&
                 items["TREECORAL"].Count >= Constants.DIVING_HELMET_COUNT_CORAL_TREE &&
                 items["NORMALFISH"].Count >= Constants.DIVING_HELMET_COUNT_FISH_NORMAL &&
                 items["YELLOWFISH"].Count >= Constants.DIVING_HELMET_COUNT_FISH_YELLOW)
            {
                items["GOLD"].RemoveRange(0, Constants.DIVING_HELMET_COUNT_ORE_GOLD);
                items["IRON"].RemoveRange(0, Constants.DIVING_HELMET_COUNT_ORE_IRON);
                items["SPIRALCORAL"].RemoveRange(0, Constants.DIVING_HELMET_COUNT_CORAL_SPIRAL);
                items["TREECORAL"].RemoveRange(0, Constants.DIVING_HELMET_COUNT_CORAL_TREE);
                items["NORMALFISH"].RemoveRange(0, Constants.DIVING_HELMET_COUNT_FISH_NORMAL);
                items["YELLOWFISH"].RemoveRange(0, Constants.DIVING_HELMET_COUNT_FISH_YELLOW);
                MessageBox.Show("Diving helmet crafted!");
                HasDivingHelmet = true;
            }
            return HasDivingHelmet;
        }

        public static bool CanCatchFish(Dictionary<string, List<string>> items)
        {
            if (CanFish) return false;

            if (items["IRON"].Count >= Constants.CATCH_FISH_COUNT_ORE_IRON &&
                 items["SILVER"].Count >= Constants.CATCH_FISH_COUNT_ORE_SILVER &&
                 items["NORMALCORAL"].Count >= Constants.CATCH_FISH_COUNT_CORAL_NORMAL &&
                 items["TREECORAL"].Count >= Constants.CATCH_FISH_COUNT_CORAL_TREE)
            {
                items["IRON"].RemoveRange(0, Constants.CATCH_FISH_COUNT_ORE_IRON);
                items["SILVER"].RemoveRange(0, Constants.CATCH_FISH_COUNT_ORE_SILVER);
                items["NORMALCORAL"].RemoveRange(0, Constants.CATCH_FISH_COUNT_CORAL_NORMAL);
                items["TREECORAL"].RemoveRange(0, Constants.CATCH_FISH_COUNT_CORAL_TREE);
                MessageBox.Show("You can catch fish!");
                CanFish = true;
            }
            return CanFish;
        }
    }
}
