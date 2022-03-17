using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;

namespace Logger
{
    class Program
    {
        static void Main(string[] args)
        {
            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;

            string address = "net.tcp://localhost:7768/WCFLogger";

            ServiceHost serviceHost = new ServiceHost(typeof(WCFLogger));
            serviceHost.AddServiceEndpoint(typeof(ILogger), binding, address);
            //Debugger.Launch();
            serviceHost.Description.Behaviors.Remove<ServiceSecurityAuditBehavior>();

            ServiceSecurityAuditBehavior newAudit = new ServiceSecurityAuditBehavior();
            newAudit.AuditLogLocation = AuditLogLocation.Application;
            newAudit.ServiceAuthorizationAuditLevel = AuditLevel.SuccessOrFailure;
            newAudit.SuppressAuditFailure = true;

            serviceHost.Description.Behaviors.Remove<ServiceSecurityAuditBehavior>();
            serviceHost.Description.Behaviors.Add(newAudit);

            WCFLogger wcfLogger = new WCFLogger();

            serviceHost.Open();
            Console.WriteLine("WCFLogger is opened. Press <enter> to exit...");
            Console.ReadLine();
            
            serviceHost.Close();
        }
    }
}
