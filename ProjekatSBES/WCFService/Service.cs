using Contracts;
using Logger;
using ManagerAplikacije;
using Service_Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WCFService
{
    public class Service : ChannelFactory<ILogger>, IWCFService
    {
        private WCFBazaPodataka bp = WCFBazaPodataka.InicijalizacijaBP();
        //IWCFService factory;

        public Service()
        {
        }

        #region Admin 's operations
        public string KreiranjeBaze(string bazaPodataka)
        {
            string imeKlijenta = ((Thread.CurrentPrincipal as CustomPrincipal).Identity).Name.Split(',', ';')[0].Split('=')[1];

            if (Thread.CurrentPrincipal.IsInRole("CreateDB"))
            {
                ServiceClientLoggerKomunikacija.Factory.UspesnaAutorizacija(imeKlijenta);

                string informacije = bp.KreiranjeBaze(bazaPodataka);

                ServiceClientLoggerKomunikacija.Factory.KorisnickeOperacije(imeKlijenta, "Kreirana baza podataka", informacije);

                return informacije;
            }
            else
            {
                ServiceClientLoggerKomunikacija.Factory.NeuspesnaAutorizacija(imeKlijenta, "Nema prava da kreira bazu podataka.");

                throw new FaultException<SecurityAccessDeniedException>(new SecurityAccessDeniedException(), new FaultReason("Nema prava.\n"));
            }
        }

        public string BrisanjeBaze(string bazaPodataka)
        {
            string imeKlijenta = ((Thread.CurrentPrincipal as CustomPrincipal).Identity).Name.Split(',', ';')[0].Split('=')[1];

            if(Thread.CurrentPrincipal.IsInRole("DeleteDB"))
            {
                ServiceClientLoggerKomunikacija.Factory.UspesnaAutorizacija(imeKlijenta);

                string informacije = bp.BrisanjeBaze(bazaPodataka);

                ServiceClientLoggerKomunikacija.Factory.KorisnickeOperacije(imeKlijenta, "Baza podataka obrisana", informacije);

                return informacije;
            }
            else
            {
                ServiceClientLoggerKomunikacija.Factory.NeuspesnaAutorizacija(imeKlijenta, "Nema prava da izbrise bazu podataka.");

                throw new FaultException<SecurityAccessDeniedException>(new SecurityAccessDeniedException(), new FaultReason("Nema prava.\n"));
            }
        }

        public string ArhiviranjeBaze(string bazaPodataka)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Writer's operations
        public string UpisUBazu(string poruka, byte[] promena)
        {
            string imeKlijenta = ((Thread.CurrentPrincipal as CustomPrincipal).Identity).Name.Split(',', ';')[0].Split('=')[1];

            if (Thread.CurrentPrincipal.IsInRole("Write"))
            {
                ServiceClientLoggerKomunikacija.Factory.UspesnaAutorizacija(imeKlijenta);

                string informacije = bp.UpisUBazu(poruka, promena);
                
                ServiceClientLoggerKomunikacija.Factory.KorisnickeOperacije(imeKlijenta, "Uspesan upis u bazu", informacije);

                return informacije;
            }
            else
            {
                ServiceClientLoggerKomunikacija.Factory.NeuspesnaAutorizacija(imeKlijenta, "Nema prava da upisuje bazu podataka.");

                throw new FaultException<SecurityAccessDeniedException>(new SecurityAccessDeniedException(), new FaultReason("Nema prava.\n"));
            }
        }

        public string ModifikacijaBaze(string poruka, byte[] promena)
        {
            string imeKlijenta = ((Thread.CurrentPrincipal as CustomPrincipal).Identity).Name.Split(',', ';')[0].Split('=')[1];

            if (Thread.CurrentPrincipal.IsInRole("Edit"))
            {
                ServiceClientLoggerKomunikacija.Factory.UspesnaAutorizacija(imeKlijenta);

                string informacije = bp.ModifikacijaBaze(poruka, promena);

                ServiceClientLoggerKomunikacija.Factory.KorisnickeOperacije(imeKlijenta, "Uspene izmene u bazi", informacije);

                return informacije;
            }
            else
            {
                ServiceClientLoggerKomunikacija.Factory.NeuspesnaAutorizacija(imeKlijenta, "Nema prava da menja bazu podataka.");

                throw new FaultException<SecurityAccessDeniedException>(new SecurityAccessDeniedException(), new FaultReason("Nema prava.\n"));
            }
        }

        #endregion

        #region Reader's operations

        public string CitanjeBaze(string bazaPodataka)
        {
            string imeKlijenta = ((Thread.CurrentPrincipal as CustomPrincipal).Identity).Name.Split(',', ';')[0].Split('=')[1];
            ServiceClientLoggerKomunikacija.Factory.UspesnaAutorizacija(imeKlijenta);

            string informacija = $"Daj mi sve";
            ServiceClientLoggerKomunikacija.Factory.KorisnickeOperacije(imeKlijenta, $"Izlistani svi entiteti iz baze podataka '{bazaPodataka}'", informacija);

            return bp.CitanjeBaze(bazaPodataka);
        }

        public string NajveciPotrosacZaRegion(string bazaPodataka)
        {
            string imeKlijenta = ((Thread.CurrentPrincipal as CustomPrincipal).Identity).Name.Split(',', ';')[0].Split('=')[1];
            ServiceClientLoggerKomunikacija.Factory.UspesnaAutorizacija(imeKlijenta);

            string informacija = $"Daj mi sve";
            ServiceClientLoggerKomunikacija.Factory.KorisnickeOperacije(imeKlijenta, $"Najveci potrosac za region iz baze podataka'{bazaPodataka}'", informacija);

            return bp.NajveciPotrosacZaRegion(bazaPodataka);
        }

        public string SrednjaVrednostZaOdredjeniGrad(string bazaPodataka, string grad)
        {
            string imeKlijenta = ((Thread.CurrentPrincipal as CustomPrincipal).Identity).Name.Split(',', ';')[0].Split('=')[1];
            ServiceClientLoggerKomunikacija.Factory.UspesnaAutorizacija(imeKlijenta);

            string informacija = $"Daj mi sve";
            ServiceClientLoggerKomunikacija.Factory.KorisnickeOperacije(imeKlijenta, $"Srednja potrosnja za grad iz baze podataka '{bazaPodataka}'", informacija);

            return bp.SrednjaVrednostZaOdredjeniGrad(bazaPodataka, grad);
        }

        public string SrednjaVrednostZaOdredjeniRegion(string bazaPodataka, string drzava)
        {
            string imeKlijenta = ((Thread.CurrentPrincipal as CustomPrincipal).Identity).Name.Split(',', ';')[0].Split('=')[1];
            ServiceClientLoggerKomunikacija.Factory.UspesnaAutorizacija(imeKlijenta);

            string informacija = $"Daj mi sve";
            ServiceClientLoggerKomunikacija.Factory.KorisnickeOperacije(imeKlijenta, $"Srednja potrosnja za region u bazi podataka '{bazaPodataka}'", informacija);

            return bp.SrednjaVrednostZaOdredjeniRegion(bazaPodataka, drzava);
        }

        #endregion
    }
}
