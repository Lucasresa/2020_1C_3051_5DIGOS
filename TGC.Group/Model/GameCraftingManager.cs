using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TGC.Group.Model.Objects;
using TGC.Group.Utils;

namespace TGC.Group.Model
{
    static class GameCraftingManager
    {
        private struct Constants
        {
            public static int WEAPON_COUNT_ORE_SILVER = 5;
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
        public static bool HasCatchFish { get; set; }

        public static void CanCraftWeapon(Dictionary<string, List<string>> items)
        {
            if (HasWeapon)
                return;

            if ( items["SILVER"].Count > Constants.WEAPON_COUNT_ORE_SILVER &&
                 items["NORMALCORAL"].Count > Constants.WEAPON_COUNT_CORAL_NORMAL &&
                 items["TREECORAL"].Count > Constants.WEAPON_COUNT_CORAL_TREE &&
                 items["NORMALFISH"].Count > Constants.WEAPON_COUNT_FISH_NORMAL &&
                 items["YELLOWFISH"].Count > Constants.WEAPON_COUNT_FISH_YELLOW)
            {
                items["SILVER"].RemoveRange(0, Constants.WEAPON_COUNT_ORE_SILVER);
                items["NORMALCORAL"].RemoveRange(0, Constants.WEAPON_COUNT_CORAL_NORMAL);
                items["TREECORAL"].RemoveRange(0, Constants.WEAPON_COUNT_CORAL_TREE);
                items["NORMALFISH"].RemoveRange(0, Constants.WEAPON_COUNT_FISH_NORMAL);
                items["YELLOWFISH"].RemoveRange(0, Constants.WEAPON_COUNT_FISH_YELLOW);

                HasWeapon = true;
            }
        }

        public static void CanCraftDivingHelmet(Dictionary<string, List<string>> items)
        {
            if (HasDivingHelmet)
                return;

            if ( items["GOLD"].Count > Constants.DIVING_HELMET_COUNT_ORE_GOLD &&
                 items["IRON"].Count > Constants.DIVING_HELMET_COUNT_ORE_IRON &&
                 items["SPIRALCORAL"].Count > Constants.DIVING_HELMET_COUNT_CORAL_SPIRAL &&
                 items["TREECORAL"].Count > Constants.DIVING_HELMET_COUNT_CORAL_TREE &&
                 items["NORMALFISH"].Count > Constants.DIVING_HELMET_COUNT_FISH_NORMAL &&
                 items["YELLOWFISH"].Count > Constants.DIVING_HELMET_COUNT_FISH_YELLOW)
            {
                items["GOLD"].RemoveRange(0, Constants.DIVING_HELMET_COUNT_ORE_GOLD);
                items["IRON"].RemoveRange(0, Constants.DIVING_HELMET_COUNT_ORE_IRON);
                items["SPIRALCORAL"].RemoveRange(0, Constants.DIVING_HELMET_COUNT_CORAL_SPIRAL);
                items["TREECORAL"].RemoveRange(0, Constants.DIVING_HELMET_COUNT_CORAL_TREE);
                items["NORMALFISH"].RemoveRange(0, Constants.DIVING_HELMET_COUNT_FISH_NORMAL);
                items["YELLOWFISH"].RemoveRange(0, Constants.DIVING_HELMET_COUNT_FISH_YELLOW);

                HasDivingHelmet = true;
            }
        }

        public static void CanCatchFish(Dictionary<string, List<string>> items)
        {
            if (HasCatchFish)
                return;

            if ( items["IRON"].Count > Constants.CATCH_FISH_COUNT_ORE_IRON &&
                 items["SILVER"].Count > Constants.CATCH_FISH_COUNT_ORE_SILVER &&
                 items["NORMALCORAL"].Count > Constants.CATCH_FISH_COUNT_CORAL_NORMAL &&
                 items["TREECORAL"].Count > Constants.CATCH_FISH_COUNT_CORAL_TREE)
            {
                items["IRON"].RemoveRange(0, Constants.CATCH_FISH_COUNT_ORE_IRON);
                items["SILVER"].RemoveRange(0, Constants.CATCH_FISH_COUNT_ORE_SILVER);
                items["NORMALCORAL"].RemoveRange(0, Constants.CATCH_FISH_COUNT_CORAL_NORMAL);
                items["TREECORAL"].RemoveRange(0, Constants.CATCH_FISH_COUNT_CORAL_TREE);

                HasCatchFish = true;
            }
        }
    }
}
