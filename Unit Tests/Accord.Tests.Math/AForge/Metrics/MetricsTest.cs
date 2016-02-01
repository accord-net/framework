using System;
using System.Collections.Generic;
using Accord;
using Accord.Math.Metrics;
using NUnit.Framework;

namespace Accord.Tests.Math
{
    [TestFixture]
    public class MetricsTest
    {
        // test data
        private double[] p0 = new double[] { 1, 0.5 };
        private double[] q0 = new double[] { 0.5, 1 };

        private double[] p1 = new double[] { 4.5, 1 };
        private double[] q1 = new double[] { 4, 2 };

        private double[] p2 = new double[] { 0, 0, 0 };
        private double[] q2 = new double[] { 0, 0, 0 };

        private double[] p3 = new double[] { 1, 1, 1 };
        private double[] q3 = new double[] { 1, 1, 1 };

        private double[] p4 = new double[] { 2.5, 3.5, 3.0, 3.5, 2.5, 3.0 };
        private double[] q4 = new double[] { 3.0, 3.5, 1.5, 5.0, 3.5, 3.0 };

        private double[] p5 = new double[] { 1, 3, 5, 6, 8, 9, 6, 4, 3, 2 };
        private double[] q5 = new double[] { 2, 5, 6, 6, 7, 7, 5, 3, 1, 1 };

        [Test]
        public void CosineDistanceTest()
        {
            CosineDistance dist = new CosineDistance();

            // TODO: Re-enable argument checking only for debugging
            // Assert.Throws<ArgumentException>(() => dist.GetDistance(p0, q4));

            double result = dist.GetDistance(p0, q0);
            Assert.AreEqual(result, .2, 0.00001);

            result = dist.GetDistance(p1, q1);
            Assert.AreEqual(result, 0.029857, 0.00001);

            result = dist.GetDistance(p2, q2);
            Assert.AreEqual(result, 1);

            result = dist.GetDistance(p3, q3);
            Assert.AreEqual(result, 0, 0.00001);

            result = dist.GetDistance(p4, q4);
            Assert.AreEqual(result, 0.039354, 0.00001);

            result = dist.GetDistance(p5, q5);
            Assert.AreEqual(result, 0.031026, 0.00001);
        }

        [Test]
        public void CosineSimilarityTest()
        {
            CosineSimilarity sim = new CosineSimilarity();

            // TODO: Re-enable argument checking only for debugging
            // Assert.Throws<ArgumentException>(() => sim.GetSimilarityScore(p0, q4));

            double result = sim.GetSimilarityScore(p0, q0);
            Assert.AreEqual(result, .8, 0.00001);

            result = sim.GetSimilarityScore(p1, q1);
            Assert.AreEqual(result, 0.97014, 0.00001);

            result = sim.GetSimilarityScore(p2, q2);
            Assert.AreEqual(result, 0);

            result = sim.GetSimilarityScore(p3, q3);
            Assert.AreEqual(result, 1, 0.00001);

            result = sim.GetSimilarityScore(p4, q4);
            Assert.AreEqual(result, 0.96065, 0.00001);

            result = sim.GetSimilarityScore(p5, q5);
            Assert.AreEqual(result, 0.96897, 0.00001);
        }

        [Test]
        public void EuclideanDistanceTest()
        {
            EuclideanDistance dist = new EuclideanDistance();

            // TODO: Re-enable argument checking only for debugging
            // Assert.Throws<ArgumentException>(() => dist.GetDistance(p0, q4));

            double result = dist.GetDistance(p0, q0);
            Assert.AreEqual(result, .70711, 0.00001);

            result = dist.GetDistance(p1, q1);
            Assert.AreEqual(result, 1.11803, 0.00001);

            result = dist.GetDistance(p2, q2);
            Assert.AreEqual(result, 0);

            result = dist.GetDistance(p3, q3);
            Assert.AreEqual(result, 0);

            result = dist.GetDistance(p4, q4);
            Assert.AreEqual(result, 2.39792, 0.00001);

            result = dist.GetDistance(p5, q5);
            Assert.AreEqual(result, 4.24264, 0.00001);
        }

