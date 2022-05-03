using System;
using System.Windows;

namespace ItemList
{
    /// <summary>
    /// Interaction logic for Input_Dialog.xaml
    /// </summary>
    public partial class Input_Dialog : Window
    {
        public Input_Dialog(string question,
            string defaultAnswer = "",
            string dropName = "",
            int checklist = 0,
            string[] list = null)
        {
            InitializeComponent();
            lblQuestion.Content = question;
            txtAnswer.Text = defaultAnswer;
            dropDown.Content = dropName;
            switch (checklist)
            {
                case 0:
                    dropDown.Visibility = Visibility.Collapsed;
                    choice.Visibility = Visibility.Collapsed;
                    raidDel.Visibility = Visibility.Collapsed;
                    bossDel.Visibility = Visibility.Collapsed;
                    break;
                case 1:
                    raidDel.Visibility = Visibility.Collapsed;
                    bossDel.Visibility = Visibility.Collapsed;
                    for (int i = 0; i < list.Length; i++)
                    {
                        choice.Items.Add(list[i]);
                    }
                    break;
                case 2:
                    lblQuestion.Visibility = Visibility.Collapsed;
                    txtAnswer.Visibility = Visibility.Collapsed;
                    choice.Visibility = Visibility.Collapsed;
                    bossDel.Visibility = Visibility.Collapsed;
                    for (int i = 0; i < list.Length; i++)
                    {
                        raidDel.Items.Add(list[i]);
                    }
                    break;
                case 3:
                    lblQuestion.Visibility = Visibility.Collapsed;
                    txtAnswer.Visibility = Visibility.Collapsed;
                    choice.Visibility = Visibility.Collapsed;
                    raidDel.Visibility = Visibility.Collapsed;
                    for (int i = 0; i < list.Length; i++)
                    {
                        bossDel.Items.Add(list[i]);
                    }
                    break;
                case 4:
                    choice.Visibility = Visibility.Collapsed;
                    bossDel.Visibility = Visibility.Collapsed;
                    for (int i = 0; i < list.Length; i++)
                    {
                        raidDel.Items.Add(list[i]);
                    }
                    break;
                case 5:
                    choice.Visibility = Visibility.Collapsed;
                    raidDel.Visibility = Visibility.Collapsed;
                    for (int i = 0; i < list.Length; i++)
                    {
                        bossDel.Items.Add(list[i]);
                    }
                    break;
                default :
                    break;

            }
        }
        private void btnDialogOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            
        }
        private void btnDialogCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            txtAnswer.SelectAll();
            txtAnswer.Focus();
        }
        public string Raid
        {
            get { return raidDel.Text; }
        }
        public string Boss
        {
            get { return bossDel.Text; }
        }
        public string Choice
        {
            get { return choice.Text; }
        }
        public string Answer
        {
            get { return txtAnswer.Text; }
        }
    }
}
