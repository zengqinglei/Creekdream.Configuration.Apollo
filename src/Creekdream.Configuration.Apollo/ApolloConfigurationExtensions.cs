using Microsoft.Extensions.Configuration;
using System;

namespace Creekdream.Configuration.Apollo
{
    /// <summary>
    /// 新增Apollo配置拓展
    /// </summary>
    public static class ApolloConfigurationExtensions
    {
        /// <summary>
        /// 构建Apollo配置
        /// </summary>
        public static IConfigurationBuilder AddApollo(this IConfigurationBuilder builder, IConfigurationSection apolloConfiguration)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            if (apolloConfiguration == null)
            {
                throw new ArgumentException(nameof(apolloConfiguration));
            }
            var options = apolloConfiguration.Get<ApolloOptions>();

            return builder.Add(new ApolloConfigurationSource(options));
        }

        /// <summary>
        /// 构建Apollo配置
        /// </summary>
        public static IConfigurationBuilder AddApollo(this IConfigurationBuilder builder, Action<ApolloOptions> optionsConfig)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            if (optionsConfig == null)
            {
                throw new ArgumentException(nameof(optionsConfig));
            }

            var options = new ApolloOptions();
            optionsConfig.Invoke(options);

            return builder.Add(new ApolloConfigurationSource(options));
        }
    }
}
