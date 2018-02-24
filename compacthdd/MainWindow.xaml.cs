using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using Microsoft.Win32;
using System.Windows.Media;

namespace compacthdd
{
    public class HDD
    {
        public String UUID;
        public String Location;
        public int Size;

        public HDD(String uuid, String location, int size) 
        {
            UUID = uuid;
            Location = location;
            Size = size;
        }
    }

    public partial class MainWindow : Window
    {
        public static string Filepath2Binary = "C:\\Program Files\\Oracle\\VirtualBox\\VBoxManage.exe";

        private static List<HDD> hdds = new List<HDD>();

        public MainWindow()
        {
            InitializeComponent();
            BinExists();
            FillListBox();
        }

        private void BinExists()
        {
            if (!File.Exists(Filepath2Binary))
            // If VBoxManage.exe doesnr exist on default location
            {
                MessageBoxResult result = MessageBox.Show("VBoxManage binary can't be found!\nWould you like to locate it?", "Warning!", MessageBoxButton.OKCancel, MessageBoxImage.Exclamation);

                switch (result)
                {
                    case MessageBoxResult.OK:
                        // Browse for file
                        FindFile();
                        if (!Filepath2Binary.Substring((Filepath2Binary.Length - 14), 14).Equals("VBoxManage.exe"))
                        {
                            FindFile();
                        }
                        break;
                    case MessageBoxResult.Cancel:
                        // Exit app
                        this.Close();
                        break;
                }
            }
        }

        private String FindFile()
        {
            // Get filename from openfiledialog
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == true)
            {
                Filepath2Binary = ofd.FileName;
                return ofd.FileName;
            }
            return "";
        }

        private void FillListBox()
        {
            GetUUIDs();
            foreach (HDD item in hdds)
            {
                Box.Items.Add(item.Location);
                BoxR.Items.Add(item.Location);
            }
        }
        
        public HDD GetByLocation(String location)
        {
            for (int i = 0; i < hdds.Count; i++)
            {
                if (hdds[i].Location.ToString() == location)
                {
                    return hdds[i];
                }
            }
            return null;
        }

        private List<HDD> GetUUIDs()
        {
            hdds.Clear();
            String Uuid = "";
            String Location = "";
            int size = 0;

            Process P = new Process();
            P.StartInfo.FileName = Filepath2Binary;
            P.StartInfo.Arguments = "list hdds";
            P.StartInfo.UseShellExecute = false;
            P.StartInfo.CreateNoWindow = true;
            P.StartInfo.RedirectStandardOutput = true;
            P.OutputDataReceived += new DataReceivedEventHandler((sender, e) =>
            {
                if (!String.IsNullOrEmpty(e.Data))
                {
                    Boolean Both = false;


                    if (e.Data.Substring(0, 4).Contains("UUID"))
                    {
                        Uuid = e.Data.Substring(16, 36);
                    }

                    if (e.Data.Substring(0, 8).Contains("Location"))
                    {
                        Location = e.Data.Substring(16);
                    }

                    if (e.Data.Substring(0, 8).Contains("Capacity"))
                    {
                        String tmp = e.Data.Substring(16);
                        size = Int32.Parse(tmp.Substring(0, tmp.Length - 7));
                        Both = true;
                    }

                    if (Both)
                    {
                        hdds.Add(new HDD(Uuid, Location, size));
                        Both = false;
                    }
                }
            });

            P.Start();
            P.BeginOutputReadLine();
            P.WaitForExit();
            P.Close();

            return hdds;
        }

        private void Compact(HDD hdd)
        {
            Process P = new Process();
            P.StartInfo.FileName = Filepath2Binary;
            String args = "modifymedium " + hdd.UUID + " --compact";
            P.StartInfo.Arguments = args;
            P.StartInfo.UseShellExecute = false;

            P.Start();
            P.WaitForExit();
            P.Close();
        }

        private void Resize(HDD hdd)
        {
            Process P = new Process();
            P.StartInfo.FileName = Filepath2Binary;
            String args = "modifymedium " + hdd.UUID + " --resize " + Int32.Parse(DesiredSize.Text);
            P.StartInfo.Arguments = args;
            P.StartInfo.UseShellExecute = false;

            P.Start();
            P.WaitForExit();
            P.Close();
        }

        private void ButtonCompact_Click(object sender, RoutedEventArgs e)
        // Code for event handler on Compact button
        {
            if (Box.SelectedItem != null)
            {
                String location = (Box.SelectedItem).ToString();
                Compact(GetByLocation(location));
            }
        }

        private void TextBox_TextChanged_1(object sender, RoutedEventArgs e)
        {
        }

        private void Box_SelectionChanged_1(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
        }

        private void Box_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void TextBox_TextChanged_2(object sender, TextChangedEventArgs e)
        {

        }

        private void ButtonResize_Click(object sender, RoutedEventArgs e)
        // Code for event handler on Resize button
        {
            if (Box.SelectedItem != null)
            {
                String location = (BoxR.SelectedItem).ToString();

                Resize(GetByLocation(location));
            }
        }

        private void BoxR_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Change value in TextBox - DesiredSize
            if (BoxR.SelectedItem != null)
                {
                String location = (BoxR.SelectedItem).ToString();
                DesiredSize.Text = GetByLocation(location).Size.ToString();
            }
        }

        private void ClearAllListBoxes()
        {
            Box.Items.Clear();
            BoxR.Items.Clear();
        }

        private void SetFirstLineSelected()
        {
            Box.SelectedIndex = 0;
            BoxR.SelectedIndex = 0;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ClearAllListBoxes();
            FillListBox();
            SetFirstLineSelected();
        }

        private void Refresh_MouseEnter(object sender, RoutedEventArgs e)
        {
        }

        private void Refresh_MouseLeave(object sender, RoutedEventArgs e)
        {
        }
    }
}
