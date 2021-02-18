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
    /// Логика взаимодействия для VisitPage.xaml
    /// </summary>
    public partial class VisitPage : Page
    {
        public VisitPage(Client client)
        {
            InitializeComponent();
            this.currentClient = client;
        }
        Client currentClient;
        static mordochkaEntities db = new mordochkaEntities();
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var clientService = db.ClientService.Where(c => c.id_client == currentClient.id_client).ToList();
            dgvClientVisit.ItemsSource = clientService;
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
