using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    [ServiceContract]
    public interface IWCFService
    {
        #region Admin's operations
        [OperationContract]
        string KreiranjeBaze(string bazaPodataka);
        [OperationContract]
        string BrisanjeBaze(string bazaPodataka);
        [OperationContract]
        string ArhiviranjeBaze(string bazaPodataka);
        #endregion

        #region Writer 's operations
        [OperationContract]
        string UpisUBazu(string poruka, byte[] promena);
        [OperationContract]
        string ModifikacijaBaze(string poruka, byte[] promena);
        #endregion

        #region Reader's operations
        [OperationContract]
        string CitanjeBaze(string bazaPodataka);
        [OperationContract]
        string SrednjaVrednostZaOdredjeniGrad(string bazaPodataka, string grad);
        [OperationContract]
        string SrednjaVrednostZaOdredjeniRegion(string bazaPodataka, string drzava);
        [OperationContract]
        string NajveciPotrosacZaRegion(string bazaPodataka);
        #endregion
    }
}