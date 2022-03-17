using Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace WCFService
{
    public class WCFBazaPodataka : IWCFService
    {
        public Dictionary<String, Dictionary<int, Informacije>> BpList = new Dictionary<string, Dictionary<int, Informacije>>();
        public String ImeBazePodataka = "bazaPodataka.txt";
        private static WCFBazaPodataka instance = null;
      
        private WCFBazaPodataka()
        {
            DeserializeData();
        }

        public static WCFBazaPodataka InicijalizacijaBP()
        {
            if(instance == null)
            {
                instance = new WCFBazaPodataka();
            }
            return instance;
        }

        #region Serializacija
        public void SerializeData()
        {
            //podesavamo serializer za informacije (kako bismo sacuvali sve postojece nazive baza)
            XmlSerializer serializerDbNames = new XmlSerializer(typeof(List<String>));
            StringWriter swDbNames = new StringWriter();

            //pomocna promenljiva u koju cemo smestati sve informacije u tekucoj bazi
            List<Informacije> tempdataitems;
            //pomocna promenljiva koja cuva nazive svih baza kako bismo ih sacuvali
            List<String> databaseNames = new List<String>();

            //prolazimo kroz sve baze u sistemu
            foreach (KeyValuePair<String, Dictionary<int, Informacije>> kvp in BpList)
            {
                //podesavamo serializer za informacije (podatke unutar svake baze)
                XmlSerializer serializer = new XmlSerializer(typeof(List<Informacije>));
                StringWriter sw = new StringWriter();

                //dodajemo nazive baza u pomocnu listu da ih sacuvamo u fajlu
                databaseNames.Add(kvp.Key);

                //za svaku bazu, napunimo je njenim podacima i to snimimo u fajl nakon fora
                tempdataitems = new List<Informacije>();

                foreach (KeyValuePair<int, Informacije> kvp1 in kvp.Value)
                {
                    tempdataitems.Add(kvp1.Value);  //dodajemo u pomocnu listu kako bismo to upisali u fajl
                }
                serializer.Serialize(sw, tempdataitems);
                //upisujemo u svaku bazu informacije koje su vezane za njih
                File.Delete(kvp.Key);
                File.AppendAllText(kvp.Key, sw.ToString());

            }
            //pravimo fajl sa svim nazivima baza podataka
            serializerDbNames.Serialize(swDbNames, databaseNames);
            File.Delete(ImeBazePodataka);
            File.AppendAllText(ImeBazePodataka, swDbNames.ToString());

        }

        public void DeserializeData()
        {
            if (!File.Exists(ImeBazePodataka))
            {
                return;
            }

            String xml = File.ReadAllText(ImeBazePodataka);   //sve baze podataka


            XmlSerializer xs = new XmlSerializer(typeof(List<String>));
            StringReader sr = new StringReader(xml);
            List<String> templist = (List<String>)xs.Deserialize(sr);

            foreach (var dbName in templist)
            {
                String xmlDb = File.ReadAllText(dbName);

                XmlSerializer xsDb = new XmlSerializer(typeof(List<Informacije>));
                StringReader srDb = new StringReader(xmlDb);
                List<Informacije> templistDb = (List<Informacije>)xsDb.Deserialize(srDb);

                Dictionary<int, Informacije> currentDb = new Dictionary<int, Informacije>();
                foreach (var info in templistDb)
                {
                    currentDb.Add(info.Id, info);
                }
                //kada smo procitali sve iz tekuce baze to dodajemo u DbList
                BpList.Add(dbName, currentDb);
            }

        }

        #endregion

        #region Admin's operations
        public string KreiranjeBaze(string bazaPodataka)
        {
            if(!BpList.ContainsKey(bazaPodataka))
            {
                BpList.Add(bazaPodataka, new Dictionary<int, Informacije>());
                SerializeData();
                return $"Baza podataka sa imenom '{bazaPodataka}' je uspesno kreirana\n";
            }
            return $"Baza podataka sa imenom '{bazaPodataka}' vec postoji\n";
        }

        public string BrisanjeBaze(string bazaPodataka)
        {
            if(BpList.ContainsKey(bazaPodataka))
            {
                BpList.Remove(bazaPodataka);
                return $"Baza podataka sa imenom '{bazaPodataka}' je uspesno izbrisana\n";
            }
            return $"Baza podataka sa imenom '{bazaPodataka}' ne postoji\n";
        }

        public string ArhiviranjeBaze(string bazaPodataka)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Writer's operations
        public string UpisUBazu(string poruka, byte[] promena)
        {
            string imeKlijenta = (Thread.CurrentPrincipal.Identity as GenericIdentity).Name.Split(',', ';')[0].Split('=')[1];
            //parts[0] = imeBaze
            //parts[1] = drzava
            //parts[2] = grad
            //parts[3] = godina
            //parts[4] = potrosnja
            //parts[5] = id

            string[] parts = poruka.Split(':');
            int id = Int32.Parse(parts[5]);

            if(BpList.ContainsKey(parts[0]))
            {
                Informacije informacije = new Informacije() { Drzava = parts[1].Trim().ToLower(), Grad = parts[2].Trim().ToLower(), Godina = parts[3].Trim().ToLower(), MesecnaPotrosnja = Double.Parse(parts[4]) };
                BpList[parts[0]].Add(informacije.Id, informacije);
                return $"Nov entitet je dodat u bazu podataka '{parts[0]}'.\n";
            }
            else
            {
                return $"Baza podataka sa imenom '{parts[0]}' ne postoji.\n";
            }

            //return $"Poruka je promenjana\n";
        }

        public string ModifikacijaBaze(string poruka, byte[] promena)
        {
            string imeKlijenta = (Thread.CurrentPrincipal.Identity as GenericIdentity).Name.Split(',', ';')[0].Split('=')[1];
            //parts[0] = imeBaze
            //parts[1] = drzava
            //parts[2] = grad
            //parts[3] = godina
            //parts[4] = potrosnja
            //parts[5] = id

            string[] parts = poruka.Split(':');
            int id = Int32.Parse(parts[5]);

            if(BpList.ContainsKey(parts[0]))
            {
                if(BpList[parts[0]].ContainsKey(id))
                {
                    BpList[parts[0]][id].Drzava = parts[1].Trim().ToLower();
                    BpList[parts[0]][id].Grad = parts[2].Trim().ToLower();
                    BpList[parts[0]][id].Godina = parts[3].Trim().ToLower();
                    BpList[parts[0]][id].MesecnaPotrosnja = Double.Parse(parts[4]);
                    SerializeData();
                    //BpList[parts[0]][id].Drzava = parts[1].Trim().ToLower();

                    return $"Postojeci entitet sa id-em '{id}' u bazi podataka '{parts[0]}' je uspesno promenjen\n";
                }
                else
                {
                    return $"Entitet sa id-em '{id}' ne postoji u pazi podataka '{parts[0]}'\n";
                }
            }

            return $"Baza podataka sa imenom '{parts[0]}' nije pronadjena\n";
        }
        #endregion

        #region Reader's operations
        public string CitanjeBaze(string bazaPodataka)
        {
            string poruka = "-------------------------------------";
            string imeKlijenta = (Thread.CurrentPrincipal.Identity as GenericIdentity).Name.Split(',', ';')[0].Split('=')[1];

            if(BpList.ContainsKey(bazaPodataka))
            {
                foreach(Informacije informacije in BpList[bazaPodataka].Values)
                {
                    poruka += informacije.ToString();
                }

                poruka += "--------------------------------------";
                return poruka;
            }
            else
            {
                return $"Baza podataka sa imenom '{bazaPodataka}' ne postoji\n";
            }
        }

        public string SrednjaVrednostZaOdredjeniGrad(string bazaPodataka, string grad)
        {
            string imeKlijenta = (Thread.CurrentPrincipal.Identity as GenericIdentity).Name.Split(',', ';')[0].Split('=')[1];

            Dictionary<string, List<Informacije>> informacije = new Dictionary<string, List<Informacije>>();
            string poruka = "-----------------------------------\nProsecna potrosnja za grad: \n\n";

            if(BpList.ContainsKey(bazaPodataka))
            {
                foreach(Informacije info in BpList[bazaPodataka].Values)
                {
                    if (info.Grad == grad.Trim().ToLower())
                    {
                        if (informacije.ContainsKey(info.Grad))
                        {
                            informacije[info.Grad].Add(info);
                        }
                        else
                        {
                            informacije.Add(info.Grad, new List<Informacije>() { info });
                        }
                    }
                }
                foreach (var i in informacije)
                {
                    poruka += $"{i.Key}:\t{i.Value.Average(x => x.MesecnaPotrosnja).ToString()}\n";
                }
            }
            else
            {
                return $"Baza podataka sa imenom '{bazaPodataka}' ne postoji.\n";
            }

            return poruka += "\n-------------------------------------\n";
        }

        public string SrednjaVrednostZaOdredjeniRegion(string bazaPodataka, string drzava)
        {
            string imeKlijenta = (Thread.CurrentPrincipal.Identity as GenericIdentity).Name.Split(',', ';')[0].Split('=')[1];

            Dictionary<string, List<Informacije>> informacije = new Dictionary<string, List<Informacije>>();

            string poruka = "-------------------------------------------\nProsecna potrosnja za odredjenu drzavu: \n\n";

            if(BpList.ContainsKey(bazaPodataka))
            {
                foreach(Informacije info in BpList[bazaPodataka].Values)
                {
                    if(info.Drzava == drzava.Trim().ToLower())
                    {
                        if (informacije.ContainsKey(info.Drzava))
                        {
                            informacije[info.Drzava].Add(info);
                        }
                        else
                        {
                            informacije.Add(info.Drzava, new List<Informacije>() { info });
                        }
                    }
                }
                foreach(var i in informacije)
                {
                    poruka += $"{i.Key}:\t{i.Value.Average(x => x.MesecnaPotrosnja).ToString()}\n";
                }
            }
            else
            {
                return $"Baza podataka sa imenom '{bazaPodataka}' ne postoji\n";
            }

            return poruka += "\n---------------------------------\n";
        }

        public string NajveciPotrosacZaRegion(string bazaPodataka)
        {
            string imeKlijenta = (Thread.CurrentPrincipal.Identity as GenericIdentity).Name.Split(',', ';')[0].Split('=')[1];

            Dictionary<string, List<Informacije>> informacije = new Dictionary<string, List<Informacije>>();

            string poruka = "-----------------------------------\nMaksimalni potrosac iz odredjenog regiona:\n\n";

            if(BpList.ContainsKey(bazaPodataka))
            {
                foreach(Informacije info in BpList[bazaPodataka].Values)
                {
                    Console.WriteLine("Koju drzavu zelite");
                    string drzava = Console.ReadLine();
                    if(informacije.ContainsKey(info.Drzava))
                    {
                        informacije[info.Drzava].Add(info);
                    }
                    else
                    {
                        informacije.Add(info.Drzava, new List<Informacije>() { info });
                    }
                }

                foreach(var i in informacije)
                {
                    poruka += $"{i.Key}:\t{i.Value.Max(x => x.MesecnaPotrosnja).ToString()}\n";
                }
            }
            else
            {
                return $"Baza podataka sa imenom '{bazaPodataka}' nije pronadjena.\n";
            }

            return poruka += "\n---------------------------------\n";
        }
        #endregion
    }
}
