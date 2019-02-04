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
        private SpeechRecognitionEngine Sre;

        public static List<Button> SummaryButtonsList = new List<Button>();

        public Summary()
        {
            InitializeComponent();
            double orderPrice = 0;

            Sre = new SpeechRecognitionEngine(new System.Globalization.CultureInfo("pl-PL"));

            SummaryButtonsList.Add(NextPizzaBtn);
            SummaryButtonsList.Add(ResetOrderBtn);

            if (HomePage.OrderList.Count > 0)
            for (var i = 0; i <= HomePage.OnTheList; i++)
            {
                HomePage.OrderList.ElementAt(i).CalculatePrice();

                var sb = new StringBuilder( (i+1).ToString() )
                    .Append(". ").Append(HomePage.OrderList.ElementAt(i).Name)
                    .Append(" - ").Append(HomePage.OrderList.ElementAt(i).PizzaSize.Name)
                    .Append(" (").Append(string.Join(",", HomePage.OrderList.ElementAt(i).AdditivesList)).Append(")")
                    .Append(" - ").Append(HomePage.OrderList.ElementAt(i).TotalPrice)
                    .Append(" zł");

                var summaryBox = new Label()
                {
                    IsEnabled = false,
                    Content = sb.ToString(),
                    Name = "summaryBox" + i,
                    Background = Brushes.LightGray
                };
                
                
                PanelSummary.Children.Add(summaryBox);

                orderPrice += HomePage.OrderList.ElementAt(i).TotalPrice;
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
                Sre.SetInputToDefaultAudioDevice();
                Sre.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(Sre_SpeechRecognized);

                Choices words = new Choices(new string[] {NextPizzaBtn.ToolTip.ToString(), ResetOrderBtn.ToolTip.ToString()});
                GrammarBuilder gramBuild = new GrammarBuilder();
                gramBuild.Append(words);
                Grammar gramSre = new Grammar(gramBuild);
                Sre.LoadGrammar(gramSre);
                Sre.RecognizeAsync(RecognizeMode.Multiple);
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

            Sre.RecognizeAsyncCancel();
            Sre.UnloadAllGrammars();
            Sre.SpeechRecognized -= new EventHandler<SpeechRecognizedEventArgs>(Sre_SpeechRecognized);

            if (HomePage.OrderList.Count > 0) HomePage.OnTheList++;

            ChoosePizza choosePizzaPage = new ChoosePizza();
            NavigationService nav = NavigationService.GetNavigationService(this);
            nav.Navigate(choosePizzaPage);
        }

        private void ResetOrderBtn_Click(object sender, RoutedEventArgs e)
        {
            HomePage.OrderList.Clear();
            HomePage.OnTheList = 0;

            PanelSummary.Children.Clear();
            NavigationService nav = NavigationService.GetNavigationService(this);
            nav.Refresh();
        }

        // MICROSOFT SPEECH PLATFORM
        private void Sre_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (e.Result.Confidence >= 0.70)
            {
                Console.WriteLine("I heard " + e.Result.Text);

                foreach (Button button in SummaryButtonsList)
                {
                    if (e.Result.Text == button.ToolTip.ToString())
                    {
                        if (button.ToolTip.ToString() == "Dodaj")
                            NextPizzaBtn_Click(button, new RoutedEventArgs());
                        else if (button.ToolTip.ToString() == "Resetuj zamówienie")
                            ResetOrderBtn_Click(button, new RoutedEventArgs());

                        break;
                    }
                    
                }
            }
            else
                Console.WriteLine("Unknown word: " + e.Result.Text + ", Confidence: " + e.Result.Confidence);
        }
        // MICROSOFT SPEECH PLATFORM

    }
}
