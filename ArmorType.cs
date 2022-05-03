using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItemList
{
    public static class ArmorType 
    {
        private static readonly string[] armorType = new string[]
        {
            "Cloth",
            "Leather",
            "Mail",
            "Plate",
            "Miscellaneous"
        };

        private static readonly string[] bodyPart = new string[]
        {
            "Head",
            "Neck",
            "Shoulder",
            "Back",
            "Chest",
            "Wrist",
            "Gloves",
            "Belt",
            "Leggs",
            "Feet",
            "Ring",
            "Trinket",
        };

        public static string[] GetBody
        {
            get => bodyPart;
        }
        public static string GetArmorPart(int num)
        {
            return bodyPart[num];
        }

        public static string[] GetArmors
        {
            get => armorType;
        }
        public static string GetType(int x)
        {
            return armorType[x];
        }

        public static int getIndex(string armor)
        {
            return Array.IndexOf(armorType, armor);
        }

        public static event EventHandler GetArmorsChanged = delegate { };
    }
}
