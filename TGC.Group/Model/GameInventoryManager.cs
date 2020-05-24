using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Group.Utils;

namespace TGC.Group.Model
{
    class GameInventoryManager
    {
        private struct Items
        {
            public int id;
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

        public void AddItem(string nameItem)
        {
            Items item;
            var nameAndID = nameItem.Split('_');
            item.name = nameAndID[0];
            item.id = int.Parse(nameAndID[1]);
            ListItems.Add(item);
        }
    }
}
