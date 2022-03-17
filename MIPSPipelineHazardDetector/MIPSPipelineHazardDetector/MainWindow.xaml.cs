using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MIPSPipelineHazardDetector
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ClientExec exec;
        static Grid diagram;

        public MainWindow()
        {
            
            InitializeComponent();
            exec = new ClientExec(this);
        }

        private void btn_new_Click(object sender, RoutedEventArgs e)
        {
            //Popup file dialog
            Microsoft.Win32.OpenFileDialog fileDialog = new Microsoft.Win32.OpenFileDialog();
            fileDialog.Multiselect = false;
            fileDialog.Filter = Strings.textFileExtensionFilter;
            fileDialog.DefaultExt = ".txt";
            Nullable<bool> dialogOK = fileDialog.ShowDialog();

            //Behavior if the dialog was cancelled
            if (dialogOK != true)
                return;

            //Get filename from Dialog
            string sFilenames = "";
            foreach (string sFilename in fileDialog.FileNames)
                sFilenames += ";" + sFilename;
            sFilenames = sFilenames.Substring(1);

            //Read text from file
            string fileContent = FileManager.ReadTextFile(sFilenames);
            (bool, List<InstructionCommand>, bool) instructionList = Coverter.StringInstructionConverter(fileContent);

            //Give to fileManager | returns true if file is invalid
            if (!instructionList.Item1)
            {
                MessageBox.Show(Strings.error_InvalidFile);
                output_textblock.Text = "";
                return;
            }
            else if (instructionList.Item3)
                MessageBox.Show(Strings.error_UnrecognizedArguments);
            else
            {
                output_textblock.Text = exec.RunApplication(instructionList.Item2);
                string[] ints = Coverter.StringToListCovnerter(fileContent);
                for(int i = 0; i < 4; i++)
                {
                    TextBlock tb = new TextBlock();
                    tb.Text = ints[i];
                    tb.Margin = new Thickness(30);
                    tb.SetValue(Grid.RowProperty, i+1);
                    tb.SetValue(Grid.ColumnProperty, 0);
                    tb.HorizontalAlignment = HorizontalAlignment.Center;
                    tb.VerticalAlignment = VerticalAlignment.Center;
                    tb.Background = new SolidColorBrush(Colors.White);
                    visualGrid.Children.Add(tb);
                }
            }
        }

        private void btn_export_Click(object sender, RoutedEventArgs e)
        {

            //null check for trying to export empty objects
            if (output_textblock.Text == "")
            {
                MessageBox.Show(Strings.error_NullExport);
                return;
            }

            //Open file dialog and save json to given filepath
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = Strings.textFileExtensionFilter;
            if (saveFileDialog.ShowDialog() == true)
                FileManager.WriteOutputToTextFile(output_textblock.Text, saveFileDialog.FileName);
        }

        private void btn_exitApp_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        public void ShowDiagramForFirstFourInstructions(List<List<string>> strings)
        {
            int offset = 0;
            for (int i = 0; i < strings.Count; i++) //for each command
            {
                for (int j = 0; j < strings[i].Count; j++) //for each letter
                {
                    TextBlock tb = new TextBlock();
                    tb.Text = strings[i][j].ToString();
                    tb.SetValue(Grid.RowProperty, i + 1);
                    tb.SetValue(Grid.ColumnProperty, j + 1 + offset);
                    tb.HorizontalAlignment = HorizontalAlignment.Center;
                    tb.VerticalAlignment = VerticalAlignment.Center;
                    visualGrid.Children.Add(tb);
                    
                }
                offset++;
                offset += strings[i].Count-5;
            }
        }

        private void canvas_MouseLeftButtonDown(object sender, RoutedEventArgs e)
        {

        }
    }
}


/*
                 UIElement GetGridElement(Grid g, Row r, Column c)
                {
                    for (int i = 0; i < g.Children.Count; i++)
                    {
                        UIElement e1 = g.Children[i];
                        if (Grid.GetRow(e1) == r && Grid.GetColumn(e1) == c)
                            return e1;
                    }
                    return null;
                }
 
 */