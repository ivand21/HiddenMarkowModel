using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiddenMarkowModel
{
    public class Casino
    {
        private const int ROLLS_NO = 100;

        public const int STATES_COUNT = 2;
        public const int NUMBERS_COUNT = 6;

        public const int TRAINING_SET_COUNT = 10;

        public List<int[]> CreateTrainingSet(int count)
        {
            var set = new List<int[]>();
            for (int i = 0; i < count; i++)
            {
                var sequence = Simulate();
                set.Add(sequence);
            }

            return set;
        }

        public int[] Simulate()
        {
            bool isFair = true;
            List<int> results = new List<int>();
            double[] unfairProbabilities = new double[] { 0.1, 0.1, 0.1, 0.1, 0.1, 0.5 };
            double swapToFairProb = 0.05;
            double swapToUnfairProb = 0.1;
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
                    }
                }
                else
                {
                    result = RollUnfairDice(rnd, unfairProbabilities);
                    if (CheckDiceSwap(rnd, swapToFairProb))
                    {
                        isFair = true;
                    }
                }
                results.Add(result);
                //Console.Write(result);
            }

            return results.ToArray();
        }

        private int RollFairDice(Random rnd)
        {
            return rnd.Next(0, 6);
        }

        private int RollUnfairDice(Random rnd, double[] probabilities)
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

        private bool CheckDiceSwap(Random rnd, double prob)
        {
            return rnd.NextDouble() < prob;
        }

    }
}
