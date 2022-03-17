using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    [Serializable]
    public class Informacije
    {
        public static int count = 0;
        public int Id { get; set; }
        public String Grad { get; set; }
        public String Drzava { get; set; }
        public String Godina { get; set; }
        public double MesecnaPotrosnja { get; set; }

        public Informacije()
        {
            Id = ++count;
        }

        public override string ToString()
        {
            return $"\nID: {Id}\nGrad: {Grad}\nDrzava: {Drzava}\nMesecna potrosnja: {MesecnaPotrosnja}\nGodina: {Godina}" + Environment.NewLine +Environment.NewLine;
        }
    }
}
