# ASP.NET WebSockets

## O que são WebSockets?

WebSockets (WS) é uma tecnologia que permite mantermos clientes conectados aos servidores e trafegar mensagens mais rápidas.

Um requisição tem alguns passos a serem seguidos, dentre eles o **handshake** (aperto de mão) e abertura da conexão.

Somente depois destes passos os dados são trafegados, e em seguida a conexão é fechada.
Neste modelo mais convencional, estes passos são realizados à cada requisição ao servidor, enquanto nos **WebSockets**, uma vez conectados, somente os dados são trafegados.

### SignalR

O ASP.NET possui um pacote chamado **SignalR** que encapsula os **WebSockets** trazendo mais funcionalidades.
Inclusive o Blazor Server se beneficia destes pontos.

Embora o **SignalR** seja um excelente pacote e principal recomendação com **WS** será utilizado apenas recursos nativos do .NET.

## Iniciando a aplicação

Deverão ser criadas duas aplicações, uma que agirá como servidor, onde estarão os **WebSockets**, e outra como cliente que irá consumi-los.

```ps
dotnet new web -o Server
```

## Adicionando suporte a WebSockets

Tudo o que é preciso para adicionar suporte a **WebSockets** nas aplicações ASP.NET é a linha **app.UseWebSockets()**
Esta chamada não exige nenhum pacote adicional.

```c#
var app = builder.Build();
app.UseWebSockets();
```

Melhorando o código tornando-o assíncrono:

```c#
app.Map("/", async context =>
{
    //ToDO
});
await app.RunAsync();
```

Note que tem-se apenas uma rota **_"/"_** que será a raiz da aplicação, também servindo como um **WebSocket** para o cliente.

### Verificando a conexão

Para adicionar suporte aos **WebSockets** utilizando **app.UseWebSockets**, tem-se acesso aos mesmos dentro do **context.WebSockets**, que permite manipular a requisição como um **WS**

Como o endpoint receberá vários tipos de requisição, pode-se tratar apenas as requisições que fazem sentido para o respectivo cenário, verificando se **IsWebSocketRequest** é verdadeiro.

```c#
if (!context.WebSockets.IsWebSocketRequest)
    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
```

### Manipulando as requisições

O primeiro passo para manipular as requisições é ter uma instância do **WebSocket** e isto pode ser feito utilizando o método **AcceptWebSocketAsync**, conforme mostrado abaixo:

```c#
using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
```

Com a requisição em mãos, pode-se enviar mensagens para os clientes conectados utilizando o **SendAsync**, que espera os parâmetros:

- Mensagem (Array de bytes)
- Tipo da mensagem (Texto)
- Fim da mensagem (Sim, não serão enviadas mensagens segmentadas)
- Cancelation Token (Token para cancelamento da requisição)

```c#
var data = Encoding.ASCII.GetBytes($".NET Rockes -> {DateTime.Now}");
await webSocket.SendAsync(
    data, // Message
    WebSocketMessageType.Text, // Type
    true, // EndOfMessage
    CancellationToken.None
);
```

Para exemplificar melhor, o conteúdo será inserido dentro de um loop infinito, com delay de um segundo entre as mensagens.
Basicamente ele enviará a mesma mensagem para o cliente a cada segundo.

```c#
while(true)
{
    await webSocket.SendAsync(
        Encoding.ASCII.GetBytes($".NET Rocks -> {DataTime.Now}"),
        WebSocketMessageType.Text,
        true,
        CancellationToken.None
    );
    await Task.Delay(1000);
}
```

### Servidor

Os clientes já podem se conectar ao servidor e receber mensagens.

Segue abaixo código completo do servidor:

```c#
using System.Net;
using System.Net.WebSockets;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();
app.UseWebSockets();
app.Map("/", async context =>
{
    if (!context.WebSockets.IsWebSocketRequest)
        context.Response.StatusCode = (int) HttpStatusCode.BadRequest;
    else
    {
        using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
        while (true)
        {
            await webSocket.SendAsync(
                Encoding.ASCII.GetBytes($".NET Rocks -> {DateTime.Now}"),
                WebSocketMessageType.Text,
                true, CancellationToken.None);
            await Task.Delay(1000);
        }
    }
});
await app.RunAsync();
```

## Cliente

Para consumir as mensagens da aplicação será criado um cliente do tipo **Consolle Application** que ficará lendo as mensagens infinitamente.

dotnet new console -o Cliente

### Adicionando suporte a WebSocket

Assim como foi feito no servidor, será necessário adicionar suporte aos **WebSockets** também, e isto é feito sem adição de qualquer pacote, apenas utilizando o **ClientWebSocket**

```c#
using var ws = new ClientWebSocket();
await ws.ConnectAsync(new Uri("ws://localhost:5065"), CancellationToken.None);
```

É importante notar a porta que a aplicação (servidor) rodará.

### Consumindo mensagens

Assim como a produção de mensagens foi um array de bytes, o consumo também será.

```c#
var buffer = new bytes[256];
```

Em seguida, as mensagens poderão ser ouvidas do servidor enquanto a conexão estiver aberta (**ws.State Open**)

```c#
while (ws.State == WebSocketState.Open)
{
    // do something...
}
```

### Recebendo as mensagens

Será utilizado o método **ReceiveAsync** que precisará do **buffer** definido e do **CancellationToken**.

```c#
var result = await ws.ReceiveAsync(buffer, CancellationToken.None);
```

Caso a mensagem seja diferente de **WebSocketMessageType.Close**, que significa que a conexão foi encerrada, ela será convertida para uma **String** e mostrada na tela.

Caso a mensagem seja de conexão encerrada, será fechado o **WebSocket** e encerrar a conexão.

```c#
if (result.MessageType == WebSocketMessageType.Close)
    await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None);
else
    Console.WriteLine(Encoding.ASCII.GetString(buffer, 0, result.Count));
```

## Cliente

E isto é tudo o que é necessário do lado do cliente.

Agora tem-se uma aplicação que produz mensagens e outra que consome, ambas usando **WebSocket** nativos do .NET, sem necessidade de pacotes externos.

```c#
using System.Net.WebSockets;
using System.Text;

using var ws = new ClientWebSocket();
await ws.ConnectAsync(new Uri("ws://localhost:5065/"), CancellationToken.None);

var buffer = new byte[256];
while (ws.State == WebSocketState.Open)
{
    var result = await ws.ReceiveAsync(buffer, CancellationToken.None);
    if (result.MessageType == WebSocketMessageType.Close)
        await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None);
    else
        Console.WriteLine(Encoding.ASCII.GetString(buffer, 0, result.Count));
}
```

## Conclusão

Utilizar **WebSockets** no ASP.NET/ .NET é uma tarefa simples e fácil, que não necessita de bibliotecas externas para o mesmo.

# Referências

[Balta.io](https://balta.io/blog/aspnet-websockets)
