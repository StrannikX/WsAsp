# WsAsp
Web Sockets library for Asp Net Core.

```csharp

/// TestWsHandler.cs
using System;
using System.Text;
using System.Threading.Tasks;
using WsAsp;

...

public class TestWsHandler : WsAsp
{
    public override Task OnConnectedAsync(HttpContext ctx)
    {
        return Task.CompletedTask;
    }

    public override Task OnDisconnectedAsync(WsCloseMessage closeMessage)
    {
        return Task.CompletedTask;
    }

    public override async Task OnReceiveAsync(WsReceiveMessage message)
    {
        if (!message.IsText) return;
        var text = message.GetString(Encoding.UTF8);

        if (text.Equals("Ding"))
        {
            await SendTextAsync("Dong", Encoding.UTF8);
        }
    }
}

```

```csharp 
// Startup.cs

public void ConfigureServices(IServiceCollection services)
{
    ...

    services.AddWsAsp();
    services.AddWsAspHandler<TestWsHandler>();

    ...
}

public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    ...

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapWsAspHandler<TestWsHandler>("/ws");
    };
    
    ...
}

```