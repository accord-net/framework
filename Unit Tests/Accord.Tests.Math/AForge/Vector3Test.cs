using System;
using AForge;
using AForge.Math;
using NUnit.Framework;
using Accord.Math;

namespace Accord.Tests.Math
{
    [TestFixture]
    public class Vector3Test
    {
        [Test]
        public void ConstructorTest()
        {
            Vector3 v1 = new Vector3(1, 2, 3);
            Vector3 v2 = new Vector3(-1, -2, -3);
            Vector3 v3 = new Vector3(7);

            Assert.AreEqual(v1.X, 1);
            Assert.AreEqual(v1.Y, 2);
            Assert.AreEqual(v1.Z, 3);

            Assert.AreEqual(v2.X, -1);
            Assert.AreEqual(v2.Y, -2);
            Assert.AreEqual(v2.Z, -3);

            Assert.AreEqual(v3.X, 7);
            Assert.AreEqual(v3.Y, 7);
            Assert.AreEqual(v3.Z, 7);
        }


        [TestCase(0, 0, 0, 0, 0, 0, 0)]
        [TestCase(0, 7, 7, 0, 7, 0, 1)]
        [TestCase(0, 0, 7, 0, 7, 0, 2)]
        [TestCase(0, 7, 0, 0, 7, 0, 1)]
        [TestCase(7, 0, 7, 0, 7, 1, 0)]
        [TestCase(5, 7, 9, 5, 9, 0, 2)]
        [TestCase(5, 9, 7, 5, 9, 0, 1)]
        [TestCase(7, 5, 9, 5, 9, 1, 2)]
        [TestCase(7, 9, 5, 5, 9, 2, 1)]
        [TestCase(9, 5, 7, 5, 9, 1, 0)]
        [TestCase(9, 7, 5, 5, 9, 2, 0)]
        public void MinMaxTest(float x, float y, float z, float expectedMin, float expectedMax,
            int expectedMinIndex, int expectedMaxIndex)
        {
            Vector3 vector = new Vector3(x, y, z);

            Assert.AreEqual(vector.Min, expectedMin);
            Assert.AreEqual(vector.Max, expectedMax);

            Assert.AreEqual(vector.MinIndex, expectedMinIndex);
            Assert.AreEqual(vector.MaxIndex, expectedMaxIndex);
        }


        [TestCase(0, 0, 0, 0)]
        [TestCase(1, 0, 0, 1)]
        [TestCase(0, 2, 0, 2)]
        [TestCase(0, 0, 3, 3)]
        [TestCase(3, 4, 0, 5)]
        [TestCase(-3, -4, 0, 5)]
        public void NormTest(float x, float y, float z, float expectedNorm)
        {
            Vector3 vector = new Vector3(x, y, z);

            float norm = vector.Norm;

            Assert.AreEqual(norm, expectedNorm);
            Assert.AreEqual(norm * norm, vector.Square);
        }


        [TestCase(1, 2, 3, 1, 2, 3, true)]
        [TestCase(-1, -2, -3, -1, -2, -3, true)]
        [TestCase(-1, -2, -3, -1, -2, 3, false)]
        public void EqualityTest(float x1, float y1, float z1, float x2, float y2, float z2, bool expected)
        {
            Vector3 vector1 = new Vector3(x1, y1, z1);
            Vector3 vector2 = new Vector3(x2, y2, z2);

            Assert.AreEqual(vector1 == vector2, expected);
            Assert.AreEqual(vector1 != vector2, !expected);

            Assert.AreEqual(vector1.Equals(vector2), expected);
            Assert.AreEqual(vector1.Equals((object)vector2), expected);
        }


        [TestCase(1, 2, 3, 4, 5, 6, 5, 7, 9)]
        [TestCase(1, 2, 3, -4, -5, -6, -3, -3, -3)]
        public void AdditionTest(float x1, float y1, float z1, float x2, float y2, float z2,
            float expectedX, float expectedY, float expectedZ)
        {
            Vector3 vector1 = new Vector3(x1, y1, z1);
            Vector3 vector2 = new Vector3(x2, y2, z2);
            Vector3 expectedResult = new Vector3(expectedX, expectedY, expectedZ);

            Vector3 result1 = vector1 + vector2;
            Vector3 result2 = Vector3.Add(vector1, vector2);

            Assert.AreEqual(expectedResult == result1, true);
            Assert.AreEqual(expectedResult == result2, true);
        }


