using System.Formats.Asn1;using System.Runtime.CompilerServices;
using static ApiClient;using System.Collections.Generic;
using System.Text.Json;using System.Text.Json.Nodes;using System.Text;
using System.Security.Cryptography.X509Certificates;using System.Security.Cryptography;
//=== parsing `keys.json` file
string keys = File.ReadAllText("keys.json");
using var doc = JsonDocument.Parse(keys);
string AUTH_KEY = doc.RootElement.GetProperty("AUTH_KEY").GetString();
//== sending http request for token
//- making `x-www-form-urlencoded` type content for request
var content = new StringContent(
                  "scope=GIGACHAT_API_PERS",
                  Encoding.UTF8,
                  "application/x-www-form-urlencoded");
//- preparing http request content 
var user = new ApiClient();
string url="https://ngw.devices.sberbank.ru:9443/api/v2/oauth";
//!- sending http request
TokenClass token = await user.GetToken(url,content,AUTH_KEY);
//display url for logs
Console.WriteLine($"url == {url}");
//=== sending http request (get) for show avaiable models
//!- sending http request with our access token
Models models = await user.GetModels(token.access_token);
//- parsed json unpacking by foreach
foreach (var m in models.data)
{
    Console.WriteLine($"{m.id}  ({m.type})");
}
//== ai chating
//- preparing chat history
var history = new List<ChatMessage>();
//!- cycle `while` for endless chatting with ai.. muheheheh
while(true){
      //- message reading
      Console.Write("Message: ");
      string userMessagecontent = Console.ReadLine();
      //- break if
      if(userMessagecontent=="/exit")break;
      //- history add
      history.Add(new ChatMessage {role="user",content=userMessagecontent});
      //- preparing url
      string murl="https://gigachat.devices.sberbank.ru/api/v1/chat/completions";
      //- http request
      Message answer = await user.SendMessage(murl, history, token.access_token);
      //- history
      history.Add(new ChatMessage {role="assistant",content="answer.choices[0].content"});
      //- display text
      Console.WriteLine($"\n\n{answer.choices[0].message.role}: {answer.choices[0].message.content} | {answer.usage.prompt_tokens} pTOK");
}
//-------------------