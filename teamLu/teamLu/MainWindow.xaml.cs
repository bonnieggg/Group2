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

namespace teamLu
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private static List<Product> products;

        public MainWindow()
        {
            InitializeComponent();
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.ShowDialog();

            string url = "https://list.lu.com/list/p2p?minMoney=&maxMoney=&minDays=&maxDays=&minRate=&maxRate=&mode=&tradingMode=&isCx=&currentPage=1&orderCondition=&isShared=&canRealized=&productCategoryEnum=";
            string htmlString = WebContent.getWebContent(url);
            products = WebContent.getWebData(htmlString);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (Product product in products)
            {
                ListViewItem item = new ListViewItem();
                item.Content = product.ProductName;
                lstView.Items.Add(item);
            }
        }
    }
}
