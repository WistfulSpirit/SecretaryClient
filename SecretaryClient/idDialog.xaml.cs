using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SecretaryClient
{
    /// <summary>
    /// Логика взаимодействия для idDialog.xaml
    /// </summary>
    public partial class idDialog : Window
    {

        public int responseID { get; private set; }

        public idDialog()
        {
            InitializeComponent();
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            int tId;
            if (Int32.TryParse(tbId.Text, out tId))
            {
                if (tId > 0)
                {
                    responseID = tId;
                    DialogResult = true;
                }
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
