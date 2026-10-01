using System.Data;using System.Diagnostics.CodeAnalysis;using Microsoft.VisualBasic;
using System.Dynamic;using System.Net;using System.Net.Http;
using System.Security.Cryptography.X509Certificates;using System.Text.Json;using System.Text;
class ApiClient
{
      private readonly HttpClient _client;
      public ApiClient()
      {
            var handler = new HttpClientHandler
            {
                  ServerCertificateCustomValidationCallback = 
                        HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
      };
      _client = new HttpClient(handler);
      _client.DefaultRequestHeaders.Add("User-Agent", "MyApp/1.0");
}
public async Task<TokenClass> GetToken(string url, HttpContent content, string AUTH_KEY)
      {
            var response = new HttpRequestMessage(HttpMethod.Post, url);
            response.Headers.Add("Accept","application/json");
            response.Headers.Add("RqUID", Guid.NewGuid().ToString());
            response.Headers.Add("Authorization", $"Basic {AUTH_KEY}");
            response.Content = content;

            var responsE = await _client.SendAsync(response);
            var json = await responsE.Content.ReadAsStringAsync();
            Console.WriteLine($"==== STATUS CODE = {responsE.StatusCode} ====\n\n\n");
            TokenClass answer =  JsonSerializer.Deserialize<TokenClass>(json);
            return answer;
      }
      public async Task<Message> SendMessage(string murl, List<ChatMessage> history, string token)
      {
            var Messageresponse = new HttpRequestMessage(HttpMethod.Post, murl);
            Messageresponse.Headers.Add("Accept","application/json");
            Messageresponse.Headers.Add("Authorization",$"Bearer {token}");
            var mcontent = new
            {
                 model="GigaChat",
                  messages = history,
                  temperature = 0.7,
                  max_tokens = 500
            };
            string body = JsonSerializer.Serialize(mcontent);
            Messageresponse.Content = new StringContent(body,Encoding.UTF8,"application/json");
            var MessageresponsE = await _client.SendAsync(Messageresponse);
            var json = await MessageresponsE.Content.ReadAsStringAsync();
            Message message = JsonSerializer.Deserialize<Message>(json);
            message.json = json;
            return message;
      }
      public async Task<Models> GetModels(string token)
      {
            var Modelresponse = new HttpRequestMessage(HttpMethod.Get,"https://gigachat.devices.sberbank.ru/api/v1/models");
            Modelresponse.Headers.Add("Accept","application/json");
            Modelresponse.Headers.Add("Authorization",$"Bearer {token}");
            var ModelresponsE = await _client.SendAsync(Modelresponse);
            var json = await ModelresponsE.Content.ReadAsStringAsync();
            Models models = JsonSerializer.Deserialize<Models>(json);
            models.json = json;
            return models;
      }
}
class TokenClass{   public string access_token {get;set;}public TokenClass(){}}

class Models{public ModelsInfo[] data {get;set;}public string json {get;set;}public Models(){}}
class ModelsInfo{public string id {get;set;}public string type {get;set;}}
class Message{
      public MessageData[] choices {get;set;}
      public MessageUsage usage {get;set;}
      public string json {get;set;}public Message(){}}
class MessageData{public ChatMessage message {get;set;}public MessageData(){}}
class ChatMessage{public string content {get;set;}public string role {get;set;}public ChatMessage(){}}
class MessageUsage{public int prompt_tokens {get;set;}public int total_tokens {get;set;}}