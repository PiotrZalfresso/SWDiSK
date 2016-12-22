using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Windows.Forms;
using System.IO;

namespace Symulator
{
    public static class Matrices
    {
        static long[,] distance;  //meters
        static long[,] time;     // seconds

        public static long[,] Distance
        {
            get
            {
                return distance;
            }

            set
            {
                distance = value;
            }
        }

        public static long[,] Time
        {
            get
            {
                return time;
            }

            set
            {
                time = value;
            }
        }

        static public void calcMatrices(DateTime departureTime, string apiKey, string HubAdress, string HubCity)
        {
            if (PackagesList.numberOfPackages > 0)
            {
                int infinity = -1;//tymczasowo -1,  mozna zmienic -> -1 = brak czasu -> brak trasy
                distance = new long[PackagesList.numberOfPackages + 1, PackagesList.numberOfPackages + 1];
                time = new long[PackagesList.numberOfPackages + 1, PackagesList.numberOfPackages + 1];


                for (int i = 0; i < PackagesList.numberOfPackages; i++) // wylicznie odleglosci i czasu pomiedzy adresami paczek
                {
                    for (int j = 0; j < PackagesList.numberOfPackages; j++)
                    {
                        string origin = PackagesList.packagesList[i].RecAdress + " " + PackagesList.packagesList[i].RecCity;
                        string destination = PackagesList.packagesList[j].RecAdress + " " + PackagesList.packagesList[j].RecCity;

                        if (i == j)
                        {
                            distance[i, j] = infinity; //tymczasowo -1, mozna zmienic
                            time[i, j] = infinity;     //tymczasowo -1, mozna zmienic
                        }
                        else
                        {
                            queryMap(departureTime, apiKey, origin, destination, i, j);

                        }
                    }
                }

                for (int i = 0; i < PackagesList.numberOfPackages; i++) // wylicznie odleglosci i czasu pomiedzy sortownia i adresami paczek
                {
                    queryMap(departureTime, apiKey, HubAdress + HubCity, 
                        PackagesList.packagesList[i].RecAdress + " " + PackagesList.packagesList[i].RecCity, i, PackagesList.numberOfPackages);
                }

                for (int j = 0; j < PackagesList.numberOfPackages; j++) // wylicznie odleglosci i czasu pomiedzy adresami paczek i sortownia
                {
                    queryMap(departureTime, apiKey, PackagesList.packagesList[j].RecAdress + " " + PackagesList.packagesList[j].RecCity,
                        HubAdress + HubCity, PackagesList.numberOfPackages, j );
                }

                distance[PackagesList.numberOfPackages, PackagesList.numberOfPackages] = infinity; 
                time[PackagesList.numberOfPackages, PackagesList.numberOfPackages] = infinity;
            }
        }
        private static void queryMap(DateTime departureTime, string apiKey, string origin, string destination, int i, int j)
        {
            string url;

            System.Threading.Thread.Sleep(400); // bo sie jebie bez tego -> powód nie nadąża z odp lub odp gdzies ginie -> ??rozwiazanie przejescie na async??

            if (apiKey.Count() > 0)
            {
                string depTime = ConvertToUnixTimestamp(departureTime).ToString();
                url = "http://maps.googleapis.com/maps/api/directions/json?origin=" + origin + "&destination=" + destination + "&departure_time=" + depTime + "&key=" + apiKey;
            }
            else
            {
                url = "http://maps.googleapis.com/maps/api/directions/json?origin=" + origin + "&destination=" + destination + "&sensor=false";
            }

            string content = fileGetContents(url);

            JObject o = JObject.Parse(content);
            try
            {
                if (o.SelectToken("status").ToString() == "OK")
                {
                    distance[i, j] = (int)o.SelectToken("routes[0].legs[0].distance.value");
                    time[i, j] = (long)o.SelectToken("routes[0].legs[0].duration.value");
                }
                else //do testowania
                {
                    var dlg = MessageBox.Show("Błąd dla " + PackagesList.packagesList[i].RecAdress
                   + " oraz " + PackagesList.packagesList[j].RecAdress, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch
            {
                var dlg = MessageBox.Show("Błąd tworzenia tablic", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private static double ConvertToUnixTimestamp(DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan diff = date.ToUniversalTime() - origin;
            return Math.Floor(diff.TotalSeconds);
        }

        static private string fileGetContents(string fileName)
        {
            string sContents = string.Empty;
            string me = string.Empty;
            try
            {
                if (fileName.ToLower().IndexOf("http:") > -1)
                {
                    System.Net.WebClient wc = new System.Net.WebClient();
                    byte[] response = wc.DownloadData(fileName);
                    sContents = System.Text.Encoding.ASCII.GetString(response);
                }
                else
                {
                    System.IO.StreamReader sr = new System.IO.StreamReader(fileName);
                    sContents = sr.ReadToEnd();
                    sr.Close();
                }
            }
            catch { sContents = "unable to connect to server "; }
            return sContents;
        }

        static public void readDistMatrixFormFile(string filepath)
        {
            string filename = Path.GetFileNameWithoutExtension(filepath);
            filename += "_dist";

            if (File.Exists(filename))
            {
                distance = new long[PackagesList.numberOfPackages + 1, PackagesList.numberOfPackages + 1];

                using (StreamReader sr = new StreamReader(filename))
                {
                    for (int i = 0; i < PackagesList.numberOfPackages + 1; i++) 
                    {
                        string line = sr.ReadLine();
                        string[] divLine= line.Split(' ');

                        for (int j = 0; j < PackagesList.numberOfPackages + 1; j++)
                        {
                            distance[i, j] = Int32.Parse(divLine[j]);
                        }
                    }
                }
            }
            else
            {
                var dlg = MessageBox.Show("Błąd: Nie znaleziono pliku " + filename, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        static public void readTimeMatrixFormFile(string filepath)
        {
            string filename = Path.GetFileNameWithoutExtension(filepath);
            filename += "_time";

            if (File.Exists(filename))
            {
                time = new long[PackagesList.numberOfPackages + 1, PackagesList.numberOfPackages + 1];

                using (StreamReader sr = new StreamReader(filename))
                {
                    for (int i = 0; i < PackagesList.numberOfPackages + 1; i++) 
                    {
                        string line = sr.ReadLine();
                        string[] divLine = line.Split(' ');

                        for (int j = 0; j < PackagesList.numberOfPackages + 1; j++)
                        {
                            time[i, j] = Int32.Parse(divLine[j]);
                        }
                    }
                }
            }
            else
            {
                var dlg = MessageBox.Show("Błąd: Nie znaleziono pliku " + filename, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        static public void exportDistMatrix(string filepath)
        {
            string filename = Path.GetFileNameWithoutExtension(filepath);
            filename += "_dist";

            using (StreamWriter sw = new StreamWriter(filename))
            {
                for (int i = 0; i < PackagesList.numberOfPackages + 1; i++) // wylicznie odleglosci i czasu pomiedzy adresami paczek
                {
                    for (int j = 0; j < PackagesList.numberOfPackages + 1; j++)
                    {
                        sw.Write(distance[i, j] + " ") ; 
                    }
                    sw.Write("\r");
                }

                sw.Close();
            }

        }

        static public void exportTimeMatrix(string filepath)
        {
            string filename = Path.GetFileNameWithoutExtension(filepath);
            filename += "_time";

            using (StreamWriter sw = new StreamWriter(filename))
            {
                for (int i = 0; i < PackagesList.numberOfPackages + 1; i++) // wylicznie odleglosci i czasu pomiedzy adresami paczek
                {
                    for (int j = 0; j < PackagesList.numberOfPackages + 1; j++)
                    {
                        sw.Write(time[i, j] + " ");
                    }
                    sw.Write("\r");
                }

                sw.Close();
            }

        }

    }
}
