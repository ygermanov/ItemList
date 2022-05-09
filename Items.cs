using System;

namespace ItemList
{
    [Serializable]
    internal class Items
    {
        
        public int ID { get; set; }
        public string itemName { get; set; }
        public string itemRarity { get; set; }
        public string bossName { get; set; }
        public string isWeapon { get; set; }
        public string itemType { get; set; }
        public string bodyPart { get; set; }
        public string imageName { get; set; }

        public Items(string name,
                     string boss,
                     string type,
                     string imagePath,
                     string body = "",
                     string rare = "",
                     string isW = "")
        {
            itemName = name;
            bodyPart = body;
            itemRarity = rare;
            bossName = boss;
            isWeapon = isW;
            itemType = type;
            imageName = imagePath;
        }
       
    }
}
