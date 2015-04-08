using Accord.Statistics.Filters;

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
