using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;

public class ChatHub : Hub
{
    public async Task SendMessageToGroup(string groupId, string user, string message)
    {
        // Send the message to the clients in the specified group
        await Clients.Group(groupId).SendAsync("ReceiveMessage", user, message);
    }

    public async Task JoinGroup(string groupId)
    {
        // Log group joining only when connection is established
        Debug.WriteLine($"User {Context.ConnectionId} is joining group {groupId}");
        await Groups.AddToGroupAsync(Context.ConnectionId, groupId);
    }

    public async Task LeaveGroup(string groupId)
    {
        // This would be useful if users leave the group at any point
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupId);
    }
}
