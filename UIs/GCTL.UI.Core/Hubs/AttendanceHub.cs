using Microsoft.AspNetCore.SignalR;

namespace GCTL.UI.Core.Hubs
{
    public class AttendanceHub : Hub
    {
        public async Task SendAttendanceUpdate()
        {
            await Clients.All.SendAsync("ReceiveAttendanceUpdate");
        }

        // KeepAlive — client থেকে ping
        public Task KeepAlive() => Task.CompletedTask;
    }
}