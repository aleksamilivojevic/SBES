using Service_Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ManagerAplikacije
{
    public class AutorizacijaManager : ServiceAuthorizationManager
    {
        protected override bool CheckAccessCore(OperationContext operationContext)
        {
            IPrincipal customPrincipal = operationContext.ServiceSecurityContext.AuthorizationContext.Properties["Principal"] as CustomPrincipal;

            if(!customPrincipal.IsInRole("View"))
            {
                //ServiceClientLoggerKomunikacija.Factory.NeuspesnaAutorizacija(customPrincipal.Identity.Name.Split(',', ';')[0].Split('=')[1], "Korisnik nije autorizovan");
                ServiceClientLoggerKomunikacija.Factory.NeuspesnaAutorizacija(customPrincipal.Identity.Name.Split('=')[0].Split(',', ';')[1], "Korisnik nije autorizovan");
            
                return false;
            }

            return true;
        }
    }
}
