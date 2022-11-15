using Microsoft.AspNetCore.SignalR;
using SignalRServer.Data;
using SignalRServer.Models;

namespace SignalRServer.Hubs;

public class ChatHub : Hub
{
    public async Task ConnectUserWithNickName(string nickName)
    {
        if (ClientSource.ClientList.Exists(c => c.ConnectionId == Context.ConnectionId))
            await Clients.Caller.SendAsync("registeredClient", ClientSource.ClientList.FirstOrDefault(c => c.ConnectionId == Context.ConnectionId)?.NickName);
        else
        {
            //if nickname is not null or empty then trim it # Nickname boş ise traşla
            nickName = string.IsNullOrEmpty(nickName) ? nickName : nickName.Trim();

            if (!string.IsNullOrEmpty(nickName))
            {
                if (ClientSource.ClientList.Exists(c => c.NickName == nickName))
                    await Clients.Caller.SendAsync("thisNicknameRegistered");
                else
                {
                    //Create a client # Bir istemci oluşturma
                    Client client = new()
                    {
                        NickName = nickName,
                        ConnectionId = Context.ConnectionId
                    };

                    //Adding client to client list # İstemciyi istemci listesine ekleme
                    ClientSource.ClientList.Add(client);

                    //Warning to other clients for new client # Diğer istemcileri yeni istemci için uyarma
                    await Clients.Others.SendAsync("clientJoined", nickName);
                    await Clients.All.SendAsync("clientListUpdated", ClientSource.ClientList);
                }
            }
            else
            {
                await Clients.Caller.SendAsync("thisNicknameRegistered");
            }
        }
    }

    public async Task SendMessageAsync(string message, string clientName)
    {
        if (!string.IsNullOrEmpty(clientName))
            clientName = clientName.Trim();
        Client? sender = ClientSource.ClientList.FirstOrDefault(c => c.ConnectionId == Context.ConnectionId);

        //is Sender Registered? # Gönderici kayıtlı mı?
        if (sender != null)
        {
            if (string.IsNullOrEmpty(clientName) || clientName == "General Chat")
                await Clients.Others.SendAsync("receiveMessage", message, $"<b>{sender.NickName}</b> - General Chat");
            else
            {
                Client? client = ClientSource.ClientList.FirstOrDefault(c => c.NickName == clientName);
                if (client != null)
                {
                    await Clients.Client(client.ConnectionId).SendAsync("receiveMessage", message, $"<b>{sender.NickName}</b> whispered:");
                }
                else
                    await Clients.Caller.SendAsync("receiveMessage", "Something gone wrong. Message not delivered.", "System Message");
            }
        }
        else
            await Clients.Caller.SendAsync("receiveMessage", "Something gone wrong. Message not delivered.", "System Message");
    }


    //When a client disconnected
    public async override Task OnDisconnectedAsync(Exception? exception)
    {
        Client? client = ClientSource.ClientList.FirstOrDefault(c => c.ConnectionId == Context.ConnectionId);
        if(client != null)
        {
            await Clients.Others.SendAsync("clientLeft", client.NickName);
            ClientSource.ClientList.Remove(client);
            await Clients.All.SendAsync("clientListUpdated", ClientSource.ClientList);
        }
    }
}
