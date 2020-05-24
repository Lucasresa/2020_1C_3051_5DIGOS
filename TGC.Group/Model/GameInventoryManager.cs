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
        private struct Items
        {
            public int ID;
            public string name;
        }

        private Ray Ray;
        private List<Items> ListItems;

        public GameInventoryManager(Ray ray)
        {
            Ray = ray;
            Init();   
        }

        private void Init()
        {
            ListItems = new List<Items>();
        }

        public void AddItem(int id, string name)
        {
            if (id == 0 && name is null)
                return;

            Items item;
            item.ID = id;
            item.name = name;
            ListItems.Add(item);
        }
    }
}
