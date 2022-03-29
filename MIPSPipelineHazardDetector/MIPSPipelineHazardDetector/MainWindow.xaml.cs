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
        public List<List<string>> outputNonforwarding;
        public List<List<string>> outputForwarding;

        public MainWindow()
        {
            
            InitializeComponent();
            exec = new ClientExec(this);
            WelcomeMemo.Text = Strings.WelcomeMemoText;
            AddLabelsToDiagram();
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
                PipelineTextBox_WF.Text = "";
                return;
            }
            else if (instructionList.Item3)
                MessageBox.Show(Strings.error_UnrecognizedArguments);
            else
            {
                //run instructions through pipeline
                exec.RunApplication(instructionList.Item2);

                //put the instructions in the diagrams
                string[] ints = Coverter.StringToListCovnerter(fileContent);
                AddInstructionsToCanvas(ints);
            }
        }

        public void DisplayForwardingPipeline(string s)
        {
            PipelineTextBox_WF.Text = s;
        }

        public void DisplayNoForwardingPipeline(string s)
        {
            PipelineTextbox_NF.Text = s;
        }

        private void btn_export_Click(object sender, RoutedEventArgs e)
        {

            //null check for trying to export empty objects
            if (PipelineTextBox_WF.Text == "")
            {
                MessageBox.Show(Strings.error_NullExport);
                return;
            }

            //
            string first = "This is the output (without spacing) of the given instructions\n" +
                "in a pipeline that does not have a forwarding unit:\n" + GetOutputString(outputNonforwarding);
            string space = "\n\n---------------------------------\n\n";
            string second = "This is the output (without spacing) of the given instructions\n" +
                "in a pipeline that has a forwarding unit:\n" + GetOutputString(outputForwarding);
            string output = first+space+second;

            //Open file dialog and save json to given filepath
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = Strings.textFileExtensionFilter;
            if (saveFileDialog.ShowDialog() == true)
                FileManager.WriteOutputToTextFile(output, saveFileDialog.FileName);
        }

        private string GetOutputString(List<List<string>> outputList)
        {
            string culm = "";
            for (int i = 0; i < outputList.Count; i++)
            {
                string commandstring = "";
                for (int j = 0; j < outputList[i].Count; j++)
                {
                    commandstring += outputList[i][j];
                }
                culm += commandstring + "\n";
            }
            return culm;
        }

        private void btn_exitApp_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        public void ShowForwardingDiagram(List<List<string>> strings)
        {
            Grid grid = ForwardingDiagram;
            AddColumnsToDiagram(5, grid);
            int d_offset = 0;
            int offset = 0;
            for (int i = 0; i < strings.Count; i++) //for each command
            {
                AddColumnsToDiagram(strings[i].Count - 5+1, grid);

                if(i == 0)
                {
                    for (int j = 0; j < strings[i].Count; j++) //for each letter
                    {
                        TextBlock tb = new TextBlock();
                        tb.Text = strings[i][j].ToString();
                        tb.SetValue(Grid.RowProperty, i + 1);
                        tb.SetValue(Grid.ColumnProperty, j + 1 + offset);
                        tb.HorizontalAlignment = HorizontalAlignment.Center;
                        tb.VerticalAlignment = VerticalAlignment.Center;
                        grid.Children.Add(tb);

                        if (strings[i][j].ToString() == "D")
                        {
                            d_offset = j + 1 + offset;
                        }
                    }
                }
                else
                {
                    int starting = d_offset;
                    for (int j = 0; j < strings[i].Count; j++) //for each letter
                    {
                        TextBlock tb = new TextBlock();
                        tb.Text = strings[i][j].ToString();
                        tb.SetValue(Grid.RowProperty, i + 1);
                        tb.SetValue(Grid.ColumnProperty, starting++);
                        tb.HorizontalAlignment = HorizontalAlignment.Center;
                        tb.VerticalAlignment = VerticalAlignment.Center;
                        grid.Children.Add(tb);

                        if (strings[i][j].ToString() == "D")
                        {
                            d_offset = j + 1 + offset;
                        }
                    }
                }
                offset++;
                //offset += strings[i].Count-5;
            }
        }

        public void ShowNonForwardingDiagram(List<List<string>> strings)
        {
            Grid diagram = NonForwardingDiagram;
            AddColumnsToDiagram(5, diagram);
            int offset = 0;
            for (int i = 0; i < strings.Count; i++) //for each command
            {
                AddColumnsToDiagram(strings[i].Count - 5 + 1, diagram);
                for (int j = 0; j < strings[i].Count; j++) //for each letter
                {
                    TextBlock tb = new TextBlock();
                    tb.Text = strings[i][j].ToString();
                    tb.SetValue(Grid.RowProperty, i + 1);
                    tb.SetValue(Grid.ColumnProperty, j + 1 + offset);
                    tb.HorizontalAlignment = HorizontalAlignment.Center;
                    tb.VerticalAlignment = VerticalAlignment.Center;
                    diagram.Children.Add(tb);

                }
                offset++;
                offset += strings[i].Count - 5;
            }
        }

        private void AddColumnsToDiagram(int num, Grid grid)
        {
            GridLengthConverter gridLengthConverter = new GridLengthConverter();

            for (int i = 0; i<num; i++)
            {
                //define new column
                ColumnDefinition a = new ColumnDefinition();
                a.Width = (GridLength)gridLengthConverter.ConvertFrom(50); ;

                //append column to grid
                grid.ColumnDefinitions.Insert(grid.ColumnDefinitions.Count, a);

                //create textblock
                TextBlock _s_ = new TextBlock();

                //number the column
                _s_.Text = (grid.ColumnDefinitions.Count-1).ToString();

                //set num to the top of the column
                _s_.SetValue(Grid.RowProperty, 0);

                //append column to end
                _s_.SetValue(Grid.ColumnProperty, grid.ColumnDefinitions.Count-1);

                //send text
                _s_.HorizontalAlignment = HorizontalAlignment.Center;

                //
                _s_.VerticalAlignment = VerticalAlignment.Center;

                //
                grid.Children.Add(_s_);
            }
        }

        private void AddRowsToDiagram(int num, Grid grid)
        {
            GridLengthConverter gridLengthConverter = new GridLengthConverter();

            for (int i = 0; i < num+1; i++)
            {
                //define new column
                RowDefinition a = new RowDefinition();
                a.Height = (GridLength)gridLengthConverter.ConvertFrom(100); ;

                //append column to grid
                grid.RowDefinitions.Insert(grid.RowDefinitions.Count, a);

                //create textblock
                TextBlock _s_ = new TextBlock();

                //number the column
                _s_.Text = grid.RowDefinitions.Count.ToString();

                //set num to the top of the column
                _s_.SetValue(Grid.RowProperty, grid.RowDefinitions.Count - 1);

                //append row in first column
                _s_.SetValue(Grid.ColumnProperty, 0);

                //send text
                _s_.HorizontalAlignment = HorizontalAlignment.Center;

                //
                _s_.VerticalAlignment = VerticalAlignment.Center;

                //
                grid.Children.Add(_s_);
            }
        }

        private void AddInstructionsToCanvas(string[] ints)
        {
            for (int i = 0; i < ints.Length; i++)
            {
                if (i > 3) //the grid has 4 rows by default
                {
                    AddRowsToDiagram(1, NonForwardingDiagram);
                    AddRowsToDiagram(1, ForwardingDiagram);
                }

                //Create a textbox and add it to each diagram
                NonForwardingDiagram.Children.Add(CreateInstructionTextBox(ints[i], i));
                ForwardingDiagram.Children.Add(CreateInstructionTextBox(ints[i], i));
            }

            //AddLabelsToDiagram();
        }

        private void AddLabelsToDiagram()
        {
            NonForwardingDiagram.Children.Add(CreateInstructionTextBox("No Forwarding\nDiagram", -1));
            ForwardingDiagram.Children.Add(CreateInstructionTextBox("Forwarding\nDiagram", -1));
        }

        public void ManifestHazards(List<HazardObject> objects)
        {
            TextBlock tb_inst;
            TextBlock tb_rs;
            TextBlock tb_rt;
            TextBlock tb_dynamic;
            for (int i = 0; i < objects.Count; i++)
            {
                HazardObject hObj = objects[i];
                tb_inst = CreateHazardText(hObj._inst.ToString(), hObj.inst__hazard);
                tb_rs = CreateHazardText(hObj.__rs.ToString(), hObj.rs__Hazard);
                tb_rt = CreateHazardText(hObj.__rt.ToString(), hObj.rt__Hazard);

                switch (hObj.instructionType)
                {
                    case InstructionType.rType:
                        if (hObj.__rTypeImmediateFlag)
                            tb_dynamic = CreateHazardText(hObj.__immediate.ToString(), HazardType.none);
                        else
                            tb_dynamic = CreateHazardText(hObj.__rd.ToString(), hObj.rd__Hazard);
                        break;
                    case InstructionType.iType:
                        tb_dynamic = CreateHazardText(hObj.__immediate.ToString(), HazardType.none);
                        break;
                    default:
                        throw new NotImplementedException();
                        //break;
                }

                StackPanel stackpanel = GetStackPanelFromTextboxes(tb_inst, tb_rs, tb_rt,tb_dynamic, hObj.instructionType);
                DependencyDiagramYAxis.Children.Add(stackpanel); //add stack panel containing objs
            }


        }

        private TextBlock CreateHazardText(string text, HazardType hT)
        {
            TextBlock tb = new TextBlock();
            if (hT != HazardType.none)
                tb.Background = GetColorFromEnum(hT);
            tb.Text = text;
            tb.HorizontalAlignment = HorizontalAlignment.Center;
            tb.VerticalAlignment = VerticalAlignment.Center;
            return tb;
        }

        private StackPanel GetStackPanelFromTextboxes(TextBlock instTB, 
            TextBlock rsTB, TextBlock rtTB, TextBlock dynamicTB,
            InstructionType iT)
        {
            StackPanel stackpanel = new StackPanel();
            TextBlock tb = new TextBlock();

            stackpanel.Orientation = Orientation.Horizontal;
            stackpanel.HorizontalAlignment = HorizontalAlignment.Center;
            stackpanel.Margin = new Thickness(5);
            stackpanel.Children.Add(instTB);
            stackpanel.Children.Add(CreateTextbox(" "));
            stackpanel.Children.Add(rsTB);
            stackpanel.Children.Add(CreateTextbox(", "));
            if (iT == InstructionType.iType)
            {
                stackpanel.Children.Add(dynamicTB);
                stackpanel.Children.Add(CreateTextbox("("));
                stackpanel.Children.Add(rtTB);
                stackpanel.Children.Add(CreateTextbox(")"));
            }
            else
            {
                stackpanel.Children.Add(rtTB);
                stackpanel.Children.Add(CreateTextbox(", "));
                stackpanel.Children.Add(dynamicTB);
            }
            return stackpanel;
        }

        private AccessText CreateTextbox(string s)
        {
            AccessText tb = new AccessText();
            tb.HorizontalAlignment = HorizontalAlignment.Center;
            tb.VerticalAlignment = VerticalAlignment.Center;
            tb.Text = s;
            return tb;
        }

        private Brush GetColorFromEnum(HazardType hazard)
        {
            switch (hazard)
            {
                case HazardType.structural:
                    return new SolidColorBrush(Colors.Yellow);
                case HazardType.data:
                    return new SolidColorBrush(Colors.Red);
                case HazardType.control:
                    return new SolidColorBrush(Colors.Blue);
                default: 
                    return new SolidColorBrush(Colors.Black);
            }
        }



        /*
         
                     if (nt == NodeType.nonLeaf)
            {
                obj.Background = new SolidColorBrush(Color.FromRgb(153, 255, 153)); //green
                TextBlock at = new TextBlock();
                at.HorizontalAlignment = HorizontalAlignment.Center;
                at.VerticalAlignment = VerticalAlignment.Center;
                at.Text = node.key; //display text
                obj.Children.Add(at);
            }
            else if (nt == NodeType.root)
            {
                obj.Background = new SolidColorBrush(Color.FromRgb(153, 255, 153)); //green
                TextBlock at = new TextBlock();
                at.HorizontalAlignment = HorizontalAlignment.Center;
                at.VerticalAlignment = VerticalAlignment.Center;
                at.Text = node.key; //display text
                obj.Children.Add(at);
            }
            else
            {//leaf
                obj.Background = new SolidColorBrush(Color.FromRgb(255, 204, 153)); //orange
         */
        private TextBlock CreateInstructionTextBox(string text, int rowNum)
        {
            TextBlock tb = new TextBlock();
            tb.Text = text;
            tb.Margin = new Thickness(30);
            tb.SetValue(Grid.RowProperty, rowNum + 1);
            tb.SetValue(Grid.ColumnProperty, 0);
            tb.HorizontalAlignment = HorizontalAlignment.Center;
            tb.VerticalAlignment = VerticalAlignment.Center;
            //tb.Background = new SolidColorBrush(Colors.White);
            return tb;
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