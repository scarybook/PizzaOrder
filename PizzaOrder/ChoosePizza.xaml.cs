using System;
using System.Collections.Generic;
using System.Linq;
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

        public static readonly SpeechRecognitionEngine Sre = new SpeechRecognitionEngine( new System.Globalization.CultureInfo("pl-PL"));
        public static readonly SpeechSynthesizer Ss = new SpeechSynthesizer();

        public static readonly List<Pizza> PizzaList = new List<Pizza>()
        {
            new Pizza("Pepperoni", new List<string>() { "ser mozzarella", "ziołowy sos pomidorowy", "kiełbasa pepperoni" }),
            new Pizza("Hawajska", new List<string>() { "ser mozzarella", "ziołowy sos pomidorowy", "szynka", "ananas" }),
            new Pizza("Farmerska", new List<string>() { "ser mozzarella", "ziołowy sos pomidorowy", "zielona papryka", "pieczarki", "cebula" }),
            new Pizza("Supreme", new List<string>() { "ser mozzarella", "ziołowy sos pomidorowy", "wołowina", "kiełbasa pepperoni", "zielona papryka", "pieczarki", "cebula" })
        };

        public static List<Pizza> OrderList = new List<Pizza>() { };

        public static List<Button> PizzaButtonsList = new List<Button>();

        public static int OnTheList = 0; //order position counter

        public ChoosePizza()
        {
            InitializeComponent();
            var count = 0; 

            for (var i = 0; i < GridPizza.RowDefinitions.Count; i++)
            {
                for (var j = 0; j < GridPizza.ColumnDefinitions.Count; j++)
                {
                    var pizzaButton = new Button
                    {
                        Tag = PizzaList.ElementAt(count),
                        Content = PizzaList.ElementAt(count).Name,
                        Margin = new Thickness(10, 10, 10, 10),
                        ToolTip = String.Join(", ", PizzaList.ElementAt(count).AdditivesList)
                    };

                    pizzaButton.Click += Btn_Click;
                    PizzaButtonsList.Add(pizzaButton);

                    Grid.SetColumn(pizzaButton, j);
                    Grid.SetRow(pizzaButton, i);
                    GridPizza.Children.Add(pizzaButton);

                    count++;
                }

               
            }

            var pizzaWordsList = new string[PizzaList.Count + 1];

            for (var index = 0; index < PizzaList.Count; index++)
            {
                pizzaWordsList[index] = PizzaList.ElementAt(index).Name;
            }

            pizzaWordsList[PizzaList.Count] = "Badziebadla";

            // MICROSOFT SPEECH PLATFORM
            try
            {
                Sre.SetInputToDefaultAudioDevice();
                Sre.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(Sre_SpeechRecognized);
                Sre.RecognizeCompleted += new EventHandler<RecognizeCompletedEventArgs>(Sre_RecognizeCompleted);

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
            Button btn = (Button)sender;
            Pizza pizza = new Pizza();
            pizza = (Pizza)btn.Tag;
            OrderList.Add(pizza);

            ChooseSize chooseSizePage = new ChooseSize();
            NavigationService nav = NavigationService.GetNavigationService(this);
            nav.Navigate(chooseSizePage);
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

                foreach (Button button in PizzaButtonsList)
                {
                    if (button.Content.ToString() == e.Result.Text)
                    {
                        ((SpeechRecognitionEngine)sender).RecognizeAsyncCancel();
                        button.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
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

    } //class
} //ns