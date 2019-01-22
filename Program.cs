using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiddenMarkowModel
{
    public class Program
    {
        public enum State { Fair, Loaded };

        public const int STATES_COUNT = 2;
        public const int NUMBERS_COUNT = 6;

        public const int TRAINING_SET_COUNT = 10;

        static void Main(string[] args)
        {
            var trainingSet = CreateTrainingSet(TRAINING_SET_COUNT);
            var hmm = new HMM(trainingSet);
            hmm.TrainBaumWelch();
            hmm.PrettyPrintModel();
        }

        static List<int[]> CreateTrainingSet(int count)
        {
            var set = new List<int[]>();
            for (int i=0; i<count; i++)
            {
                var sequence = Simulate();
                set.Add(sequence);
            }

            return set;
        }

        static int[] Simulate()
        {
            bool isFair = true;
            List<int> results = new List<int>();
            double[] unfairProbabilities = new double[] { 0.1, 0.1, 0.1, 0.1, 0.1, 0.5 };
            double swapToFairProb = 0.05;
            double swapToUnfairProb = 0.1;
            const int ROLLS_NO = 100;
            Random rnd = new Random();

            for (int i = 0; i < ROLLS_NO; i++)
            {
                int result;
                if (isFair)
                {
                    result = RollFairDice(rnd);
                    if (CheckDiceSwap(rnd, swapToUnfairProb))
                    {
                        isFair = false;
                        //Console.WriteLine("Zaczynam kantować!");
                    }
                }
                else
                {
                    result = RollUnfairDice(rnd, unfairProbabilities);
                    if (CheckDiceSwap(rnd, swapToFairProb))
                    {
                        isFair = true;
                        //Console.WriteLine("Już będę uczciwy!");
                    }
                }
                results.Add(result);
                //Console.Write(result);
            }

            return results.ToArray();
        }

        static int RollFairDice(Random rnd)
        {
            return rnd.Next(0, 6);
        }

        static int RollUnfairDice(Random rnd, double[] probabilities)
        {
            double randomNumber = rnd.NextDouble();
            double sum = 0.0;
            for (int i = 0; i < probabilities.Length; i++)
            {
                sum += probabilities[i];
                if (sum > randomNumber)
                {
                    return i;
                }
            }
            return -1;
        }

        static bool CheckDiceSwap(Random rnd, double prob)
        {
            return rnd.NextDouble() < prob;
        }
    }
}
