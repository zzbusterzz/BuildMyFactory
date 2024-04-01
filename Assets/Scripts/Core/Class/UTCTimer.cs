using System;
using System.Globalization;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;

namespace Test.FactoryRun.Core
{
    public static class UTCTimer
    {
        public static DateTime UtcNow => _utcStartTime.AddSeconds(Time.realtimeSinceStartup);

        private static DateTime _utcStartTime;

        public static void InitializeTime()
        {
            GetUtcTimeAsync().WrapErrors();
        }

        private static async Task GetUtcTimeAsync()
        {
            try
            {
                var client = new TcpClient();
                await client.ConnectAsync("time.nist.gov", 13);
                using var streamReader = new StreamReader(client.GetStream());
                var response = await streamReader.ReadToEndAsync();
                var utcDateTimeString = response.Substring(7, 17);
                _utcStartTime = DateTime.ParseExact(utcDateTimeString, "yy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            }
            catch(Exception ex)
            {
                Debug.LogError(ex);
            }
        }

        private static async void WrapErrors(this Task task)
        {
            await task;
        }
    }
}