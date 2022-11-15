using SignalRServer.Hubs;

var builder = WebApplication.CreateBuilder(args);

//Configure CORS Policy # CORS Politikasýnýn Ayarlanmasý
builder.Services.AddCors(options => options.AddDefaultPolicy(policy => policy
    .AllowCredentials()
    .AllowAnyHeader()
    .AllowAnyMethod()
    .SetIsOriginAllowed(X => true)
    ));

//Adding SignalR Service # SignalR'ýn Servislere Eklenmesi
builder.Services.AddSignalR();



var app = builder.Build();

//Use CORS Policy # CORS Politikasýnýn Uygulanmasý
app.UseCors();

//Default(Index) Page # Varsayýlan(Ana) Sayfa / Rota
//app.MapGet("/", () => "Hello World!");

//SignalR ChatHub Routing # SignalR ChatHub'ýn Link/Rotaya Eklenmesi
app.MapHub<ChatHub>("/chathub");



app.Run();
