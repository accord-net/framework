using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Accord.Statistics.Filters;


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
        private int mDegreeOfParallelism;

        public RandomForest(double maxFeatures = 0, double sizeOfRandomSample = .632, int nTrees = 100, int degreeOfParallelism = 1)
        {
            mNColsPerRandomSample = maxFeatures;
            mSizeOfRandomSample = sizeOfRandomSample;
            mNTrees = nTrees;
            mDegreeOfParallelism = degreeOfParallelism;
        }

        public void Fit(DataTable data, string[] inputColumns, string outputColumn)
        {
            mNRows = data.Rows.Count;
            mNCols = data.Columns.Count - 1;
            if (mNColsPerRandomSample == 0)
            {
                mNColsPerRandomSample = System.Math.Sqrt(mNCols) / mNCols;
            }
            mInputColumns = inputColumns;
            mOutputColumn = outputColumn;
            mCodebook = new Codification(data);
            DataTable symbols = mCodebook.Apply(data);
            mData = symbols;
            createForest();
        }

        public string[] Predict(DataTable data, double threshold)
        {
            DataTable symbols = mCodebook.Apply(data);
            int[][] treePreds = mTrees.Select(x => x.Predict(symbols)).ToArray();
            List<double> predProbs = new List<double>();
            int i = 0;
            while(i < data.Rows.Count)
            {
                predProbs.Add(treePreds.Select(x => x[i]).Average());
                i += 1;
            }

            return predProbs.Select(x => mCodebook.Translate(mOutputColumn, Convert.ToInt32(x > threshold))).ToArray();
        }


        private void createForest()
        {
            object lck = new object();
            ParallelOptions options = new ParallelOptions {MaxDegreeOfParallelism = mDegreeOfParallelism};
            Parallel.For(0, mNTrees, i =>
            {
                Random rnd = new Random();
                DataTable dataSubset = mData.AsEnumerable().Where(x => rnd.Next(100) < mSizeOfRandomSample * 100).CopyToDataTable();
                int classCnt = dataSubset.AsEnumerable().Select(y => y.Field<object>(mOutputColumn)).Distinct().Count();
                while (classCnt < 2)
                {
                    dataSubset = mData.AsEnumerable().Where(x => rnd.Next(100) < mSizeOfRandomSample * 100).CopyToDataTable();
                    classCnt = dataSubset.AsEnumerable().Select(y => y.Field<object>(mOutputColumn)).Distinct().Count();
                }
                string[] inputColSubset = mInputColumns.Where(x => rnd.Next(100) < mNColsPerRandomSample * 100).ToArray();
                inputColSubset = inputColSubset.Where(x => dataSubset.AsEnumerable().Select(y => y.Field<object>(x)).Distinct().Count() > 1).ToArray();
                ForestTree tree = new ForestTree(inputColSubset, mOutputColumn, mCodebook);
                tree.Fit(dataSubset);
                lock (lck)
                {mTrees.Add(tree);}
                
            } );


            


            
        }

    }
}