        [Test]
        public void EuclideanSimilarityTest()
        {
            EuclideanSimilarity sim = new EuclideanSimilarity();

            // Assert.Throws<ArgumentException>(() => sim.GetSimilarityScore(p0, q4));

            double result = sim.GetSimilarityScore(p0, q0);
            Assert.AreEqual(result, 0.58578, 0.00001);

            result = sim.GetSimilarityScore(p1, q1);
            Assert.AreEqual(result, 0.47213, 0.00001);

            result = sim.GetSimilarityScore(p2, q2);
            Assert.AreEqual(result, 1);

            result = sim.GetSimilarityScore(p3, q3);
            Assert.AreEqual(result, 1);

            result = sim.GetSimilarityScore(p4, q4);
            Assert.AreEqual(result, 0.2943, 0.00001);

            result = sim.GetSimilarityScore(p5, q5);
            Assert.AreEqual(result, 0.19074, 0.00001);
        }

        [Test]
        public void HammingDistanceTest()
        {
            HammingDistance dist = new HammingDistance();

            // TODO: Re-enable argument checking only for debugging
            // Assert.Throws<ArgumentException>(() => dist.GetDistance(p0, q4));

            double result = dist.GetDistance(p0, q0);
            Assert.AreEqual(result, 2);

            result = dist.GetDistance(p1, q1);
            Assert.AreEqual(result, 2);

            result = dist.GetDistance(p2, q2);
            Assert.AreEqual(result, 0);

            result = dist.GetDistance(p3, q3);
            Assert.AreEqual(result, 0);

            result = dist.GetDistance(p4, q4);
            Assert.AreEqual(result, 4);

            result = dist.GetDistance(p5, q5);
            Assert.AreEqual(result, 9);
        }

        [Test]
        public void JaccardDistanceTest()
        {
            JaccardDistance dist = new JaccardDistance();

            //Assert.Throws<ArgumentException>(() => dist.GetDistance(p0, q4));

            double result = dist.GetDistance(p0, q0);
            Assert.AreEqual(result, 1);

            result = dist.GetDistance(p1, q1);
            Assert.AreEqual(result, 1);

            result = dist.GetDistance(p2, q2);
            Assert.AreEqual(result, 0);

            result = dist.GetDistance(p3, q3);
            Assert.AreEqual(result, 0);

            result = dist.GetDistance(p4, q4);
            Assert.AreEqual(result, 0.66666, 0.00001);

            result = dist.GetDistance(p5, q5);
            Assert.AreEqual(result, 0.9, 0.1);
        }

        [Test]
        public void ManhattanDistanceTest()
        {
            ManhattanDistance dist = new ManhattanDistance();

            // Assert.Throws<ArgumentException>(() => dist.GetDistance(p0, q4));

            double result = dist.GetDistance(p0, q0);
            Assert.AreEqual(result, 1);

            result = dist.GetDistance(p1, q1);
            Assert.AreEqual(result, 1.5);

            result = dist.GetDistance(p2, q2);
            Assert.AreEqual(result, 0);

            result = dist.GetDistance(p3, q3);
            Assert.AreEqual(result, 0);

            result = dist.GetDistance(p4, q4);
            Assert.AreEqual(result, 4.5);

            result = dist.GetDistance(p5, q5);
            Assert.AreEqual(result, 12);
        }

        [Test]
        public void PearsonCorrelationTest()
        {
            PearsonCorrelation sim = new PearsonCorrelation();

            // Assert.Throws<ArgumentException>(() => sim.GetSimilarityScore(p0, q4));

            double result = sim.GetSimilarityScore(p0, q0);
            Assert.AreEqual(result, -1);

            result = sim.GetSimilarityScore(p1, q1);
            Assert.AreEqual(result, 1);

            result = sim.GetSimilarityScore(p2, q2);
            Assert.AreEqual(result, 0);

            result = sim.GetSimilarityScore(p3, q3);
            Assert.AreEqual(result, 0);

            result = sim.GetSimilarityScore(p4, q4);
            Assert.AreEqual(result, 0.396059, 0.00001);

            result = sim.GetSimilarityScore(p5, q5);
            Assert.AreEqual(result, 0.85470, 0.00001);
        }
    }
}
