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
using Microsoft.Speech.Recognition;

namespace PizzaOrder
{
    /// <summary>
    /// Interaction logic for ChangeAdditives.xaml
    /// </summary>
    public partial class ChangeAdditives : Page
    {
        private SpeechRecognitionEngine Sre;

        public static List<string> AdditiveList;


        public static List<ToggleButton> AdditiveButtonsList = new List<ToggleButton>();

        public ChangeAdditives()
        {
            InitializeComponent();

            int count = 0;

            AdditiveList = new List<string>()
            {
                "wołowina",
                "kiełbasa pepperoni",
                "zielona papryka",
                "pieczarki",
                "cebula",
                "szynka",
                "ananas",
                "kurczak",
                "pomidor"
            };

            Sre = new SpeechRecognitionEngine(new System.Globalization.CultureInfo("pl-PL"));

            for (int i = 0; i < GridAdditives.RowDefinitions.Count; i++)
            {
                for (int j = 0; j < GridAdditives.ColumnDefinitions.Count; j++)
                {
                    ToggleButton additiveButton = new ToggleButton
                    {
                        Content = AdditiveList.ElementAt(count),
                        Margin = new Thickness(10, 10, 10, 10),
                    };

                    additiveButton.Checked += Btn_Check;
                    additiveButton.Unchecked += Btn_Uncheck;

                    foreach (var additive in HomePage.OrderList.ElementAt(HomePage.OnTheList).AdditivesList)
                    {
                        if (additive == additiveButton.Content.ToString())
                            additiveButton.IsChecked = true;
                    }

                    AdditiveButtonsList.Add(additiveButton);

                    Grid.SetColumn(additiveButton, j);
                    Grid.SetRow(additiveButton, i);
                    GridAdditives.Children.Add(additiveButton);


                    count++;
                }
            }
            
            var additiveWordsList = new string[AdditiveList.Count+1];

            for (var index = 0; index < AdditiveList.Count; index++)
            {
                additiveWordsList[index] = AdditiveList.ElementAt(index);
            }

            additiveWordsList[AdditiveList.Count] = "Dalej";

            // MICROSOFT SPEECH PLATFORM
            try
            {
                Sre.SetInputToDefaultAudioDevice();
                Sre.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(Sre_SpeechRecognized);

                Choices words = new Choices(additiveWordsList);
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

        private void Btn_Check(object sender, RoutedEventArgs e)
        {
            ToggleButton btn = (ToggleButton)sender;
            btn.IsChecked = true;
            //Console.WriteLine("You've checked " + btn.Content);
        }

        private void Btn_Uncheck(object sender, RoutedEventArgs e)
        {
            ToggleButton btn = (ToggleButton)sender;
            btn.IsChecked = false;
            //Console.WriteLine("You've unchecked " + btn.Content);
        }

        private void NextStepButton_Click(object sender, RoutedEventArgs e)
        {
            Sre.RecognizeAsyncCancel();
            Sre.UnloadAllGrammars();
            Sre.SpeechRecognized -= new EventHandler<SpeechRecognizedEventArgs>(Sre_SpeechRecognized);

            var new_list = new List<string>();

            new_list.Add(HomePage.OrderList.ElementAt(HomePage.OnTheList).AdditivesList.ElementAt(0));
            new_list.Add(HomePage.OrderList.ElementAt(HomePage.OnTheList).AdditivesList.ElementAt(1));

            foreach (var button in AdditiveButtonsList)
            {
                if(button.IsChecked == true)
                    new_list.Add(button.Content.ToString());
            }

            HomePage.OrderList.ElementAt(HomePage.OnTheList).AdditivesList = new_list;

            foreach (var button in AdditiveButtonsList)
            {
                if (button.IsChecked == true)
                    button.IsChecked = false;

                button.Checked -= Btn_Check;
                button.Unchecked -= Btn_Check;
            }

            
            Summary summaryPage = new Summary();
            NavigationService nav = NavigationService.GetNavigationService(this);
            nav.Navigate(summaryPage);
        }

        // MICROSOFT SPEECH PLATFORM
        private void Sre_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (e.Result.Confidence >= 0.70)
            {
                Console.WriteLine("I heard " + e.Result.Text);

                foreach (ToggleButton button in AdditiveButtonsList)
                {
                    if (button.Content.ToString() == e.Result.Text)
                    {
                        if (button.IsChecked == true)
                        {
                            Btn_Uncheck(button, new RoutedEventArgs());
                        }
                        else
                        {
                            Btn_Check(button, new RoutedEventArgs());
                        }

                        break;
                    }
                }

                if(NextStepButton.Content.ToString() == e.Result.Text)
                    NextStepButton_Click(NextStepButton, new RoutedEventArgs());
            }
            else
                Console.WriteLine("Unknown word: " + e.Result.Text + ", Confidence: " + e.Result.Confidence);
        }
        // MICROSOFT SPEECH PLATFORM
    }
}
