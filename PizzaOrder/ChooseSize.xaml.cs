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

namespace PizzaOrder
{
    /// <summary>
    /// Interaction logic for ChooseSize.xaml
    /// </summary>
    public partial class ChooseSize : Page
    {

        public static readonly List<PizzaSize> SizeList = new List<PizzaSize>()
        {
            new PizzaSize("Mała (25 cm)", 15),
            new PizzaSize("Średnia (32 cm)", 20),
            new PizzaSize("Duża (40 cm)", 30),
            new PizzaSize("Rodzinna (50 cm)", 40)
        };

        public ChooseSize()
        {
            InitializeComponent();
            int count = 0;


            for (int i = 0; i < GridSize.RowDefinitions.Count; i++)
            {
                for (int j = 0; j < GridSize.ColumnDefinitions.Count; j++)
                {
                    Button sizeButton = new Button
                    {
                        Tag = SizeList.ElementAt(count),
                        Content = SizeList.ElementAt(count).Name,
                        Margin = new Thickness(10, 10, 10, 10)
                    };
                    sizeButton.Click += btn_Click;

                    Grid.SetColumn(sizeButton, j);
                    Grid.SetRow(sizeButton, i);
                    GridSize.Children.Add(sizeButton);

                    count++;
                }
            }
        }

        private void btn_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            PizzaSize size = new PizzaSize();
            size = (PizzaSize)btn.Tag;

            ChoosePizza.OrderList.ElementAt(ChoosePizza.OnTheList).PizzaSize = size;

            Summary summaryPage = new Summary();
            NavigationService.Navigate(summaryPage);
        }
    }
}
