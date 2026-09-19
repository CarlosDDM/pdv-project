namespace Pdv.Application.Interfaces;

public interface IStorageService
{
    Task<string> UploadAsync(Stream content, string contentType, string keyPrefix, CancellationToken ct);
    Task DeleteAsync(string key, CancellationToken ct);
    string GetPublicUrl(string key);
}
