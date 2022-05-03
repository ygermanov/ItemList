using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ItemList
{

    public static class Classes
    {
        private static readonly string[] classNames = new string[]
        {
            "Warlock",
            "Paladin",
            "Priest",
            "Mage",
            "Warrior",
            "Shaman",
            "Hunter",
            "Rouge",
            "Druid"
        };

        private static readonly Brush[] classColor = new Brush[]
        {
            Brushes.MediumPurple,
            Brushes.LightPink,
            Brushes.White,
            Brushes.DeepSkyBlue,
            Brushes.SaddleBrown,
            Brushes.Blue,
            Brushes.Green,
            Brushes.Yellow,
            Brushes.Orange
        };

        #region get items
        public static string[] getClasses()
        {
            return classNames;
        }

        public static string ArmorTypeUsage(string character)
        {
            string type = character switch
            {
                "Warlock" or "Priest" or "Mage" => "Cloth",
                "Druid" or "Rouge" => "Leather",
                "Hunter" or "Shaman" => "Mail",
                "Warrior" or "Paladin" => "Plate",
                _ => null,
            };
            return type;
        }
        public static string takeClassName(int x)
        {
            return classNames[x];
        }

        public static Brush getClassColor(string name)
        {
            return classColor[Array.IndexOf(classNames, name)];
        }

        #endregion
    }
}
