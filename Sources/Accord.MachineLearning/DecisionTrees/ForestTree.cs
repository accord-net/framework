using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Accord.MachineLearning.DecisionTrees;
using Accord.Statistics.Filters;
using Accord.MachineLearning.DecisionTrees.Learning;
using Accord.Math;


namespace Accord.MachineLearning.DecisionTrees
{
    class ForestTree
    {
        private static Codification mCodebook = null;
        private static string[] mInputCols = null;
        private static string mOutputCol = null;
        private DecisionTree mTree = null;

        public ForestTree(string[] inputCols, string outputCol)
        {
            mInputCols = inputCols;
            mOutputCol = outputCol;
        }

        public void Fit(DataTable symbols)
        {
            double[][] inputs = symbols.ToArray(mInputCols);
            int[] outputs = symbols.ToArray<int>(mOutputCol);
            DecisionVariable[] attributes = DecisionVariable.FromCodebook(mCodebook, mInputCols);
            mTree = new DecisionTree(attributes, 2);
            C45Learning c45 = new C45Learning(mTree);
            c45.Run(inputs, outputs);
        }

        public int[] Predict(DataTable symbols)
        {
            double[][] inputs = symbols.ToArray(mInputCols);
            return inputs.Select(x => predict(x)).ToArray();
        }

        private int predict(double[] data)
        {
            return mTree.Compute(data);
        }

    }
}
