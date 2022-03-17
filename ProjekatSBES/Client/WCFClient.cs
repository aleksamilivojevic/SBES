using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.Text;
using System.Threading.Tasks;
//using Contracts;
using ManagerAplikacije;
using System.Security.Principal;
using System.Security.Cryptography.X509Certificates;

namespace Client
{
    public class WCFClient : ChannelFactory<IWCFService>, IWCFService, IDisposable
    {
        IWCFService factory;

        public WCFClient(NetTcpBinding binding, EndpointAddress address)
            : base(binding, address)
        {
            //uzimam windows naziv korisnika koji je pokrenuo ovo i dobijem nesto tipa xxx\userAdmin i formater mi vrati userAdmin
            String clientCertificateCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);
            //string srvName = WindowsIdentity.GetCurrent().Name.Split('\\')[1];

            //podesavamo u channel factory da mod validacije bude chaintrust i da se revokacija ne proverava (da li je sertifikat povucen -> NoCheck)
            this.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.ChainTrust;
            this.Credentials.ServiceCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;

            //postavljam i svoj sertifikat (client)
            this.Credentials.ClientCertificate.Certificate = Sertifikati.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, clientCertificateCN/*srvName*/);
            
            binding.Security.Mode = SecurityMode.Transport;
            binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;

            factory = this.CreateChannel();
        }

        #region Admin's operations
        public string KreiranjeBaze(string bazaPodataka)
        {
            try
            {
                return factory.KreiranjeBaze(bazaPodataka);
            }
            catch (SecurityAccessDeniedException e)
            {
                return $"Greska prilikom kreiranja baze podataka: {e.Message}";
            }
            catch (FaultException e)
            {
                return $"Greska prilikom kreiranja baze podataka: {e.Message}";
            }
        }

        public string BrisanjeBaze(string bazaPodataka)
        {
            try
            {
                return factory.BrisanjeBaze(bazaPodataka);
            }
            catch (SecurityAccessDeniedException e)
            {
                return $"Greska prilikom brisanja baze podataka: {e.Message}";
            }
            catch (FaultException e)
            {
                return $"Greska prilikom brisanja baze podataka: {e.Message}";
            }
        }


        public string ArhiviranjeBaze(string bazaPodataka)
        {
            try
            {
                return factory.ArhiviranjeBaze(bazaPodataka);
            }
            catch (SecurityAccessDeniedException e)
            {
                return $"Greska prilikom arhiviranja baze podataka: {e.Message}";
            }
            catch (FaultException e)
            {
                return $"Greska prilikom arhiviranja baze podataka: {e.Message}";
            }
        }

        #endregion

        #region Writer's operations
        public string UpisUBazu(string poruka, byte[] promena)
        {
            try
            {
                return factory.UpisUBazu(poruka, promena);
            }
            catch (SecurityAccessDeniedException e)
            {
                return $"Greska prilikom upisa u bazu podataka: {e.Message}";
            }
            catch (FaultException e)
            {
                return $"Greska prilikom upisa u bazi podataka: {e.Message}";
            }
        }

        public string ModifikacijaBaze(string poruka, byte[] promena)
        {
            try
            {
                return factory.ModifikacijaBaze(poruka, promena);
            }
            catch (SecurityAccessDeniedException e)
            {
                return $"Greska prilikom modifikacija u bazi podataka: {e.Message}";
            }
            catch (FaultException e)
            {
                return $"Greska prilikom modifikacija u bazi podataka: {e.Message}";
            }
        }

        #endregion

        #region Reader's operations
        public string CitanjeBaze(string bazaPodataka)
        {
            try
            {
                return factory.CitanjeBaze(bazaPodataka);
            }
            catch (SecurityAccessDeniedException e)
            {
                return "Greska prilikom citanja baze podataka: " + e.Message + Environment.NewLine;
                //return new byte[0];
            }
            catch (FaultException e)
            {
                return "Greska prilikom citanja baze podataka: " + e.Message + Environment.NewLine;
                //return new byte[0];
            }
        }

        public string NajveciPotrosacZaRegion(string bazaPodataka)
        {
            try
            {
                return factory.NajveciPotrosacZaRegion(bazaPodataka);
            }
            catch (SecurityAccessDeniedException e)
            {
                return "Greska prilikom prikaza najveceg potrosaca za region " + e.Message + Environment.NewLine;
                //return new byte[0];
            }
            catch (FaultException e)
            {
                return "Greska prilikom prikaza najveceg potrosaca za region " + e.Message + Environment.NewLine;
                //return new byte[0];
            }
        }

        public string SrednjaVrednostZaOdredjeniGrad(string bazaPodataka, string grad)
        {
            try
            {
                return factory.SrednjaVrednostZaOdredjeniGrad(bazaPodataka, grad);
            }
            catch (SecurityAccessDeniedException e)
            {
                return "Greska prilikom racunanja srednje vrednosti za odredjeni grad " + e.Message + Environment.NewLine;
                //return new byte[0];
            }
            catch (FaultException e)
            {
                return "Greska prilikom racunanja srednje vrednosti za odredjeni grad " + e.Message + Environment.NewLine;
                //return new byte[0];
            }
        }

        public string SrednjaVrednostZaOdredjeniRegion(string bazaPodataka, string drzava)
        {
            try
            {
                return factory.SrednjaVrednostZaOdredjeniRegion(bazaPodataka, drzava);
            }
            catch (SecurityAccessDeniedException e)
            {
                return "Greska prilikom racunanja srednje vrednosti za odredjeni region " + e.Message + Environment.NewLine;
                //return new byte[0];
            }
            catch (FaultException e)
            {
                return "Greska prilikom racunanja srednje vrednosti za odredjeni region " + e.Message + Environment.NewLine;
                //return new byte[0];
            }
        }

        #endregion
        public void Dispose()
        {
            if (factory != null)
            {
                factory = null;
            }

            this.Close();
        }
    }
}
