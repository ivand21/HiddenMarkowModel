using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiddenMarkowModel
{
    public class Program
    {
        static void Main(string[] args)
        {
            var casino = new Casino();
            var trainingSet = casino.CreateTrainingSet(Casino.TRAINING_SET_COUNT);
            var hmm = new HMM(trainingSet);
            hmm.TrainBaumWelch();
            hmm.PrettyPrintModel();
        }
    }
}
