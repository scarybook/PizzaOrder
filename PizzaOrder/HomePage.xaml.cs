using System;
using System.Collections.Generic;
using System.Globalization;
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
using Microsoft.Speech.Recognition;

namespace PizzaOrder
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : Page
    {
        public static List<Button> HomePageButtonsList = new List<Button>();
        private SpeechRecognitionEngine Sre;
        
        public static List<Pizza> OrderList = new List<Pizza>() { };
        public static int OnTheList = 0; //order position counter


        public HomePage()
        {
            InitializeComponent();

            Sre = new SpeechRecognitionEngine(new CultureInfo("pl-PL"));

            HomePageButtonsList.Add(BeginOrderButton);

            // MICROSOFT SPEECH PLATFORM
            try
            {
                Sre.SetInputToDefaultAudioDevice();
                Sre.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(Sre_SpeechRecognized);

                Choices words = new Choices(new string[] { BeginOrderButton.ToolTip.ToString()});
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

        // MICROSOFT SPEECH PLATFORM
        private void Sre_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (e.Result.Confidence >= 0.70)
            {
                Console.WriteLine("I heard " + e.Result.Text);

                foreach (Button button in HomePageButtonsList)
                {
                    if (button.ToolTip.ToString() == e.Result.Text)
                    {
                        BeginOrderBtn_Click(button, new RoutedEventArgs());
                        break;
                    }
                }
            }
            else
                Console.WriteLine("Unknown word: " + e.Result.Text + ", Confidence: " + e.Result.Confidence);
        }
        // MICROSOFT SPEECH PLATFORM

        private void BeginOrderBtn_Click(object sender, RoutedEventArgs e)
        {
            Sre.RecognizeAsyncCancel();
            Sre.UnloadAllGrammars();
            Sre.SpeechRecognized -= new EventHandler<SpeechRecognizedEventArgs>(Sre_SpeechRecognized);

            ChoosePizza choosePizzaPage = new ChoosePizza();
            NavigationService nav = NavigationService.GetNavigationService(this);
            nav.Navigate(choosePizzaPage);
        }
    }
}
