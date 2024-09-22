using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using _Project.Scripts.Configs;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace _Project.Scripts.Services.TimeService
{
    public class TimeService : ITimeService
    {
        public bool IsTimePreloaded => _isTimePreloaded;
        private bool _isTimePreloaded = false;
        private DateTime _preloadedTime;
        private MainConfig _mainConfig;
        private static readonly HttpClient _httpClient = new HttpClient();

        public TimeService(MainConfig mainConfig)
        {
            _mainConfig = mainConfig;
        }

        public async UniTask<DateTime> GetOnlineTimeAsync()
        {
            using (CancellationTokenSource cts = new CancellationTokenSource(
                       TimeSpan.FromSeconds(_mainConfig.ServerWaitingForResponseTime)))
            {
                try
                {
                    HttpResponseMessage response = await _httpClient.GetAsync(_mainConfig.TimeServerUrl, cts.Token);
                    response.EnsureSuccessStatusCode();

                    string responseBody = await response.Content.ReadAsStringAsync();
                    JObject json = JObject.Parse(responseBody);
                    long timestamp = json[_mainConfig.TimeKeyJSON].Value<long>();

                    DateTime onlineTime = DateTimeOffset.FromUnixTimeMilliseconds(timestamp).DateTime;
                    return onlineTime;
                }
                catch (TaskCanceledException ex) when (ex.CancellationToken == cts.Token)
                {
                    Debug.LogError("Время ожидания ответа сервера истекло.");
                    return DateTime.Now;
                }
                catch (HttpRequestException ex)
                {
                    Debug.LogError($"Ошибка сети: {ex.Message}");
                    return DateTime.Now;
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Ошибка получения времени с сервера: {ex.Message}");
                    return DateTime.Now;
                }
            }
        }

        public async UniTask PreloadTimeAsync()
        {
            _preloadedTime = await GetOnlineTimeAsync();
            _isTimePreloaded = true;
        }

        public DateTime GetPreloadedTime()
        {
            return _preloadedTime;
        }
    }
}