using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using ManagerAplikacije;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            String serverCertificateCN = "wcfservice";
            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;
            X509Certificate2 serverCertificate = Sertifikati.GetCertificateFromStorage(StoreName.TrustedPeople, StoreLocation.LocalMachine, serverCertificateCN);
            string address = "net.tcp://localhost:7776/Service";
            //kreiramo endpoint i setujemo mu sertifikat
            EndpointAddress endpointAddress = new EndpointAddress(new Uri(address),new X509CertificateEndpointIdentity(serverCertificate));
            
            using (WCFClient proxy = new WCFClient(binding, endpointAddress))
            {
                string option;

                do
                {
                    Meni();
                    option = Console.ReadLine();

                    if (Int32.TryParse(option, out int opt) && option != "9")
                        IzaberiOpciju(proxy, option);

                } while (option != "9");
            }

            Console.Write("\nPritisni nesto za kraj.");
            Console.ReadLine();
        }

        private static void IzaberiOpciju(WCFClient proxy, string opcija)
        {
            //WCFClient klijent;

            string imeBaze = String.Empty;
            string povratnaVrednost = String.Empty;
            string drzava = String.Empty;
            string grad = String.Empty;
            string maksimalnaPotrosnja = String.Empty;
            string poruka = String.Empty;
            string temp = String.Empty;
            byte[] promena = null;

            
            Console.Write("\nUnesite ime baze podataka: ");
            imeBaze = Console.ReadLine();
           

            switch(opcija)
            {
                case "1":
                    povratnaVrednost = proxy.KreiranjeBaze(imeBaze);
                    Console.WriteLine(Environment.NewLine + povratnaVrednost);
                    break;
                case "2":
                    povratnaVrednost = proxy.BrisanjeBaze(imeBaze);
                    Console.WriteLine(Environment.NewLine + povratnaVrednost);
                    break;
                case "3":
                    poruka = CreateMessage(imeBaze, "Unesi");

                    povratnaVrednost = proxy.UpisUBazu(poruka, promena);
                    Console.WriteLine(Environment.NewLine + povratnaVrednost);
                    break;
                case "4":
                    poruka = CreateMessage(imeBaze, "Modifikacija");

                    povratnaVrednost = proxy.ModifikacijaBaze(poruka, promena);
                    Console.WriteLine(Environment.NewLine + povratnaVrednost);
                    break;
                case "5":
                    povratnaVrednost = proxy.CitanjeBaze(imeBaze);
                    Console.WriteLine(Environment.NewLine + povratnaVrednost);
                    break;
                case "6":
                    //Console.WriteLine(Environment.NewLine + "Unesite ime regije");
                    //drzava = Console.ReadLine();
                    povratnaVrednost = proxy.NajveciPotrosacZaRegion(imeBaze);//unesi ime drzave
                    Console.WriteLine(Environment.NewLine + povratnaVrednost);
                    break;
                case "7":
                    Console.WriteLine(Environment.NewLine + "Unesite ime grada: ");
                    grad = Console.ReadLine();
                    povratnaVrednost = proxy.SrednjaVrednostZaOdredjeniGrad(imeBaze, grad);
                    Console.WriteLine(Environment.NewLine + povratnaVrednost);
                    break;
                case "8":
                    Console.WriteLine(Environment.NewLine + "Unesite ime regiona: ");
                    drzava = Console.ReadLine();
                    povratnaVrednost = proxy.SrednjaVrednostZaOdredjeniRegion(imeBaze, drzava);
                    Console.WriteLine(Environment.NewLine + povratnaVrednost);
                    break;
                case "9":
                    Console.WriteLine(Environment.NewLine + "Otvori mi svoja vrata, da izadjem");
                    break;
                default:
                    Console.WriteLine(Environment.NewLine + "Nepoznata komanda");
                    break;

                
            }
        }

        private static void Meni()
        {
            Console.WriteLine("Meni:");
            Console.WriteLine("\t1. Kreiraj bazu podataka");
            Console.WriteLine("\t2. Obrisi bazu podataka");
            Console.WriteLine("\t3. Dodaj nov entitet u bazu podataka");
            Console.WriteLine("\t4. Izmeni postojeci entitet u bazu podataka");
            Console.WriteLine("\t5. Izlistaj sve entitete iz baze podataka");
            Console.WriteLine("\t6. Prikazi maksimalnu potrosnju za region iz baze podataka");
            Console.WriteLine("\t7. Prikazi srednju vrednost za odredjeni grad iz baze podataka");
            Console.WriteLine("\t8. Prikazi srednju vrednost za odredjeni region iz baze podataka");
            //Console.WriteLine("\t9. Prikazi ime baze podataka");
            Console.WriteLine("\t9. Izlaz");
            Console.Write("\t>> ");
        }


        private static string CreateMessage(string databaseName, string operation)
        {
            string temp;
            //string payday;
            double salary;
            int id = -1;

            //short age;

            if (operation == "Edit")
            {
                do
                {
                    Console.Write("ID: ");
                } while (!Int32.TryParse(Console.ReadLine(), out id));
            }

            Console.Write("Country: ");
            string country = Console.ReadLine();

            Console.Write("City: ");
            string city = Console.ReadLine();


            do
            {
                Console.Write("Salary: ");
                temp = Console.ReadLine();
            } while (!Double.TryParse(temp, out salary));


            string message = $"{databaseName}:{country}:{city}:{salary}:{id}";

            return message;
        }
    }
}
