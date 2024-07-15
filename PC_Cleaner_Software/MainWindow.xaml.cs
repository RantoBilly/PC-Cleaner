using System;
using System.Collections.Generic;
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
using System.Diagnostics;
using System.IO;
using System.Net;

namespace PC_Cleaner_Software
{
    
    public partial class MainWindow : Window
    {
        string version = "1.0.0";
        public DirectoryInfo winTemp; // Informations of a folder which contains temporary files 
        public DirectoryInfo appTemp; //Informations of an app which can create temporary files

        public MainWindow()
        {
            InitializeComponent();
            winTemp = new DirectoryInfo(@"C:\Windows\Temp"); // path to the temporary files of Windows
            appTemp = new DirectoryInfo(System.IO.Path.GetTempPath()); // path to the apss's temporary files wherever is its location
            CheckNews();
            GetDate();
        }

        //GET NEWS from a text file via HTTP request
        public void CheckNews()
        {
            string url = "http://localhost/PC_clean/News.txt"; // text file on the local web server

            using(WebClient webclient = new WebClient()) //WebClient makes a HTTP request
            {
                string news = webclient.DownloadString(url);
                if(news != String.Empty)
                {
                    NewsTxt.Content = news;
                    NewsTxt.Visibility = Visibility.Visible; // NewsTxT label becomes visible
                    redContent.Visibility = Visibility.Visible; // redContent becomes visible
                }
            }
        }

        public void CheckVersion()
        {
            string v = "http://localhost/PC_clean/Version.txt";
            using(WebClient webclient = new WebClient())
            {
                string ver = webclient.DownloadString(v);
                if(ver != version)
                {
                    MessageBox.Show("New version available!", "Update",MessageBoxButton.OK,MessageBoxImage.Information);
                }else MessageBox.Show("This software is in the current version", "Update", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        public void OpenWebsite()
        {
            //The user may not have a browser on his PC
            try
            {
                // Open an URL from the default browser
                Process.Start(new ProcessStartInfo("http://localhost/PC_clean/PC_clean.html")
                {
                    UseShellExecute = true
                });;
            }catch(Exception ew)
            {
                Console.WriteLine(ew.Message);
            }
        }

        // Check the Size of a folder in order to do a test or not
        public long DirSize(DirectoryInfo dir)
        {
            return dir.GetFiles().Sum(f => f.Length) + dir.GetDirectories().Sum(di => DirSize(di));
            /*
              GetFiles() : get all the contained files in the folder
              Sum() : Sum for getting the number of the files in the folder
              GetDirectories().Sum() : Sum for getting the size of the files
                */
        }

        //Deletion function
        public void CleanTempData(DirectoryInfo di)
        {
            //file deletion
            foreach (FileInfo file in di.GetFiles())
            {
                try
                {
                    file.Delete(); //may fail in case of unauthorized file deletion
                    Console.WriteLine(file.FullName); // show the name of the deleted file
                }
                catch(Exception ex)
                {
                    continue;
                }
            }

            //folder deletion
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                try
                {
                    dir.Delete(true); // It's recursive which means that even delete the subfolders inside the folder
                    Console.WriteLine(dir.FullName);
                }
                catch(Exception ex)
                {
                    continue;
                }
            }
        }

        public void Test()
        {
            Console.WriteLine("Test starting");
            long totalSize = 0; //stock the total of clearable space
            totalSize += DirSize(winTemp) / 1000000; // represents the size in mega octet
            totalSize += DirSize(appTemp) / 1000000;

            try {
                space.Content = totalSize + " Mb"; // change the content of "space" label in the MainWindow
                date.Content = DateTime.Today; // change the content of "date" label in the MainWindow
            }
            catch(Exception ex)
            {
                Console.WriteLine("Impossible test "+ex.Message);
            }
            Title.Content = "Launched Test";
            SaveDate();       
        }

        /// <summary>
        /// Save the date in a text file
        /// </summary>
        public void SaveDate()
        {
            string date = DateTime.Today.ToString(); // convert the date into a string
            File.WriteAllText("date.txt", date);
        }

        public void GetDate()
        {
            string dateTXT = File.ReadAllText("date.txt");
            if (dateTXT != String.Empty)
            {
                date.Content = dateTXT;
            }
        }

        // CLEAN
        private void Clean_Button(object sender, RoutedEventArgs e)
        {
            cleanBTN.Content = "Cleaning ...";
            Clipboard.Clear(); // clean everything in the clipboard ("presse-papier")

            try
            {
                CleanTempData(winTemp);
            }
            catch (Exception e1)
            {
                Console.WriteLine("Error "+e1.Message);
            }

            try
            {
                CleanTempData(appTemp);
            }
            catch (Exception e2)
            {
                Console.WriteLine("Error "+e2.Message);
            }

            cleanBTN.Content = "Cleaned";
        }

        //UPDATE
        private void Update_Button(object sender, RoutedEventArgs e)
        {
            //The 2nd param is like the title of the MessageBox
            //MessageBox.Show("Your software is on its current version", "Update",MessageBoxButton.OK,MessageBoxImage.Information);
            CheckVersion();
        }

        //HISTORY
        private void History_Button(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Create history page", "History", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        //WEBSITE
        private void Website_Button(object sender, RoutedEventArgs e)
        {
            OpenWebsite();
        }

        private void LaunchTest_Button(object sender, RoutedEventArgs e)
        {
            Test();
        }
    }
}