        [TestCase(1, 2, 3, 4, 5, 6, 7)]
        [TestCase(1, 2, 3, -4, -3, -2, -1)]
        public void AdditionWithConstTest(float x, float y, float z, float value,
            float expectedX, float expectedY, float expectedZ)
        {
            Vector3 vector = new Vector3(x, y, z);
            Vector3 expectedResult = new Vector3(expectedX, expectedY, expectedZ);

            Vector3 result1 = vector + value;
            Vector3 result2 = Vector3.Add(vector, value);

            Assert.AreEqual(expectedResult == result1, true);
            Assert.AreEqual(expectedResult == result2, true);
        }


        [TestCase(1, 2, 3, 4, 5, 6, -3, -3, -3)]
        [TestCase(1, 2, 3, -4, -5, -6, 5, 7, 9)]
        public void SubtractionTest(float x1, float y1, float z1, float x2, float y2, float z2,
            float expectedX, float expectedY, float expectedZ)
        {
            Vector3 vector1 = new Vector3(x1, y1, z1);
            Vector3 vector2 = new Vector3(x2, y2, z2);
            Vector3 expectedResult = new Vector3(expectedX, expectedY, expectedZ);

            Vector3 result1 = vector1 - vector2;
            Vector3 result2 = Vector3.Subtract(vector1, vector2);

            Assert.AreEqual(expectedResult == result1, true);
            Assert.AreEqual(expectedResult == result2, true);
        }


        [TestCase(1, 2, 3, 4, -3, -2, -1)]
        [TestCase(1, 2, 3, -4, 5, 6, 7)]
        public void SubtractionWithConstTest(float x, float y, float z, float value,
            float expectedX, float expectedY, float expectedZ)
        {
            Vector3 vector = new Vector3(x, y, z);
            Vector3 expectedResult = new Vector3(expectedX, expectedY, expectedZ);

            Vector3 result1 = vector - value;
            Vector3 result2 = Vector3.Subtract(vector, value);

            Assert.AreEqual(expectedResult == result1, true);
            Assert.AreEqual(expectedResult == result2, true);
        }


        [TestCase(1, 2, 3, 4, 5, 6, 4, 10, 18)]
        [TestCase(1, 2, 3, -4, -5, -6, -4, -10, -18)]
        public void MultiplicationTest(float x1, float y1, float z1, float x2, float y2, float z2,
            float expectedX, float expectedY, float expectedZ)
        {
            Vector3 vector1 = new Vector3(x1, y1, z1);
            Vector3 vector2 = new Vector3(x2, y2, z2);
            Vector3 expectedResult = new Vector3(expectedX, expectedY, expectedZ);

            Vector3 result1 = vector1 * vector2;
            Vector3 result2 = Vector3.Multiply(vector1, vector2);

            Assert.AreEqual(expectedResult == result1, true);
            Assert.AreEqual(expectedResult == result2, true);
        }


        [TestCase(1, 2, 3, 4, 4, 8, 12)]
        [TestCase(1, 2, 3, -4, -4, -8, -12)]
        public void MultiplicationWithConstTest(float x, float y, float z, float value,
            float expectedX, float expectedY, float expectedZ)
        {
            Vector3 vector = new Vector3(x, y, z);
            Vector3 expectedResult = new Vector3(expectedX, expectedY, expectedZ);

            Vector3 result1 = vector * value;
            Vector3 result2 = Vector3.Multiply(vector, value);

            Assert.AreEqual(expectedResult == result1, true);
            Assert.AreEqual(expectedResult == result2, true);
        }


