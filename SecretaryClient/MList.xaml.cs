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
    /// Логика взаимодействия для MList.xaml
    /// </summary>
    public partial class MList : Window
    {
        public List<Message> messages { get; private set; }
        public MList(List<Message> Messages)
        {
            messages = Messages;
            InitializeComponent();
        }
    }
}
