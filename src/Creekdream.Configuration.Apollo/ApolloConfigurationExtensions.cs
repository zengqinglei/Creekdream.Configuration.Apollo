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
            => builder.AddApollo(apolloConfiguration.Get<ApolloOptions>());

        /// <summary>
        /// 构建Apollo配置
        /// </summary>
        public static IConfigurationBuilder AddApollo(this IConfigurationBuilder builder, Action<ApolloOptions> optionsConfig)
        {
            var options = new ApolloOptions();
            optionsConfig.Invoke(options);

            return builder.Add(new ApolloConfigurationSource(options));
        }

        /// <inheritdoc />
        private static IConfigurationBuilder AddApollo(this IConfigurationBuilder builder, ApolloOptions  apolloOptions)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            if (apolloOptions == null)
            {
                throw new ArgumentException(nameof(apolloOptions));
            }

            return builder.Add(new ApolloConfigurationSource(apolloOptions));
        }
    }
}
