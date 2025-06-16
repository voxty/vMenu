using CitizenFX.Core;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static vMenuShared.ConfigManager;

namespace vMenuServer
{
    public class LogManager : BaseScript
    {
        private static readonly string DISCORD_WEBHOOK = GetSettingsString(Setting.vmenu_discord_webhook);
        private static readonly string DISCORD_NAME = GetSettingsString(Setting.vmenu_discord_username);
        private static readonly string DISCORD_IMAGE = GetSettingsString(Setting.vmenu_discord_logo);

        public LogManager()
        {
            EventHandlers["vMenu:discordLogs"] += new Action<string, string, int>(SendLog);
        }

        private async void SendLog(string name, string message, int color)
        {
            if (string.IsNullOrEmpty(DISCORD_WEBHOOK))
            {
                Debug.WriteLine("Discord webhook URL is not configured. Skipping log send.");
                return;
            }

            var embed = new
            {
                color = color,
                title = $"**{name}**",
                description = message,
                thumbnail = new
                {
                    url = DISCORD_IMAGE
                },
                /*
                footer = new
                {
                    text = "© Peachtree Project",
                    icon_url = LogManager.DISCORD_IMAGE
                },
                timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")
                */
            };


            var payload = new
            {
                username = DISCORD_NAME,
                embeds = new[] { embed },
                avatar_url = DISCORD_IMAGE
            };

            var originalCallback = ServicePointManager.ServerCertificateValidationCallback;

            try
            {
                ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;

                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(10);
                    client.DefaultRequestHeaders.Add("User-Agent", "FiveM-DiscordLogger");

                    var json = JsonConvert.SerializeObject(payload);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync(DISCORD_WEBHOOK, content);

                    if (!response.IsSuccessStatusCode)
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        Debug.WriteLine($"Discord log failed: HTTP {(int)response.StatusCode} {response.ReasonPhrase}, Response: {errorContent}");
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine($"HttpRequestException sending Discord log: {ex.Message}, InnerException: {ex.InnerException?.Message}");
            }
            catch (TaskCanceledException)
            {
                Debug.WriteLine("Discord log request timed out.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unexpected error sending Discord log: {ex.Message}, StackTrace: {ex.StackTrace}");
            }
            finally
            {
                ServicePointManager.ServerCertificateValidationCallback = originalCallback;
            }
        }
    }
}