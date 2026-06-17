
using System.Net.Http.Headers;

using HiTechStore.Core.Common.Interfaces.Infra;
using HiTechStore.Infrastructure.Utils;

using Microsoft.Extensions.Options;

namespace HiTechStore.Infrastructure.AssetStorage;

public class SupabaseAssetRegisterer : AssetRegistererBase, IPublicAssetRegisterer
{
    private readonly HttpClient _httpClient;
    private readonly SupabaseOptions _supabaseOptions;

    public SupabaseAssetRegisterer(
        HttpClient httpClient,
        IOptions<SupabaseOptions> supabaseOptions,
        IWellDistributedPathGenerator wellDistributedPathGenerator
    ) : base(wellDistributedPathGenerator)
    {
        _httpClient = httpClient;
        _supabaseOptions = supabaseOptions.Value;
    }

    private string List()
        => $"storage/v1/object/list/{_supabaseOptions.BucketName}".TrimEnd('/');

    private string Object(string path)
        => $"storage/v1/object/{_supabaseOptions.BucketName}/{NormalizeUrl(path)}".TrimEnd('/');

    private string PublicObject(string path)
        => $"storage/v1/object/public/{_supabaseOptions.BucketName}/{NormalizeUrl(path)}".TrimEnd('/');

    override public void DeleteFile(string relativePath)
    {
        using var response = _httpClient.DeleteAsync(Object(relativePath)).Result;

        response.EnsureSuccessStatusCode();
    }

    override public string GetPublicUrl(string relativePath)
    {
        return new Uri(
            _httpClient.BaseAddress!,
            PublicObject(relativePath)
        ).ToString();
    }

    class SupabaseObjectItem
    {
        public string name { get; set; } = default!;
        public string id { get; set; } = default!;
    }

    override public bool IsExist(string? relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
            return false;

        using var request = new HttpRequestMessage(
            HttpMethod.Head, PublicObject(relativePath)
        );

        using var response = _httpClient
            .SendAsync(request)
            .GetAwaiter()
            .GetResult();

        try
        {
            response.EnsureSuccessStatusCode();
        }
        catch
        {
            return false;
        }

        return true;
    }

    override public async Task SaveFileAsync(AppFile file, string filePublicPath)
    {
        using var content = new StreamContent(file.File);

        content.Headers.ContentType =
            new MediaTypeHeaderValue(file.ContentType);

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            Object(filePublicPath));

        request.Headers.Add("x-upsert", "true");

        request.Content = content;

        using var response = await _httpClient.SendAsync(request);

        response.EnsureSuccessStatusCode();

    }

    override public async Task<string> SaveFileAsync(AppFile file, WriteFileOptions options)
    {
        var objectPath = await ProvidePath(options, file.FileName);

        await SaveFileAsync(file, objectPath);

        return objectPath;
    }
}


public class SupabaseOptions
{
    public const string SectionName = "Supabase";
    public required string BaseUrl { get; set; }
    public required string SecretKey { get; set; }
    public required string BucketName { get; set; }
}

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSupabaseStorage(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddOptions<SupabaseOptions>()
            .Bind(configuration.GetSection(SupabaseOptions.SectionName))
            .Validate(x => !string.IsNullOrWhiteSpace(x.BaseUrl),
                "Supabase BaseUrl is required.")
            .Validate(x => !string.IsNullOrWhiteSpace(x.SecretKey),
                "Supabase SecretKey is required.")
            .Validate(x => !string.IsNullOrWhiteSpace(x.BucketName),
                "Supabase BucketName is required.")
            .ValidateOnStart();

        services.AddHttpClient<IPublicAssetRegisterer, SupabaseAssetRegisterer>(
            (sp, client) =>
            {
                var options = sp
                    .GetRequiredService<IOptions<SupabaseOptions>>()
                    .Value;

                client.BaseAddress = new Uri(options.BaseUrl!.TrimEnd('/') + "/");

                client.Timeout = TimeSpan.FromSeconds(30);

                client.DefaultRequestHeaders.Add(
                    "apikey",
                    options.SecretKey);

                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue(
                        "Bearer",
                        options.SecretKey);

                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue(
                        "application/json"));
            });

        return services;
    }
}