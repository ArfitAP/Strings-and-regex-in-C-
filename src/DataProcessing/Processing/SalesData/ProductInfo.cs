using System.Text.RegularExpressions;

namespace DataProcessing;

internal readonly struct ProductInfo : IEquatable<ProductInfo>
{
    //language=regex
    private const string ParsePattern = @"^(?<code>\d+)[:-](?:[a-z]#)?(?!.*#|.*-)(?<sku>[^(]+)";

    public const string InvalidValue = "INVALID";
    public static ProductInfo Invalid = new(InvalidValue, InvalidValue);

    public ProductInfo(string saleCode, string sku) => (SalesCode, Sku) = (saleCode, sku);

    public string SalesCode { get; }
    public string Sku { get; }

    public static ProductInfo Parse(string productInfoString)
    {
        ArgumentNullException.ThrowIfNull(productInfoString);
        // TODO - Implementation
        productInfoString = productInfoString.Replace(':', '-');

        if(string.Empty.Equals(productInfoString) || !productInfoString.Contains('-'))
        {
            return Invalid;
        }

        var parts = productInfoString.Split('-');
        if(parts.Length != 2 || string.IsNullOrEmpty(parts[0]) || string.IsNullOrEmpty(parts[1]))
        {
            return Invalid;
        }

        var salesCode = parts[0];
        var SKU = parts[1];

        if(salesCode.Any(c => !char.IsDigit(c)))
        {
            return Invalid;
        }

        if (char.IsLower(SKU[0]) && SKU[1].Equals('#'))
        {
            SKU = SKU[..2];
        }
        else if(SKU.Contains('#'))
        {
            return Invalid;
        }

        var parenStart = SKU.IndexOf('(');
        if(parenStart != -1)
        {
            SKU = SKU.Remove(parenStart);
        }

        return new ProductInfo(salesCode, SKU);
    }

    public static ProductInfo ParseUsingRegex(string productInfoString)
    {
        ArgumentNullException.ThrowIfNull(productInfoString);
        // TODO - Implementation
        var match = Regex.Match(productInfoString, ParsePattern, RegexOptions.None, TimeSpan.FromSeconds(1));

        return match.Success && match.Groups.ContainsKey("code") && match.Groups.ContainsKey("sku")
            ? new ProductInfo(match.Groups["code"].Value, match.Groups["sku"].Value)
            : Invalid;

    }

    public override bool Equals(object? obj) => obj is ProductInfo info && Equals(info);
    public bool Equals(ProductInfo other) => SalesCode == other.SalesCode && Sku == other.Sku;
    public override int GetHashCode() => HashCode.Combine(SalesCode, Sku);
    public static bool operator ==(ProductInfo left, ProductInfo right) => left.Equals(right);
    public static bool operator !=(ProductInfo left, ProductInfo right) => !(left == right);
}
