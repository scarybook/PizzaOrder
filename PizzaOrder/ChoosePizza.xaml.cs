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
using System.Windows.Resources;
using System.Windows.Shapes;

namespace PizzaOrder
{
    /// <summary>
    /// Interaction logic for ChoosePizza.xaml
    /// </summary>
    public partial class ChoosePizza : Page
    {
    
        public static readonly List<Pizza> PizzaList = new List<Pizza>()
        {
            new Pizza("Pepperoni", new List<string>() { "ser mozzarella", "ziołowy sos pomidorowy", "kiełbasa pepperoni" }),
            new Pizza("Hawajska", new List<string>() { "ser mozzarella", "ziołowy sos pomidorowy", "szynka", "ananas" }),
            new Pizza("Farmerska", new List<string>() { "ser mozzarella", "ziołowy sos pomidorowy", "zielona papryka", "pieczarki", "cebula" }),
            new Pizza("Supreme", new List<string>() { "ser mozzarella", "ziołowy sos pomidorowy", "wołowina", "kiełbasa pepperoni", "zielona papryka", "pieczarki", "cebula" })
        };

        public static List<Pizza> OrderList =
            new List<Pizza>() { };

        public static int OnTheList = 0;
        
        public ChoosePizza()
        {
            InitializeComponent();
            int count = 0;


            for (int i = 0; i < GridPizza.RowDefinitions.Count; i++)
            {
                for (int j = 0; j < GridPizza.ColumnDefinitions.Count; j++)
                {
                    Button pizzaButton = new Button
                    {
                        Tag = PizzaList.ElementAt(count),
                        Content = PizzaList.ElementAt(count).Name,
                        Margin = new Thickness(10, 10, 10, 10)
                    };

                    pizzaButton.Click += btn_Click;

                    /*
                    Uri resourceUri = new Uri("Images/" + "pizza" + ".jpg", UriKind.Relative);
                    StreamResourceInfo streamInfo = Application.GetResourceStream(resourceUri);

                    BitmapFrame temp = BitmapFrame.Create(streamInfo.Stream);
                    var brush = new ImageBrush();
                    brush.ImageSource = temp;

                    pizzaButton.Background = brush;
                    */

                    Grid.SetColumn(pizzaButton, j);
                    Grid.SetRow(pizzaButton, i);
                    GridPizza.Children.Add(pizzaButton);

                    count++;
                }

            }
        }

        private void btn_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            Pizza pizza = new Pizza();
            pizza = (Pizza)btn.Tag;
            OrderList.Add(pizza);

            ChooseSize chooseSizePage = new ChooseSize();
            NavigationService.Navigate(chooseSizePage);
        }
    }
}