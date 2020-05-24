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
        public struct Items
        {
            public int ID;
            public string name;
        }

        public List<Items> ListItems { get; set; }

        public GameInventoryManager()
        {
            Init();   
        }

        private void Init()
        {
            ListItems = new List<Items>();
        }

        public void AddItem((int ID, string name) itemSelected)
        {
            if ( itemSelected.name is null )
                return;

            Items item;
            item.ID = itemSelected.ID;
            item.name = itemSelected.name;
            ListItems.Add(item);
        }
    }
}
