using Mordochka.Models;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Mordochka.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для ClientsPage.xaml
    /// </summary>
    public partial class ClientsPage : Page
    {
        public ClientsPage()
        {
            InitializeComponent();  
        }
        static mordochkaEntities db;
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            db = new mordochkaEntities();
            cmbCount.SelectedIndex = 3;
            cmbGender.SelectedIndex = 0;
            Update();
        }

        private void Update(string search = "", string gender = "", string sort = "", bool? birthday = false)
        {
            var client = db.Client.ToList();
            int count = client.Count;
            if(!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
            {
                client = client.Where(c => c.name.ToLower().Contains(search.ToLower()) ||
                c.surname.ToLower().Contains(search.ToLower()) || c.patronymic.ToLower().Contains(search.ToLower()) ||
                c.email.ToLower().Contains(search.ToLower()) || c.phone.ToLower().Contains(search.ToLower())).ToList();
            }
            if (!string.IsNullOrEmpty(gender) && !string.IsNullOrWhiteSpace(gender) && gender != "Все")
            {
                client = client.Where(c => c.Gender.gender1 == gender).ToList();
            }
            if (!string.IsNullOrEmpty(sort) && !string.IsNullOrWhiteSpace(sort))
            {
                if(sort == "По фамилии")
                {
                    client = client.OrderBy(c => c.surname).ToList();
                }
                if (sort == "По дате последнего посещения")
                {
                    client = client.OrderByDescending(c => c.LastDate).ToList();
                }
                if (sort == "По количеству посещений")
                {
                    client = client.OrderByDescending(c => c.CountEnter).ToList();
                }
            }
            if(birthday == true)
            {
                client = client.Where(c => c.birthdate.Month == DateTime.Now.Month).ToList(); 
            }
            if(countNext == -1)
            {
                dgvClient.ItemsSource = client;
                textCount.Text = $"{client.Count} из {count}";
            }
            else if(countSkip == 0)
            {
                dgvClient.ItemsSource = client.OrderBy(c => c.id_client).Take(countNext).ToList();
                textCount.Text = $"{countNext} из {count}";
            }
            else
            {
                client = client.OrderBy(c => c.id_client).Skip(countSkip).Take(countNext).ToList();
                if(client.Count != 0)
                {
                    flag = true;
                    dgvClient.ItemsSource = client;
                    textCount.Text = $"{countNext} из {count}";
                }
                else
                {
                    countSkip -= countNext;
                    flag = false;
                    textCount.Text = $"{countNext} из {count}";
                }
            }
        }

        private void cmbGender_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Update(txtSearch.Text, (cmbGender.SelectedItem as ComboBoxItem).Content.ToString(), cmbSort.Text, chbBirtDay.IsChecked);
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            Update(txtSearch.Text, cmbGender.Text, cmbSort.Text, chbBirtDay.IsChecked);
        }

        private void cmbSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Update(txtSearch.Text, cmbGender.Text, (cmbSort.SelectedItem as ComboBoxItem).Content.ToString(), chbBirtDay.IsChecked);
        }

        private void chbBirtDate_Checked(object sender, RoutedEventArgs e)
        {
            Update(txtSearch.Text, cmbGender.Text, cmbSort.Text, chbBirtDay.IsChecked);
        }

        private void chbBirtDate_Unchecked(object sender, RoutedEventArgs e)
        {
            Update(txtSearch.Text, cmbGender.Text, cmbSort.Text, chbBirtDay.IsChecked);
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            var selectClient = dgvClient.SelectedItem as Client;
            var visit = db.ClientService.Where(c => c.id_client == selectClient.id_client).ToList();
            if(visit.Count == 0)
            {
                if(MessageBox.Show("Вы действительно хотите удалить клиента?", "Информация", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        var tags = db.TagClient.Where(c => c.id_client == selectClient.id_client).ToList();
                        db.TagClient.RemoveRange(tags);
                        db.Client.Remove(selectClient);
                        db.SaveChanges();
                        MessageBox.Show("Клиент удален.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                        Update(txtSearch.Text, cmbGender.Text, cmbSort.Text, chbBirtDay.IsChecked);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Клиент не может быть удален!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            var clientSelect = dgvClient.SelectedItem as Client;
            if (clientSelect != null)
            {
                NavigationService.Navigate(new EditClientPage(clientSelect));
            }
            else
            {
                MessageBox.Show("Выберите запись!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new EditClientPage());
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            var clientSelect = dgvClient.SelectedItem as Client;
            if(clientSelect != null)
            {
                NavigationService.Navigate(new VisitPage(clientSelect));
            }
            else
            {
                MessageBox.Show("Выберите запись!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        static int countNext = -1;
        static int countSkip = 0;
        bool flag = true;
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            if (cmbCount.Text != "Все")
            {
                if (countSkip != 0)
                {
                    countSkip -= countNext;
                    Update(txtSearch.Text, cmbGender.Text, cmbSort.Text, chbBirtDay.IsChecked);
                }
            }
        }

        private void btnGo_Click(object sender, RoutedEventArgs e)
        {
            if (cmbCount.Text != "Все")
            {
                if (flag)
                {
                    countSkip += countNext;
                    Update(txtSearch.Text, cmbGender.Text, cmbSort.Text, chbBirtDay.IsChecked);
                }
            }
        }

        private void cmbCount_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(cmbCount.SelectedIndex == 0)
            {
                countNext = 10;
                countSkip = 0;
                Update(txtSearch.Text, cmbGender.Text, cmbSort.Text, chbBirtDay.IsChecked);
            }
            if(cmbCount.SelectedIndex == 1)
            {
                countNext = 50;
                countSkip = 0;
                Update(txtSearch.Text, cmbGender.Text, cmbSort.Text, chbBirtDay.IsChecked);
            }
            if (cmbCount.SelectedIndex == 2)
            {
                countNext = 200;
                countSkip = 0;
                Update(txtSearch.Text, cmbGender.Text, cmbSort.Text, chbBirtDay.IsChecked);
            }
            if (cmbCount.SelectedIndex == 3)
            {
                countNext = -1;
                countSkip = 0;
                Update(txtSearch.Text, cmbGender.Text, cmbSort.Text, chbBirtDay.IsChecked);
            }
        }
    }
}
