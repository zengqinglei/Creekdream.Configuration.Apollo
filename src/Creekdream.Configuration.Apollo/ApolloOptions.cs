using System.Collections.Generic;

namespace Creekdream.Configuration.Apollo
{
    /// <summary>
    /// apollo 配置
    /// </summary>
    public class ApolloOptions
    {
        /// <summary>
        /// 应用Id
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// 服务发现地址
        /// </summary>
        public string MetaServer { get; set; }

        /// <summary>
        /// 节点(默认：default)
        /// </summary>
        public string Cluster { get; set; } = "default";

        /// <summary>
        /// 本地Ip(默认自动获取)
        /// </summary>
        public string LocalIp { get; set; }

        /// <summary>
        /// 命名空间
        /// </summary>
        public List<string> Namespaces { get; set; } = new List<string>();
    }
}
