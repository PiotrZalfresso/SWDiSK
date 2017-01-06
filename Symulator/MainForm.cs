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
    enum AlgorithmType
    {
        annealing,
        genetic,
    }

    enum OptimizitaionTarget
    {
        distance,
        time,
    }

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
                    ShowRouteBtn.Enabled = readMatrix = textBox14.Visible = CalcTimeDtp.Enabled = false;

        }

        private void readDlgBtn_Click(object sender, EventArgs e)
        {
            using (var dlg = new OpenFileDialog())
            {
                dlg.Multiselect = false;
                dlg.Filter = "CSV files (*.csv)|*.csv";
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

                if (PackagesList.packagesList[i].RecTimeFrom != null)
                    listitem.SubItems.Add(PackagesList.packagesList[i].RecTimeFrom.Value.ToString("HH:mm"));
                else
                    listitem.SubItems.Add("-");

                if (PackagesList.packagesList[i].RecTimeTo != null)
                    listitem.SubItems.Add(PackagesList.packagesList[i].RecTimeTo.Value.ToString("HH:mm"));
                else
                    listitem.SubItems.Add("-");
              
                listitem.SubItems.Add(PackagesList.packagesList[i].Size.GetDescription());
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
                    Optimize(AlgorithmType.annealing, OptimizitaionTarget.distance);
                else if (geneticAlgorithm.Checked)
                    Optimize(AlgorithmType.genetic, OptimizitaionTarget.distance);
                else
                    throw new InvalidOperationException("Nie wskazano algorytmu");
            }
            else if (timeOptimization.Checked)
            {
                if (simulatedAnnealing.Checked)
                    Optimize(AlgorithmType.annealing, OptimizitaionTarget.time);
                else if (geneticAlgorithm.Checked)
                    Optimize(AlgorithmType.genetic, OptimizitaionTarget.time);
                else
                    throw new InvalidOperationException("Nie wskazano algorytmu");
            }
            else
            {
                throw new InvalidOperationException("Nie wskazano typu optymalizacji");
            }
            
        }

        private void Optimize(AlgorithmType type, OptimizitaionTarget target)
        {
             // Clean up after previous optimizations
            if (Graph.deliveredItems.Count > 0) {
                Graph.Clear();
            }

            Dictionary<packageSize, int> sizeMap = GetPackageSizeMapping();
            Dictionary<packageSize, long> timeMap = GetPackageSizeTimeMapping();

            int neighbourhoodSize = Int32.Parse(carCapTb.Text) / Int32.Parse(pckSmSizeTb.Text);
            int carsNumber = Int32.Parse(carNumTb.Text);
            int carCapacity = Int32.Parse(carCapTb.Text);
            int saTemp = Int32.Parse(saTempTb.Text);
            float saLambda = Single.Parse(saLambdaTb.Text);
            int saRepet = Int32.Parse(saRepetTb.Text);
            int gaPopulation = Int32.Parse(gaPopulationSizeTb.Text);
            int gaGenerations = Int32.Parse(gaGenerationsNmbTb.Text);

            long[,] costs;
            if (target == OptimizitaionTarget.distance)
            {
                costs = Matrices.Distance;
            }
            else if (target == OptimizitaionTarget.time)
            {
                costs = Matrices.Time;
            }
            else
            {
                throw new InvalidEnumArgumentException("Wrong optization target!");
            }

            int carId = 0;
            long[] carsTimes = Enumerable.Repeat(0L, carsNumber).ToArray();
            while (Graph.numberOfDelivered < PackagesList.numberOfPackages) {
                if ((PackagesList.numberOfPackages - Graph.numberOfDelivered) < neighbourhoodSize)
                {
                    neighbourhoodSize = PackagesList.numberOfPackages - Graph.numberOfDelivered;
                }
                Console.WriteLine(String.Format("Pozostało {0} paczek do rozwiezienia", PackagesList.numberOfPackages - Graph.numberOfDelivered));
                int[] toDeliver = Tsp.getNeighbors(neighbourhoodSize, Matrices.Distance);
                Console.Write("Rozważamy sąsiedztwo: ");
                for (int i = 0; i < neighbourhoodSize; i++)
                {
                    Console.Write(toDeliver[i].ToString() + " ");
                }
                Console.WriteLine();

                Knapsack packageSelector = new Knapsack(toDeliver, sizeMap, carCapacity);
                SimulatedAnnealing algorithm = new SimulatedAnnealing(saTemp, saLambda, saRepet);
               // GeneticAlgorithm algorithm = new GeneticAlgorithm(gaPopulation, gaGenerations);
                int[] finalToDeliver = algorithm.Calculate(packageSelector);

                Console.Write(String.Format("Samochód {0} dostarczy do punktów: ", carId));
                for (int i = 0; i < finalToDeliver.Length; i++)
                {
                    Console.Write(finalToDeliver[i].ToString() + " ");
                }
                Console.WriteLine();

                Tsp pointsSorter;
                if ( target == OptimizitaionTarget.time) {
                    pointsSorter = new Tsp(finalToDeliver, costs, timeMap, true);
                }
                else if (target == OptimizitaionTarget.distance)
                {
                    pointsSorter = new Tsp(finalToDeliver, costs, null, true);
                }
                else
                {
                    throw new InvalidEnumArgumentException("Wrong optization target!");
                }

                int[] solution;
                long travelTime;
                if (type == AlgorithmType.annealing)
                {
                    solution = (new SimulatedAnnealing(saTemp, saLambda, saRepet)).Calculate(pointsSorter);
                }
                else if (type == AlgorithmType.genetic)
                {
                    solution = (new GeneticAlgorithm(gaPopulation, gaGenerations)).Calculate(pointsSorter);
                }
                else
                {
                    throw new InvalidEnumArgumentException("Wrong algorithm type!");
                }
                long[] times = CalcTimes(solution, carsTimes[carId], timeMap);

                if (target == OptimizitaionTarget.time)
                {
                    travelTime = pointsSorter.GetCost(solution);
                    Graph.totalTime += travelTime;
                    carsTimes[carId] += travelTime;
                    Graph.totalDistance += (new Tsp(finalToDeliver, Matrices.Distance, null, true)).GetCost(solution);
                }
                else if (target == OptimizitaionTarget.distance)
                {
                    Graph.totalDistance += pointsSorter.GetCost(solution);
                    travelTime = (new Tsp(finalToDeliver, Matrices.Time, timeMap, true)).GetCost(solution);
                    carsTimes[carId] += travelTime;
                    Graph.totalTime += travelTime;
                }
                else
                {
                    throw new InvalidEnumArgumentException("Wrong optization target!");
                }


                Console.Write("W kolejności: baza ");
                for (int i = 0; i < solution.Length; i++)
                {
                    Console.Write(solution[i].ToString() + " ");
                    Graph.deliveredItems.Add(new DeliveryItem(PackagesList.packagesList[solution[i]], times[i], carId)); // Set package as delived
                }
                Console.WriteLine(" baza");
                if(++carId >= carsNumber)
                {
                    carId = 0;
                }
            }

            ResultsLvRefresh();
            ShowRouteBtn.Enabled = true;

            MainTabControl.SelectedTab = HistoryTab;
        }

        private long[] CalcTimes(int[] solution, long startTime, Dictionary<packageSize, long> timeMap)
        {
            long[] times = new long[solution.Count()];

            for (int i = 0; i < solution.Length; i++)
            {
                if (i == 0)
                {
                    times[i] = startTime + Matrices.Time[PackagesList.numberOfPackages, solution[i]];
                    times[i] += timeMap[PackagesList.packagesList[solution[i]].Size];
                }
                else
                {
                    times[i] = times[i - 1] + Matrices.Time[solution[i - 1], solution[i]];
                    times[i] += timeMap[PackagesList.packagesList[solution[i]].Size];
                }
            }

            return times;
        }

        private Dictionary<packageSize, int> GetPackageSizeMapping()
        {
            Dictionary<packageSize, int> sizeMap = new Dictionary<packageSize, int>();
            sizeMap.Add(packageSize.small, Int32.Parse(pckSmSizeTb.Text));
            sizeMap.Add(packageSize.medium, Int32.Parse(pckMdSizeTb.Text));
            sizeMap.Add(packageSize.big, Int32.Parse(pckBgSizeTb.Text));

            return sizeMap;
        }

        private Dictionary<packageSize, long> GetPackageSizeTimeMapping()
        {
            Dictionary<packageSize, long> sizeMap = new Dictionary<packageSize, long>();
            sizeMap.Add(packageSize.small, Int64.Parse(pckSmTimeTb.Text) * 60);
            sizeMap.Add(packageSize.medium, Int64.Parse(pckMdTimeTb.Text) * 60);
            sizeMap.Add(packageSize.big, Int64.Parse(pckBgTimeTb.Text) * 60);

            return sizeMap;
        }

        private void ResultsLvRefresh()
        {
            totalDistanceTb.Text = Graph.totalDistance.ToString();
            totalTimeTb.Text = Graph.totalTime.ToString();

            resultsLv.Items.Clear();

            for (int i = 0; i < Graph.numberOfDelivered; i++)
            {
                ListViewItem listitem = new ListViewItem(Graph.deliveredItems[i].package.Id);
                listitem.SubItems.Add(Graph.deliveredItems[i].package.RecName);
                listitem.SubItems.Add(Graph.deliveredItems[i].package.RecAdress);
                listitem.SubItems.Add(Graph.deliveredItems[i].package.RecZipCode);
                listitem.SubItems.Add(Graph.deliveredItems[i].package.RecTelNum);
                listitem.SubItems.Add(Graph.deliveredItems[i].package.RecCity);
                listitem.SubItems.Add(estimateDeliveryTime(Graph.deliveredItems[i].time).ToString("HH:mm"));//TODO
                listitem.SubItems.Add(Graph.deliveredItems[i].carId.ToString());
                resultsLv.Items.Add(listitem);
            }

        }

        private DateTime estimateDeliveryTime(long time)
        {
            DateTime dt =  hubWrkTmStartDtp.Value;
            dt = dt.AddSeconds((double)time);
            return dt;
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

        private void OptChooseChange(object sender, EventArgs e)
        {
            if (distanceOptimization.Checked == true)
                textBox8.Visible = true;
            else
                textBox8.Visible = false;
        }

        private void ReadFroFileCheckedChanged(object sender, EventArgs e)
        {
            if (reabTablesFromFile.Checked == true)
                textBox14.Visible = true;
            else
                textBox14.Visible = false;
        }

        private void APKey(object sender, EventArgs e)
        {
            if (apKeyTb.Text.Length == 0)
                CalcTimeDtp.Enabled = false;
            else
                CalcTimeDtp.Enabled = true; 
        }

        private void ShowRouteBtn_Click(object sender, EventArgs e)
        {

            if (resultsLv.SelectedItems.Count > 0)
            {
                string start = resultsLv.SelectedItems[0].SubItems[2].Text + ",+" + resultsLv.SelectedItems[0].SubItems[5].Text;
                string end = "";

                if (resultsLv.Items.IndexOf(resultsLv.SelectedItems[0]) == 0
                    || (resultsLv.SelectedItems[0].SubItems[7].Text != resultsLv.Items[resultsLv.Items.IndexOf(resultsLv.SelectedItems[0]) - 1].SubItems[7].Text
                    && resultsLv.Items.IndexOf(resultsLv.SelectedItems[0]) > 0))
                {
                    end = hubAdressTb.Text + ",+" + hubCityTb.Text;
                }
                else
                {
                    end = resultsLv.Items[resultsLv.Items.IndexOf(resultsLv.SelectedItems[0]) - 1].SubItems[2].Text 
                        + ",+" + resultsLv.Items[resultsLv.Items.IndexOf(resultsLv.SelectedItems[0]) - 1].SubItems[5].Text;
                }
                  
                var form = new RouteMap(start, end);
                form.Show();
            }
        }
    }
}
