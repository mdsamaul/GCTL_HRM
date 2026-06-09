using GCTL.UI.Core.Hubs;
using Microsoft.AspNetCore.SignalR;
using System.Data.SqlClient;

namespace GCTL.UI.Core.BackgroundServices
{
    public class AttendanceSqlWatcher : BackgroundService
    {
        private readonly string _conn;
        private readonly IHubContext<AttendanceHub> _hub;
        private readonly ILogger<AttendanceSqlWatcher> _logger;

        public AttendanceSqlWatcher(
            IConfiguration config,
            IHubContext<AttendanceHub> hub,
            ILogger<AttendanceSqlWatcher> logger)
        {
            _conn = config.GetConnectionString("ApplicationDbConnection");
            _hub = hub;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogWarning("AttendanceSqlWatcher starting...");

            SqlDependency.Start(_conn);

            try
            {
                // দুটো table আলাদা আলাদা watch করো
                var t1 = WatchTableAsync(
                    "SELECT [autoId] FROM [dbo].[HRM_ATD_MachineData]",
                    "MachineData", stoppingToken);

                var t2 = WatchTableAsync(
                    "SELECT [autoId] FROM [dbo].[HRM_ATD_Manual]",
                    "Manual", stoppingToken);

                await Task.WhenAll(t1, t2);
            }
            catch (TaskCanceledException) { }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AttendanceSqlWatcher fatal error.");
            }
            finally
            {
                SqlDependency.Stop(_conn);
                _logger.LogWarning("AttendanceSqlWatcher stopped.");
            }
        }

        private async Task WatchTableAsync(
            string query, string tableName, CancellationToken ct)
        {
            _logger.LogWarning("Watching table: {Table}", tableName);

            while (!ct.IsCancellationRequested)
            {
                try
                {
                    await ListenOnceAsync(query, tableName, ct);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex,
                        "SqlDependency error on {Table}, retry in 5s...", tableName);
                    await Task.Delay(5000, ct);
                }
            }
        }

        private Task ListenOnceAsync(
            string query, string tableName, CancellationToken ct)
        {
            var tcs = new TaskCompletionSource<bool>(
                TaskCreationOptions.RunContinuationsAsynchronously);

            ct.Register(() => tcs.TrySetCanceled());

            using var conn = new SqlConnection(_conn);
            conn.Open();

            using var cmd = new SqlCommand(query, conn);
            var dep = new SqlDependency(cmd);

            dep.OnChange += async (sender, e) =>
            {
                _logger.LogWarning(
                    "Change detected on {Table} — Type:{Type} Info:{Info}",
                    tableName, e.Type, e.Info);

                // Subscribe error হলে retry — notify করো না
                if (e.Type == SqlNotificationType.Subscribe)
                {
                    _logger.LogWarning(
                        "Subscribe error on {Table}: {Info}", tableName, e.Info);
                    tcs.TrySetResult(true);
                    return;
                }

                // Change হলে সব client notify করো
                if (e.Type == SqlNotificationType.Change)
                {
                    try
                    {
                        await _hub.Clients.All.SendAsync(
                            "ReceiveAttendanceUpdate",
                            cancellationToken: ct);

                        _logger.LogWarning(
                            "Hub notified for {Table}", tableName);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Hub notify failed.");
                    }
                }

                tcs.TrySetResult(true);
            };

            // Execute করতে হবে — না হলে dependency register হয় না
            using var rdr = cmd.ExecuteReader();
            rdr.Close();

            return tcs.Task;
        }
    }
}