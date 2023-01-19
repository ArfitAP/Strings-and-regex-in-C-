namespace DataProcessing.Reporting;

internal class SalesDataSummaryReportWriter : DataWriter<IEnumerable<HistoricalSalesData>>
{
    public SalesDataSummaryReportWriter(ProcessingOptions options) : base(options)
    {
    }

    protected override async Task WriteAsyncCore(
        string pathAndFileName, 
        IEnumerable<HistoricalSalesData> data, 
        CancellationToken cancellationToken = default)
    {
        var formattedOutput = ProduceReportString(data, cancellationToken);

        foreach (var writer in OutputWriters)
        {
            await writer.WriteDataAsync(formattedOutput, pathAndFileName, cancellationToken);
        }
    }

    private string ProduceReportString(
        IEnumerable<HistoricalSalesData> salesData, 
        CancellationToken cancellationToken = default)
    {
        // TODO - Implementation
        //var formattedOutput = "Data exported by " + SessionContext.Forename + " " + SessionContext.Surname + Environment.NewLine;
        var formattedOutput = $"Data exported by {SessionContext.Forename} {SessionContext.Surname}{Environment.NewLine}";

        foreach (var item in salesData.OrderBy(s => s.UtcSalesDateTime))
        {
            //formattedOutput += "Date: " + item.UtcSalesDateTime.ToString("D", Options.ApplicationCulture) + Environment.NewLine;
            formattedOutput += string.Format(Options.ApplicationCulture, "Date: {0:D}{1}", item.UtcSalesDateTime, Environment.NewLine);
            formattedOutput += "Product name: " + item.ProductName + Environment.NewLine;
            formattedOutput += "Product sku: " + item.ProductSku + Environment.NewLine;
        }

        return formattedOutput;
    }
}
