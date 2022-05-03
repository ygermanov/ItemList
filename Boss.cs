using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItemList
{
    [Serializable]
    internal class Boss
    {
        public int ID { get; set; }
        public string bossName { get; set; }
        public string raidName { get; set; }
        public List<Items> dropableItems { get; set; }
    }
}
