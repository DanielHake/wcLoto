using Microsoft.Extensions.Options;
using System;
using StackExchange.Redis;


namespace helpers.Redis {
    public interface IRedisConnectionFactory
    {
        ConnectionMultiplexer Connection();
    }

 
    public class RedisConnectionFactory : IRedisConnectionFactory
    {
        /// <summary>
        ///     The _connection.
        /// </summary>
        private readonly Lazy<ConnectionMultiplexer> _connection;
        

        private readonly IOptions<ConfigurationOptions> redis;

        public RedisConnectionFactory(IOptions<ConfigurationOptions> redis)
        {
            this._connection = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect("sl-eu-lon-2-portal.8.dblayer.com:22605,password=ZWBVRIYVVZAMWJUG"));
        }

        public ConnectionMultiplexer Connection()
        {
            return this._connection.Value;
        }
    
    }
}
    
