// Accord (Experimental) Audio Library
// Accord.NET framework
// http://www.crsouza.com
//
// Copyright © César Souza, 2009-2011
// cesarsouza at gmail.com
//

namespace Accord.Audio.Filters
{
    using System.Collections.Generic;

    public class FormatConverter : BaseFilter
    {
        private SampleFormat destinationFormat;

        private Dictionary<SampleFormat, SampleFormat> formatTranslations = new Dictionary<SampleFormat, SampleFormat>();

        public override Dictionary<SampleFormat, SampleFormat> FormatTranslations
        {
            get { return formatTranslations; }
        }

        public FormatConverter(SampleFormat destinationFormat)
        {
            this.destinationFormat = destinationFormat;

            if (destinationFormat == SampleFormat.Format32BitIeeeFloat)
            {
                formatTranslations[SampleFormat.Format16Bit] = destinationFormat;
                formatTranslations[SampleFormat.Format32Bit] = destinationFormat;
            }
            else
            {
                throw new UnsupportedSampleFormatException();
            }
        }

        protected override void ProcessFilter(Signal sourceData, Signal destinationData)
        {
            int channels = sourceData.Channels;
            int length = sourceData.Length;
            SampleFormat dstFormat = destinationData.SampleFormat;
            SampleFormat srcFormat = sourceData.SampleFormat;

            if (dstFormat == SampleFormat.Format32BitIeeeFloat)
            {
                float dst;

                if (srcFormat == SampleFormat.Format16Bit)
                {
                    short src;
                    for (int c = 0; c < channels; c++)
                    {
                        for (int i = 0; i < length; i++)
                        {
                            src = (short)sourceData.GetSample(c, i);
                            SampleConverter.Convert(src, out dst);
                        }
                    }
                }
                else if (srcFormat == SampleFormat.Format32Bit)
                {
                    int src;
                    for (int c = 0; c < channels; c++)
                    {
                        for (int i = 0; i < length; i++)
                        {
                            src = (int)sourceData.GetSample(c, i);
                            SampleConverter.Convert(src, out dst);
                        }
                    }
                }
            }
        }
    }
}
