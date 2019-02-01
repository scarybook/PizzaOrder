using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Microsoft.Speech.Recognition;

namespace PizzaOrder
{
    /// <summary>
    /// Interaction logic for Summary.xaml
    /// </summary>
    public partial class Summary : Page
    {
        public static List<Button> SummaryButtonsList = new List<Button>();

        public Summary()
        {
            InitializeComponent();
            double orderPrice = 0;

            SummaryButtonsList.Add(NextPizzaBtn);
            SummaryButtonsList.Add(ResetOrderBtn);

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

            // MICROSOFT SPEECH PLATFORM
            try
            {
                ChoosePizza.Sre.SetInputToDefaultAudioDevice();
                ChoosePizza.Sre.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(Sre_SpeechRecognized);
                ChoosePizza.Sre.RecognizeCompleted += new EventHandler<RecognizeCompletedEventArgs>(Sre_RecognizeCompleted);

                Choices words = new Choices(new string[] {NextPizzaBtn.ToolTip.ToString(), ResetOrderBtn.ToolTip.ToString(), "Badziebadla"});
                GrammarBuilder gramBuild = new GrammarBuilder();
                gramBuild.Append(words);
                Grammar gramSre = new Grammar(gramBuild);
                ChoosePizza.Sre.LoadGrammar(gramSre);

                ChoosePizza.Sre.RecognizeAsync(RecognizeMode.Multiple);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }
            // MICROSOFT SPEECH PLATFORM

        }

        private void NextPizzaBtn_Click(object sender, RoutedEventArgs e)
        {

            if(ChoosePizza.OrderList.Count > 0) ChoosePizza.OnTheList++;

            ChoosePizza choosePizzaPage = new ChoosePizza();
            NavigationService nav = NavigationService.GetNavigationService(this);
            nav.Navigate(choosePizzaPage);
        }

        private void ResetOrderBtn_Click(object sender, RoutedEventArgs e)
        {
            ChoosePizza.OrderList.Clear();
            ChoosePizza.OnTheList = 0;

            PanelSummary.Children.Clear();
            NavigationService nav = NavigationService.GetNavigationService(this);
            nav.Refresh();
        }

        // MICROSOFT SPEECH PLATFORM
        private static void Sre_SpeechRecognized(object sender,
            SpeechRecognizedEventArgs e)
        {
            if (e.Result.Text == "Badziebadla")
            {
                ((SpeechRecognitionEngine)sender).RecognizeAsyncCancel();
                Console.WriteLine("Badziebadla");
                return;
            }
            if (e.Result.Confidence >= 0.75)
            {
                Console.WriteLine("I heard " + e.Result.Text);

                foreach (Button button in SummaryButtonsList)
                {
                    if (button.ToolTip.ToString() == e.Result.Text)
                    {
                        ((SpeechRecognitionEngine)sender).RecognizeAsyncCancel();
                        button.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                        break;
                    }
                }
            }
            else
                Console.WriteLine("Unknown word, try again");
        }

        private static void Sre_RecognizeCompleted(object sender,
            RecognizeCompletedEventArgs e)
        {
            return;
        }
        // MICROSOFT SPEECH PLATFORM

    }
}
