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
    public enum TableFormat
    {
        SerializedXml,
        SerializedBin,
        OctaveMatFile,
        OpenDocument,
        OlderExcel,
        Csv,
        Tsv,
        LibSVM,
        Idx,
        CSharp
    }

    public static partial class Formats
    {
        public static void Save(this DataTable table, Stream stream, TableFormat format)
        {
            switch (format)
            {
                case TableFormat.SerializedXml:
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(DataTable));
                        serializer.Serialize(stream, table);
                    } break;

                case TableFormat.SerializedBin:
                    {
                        BinaryFormatter serializer = new BinaryFormatter();
                        serializer.Serialize(stream, table);
                    } break;

                case TableFormat.Csv:
                    {
                        var writer = new CsvWriter(new StreamWriter(stream));
                        writer.Delimiter = ";";
                        writer.Write(table);
                    }
                    break;
                case TableFormat.Tsv:
                    {
                        var writer = new CsvWriter(new StreamWriter(stream));
                        writer.Delimiter = "\t";
                        writer.Write(table);
                    }
                    break;

                case TableFormat.CSharp:
                    {
                        var matrix = table.ToMatrix();
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

        public static DataTable Load(Stream stream, TableFormat format)
        {
            switch (format)
            {
                case TableFormat.SerializedXml:
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(DataTable));
                        return (DataTable)serializer.Deserialize(stream);
                    }

                case TableFormat.SerializedBin:
                    {
                        BinaryFormatter serializer = new BinaryFormatter();
                        return (DataTable)serializer.Deserialize(stream);
                    }

                case TableFormat.OctaveMatFile:
                    {
                        MatReader reader = new MatReader(stream);
                        return reader.Fields.First().Value.GetValue<double[,]>().ToTable();
                    }

                case TableFormat.OpenDocument:
                    {
                        ExcelReader reader = new ExcelReader(stream, true);
                        string ws = reader.GetWorksheetList().First();
                        return reader.GetWorksheet(ws);
                    }

                case TableFormat.OlderExcel:
                    {
                        ExcelReader reader = new ExcelReader(stream, false);
                        string ws = reader.GetWorksheetList().First();
                        return reader.GetWorksheet(ws);
                    }

                case TableFormat.Csv:
                    {
                        CsvReader reader = new CsvReader(stream, true);
                        return reader.ToTable();
                    }

                case TableFormat.Tsv:
                    {
                        CsvReader reader = new CsvReader(stream, true, '\t');
                        return reader.ToTable();
                    }

                case TableFormat.LibSVM:
                    {
                        SparseReader reader = new SparseReader(stream);
                        return reader.ToTable();
                    }

                case TableFormat.Idx:
                    {
                        IdxReader reader = new IdxReader(stream);
                        return reader.ReadToEndAsVectors().ToTable();
                    }

                case TableFormat.CSharp:
                    throw new NotSupportedException();
            }
        }
    }
}
