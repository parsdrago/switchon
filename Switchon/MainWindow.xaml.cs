using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Switchon
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = new PrefList();
        }
    }
}
public class PrefList
{
    public ObservableCollection<string> Data { get; }
    public PrefList()
    {
        Data = new ObservableCollection<string>();
        
        foreach (Process p in Process.GetProcesses())
        {
            if (p.MainWindowTitle.Length != 0)
            {
                Data.Add($"{p.ProcessName} - {p.MainWindowTitle}");
            }
        }
    }
}