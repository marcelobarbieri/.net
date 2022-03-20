using System.Net.WebSockets;
using System.Text;

// adiciona suporte a WebSocket
using var ws = new ClientWebSocket();
await ws.ConnectAsync(new Uri("ws://localhost:5138/"), CancellationToken.None);

// o consumo será um array de bytes assim como a produção de mensagens
var buffer = new byte[256];

// escuta as mensagens enquanto a conexão estiver aberta
while (ws.State == WebSocketState.Open)
{
    // recebe as mensagens
    var result = await ws.ReceiveAsync(buffer, CancellationToken.None);

    // verifica se a conexão foi encerrada
    if (result.MessageType == WebSocketMessageType.Close)
        // fecha o WebSocket
        await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None);
    else
        // mostra a mensagem obtida na tela
        Console.WriteLine(Encoding.ASCII.GetString(buffer, 0, result.Count));
}