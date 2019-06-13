using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;

namespace SecretaryClient
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        HttpClient client = new HttpClient();
        int curMessageId = -1;
        public MainWindow()
        {
            InitializeComponent();
            client.BaseAddress = new Uri("http://localhost:54014/");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            this.Loaded += MainWindow_LoadedAsync;
        }

        async void MainWindow_LoadedAsync(object sender, RoutedEventArgs e)
        {
            FillTags();
            FillPersons();
        }

        public async void FillTags()
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync("/api/Tag");
                response.EnsureSuccessStatusCode(); // Throw on error code.
                var tags = JsonConvert.DeserializeObject<List<Tag>>(response.Content.ReadAsStringAsync().Result);
                lbTags.ItemsSource = tags;
                lbTags.SelectedValuePath = "Id";
                lbTags.DisplayMemberPath = "Value";
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show("Не удалось подключиться к серверу", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public async void FillPersons()
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync("/api/Person");
                response.EnsureSuccessStatusCode(); // Throw on error code.
                var persons = JsonConvert.DeserializeObject<List<Person>>(response.Content.ReadAsStringAsync().Result);// ReadAsAsync<IEnumerable<Person>>();
                cbReciever.ItemsSource = persons;
                cbReciever.SelectedValuePath = "Id";
                cbReciever.DisplayMemberPath = "DataTextFieldLabel";
                cbReciever.Text = "Выберите получателя";
                cbSender.ItemsSource = persons;
                cbSender.SelectedValuePath = "Id";
                cbSender.DisplayMemberPath = "DataTextFieldLabel";
                cbSender.Text = "Выберите отправителя";
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show("Не удалось подключиться к серверу", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private async void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!IsFormValid())
            {
                MessageBox.Show("Заполните все поля!", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            List<Tag> tags = lbTags.SelectedItems.Cast<Tag>().ToList();
            var message = new Message()
            {
                Sender = (Person)cbSender.SelectedItem, 
                Adressee = (Person)cbReciever.SelectedItem,
                Title = tbTitle.Text,
                Registry_Date = DateTime.Now,
                Content = tbContent.Text,
                Tags = tags
            };
            try
            {
                if (((Button)sender).Tag.ToString() == "addNewMessage")
                {
                    var response = await client.PostAsync("/api/Message/", new StringContent(JsonConvert.SerializeObject(message), Encoding.UTF8, "application/json"));
                    response.EnsureSuccessStatusCode(); // Throw on error code.
                    MessageBox.Show("Сообщение добавлено успешно", "Результат", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    if (curMessageId != -1)
                    {
                        message.Id = curMessageId;
                        var response = await client.PutAsync("/api/Message/" + message.Id, new StringContent(JsonConvert.SerializeObject(message), Encoding.UTF8, "application/json"));
                        response.EnsureSuccessStatusCode(); // Throw on error code.
                        MessageBox.Show("Сообщение отредактировано успешно", "Результат", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Сперва получите сообщение по Id", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show("Не удалось подключиться к серверу", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public bool IsFormValid()
        {
            if (cbSender.SelectedItem == null || cbReciever.SelectedItem == null ||
                String.IsNullOrWhiteSpace(tbTitle.Text) || String.IsNullOrWhiteSpace(tbContent.Text))
                return false;

            return true;
        }

        private async void BtnShowAllMessages_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync("/api/Message");
                response.EnsureSuccessStatusCode(); // Throw on error code.
                var messages = JsonConvert.DeserializeObject<List<Message>>(response.Content.ReadAsStringAsync().Result);
                MList mList = new MList(messages);
                mList.Show();
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show("Не удалось подключиться к серверу", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private async void BtnGetMessage_Click(object sender, RoutedEventArgs e)
        {
            int id = -1;
            lbTags.SelectedIndex = -1;
            idDialog dialog = new idDialog();
            if (dialog.ShowDialog() == true)
            {
                id = dialog.responseID;
            }
            else
                return;
            try
            {
                HttpResponseMessage response = await client.GetAsync("/api/Message/" + id);
                response.EnsureSuccessStatusCode(); // Throw on error code.
                var message = JsonConvert.DeserializeObject<Message>(response.Content.ReadAsStringAsync().Result);// ReadAsAsync<IEnumerable<Person>>();
                if (message == null)
                {
                    MessageBox.Show("Сообщения с таким id нет!", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }
                curMessageId = message.Id;
                tbTitle.Text = message.Title;
                tbContent.Text = message.Content;
                cbSender.SelectedValue = message.Sender.Id;
                cbReciever.SelectedValue = message.Adressee.Id;
                if (message.Tags != null && message.Tags.Count != 0)
                {
                    foreach (Tag tag in message.Tags)
                    {
                        int i = ((List<Tag>)lbTags.ItemsSource).FindIndex(t => t.Id == tag.Id);
                        ((ListBoxItem)lbTags.ItemContainerGenerator.ContainerFromIndex(i)).IsSelected = true;
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show("Не удалось подключиться к серверу", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }




        }

        private async void BtnAddPerson_Click(object sender, RoutedEventArgs e)
        {
            PForm pForm = new PForm();
            if (pForm.ShowDialog() != true)
                return;
            try
            {
                var response = await client.PostAsync("/api/Person/", new StringContent(JsonConvert.SerializeObject(pForm.NPerson), Encoding.UTF8, "application/json"));
                response.EnsureSuccessStatusCode(); // Throw on error code.
                MessageBox.Show("Участник добавлен успешно", "Результат", MessageBoxButton.OK, MessageBoxImage.Information);
                FillPersons();
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show("Не удалось подключиться к серверу", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private async void BtnAddTag_Click(object sender, RoutedEventArgs e)
        {
            TForm tForm = new TForm();
            if (tForm.ShowDialog() != true)
                return;
            try
            {
                var response = await client.PostAsync("/api/Tag/", new StringContent(JsonConvert.SerializeObject(tForm.NTag), Encoding.UTF8, "application/json"));
                response.EnsureSuccessStatusCode(); // Throw on error code.
                MessageBox.Show("Тэг добавлен успешно", "Результат", MessageBoxButton.OK, MessageBoxImage.Information);
                FillTags();
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show("Не удалось подключиться к серверу", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
