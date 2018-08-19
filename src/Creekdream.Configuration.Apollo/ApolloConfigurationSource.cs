using Microsoft.Extensions.Configuration;

namespace Creekdream.Configuration.Apollo
{
    /// <summary>
    /// 携程Apollo配置数据
    /// </summary>
    public class ApolloConfigurationSource : IConfigurationSource
    {
        private readonly ApolloOptions _apolloOptions;

        /// <inheritdoc />
        public ApolloConfigurationSource(ApolloOptions apolloOptions)
        {
            _apolloOptions = apolloOptions;
        }

        /// <inheritdoc />
        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new ApolloConfigurationProvider(_apolloOptions);
        }
    }
}
