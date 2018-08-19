using Microsoft.Extensions.Configuration;
using Shouldly;
using System.Threading;
using Xunit;

namespace Creekdream.Configuration.Apollo.Tests
{
    /// <summary>
    /// Apollo 配置测试
    /// </summary>
    public class ApolloConfigurationTest
    {
        private readonly IConfiguration _configuration;

        public ApolloConfigurationTest()
        {
            var builder = new ConfigurationBuilder();
            builder.AddApollo(
                optionsConfig =>
                {
                    optionsConfig.AppId = "FabricDemo.UserService";
                    optionsConfig.MetaServer = "http://pro.meta-server.zengql.local";
                    optionsConfig.Namespaces.Add("application");
                    optionsConfig.Namespaces.Add("TEST1.ConsulPublic");
                });
            _configuration = builder.Build();
        }

        [Fact]
        public void Test_Get_ConsulAddress()
        {
            var address = _configuration.GetValue<string>("ConsulClient:ClientAddress");
            address.ShouldNotBeNull();
        }

        [Fact]
        public void Test_Get_ServiceName()
        {
            var serviceNmae = _configuration.GetValue<string>("ConsulService:ServiceName");
            serviceNmae.ShouldNotBeNull();
        }

        [Fact]
        public void Test_LongPolling_Get_TestKey()
        {
            // TODO: 在Apollo后台添加或修改键值：TestKey
            var maxTimes = 100;
            for (int i = 0; i < maxTimes; i++)
            {
                var testValue = _configuration.GetValue<string>("TestKey");
                if (testValue != null || i == maxTimes - 1)
                {
                    testValue.ShouldNotBeNull();
                    break;
                }
                Thread.Sleep(1000);
            }
        }
    }
}
