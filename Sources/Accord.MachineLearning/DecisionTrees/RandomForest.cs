using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Accord.Statistics.Filters;
using Accord.Math;
using AForge;


namespace Accord.MachineLearning.DecisionTrees
{
    [Serializable]
    public class RandomForest
    {
		// trees in the forest
        private List<ForestTree> Trees = new List<ForestTree>();
		// bootstrapped training samples
        private List<double[][]> Subsets = new List<double[][]>();
		// number of rows in the training data
        private int NRows;
		// number of columns in the training data
        private int NCols;
		// number of trees in the forest
        private int NTrees;
		// proportion of the total training data to use in each bootstrapped sample (defaults to .632)
        private double SizeOfRandomSample;
		// proportion of columns to consider at each split (defaults to the square root of the total number of columns divided by the total number of columns)
        private double NColsPerRandomSample;
		// names of feature columns
        private string[] InputColumns;
		// name of label column
        private string OutputColumn;
		// training data
        private DataTable TrainData;
		// categorical value encoding table
        private Codification Codebook;
		// parallelism options and lock
        [System.NonSerialized]
        private ParallelOptions ParallelOptions;
        private object MyLock;

        public RandomForest(double maxFeatures = 0, double sizeOfRandomSample = .632, int nTrees = 100, int degreeOfParallelism = 1)
        {
            NColsPerRandomSample = maxFeatures;
            SizeOfRandomSample = sizeOfRandomSample;
            NTrees = nTrees;
            ParallelOptions = new ParallelOptions { MaxDegreeOfParallelism = degreeOfParallelism };
            MyLock = new object();
        }

        public void Fit(DataTable data, string[] inputColumns, string outputColumn)
        {
            NRows = data.Rows.Count;
            InputColumns = inputColumns;
            OutputColumn = outputColumn;
            Codebook = new Codification(data);
            DataTable symbols = Codebook.Apply(data);
            NCols = symbols.Columns.Count - 1;
            if (NColsPerRandomSample == 0)
            {
                NColsPerRandomSample = System.Math.Sqrt(NCols);
            } else {
                NColsPerRandomSample = NCols * NColsPerRandomSample;
            }
            NColsPerRandomSample = NColsPerRandomSample / NCols;
            TrainData = symbols;
            createForest();
        }

        public string[] Predict(DataTable data, double threshold = .5)
        {
            DataTable symbols = Codebook.Apply(data);
			// generate predictions for each data point from each tree in the forest
            int[][] treePreds = Trees.Select(x => x.Predict(symbols)).ToArray();
			// average the prediction of each tree for each datapoint to determine the final estimated class probability
            List<double> predProbs = new List<double>();
            for(int i = 0; i < data.Rows.Count; i++)
            {
                double predProb = treePreds.Select(x => x[i]).Average();
                //double predProb = treePreds.Last()[i];
                predProbs.Add(predProb);
            }

            return predProbs.Select(x => Codebook.Translate(OutputColumn, Convert.ToInt32(x > threshold))).ToArray();
        }

        /// <summary>
        ///   Loads a forest from a file.
        /// </summary>
        /// 
        /// <param name="path">The path to the file from which the forest is to be deserialized.</param>
        /// 
        /// <returns>The deserialized forest.</returns>
        /// 
        public void Save(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                Save(fs);
            }
        }

        /// <summary>
        ///   Saves the forest to a stream.
        /// </summary>
        /// 
        /// <param name="stream">The stream to which the forest is to be serialized.</param>
        /// 
        public void Save(Stream stream)
        {
            BinaryFormatter b = new BinaryFormatter();
            b.Serialize(stream, this);
        }

        /// <summary>
        ///   Loads a forest from a stream.
        /// </summary>
        /// 
        /// <param name="stream">The stream from which the forest is to be deserialized.</param>
        /// 
        /// <returns>The deserialized forest.</returns>
        /// 
        public static RandomForest Load(Stream stream)
        {
            BinaryFormatter b = new BinaryFormatter();
            return (RandomForest)b.Deserialize(stream);
        }

        /// <summary>
        ///   Loads a tree from a file.
        /// </summary>
        /// 
        /// <param name="path">The path to the tree from which the machine is to be deserialized.</param>
        /// 
        /// <returns>The deserialized tree.</returns>
        /// 
        public static RandomForest Load(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                return Load(fs);
            }
        }


        private void createForest()
        {
            Parallel.For(0, NTrees, ParallelOptions, i =>
            {
				// create bootstrapped sample, while making sure each sample has at least one example of each class
                Random rnd = new Random();
                DataTable dataSubset = TrainData.AsEnumerable().Where(x => rnd.Next(100) <= SizeOfRandomSample * 100).CopyToDataTable();
                int classCnt = dataSubset.AsEnumerable().Select(y => y.Field<object>(OutputColumn)).Distinct().Count();
                while (classCnt < 2)
                {
                    dataSubset = TrainData.AsEnumerable().Where(x => rnd.Next(100) <= SizeOfRandomSample * 100).CopyToDataTable();
                    classCnt = dataSubset.AsEnumerable().Select(y => y.Field<object>(OutputColumn)).Distinct().Count();
                }
				// only use columns that have multiple values in the bootstrapped sample
                string[] inputColSubset = InputColumns.Where(x => dataSubset.AsEnumerable().Select(y => y.Field<object>(x)).Distinct().Count() > 1).ToArray();
                List<DecisionVariable> attributes = new List<DecisionVariable>();
                foreach (string inputCol in inputColSubset)
                {
                    double[] colVals = dataSubset.ToArray<double>(inputCol);
                    DoubleRange currRange = new DoubleRange(0, colVals.Max());
                    attributes.Add(new DecisionVariable(inputCol, currRange));
                }

                ForestTree tree = new ForestTree(NColsPerRandomSample, inputColSubset, OutputColumn, Codebook, attributes);
                tree.Fit(dataSubset);
                lock (MyLock)
                {Trees.Add(tree);}
                
            }
            );


            


            
        }

    }
}
