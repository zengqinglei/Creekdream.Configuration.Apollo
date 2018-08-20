using Creekdream.Configuration.Apollo.Dto;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Creekdream.Configuration.Apollo
{
    /// <summary>
    /// 携程Apollo配置工厂
    /// </summary>
    public class ApolloConfigurationProvider : ConfigurationProvider
    {
        private readonly ConcurrentDictionary<string, List<string>> _namespaceDataKeys;
        private readonly IList<NamespaceNotification> _namespaceNotifications;
        private readonly ApolloOptions _apolloOptions;
        private readonly HttpClient _httpClient;

        /// <inheritdoc />
        public ApolloConfigurationProvider(ApolloOptions apolloOptions)
        {
            _apolloOptions = apolloOptions;
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(_apolloOptions.MetaServer)
            };
            _namespaceDataKeys = new ConcurrentDictionary<string, List<string>>();
            _namespaceNotifications = new List<NamespaceNotification>();
        }

        /// <inheritdoc />
        public override void Load()
        {
            _apolloOptions.Namespaces.ForEach(
                namespaceName =>
                {
                    _namespaceNotifications.Add(
                        new NamespaceNotification()
                        {
                            NamespaceName = namespaceName,
                            NotificationId = -1
                        });
                });

            UpdateNotifications();

            // 长轮询刷新配置
            Task.Factory.StartNew(() =>
            {
                // 首次启动等待10s
                Thread.Sleep(10000);

                // 长轮询获取是否有更新
                while (true)
                {
                    if (UpdateNotifications())
                    {
                        OnReload();
                    }
                }
            }, TaskCreationOptions.LongRunning);
        }

        /// <summary>
        /// 更新通知信息并更新配置信息
        /// </summary>
        /// <returns>返回是否有更新</returns>
        private bool UpdateNotifications()
        {
            var notificationsInput = JsonConvert.SerializeObject(
                    _namespaceNotifications,
                    new JsonSerializerSettings()
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    });
            var url = $"notifications/v2" +
                $"?appId={_apolloOptions.AppId}" +
                $"&cluster={_apolloOptions.Cluster}" +
                $"&notifications={WebUtility.UrlEncode(notificationsInput)}";
            var response = _httpClient.GetAsync(url).GetAwaiter().GetResult();
            if (response.IsSuccessStatusCode)
            {
                var notificationsResultString = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                var notificationsOutput = JsonConvert.DeserializeObject<List<NamespaceNotification>>(notificationsResultString);
                foreach (var notificationOutput in notificationsOutput)
                {
                    var namespaceNotification = _namespaceNotifications
                        .SingleOrDefault(m => m.NamespaceName == notificationOutput.NamespaceName);
                    if (namespaceNotification == null)
                    {
                        _namespaceNotifications.Add(notificationOutput);
                    }
                    else
                    {
                        namespaceNotification.NotificationId = notificationOutput.NotificationId;
                    }
                    GetRealtimeConfig(notificationOutput.NamespaceName);
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// 即时获取配置信息
        /// </summary>
        private void GetRealtimeConfig(string namespaceName)
        {
            var url = $"configs/{_apolloOptions.AppId}/{_apolloOptions.Cluster}/{namespaceName}" +
                $"?ip={_apolloOptions.LocalIp ?? GetLocalIPAddress()}";
            var dataResultString = _httpClient.GetStringAsync(url).GetAwaiter().GetResult();
            var realtimeConfigOutput = JsonConvert.DeserializeObject<RealtimeConfigOutput>(dataResultString);

            if (_namespaceDataKeys.TryGetValue(namespaceName, out List<string> dataKeys))
            {
                dataKeys.ForEach(dataKey => Data.Remove(dataKey));
            }
            _namespaceDataKeys[namespaceName] = new List<string>();
            foreach (var keyValue in realtimeConfigOutput.Configurations)
            {
                Data[keyValue.Key] = keyValue.Value;
                _namespaceDataKeys[namespaceName].Add(keyValue.Key);
            }
        }

        /// <summary>
        /// 获取本地IP地址
        /// </summary>
        private static string GetLocalIPAddress()
        {
            var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (var network in networkInterfaces)
            {
                if (network.OperationalStatus != OperationalStatus.Up)
                    continue;
                var properties = network.GetIPProperties();
                if (properties.GatewayAddresses.Count == 0)
                    continue;

                foreach (var address in properties.UnicastAddresses)
                {
                    if (address.Address.AddressFamily != AddressFamily.InterNetwork)
                        continue;
                    if (IPAddress.IsLoopback(address.Address))
                        continue;
                    return address.Address.ToString();
                }
            }
            return "127.0.0.1";
        }
    }
}
