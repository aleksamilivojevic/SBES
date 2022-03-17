using Contracts;
using ManagerAplikacije;
using Service_Client;
using System;
using System.Collections.Generic;
using System.IdentityModel.Policy;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Security;
using System.Text;
using System.Threading.Tasks;

namespace WCFService
{
    class Program
    {
        static void Main(string[] args)
        {
            WCFBazaPodataka db = WCFBazaPodataka.InicijalizacijaBP();

            //uzmemo username od servera kako bismo uzeli certificate uz pomoc toga
            String serviceCertificateCN = ManagerAplikacije.Formatter.ParseName(WindowsIdentity.GetCurrent().Name);
            //String serverCertificateCN = "wcfclient";
            //string srvName = WindowsIdentity.GetCurrent().Name.Split('\\')[1];

            X509Certificate2 serverCertificate = Sertifikati.GetCertificateFromStorage(StoreName.TrustedPeople, StoreLocation.LocalMachine, serviceCertificateCN/*srvName*/);


            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;

            string address = "net.tcp://localhost:7776/Service";

            ServiceHost serviceHost = new ServiceHost(typeof(WCFService.Service));
            serviceHost.AddServiceEndpoint(typeof(IWCFService), binding, address);

            //kazemo da ne gleda da li je povucen sertifikat i setujemo .cer fajl od servera
            serviceHost.Credentials.ClientCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;
            serviceHost.Credentials.ClientCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.ChainTrust;
            serviceHost.Credentials.ServiceCertificate.Certificate = Sertifikati.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, serviceCertificateCN/*srvName*/);
            //serviceHost.Credentials.ServiceCertificate.Certificate = Sertifikati.GetCertificateFromFile("WCFService.pfx");

            serviceHost.Description.Behaviors.Remove(typeof(ServiceDebugBehavior));
            serviceHost.Description.Behaviors.Add(new ServiceDebugBehavior() { IncludeExceptionDetailInFaults = true });


            serviceHost.Authorization.ServiceAuthorizationManager = new AutorizacijaManager();
            binding.Security.Mode = SecurityMode.Transport;//za poruke
            binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;//potpisi ko je poslao i sifruj
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;//radice se preko windows auth*/

            //polisa sadrzi uslove koje omogucavaju evaluaciju korisnika(da li ima pravo pristupa nekoj metodi)
            //na osnovu polise radimo proveru
            serviceHost.Authorization.PrincipalPermissionMode = PrincipalPermissionMode.UseAspNetRoles;
            List<IAuthorizationPolicy> policies = new List<IAuthorizationPolicy>();
            policies.Add(new CustomPolicy());
            serviceHost.Authorization.ExternalAuthorizationPolicies = policies.AsReadOnly();

            serviceHost.Description.Behaviors.Remove<ServiceSecurityAuditBehavior>();
            ///////////////////////// LOGGER /////////////////////////
            NetTcpBinding bindingLogger = new NetTcpBinding();
            bindingLogger.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            string addressLogger = "net.tcp://localhost:7768/WCFLogger";
            EndpointAddress endpointAddress = new EndpointAddress(new Uri(addressLogger));
            //////////////////////////////////////////////////////////*

        
            ServiceClientLoggerKomunikacija.InitializeService(bindingLogger, endpointAddress);

            serviceHost.Open();
            Console.WriteLine("WCFService is opened. Press <enter> to finish and save databases...");
            Console.ReadLine();

            serviceHost.Close();

            db.SerializeData();
        }
    }
}
