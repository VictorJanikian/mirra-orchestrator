using Mirra_Orchestrator.Exception;
using Mirra_Orchestrator.Integration.Interfaces;
using Mirra_Orchestrator.Model;
using System.Text.Json;
using static Mirra_Orchestrator.Helpers.Constants;

namespace Mirra_Orchestrator.Integration
{
    class InstagramIntegration : IInstagramIntegration
    {
        private static readonly TimeSpan MEDIA_CONTAINER_POLLING_INTERVAL = TimeSpan.FromSeconds(2);
        private const int MEDIA_CONTAINER_POLLING_ATTEMPTS = 10;

        private const string MEDIA_CONTAINER_READY = "FINISHED";
        private const string MEDIA_CONTAINER_IN_PROGRESS = "IN_PROGRESS";

        private const string AI_GENERATED_LABEL_FIELD = "is_ai_generated";
        private const string PAID_PARTNERSHIP_LABEL_FIELD = "is_paid_partnership";

        private readonly IRestClient _restClient;

        public InstagramIntegration(IRestClient restClient)
        {
            _restClient = restClient;
        }

        // A publicacao na Graph API tem duas etapas: primeiro sobe-se um container de midia
        // apontando para a imagem, depois esse container e publicado no perfil
        public async Task<string> PublishImagePost(CustomerPlatformConfiguration platformConfiguration, string imageUrl, string caption, InstagramPostLabels labels)
        {
            var accessToken = getRequiredConfiguration(platformConfiguration.InstagramAccessToken, nameof(platformConfiguration.InstagramAccessToken));
            var userId = getRequiredConfiguration(platformConfiguration.InstagramUserId, nameof(platformConfiguration.InstagramUserId));

            var creationId = await createMediaContainer(userId, accessToken, imageUrl, caption, labels);
            await waitUntilMediaContainerIsReady(creationId, accessToken);

            return await publishMediaContainer(userId.ToString(), accessToken, creationId);
        }

        private async Task<string> createMediaContainer(long? userId, string accessToken, string imageUrl, string caption, InstagramPostLabels labels)
        {
            var parameters = new Dictionary<string, string>
            {
                { "image_url", imageUrl },
                { "access_token", accessToken }
            };

            if (!string.IsNullOrWhiteSpace(caption))
                parameters.Add("caption", caption);

            if (labels != null && labels.IsAIGenerated)
                parameters.Add(AI_GENERATED_LABEL_FIELD, "true");

            if (labels != null && labels.IsPaidPartnership)
                parameters.Add(PAID_PARTNERSHIP_LABEL_FIELD, "true");

            using var content = new FormUrlEncodedContent(parameters);
            using var response = await _restClient.post(buildUserEndpoint(userId.ToString(), "media"), content);

            return await getIdFromResponse(response);
        }

        // O container e montado de forma assincrona: publicar antes de ele terminar de baixar a imagem falha
        private async Task waitUntilMediaContainerIsReady(string creationId, string accessToken)
        {
            for (var attempt = 1; attempt <= MEDIA_CONTAINER_POLLING_ATTEMPTS; attempt++)
            {
                var (statusCode, statusDetail) = await getMediaContainerStatus(creationId, accessToken);

                if (statusCode == MEDIA_CONTAINER_READY)
                    return;

                if (statusCode != MEDIA_CONTAINER_IN_PROGRESS)
                    throw new InstagramException($"O container de mídia '{creationId}' não pôde ser publicado ({statusCode}): {statusDetail}");

                if (attempt < MEDIA_CONTAINER_POLLING_ATTEMPTS)
                    await Task.Delay(MEDIA_CONTAINER_POLLING_INTERVAL);
            }

            throw new InstagramException($"O container de mídia '{creationId}' não ficou pronto após {MEDIA_CONTAINER_POLLING_ATTEMPTS} tentativas.");
        }

        private async Task<(string StatusCode, string StatusDetail)> getMediaContainerStatus(string creationId, string accessToken)
        {
            var endpoint = $"{INSTAGRAM_GRAPH_API_URL}/{creationId}?fields=status_code,status&access_token={Uri.EscapeDataString(accessToken)}";

            using var response = await _restClient.get(endpoint);

            var responseStream = await response.Content.ReadAsStreamAsync();
            using var responseJson = await JsonDocument.ParseAsync(responseStream);

            if (!responseJson.RootElement.TryGetProperty("status_code", out JsonElement statusCodeElement))
                throw new InstagramException("A resposta não contém o atributo 'status_code'.");

            var statusDetail = responseJson.RootElement.TryGetProperty("status", out JsonElement statusElement)
                ? statusElement.GetString()!
                : string.Empty;

            return (statusCodeElement.GetString()!, statusDetail);
        }

        private async Task<string> publishMediaContainer(string userId, string accessToken, string creationId)
        {
            var parameters = new Dictionary<string, string>
            {
                { "creation_id", creationId },
                { "access_token", accessToken }
            };

            using var content = new FormUrlEncodedContent(parameters);
            using var response = await _restClient.post(buildUserEndpoint(userId, "media_publish"), content);

            return await getIdFromResponse(response);
        }

        private string buildUserEndpoint(string userId, string edge)
        {
            return $"{INSTAGRAM_GRAPH_API_URL}/{userId}/{edge}";
        }

        private async Task<string> getIdFromResponse(HttpResponseMessage response)
        {
            var responseStream = await response.Content.ReadAsStreamAsync();
            using var responseJson = await JsonDocument.ParseAsync(responseStream);

            if (!responseJson.RootElement.TryGetProperty("id", out JsonElement idElement))
                throw new InstagramException("A resposta não contém o atributo 'id'.");

            return idElement.GetString()!;
        }

        private string getRequiredConfiguration(string value, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new InstagramException($"O campo '{fieldName}' não está preenchido para este cliente.");

            return value;
        }

        private long? getRequiredConfiguration(long? value, string fieldName)
        {
            if (!value.HasValue || value == 0)
                throw new InstagramException($"O campo '{fieldName}' não está preenchido para este cliente.");

            return value;
        }
    }
}
