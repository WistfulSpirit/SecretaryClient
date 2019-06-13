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
    /// Логика взаимодействия для PForm.xaml
    /// </summary>
    public partial class PForm : Window
    {

        public Person NPerson{ get; set; }

        public PForm()
        {
            InitializeComponent();
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(tbName.Text) && !String.IsNullOrWhiteSpace(tbEmail.Text))
            {
                NPerson = new Person() { Name = tbName.Text, Email = tbEmail.Text };
                DialogResult = true;
            }
            else
            {
                MessageBox.Show("Заполните все поля!", "Внимание!", MessageBoxButton.OK);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
