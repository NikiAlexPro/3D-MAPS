using System;
using System.Net;
using System.IO;
using System.Globalization;
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
using Microsoft.Maps.MapControl.WPF;
using Alpinechough.Srtm;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Diagnostics;
using System.IO.Compression;
using Path = System.IO.Path;
using Microsoft.Win32;

namespace WPFmaps
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            myMap.Focus();                      //Zoom + - 
            myMap.Mode = new AerialMode(true);
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
            terrainVisual.Children.Add(visualChild);
            
            //Домен запущенного приложения
            //string directoryProgram = AppDomain.CurrentDomain.BaseDirectory;

        }

        public double startCoordinatePositionX, startCoordinatePositionY, endCoordinatePositionX, endCoordinatePositionY;
        private Rectangle rect;
        private Point startPoint;
        private string bingMapsAPIKey = "1nCxuVig4JDRhCn7Wrr2~m3d0O1f5n_vAmQaRx3OPfQ~AtKufP8nZg_HlYIWmcwsI7PK9CCIHCj6myftb3-lFSUZtPrOOjPFmMCdZeZyKX70";
        double H, W;
        ModelVisual3D visualChild = new ModelVisual3D();
        TerrainModel r = new TerrainModel();
        string imageMapName, imagePrevious;
        List<string> absentSRTMdata;
        string dataDirectory = "SrtmDataFiles";

        private void MouseMoved(object sender, MouseEventArgs e)
        {
            double mousePositionX = e.GetPosition(mainGrid).X;
            double mousePositionY = e.GetPosition(mainGrid).Y;
            labelCoordinate.Content = string.Format($"X:{mousePositionX}, Y:{mousePositionY}, Width:{W}, Height:{H}");

            if(e.RightButton == MouseButtonState.Released || rect == null)
            {
                return;
            }

            var pos = e.GetPosition(mainCanvas);
            var x = Math.Min(pos.X, startPoint.X);
            var y = Math.Min(pos.Y, startPoint.Y);

            var w = Math.Max(pos.X, startPoint.X) - x;
            var h = Math.Max(pos.Y, startPoint.Y) - y;

            //Для рисования квадрата используется переменная w
            //Чтобы убрать ограничичение - rect.Height = h;
            rect.Width = w;
            rect.Height = h;

            Canvas.SetLeft(rect, x);
            Canvas.SetTop(rect, y);
            H = h;
            W = w;
            
        }

        private void Button_ExportModel(object sender, RoutedEventArgs e)
        {
            //string directoryProgram = AppDomain.CurrentDomain.BaseDirectory;
            ObjExporter objExporter = new ObjExporter();
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "3D model files(*.obj)|*.obj";
            if (saveFileDialog.ShowDialog() == true)
            {
                var path = saveFileDialog.FileName;
                var filename = Path.GetFileName(path);    
                if (path == null)
                {
                    return;
                }
                using (Stream stream = File.Create(path))
                {
                    objExporter.MaterialsFile = Path.ChangeExtension(path, ".mtl");
                    objExporter.Export(visualChild, stream);
                }
                visualChild.Content = null;
            }
        }

        private void MouseRightButtonDownE(object sender, MouseButtonEventArgs e)
        {
            
            Point mousePosition = e.GetPosition(this);
            startPoint = e.GetPosition(mainCanvas);
            Location pinLocation = myMap.ViewportPointToLocation(mousePosition);
            startCoordinatePositionY = pinLocation.Latitude;
            startCoordinatePositionX = pinLocation.Longitude;
            
            rect = new Rectangle
            {
                Stroke = Brushes.LightBlue,
                StrokeThickness = 2
            };

            Canvas.SetLeft(rect, startPoint.X);
            Canvas.SetTop(rect, startPoint.Y);
            mainCanvas.Children.Add(rect);
            ///////////////////////////////////////
                
        }

        private async void MouseRightButtonUpE(object sender, MouseButtonEventArgs e)
        {
            rect = null;
            mainCanvas.Children.Clear();
            Point mousePosition = e.GetPosition(this);
            Location pinLocation = myMap.ViewportPointToLocation(mousePosition);
            endCoordinatePositionY = pinLocation.Latitude;
            endCoordinatePositionX = pinLocation.Longitude;
            MessageBox.Show($"{startCoordinatePositionY}, {startCoordinatePositionX}, {endCoordinatePositionY}, {endCoordinatePositionX}, Height={H}, Width={W}");

            string separator = ",";
            string mapArea = String.Concat(endCoordinatePositionY, separator, startCoordinatePositionX, separator, startCoordinatePositionY, separator, endCoordinatePositionX);

            //Обработка выделения
            if (mousePosition.X < startPoint.X || mousePosition.Y < startPoint.Y)
                return;
            //Обработка площади выделения!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

            if (visualChild.Content != null)
            {
                visualChild.Content = null;
                imagePrevious = imageMapName;
            }

            await DownloadTextureFile(mapArea, bingMapsAPIKey);
            absentSRTMdata = CheckSRTMdata.FindLoseSRTMdata(startCoordinatePositionY, startCoordinatePositionX, endCoordinatePositionY, endCoordinatePositionX);
            await DownloadSRTMData(absentSRTMdata, dataDirectory);
            //if (imagePrevious != null)
                //File.Delete(imagePrevious);
        }

        private async Task DownloadTextureFile(string mapCoordinate, string apiKey)
        {
            MAPmodel.IsEnabled = false;
            WindowProgressDownload windowProgressDownload = new WindowProgressDownload();
            windowProgressDownload.textblockfilename.Text = "Загрузка файла: Текстура Геополя";
            // данные передавать Снизу-вверх, Слева-Направо
            WebClient webClient = new WebClient();
            Stopwatch sw = new Stopwatch();
            sw.Start();

            imageMapName = "Image" + mapCoordinate + ".png";

            webClient.DownloadProgressChanged += (s, e) =>
            {
                windowProgressDownload.progressBar.Value = e.ProgressPercentage;
                windowProgressDownload.textblockSpeedByteRecived.Text = "Скорость загрузки: " + ((Convert.ToDouble(e.BytesReceived) / 1024) / sw.Elapsed.TotalSeconds).ToString("0.00") + " КБ/с" + " // " + " Размер файла: " + (Convert.ToDouble(e.BytesReceived) / 1024 / 1024).ToString("0.00") + " МБ" + "  /  " + (Convert.ToDouble(e.TotalBytesToReceive) / 1024 / 1024).ToString("0.00") + " МБ";
            };

            webClient.DownloadFileCompleted += (s, e) =>
            {
                windowProgressDownload.Close();
                sw.Stop();
            };
            
            windowProgressDownload.Show();


            try
            {
                await webClient.DownloadFileTaskAsync($"https://dev.virtualearth.net/REST/v1/Imagery/Map/Aerial?mapArea={mapCoordinate}&mapSize=1500,1500&dpi=Large&mapLayer=Basemap,Buildings&key={apiKey}", imageMapName);
                
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }

        private async Task DownloadSRTMData(List<string> loseSRTMdata, string dataDirectory)
        {
            
            bool fileStatus = true;
            string filePath = null;
            Stopwatch sw = new Stopwatch();
            
            foreach (var filename in loseSRTMdata)
            {
                sw.Start();
                WindowProgressDownload windowProgressDownload = new WindowProgressDownload();
                windowProgressDownload.textblockfilename.Text = "Загрузка файла: " + filename + " // " + (loseSRTMdata.IndexOf(filename) + 1) + " из " + loseSRTMdata.Count;
                
                filePath = Path.Combine(dataDirectory, filename);
                WebClient webClient = new WebClient();
                webClient.DownloadProgressChanged += (s, e) =>
                {
                    windowProgressDownload.progressBar.Value = e.ProgressPercentage;
                    windowProgressDownload.textblockSpeedByteRecived.Text = "Скорость загрузки: " + ((Convert.ToDouble(e.BytesReceived) / 1024) / sw.Elapsed.TotalSeconds).ToString("0.00") + " КБ/с" + " // " + " Размер файла: " + (Convert.ToDouble(e.BytesReceived) / 1024 / 1024).ToString("0.00") + " МБ" + "  /  " + (Convert.ToDouble(e.TotalBytesToReceive) / 1024 / 1024).ToString("0.00") + " МБ";
                };
                webClient.DownloadFileCompleted += (s, e) =>
                {
                    if(e.Error == null)
                    {
                        sw.Stop();
                        windowProgressDownload.Close();

                    }
                };
                //WebClient и т.д.
                //Метод GetSRTMdata();
                string continent = null;
                int counter = 0;
                bool isDowloading = false;
                windowProgressDownload.Show();
                while (isDowloading != true)
                {
                    try
                    {
                        
                        await webClient.DownloadFileTaskAsync($"https://dds.cr.usgs.gov/srtm/version2_1/SRTM3/{continent}/{filename}.zip", filePath + ".zip");
                        isDowloading = true;
                        fileStatus = true;
                        webClient.Dispose();

                    }
                    catch(Exception ex)
                    {
                        counter++;
                        if (counter == 1)
                            continent = "Africa";
                        else if (counter == 2)
                            continent = "Australia";
                        else if (counter == 3)
                            continent = "Eurasia";
                        else if (counter == 4)
                            continent = "Islands";
                        else if (counter == 5)
                            continent = "North_America";
                        else if (counter == 6)
                            continent = "South_America";
                        else
                        {
                            fileStatus = false;
                            windowProgressDownload.Close();
                            MessageBox.Show("Данные в выделенной области отсутствуют // " + ex.Message);
                            break;

                        }

                    }


                }
                if (fileStatus == true)
                {
                    ExtractZippedFile(filePath + ".zip");
                }
                else
                    break;
            }
            if (fileStatus == true)
                GetSRTMdata();
        }

        private void ExtractZippedFile(string source)
        {
            ZipFile.ExtractToDirectory(source, dataDirectory);
            File.Delete(source);

        }

        private void GetSRTMdata()
        {
            SrtmData srtmData = new SrtmData("SrtmDataFiles");
            //Вызывать метод для каждой точки!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

            //double latitude = -2.9144655987943375;
            //double longitude = 37.30522969895874;

            double latitude = startCoordinatePositionY;
            double longitude = startCoordinatePositionX;
            double latitudeEnd = endCoordinatePositionY;
            double longitudeEnd = endCoordinatePositionX;

            double tempLon = longitude;
            double tempLat = latitude;
            //int width = (int)W;
            //int height = (int)H;



            double differenceLat = Math.Abs(latitude - latitudeEnd);
            double differenceLon = Math.Abs(longitude - longitudeEnd);

            int Distance(double startLatitude, double startLongitude, double endLatitude, double endLongitude)
            {
                double R = 6371;
                double d = (Math.Sin((Math.PI * startLatitude) / 180) * Math.Sin((Math.PI * endLatitude) / 180)) + ((Math.Cos((Math.PI * startLatitude) / 180) * Math.Cos((Math.PI * endLatitude) / 180)) * (Math.Cos((Math.PI * (startLongitude - endLongitude)) / 180)));
                double L = Math.Acos(d) * R;
                return (int)(L * 10);
            }

            


            int width = Distance(latitude, longitude, latitude, longitudeEnd);
            int height = Distance(latitude, longitude, latitudeEnd, longitude);


            int?[,] elevations = new int?[width, height];
            List<Point3D> point = new List<Point3D>(width * height);

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {

                    int? elev = srtmData.GetElevation(new GeographicalCoordinates(latitude, longitude));
                    
                    point.Add(new Point3D(i, j, ((double)elev / 100)));
                    
                    longitude += ((differenceLon) / (width)); //0.001 //0.0001 при 1000 height
                }

                latitude -= ((differenceLat) / (height)); //0.0001
                longitude = tempLon;
            }





            double maxElevation = 0;
            double minElevation = 100;
            
            
            for(int i = 0; i < point.Count; i++)
            {
                if(point[i].Z == 0 && i > 1)
                {
                    var a = point[i];
                    a.Z = point[i-1].Z;
                    point[i] = a;
                }
                if (point[i].Z > maxElevation)
                    maxElevation = point[i].Z;
                else if(point[i].Z < minElevation)
                    minElevation = point[i].Z;

            }
            
            for (int i = 0; i < point.Count; i++)
            {
                    var a = point[i];
                    a.Z = point[i].Z - minElevation;
                    a.X = point[i].X - (height / 2);
                    a.Y = point[i].Y - (width / 2);
                    point[i] = a;
            }

            
            

            r.Texture = new MapTexture(imageMapName);
            visualChild.Content = r.CreateModel(point, height, width);
            MAPmodel.IsEnabled = true;

        }

    }
}
