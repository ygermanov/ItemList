using LiteDB;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ItemList
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<Players> selectedList = new ObservableCollection<Players>();
        private ObservableCollection<Items> selectedItem = new ObservableCollection<Items>();
        private Players playersDel;
        private readonly string dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\DataBase\";//Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + @"\DataBase\";
        private readonly string imageDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\Images\";//Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + @"\Images\";
        
        bool new_Entry = true;
        int updateID = -1;
        bool openUpdate = false;
        private bool bisListUpdate = false;
        private bool dataBaseEdit = false;
        private bool isImageLoaded = false;
        private static readonly Dictionary<string, string> BISListDictionary = new Dictionary<string, string>()
        {
            { "HeadUpdate", "headSlot" },
            { "NeckUpdate","neckSlot" },
            { "ShoulderUpdate","shoulderSlot" },
            { "BackUpdate","backSlot" },
            { "ChestUpdate","chestSlot" },
            { "WristUpdate","wristSlot" },
            { "GlovesUpdate","glovesSlot" },
            { "BeltUpdate","beltSlot" },
            { "LeggsUpdate","leggsSlot" },
            { "FeetUpdate","feetSlot" },
            { "RingOneUpdate","ringOneSlot" },
            { "RingTwoUpdate","ringTwoSlot" },
            { "TrinketOneUpdate","trinketOneSlot" },
            { "TrinketTwoUpdate","trinketTwoSlot" },
            { "MainHandUpdate","weaponSlot" },
            { "OffHandUpdate","offhandSlot" },
            { "ThrowableUpdate","throwableSlot" }
        };
        private static readonly Dictionary<string, string> ItemListDictionary = new Dictionary<string, string>()
        {
            { "HeadUpdate", "Head" },
            { "NeckUpdate","Neck" },
            { "ShoulderUpdate","Shoulder" },
            { "BackUpdate","Back" },
            { "ChestUpdate","Chest" },
            { "WristUpdate","Wrist" },
            { "GlovesUpdate","Gloves" },
            { "BeltUpdate","Belt" },
            { "LeggsUpdate","Leggs" },
            { "FeetUpdate","Feet" },
            { "RingOneUpdate","Ring" },
            { "RingTwoUpdate","Ring" },
            { "TrinketOneUpdate","Trinket" },
            { "TrinketTwoUpdate","Trinket" },
            { "MainHandUpdate","Main Hand" },
            { "OffHandUpdate","Off Hand" },
            { "ThrowableUpdate","Throwable" }
        };
        private static readonly Dictionary<string, int> itemSlotNumber = new Dictionary<string, int>()
        {
            { "Head", 0 },
            { "Neck", 1 },
            { "Shoulder", 2 },
            { "Back", 3 },
            { "Chest", 4 },
            { "Wrist", 5 },
            { "Gloves", 6 },
            { "Belt", 9 },
            { "Leggs", 10 },
            { "Feet", 11 },
            { "Ring", 12 },
            { "Trinket", 14 },
            { "Main Hand", 7 },
            { "Off Hand", 8 },
            { "Throwable", 16 }
        };
        private class playerData
        {
            public string playerName { get; set; }
            public Brush classColor { get; set; }
            public string owned { get; set; }
            public playerData(string name, Brush color, int temp)
            {
                playerName = name;
                classColor = color;
                owned = temp switch
                {
                    1 => "Yes",
                    0 => "No",
                    _ => ""
                };
            }
        };
        
        public MainWindow()
        {
            InitializeComponent();
            LoadDBInfo();
            LoadRaidDropDownMenu();
            LoadPlayerInfo();
            LoadClassValue();
            view.Items.Add(new BISList());
            LoadAddItems();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public static string GetParameterName2<T>(T item) where T : class
        {
            if (item == null)
                return string.Empty;

            return typeof(T).GetProperties()[0].Name;
        }
        public string ConvertRarityToColor(string rarity)
        {
            return rarity switch
            {
                "Common" => "Green",
                "Rare" => "Blue",
                "Epic" => "Purple",
                "Legendary" => "Orange",
                _ => "Black"
            };
        }
        public string ConvertColorToRarity(string color)
        {
            return color switch
            {
                "Green" => "Common",
                "Blue" => "BlueRare",
                "Purple" => "Epic",
                "Orange" => "Legendary",
                _ => "Show All"
            };
        }


        #region Players Editing button functions

        public void New_Entry(object sender, RoutedEventArgs e)
        {
            ClearAddPlayers();
        }
        public void Update(object sender, RoutedEventArgs e)
        {
            Players plays = new(txtName.Text, txtClass.Text);
            if (new_Entry)
            {
                using (var db = new LiteRepository(dir + DataBaseNames.PlayersDataBase))
                {
                    db.Insert(plays);
                    plays.playerColor = Classes.getClassColor(plays.playerClass);
                    ItemList.Items.Add(plays);
                }
            }
            else
            {
                using (var db = new LiteDatabase(dir + DataBaseNames.PlayersDataBase))
                {
                    var collection = db.GetCollection<Players>("players");
                    var item = collection.FindById(updateID);
                    item.playerName = txtName.Text;
                    item.playerClass = txtClass.Text;
                    collection.Update(item);
                    ItemList.Items.Remove(ItemList.SelectedItem);
                    item.playerColor = Classes.getClassColor(item.playerClass);
                    ItemList.Items.Add(item);
                }

            }

            LoadDBInfo();
            ClearAddPlayers();
        }
        public void btn_Clear(object sender, RoutedEventArgs e)
        {
            ClearAddItem();
            
        }
        public void btn_Create(object sender, RoutedEventArgs e)
        {
            if (nameOfItem.Text == ""
                    || nameOfRarity.Text == ""
                    || nameOfBoss.Text == ""
                    || nameOfType.Text == ""
                    || nameOfPart.Text == ""
                    || checkIfWeapon.Text == "")
            {

                MessageBox.Show("Fill all fields before creating an Item !!", "Empty field detected", MessageBoxButton.OK);
                return;
            }
            using (var db = new LiteDatabase(dir + DataBaseNames.ItemDataBase))
            {
                
                var collection = db.GetCollection<Items>("item");
                string imagePath = "images/" + nameOfRaid.Text + "/" + nameOfBoss.Text + "/" + nameOfItem.Text + ".png";
                var item = new Items(nameOfItem.Text, nameOfBoss.Text, nameOfType.Text, imagePath, nameOfPart.Text, ConvertRarityToColor(nameOfRarity.Text), checkIfWeapon.Text);
                collection.Insert(item);
            }
            if (isImageLoaded)
            {

                BitmapEncoder image = new PngBitmapEncoder();
                image.Frames.Add(BitmapFrame.Create((BitmapImage)showImage.Source));
                using (var fileStream = new FileStream(imageDir + nameOfRaid.Text + @"\" + nameOfBoss.Text + @"\" + nameOfItem.Text + @".png", FileMode.Create))
                {
                    
                    image.Save(fileStream);
                }
            }
            ClearAddItem();
        }

        public void Browse_Image(object sender, RoutedEventArgs e)
        {
            OpenFileDialog open_dialog = new OpenFileDialog
            {
                Filter = "Image Files (*.jpeg; *.png; *.bmp)|*.jpg; *.png; *.bmp"
            };

            if (open_dialog.ShowDialog() == true)
            {
                showImage.Source = new BitmapImage(new Uri(open_dialog.FileName));
                isImageLoaded = true;

            }
        }
        public void DeletePlayer(object sender, RoutedEventArgs e)
        {
            using (var db = new LiteDatabase(dir + DataBaseNames.PlayersDataBase))
            {
                var collection = db.GetCollection<Players>("players");
                var id = collection.Query()
                        .Where(x => x.playerName == playersDel.playerName)
                        .Select(x => new { x.ID })
                        .FirstOrDefault();
                collection.Delete(id.ID);
            }
            ItemList.Items.Remove(ItemList.SelectedItem);
            playersDel = null;
            ClearAddPlayers();
        }
        public void DeleteItem(object sender, RoutedEventArgs e)
        {
            foreach(var item in selectedItem)
            {
                using(var db = new LiteDatabase(dir + DataBaseNames.ItemDataBase))
                {
                    var collection = db.GetCollection<Items>("item");
                    var temp = collection.Query()
                        .Where(x => x.itemName == item.itemName)
                        .Select(x => new { x.ID })
                        .FirstOrDefault(); 
                    collection.Delete(temp.ID);
                    delateItem.IsEnabled = false;
                }
            }
        }
        public void UpdateBISList(object sender, RoutedEventArgs e)
        {
            if(!bisListUpdate)
            {
                openUpdate = true;
                Players selectedPlayer = new Players("","");
                int classType = 0;
                using (var db = new LiteDatabase(dir + DataBaseNames.PlayersDataBase))
                {
                    selectedPlayer = db.GetCollection<Players>("players")
                                        .Query()
                                        .Where(x => x.playerName == playerBIS.Text)
                                        .FirstOrDefault();
                    classType = Classes.ArmorTypeUsage(selectedPlayer.playerClass) switch
                    {
                        "Cloth" => 1,
                        "Leather" => 2,
                        "Mail" => 3,
                        "Plate" => 4,
                        _ => 5
                    };
                }
                using (var db = new LiteDatabase(dir + DataBaseNames.ItemDataBase))
                {
                    int itemClassType = -1;
                    string tempText;
                    foreach(ComboBox t in rollOneOfBISItems.Children)
                    {
                        tempText = t.Text;
                        var collection = db.GetCollection<Items>("item");
                        string name = ItemListDictionary[(t as FrameworkElement).Name];
                        if(name == "Main Hand")
                        {
                            var p = collection
                            .Find(x => x.bodyPart == name || x.bodyPart == "Two Hand");
                            foreach (var item in p)
                            {
                                if (tempText == item.itemName) continue;
                                t.Items.Add(new ComboBoxItem { Content = item.itemName });
                                if (item != null) ((ComboBoxItem)t.Items[^1]).Foreground = (Brush)new BrushConverter().ConvertFromString(item.itemRarity);
                            }
                        }
                        else
                        {
                            var p = collection
                            .Find(x => x.bodyPart == name);
                            foreach (var item in p)
                            {
                                itemClassType = item.itemType switch
                                {
                                    "Miscellaneous" => 0,
                                    "Cloth" => 1,
                                    "Leather" => 2,
                                    "Mail" => 3,
                                    "Plate" => 4,
                                    _ => -1
                                };
                                if (tempText == item.itemName) continue;
                                if (itemClassType > classType)
                                {
                                    continue;
                                }
                                t.Items.Add(new ComboBoxItem { Content = item.itemName });
                                if (item != null) ((ComboBoxItem)t.Items[^1]).Foreground = (Brush)new BrushConverter().ConvertFromString(item.itemRarity);
                            }
                        }
                        t.Text = tempText;
                        
                    }
                    foreach (ComboBox t in rollTwoOfBISItems.Children)
                    {
                        tempText = t.Text;
                        var collection = db.GetCollection<Items>("item");
                        string name = ItemListDictionary[(t as FrameworkElement).Name];
                        var p = collection
                            .Find(x => x.bodyPart == name);

                        foreach (var item in p)
                        {
                            itemClassType = item.itemType switch
                            {
                                "Miscellaneous" => 0,
                                "Cloth" => 1,
                                "Leather" => 2,
                                "Mail" => 3,
                                "Plate" => 4,
                                _ => -1
                            };
                            if (tempText == item.itemName) continue;
                            if (itemClassType > classType)
                            {
                                continue;
                            }
                            t.Items.Add(new ComboBoxItem { Content = item.itemName });
                            if (item != null) ((ComboBoxItem)t.Items[^1]).Foreground = (Brush)new BrushConverter().ConvertFromString(item.itemRarity);
                        }
                        t.Text = tempText;
                    }

                }
                unlockList.Content = "Save";
                cancelList.Visibility = Visibility.Visible;
                checkBoxItemsOwnedOne.IsEnabled = true;
                checkBoxItemsOwnedTwo.IsEnabled = true;
                rollOneOfBISItems.IsEnabled = true;
                rollTwoOfBISItems.IsEnabled = true;
                bisListUpdate = true;
                openUpdate = false;
                return;
            }
            using(var db = new LiteDatabase(dir + DataBaseNames.BISListDataBase))
            {
                int checkSum = 0;
                var collection = db.GetCollection<BISList>("bis");
                var item = collection
                    .Query()
                    .Where(x => x.PlayerName == playerBIS.Text)
                    .FirstOrDefault();
                bool newOrNot = false;
                if (item == null)
                {
                    newOrNot = true;
                    item = new BISList();
                    item.PlayerName = playerBIS.Text;
                }
                foreach (CheckBox t in checkBoxItemsOwnedOne.Children)
                {
                    if (t.IsChecked == true)
                    {
                        item.checkOwned[checkSum] = 1;
                    }
                    else
                    {
                        item.checkOwned[checkSum] = 0;
                    }
                    checkSum++;
                }
                foreach (CheckBox t in checkBoxItemsOwnedTwo.Children)
                {
                    if (t.IsChecked == true)
                    {
                        item.checkOwned[checkSum] = 1;
                    }
                    else
                    {
                        item.checkOwned[checkSum] = 0;
                    }

                    checkSum++;
                }
                using (var itemDB = new LiteDatabase(dir + DataBaseNames.ItemDataBase))
                {
                    var itemCollection = itemDB.GetCollection<Items>("item");
                    foreach (ComboBox t in rollOneOfBISItems.Children)
                    {
                        var tempItem = itemCollection.Find(x => x.itemName == t.Text).FirstOrDefault();
                        var propName = item.GetType().GetProperty(BISListDictionary[((t as FrameworkElement).Name)]);
                        if (tempItem == null) tempItem = new Items("","","","");
                        //else t.Foreground = (Brush)new BrushConverter().ConvertFromString(tempItem.itemRarity);
                        propName.SetValue(item, tempItem, null);
                    }
                    foreach (ComboBox t in rollTwoOfBISItems.Children)
                    {
                        var tempItem = itemCollection.Find(x => x.itemName == t.Text).FirstOrDefault();
                        var propName = item.GetType().GetProperty(BISListDictionary[((t as FrameworkElement).Name)]);
                        if (tempItem == null) tempItem = new Items("", "", "", "");
                        //else t.Foreground = (Brush)new BrushConverter().ConvertFromString(tempItem.itemRarity);
                        propName.SetValue(item, tempItem, null);
                    }
                    if (newOrNot)
                        collection.Insert(item);
                    else
                        collection.Update(item);
                }
                
            }
            unlockList.Content = "Unlock";
            cancelList.Visibility = Visibility.Collapsed;
            checkBoxItemsOwnedOne.IsEnabled = false;
            checkBoxItemsOwnedTwo.IsEnabled = false;
            rollOneOfBISItems.IsEnabled = false;
            rollTwoOfBISItems.IsEnabled = false;
            bisListUpdate = false;
            
        }
        public void CancelBISList(object sender, RoutedEventArgs e)
        {
            unlockList.Content = "Unlock";
            cancelList.Visibility = Visibility.Collapsed;
            checkBoxItemsOwnedOne.IsEnabled = false;
            checkBoxItemsOwnedTwo.IsEnabled = false;
            rollOneOfBISItems.IsEnabled = false;
            rollTwoOfBISItems.IsEnabled = false;
            bisListUpdate = false;
            LoadBISList(playerBIS.Text);
        }

        #endregion

        #region clear functions

        public void ClearTables()
        {
            WarlockList.Items.Clear();
            PaladinList.Items.Clear();
            PriestList.Items.Clear();
            MageList.Items.Clear();
            WarriorList.Items.Clear();
            ShamanList.Items.Clear();
            HunterList.Items.Clear();
            RougeList.Items.Clear();
            DruidList.Items.Clear();
        }
        public void ClearAddPlayers()
        {
            txtClass.Text = "";
            txtName.Text = "";
            btnUpdate.Content = "Create";
            new_Entry = true;
            updateID = -1;
        }
        public void ClearAddItem()
        {
            dataBaseEdit = true;
            nameOfItem.Text = "";
            nameOfRarity.Text = "";
            nameOfRaid.Text = "";
            nameOfBoss.Text = "";
            nameOfType.Text = "";
            nameOfPart.Text = "";
            checkIfWeapon.Text = "";
            showImage.Source = null;
            isImageLoaded = false;
            nameOfPart.IsEnabled = false;
            nameOfType.IsEnabled = false;
            LoadAddItems();
        }
        #endregion

        #region Load Database
        public void LoadRaidDropDownMenu()
        {
            using (var db = new LiteDatabase(dir + DataBaseNames.RaidDataBase))
            {
                raid.Items.Clear();
                boss.Items.Clear();
                boss.IsEnabled = false;
                ItemForAsignment.Items.Clear();
                ItemForAsignment.IsEnabled = false;
                var collection = db.GetCollection<Raid>("raid");
                var item = collection.FindAll();
                foreach (var temp in item)
                {
                    raid.Items.Add(temp.raidName);
                }
            }
            rarityOfItem.SelectedIndex = 4;
            dataBaseEdit = false;
        }
        public void LoadDBInfo()
        {
            ClearTables();
            using (var db = new LiteDatabase(dir + DataBaseNames.PlayersDataBase))
            {
                var collection = db.GetCollection<Players>("players");

                foreach (var player in collection.FindAll())
                {
                    switch (player.playerClass)
                    {
                        case "Warlock":
                            player.playerColor = Classes.getClassColor(player.playerClass);
                            WarlockList.Items.Add(player);
                            break;
                        case "Paladin":
                            player.playerColor = Classes.getClassColor(player.playerClass);
                            PaladinList.Items.Add(player);
                            break;
                        case "Priest":
                            player.playerColor = Classes.getClassColor(player.playerClass);
                            PriestList.Items.Add(player);
                            break;
                        case "Mage":
                            player.playerColor = Classes.getClassColor(player.playerClass);
                            MageList.Items.Add(player);
                            break;
                        case "Warrior":
                            player.playerColor = Classes.getClassColor(player.playerClass);
                            WarriorList.Items.Add(player);
                            break;
                        case "Shaman":
                            player.playerColor = Classes.getClassColor(player.playerClass);
                            ShamanList.Items.Add(player);
                            break;
                        case "Hunter":
                            player.playerColor = Classes.getClassColor(player.playerClass);
                            HunterList.Items.Add(player);
                            break;
                        case "Rouge":
                            player.playerColor = Classes.getClassColor(player.playerClass);
                            RougeList.Items.Add(player);
                            break;
                        case "Druid":
                            player.playerColor = Classes.getClassColor(player.playerClass);
                            DruidList.Items.Add(player);
                            break;
                        default:
                            break;
                    }
                }
            }

            

        }

        public void LoadPlayerInfo()
        {
            using (var db = new LiteDatabase(dir + DataBaseNames.PlayersDataBase))
            {
                var collection = db.GetCollection<Players>("players");
                foreach (var play in collection.FindAll().OrderBy(x => x.playerClass))
                {
                    play.playerColor = Classes.getClassColor(play.playerClass);
                    ItemList.Items.Add(play);
                }
            }
        }

        public void LoadClassValue()
        {
            txtClass.Items.Add("Warlock");
            txtClass.Items.Add("Paladin");
            txtClass.Items.Add("Priest");
            txtClass.Items.Add("Mage");
            txtClass.Items.Add("Warrior");
            txtClass.Items.Add("Shaman");
            txtClass.Items.Add("Hunter");
            txtClass.Items.Add("Rouge");
            txtClass.Items.Add("Druid");

        }

        public void LoadAddItems()
        {

            using (var db = new LiteDatabase(dir + DataBaseNames.RaidDataBase))
            {
                nameOfRaid.Items.Clear();
                nameOfBoss.Items.Clear();
                nameOfBoss.IsEnabled = false;
                var collection = db.GetCollection<Raid>("raid");
                var item = collection.FindAll();
                foreach (var temp in item)
                {
                    nameOfRaid.Items.Add(temp.raidName);
                }
            }
            dataBaseEdit = false;
        }

        public void LoadItems()
        {

            if (itemPart.Text == "" || itemType.Text == "") return;
            ItemShowGrid.Items.Clear();
            using (var db = new LiteDatabase(dir + DataBaseNames.ItemDataBase))
            {
                string weapon = typeOfItem.Text == "Weapon" ? "Yes" : "No";
                var collection = db.GetCollection<Items>("item");
                if (rarityOfItem.Text == "Show All")
                {
                    var items = collection.Query()
                    .Where(x => x.isWeapon == weapon
                        && x.itemType == itemType.Text
                        && x.bodyPart == itemPart.Text);
                    //.Select(x => new { x.itemName, x.itemType, x.bossName, x.imageName });
                    foreach (var item in items.ToArray())
                    {
                        ItemShowGrid.Items.Add(item);
                    }
                }
                else
                {
                    var items = collection.Query()
                    .Where(x => x.isWeapon == weapon
                        && x.itemRarity == ConvertRarityToColor(rarityOfItem.Text) 
                        && x.itemType == itemType.Text
                        && x.bodyPart == itemPart.Text);
                    //.Select(x => new { x.itemName, x.itemType, x.bossName, x.imageName });
                    foreach (var item in items.ToArray())
                    {
                        ItemShowGrid.Items.Add(item);
                    }
                }

            }
        }
        public void LoadBISList(string playerName)
        {
            using(var db = new LiteDatabase(dir + DataBaseNames.BISListDataBase))
            {
                int checkSum = 0;
                var collection = db.GetCollection<BISList>("bis")
                    .Query()
                    .Where(x => x.PlayerName == playerName)
                    .FirstOrDefault();
                if (collection == null)
                {
                    foreach (CheckBox t in checkBoxItemsOwnedOne.Children)
                    {
                        t.IsChecked = false;
                    }
                    foreach (CheckBox t in checkBoxItemsOwnedTwo.Children)
                    {
                        t.IsChecked = false;
                    }
                    foreach (ComboBox t in rollOneOfBISItems.Children)
                    {
                        t.Items.Clear();
                        t.Text = "";
                    }
                    foreach (ComboBox t in rollTwoOfBISItems.Children)
                    {
                        t.Items.Clear();
                        t.Text = "";
                    }
                    return;
                }
                foreach (CheckBox t in checkBoxItemsOwnedOne.Children)
                {
                    if(collection.checkOwned[checkSum] == 0)
                    {
                        t.IsChecked = false;
                    }
                    else
                    {
                        t.IsChecked = true;
                    }

                    checkSum++;
                }
                foreach (CheckBox t in checkBoxItemsOwnedTwo.Children)
                {
                    if (collection.checkOwned[checkSum] == 0)
                    {
                        t.IsChecked = false;
                    }
                    else
                    {
                        t.IsChecked = true;
                    }

                    checkSum++;
                }
                foreach (ComboBox t in rollOneOfBISItems.Children)
                {
                    t.Items.Clear();
                    t.SelectedIndex = 0;
                    string name = BISListDictionary[(t as FrameworkElement).Name];
                    Items item = (Items)collection.GetType().GetProperty(name).GetValue(collection, null);
                    t.Items.Add(new ComboBoxItem { Content = item.itemName });
                    if (item.itemRarity != null)
                    {
                        t.Foreground = (Brush)new BrushConverter().ConvertFromString(item.itemRarity);
                        ((ComboBoxItem)t.Items[^1]).Foreground = (Brush)new BrushConverter().ConvertFromString(item.itemRarity);
                        
                    }

                }
                foreach (ComboBox t in rollTwoOfBISItems.Children)
                {
                    t.Items.Clear();
                    t.SelectedIndex = 0;
                    string name = BISListDictionary[(t as FrameworkElement).Name];
                    Items item = (Items)collection.GetType().GetProperty(name).GetValue(collection, null);
                    t.Items.Add(new ComboBoxItem { Content = item.itemName });
                    if (item.itemRarity != null)
                    {
                        t.Foreground = (Brush)new BrushConverter().ConvertFromString(item.itemRarity);
                        ((ComboBoxItem)t.Items[^1]).Foreground = (Brush)new BrushConverter().ConvertFromString(item.itemRarity);
                        
                    }
                }
            }

        }

        #endregion

        #region Menu Items

        public void Add_Raid(object sender, RoutedEventArgs e)
        {
            Input_Dialog inputDialog = new Input_Dialog("Enter Raid name:", "");
            if (inputDialog.ShowDialog() == true)
            {
                using (var db = new LiteDatabase(dir + DataBaseNames.RaidDataBase))
                {
                    var collection = db.GetCollection<Raid>("raid");
                    Raid item = new Raid
                    {
                        raidName = inputDialog.Answer
                    };
                    collection.Insert(item);
                };
            }
            dataBaseEdit = true;
            LoadRaidDropDownMenu();


        }

        public void Add_Boss(object sender, RoutedEventArgs e)
        {
            string[] raidList = null;
            using (var db = new LiteDatabase(dir + DataBaseNames.RaidDataBase))
            {
                
                var collection = db.GetCollection<Raid>("raid");
                List<string> temp = new List<string>();
                foreach (var item in collection.FindAll())
                {
                    temp.Add(item.raidName);

                }
                raidList = temp.ToArray();
            }
            Input_Dialog inputDialog = new Input_Dialog("Enter Boss name:", "", "Choose Raid:", 1, raidList);
            if (inputDialog.ShowDialog() == true)
            {
                using (var db = new LiteDatabase(dir + DataBaseNames.BossDataBase))
                {
                    var collection = db.GetCollection<Boss>("boss");
                    Boss item = new Boss
                    {
                        bossName = inputDialog.Answer,
                        raidName = inputDialog.Choice,
                        dropableItems = new List<Items>()
                    };
                    string folderPath1 = imageDir + inputDialog.Choice;
                    string folderPath = imageDir + inputDialog.Choice + @"\" + inputDialog.Answer;
                    Directory.CreateDirectory(folderPath1);
                    Directory.CreateDirectory(folderPath);
                    collection.Insert(item);
                };
            }
            dataBaseEdit = true;
            LoadRaidDropDownMenu();
        }

        public void Remove_Boss(object sender, RoutedEventArgs e)
        {
            string[] raidList = null;
            using (var db = new LiteDatabase(dir + DataBaseNames.BossDataBase))
            {
                var collection = db.GetCollection<Boss>("boss");
                List<string> temp = new List<string>();
                foreach (var item in collection.FindAll())
                {
                    temp.Add(item.bossName);

                }
                raidList = temp.ToArray();
                Input_Dialog inputDialog = new Input_Dialog("", "", "Choose Boss to Delete:", 3, raidList);
                if (inputDialog.ShowDialog() == true)
                {
                    var id = collection.Query()
                        .Where(x => x.bossName == inputDialog.Boss)
                        .Select(x => new { x.ID })
                        .FirstOrDefault();
                    collection.Delete(id.ID);
                }
            }
            dataBaseEdit = true;
            LoadRaidDropDownMenu();
        }
        public void Remove_Raid(object sender, RoutedEventArgs e)
        {
            string[] raidList = null;
            using (var db = new LiteDatabase(dir + DataBaseNames.RaidDataBase))
            {
                var collection = db.GetCollection<Raid>("raid");
                List<string> temp = new List<string>();
                foreach (var item in collection.FindAll())
                {
                    temp.Add(item.raidName);

                }
                raidList = temp.ToArray();
                Input_Dialog inputDialog = new Input_Dialog("", "", "Choose Raid to Delete:", 2, raidList);
                if (inputDialog.ShowDialog() == true)
                {
                    var id = collection.Query()
                        .Where(x => x.raidName == inputDialog.Raid)
                        .Select(x => new { x.ID })
                        .FirstOrDefault();
                    collection.Delete(id.ID);
                }
            }
            dataBaseEdit = true;
            LoadRaidDropDownMenu();
        }

        public void Rename_Raid(object sender, RoutedEventArgs e)
        {
            string[] raidList = null;
            using (var db = new LiteDatabase(dir + DataBaseNames.RaidDataBase))
            {
                var collection = db.GetCollection<Raid>("raid");
                List<string> temp = new List<string>();
                foreach (var item in collection.FindAll())
                {
                    temp.Add(item.raidName);

                }
                raidList = temp.ToArray();
                Input_Dialog inputDialog = new Input_Dialog("Enter the new Raid name :", "", "Choose Raid to rename :", 4, raidList);
                if (inputDialog.ShowDialog() == true)
                {
                    var id = collection.Query()
                        .Where(x => x.raidName == inputDialog.Raid)
                        .Select(x => new { x.ID })
                        .FirstOrDefault();
                    var item = collection.FindById(id.ID);
                    item.raidName = inputDialog.Answer;
                    collection.Update(item);
                    using (var bs = new LiteDatabase(dir + DataBaseNames.BossDataBase))
                    {
                        var bossCollection = bs.GetCollection<Boss>("boss");
                        foreach(var bossItems in bossCollection.FindAll().Where(x => x.raidName == inputDialog.Raid))
                        {
                            bossItems.raidName = inputDialog.Answer;
                            bossCollection.Update(bossItems);
                        }
                    }
                }
            }
            dataBaseEdit = true;
            LoadRaidDropDownMenu();
        }
        public void Rename_Boss(object sender, RoutedEventArgs e)
        {
            string[] raidList = null;
            using (var db = new LiteDatabase(dir + DataBaseNames.BossDataBase))
            {
                var collection = db.GetCollection<Boss>("boss");
                List<string> temp = new List<string>();
                foreach (var item in collection.FindAll())
                {
                    temp.Add(item.bossName);

                }
                raidList = temp.ToArray();
                Input_Dialog inputDialog = new Input_Dialog("Enter the new Boss name :", "", "Choose Boss to rename :", 54, raidList);
                if (inputDialog.ShowDialog() == true)
                {
                    var id = collection.Query()
                        .Where(x => x.bossName == inputDialog.Boss)
                        .Select(x => new { x.ID })
                        .FirstOrDefault();
                    var item = collection.FindById(id.ID);
                    item.bossName = inputDialog.Answer;
                    collection.Update(item);
                }
            }
            dataBaseEdit = true;
            LoadRaidDropDownMenu();
        }

        public void Exit_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        #endregion

        #region Selection change

        public void Player_SelectionChange(object sender, SelectionChangedEventArgs e)
        {
            selectedList.Clear();

            var listBox = (ListBox)e.Source;
            if (listBox.SelectedItem is Players)
            {
                
                selectedList.Add((Players)listBox.SelectedItem);
                playersDel = (Players)listBox.SelectedItem;

                
                foreach (var item in selectedList)
                {
                    using (var db = new LiteDatabase(dir + DataBaseNames.BISListDataBase))
                    {
                        int checkSum = 0;
                        view.Items.Clear();
                        var collection = db.GetCollection<BISList>("bis");
                        var itemSelected = collection.Find(x => x.PlayerName == item.playerName).FirstOrDefault();
                        if (itemSelected != null)
                        {
                            bool check = false;
                            foreach (CheckBox t in ownedItems.Children) // 7 8 9 - 15 16 17
                            {
                                if (checkSum == 16) check = true;
                                checkSum = checkSum switch
                                {
                                    7 => 9,
                                    16 => 7,
                                    _ => checkSum

                                };
                                if (check && checkSum == 9) checkSum = 16;
                                if (itemSelected.checkOwned[checkSum] == 1)
                                {
                                    t.IsChecked = true;
                                }
                                else
                                {
                                    t.IsChecked = false;
                                }
                                checkSum++;
                            }

                        }
                        view.Items.Add(itemSelected);
                    }
                    
                    classPlayer.Text = item.playerName;
                    classPlayer.Background = Classes.getClassColor(item.playerClass);
                    playerBIS.Text = item.playerName;
                    LoadBISList(playerBIS.Text);
                    playerBIS.Background = Classes.getClassColor(item.playerClass);


                }
            }
            listBox.UnselectAll();
        }

        private void ItemList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedList.Clear();
            if (ItemList.SelectedItem is Players)
            {
                selectedList.Add(((Players)this.ItemList.SelectedItem));
                playersDel = (Players)this.ItemList.SelectedItem;
            }
            foreach (var item in selectedList)
            {
                txtName.Text = item.playerName;
                txtClass.Text = item.playerClass;
                updateID = item.ID;
            }
            btnUpdate.Content = "Update";
            new_Entry = false;

        }

        private void ItemAddRaid_SelectionChange(object sender, SelectionChangedEventArgs e)
        {
            if (dataBaseEdit) return;

            if (!nameOfBoss.IsEnabled)
            {
                nameOfBoss.IsEnabled = true;
            }
            else
            {
                nameOfBoss.Items.Clear();
            }
            var raidFrame = (ComboBox)e.Source;

            using (var db = new LiteDatabase(dir + DataBaseNames.BossDataBase))
            {
                var collection = db.GetCollection<Boss>("boss");
                var item = collection.FindAll()
                    .Where(x => x.raidName == raidFrame.SelectedItem.ToString());
                foreach (var temp in item)
                {
                    nameOfBoss.Items.Add(temp.bossName);

                }
            }
        }
        private void ItemAddIsWeapon_SelectionChange(object sender, SelectionChangedEventArgs e)
        {
            nameOfType.Items.Clear();
            nameOfPart.Items.Clear();
            var raidFrame = (ComboBox)e.Source;
            switch (raidFrame.SelectedIndex)
            {
                case 0:
                    nameOfType.IsEnabled = true;
                    nameOfPart.IsEnabled = true;
                    foreach (var item in WeaponType.GetWeapons)
                    {
                        nameOfType.Items.Add(item);
                    }
                    foreach(var item in WeaponType.GetParts)
                    {
                        nameOfPart.Items.Add(item);
                    }
                    break;
                case 1:
                    nameOfType.IsEnabled = true;
                    nameOfPart.IsEnabled = true;
                    foreach (var item in ArmorType.GetArmors)
                    {
                        nameOfType.Items.Add(item);
                    }
                    foreach (var item in ArmorType.GetBody)
                    {
                        nameOfPart.Items.Add(item);
                    }
                    break;
                default:
                    break;
            }

            
        }
        private void raidSelect_SelectionChange(object sender, SelectionChangedEventArgs e)
        {
            if (dataBaseEdit) return;

            if (!boss.IsEnabled)
            {
                boss.IsEnabled = true;
            }
            else
            {
                boss.Items.Clear();
            }

            var raidFrame = (ComboBox)e.Source;

            using (var db = new LiteDatabase(dir + DataBaseNames.BossDataBase))
            {
                var collection = db.GetCollection<Boss>("boss");
                var item = collection.FindAll()
                    .Where(x => x.raidName == raidFrame.SelectedItem.ToString());
                foreach(var temp in item)
                {
                    boss.Items.Add(temp.bossName);
                    
                }
            }
        }

        private void bossSelect_SelectionChange(object sender, SelectionChangedEventArgs e)
        {
            if (!ItemForAsignment.IsEnabled)
            {
                ItemForAsignment.IsEnabled = true;
            }
            else
            {
                ItemForAsignment.Items.Clear();
            }

            var bossFrame = (ComboBox)e.Source;
            if (bossFrame.SelectedItem == null) return;
            using (var db = new LiteDatabase(dir + DataBaseNames.ItemDataBase))
            {
                var collection = db.GetCollection<Items>("item");
                var item = collection.FindAll()
                    .Where(x => x.bossName == bossFrame.SelectedItem.ToString());
                foreach (var temp in item)
                {
                    ItemForAsignment.Items.Add(temp.itemName);

                }

            }
        }
        public void ItemListType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            itemPart.Items.Clear();
            itemType.Items.Clear();
            var type = (ComboBox)e.Source;
            switch (type.SelectedIndex)
            {
                case 0:
                    itemType.IsEnabled = true;
                    itemPart.IsEnabled = true;
                    foreach(var item in WeaponType.GetParts)
                    {
                        itemPart.Items.Add(item);
                    }
                    foreach(var item in WeaponType.GetWeapons)
                    {
                        itemType.Items.Add(item);
                    }
                    break;
                case 1:
                    itemType.IsEnabled = true;
                    itemPart.IsEnabled = true;
                    foreach (var item in ArmorType.GetBody)
                    {
                        itemPart.Items.Add(item);
                    }
                    foreach (var item in ArmorType.GetArmors)
                    {
                        itemType.Items.Add(item);
                    }
                    break;
                default:
                    itemType.Text = "";
                    itemPart.Text = "";
                    itemType.IsEnabled = false;
                    itemPart.IsEnabled = false;

                    break;
            }
            ItemShowGrid.Items.Clear();
        }
        public void Color_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var color = (ComboBox)e.Source;
            color.Foreground = color.SelectedIndex switch
            {
                0 => Brushes.Green,
                1 => Brushes.Blue,
                2 => Brushes.Purple,
                3 => Brushes.Orange,
                _ => Brushes.Black,
            };
            rarityOfItem.Text = color.SelectedIndex switch
            {
                0 => "Common",
                1 => "Rare",
                2 => "Epic",
                3 => "Legendary",
                _ => "Show All"
            };
            LoadItems();
        }

        public void DiffItem_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var temp = (ComboBox)e.Source;
            if (temp == itemPart) itemPart.Text = (string)temp.SelectedItem;
            if (temp == itemType) itemType.Text = (string)temp.SelectedItem;
            LoadItems();
        }

        public void DeleteItem_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedItem.Clear();
            if(ItemShowGrid.SelectedItem is Items)
            {
                selectedItem.Add((Items)ItemShowGrid.SelectedItem);
                delateItem.IsEnabled = true;
            }
            
            
            
        }

        public void DisableOffHand_SelectionChange(object sender, SelectionChangedEventArgs e)
        {
            ComboBox mainHand = (ComboBox)e.Source;

            if (MainHandUpdate.Text == "" && openUpdate) return;
            if((mainHand.SelectedItem as ComboBoxItem) != null)
            {
                if ((mainHand.SelectedItem as ComboBoxItem).Content == null)
                {
                    return;
                }
                using (var db = new LiteDatabase(dir + DataBaseNames.ItemDataBase))
                {
                    
                    var item = db.GetCollection<Items>("item")
                        .Query()
                        .Where(x => x.itemName == (mainHand.SelectedItem as ComboBoxItem).Content.ToString())
                        .FirstOrDefault();
                    if (item == null)
                    {
                        return;
                    }
                    if (item.bodyPart == "Two Hand")
                    {
                        OffHandUpdate.IsEnabled = false;
                        OffHandUpdate.Text = "";
                    }
                    else
                    {
                        OffHandUpdate.IsEnabled = true;
                    }
                }
            }
            
        }

        public void itemOwned_SelectionChange(object sender, SelectionChangedEventArgs e)
        {
            var item = (ComboBox)e.Source;
            OwnedItems.Items.Clear();
            if (item.SelectedItem == null) return;
            using (var db = new LiteDatabase(dir + DataBaseNames.BISListDataBase))
            {
                var collection = db.GetCollection<BISList>("bis").FindAll();
                foreach(var t in collection)
                {
                    foreach(PropertyInfo prop in t.GetType().GetProperties())
                    {
                        if(prop.GetValue(t, null) is Items)
                        {
                            Items itemCheck = (Items)prop.GetValue(t, null);
                            if(itemCheck.itemName == item.SelectedItem.ToString())
                            {
                                Brush color = Brushes.White;
                                using(var play = new LiteDatabase(dir + DataBaseNames.PlayersDataBase))
                                {
                                    var playCollecton = play.GetCollection<Players>("players")
                                        .Query()
                                        .Where(x => x.playerName == t.PlayerName)
                                        .FirstOrDefault();
                                    color = Classes.getClassColor(playCollecton.playerClass);
                                }
                                int index = itemSlotNumber[itemCheck.bodyPart];
                                if(itemCheck.bodyPart == "Ring")
                                {
                                    if (t.checkOwned[index + 1] == 1) index++;
                                }
                                if (itemCheck.bodyPart == "Trinket")
                                {
                                    if (t.checkOwned[index + 1] == 1) index++;
                                }
                                playerData info = new playerData(t.PlayerName, color, t.checkOwned[index]);
                                OwnedItems.Items.Add(info);
                            }
                            
                        }
                    }
                    
                }

            }
        }

        #endregion

    }
}
