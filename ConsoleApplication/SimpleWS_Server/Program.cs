using System.Net;
using System.Text;
using System.Net.WebSockets;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://localhost:6969");
var app = builder.Build();
app.UseWebSockets();
app.Map("/ws", async context =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        using var ws = await context.WebSockets.AcceptWebSocketAsync();
        while(true)
        {
            var message = "The current time is: " + DateTime.Now.ToString("HH:mm:ss");
            var bytes = Encoding.UTF8.GetBytes(message);
            var ArraySegment = new ArraySegment<byte>(bytes,0, bytes.Length);
            if (ws.State == System.Net.WebSockets.WebSocketState.Open)
                await ws.SendAsync(
                    ArraySegment, 
                    WebSocketMessageType.Text, 
                    true, 
                    CancellationToken.None
                    );
            else if (ws.State == WebSocketState.Closed || ws.State == WebSocketState.Aborted)
            {
                break;
            }
            Thread.Sleep(2000);
        }
    }
    else
    {
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
    }
});


await app.RunAsync();
