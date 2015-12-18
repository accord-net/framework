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
	// wrapper class for a decision tree with random forest hyperparameters
    class ForestTree
    {
		// categorical value encoding table
        private Codification mCodebook = null;
		// proportion of features to use at each split
        private double mPcntFeaturesToUse = 0;
		// names of feature columns
        private string[] mInputCols = null;
		// name of label column
        private string mOutputCol = null;
		// decision tree
        private DecisionTree mTree = null;
        List<DecisionVariable> mAttributes = null;

        public ForestTree(double pcntFeaturesToUse, string[] inputCols, string outputCol, Codification codebook, List<DecisionVariable> attributes)
        {
            mPcntFeaturesToUse = pcntFeaturesToUse;
            mInputCols = inputCols;
            mOutputCol = outputCol;
            mCodebook = codebook;
            mAttributes = attributes;
        }

        public void Fit(DataTable symbols)
        {
            double[][] inputs = symbols.ToArray(mInputCols);
            int[] outputs = symbols.ToArray<int>(mOutputCol);
            mTree = new DecisionTree(mAttributes, 2);
            mTree.pcntAttributesToUse = mPcntFeaturesToUse;
            C45Learning c45 = new C45Learning(mTree);
            c45.Join = 100;
            c45.Run(inputs, outputs);
        }

        public int[] Predict(DataTable symbols)
        {
            double[][] inputs = symbols.ToArray();
            return inputs.Select(x => predict(x)).ToArray();
        }

        private int predict(double[] data)
        {
            return mTree.Compute(data);
        }

    }
}
