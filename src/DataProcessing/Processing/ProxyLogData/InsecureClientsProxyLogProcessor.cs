using System.Net;
using System.Text.RegularExpressions;

namespace DataProcessing;

internal class InsecureClientsProxyLogProcessor : Processor<InsecureHostsByIPAddressDictionary>
{
    // language=regex
    private const string RegexPattern = @"^\[WARN\].+?Z\s(?<ip>(?:\d{1,3}\.){3}\d{1,3}).+?(?:http:\/\/)(?<hostname>[^:\/]+)[^\s]* Insecure scheme detected.*$";

    public InsecureClientsProxyLogProcessor(ProcessingOptions processingOptions) : base(processingOptions)
    {
    }

    public override async Task<InsecureHostsByIPAddressDictionary> ProcessAsync(string filename, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(filename);

        var dataReader = new DataReader(Path.Combine(BaseInputPath, filename));
        var logData = await dataReader.ReadAllRowsAsync(cancellationToken);

        // TODO - Match
        var match = Regex.Match(logData, RegexPattern, RegexOptions.Multiline, TimeSpan.FromSeconds(1));

        var insecureHostsByIp = new InsecureHostsByIPAddressDictionary();

        while(match.Success)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (IPAddress.TryParse(match.Groups["ip"].Value, out var iPAddress))
            {
                insecureHostsByIp.AddWhenUnique(iPAddress, match.Groups["hostname"].Value);
            }

            match = match.NextMatch();
        }

        // TODO - Process Matches

        return insecureHostsByIp;
    }
}
