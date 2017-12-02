// Accord Unit Tests
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2017
// cesarsouza at gmail.com
//
//    This library is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Lesser General Public
//    License as published by the Free Software Foundation; either
//    version 2.1 of the License, or (at your option) any later version.
//
//    This library is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Lesser General Public
//    License along with this library; if not, write to the Free Software
//    Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
//

namespace Accord.Tests.Math
{
    using Accord.IO;
    using Accord.Math;
    using Accord.Tests.Math.Properties;
    using NUnit.Framework;
    using System;
    using System.IO;

    [TestFixture]
    public class MatReaderTest
    {
        public static FileStream GetMat(string resourceName)
        {
            string fileName = Path.Combine(TestContext.CurrentContext.TestDirectory, "Resources", "mat", resourceName);
            return new FileStream(fileName, FileMode.Open, FileAccess.Read);
        }

      
        [Test]
        public void matrix_test_int32()
        {
            string localPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "Resources", "mat");

            #region doc_matrix_int32
            // Let's say we would like to load different .mat files which can be found at:
            // https://github.com/accord-net/framework/blob/development/Unit%20Tests/Accord.Tests.Math/Resources/mat/

            // Let's assume they all currently reside in a "localPath" 
            // folder. So let's start by trying to load a 32-bit matrix:
            string pathInt32 = Path.Combine(localPath, "int32.mat");

            // Create a .MAT reader for the file:
            var reader = new MatReader(pathInt32);

            // Let's check what is the name of the variable we need to load:
            string[] names = reader.FieldNames; // should be { "a" }

            // Ok, so we have to load the matrix called "a".

            // However, what if we didn't know the matrix type in advance?
            // In this case, we could use the non-generic version of Read:
            object unknown = reader.Read("a");

            // And we could check it's type through C#:
            Type t = unknown.GetType(); // should be typeof(int[,])

            // Now we could either cast it to the correct type or
            // use the generic version of Read<> to read it again:
            int[,] matrix = reader.Read<int[,]>("a");

            // The a matrix should be equal to { 1, 2, 3, 4 }
            #endregion

            Assert.AreEqual(typeof(int[,]), t);
            Assert.AreEqual(new int[,]
            {
                { 1, 2, 3, 4 },
            }, matrix);
            Assert.AreEqual(new[] { "a" }, names);
        }

        [Test]
        public void matrix_test_bytes()
        {
            string localPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "Resources", "mat");

            #region doc_matrix_byte
            // Let's say we would like to load different .mat files which can be found at:
            // https://github.com/accord-net/framework/blob/development/Unit%20Tests/Accord.Tests.Math/Resources/mat/

            // Let's assume they all currently reside in a "localPath" 
            // folder. So let's start by trying to load a 8-bit matrix:
            string pathInt8 = Path.Combine(localPath, "int8.mat");

            // Create a .MAT reader for the file:
            var reader = new MatReader(pathInt8);

            // The variable in the file is called "arr"
            sbyte[,] matrix = reader.Read<sbyte[,]>("arr");

            // The arr matrix should be equal to { -128, 127 }

            // (in case he didn't know the name of the variable,
            // we would have inspected the FieldNames property:
            string[] names = reader.FieldNames; // should contain "arr"
            #endregion

