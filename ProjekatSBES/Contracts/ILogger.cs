using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    [ServiceContract]
    public interface ILogger
    {
        [OperationContract]
        void KorisnickeOperacije(string korisnik, string operacija, string informacija);
        [OperationContract]
        void UspesnaAutentifikacija(string korisnik);
        [OperationContract]
        void NeuspesnaAutentifikacija(string korisnik);
        [OperationContract]
        void UspesnaAutorizacija(string korisnik);
        [OperationContract]
        void NeuspesnaAutorizacija(string korisnik, string razlog);
    }
}
