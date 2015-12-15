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
        private List<ForestTree> mTrees = new List<ForestTree>();
        private List<double[][]> mSubsets = new List<double[][]>();
        private int mNRows;
        private int mNCols;
        private int mNTrees;
        private double mSizeOfRandomSample;
        private double mNColsPerRandomSample;
        private string[] mInputColumns;
        private string mOutputColumn;
        private DataTable mData;
        private Codification mCodebook;
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
            int[][] treePreds = mTrees.Select(x => x.Predict(symbols)).ToArray();
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
            //for(int i = 0; i < mNTrees; i++)
            {
                Random rnd = new Random();
                DataTable dataSubset = mData.AsEnumerable().Where(x => rnd.Next(100) <= mSizeOfRandomSample * 100).CopyToDataTable();
                int classCnt = dataSubset.AsEnumerable().Select(y => y.Field<object>(mOutputColumn)).Distinct().Count();
                while (classCnt < 2)
                {
                    dataSubset = mData.AsEnumerable().Where(x => rnd.Next(100) <= mSizeOfRandomSample * 100).CopyToDataTable();
                    classCnt = dataSubset.AsEnumerable().Select(y => y.Field<object>(mOutputColumn)).Distinct().Count();
                }
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
