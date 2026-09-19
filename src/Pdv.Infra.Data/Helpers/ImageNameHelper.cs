namespace Pdv.Infra.Data.Helpers;

public static class ImageNameHelper
{
    private static readonly Dictionary<string, string> Extensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ["image/jpeg"] = ".jpg",
        ["image/png"] = ".png",
        ["image/webp"] = ".webp"
    };

    private static string GetExtension(string contentType)
    {
        return Extensions.GetValueOrDefault(contentType, string.Empty);
    }

    public static string ChangeImageName(string contentType)
    {
        var extension = GetExtension(contentType);
        return $"{Guid.NewGuid()}-{DateTime.UtcNow:yyyyMMddHHmmss}{extension}";
    }
}
