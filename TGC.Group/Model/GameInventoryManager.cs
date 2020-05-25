using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TGC.Group.Utils;

namespace TGC.Group.Model
{
    class GameInventoryManager
    {
        public Dictionary<string, List<string>> Items;
        public readonly List<string> CoralNormal = new List<string>();
        public readonly List<string> CoralSpiral = new List<string>();
        public readonly List<string> CoralTree = new List<string>();
        public readonly List<string> FishNormal = new List<string>();
        public readonly List<string> FishYellow = new List<string>();
        public readonly List<string> OreGold = new List<string>();
        public readonly List<string> OreIron = new List<string>();
        public readonly List<string> OreSilver = new List<string>();
        public readonly List<string> Rock = new List<string>();

        public GameInventoryManager()
        {
            Init();   
        }

        private void Init()
        {
            Items = new Dictionary<string, List<string>>
            {
                { "NORMALCORAL", CoralNormal },
                { "SPIRALCORAL", CoralSpiral },
                { "TREECORAL", CoralTree },
                { "NORMALFISH", FishNormal },
                { "YELLOWFISH", FishYellow },
                { "GOLD", OreGold },
                { "IRON", OreIron },
                { "SILVER", OreSilver },
                { "ROCK", Rock }
            };
        }

        public void AddItem(string itemSelected)
        {
            if ( itemSelected is null )
                return;

            var name = itemSelected.Substring(0, itemSelected.IndexOf('_'));
            Items[name].Add(itemSelected);
        }
    }
}
