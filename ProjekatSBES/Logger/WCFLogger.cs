using Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logger
{
    public class WCFLogger : ILogger, IDisposable
    {
        private static EventLog logger = null;
        const string ImeIzvora = "LoggerData";
        const string ImeLogera = "Aplication";

        public WCFLogger()
        {
            //Debugger.Launch();
            try
            {
                if(!EventLog.SourceExists(ImeIzvora))
                {
                    EventLog.CreateEventSource(ImeIzvora, ImeLogera);
                }

                logger = new EventLog(ImeLogera, Environment.MachineName, ImeIzvora);
            }
            catch(Exception e)
            {
                logger = null;
                Console.WriteLine("Greska prilikom kreiranja loggera. Greska: {0}", e.Message);
            }
        }

        public void KorisnickeOperacije(string korisnik, string operacija, string informacija)
        {
            if(logger != null)
            {
                string poruka = $"Korisnik '{korisnik}' je pozvao:\nOperaciju: {operacija}\nInformacije: {informacija}";
                logger.WriteEntry(poruka);
            }
            else
            {
                throw new ArgumentException($"Greska prilikom pokusaja upisa dogadjaja {operacija} u logger");
            }
        }

        public void NeuspesnaAutentifikacija(string korisnik)
        {
            if(logger != null)
            {
                string poruka = $"Korisnik '{korisnik}: Greska u autentifikaciji!" + Environment.NewLine;
                logger.WriteEntry(poruka);
            }
            else
            {
                throw new ArgumentException("Greska prilikom upisa dogadjaja(greksa u autentifikaciji) u logger");
            }
        }

        public void UspesnaAutentifikacija(string korisnik)
        {
            if (logger != null)
            {
                string poruka = $"Korisnik '{korisnik}: Autentifikacija uspesna!" + Environment.NewLine;
                logger.WriteEntry(poruka);
            }
            else
            {
                throw new ArgumentException("Greska prilikom upisa dogadjaja(greksa u autentifikaciji) u logger");
            }
        }

        public void NeuspesnaAutorizacija(string korisnik, string razlog)
        {
            if (logger != null)
            {
                string poruka = $"Korisnik '{korisnik}: Greska u autorizaciji" + Environment.NewLine + $"Razlog: {razlog}";
                logger.WriteEntry(poruka);
            }
            else
            {
                throw new ArgumentException("Greska prilikom upisa dogadjaja(greksa u autorizacija) u logger");
            }
        }

        public void UspesnaAutorizacija(string korisnik)
        {
            if (logger != null)
            {
                string poruka = $"Korisnik '{korisnik}: Autorizacija uspesna" + Environment.NewLine;
                logger.WriteEntry(poruka);
            }
            else
            {
                throw new ArgumentException("Greska prilikom upisa dogadjaja(greksa u autorizacija) u logger");
            }
        }

        public void Dispose()
        {
            if(logger != null)
            {
                logger.Dispose();
                logger = null;
            }
        }
    }
}
