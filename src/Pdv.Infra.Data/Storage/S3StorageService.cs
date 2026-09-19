using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;
using Pdv.Application.Interfaces;
using Pdv.Infra.Data.Helpers;

namespace Pdv.Infra.Data.Storage;

public sealed class S3StorageService : IStorageService
{
    private readonly IAmazonS3 _client;
    private readonly string _bucketName;
    private readonly string _baseUrl;

    public S3StorageService(IAmazonS3 client, IConfiguration configuration)
    {
        _client = client;
        _bucketName = configuration["AWS:S3:BucketName"]!;
        _baseUrl = configuration["AWS:S3:BaseUrl"]!;
    }
    public async Task DeleteAsync(string key, CancellationToken ct)
    {
        await _client.DeleteObjectAsync(new DeleteObjectRequest 
        { 
            BucketName = _bucketName,
            Key = key 
        }, ct);
    }

    public string GetPublicUrl(string key)
    {
        return $"{_baseUrl}/{key}";
    }

    public async Task<string> UploadAsync(Stream content, string contentType, string keyPrefix, CancellationToken ct)
    {
        var key = $"{keyPrefix}/{ImageNameHelper.ChangeImageName(contentType)}";

        var request = new PutObjectRequest
        {
            BucketName = _bucketName,
            Key = key,
            InputStream = content,
            ContentType = contentType
        };

        await _client.PutObjectAsync(request, ct);
        return key;
    }
}
