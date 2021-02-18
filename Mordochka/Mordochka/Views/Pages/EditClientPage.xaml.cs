using Microsoft.Win32;
using Mordochka.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.Pkcs;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Mordochka.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для EditClientPage.xaml
    /// </summary>
    public partial class EditClientPage : Page
    {
        public EditClientPage()
        {
            InitializeComponent();
            spId.Visibility = Visibility.Collapsed;
        }
        public EditClientPage(Client client)
        {
            InitializeComponent();
            txtId.Text = client.id_client.ToString();
            txtSurname.Text = client.surname.ToString();
            txtName.Text = client.name.ToString();
            txtPatronymic.Text = client.patronymic.ToString();
            txtPhone.Text = client.phone.ToString();
            txtEmail.Text = client.email.ToString();
            dpBirthDate.SelectedDate = client.birthdate;
            this.currentClient = client;
            this.photo = currentClient.photo.Trim();
            if (!string.IsNullOrEmpty(photo) && !string.IsNullOrWhiteSpace(photo))
            {
                byte[] photos = File.ReadAllBytes(Environment.CurrentDirectory + "\\" + currentClient.photo.Trim());

                using (MemoryStream stream = new MemoryStream(photos))
                {
                    BitmapImage image = new BitmapImage();
                    stream.Seek(0, SeekOrigin.Begin);
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = stream;
                    image.EndInit();
                    imgClient.Source = image;
                }
            }
        }
        void SaveImage()
        {
            if(!string.IsNullOrEmpty(photo) && !string.IsNullOrWhiteSpace(photo) && photo.Trim() != currentClient.photo.Trim())
            {
                File.Copy(photo, Environment.CurrentDirectory + "\\Клиенты\\" + System.IO.Path.GetFileName(photo));
            }
        }
        string photo = "";
        Client currentClient;
        static mordochkaEntities db = new mordochkaEntities();

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (Check())
            {
                try
                {
                    if (currentClient == null)
                    {

                        int id = db.Client.Max(c => c.id_client) + 1;
                        SaveImage();
                        Client client = new Client
                        {
                            id_client = id,
                            surname = txtSurname.Text,
                            name = txtName.Text,
                            patronymic = txtPatronymic.Text,
                            birthdate = dpBirthDate.SelectedDate.Value,
                            id_gender = (int)cbGender.SelectedValue,
                            phone = txtPhone.Text,
                            email = txtEmail.Text,
                            photo = photo
                        };
                        db.Client.Add(client);
                        db.SaveChanges();
                        foreach (var item in tags)
                        {
                            db.TagClient.Add(new TagClient
                            {
                                id_tag = item.id_tag,
                                id_client = id
                            });
                        }
                        db.SaveChanges();
                        NavigationService.GoBack();

                    }
                    else
                    {
                        SaveImage();
                        var clientadd = db.Client.FirstOrDefault(c => c.id_client == currentClient.id_client);
                        clientadd.surname = txtSurname.Text;
                        clientadd.name = txtName.Text;
                        clientadd.patronymic = txtPatronymic.Text;
                        clientadd.birthdate = dpBirthDate.SelectedDate.Value;
                        clientadd.id_gender = (int)cbGender.SelectedValue;
                        clientadd.phone = txtPhone.Text;
                        clientadd.email = txtEmail.Text;
                        clientadd.photo = @"Клиенты\" + System.IO.Path.GetFileName(photo);
                        Properties.Settings.Default.editClientId = 0;

                        foreach (var item in remuveTag)
                        {
                            var ClientTags = db.TagClient.FirstOrDefault(c => c.id_client == currentClient.id_client && c.id_tag == item.id_tag);
                            if (ClientTags != null)
                            {
                                db.TagClient.Remove(ClientTags);
                            }
                        }
                        db.SaveChanges();
                        foreach (var tag in currentClientTag)
                        {
                            var ClientTags = db.TagClient.FirstOrDefault(c => c.id_client == currentClient.id_client && c.id_tag == tag.id_tag);
                            if (ClientTags == null)
                            {
                                db.TagClient.Add(
                                    new TagClient
                                    {
                                        id_client = currentClient.id_client,
                                        id_tag = tag.id_tag
                                    });
                            }
                        }
                        db.SaveChanges();
    
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Не верно введены данные!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        bool Check()
        {
            bool check = true;
            if(string.IsNullOrEmpty(txtName.Text) || string.IsNullOrWhiteSpace(txtName.Text))
            {
                check = false;
            }
            if (string.IsNullOrEmpty(txtSurname.Text) || string.IsNullOrWhiteSpace(txtName.Text))
            {
                check = false;
            }
            if (string.IsNullOrEmpty(txtPatronymic.Text) || string.IsNullOrWhiteSpace(txtPatronymic.Text))
            {
                check = false;
            }
            if(txtName.Text.Length > 40 || txtSurname.Text.Length > 40 || txtPatronymic.Text.Length > 40)
            {
                check = false;
            }
            if(!txtEmail.Text.Contains("@"))
            {
                check = false;
            }
            txtPhone.Text = txtPhone.Text.Trim();
            for(int i = 0;txtPhone.Text.Length > i;i++)
            {
                if(!char.IsDigit(txtPhone.Text[i]))
                {
                    if(txtPhone.Text[i] != '+')
                    {
                        if (txtPhone.Text[i] != '-')
                        {
                            if (txtPhone.Text[i] != '(')
                            {
                                if (txtPhone.Text[i] != ')')
                                {
                                    if (txtPhone.Text[i] != ' ')
                                    {
                                        check = false;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return check;
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
            Properties.Settings.Default.editClientId = 0;
        }

        private void btnImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog();
            if(file.ShowDialog() == true)
            {
                this.photo = file.FileName;
                byte[] photos = File.ReadAllBytes(file.FileName);
                using (MemoryStream stream = new MemoryStream(photos))
                {
                    BitmapImage image = new BitmapImage();
                    stream.Seek(0, SeekOrigin.Begin); 
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = stream;
                    image.EndInit();
                    imgClient.Source = image;
                }
            }
        }
        List<Tag> currentClientTag = new List<Tag>();
        List<Tag> remuveTag = new List<Tag>();
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if(currentClient != null)
            {
                Properties.Settings.Default.editClientId = currentClient.id_client;
                cbGender.ItemsSource = db.Gender.ToList();
                cbGender.Text = currentClient.Gender.gender1;
                currentClientTag = db.TagClient.Where(c => c.id_client == currentClient.id_client).Select(c => c.Tag).ToList();
                lvTag.ItemsSource = db.Tag.ToList();
            }
            else
            {
                lvTag.ItemsSource = db.Tag.ToList();
                cbGender.ItemsSource = db.Gender.ToList();
            }
        }
        List<Tag> tags = new List<Tag>();
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if(currentClient == null)
            {
                var tag = (sender as CheckBox).DataContext as Tag;
                tags.Add(tag);
            }
            else
            {
                var tag = (sender as CheckBox).DataContext as Tag;
                remuveTag.Remove(tag);
                currentClientTag.Add(tag);
            }
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (currentClient == null)
            {
                var tag = (sender as CheckBox).DataContext as Tag;
                tags.Remove(tag);
            }
            else
            {
                var tag = (sender as CheckBox).DataContext as Tag;
                currentClientTag.Remove(tag);
                remuveTag.Add(tag);
            }
        }
    }
}
