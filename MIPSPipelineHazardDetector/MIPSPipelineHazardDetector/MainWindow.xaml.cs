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

        public MainWindow()
        {
            
            InitializeComponent();
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
            (bool, dynamic) instructionList = InstructionConverter.StringInstructionConverter(fileContent);

            //Give to fileManager | returns true if file is valid
            if (!instructionList.Item1)
                MessageBox.Show(Strings.error_InvalidFile);
            else
                exec.Foo();
        }

        private void btn_export_Click(object sender, RoutedEventArgs e)
        {

            //null check for trying to export empty objects
            if ("ToDo" == null)
            {
                MessageBox.Show(Strings.error_NullExport);
                return;
            }

            //Open file dialog and save json to given filepath
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = Strings.textFileExtensionFilter;
            if (saveFileDialog.ShowDialog() == true)
                FileManager.WriteOutputToTextFile("ToDO", saveFileDialog.FileName);

        }

        private void btn_exitApp_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void canvas_MouseLeftButtonDown(object sender, RoutedEventArgs e)
        {

        }
    }
}
