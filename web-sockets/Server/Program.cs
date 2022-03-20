using System.Net;
using System.Net.WebSockets;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// --- WebSockets ------------------------------

// adiciona suporte a WebSockets
app.UseWebSockets();

// app assincrono
// rota: "/", raiz da aplicação -> WebSocket para o cliente
app.Map("/", async context =>
{
    // o WebSocket pode tratar vários tipos de requisição
    // será tratado apenas requisições de pedido de estabelecimento de WebSocket
    if (!context.WebSockets.IsWebSocketRequest)
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
    else
    {
        // instância do WebSocket
        using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
        while (true)
        {
            // com a requisição pode-se enviar mensagens para os clientes conectados com SendAsync
            // envio de mensagem para o cliente a cada segundo
            await webSocket.SendAsync(
                Encoding.ASCII.GetBytes($".NET Rocks -> {DateTime.Now}"),
                WebSocketMessageType.Text,
                true, CancellationToken.None);

            // delay de 1s
            await Task.Delay(1000);
        }
    }
});

// ------------------------------ WebSockets ---

app.Run();
