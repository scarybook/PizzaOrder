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
using System.Windows.Threading;

namespace PizzaOrder
{
    /// <summary>
    /// Interaction logic for Summary.xaml
    /// </summary>
    public partial class Summary : Page
    {
        public Summary()
        {
            double orderPrice = 0;

            InitializeComponent();
            if (ChoosePizza.OrderList.Count > 0)
            for (var i = 0; i <= ChoosePizza.OnTheList; i++)
            {
                ChoosePizza.OrderList.ElementAt(i).CalculatePrice();

                var sb = new StringBuilder( (i+1).ToString() )
                    .Append(". ").Append(ChoosePizza.OrderList.ElementAt(i).Name)
                    .Append(", ").Append(ChoosePizza.OrderList.ElementAt(i).PizzaSize.Name)
                    .Append(", ").Append(ChoosePizza.OrderList.ElementAt(i).TotalPrice)
                    .Append(" zł");

                    var summaryBox = new Label()
                    {
                        IsEnabled = false,
                        Content = sb.ToString(),
                        Name = "summaryBox" + i,
                        Background = Brushes.LightGray
                    };

                    PanelSummary.Children.Add(summaryBox);

                orderPrice += ChoosePizza.OrderList.ElementAt(i).TotalPrice;
            }

            var orderPriceBox = new Label()
            {
                Content = "CAŁKOWITY KOSZT ZAMÓWIENIA: " + orderPrice + " zł",
                Name = "orderPriceBox",
                Margin = new Thickness(0, 10, 0, 0),
                Background = Brushes.White,
                FontWeight = FontWeights.Bold,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            PanelSummary.Children.Add(orderPriceBox);
        }

        private void OnClick_nextPizzaBtn(object sender, RoutedEventArgs e)
        {

            if(ChoosePizza.OrderList.Count > 0) ChoosePizza.OnTheList++;

            ChoosePizza choosePizzaPage = new ChoosePizza();
            NavigationService.Navigate(choosePizzaPage);
        }

        private void OnClick_resetOrderBtn(object sender, RoutedEventArgs e)
        {
            ChoosePizza.OrderList.Clear();
            ChoosePizza.OnTheList = 0;

            PanelSummary.Children.Clear();
            NavigationService.Refresh();
             
            
        }
    }
}
