using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Accord.Statistics.Filters;
using System.Data;

namespace Tabular
{
    public class FilterDescriptor
    {
        public IFilter Filter { get; private set; }
        public string Name { get; private set; }

        public FilterDescriptor(IFilter filter, string name)
        {
            Name = name;
            Filter = filter;
        }
    }
}
