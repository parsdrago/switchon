using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Switchon
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<string> allWindows;
        private List<IntPtr> windowHandles;

        public MainWindow()
        {
            InitializeComponent();

            (allWindows, windowHandles) = GetWindowList();
            DataContext = ObservableCollectionFromList(allWindows);
            SearchBox.Focus();

            SelectFirstItem();
        }

        private (List<string>, List<IntPtr>) GetWindowList()
        {
            var windowNames = new List<string>();
            var windowHandles = new List<IntPtr>();

            foreach (Process p in Process.GetProcesses())
            {
                if (p.MainWindowTitle.Length != 0)
                {
                    windowNames.Add($"{p.ProcessName} - {p.MainWindowTitle}");
                    windowHandles.Add(p.MainWindowHandle);
                }
            }

            return (windowNames, windowHandles);
        }

        private static ObservableCollection<T> ObservableCollectionFromList<T>(List<T> list)
        {
            var result = new ObservableCollection<T>();

            foreach (var l in list)
            {
                result.Add(l);
            }
            return result;
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var searchString = (sender as TextBox).Text.ToLower();

            var selectedList = ObservableCollectionFromList(allWindows.Where(w => w.ToLower().Contains(searchString)).ToList());

            DataContext = selectedList;
            SelectFirstItem();
        }

        private void SelectFirstItem()
        {
            if (!WindowList.Items.IsEmpty)
            {
                WindowList.SelectedItem = WindowList.Items[0];
            }
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsIconic(IntPtr hWnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private static void ActivateWindow(IntPtr windowHandle)
        {
            if (IsIconic(windowHandle))
            {
                ShowWindow(windowHandle, 9);
            }
            SetForegroundWindow(windowHandle);
        }

        private void WindowList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ActivateSelectedWindow();
            }
        }

        private void SearchBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (WindowList.Items.Count == 1)
                {
                    ActivateSelectedWindow();
                }
                SearchBox.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
        }

        private void ActivateSelectedWindow()
        {
            var handle = Enumerable.Zip(allWindows, windowHandles).Where(w => w.First == (WindowList.SelectedItem as string)).First().Second;
            ActivateWindow(handle);
            Application.Current.Shutdown();
        }
    }
}
