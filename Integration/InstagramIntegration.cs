using System.Text.Json;
using Mirra_Orchestrator.Exception;
using Mirra_Orchestrator.Helpers;
using Mirra_Orchestrator.Integration.Interfaces;
using Mirra_Orchestrator.Integration.Model.Request;
using Mirra_Orchestrator.Model;

namespace Mirra_Orchestrator.Integration
{
    public class InstagramIntegration : IInstagramIntegration
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly SymmetricEncryptionHelper _symmetricEncryptionHelper;

        public InstagramIntegration(
            IHttpClientFactory httpClientFactory,
            SymmetricEncryptionHelper symmetricEncryptionHelper)
        {
            _httpClientFactory = httpClientFactory;
            _symmetricEncryptionHelper = symmetricEncryptionHelper;
        }

        public async Task<string> PublishPhotoPost(CustomerPlatformConfiguration config, InstagramPost post)
        {
            if (string.IsNullOrWhiteSpace(config.ExternalAccountId))
                throw new InstagramException("ExternalAccountId (IG user id) is not configured for this customer platform row.");
            if (string.IsNullOrWhiteSpace(config.AccessToken))
                throw new InstagramException("AccessToken is not configured for this customer platform row.");

            var accessToken = _symmetricEncryptionHelper.Decrypt(config.AccessToken);
            var igUserId = config.ExternalAccountId.Trim();
            // Instagram cap for captions; Graph will reject or truncate unpredictably if exceeded.
            var caption = post.Caption ?? string.Empty;
            if (caption.Length > 2200)
                caption = caption.Substring(0, 2200);

            var client = _httpClientFactory.CreateClient("instagram");

            // 1) Create media container
            using var formCreate = new MultipartFormDataContent();
            formCreate.Add(new StringContent(post.ImageUrl), "image_url");
            formCreate.Add(new StringContent(caption), "caption");
            formCreate.Add(new StringContent(accessToken), "access_token");

            var createResponse = await client.PostAsync(new Uri($"{igUserId}/media", UriKind.Relative), formCreate);
            var createJson = await createResponse.Content.ReadAsStringAsync();
            var containerId = ParseIdOrThrow(createJson, createResponse, "Failed to create Instagram media container");

            // 2) Wait until the container is ready
            await WaitForContainerReadyAsync(client, containerId, accessToken);

            // 3) Publish
            using var formPublish = new MultipartFormDataContent();
            formPublish.Add(new StringContent(containerId), "creation_id");
            formPublish.Add(new StringContent(accessToken), "access_token");

            var publishResponse = await client.PostAsync(new Uri($"{igUserId}/media_publish", UriKind.Relative), formPublish);
            var publishJson = await publishResponse.Content.ReadAsStringAsync();
            return ParseIdOrThrow(publishJson, publishResponse, "Failed to publish Instagram media");
        }

        private static string ParseIdOrThrow(string json, HttpResponseMessage response, string contextMessage)
        {
            using var doc = JsonDocument.Parse(string.IsNullOrWhiteSpace(json) ? "{}" : json);
            var root = doc.RootElement;
            if (root.TryGetProperty("error", out var err))
            {
                var message = err.TryGetProperty("message", out var m) ? m.GetString() : root.GetRawText();
                throw new InstagramException($"{contextMessage}: {message}");
            }
            if (!response.IsSuccessStatusCode)
                throw new InstagramException($"{contextMessage}: HTTP {response.StatusCode} — {json}");

            if (root.TryGetProperty("id", out var idEl))
            {
                var id = idEl.GetString();
                if (!string.IsNullOrEmpty(id))
                    return id;
            }
            throw new InstagramException($"{contextMessage}: response did not contain a valid 'id' field. Body: {json}");
        }

        private static async Task WaitForContainerReadyAsync(HttpClient client, string containerId, string accessToken)
        {
            for (var attempt = 0; attempt < 30; attempt++)
            {
                var relative = $"{containerId}?fields=status_code&access_token={Uri.EscapeDataString(accessToken)}";
                using var response = await client.GetAsync(new Uri(relative, UriKind.Relative));
                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(string.IsNullOrWhiteSpace(json) ? "{}" : json);
                var root = doc.RootElement;
                if (root.TryGetProperty("error", out var err))
                {
                    var message = err.TryGetProperty("message", out var m) ? m.GetString() : json;
                    throw new InstagramException($"Error checking container status: {message}");
                }
                if (root.TryGetProperty("status_code", out var sc))
                {
                    var code = sc.GetString();
                    if (string.Equals(code, "FINISHED", StringComparison.OrdinalIgnoreCase))
                        return;
                    if (string.Equals(code, "ERROR", StringComparison.OrdinalIgnoreCase))
                        throw new InstagramException($"Instagram media container status ERROR. Body: {json}");
                }
                await Task.Delay(2000);
            }
            throw new InstagramException("Timeout waiting for Instagram media container to reach FINISHED status.");
        }
    }
}
