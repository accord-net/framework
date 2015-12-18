using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Accord.Statistics.Filters;
using Accord.Math;
using AForge;


namespace Accord.MachineLearning.DecisionTrees
{
    public class RandomForest
    {
		// trees in the forest
        private List<ForestTree> mTrees = new List<ForestTree>();
		// bootstrapped training samples
        private List<double[][]> mSubsets = new List<double[][]>();
		// number of rows in the training data
        private int mNRows;
		// number of columns in the training data
        private int mNCols;
		// number of trees in the forest
        private int mNTrees;
		// proportion of the total training data to use in each bootstrapped sample (defaults to .632)
        private double mSizeOfRandomSample;
		// proportion of columns to consider at each split (defaults to the square root of the total number of columns divided by the total number of columns)
        private double mNColsPerRandomSample;
		// names of feature columns
        private string[] mInputColumns;
		// name of label column
        private string mOutputColumn;
		// training data
        private DataTable mData;
		// categorical value encoding table
        private Codification mCodebook;
		// parallelism options and lock
        private ParallelOptions mParallelOptions;
        private object mLock;

        public RandomForest(double maxFeatures = 0, double sizeOfRandomSample = .632, int nTrees = 100, int degreeOfParallelism = 1)
        {
            mNColsPerRandomSample = maxFeatures;
            mSizeOfRandomSample = sizeOfRandomSample;
            mNTrees = nTrees;
            mParallelOptions = new ParallelOptions { MaxDegreeOfParallelism = degreeOfParallelism };
            mLock = new object();
        }

        public void Fit(DataTable data, string[] inputColumns, string outputColumn)
        {
            mNRows = data.Rows.Count;
            mInputColumns = inputColumns;
            mOutputColumn = outputColumn;
            mCodebook = new Codification(data);
            DataTable symbols = mCodebook.Apply(data);
            mNCols = symbols.Columns.Count - 1;
            if (mNColsPerRandomSample == 0)
            {
                mNColsPerRandomSample = System.Math.Sqrt(mNCols);
            } else {
                mNColsPerRandomSample = mNCols * mNColsPerRandomSample;
            }
            mNColsPerRandomSample = mNColsPerRandomSample / mNCols;
            mData = symbols;
            createForest();
        }

        public string[] Predict(DataTable data, double threshold = .5)
        {
            DataTable symbols = mCodebook.Apply(data);
			// generate predictions for each data point from each tree in the forest
            int[][] treePreds = mTrees.Select(x => x.Predict(symbols)).ToArray();
			// average the prediction of each tree for each datapoint to determine the final estimated class probability
            List<double> predProbs = new List<double>();
            for(int i = 0; i < data.Rows.Count; i++)
            {
                double predProb = treePreds.Select(x => x[i]).Average();
                //double predProb = treePreds.Last()[i];
                predProbs.Add(predProb);
            }

            return predProbs.Select(x => mCodebook.Translate(mOutputColumn, Convert.ToInt32(x > threshold))).ToArray();
        }


        private void createForest()
        {
            Parallel.For(0, mNTrees, mParallelOptions, i =>
            {
				// create bootstrapped sample, while making sure each sample has at least one example of each class
                Random rnd = new Random();
                DataTable dataSubset = mData.AsEnumerable().Where(x => rnd.Next(100) <= mSizeOfRandomSample * 100).CopyToDataTable();
                int classCnt = dataSubset.AsEnumerable().Select(y => y.Field<object>(mOutputColumn)).Distinct().Count();
                while (classCnt < 2)
                {
                    dataSubset = mData.AsEnumerable().Where(x => rnd.Next(100) <= mSizeOfRandomSample * 100).CopyToDataTable();
                    classCnt = dataSubset.AsEnumerable().Select(y => y.Field<object>(mOutputColumn)).Distinct().Count();
                }
				// only use columns that have multiple values in the bootstrapped sample
                string[] inputColSubset = mInputColumns.Where(x => dataSubset.AsEnumerable().Select(y => y.Field<object>(x)).Distinct().Count() > 1).ToArray();
                List<DecisionVariable> attributes = new List<DecisionVariable>();
                foreach (string inputCol in inputColSubset)
                {
                    double[] colVals = dataSubset.ToArray<double>(inputCol);
                    DoubleRange currRange = new DoubleRange(0, colVals.Max());
                    attributes.Add(new DecisionVariable(inputCol, currRange));
                }

                ForestTree tree = new ForestTree(mNColsPerRandomSample, inputColSubset, mOutputColumn, mCodebook, attributes);
                tree.Fit(dataSubset);
                lock (mLock)
                {mTrees.Add(tree);}
                
            }
            );


            


            
        }

    }
}
