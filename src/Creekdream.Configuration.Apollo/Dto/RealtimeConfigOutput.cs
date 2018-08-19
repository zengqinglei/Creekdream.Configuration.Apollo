using System.Collections.Generic;

namespace Creekdream.Configuration.Apollo.Dto
{
    /// <summary>
    /// 即时配置输出信息
    /// </summary>
    public class RealtimeConfigOutput
    {
        /// <summary>
        /// 应用Id
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// 集群节点
        /// </summary>
        public string Cluster { get; set; }

        /// <summary>
        /// 命名空间
        /// </summary>
        public string NamespaceName { get; set; }

        /// <summary>
        /// 键值信息
        /// </summary>
        public Dictionary<string, string> Configurations { get; set; }

        /// <summary>
        /// 发布Key
        /// </summary>
        public string ReleaseKey { get; set; }
    }
}
