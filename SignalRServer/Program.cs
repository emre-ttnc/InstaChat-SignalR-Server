using SignalRServer.Hubs;

var builder = WebApplication.CreateBuilder(args);

//Configure CORS Policy # CORS Politikas�n�n Ayarlanmas�
builder.Services.AddCors(options => options.AddDefaultPolicy(policy => policy
    .AllowCredentials()
    .AllowAnyHeader()
    .AllowAnyMethod()
    .SetIsOriginAllowed(X => true)
    ));

//Adding SignalR Service # SignalR'�n Servislere Eklenmesi
builder.Services.AddSignalR();



var app = builder.Build();

//Use CORS Policy # CORS Politikas�n�n Uygulanmas�
app.UseCors();

//Default(Index) Page # Varsay�lan(Ana) Sayfa / Rota
//app.MapGet("/", () => "Hello World!");

//SignalR ChatHub Routing # SignalR ChatHub'�n Link/Rotaya Eklenmesi
app.MapHub<ChatHub>("/chathub");



app.Run();
