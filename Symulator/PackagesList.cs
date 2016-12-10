using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Symulator
{
    public static class PackagesList
        //TODO czy to mam sens?
    {
        static public List<Package> packagesList = new List<Package>();
        static public int numberOfPackages = 0;

        static public void readFromFile (string filePath)
        {
            try
            {   // Open the text file using a stream reader.
                using (StreamReader sr = new StreamReader(filePath))
                {
                    numberOfPackages = Int32.Parse(sr.ReadLine());

                    for (int i = 0; i < numberOfPackages; i++)
                    {
                        string id = sr.ReadLine();       
                        string recName = sr.ReadLine(); 
                        string recAdress = sr.ReadLine();
                        string recZipCode = sr.ReadLine();
                        string recCity = sr.ReadLine();

                        string lineDate = sr.ReadLine();
                        Nullable<DateTime> recTimeFrom;
                        if (lineDate != "null")
                        {
                            recTimeFrom = DateTime.Parse(lineDate);
                        }
                        else
                        { 
                            recTimeFrom = null;
                        }

                        lineDate = sr.ReadLine();
                        Nullable<DateTime> recTimeTo;

                        if (lineDate != "null")
                        {
                            recTimeTo = DateTime.Parse(lineDate);
                        }
                        else
                        {  
                            recTimeTo = null;
                        }
                        string recTelNum = sr.ReadLine();
                        packageSize size = (packageSize)Enum.Parse(typeof(packageSize), sr.ReadLine());

                        Package pkg = new Package(i, id, recName, recAdress, recZipCode, recCity, recTimeFrom, recTimeTo, recTelNum, size);

                        packagesList.Add(pkg);
                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
        }

        static public void exportSolutionToFile(string filePath)
        {

        }
    }
}
