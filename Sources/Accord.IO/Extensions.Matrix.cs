using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Accord.IO.Csv;
using Accord.Math;

namespace Accord.IO
{
    public static partial class Formats
    {

        public static void Save(this double[,] matrix, Stream stream, TableFormat format)
        {
            switch (format)
            {
                case TableFormat.SerializedXml:
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(double[,]));
                        serializer.Serialize(stream, matrix);
                    } break;

                case TableFormat.SerializedBin:
                    {
                        BinaryFormatter serializer = new BinaryFormatter();
                        serializer.Serialize(stream, matrix);
                    } break;

                case TableFormat.Csv:
                    {
                        var writer = new CsvWriter(new StreamWriter(stream));
                        writer.Delimiter = ';';
                        writer.Write(matrix);
                    }
                    break;
                case TableFormat.Tsv:
                    {
                        var writer = new CsvWriter(new StreamWriter(stream));
                        writer.Delimiter = '\t';
                        writer.Write(matrix);
                    }
                    break;

                case TableFormat.CSharp:
                    {
                        var str = matrix.ToString(CSharpMatrixFormatProvider.InvariantCulture);
                        TextWriter writer = new StreamWriter(stream);
                        writer.WriteLine(str);
                    }
                    break;


                case TableFormat.OctaveMatFile:
                case TableFormat.OpenDocument:
                case TableFormat.OlderExcel:
                case TableFormat.LibSVM:
                case TableFormat.Idx:
                    throw new NotSupportedException();
            }
        }

        public static void Save(this double[][] matrix, Stream stream, TableFormat format)
        {
            switch (format)
            {
                case TableFormat.SerializedXml:
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(double[][]));
                        serializer.Serialize(stream, matrix);
                    } break;

                case TableFormat.SerializedBin:
                    {
                        BinaryFormatter serializer = new BinaryFormatter();
                        serializer.Serialize(stream, matrix);
                    } break;

                case TableFormat.Csv:
                    {
                        var writer = new CsvWriter(new StreamWriter(stream));
                        writer.Delimiter = ";";
                        writer.Write(matrix);
                    }
                    break;
                case TableFormat.Tsv:
                    {
                        var writer = new CsvWriter(new StreamWriter(stream));
                        writer.Delimiter = "\t";
                        writer.Write(matrix);
                    }
                    break;

                case TableFormat.CSharp:
                    {
                        var str = matrix.ToString(CSharpMatrixFormatProvider.InvariantCulture);
                        TextWriter writer = new StreamWriter(stream);
                        writer.WriteLine(str);
                    }
                    break;


                case TableFormat.OctaveMatFile:
                case TableFormat.OpenDocument:
                case TableFormat.OlderExcel:
                case TableFormat.LibSVM:
                case TableFormat.Idx:
                    throw new NotSupportedException();
            }
        }

    }
}
