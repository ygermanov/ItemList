using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItemList
{
    public static class WeaponType
    {
        private static readonly string[] weaponType = new string[]
        {
            "Sword",
            "Mace",
            "Dagger",
            "Staff",
            "Bow",
            "Crossbow",
            "Gun",
            "Thrown",
            "Wand",
            "Idol",
            "Libram",
            "Shield",
            "Totem",
            "Fist",
            "Axe",
            "Polearm",
            "Off-Hand"
        };


        private static readonly string[] weaponPart = new string[]
        {
            "Main Hand",
            "Off Hand",
            "Two Hand",
            "Throwable",
        };

        public static string[] GetParts
        {
            get => weaponPart;
        }
        public static string GetWeaponPart(int num)
        {
            return weaponPart[num];
        }
        public static string[] GetWeapons
        {
            get => weaponType;
        }
        public static string GetType(int x)
        {
            return weaponType[x];
        }

        public static int getIndex(string weapon)
        {
            return Array.IndexOf(weaponType, weapon);
        }

        public static event EventHandler GetWeaponsChanged = delegate { };
    }
}