            Assert.AreEqual(new sbyte[,]
            {
                { -128, 127 },
            }, matrix);
            Assert.AreEqual(new[] { "arr" }, names);
        }

        [Test]
        public void structure_test()
        {
            string localPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "Resources", "mat");

            #region doc_structure
            // Let's say we would like to load a .mat file
            // called "simplestruct.mat". It can be found at
            // https://github.com/accord-net/framework/blob/development/Unit%20Tests/Accord.Tests.Math/Resources/mat/simplestruct.mat

            // Let's assume it currently resides in a "localPath" folder
            string fileName = Path.Combine(localPath, "simplestruct.mat");

            // Create a .MAT reader for the file:
            var reader = new MatReader(fileName);

            // We can extract some basic information about the file:
            string description = reader.Description; // "MATLAB 5.0 MAT-file, Platform: PCWIN"
            int version = reader.Version;     // 256
            bool bigEndian = reader.BigEndian;   // false

            // Enumerate the fields in the file
            foreach (var field in reader.Fields)
                Console.WriteLine(field.Key); // "structure"

            // We have the single following field
            var structure = reader["structure"];

            // Enumerate the fields in the structure
            foreach (var field in structure.Fields)
                Console.WriteLine(field.Key); // "a", "string"

            // Check the type for the field "a"
            var aType = structure["a"].ValueType; // byte[,]

            // Retrieve the field "a" from the file
            var a = structure["a"].GetValue<byte[,]>();

            // We can also do directly if we know the type in advance
            var s = reader["structure"]["string"].GetValue<string>();
            #endregion

            Assert.AreEqual(typeof(byte[,]), aType);
            Assert.AreEqual(typeof(string), reader["structure"]["string"].ValueType);

            Assert.AreEqual(
                "MATLAB 5.0 MAT-file, Platform: PCWIN, Created on: Thu Feb 22 01:39:50 2007",
                reader.Description);
            Assert.AreEqual(256, reader.Version);
            Assert.IsFalse(reader.BigEndian);

            byte[,] expected =
            {
                { 1, 2, 3 },
                { 4, 5, 6 },
            };

            Assert.IsTrue(expected.IsEqual(a));
            Assert.AreEqual("ala ma kota", s);
        }






        [Test]
        public void readInt8()
        {
            var file = GetMat("int8.mat");
            MatReader reader = new MatReader(file);

            Assert.AreEqual(
                "MATLAB 5.0 MAT-file, Platform: PCWIN, Created on: Wed Jun 27 17:40:39 2007",
                reader.Description);

            Assert.AreEqual(256, reader.Version);
            Assert.IsFalse(reader.BigEndian);

            var node = reader["arr"];
            var value = node.Value as sbyte[,];

            sbyte[,] expected = 
            {
                { -128, 127 },
            };

            Assert.IsTrue(expected.IsEqual(value));
        }

        [Test]
        public void readInt32()
        {
            var file = GetMat("int32.mat");
            MatReader reader = new MatReader(file);

            Assert.AreEqual(
                "MATLAB 5.0 MAT-file, Platform: PCWIN, Created on: Tue Dec 04 11:46:17 2012",
                reader.Description);

            Assert.AreEqual(256, reader.Version);
            Assert.IsFalse(reader.BigEndian);

            var node = reader["a"];
            var value = node.Value as int[,];

            int[,] expected = 
            {
                { 1, 2, 3, 4 },
            };

            Assert.IsTrue(expected.IsEqual(value));
        }

        [Test]
        public void readInt64()
        {
            var file = GetMat("int64.mat");
            MatReader reader = new MatReader(file);

            Assert.AreEqual(
                "MATLAB 5.0 MAT-file, Platform: PCWIN, Created on: Wed Jun 27 17:41:23 2007",
                reader.Description);

            Assert.AreEqual(256, reader.Version);
            Assert.IsFalse(reader.BigEndian);

            var node = reader["arr"];
            var value = node.Value as long[,];

            System.Int64[,] expected = 
            {
                { 0, -1 },
            };

            Assert.IsTrue(expected.IsEqual(value));
        }

        [Test]
        public void readInt64_2()
        {
            var file = GetMat("a64.mat");
            MatReader reader = new MatReader(file);

            Assert.AreEqual(
                "MATLAB 5.0 MAT-file, written by Octave 3.8.1, 2014-07-14 10:52:44 UTC",
                reader.Description);

            Assert.AreEqual(256, reader.Version);
            Assert.IsFalse(reader.BigEndian);

            var node = reader["A64"];
            var value = node.Value as long[,];

            long[,] expected = 
            {
               {  -83,  -91,  -92,  -93,   -1,   92,  -78,   42,  -92,    25 },
               {  -79,  -60,   96,  -23,  -85,  -44,   85,   48,   71,   -17 },
               {   42,   57,  -13,  -39,   54,  -18,    6,   23,   98,    51 },
               {  -62,   63,   43,   41,  -22,  -38,   22,  -93,   22,    34 },
               {   79,   64,   32,  -73,  -53,   -8,   75,   77,   23,     8 },
            };

            Assert.IsTrue(expected.IsEqual(value));
        }

        [Test]
        public void readUInt64()
        {
            var file = GetMat("uint64.mat");
            MatReader reader = new MatReader(file);

            Assert.AreEqual(
                "MATLAB 5.0 MAT-file, Platform: PCWIN, Created on: Wed Jun 27 17:43:04 2007",
                reader.Description);

            Assert.AreEqual(256, reader.Version);
            Assert.IsFalse(reader.BigEndian);

            var node = reader["arr"];
            var value = node.Value as ulong[,];

            System.UInt64[,] expected = 
            {
                { 0, unchecked ((System.UInt64)(-1)) },
            };

            Assert.IsTrue(expected.IsEqual(value));
        }

        [Test]
        public void readSingle()
        {
            var file = GetMat("single.mat");
            MatReader reader = new MatReader(file);

            Assert.AreEqual(
                "MATLAB 5.0 MAT-file, Platform: PCWIN, Created on: Wed Jun 04 13:29:10 2008",
                reader.Description);

            Assert.AreEqual(256, reader.Version);
            Assert.IsFalse(reader.BigEndian);

            var node = reader["arr"];
            var value = node.Value as float[,];

            float[,] expected = 
            {
                { 1.1f, 2.2f, 3.3f } 
            };

            Assert.IsTrue(expected.IsEqual(value));
        }

        [Test]
        public void readDouble()
        {
            var file = GetMat("matnativedouble.mat");
            MatReader reader = new MatReader(file);

            Assert.AreEqual(
                "MATLAB 5.0 MAT-file, Platform: PCWIN, Created on: Wed Feb 21 18:57:45 2007",
                reader.Description);

            Assert.AreEqual(256, reader.Version);
            Assert.IsFalse(reader.BigEndian);

            var node = reader["arr"];
            var value = node.Value as byte[,];

            byte[,] expected = 
            {
                { 1, 4 },
                { 2, 5 },
                { 3, 6 } 
            };

            Assert.IsTrue(expected.IsEqual(value));
        }

        [Test]
        public void readDouble2()
        {
            var file = GetMat("matnativedouble2.mat");
            MatReader reader = new MatReader(file);

            Assert.AreEqual(
                "MATLAB 5.0 MAT-file, Platform: PCWIN, Created on: Fri Mar 02 12:35:43 2007",
                reader.Description);

            Assert.AreEqual(256, reader.Version);
            Assert.IsFalse(reader.BigEndian);

            var node = reader["arr"];
            var value = node.Value as double[,];

            double[,] expected = 
            {
                { 1.1, 4.4 },
                { 2.2, 5.5 },
                { 3.3, 6.6 } 
            };

            Assert.IsTrue(expected.IsEqual(value));
        }

        [Test]
        public void readLogical()
        {
            var file = GetMat("logical.mat");
            MatReader reader = new MatReader(file);

            Assert.AreEqual(
                "MATLAB 5.0 MAT-file, Platform: PCWIN, Created on: Mon Feb 25 20:07:08 2013",
                reader.Description);

            Assert.AreEqual(256, reader.Version);
            Assert.IsFalse(reader.BigEndian);

            var node = reader["bool"];
            var value = node.Value as byte[,];

            byte[,] expected = 
            {
                { 1, 0 },
            };

            Assert.IsTrue(expected.IsEqual(value));
        }

        [Test]
        public void readStruct()
        {
            var file = GetMat("simplestruct.mat");
            MatReader reader = new MatReader(file);

            Assert.AreEqual(
                "MATLAB 5.0 MAT-file, Platform: PCWIN, Created on: Thu Feb 22 01:39:50 2007",
                reader.Description);

            Assert.AreEqual(256, reader.Version);
            Assert.IsFalse(reader.BigEndian);

            var node = reader["structure"];

            var value1 = node["a"];
            var value2 = node["string"];

            Assert.AreEqual("a", value1.Name);
            var a = value1.Value as byte[,];

            byte[,] expected = 
            {
                { 1, 2, 3 },
                { 4, 5, 6 },
            };

            Assert.IsTrue(expected.IsEqual(a));

            Assert.AreEqual("string", value2.Name);
            var s = value2.Value as string;
            Assert.AreEqual("ala ma kota", s);
        }

        [Test]
        public void readCell()
        {
            var file = GetMat("cell.mat");
            MatReader reader = new MatReader(file);

            Assert.AreEqual(
                "MATLAB 5.0 MAT-file, Platform: PCWIN, Created on: Thu Feb 22 03:12:25 2007",
                reader.Description);

            Assert.AreEqual(256, reader.Version);
            Assert.IsFalse(reader.BigEndian);

            var cel = reader["cel"];
            Assert.IsNotNull(cel["xBF"]);
            Assert.IsNotNull(cel["xY"]);
            Assert.IsNotNull(cel["nscan"]);
            Assert.IsNotNull(cel["Sess"]);
            Assert.IsNotNull(cel["xX"]);

            var xBF = cel["xBF"];

            Assert.AreEqual("xBF", xBF.Name);
            Assert.AreEqual(9, xBF.Count);

            var T = xBF["T"];
            var T0 = xBF["T0"];
            var dt = xBF["dt"];
            var UNITS = xBF["UNITS"];
            var name = xBF["name"];
            var order = xBF["order"];
            var bf = xBF["bf"];

            Assert.AreEqual(16, (T.Value as byte[,])[0, 0]);
            Assert.AreEqual(1, (T0.Value as byte[,])[0, 0]);
            Assert.AreEqual(0.1875, (dt.Value as double[,])[0, 0]);
            Assert.AreEqual("scans", UNITS.Value as string);
            Assert.AreEqual("hrf (with time derivative)", name.Value as string);
            Assert.AreEqual(2, (order.Value as byte[,])[0, 0]);
            Assert.IsTrue(expectedBfValues.IsEqual(bf.Value as double[,], 1e-15));

            var nscan = cel["nscan"];
            Assert.AreEqual(0, nscan.Count);
            Assert.AreEqual(96, (nscan.Value as byte[,])[0, 0]);

            var xY = cel["xY"];

            Assert.AreEqual("xY", xY.Name);
            Assert.AreEqual(1, xY.Count);

            var RT = xY["RT"];
            Assert.AreEqual(3, (RT.Value as byte[,])[0, 0]);

            var xX = cel["xX"];

            Assert.AreEqual("xX", xX.Name);
            Assert.AreEqual(6, xX.Count);

            var X = xX["X"];
            var iH = xX["iH"];
            var iC = xX["iC"];
            var iB = xX["iB"];
            var iG = xX["iG"];
            var xname = xX["name"];

            Assert.IsTrue(expectedxXValues.IsEqual(X.Value as double[,], 1e-15));

            Assert.AreEqual("Sn(1) test*bf(1)", xname["0"].Value);
            Assert.AreEqual("Sn(1) test*bf(2)", xname["1"].Value);
            Assert.AreEqual("Sn(1) constant", xname["2"].Value);



            var Sess = cel["Sess"];

            Assert.AreEqual(5, Sess.Count);

            var U = Sess["U"];

            Assert.AreEqual(7, U.Count);

            var Uname = U["name"];
            var Uons = U["ons"];
            var Udur = U["dur"];
            var Udt = U["dt"];
            var Uu = U["u"];
            var Upst = U["pst"];
            var P = U["P"];

            Assert.AreEqual("test", (Uname["0"] as MatNode).Value as string);
            Assert.AreEqual(8.00000000000000e+00, (Uons.Value as byte[,])[0, 0]);
            Assert.AreEqual(2.40000000000000e+01, (Uons.Value as byte[,])[1, 0]);
            Assert.AreEqual(4.00000000000000e+01, (Uons.Value as byte[,])[2, 0]);
            Assert.AreEqual(5.60000000000000e+01, (Uons.Value as byte[,])[3, 0]);
            Assert.AreEqual(7.20000000000000e+01, (Uons.Value as byte[,])[4, 0]);
            Assert.AreEqual(8.80000000000000e+01, (Uons.Value as byte[,])[5, 0]);

            for (int i = 0; i < 6; i++)
                Assert.AreEqual(8, (Udur.Value as byte[,])[i, 0]);

            Assert.AreEqual(1.87500000000000e-01, (Udt.Value as double[,])[0, 0]);

            var sparse = Uu.Value as MatSparse;
            Assert.AreEqual(774, sparse.Rows.Length);
            Assert.AreEqual(2, sparse.Columns.Length);
            Assert.AreEqual(774, sparse.Values.Length);

            int j = 0;
            for (int i = 160; i <= 288; i++, j++)
                Assert.AreEqual(i - 1, sparse.Rows[j]);

            for (int i = 416; i <= 544; i++, j++)
                Assert.AreEqual(i - 1, sparse.Rows[j]);

            for (int i = 672; i <= 800; i++, j++)
                Assert.AreEqual(i - 1, sparse.Rows[j]);

            for (int i = 928; i <= 1056; i++, j++)
                Assert.AreEqual(i - 1, sparse.Rows[j]);

            for (int i = 1184; i <= 1312; i++, j++)
                Assert.AreEqual(i - 1, sparse.Rows[j]);

            for (int i = 1440; i <= 1568; i++, j++)
                Assert.AreEqual(i - 1, sparse.Rows[j]);

            Assert.AreEqual(774, j);
            for (int i = 0; i < sparse.Values.Length; i++)
                Assert.AreEqual(1.0, sparse.Values.GetValue(i));

            Assert.AreEqual(2, sparse.Columns.Length);
            Assert.AreEqual(0, sparse.Columns[0]);
            Assert.AreEqual(774, sparse.Columns[1]);

            Assert.AreEqual(-21, (Upst.Value as short[,])[0, 0]);
            Assert.AreEqual(24, (Upst.Value as short[,])[0, 95]);


            var Pname = P["name"];
            var PP = P["P"];
            var Ph = P["h"];
            var Pi = P["i"];

            Assert.AreEqual("none", Pname.Value);
            var ppv = PP.Value as ushort[,];
            Assert.AreEqual(6, ppv.Length);
            Assert.AreEqual(2.40000000000000e+01, ppv[0, 0]);
            Assert.AreEqual(7.20000000000000e+01, ppv[1, 0]);
            Assert.AreEqual(1.20000000000000e+02, ppv[2, 0]);
            Assert.AreEqual(1.68000000000000e+02, ppv[3, 0]);
            Assert.AreEqual(2.16000000000000e+02, ppv[4, 0]);
            Assert.AreEqual(2.64000000000000e+02, ppv[5, 0]);

            Assert.AreEqual(0, (Ph.Value as byte[,])[0, 0]);
            Assert.AreEqual(1, (Pi.Value as byte[,])[0, 0]);

            var C = Sess["C"];
            Assert.AreEqual(2, C.Count);

            Assert.AreEqual(0, (C["C"].Value as byte[,]).Length);
            Assert.IsNull(C["name"].Value);

            var row = Sess["row"];
            for (int i = 0; i < 96; i++)
                Assert.AreEqual(i + 1, (row.Value as byte[,])[0, i]);

            var col = Sess["col"];
            Assert.AreEqual(1, (col.Value as byte[,])[0, 0]);
            Assert.AreEqual(2, (col.Value as byte[,])[0, 1]);

            var Fc = Sess["Fc"];

            var Fci = Fc["i"];
            var Fname = Fc["name"];

            Assert.AreEqual(1, (Fci.Value as byte[,])[0, 0]);
            Assert.AreEqual(2, (Fci.Value as byte[,])[0, 1]);
            Assert.AreEqual("test", Fname.Value);
        }



        private double[,] expectedBfValues = new double[,]
        {
#region bf values
            {0,0 },
            { 3.601796129597225e-07,3.398935362925512e-07 },
            { 9.555180381393791e-06,9.01701243737059e-06 },
            { 6.01540635015875e-05,5.676605957208259e-05 },
            { 0.0002101495370572841,0.0001983134711975782 },
            { 0.0005316775066875365,0.0005017323063630528 },
            { 0.001096792071016874,0.001034967939484985 },
            { 0.001965296292608711,0.00185051938139665 },
            { 0.003176563593356988,0.002962122160933246 },
            { 0.004745587114802391,0.004333532962928722 },
            { 0.006662652847610445,0.005886593199047978 },
            { 0.008895705361633635,0.007517462904457563 },
            { 0.01139443948618294,0.009113667341319395 },
            { 0.01409526531916605,0.01056837876589232 },
            { 0.01692647026486212,0.01179060832385636 },
            { 0.01981308951341808,0.01271124182076537 },
            { 0.0226811681221252,0.01328548389733397 },
            { 0.0254612410832852,0.01349252378330614 },
            { 0.02809096882983074,0.01333326447895277 },
            { 0.03051694593804833,0.01282687031738699 },
            { 0.03269575427457657,0.01200675044901568 },
            { 0.0345943634245192,0.01091644656649299 },
            { 0.03618999591748269,0.009605753576118904 },
            { 0.03746957709294589,0.008127282310792291 },
            { 0.03842888330238635,0.006533577381312466 },
            { 0.03907149068337534,0.0048748305465038 },
            { 0.03940761240064618,0.003197178129340495 },
            { 0.03945289683776827,0.001541536682602707 },
            { 0.03922724401190041,-5.708922183549504e-05 },
            { 0.03875368331097368,-0.001569902873543022 },
            { 0.03805734301752332,-0.002974292697396646 },
            { 0.0371645312326401,-0.004253565274201936 },
            { 0.03610193881102028,-0.005396501738540722 },
            { 0.03489596770294368,-0.006396796604250659 },
            { 0.03357218253061543,-0.00725242873462592 },
            { 0.03215487911897049,-0.007965004865632067 },
            { 0.0306667608490107,-0.008539107322583431 },
            { 0.02912871189714336,-0.008981669657730059 },
            { 0.02755965546854944,-0.009301397040136293 },
            { 0.02597648484509246,-0.009508242402855648 },
            { 0.02439405528839749,-0.009612944573072157 },
            { 0.02282522542862995,-0.009626630807348668 },
            { 0.02128093761367424,-0.009560483226648793 },
            { 0.0197703276973091,-0.009425466481161028 },
            { 0.01830085583320182,-0.009232112456806727 },
            { 0.01687845095550053,-0.008990356851424136 },
            { 0.0155076627224063,-0.008709421895244157 },
            { 0.01419181574440586,-0.008397739274690702 },
            { 0.0129331618919548,-0.008062907360073309 },
            { 0.01173302736446232,-0.007711677067944568 },
            { 0.01059195199600542,-0.007349961050855187 },
            { 0.00950981897072423,-0.006982861354709485 },
            { 0.008485973723452996,-0.006614711180120498 },
            { 0.007519331312612886,-0.006249126900463333 },
            { 0.006608471978354303,-0.005889067004126843 },
            { 0.005751724946191545,-0.005536895125919507 },
            { 0.004947240812406425,-0.005194444801603177 },
            { 0.004193053060044598,-0.004863084012824212 },
            { 0.003487129411128788,-0.00454377798299172 },
            { 0.002827413829259736,-0.004237149035926072 },
            { 0.002211860054176799,-0.003943532638148187 },
            { 0.001638457582742037,-0.00366302901352734 },
            { 0.001105251015271531,-0.003395549947620861 },
            { 0.0006103536676831894,-0.003140860590969993 },
            { 0.0001519563134876465,-0.002898616228815314 },
            { -0.00027166813042697,-0.00266839411229605 },
            { -0.000662163220168113,-0.002449720546412579 },
            { -0.001021090215778557,-0.002242093506042243 },
            { -0.001349931309369107,-0.00204500110616791 },
            { -0.00165009430472146,-0.001857936289138919 },
            { -0.001922918273795172,-0.001680408112957959 },
            { -0.002169679784698746,-0.001511950032807211 },
            { -0.002391599361560423,-0.001352125565596069 },
            { -0.002589847898458061,-0.001200531716318965 },
            { -0.002765552806476577,-0.001056800527319185 },
            { -0.002919803724638189,-0.0009205990888154628 },
            { -0.003053657671660031,-0.0007916283227085033 },
            { -0.003168143556161909,-0.0006696208229956395 },
            { -0.003264265998144233,-0.0005543380061490225 },
            { -0.003343008444452846,-0.0004455667944615679 },
            { -0.003405335585803669,-0.0003431160253678709 },
            { -0.003452195103079831,-0.000246812750713489 },
            { -0.003484518786410461,-0.0001564985623408065 },
            { -0.003503223082393373,-7.202605454249925e-05 },
            { -0.003509209133150965,6.744489842442936e-06 },
            { -0.003503362376137302,7.994812448328993e-05 },
            { -0.003486551776157423,0.0001477179298252371 },
            { -0.003459628761324988,0.0002101873395824487 },
            { -0.003423425933058172,0.0002674920891228104 },
            { -0.003378755617053545,0.0003197717838485882 },
            { -0.003326408317820323,0.000367171096002052 },
            { -0.003267151134101988,0.0004098406066770076 },
            { -0.003201726186634544,0.0004479373163446366 },
            { -0.003130849103422784,0.0004816248520794341 },
            { -0.003055207601272127,0.0005110734030644643 },
            { -0.002975460195867283,0.0005364594180467403 },
            { -0.002892235066383666,0.0005579650993748488 },
            { -0.002806129094580418,0.0005757777282510426 },
            { -0.002717707092642814,0.0005900888550292115 },
            { -0.002627501228789641,0.0006010933869413526 },
            { -0.00253601065488845,0.0006089886036745826 },
            { -0.002443701336060882,0.000613973128877438 },
            { -0.002351006078522911,0.0006162458830625603 },
            { -0.002258324748700532,0.0006160050405883351 },
            { -0.002166024673971603,0.0006134470105388589 },
            { -0.002074441213199127,0.0006087654584494041 },
            { -0.001983878483506479,0.0006021503830062818 },
            { -0.001894610228476022,0.0005937872591390201 },
            { -0.001806880812086508,0.0005838562563550141 },
            { -0.001720906322208958,0.0005725315387785677 },
            { -0.001636875767310268,0.0005599806511638225 },
            { -0.001554952350132764,0.0005463639931727984 },
            { -0.001475274802481353,0.0005318343824510933 },
            { -0.001397958765822938,0.0005165367054983886 },
            { -0.001323098203146963,0.0005006076540132699 },
            { -0.001250766828414536,0.0004841755432889002 },
            { -0.001181019540908006,0.0004673602083332894 },
            { -0.001113893852849863,0.0004502729726778425 },
            { -0.001049411299765636,0.0004330166843006872 },
            { -0.00098757882419373,0.0004156858127181837 },
            { -0.0009283901244782654,0.0003983666010660576 },
            { -0.0008718269614977664,0.0003811372668900362 },
            { -0.0008178604172708707,0.0003640682453747665 },
            { -0.0007664521004249505,0.0003472224688460075 },
            { -0.0007175552945072345,0.0003306556765667799 },
            { -0.0006711160460506819,0.0003144167491011049 },
            { -0.0006270741901739286,0.0002985480618251329 },
            { -0.0005853643122921709,0.0002830858525113814 },
            { -0.0005459166452411318,0.0002680605982886723 },
            { -0.0005086579017685707,0.0002534973976755228 },
            { -0.0004735120429280262,0.0002394163537913462 },
            { -0.0004404009834183463,0.0002258329552586328 },
            { -0.0004092452353524919,0.0002127584517156441 },
            { -0.0003799644923137289,0.0002002002212542039 },
            { -0.0003524781558691207,0.000188162127480564 },
            { -0.0003267058069640131,0.0001766448642609271 },
            { -0.0003025676248205929,0.0001656462865582918 },
            { -0.0002799847561133934,0.000155161726087787 },
            { -0.0002588796372991112,0.0001451842908155405 },
            { -0.0002391762730415988,0.0001357051475983622 },
            { -0.0002208004737002517,0.0001267137875080467 },
            { -0.0002036800548451046,0.0001181982736063161 },
            { -0.0001877450017292054,0.0001101454711325937 },
            { -0.0001729276015919148,0.0001025412602398943 },
            { -0.0001591625465896878,9.53707315634928e-05 },
            { -0.0001463870100568889,8.861836503453036e-05 },
            { -0.0001345406986913446,8.226819245806615e-05 },
            { -0.0001235658831410624,7.63039444625596e-05 },
            { -0.0001134074093417951,7.070918249832532e-05 },
            { -0.0001040126928230042,6.546741661627186e-05 },
            { -9.533169806380272e-05,6.056220979784847e-05 },
            { -8.731690484300951e-05,5.597726963334786e-05 },
            { -7.99232633897427e-05,5.169652816049947e-05 },
            { -7.310814000479113e-05,4.770421067959144e-05 },
            { -6.683125468929758e-05,4.398489435703426e-05 },
            { -6.105461218710864e-05,4.052355741720531e-05 },
            { -5.574242772149944e-05,3.730561970378027e-05 },
            { -5.086104858622703e-05,3.431697536805785e-05 },
            { -4.637887263581875e-05,3.154401841340591e-05 },
            { -4.226626461080931e-05,2.897366179344721e-05 },
            { -3.849547113072497e-05,2.659335072723442e-05 },
            { -3.504053509103747e-05,2.439107085858443e-05 },
            { -3.187721011016032e-05,2.235535184938244e-05 },
            { -2.89828755888739e-05,2.047526695865351e-05 },
            { -2.63364528671714e-05,1.87404291210447e-05 },
            { -2.391832289238013e-05,1.714098400051346e-05 },
            { -2.17102457472531e-05,1.566760045777108e-05 },
            { -1.969528232742678e-05,1.431145883373906e-05 },
            { -1.78577184038482e-05,1.306423741619283e-05 },
            { -1.618299125730482e-05,1.191809742304469e-05 },
            { -1.465761902870992e-05,1.086566680354231e-05 }
#endregion
        };

        private double[,] expectedxXValues = new double[,]
        {
#region xX values
            { 0,0,1 },
            { 0,0,1 },
            { 0,0,1 },
            { 0,0,1 },
            { 0,0,1 },
            { 0,0,1 },
            { 0,0,1 },
            { 0,0,1 },
            { 3.601796129597225e-07,3.398935362925512e-07,1 },
            { 0.1377261675373094,0.09531327122382906,1 },
            { 0.7178453643258904,0.1576286132525797,1 },
            { 1.074719559575214,0.01675102323460202,1 },
            { 1.14408788011833,-0.06042003774631569,1 },
            { 1.104958592859363,-0.07978828696028833,1 },
            { 1.052554750693593,-0.07509710030062655,1 },
            { 1.019908696134792,-0.06572145176389985,1 },
            { 1.005962713591808,-0.0597312739704222,1 },
            { 0.8891441694411503,-0.139065721097039,1 },
            { 0.3172518220952767,-0.2204920614104011,1 },
            { -0.06298653221075204,-0.08078481276325353,1 },
            { -0.1443595482487565,0.001429531173312774,1 },
            { -0.1084107879624424,0.02321936174886793,1 },
            { -0.05536087978817366,0.01935076556817072,1 },
            { -0.02130665490061492,0.009915876008691338,1 },
            { -0.006471011313963291,0.003662998800927133,1 },
            { 0.1361706846320783,0.09632475078467274,1 },
            { 0.7176091050725762,0.1577975434163021,1 },
            { 1.074719559575214,0.01675102323460202,1 },
            { 1.14408788011833,-0.06042003774631569,1 },
            { 1.104958592859363,-0.07978828696028833,1 },
            { 1.052554750693593,-0.07509710030062655,1 },
            { 1.019908696134792,-0.06572145176389985,1 },
            { 1.005962713591808,-0.0597312739704222,1 },
            { 0.8891441694411503,-0.139065721097039,1 },
            { 0.3172518220952767,-0.2204920614104011,1 },
            { -0.06298653221075204,-0.08078481276325353,1 },
            { -0.1443595482487565,0.001429531173312774,1 },
            { -0.1084107879624424,0.02321936174886793,1 },
            { -0.05536087978817366,0.01935076556817072,1 },
            { -0.02130665490061492,0.009915876008691338,1 },
            { -0.006471011313963291,0.003662998800927133,1 },
            { 0.1361706846320783,0.09632475078467274,1 },
            { 0.7176091050725762,0.1577975434163021,1 },
            { 1.074719559575214,0.01675102323460202,1 },
            { 1.14408788011833,-0.06042003774631569,1 },
            { 1.104958592859363,-0.07978828696028833,1 },
            { 1.052554750693593,-0.07509710030062655,1 },
            { 1.019908696134792,-0.06572145176389985,1 },
            { 1.005962713591808,-0.0597312739704222,1 },
            { 0.8891441694411503,-0.139065721097039,1 },
            { 0.3172518220952767,-0.2204920614104011,1 },
            { -0.06298653221075204,-0.08078481276325353,1 },
            { -0.1443595482487565,0.001429531173312774,1 },
            { -0.1084107879624424,0.02321936174886793,1 },
            { -0.05536087978817366,0.01935076556817072,1 },
            { -0.02130665490061492,0.009915876008691338,1 },
            { -0.006471011313963291,0.003662998800927133,1 },
            { 0.1361706846320783,0.09632475078467274,1 },
            { 0.7176091050725762,0.1577975434163021,1 },
            { 1.074719559575214,0.01675102323460202,1 },
            { 1.14408788011833,-0.06042003774631569,1 },
            { 1.104958592859363,-0.07978828696028833,1 },
            { 1.052554750693593,-0.07509710030062655,1 },
            { 1.019908696134792,-0.06572145176389985,1 },
            { 1.005962713591808,-0.0597312739704222,1 },
            { 0.8891441694411503,-0.139065721097039,1 },
            { 0.3172518220952767,-0.2204920614104011,1 },
            { -0.06298653221075204,-0.08078481276325353,1 },
            { -0.1443595482487565,0.001429531173312774,1 },
            { -0.1084107879624424,0.02321936174886793,1 },
            { -0.05536087978817366,0.01935076556817072,1 },
            { -0.02130665490061492,0.009915876008691338,1 },
            { -0.006471011313963291,0.003662998800927133,1 },
            { 0.1361706846320783,0.09632475078467274,1 },
            { 0.7176091050725762,0.1577975434163021,1 },
            { 1.074719559575214,0.01675102323460202,1 },
            { 1.14408788011833,-0.06042003774631569,1 },
            { 1.104958592859363,-0.07978828696028833,1 },
            { 1.052554750693593,-0.07509710030062655,1 },
            { 1.019908696134792,-0.06572145176389985,1 },
            { 1.005962713591808,-0.0597312739704222,1 },
            { 0.8891441694411503,-0.139065721097039,1 },
            { 0.3172518220952767,-0.2204920614104011,1 },
            { -0.06298653221075204,-0.08078481276325353,1 },
            { -0.1443595482487565,0.001429531173312774,1 },
            { -0.1084107879624424,0.02321936174886793,1 },
            { -0.05536087978817366,0.01935076556817072,1 },
            { -0.02130665490061492,0.009915876008691338,1 },
            { -0.006471011313963291,0.003662998800927133,1 },
            { 0.1361706846320783,0.09632475078467274,1 },
            { 0.7176091050725762,0.1577975434163021,1 },
            { 1.074719559575214,0.01675102323460202,1 },
            { 1.14408788011833,-0.06042003774631569,1 },
            { 1.104958592859363,-0.07978828696028833,1 },
            { 1.052554750693593,-0.07509710030062655,1 },
            { 1.019908696134792,-0.06572145176389985,1 },
#endregion
        };
    }
}
