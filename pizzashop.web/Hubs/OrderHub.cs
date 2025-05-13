
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace pizzashop.Hubs 
{
    public class OrderHub : Hub
    {
        //Join or leave group
        public async Task JoinOrderGroup(string orderId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Order_{orderId}");
        }
        public async Task LeaveOrderGroup(string orderId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Order_{orderId}");
        }
    }
}