using System.Text;

namespace DataProcessing.Reporting;

internal class CustomerDataUniqueCountriesWriter : DataWriter<IEnumerable<HistoricalCustomerData>>
{
    private readonly StringComparer _stringComparer;

    public CustomerDataUniqueCountriesWriter(ProcessingOptions options, CultureInfo cultureInfo) : base(options)
    {
        _stringComparer = StringComparer.Create(cultureInfo, true);
    }

    public CustomerDataUniqueCountriesWriter(ProcessingOptions options) : this(options, options.ApplicationCulture)
    {
    }

    protected override async Task WriteAsyncCore(
        string pathAndFileName, 
        IEnumerable<HistoricalCustomerData> data, 
        CancellationToken cancellationToken = default)
    {
        // TODO - Implementation
        var contries = new SortedSet<string>(data.Select(d => d.Country), _stringComparer);

        var stringBuilder = new StringBuilder();
        foreach(var country  in contries)
        {
            stringBuilder.AppendLine(country);
        }

        foreach(var writer in OutputWriters)
        {
            await writer.WriteDataAsync(stringBuilder.ToString(), pathAndFileName, cancellationToken);
        }

    }
}
