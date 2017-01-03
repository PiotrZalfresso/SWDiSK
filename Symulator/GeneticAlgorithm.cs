using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symulator
{
    class GeneticInstanceIComparer : IComparer
    {
        int IComparer.Compare(Object x, Object y)
        {
            Tuple<int[], long> t1 = (Tuple<int[], long>)x;
            Tuple<int[], long> t2 = (Tuple<int[], long>)y;
            if (t1.Item2 < t2.Item2)               // Item2 zawiera odległości między miastami
            {
                return -1;
            }
            else if (t1.Item2 > t2.Item2)
            {
                return 1;
            }
            else
                return 0;
        }
    }

    public class GeneticAlgorithm
    {
        int populationSize;
        int generationsNmb;
        Random generator = new Random();

        public int PopulationSize
        {
            get
            {
                return populationSize;
            }
            set
            {
                populationSize = value;
            }
        }

        public int GenerationsNmb
        {
            get
            {
                return generationsNmb;
            }
            set
            {
                generationsNmb = value;
            }
        }

        public GeneticAlgorithm(int populationSize = 200, int generationsNmb = 500)
        {
            this.populationSize = populationSize;
            this.generationsNmb = generationsNmb;
        }

        public int[] Calculate(IGeneticAlgorithm instance)
        {
            int[][] popul = new int[populationSize][];
            for (int i=0; i < populationSize; i++)
            {
                popul[i] = instance.GetInitialSolution();
                int c = 0;
                while (instance.Check(popul, i) && c < 10)
                {
                    popul[i] = instance.Mutate(popul[i]);
                    c++;
                }
            }
            popul = Sort(popul, instance);

            for (int k = 0; k < generationsNmb; k++)
            {
                for (int i = populationSize / 2; i < populationSize - 1; i += 2)
                {
                    if (popul[i - populationSize / 2] == null) {
                        Console.WriteLine("null!");
                    }
                    Array.Copy(popul[i - populationSize / 2], popul[i], instance.GetInstanceSize()); 
                    Array.Copy(popul[i + 1 - populationSize / 2], popul[i + 1], instance.GetInstanceSize());  
                    Tuple<int[], int[]> afterCross = instance.Crossover(popul[i], popul[i + 1]);
                    popul[i] = afterCross.Item1;
                    popul[i + 1] = afterCross.Item2;
                    int c = 0;
                    while (instance.Check(popul, i) && c < 10)
                    {         
                        popul[i] = instance.Mutate(popul[i]);
                        c++;
                    }
                    c = 0;
                    while (instance.Check(popul, i + 1) && c < 10)
                    {
                        popul[i + 1] = instance.Mutate(popul[i + 1]);
                        c++;
                    }
                }
                popul = Sort(popul, instance);

                for (int i = 1; i < populationSize; i++)
                {
                    if (generator.Next(100) < 20)                
                        popul[i] = instance.Mutate(popul[i]);
                    int c = 0;
                    while (instance.Check(popul, i) && c < 10)
                    {         
                        popul[i] = instance.Mutate(popul[i]);
                        c++;
                    }
                }
                popul = Sort(popul, instance);

            }

            return instance.ConvertToFinalSolution(popul[0]);
        }

        

        private int[][] Sort(int[][] popul, IGeneticAlgorithm instance)
        {
            Tuple<int[], long>[] temp = new Tuple<int[], long>[populationSize];
            for (int i = 0; i < populationSize; i++)
            {
                temp[i] = new Tuple<int[], long>(popul[i], instance.GetCost(popul[i]));
            }
            Array.Sort(temp, new GeneticInstanceIComparer());
            int[][] sorted = new int[populationSize][];
            for (int i = 0; i < populationSize; i++)
            {
                sorted[i] = new int[instance.GetInstanceSize()];
                Array.Copy(temp[i].Item1, sorted[i], instance.GetInstanceSize());
            }
          //  Console.WriteLine($"Element 0 {temp[0].Item2}, a element 2 ma {temp[1].Item2}, a populacja {PopulationSize}, a ostatni... {temp[PopulationSize-1].Item2}");
            return sorted;
        }
        
    }
}
