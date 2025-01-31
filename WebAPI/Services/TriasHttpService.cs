using System.Text;
using System.Xml.Serialization;
using TRIAS.NET.Models.Trias;
using TRIAS.NET.WebAPI.Helper;

namespace TRIAS.NET.WebAPI.Services;

public abstract class TriasHttpService<TRequest, TResponse>
{
    private readonly HttpClient _httpClient;
    private readonly XmlSerializer _serializer;
    private readonly string _requestorRef;

    public TriasHttpService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClient = httpClientFactory.CreateClient("TriasClient");
        _serializer = new XmlSerializer(typeof(Trias));
        _requestorRef = configuration.GetValue<string>("TRIAS.NET:RequestorRef") ?? "";
    }

    protected async Task<TResponse> Request(TRequest request, CancellationToken cancellationToken)
    {
        var message = await CreateTriasMessage(request);
        var response = await SendTriasMessage(message, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            throw new TriasException(((int)response.StatusCode), await response.Content.ReadAsStringAsync(cancellationToken));
        }
        var result = await response.Content.ReadAsStringAsync();
        var resultStream = new MemoryStream(Encoding.UTF8.GetBytes(result));
        var serialized = _serializer.Deserialize(resultStream);
        if (serialized is Trias trias && trias.Item is ServiceDeliveryStructure serviceDelivery && serviceDelivery.DeliveryPayload.Item is TResponse payload)
        {
            return payload;
        }
        else
        {
            throw new Exception("Invalid response");
        }
    }

    protected async Task<HttpResponseMessage> SendTriasMessage(Trias message, CancellationToken cancellationToken)
    {
        using var memoryStream = new MemoryStream();
        _serializer.Serialize(memoryStream, message);
        var xml = Encoding.UTF8.GetString(memoryStream.ToArray());
        var content = new StringContent(xml, Encoding.UTF8, "text/xml");
        var result = await _httpClient.PostAsync("", content, cancellationToken);
        return result;
    }

    private Task<Trias> CreateTriasMessage(TRequest request)
    {
        return Task.FromResult(new Trias
        {
            Item = new ServiceRequestStructure
            {
                RequestTimestamp = System.DateTime.Now,
                RequestorRef = new ParticipantRefStructure1 { Value = _requestorRef },
                RequestPayload = new RequestPayloadStructure
                {
                    Item = request
                }
            }
        });
    }
}
