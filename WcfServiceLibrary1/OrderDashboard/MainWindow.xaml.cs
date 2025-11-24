using System.Collections.Generic;
using System.Windows;
using OrderDashboard.OrderServiceRef;

namespace OrderDashboard
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            var client = new OrderServiceClient();
            var statuses = new List<OrderStatus>(client.GetOrderStatuses());

            // Бар-график
            ChartItems.ItemsSource = statuses;

            // Pie Chart
            Pie.Draw(statuses);

            client.Close();
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }
    }
}
