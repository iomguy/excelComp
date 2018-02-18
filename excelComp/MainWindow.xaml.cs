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
using System.IO;
using compare2Excels;

namespace excelComp
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string[] FileNames_OldResults { get; private set; }
        public string FileName_NewResult     { get; private set; }
        public string FoldName_OldResults  { get; private set; }
        public string FoldName_NewResult   { get; private set; }

        public ExcelData[] Data_OldResults { get; private set; }
        public ExcelData Data_NewResult    { get; private set; }

        public MainWindow()
        {
            InitializeComponent();

            TextBox_Path_OldResullts.IsReadOnly = true;             ///в окошко записи пути к файлу нельзя самому писать
            TextBox_Path_OldResullts.IsReadOnlyCaretVisible = true; ///но в окошке записи пути к файлу можно ставить каретку
            TextBox_Path_OldResullts.IsEnabled = false;             ///пока файл не выбран, окошко записи пути отключено

            TextBox_Path_NewResullts.IsReadOnly = true;             ///в окошко записи пути к файлу нельзя самому писать
            TextBox_Path_NewResullts.IsReadOnlyCaretVisible = true; ///но в окошке записи пути к файлу можно ставить каретку
            TextBox_Path_NewResullts.IsEnabled = false;             ///пока файл не выбран, окошко записи пути отключено
        }

        private void openButton_OldResults_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Excel |*.xlsx; *.xls";
            
            dialog.Multiselect = true;
            dialog.Title = "Выберите файлы со старыми результатами";
            
            if (!((bool)dialog.ShowDialog(this)))
                return;

            FileNames_OldResults = dialog.FileNames;

            this.FoldName_OldResults = System.IO.Path.GetDirectoryName(FileNames_OldResults[0]); ///записываем путь к папке для, например, первого файла из списка, т.к. предполагается, что мы берём файлы из одной папки

            TextBox_Path_OldResullts.IsEnabled = true; ///активируем окошко с путём к файлу
            TextBox_Path_OldResullts.Text = this.FoldName_OldResults; ///записываем путь к папке в textbox 

            int l = FileNames_OldResults.Length;
            Data_OldResults = new ExcelData[l];
            
            for (int i = 0; i < l; i++)
            {
                ExcelData Data_OldResult = new ExcelData(FileNames_OldResults[i]);
                Data_OldResults[i] = Data_OldResult;
            }

        }

        private void openButton_NewResults_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();            
            dialog.Filter = "Excel |*.xlsx; *.xls";

            dialog.Multiselect = false;
            dialog.Title = "Выберите один файл с новыми результатами";

            if (!((bool)dialog.ShowDialog(this)))
                return;

            FileName_NewResult = dialog.FileName;

            this.FoldName_NewResult = System.IO.Path.GetDirectoryName(FileName_NewResult); ///записываем путь к папке для, например, первого файла из списка, т.к. предполагается, что мы берём файлы из одной папки

            TextBox_Path_NewResullts.IsEnabled = true; ///активируем окошко с путём к файлу
            TextBox_Path_NewResullts.Text = this.FoldName_NewResult; ///записываем путь к папке в textbox 

            Data_NewResult = new ExcelData(FileName_NewResult);
        }

        private void fileTypeCombo_OldResults_SelectionChanged(object sender, SizeChangedEventArgs e)
        {

        }

        private void openButton_CompareResults_Click(object sender, RoutedEventArgs e)
        {
            string outp_path = this.FoldName_NewResult;
            DateTime thisDayTime = DateTime.Now;           

            string writeFilePath = String.Format("{0}\\{1}_compared.csv", outp_path, thisDayTime.ToString("MM.dd.yy_HH.mm"));
            StreamWriter csv = new System.IO.StreamWriter(writeFilePath, true, System.Text.Encoding.GetEncoding(1251)); // Win-кодировка
            //csv.WriteLine("File №; EulHar; b0; b1; b2"); // заголовок. разделитель ; может быть другим
            csv.WriteLine(String.Format("Новый файл результатов:; {0}", Data_NewResult.filename));

            foreach (ExcelData data_OldResult in this.Data_OldResults)
            {
                csv.WriteLine(String.Format("Сравниваем с:; {0}", data_OldResult.filename));
                csv.WriteLine(String.Format("; {0}% совпадают", Compare.percentOfDuplicates(Data_NewResult, data_OldResult)));
            }

            csv.Close();
            statusBarMessage.Content = "Сравнение записано в .csv-файл";
        }
    }
}
