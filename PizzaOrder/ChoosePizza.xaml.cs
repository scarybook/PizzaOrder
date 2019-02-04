using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Resources;
using System.Windows.Shapes;
using Microsoft.Speech.Recognition;
using Microsoft.Speech.Synthesis;

namespace PizzaOrder
{
    /// <summary>
    /// Interaction logic for ChoosePizza.xaml
    /// </summary>
    public partial class ChoosePizza : Page
    {
        private SpeechRecognitionEngine Sre;

        public static List<Pizza> PizzaList;
        
        public static List<Button> PizzaButtonsList = new List<Button>();

        public ChoosePizza()
        {
            InitializeComponent();

            var count = 0;

            PizzaList = new List<Pizza>()
            {
                new Pizza("Pepperoni", new List<string>() { "ser mozzarella", "ziołowy sos pomidorowy", "kiełbasa pepperoni" }),
                new Pizza("Hawajska", new List<string>() { "ser mozzarella", "ziołowy sos pomidorowy", "szynka", "ananas" }),
                new Pizza("Farmerska", new List<string>() { "ser mozzarella", "ziołowy sos pomidorowy", "zielona papryka", "pieczarki", "cebula" }),
                new Pizza("Supreme", new List<string>() { "ser mozzarella", "ziołowy sos pomidorowy", "wołowina", "kiełbasa pepperoni", "zielona papryka", "pieczarki", "cebula" })
            };

            Sre = new SpeechRecognitionEngine(new System.Globalization.CultureInfo("pl-PL"));

            for (var i = 0; i < GridPizza.RowDefinitions.Count; i++)
            {
                for (var j = 0; j < GridPizza.ColumnDefinitions.Count; j++)
                {
                    var pizzaButton = new Button
                    {
                        Tag = PizzaList.ElementAt(count),
                        Content = PizzaList.ElementAt(count).Name,
                        Margin = new Thickness(10, 10, 10, 10),
                        ToolTip = string.Join(", ", PizzaList.ElementAt(count).AdditivesList)
                    };

                    pizzaButton.Click += Btn_Click;
                    PizzaButtonsList.Add(pizzaButton);
                    
                    Grid.SetColumn(pizzaButton, j);
                    Grid.SetRow(pizzaButton, i);
                    GridPizza.Children.Add(pizzaButton);

                    count++;
                }
            }

            var pizzaWordsList = new string[PizzaList.Count];

            for (var index = 0; index < PizzaList.Count; index++)
            {
                pizzaWordsList[index] = PizzaList.ElementAt(index).Name;
            }

            // MICROSOFT SPEECH PLATFORM
            try
            {
                Sre.SetInputToDefaultAudioDevice();
                Sre.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(Sre_SpeechRecognized);

                Choices words = new Choices(pizzaWordsList);
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

        private void Btn_Click(object sender, RoutedEventArgs e)
        {

            Sre.RecognizeAsyncStop();
            Sre.UnloadAllGrammars();
            Sre.SpeechRecognized -= new EventHandler<SpeechRecognizedEventArgs>(Sre_SpeechRecognized);

            Button btn = (Button)sender;
            Pizza pizza = new Pizza((Pizza)btn.Tag);

            HomePage.OrderList.Add(pizza);

            ChooseSize chooseSizePage = new ChooseSize();
            NavigationService nav = NavigationService.GetNavigationService(this);
            nav.Navigate(chooseSizePage);
        }

        // MICROSOFT SPEECH PLATFORM
        private void Sre_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (e.Result.Confidence >= 0.70)
            { 
                Console.WriteLine("I heard " + e.Result.Text);

                foreach (Button button in PizzaButtonsList)
                {
                    if (button.Content.ToString() == e.Result.Text)
                    {
                        Btn_Click(button, new RoutedEventArgs());
                        break;
                    }
                }
            }
            else
                Console.WriteLine("Unknown word: " + e.Result.Text + ", Confidence: " + e.Result.Confidence);
        }
        // MICROSOFT SPEECH PLATFORM
    } //class
} //ns