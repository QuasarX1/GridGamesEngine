using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Xml;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace WindowsLauncher
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HomePage : Page
    {
        private string gamesDirectory;
        private List<string> gamesFolders = new List<string>();

        public HomePage()
        {
            this.InitializeComponent();
            
            //Windows.Storage.ApplicationDataContainer roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
            Windows.Storage.StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            
            //Windows.Storage.StorageFile sampleFile = roamingFolder.CreateFileAsync("dataFile.txt", Windows.Storage.CreationCollisionOption.OpenIfExists).AsTask().Result;
            //Windows.Storage.FileIO.WriteTextAsync(sampleFile, "String").AsTask();






            // Get the filepath to the application's data
            string folderpath = localFolder.Path;

            //if (Windows.Storage.ApplicationData.Current.LocalSettings.Values.Keys.Contains("Folderpath"))
            //{
            //    folderpath = (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["Folderpath"];
            //}

            // Check application's data folder exists and create if it dosen't
            gamesDirectory = folderpath + "\\GridGamesEngine";

            if (!Directory.Exists(gamesDirectory))
            {
                 Directory.CreateDirectory(gamesDirectory);
            }

            // Get all of the folders in the selected game directory
            string[] folders = Directory.GetDirectories(gamesDirectory);

            // Validate each game folder ensuring it contains the nessessary files
            foreach (string folder in folders)
            {
                string[] gameFiles = Directory.GetFiles(folder);
                if (/*((Path.GetExtension(gameFiles[0]) == ".xml" && Path.GetExtension(gameFiles[1]) == ".dll") || (Path.GetExtension(gameFiles[0]) == ".dll" && Path.GetExtension(gameFiles[1]) == ".xml")) && */Directory.EnumerateFiles(folder).Any(f => f.Contains("_gamedata.xml")) && Directory.EnumerateFiles(folder).Any(f => f.Contains("_gametypes.dll")))
                {
                    gamesFolders.Add(folder);
                }
            }

            // Exit full screen mode in case returning from a game window
            Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().ExitFullScreenMode();

            // Display a link for each game
            int x = 0;
            int y = 0;
            foreach (string gameFolder in gamesFolders)
            {
                Image image = new Windows.UI.Xaml.Controls.Image();

                image.Name = gameFolder;
                Grid.SetColumn(image, x);
                Grid.SetRow(image, y);
                image.HorizontalAlignment = HorizontalAlignment.Stretch;
                image.VerticalAlignment = VerticalAlignment.Stretch;
                image.Stretch = Stretch.Uniform;

                //Subscribe to the tapped event handeler
                image.Tapped += Image_Tapped;


                image.Source = new BitmapImage(new Uri(gameFolder + "\\gameimage.jpg", UriKind.Absolute));

                GamesGrid.Children.Add(image);


                x++;
                if (x > 7)
                {
                    x = 0;
                    y++;
                }
                
                if (y > 4)
                {
                    // DO somthing
                }
            }
        }

        private void Image_Tapped(object sender, TappedRoutedEventArgs e)
        {
            XmlDocument gameData = new XmlDocument();
            gameData.Load(((Image)sender).Name + "\\" + Path.GetFileName(((Image)sender).Name) + "_gamedata.xml");
            //// Not alowed to load DLLs!
            //Assembly types = Assembly.LoadFrom(((Image)sender).Name + "\\" + Path.GetFileName(((Image)sender).Name) + "_gametypes.dll");

            Frame.Navigate(typeof(GamePage), gameData);
        }

        private void Button_Tapped(object sender, TappedRoutedEventArgs e)
        {

        }
    }
}
