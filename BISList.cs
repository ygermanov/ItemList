using System;

namespace ItemList
{
    [Serializable]
    internal class BISList
    {
        public int ID { get; set; }
        public string PlayerName { get; set; }
        public Items headSlot { get; set; }
        public Items neckSlot { get; set; }
        public Items shoulderSlot { get; set; }
        public Items backSlot { get; set; }
        public Items chestSlot { get; set; }
        public Items wristSlot { get; set; }
        public Items glovesSlot { get; set; }
        public Items beltSlot { get; set; }
        public Items leggsSlot { get; set; }
        public Items feetSlot { get; set; }
        public Items ringOneSlot { get; set; }
        public Items ringTwoSlot { get; set; }
        public Items trinketOneSlot { get; set; }
        public Items trinketTwoSlot { get; set; }
        public Items weaponSlot { get; set; }
        public Items offhandSlot { get; set; }
        public Items throwableSlot { get; set; }
        public int[] checkOwned { get; set; }
        public BISList()
        {
            PlayerName = "";
            headSlot = new Items("", "", "", "");
            neckSlot = new Items("", "", "", "");
            shoulderSlot = new Items("", "", "", "");
            backSlot = new Items("", "", "", "");
            chestSlot = new Items("", "", "", "");
            wristSlot = new Items("", "", "", "");
            glovesSlot = new Items("", "", "", "");
            beltSlot = new Items("", "", "", "");
            leggsSlot = new Items("", "", "", "");
            feetSlot = new Items("", "", "", "");
            ringOneSlot = new Items("", "", "", "");
            ringTwoSlot = new Items("", "", "", "");
            trinketOneSlot = new Items("", "", "", "");
            trinketTwoSlot = new Items("", "", "", "");
            weaponSlot = new Items("", "", "", "");
            offhandSlot = new Items("", "", "", "");
            throwableSlot = new Items("", "", "", "");
            checkOwned = new int[17];
        }

        #region getting item color for XAML Bindings
        public string HeadColor => headSlot.itemRarity;
        public string NeckColor => neckSlot.itemRarity;
        public string ShoulderColor => shoulderSlot.itemRarity;
        public string BackColor => backSlot.itemRarity;
        public string ChestColor => chestSlot.itemRarity;
        public string WristColor => wristSlot.itemRarity;
        public string GlovesColor => glovesSlot.itemRarity;
        public string BeltColor => beltSlot.itemRarity;
        public string LeggsColor => leggsSlot.itemRarity;
        public string FeetColor => feetSlot.itemRarity;
        public string RingOneColor => ringOneSlot.itemRarity;
        public string RingTwoColor => ringTwoSlot.itemRarity;
        public string TrinketOneColor => trinketOneSlot.itemRarity;
        public string TrinketTwoColor => trinketTwoSlot.itemRarity;
        public string WeaponColor => weaponSlot.itemRarity;
        public string OffHandColor => offhandSlot.itemRarity;
        public string ThrowableColor => throwableSlot.itemRarity;

        #endregion

        #region getting items names for XAML Bindings

        public string HeadName => headSlot.itemName;
        public string NeckName => neckSlot.itemName;
        public string ShoulderName => shoulderSlot.itemName;
        public string BackName => backSlot.itemName;
        public string ChestName => chestSlot.itemName;
        public string WristName => wristSlot.itemName;
        public string GlovesName => glovesSlot.itemName;
        public string BeltName => beltSlot.itemName;
        public string LeggsName => leggsSlot.itemName;
        public string FeetName => feetSlot.itemName;
        public string RingOneName => ringOneSlot.itemName;
        public string RingTwoName => ringTwoSlot.itemName;
        public string TrinketOneName => trinketOneSlot.itemName;
        public string TrinketTwoName => trinketTwoSlot.itemName;
        public string MainHandName => weaponSlot.itemName;
        public string OffHandName => offhandSlot.itemName;
        public string ThrowableName => throwableSlot.itemName;

        #endregion

        #region Image path for XAML Bindings

        public string HeadImage => headSlot.imageName;
        public string NeckImage => neckSlot.imageName;
        public string ShoulderImage => shoulderSlot.imageName;
        public string BackImage => backSlot.imageName;
        public string ChestImage => chestSlot.imageName;
        public string WristImage => wristSlot.imageName;
        public string GlovesImage => glovesSlot.imageName;
        public string BeltImage => beltSlot.imageName;
        public string LeggsImage => leggsSlot.imageName;
        public string FeetImage => feetSlot.imageName;
        public string RingOneImage => ringOneSlot.imageName;
        public string RingTwoImage => ringTwoSlot.imageName;
        public string TrinketOneImage => trinketOneSlot.imageName;
        public string TrinketTwoImage => trinketTwoSlot.imageName;
        public string MainHandImage => weaponSlot.imageName;
        public string OffHandImage => offhandSlot.imageName;
        public string ThrowableImage => throwableSlot.imageName;

        #endregion
    }
}
