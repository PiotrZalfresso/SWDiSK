using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Symulator
{
    public partial class MainForm : Form
    {
        Boolean readPackages;
        Boolean readMatrix;
      
        string filename;

        public MainForm()
        {
            InitializeComponent();
            OptBtn.Enabled = readPackages = false;
            ExportDistMatrixBtn.Enabled = ExportTimeMatrixBtn.Enabled = 
                ShowDistMatrixBtn.Enabled = ShowTimeMatrixBtn.Enabled = ExportSolutionBtn.Enabled =
                    ShowRouteBtn.Enabled = readMatrix = false;

        }

        private void readDlgBtn_Click(object sender, EventArgs e)
        {
            using (var dlg = new OpenFileDialog())
            {
                dlg.Multiselect = false;
                dlg.Filter = "Text files (*.txt)|*.txt";
                dlg.InitialDirectory = Directory.GetCurrentDirectory();
                if (dlg.ShowDialog() == DialogResult.OK)
                    pathFileTb.Text = dlg.FileName;
                
            }
        }

        private void readBtn_Click(object sender, EventArgs e)
        {
            filename = pathFileTb.Text;

            if (pathFileTb.Text.Length != 0 && File.Exists(filename))
            {
                StatusLbl.Text = "Wcztywanie pliku";
                PackagesList.readFromFile(filename);
                OptBtn.Enabled = readPackages = true;
                listViewRefresh();
                StatusLbl.Text = "";
            }
            else
            {
                var dlg = MessageBox.Show("Błąd: Nieprawidłówa śćieżka do pliku", "Błąd", MessageBoxButtons.OK,MessageBoxIcon.Error);
                    
            }
        }

        private void listViewRefresh()
        {
            for(int i=0; i< PackagesList.numberOfPackages; i++)
            {
                ListViewItem listitem = new ListViewItem(PackagesList.packagesList[i].Id);
                listitem.SubItems.Add(PackagesList.packagesList[i].RecName);
                listitem.SubItems.Add(PackagesList.packagesList[i].RecAdress);
                listitem.SubItems.Add(PackagesList.packagesList[i].RecZipCode);
                listitem.SubItems.Add(PackagesList.packagesList[i].RecCity);
                listitem.SubItems.Add(PackagesList.packagesList[i].RecTelNum);
                listitem.SubItems.Add(PackagesList.packagesList[i].RecTimeFrom.ToString());
                listitem.SubItems.Add(PackagesList.packagesList[i].RecTimeTo.ToString());
                listitem.SubItems.Add(PackagesList.packagesList[i].Size.ToString());
                inputDataLv.Items.Add(listitem);

            }

        }

        private void OptBtn_Click(object sender, EventArgs e)
        {
            string filename = pathFileTb.Text;

            if (hubAdressTb.Text == "" && hubCityTb.Text == "")
            {
                var dlg = MessageBox.Show("Błąd: Brak adresu sortowni", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (reabTablesFromFile.Checked == true)
            {
                StatusLbl.Text = "Wcztywanie tablic czasu i dystansu";
                Matrices.readDistMatrixFormFile(filename);
                Matrices.readTimeMatrixFormFile(filename);
                StatusLbl.Text = "";

                ExportDistMatrixBtn.Enabled = ExportTimeMatrixBtn.Enabled
               = ShowDistMatrixBtn.Enabled = ShowTimeMatrixBtn.Enabled = readMatrix = true;
            }
            else
            {
                StatusLbl.Text = "Generwonie tablic czasu i dystansu";
                Matrices.calcMatrices(CalcTimeDtp.Value, apKeyTb.Text, hubAdressTb.Text, hubCityTb.Text);
                StatusLbl.Text = "";
                ExportDistMatrixBtn.Enabled = ExportTimeMatrixBtn.Enabled
               = ShowDistMatrixBtn.Enabled = ShowTimeMatrixBtn.Enabled = readMatrix = true;
            }

            if (distanceOptimization.Checked) {
                if (simulatedAnnealing.Checked)
                    CalculateDistanceOptimizationSA();
                else
                    throw new InvalidOperationException("Nie wskazano algorytmu");
            }
            else if (timeOptimization.Checked)
            {
                throw new NotImplementedException();
            }
            else
            {
                throw new InvalidOperationException("Nie wskazano typu optymalizacji");
            }
            
            //TODO run algorithm here
        }

        private void CalculateDistanceOptimizationSA()
        {
            Dictionary<packageSize, int> sizeMap = GetPackageSizeMapping();
            int carCapacity = Int32.Parse(carCapTb.Text);
            int carNumber = Int32.Parse(carNumTb.Text);

            int neighbourhoodSize = Int32.Parse(carCapTb.Text) / Int32.Parse(pckSmSizeTb.Text);
            int carsNumber = Int32.Parse(carNumTb.Text);

            int carId = 0;
            while (Graph.numberOfDelivered < PackagesList.numberOfPackages) {
                if ((PackagesList.numberOfPackages - Graph.numberOfDelivered) < neighbourhoodSize)
                {
                    neighbourhoodSize = PackagesList.numberOfPackages - Graph.numberOfDelivered;
                }

                int[] toDeliver = Tsp.getNeighbors(neighbourhoodSize);
                Console.Write("Rozważamy sąsiedztwo: ");
                for (int i = 0; i < neighbourhoodSize; i++)
                {
                    Console.Write(toDeliver[i].ToString() + " ");
                }
                Console.WriteLine();

                Knapsack packageSelector = new Knapsack(toDeliver, sizeMap, carCapacity);
                SimulatedAnnealing algoritm = new SimulatedAnnealing(1000, 0.1, 100);
                int[] finalToDeliver = algoritm.Calculate(packageSelector);

                Console.Write(String.Format("Samochód {0} dostarczy do punktów: ", carId));
                for (int i = 0; i < finalToDeliver.Length; i++)
                {
                    Console.Write(finalToDeliver[i].ToString() + " ");
                }
                Console.WriteLine();

                Tsp pointsSorter = new Tsp(finalToDeliver);
                int[] solution = algoritm.Calculate(pointsSorter);

                Console.Write("W kolejności: ");
                for (int i = 0; i < solution.Length; i++)
                {
                    Console.Write(solution[i].ToString() + " ");
                    Graph.deliveredItems.Add(new DeliveryItem(PackagesList.packagesList[solution[i]], carId)); // Set package as delived
                }
                Console.WriteLine();
                if(++carId >= carsNumber)
                {
                    carId = 0;
                }
            }

            ResultsLvRefresh();

            // TODO all of algorithm -> knapsack in one car + TSP for that car + iterate over cars
        }

        private Dictionary<packageSize, int> GetPackageSizeMapping()
        {
            Dictionary<packageSize, int> sizeMap = new Dictionary<packageSize, int>();
            sizeMap.Add(packageSize.small, Int32.Parse(pckSmSizeTb.Text));
            sizeMap.Add(packageSize.medium, Int32.Parse(pckMdSizeTb.Text));
            sizeMap.Add(packageSize.big, Int32.Parse(pckBgSizeTb.Text));

            return sizeMap;
        }

        private void ResultsLvRefresh()
        {
            for (int i = 0; i < Graph.numberOfDelivered; i++)
            {
                ListViewItem listitem = new ListViewItem(Graph.deliveredItems[i].package.Id);
                listitem.SubItems.Add(Graph.deliveredItems[i].package.RecName);
                listitem.SubItems.Add(Graph.deliveredItems[i].package.RecAdress);
                listitem.SubItems.Add(Graph.deliveredItems[i].package.RecZipCode);
                listitem.SubItems.Add(Graph.deliveredItems[i].package.RecCity);
                listitem.SubItems.Add(Graph.deliveredItems[i].package.RecTelNum);
                listitem.SubItems.Add("");
                listitem.SubItems.Add(Graph.deliveredItems[i].carId.ToString());
                resultsLv.Items.Add(listitem);

            }

        }

        private void ShowDistMatrixBtn_Click(object sender, EventArgs e)//test
        {
            var form = new ShowDistMatrixForm();
            form.bindDataToGidView();
            form.Show();
        }

        private void ShowTimeMatrixBtn_Click(object sender, EventArgs e)//test
        {
            var form = new ShowTimeMatrixForm();
            form.bindDataToGidView();
            form.Show();
        }

        private void ExportDistMatrixBtn_Click(object sender, EventArgs e)
        {
            Matrices.exportDistMatrix(pathFileTb.Text);
            var dlg = MessageBox.Show("Info: Wyekspotowano " + filename, "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ExportTimeMatrixBtn_Click(object sender, EventArgs e)
        {
            Matrices.exportTimeMatrix(pathFileTb.Text);
            var dlg = MessageBox.Show("Info: Wyekspotowano " + filename, "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