        [TestCase(1f, 2f, 3f, 1f, 4f, 2f, 1f, 0.5f, 1.5f)]
        [TestCase(1f, 2f, 3f, -1f, -4f, -2f, -1f, -0.5f, -1.5f)]
        public void DivisionTest(float x1, float y1, float z1, float x2, float y2, float z2,
            float expectedX, float expectedY, float expectedZ)
        {
            Vector3 vector1 = new Vector3(x1, y1, z1);
            Vector3 vector2 = new Vector3(x2, y2, z2);
            Vector3 expectedResult = new Vector3(expectedX, expectedY, expectedZ);

            Vector3 result1 = vector1 / vector2;
            Vector3 result2 = Vector3.Divide(vector1, vector2);

            Assert.AreEqual(expectedResult == result1, true);
            Assert.AreEqual(expectedResult == result2, true);
        }


        [TestCase(1f, 2f, 3f, 2f, 0.5f, 1f, 1.5f)]
        [TestCase(1f, 2f, 3f, -2f, -0.5f, -1f, -1.5f)]
        public void DivisionWithConstTest(float x, float y, float z, float value,
            float expectedX, float expectedY, float expectedZ)
        {
            Vector3 vector = new Vector3(x, y, z);
            Vector3 expectedResult = new Vector3(expectedX, expectedY, expectedZ);

            Vector3 result1 = vector / value;
            Vector3 result2 = Vector3.Divide(vector, value);

            Assert.AreEqual(expectedResult == result1, true);
            Assert.AreEqual(expectedResult == result2, true);
        }


        [TestCase(1, 0, 0, 1, 0, 0)]
        [TestCase(0, 1, 0, 0, 1, 0)]
        [TestCase(0, 0, 1, 0, 0, 1)]
        [TestCase(3f, 4f, 0f, 0.6f, 0.8f, 0f)]
        [TestCase(3f, 0f, 4f, 0.6f, 0f, 0.8f)]
        public void NormalizeTest(float x, float y, float z, float expectedX, float expectedY, float expectedZ)
        {
            Vector3 vector = new Vector3(x, y, z);
            Vector3 expectedResult = new Vector3(expectedX, expectedY, expectedZ);

            float norm1 = vector.Norm;
            float norm2 = vector.Normalize();

            Assert.AreEqual(expectedResult == vector, true);
            Assert.AreEqual(norm1, norm2);
        }


        [TestCase(1, 0, 0, 1, 0, 0)]
        [TestCase(0, 0, 0, 0, 0, 0)]
        [TestCase(2f, 4f, 8f, 0.5f, 0.25f, 0.125f)]
        [TestCase(-2f, -4f, -8f, -0.5f, -0.25f, -0.125f)]
        [TestCase(0.5f, 0.25f, 0.125f, 2f, 4f, 8f)]
        public void InverseTest(float x, float y, float z, float expectedX, float expectedY, float expectedZ)
        {
            Vector3 vector = new Vector3(x, y, z);
            Vector3 expectedResult = new Vector3(expectedX, expectedY, expectedZ);

            Vector3 result = vector.Inverse();

            Assert.AreEqual(expectedResult == result, true);
        }


        [TestCase(1, 2, 3, 0, 0, 0, 0)]
        [TestCase(1, 2, 3, 1, 1, 1, 6)]
        [TestCase(1, 2, 3, 3, 2, 1, 10)]
        [TestCase(1, 2, 3, -3, -2, -1, -10)]
        public void DotTest(float x1, float y1, float z1, float x2, float y2, float z2, float expectedResult)
        {
            Vector3 vector1 = new Vector3(x1, y1, z1);
            Vector3 vector2 = new Vector3(x2, y2, z2);

            Assert.AreEqual(Vector3.Dot(vector1, vector2), expectedResult);
        }


        [TestCase(1, 0, 0, 0, 1, 0, 0, 0, 1)]
        [TestCase(1, 1, 1, 1, 1, 1, 0, 0, 0)]
        [TestCase(1, 2, 3, 4, 5, 6, -3, 6, -3)]
        public void CrossTest(float x1, float y1, float z1, float x2, float y2, float z2,
            float expectedX, float expectedY, float expectedZ)
        {
            Vector3 vector1 = new Vector3(x1, y1, z1);
            Vector3 vector2 = new Vector3(x2, y2, z2);
            Vector3 expectedResult = new Vector3(expectedX, expectedY, expectedZ);

            Assert.AreEqual(Vector3.Cross(vector1, vector2) == expectedResult, true);
        }
    }
}
