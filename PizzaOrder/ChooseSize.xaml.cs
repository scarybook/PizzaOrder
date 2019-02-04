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
    /// Interaction logic for ChooseSize.xaml
    /// </summary>
    public partial class ChooseSize : Page
    {
        private SpeechRecognitionEngine Sre;

        public static List<PizzaSize> SizeList;

        public static List<Button> SizeButtonsList = new List<Button>();

        public ChooseSize()
        {
            InitializeComponent();

            int count = 0;
            
            SizeList = new List<PizzaSize>()
            {
                new PizzaSize("Mała", "(25 cm)", 15),
                new PizzaSize("Średnia", "(32 cm)", 20),
                new PizzaSize("Duża", "(40 cm)", 30),
                new PizzaSize("Rodzinna", "(50 cm)", 40)
            };

            Sre = new SpeechRecognitionEngine(new System.Globalization.CultureInfo("pl-PL"));

            for (int i = 0; i < GridSize.RowDefinitions.Count; i++)
            {
                for (int j = 0; j < GridSize.ColumnDefinitions.Count; j++)
                {
                    Button sizeButton = new Button
                    {
                        Tag = SizeList.ElementAt(count),
                        Content = SizeList.ElementAt(count).Name,
                        Margin = new Thickness(10, 10, 10, 10),
                        ToolTip = SizeList.ElementAt(count).Tooltip
                    };

                    sizeButton.Click += Btn_Click;
                    SizeButtonsList.Add(sizeButton);

                    Grid.SetColumn(sizeButton, j);
                    Grid.SetRow(sizeButton, i);
                    GridSize.Children.Add(sizeButton);

                    count++;
                }
            }

            var sizeWordsList = new string[SizeList.Count];

            for (var index = 0; index < SizeList.Count; index++)
            {
                sizeWordsList[index] = SizeList.ElementAt(index).Name;
            }

            // MICROSOFT SPEECH PLATFORM
            try
            {
                Sre.SetInputToDefaultAudioDevice();
                Sre.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(Sre_SpeechRecognized);

                Choices words = new Choices(sizeWordsList);
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
            Sre.RecognizeAsyncCancel();
            Sre.UnloadAllGrammars();
            Sre.SpeechRecognized -= new EventHandler<SpeechRecognizedEventArgs>(Sre_SpeechRecognized);

            Button btn = (Button)sender;
            PizzaSize size = (PizzaSize)btn.Tag;
            
            HomePage.OrderList.ElementAt(HomePage.OnTheList).PizzaSize = size;

            ChangeAdditives changeAdditivesPage = new ChangeAdditives();
            NavigationService nav = NavigationService.GetNavigationService(this);
            nav.Navigate(changeAdditivesPage);
        }

        // MICROSOFT SPEECH PLATFORM
        private void Sre_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (e.Result.Confidence >= 0.70)
            {
                Console.WriteLine("I heard " + e.Result.Text);

                foreach (Button button in SizeButtonsList)
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
    }
}
