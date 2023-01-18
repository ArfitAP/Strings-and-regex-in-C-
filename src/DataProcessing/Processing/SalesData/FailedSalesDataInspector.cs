namespace DataProcessing;

internal class FailedSalesDataInspector
{
    private readonly ILogger _logger;

    public FailedSalesDataInspector(ProcessingOptions options) => _logger = options.LoggerFactory.CreateLogger<FailedSalesDataInspector>();

    public void InspectAll(IEnumerable<string> failedRows)
    {
        foreach (var failedRow in failedRows)
            Inspect(failedRow);
    }

    public void Inspect(string failedRow)
    {
        ArgumentNullException.ThrowIfNull(failedRow);

        // TODO - Implementation
        var separatorCount = failedRow.Count(c => c.Equals('|'));

        if(separatorCount < 6)
        {
            _logger.LogWarning("Too few elements");
            return;
        }

        if(separatorCount > 6) 
        {
            _logger.LogWarning("Too many elements");
            return;
        }

        var lastSeparatorIndex = failedRow.LastIndexOf('|');
        var categoryColonIndex = failedRow.IndexOf("|", lastSeparatorIndex);

        if(categoryColonIndex == -1)
        {
            _logger.LogWarning("Missing colon");
            return;
        }

        if (categoryColonIndex == failedRow.Length - 1)
        {
            _logger.LogWarning("Missing colon");
            return;
        }

        var codeLength = 0;
        for(var index = lastSeparatorIndex + 1; index < categoryColonIndex; index++)
        {
            if (!char.IsWhiteSpace(failedRow[index]))
            {
                codeLength++;
            }
        }

        if(codeLength != 6)
        {
            _logger.LogWarning("Invalid code length");
            return;
        }

        var hasDesc = false;
        for (var index = categoryColonIndex + 1; index < failedRow.Length; index++)
        {
            if (!char.IsWhiteSpace(failedRow[index]))
            {
                hasDesc = true;
                break;
            }
        }

        if(!hasDesc)
        {
            _logger.LogWarning("Invalid description");
            return;
        }

    }
}