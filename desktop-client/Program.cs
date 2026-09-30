using System.Formats.Asn1;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Nodes;
using static ApiClient;
using System.Text;
using System.Security.Cryptography.X509Certificates;


string keys = File.ReadAllText("keys.json");
using var doc = JsonDocument.Parse(keys);
string AUTH_KEY = doc.RootElement.GetProperty("AUTH_KEY").GetString();

var content = new StringContent(
                  "scope=GIGACHAT_API_PERS",
                  Encoding.UTF8,
                  "application/x-www-form-urlencoded"
            );


string url="https://ngw.devices.sberbank.ru:9443/api/v2/oauth";
string murl="https://gigachat.devices.sberbank.ru/api/v1/chat/completions";
Console.WriteLine($"url == {url}");

var user = new ApiClient();
TokenClass token = await user.GetToken(url, content, AUTH_KEY);
Console.WriteLine($"{token.access_token}\n\n");

Models models = await user.GetModels(token.access_token);
foreach (var m in models.data)
{
    Console.WriteLine($"{m.id}  ({m.type})");
}
Console.WriteLine($"\n{models.json}");
Console.Write("message: ");
string Messagecontent = Console.ReadLine();
var mcontent = new
{
      model="GigaChat",
      messages = new[]
      {
            new { role = "user", content = Messagecontent }
      },
      temperature = 0.7,
      max_tokens = 500
};
string body = JsonSerializer.Serialize(mcontent);
Message answer = await user.SendMessage(murl, body, token.access_token);
Console.WriteLine($"\n\n{answer.json}");
string promptTokens;
string totalTokens;
string messageRole;
string messageContent;
Console.WriteLine($"\n\n{answer.choices[0].message.role}: {answer.choices[0].message.content} | {answer.usage.prompt_tokens} pTOK");