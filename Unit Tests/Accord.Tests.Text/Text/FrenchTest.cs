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

namespace Accord.Tests.MachineLearning
{
    using System;
    using NUnit.Framework;
    using Accord.MachineLearning.Text.Stemmers;

    [TestFixture]
    public class FrenchTest
    {
        [Test]
        public void BaseTest_NormalPath1()
        {
            FrenchStemmer stemmer = new FrenchStemmer();

            string[] inputs = 
            {
                "continu", "continua", "continuait", "continuant", "continuation", "continue",
                "continué", "continuel", "continuelle", "continuellement", "continuelles",
                "continuels", "continuer", "continuera", "continuerait", "continueront",
                "continuez", "continuité", "continuons", "contorsions", "contour", "contournait",
                "contournant", "contourne", "contours", "contractait", "contracté", "contractée",
                "contracter", "contractés", "contractions", "contradictoirement", "contradictoires",
                "contraindre", "contraint", "contrainte", "contraintes", "contraire", "contraires",
                "contraria"
            };

            string[] stemmed =
            {
                "continu", "continu", "continu", "continu", "continu", "continu", "continu", 
                "continuel", "continuel", "continuel", "continuel", "continuel", "continu", 
                "continu", "continu", "continu", "continu", "continu", "continuon", "contors", 
                "contour", "contourn", "contourn", "contourn", "contour", "contract", "contract", 
                "contract", "contract", "contract", "contract", "contradictoir", "contradictoir", 
                "contraindr", "contraint", "contraint", "contraint", "contrair", "contrair", "contrari", 
            };

            for (int i = 0; i < inputs.Length; i++)
            {
                string expected = stemmed[i];
                string actual = stemmer.Stem(inputs[i]);
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void BaseTest_NormalPath2()
        {
            FrenchStemmer stemmer = new FrenchStemmer();

            string[] inputs = 
            {
                "main", "mains", "maintenaient", "maintenait", "maintenant", "maintenir", "maintenue",
                "maintien", "maintint", "maire", "maires", "mairie", "mais", "maïs", "maison", "maisons",
                "maistre", "maitre", "maître", "maîtres", "maîtresse", "maîtresses", "majesté", "majestueuse",
                "majestueusement", "majestueux", "majeur", "majeure", "major", "majordome", "majordomes",
                "majorité", "majorités", "mal", "malacca", "malade", "malades", "maladie", "maladies", "maladive"
            };

            string[] stemmed =
            {
                "main", "main", "mainten", "mainten", "mainten", "mainten", "maintenu", "maintien", "maintint",
                "mair", "mair", "mair", "mais", "maï", "maison", "maison", "maistr", "maitr", "maîtr", "maîtr", 
                "maîtress", "maîtress", "majest", "majestu", "majestu", "majestu", "majeur", "majeur", "major",
                "majordom", "majordom", "major", "major", "mal", "malacc", "malad", "malad", "malad", "malad", 
                "malad"
            };

            for (int i = 0; i < inputs.Length; i++)
            {
                string expected = stemmed[i];
                string actual = stemmer.Stem(inputs[i]);
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void AccentTest()
        {
            FrenchStemmer stemmer = new FrenchStemmer();

            Assert.AreEqual("majest", stemmer.Stem("majesté"));
            Assert.AreEqual("affection", stemmer.Stem("affectionné"));
        }

        [Test]
        public void UnaccentTest()
        {
            FrenchStemmer stemmer = new FrenchStemmer();

            Assert.AreEqual("abneg", stemmer.Stem("abnégation"));
        }

        [Test]
        public void French_FullTest()
        {
            Tools.Test(new FrenchStemmer(), "french");
        }
    }
}
