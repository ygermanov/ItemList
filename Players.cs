using System;
using System.Windows.Media;

namespace ItemList
{
    [Serializable]
    internal class Players
    {
        public int ID { get; set; }
        public string playerName { get; set; }
        public string playerClass { get; set; }
        public Brush playerColor { get; set; }

        public Players(string name, string playerclass)
        {
            playerName = name;
            playerClass = playerclass;
            playerColor = null;

        }

    }
}
