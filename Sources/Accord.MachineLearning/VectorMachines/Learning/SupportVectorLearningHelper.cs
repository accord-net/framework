using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Accord.MachineLearning.VectorMachines.Learning
{
    internal static class SupportVectorLearningHelper
    {

        public static void CheckArgs(SupportVectorMachine machine, double[][] inputs, int[] outputs)
        {
            // Initial argument checking
            if (machine == null)
                throw new ArgumentNullException("machine");

            if (inputs == null)
                throw new ArgumentNullException("inputs");

            if (outputs == null)
                throw new ArgumentNullException("outputs");

            if (inputs.Length != outputs.Length)
                throw new DimensionMismatchException("outputs",
                    "The number of input vectors and output labels does not match.");

            if (inputs.Length == 0)
                throw new ArgumentOutOfRangeException("inputs",
                    "Training algorithm needs at least one training vector.");

            if (machine.Inputs > 0)
            {
                // This machine has a fixed input vector size
                for (int i = 0; i < inputs.Length; i++)
                {
                    if (inputs[i].Length != machine.Inputs)
                    {
                        throw new DimensionMismatchException("inputs",
                            "The size of the input vector at index " + i
                            + " does not match the expected number of inputs of the machine."
                            + " All input vectors for this machine must have length " + machine.Inputs);
                    }
                }
            }

            for (int i = 0; i < outputs.Length; i++)
            {
                if (outputs[i] != 1 && outputs[i] != -1)
                {
                    throw new ArgumentOutOfRangeException("outputs",
                        "The output label at index " + i + " should be either +1 or -1.");
                }
            }
        }

    }
}
