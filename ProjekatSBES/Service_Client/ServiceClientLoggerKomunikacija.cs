using Contracts;
using Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Service_Client
{
    public class ServiceClientLoggerKomunikacija : ChannelFactory<ILogger>
    {
        static private ILogger factory;
        
        private ServiceClientLoggerKomunikacija(NetTcpBinding binding, EndpointAddress address) 
            :base(binding, address)
        {
            factory = this.CreateChannel();
        }

        public static ILogger Factory { get => factory; }

        public static ILogger InitializeService(NetTcpBinding binding, EndpointAddress address)
        {
            if(factory == null)
            {
                var servis = new ServiceClientLoggerKomunikacija(binding, address);
            }

            return factory;
        }
    }
}
