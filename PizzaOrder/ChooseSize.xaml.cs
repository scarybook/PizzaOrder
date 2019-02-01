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

        public static readonly List<PizzaSize> SizeList = new List<PizzaSize>()
        {
            new PizzaSize("Mała", "(25 cm)", 15),
            new PizzaSize("Średnia", "(32 cm)", 20),
            new PizzaSize("Duża", "(40 cm)", 30),
            new PizzaSize("Rodzinna", "(50 cm)", 40)
        };

        public static List<Button> SizeButtonsList = new List<Button>();


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

            var sizeWordsList = new string[SizeList.Count + 1];

            for (var index = 0; index < SizeList.Count; index++)
            {
                sizeWordsList[index] = SizeList.ElementAt(index).Name;
            }

            sizeWordsList[SizeList.Count] = "Badziebadla";

            // MICROSOFT SPEECH PLATFORM
            try
            {
                ChoosePizza.Sre.SetInputToDefaultAudioDevice();
                ChoosePizza.Sre.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(Sre_SpeechRecognized);
                ChoosePizza.Sre.RecognizeCompleted += new EventHandler<RecognizeCompletedEventArgs>(Sre_RecognizeCompleted);

                Choices words = new Choices(sizeWordsList);
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

        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            PizzaSize size = new PizzaSize();
            size = (PizzaSize)btn.Tag;

            ChoosePizza.OrderList.ElementAt(ChoosePizza.OnTheList).PizzaSize = size;

            Summary summaryPage = new Summary();
            NavigationService nav = NavigationService.GetNavigationService(this);
            nav.Navigate(summaryPage);
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

                foreach (Button button in SizeButtonsList)
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
    }
}
