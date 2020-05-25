using System;
using System.Collections.Generic;
using System.Linq;
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

        public static bool CanCraftWeapon(ref List<string> items)
        {
            if ( FastUtils.CountOcurrences(items, "SILVER") > Constants.WEAPON_COUNT_ORE_SILVER &&
                 FastUtils.CountOcurrences(items, "NORMALCORAL") > Constants.WEAPON_COUNT_CORAL_NORMAL &&
                 FastUtils.CountOcurrences(items, "TREECORAL") > Constants.WEAPON_COUNT_CORAL_TREE &&
                 FastUtils.CountOcurrences(items, "NORMALFISH") > Constants.WEAPON_COUNT_FISH_NORMAL &&
                 FastUtils.CountOcurrences(items, "YELLOWFISH") > Constants.WEAPON_COUNT_FISH_YELLOW ) 
                return true;
            return false;
        }

        public static bool CanCraftDivingHelmet(ref List<string> items)
        {
            if ( FastUtils.CountOcurrences(items, "GOLD") > Constants.DIVING_HELMET_COUNT_ORE_GOLD &&
                 FastUtils.CountOcurrences(items, "IRON") > Constants.DIVING_HELMET_COUNT_ORE_IRON &&
                 FastUtils.CountOcurrences(items, "SPIRALCORAL") > Constants.DIVING_HELMET_COUNT_CORAL_SPIRAL &&
                 FastUtils.CountOcurrences(items, "TREECORAL") > Constants.DIVING_HELMET_COUNT_CORAL_TREE &&
                 FastUtils.CountOcurrences(items, "NORMALFISH") > Constants.DIVING_HELMET_COUNT_FISH_NORMAL &&
                 FastUtils.CountOcurrences(items, "YELLOWFISH") > Constants.DIVING_HELMET_COUNT_FISH_YELLOW )
                return true;
            return false;
        }

        public static bool CanCatchFish(ref List<string> items)
        {
            if (FastUtils.CountOcurrences(items, "IRON") > Constants.CATCH_FISH_COUNT_ORE_IRON &&
                 FastUtils.CountOcurrences(items, "SILVER") > Constants.CATCH_FISH_COUNT_ORE_SILVER &&
                 FastUtils.CountOcurrences(items, "NORMALCORAL") > Constants.CATCH_FISH_COUNT_CORAL_NORMAL &&
                 FastUtils.CountOcurrences(items, "TREECORAL") > Constants.CATCH_FISH_COUNT_CORAL_TREE)
            {
                return true;
            }
            return false;
        }
    }
}
