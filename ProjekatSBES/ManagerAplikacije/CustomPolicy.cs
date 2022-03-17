using Service_Client;
using System;
using System.Collections.Generic;
using System.IdentityModel.Claims;
using System.IdentityModel.Policy;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace ManagerAplikacije
{
    public class CustomPolicy : IAuthorizationPolicy
    {
        readonly string id;

        public CustomPolicy()
        {
            this.id = Guid.NewGuid().ToString();
        }

        public ClaimSet Issuer
        {
            get { return ClaimSet.System; }
        }

        public string Id
        {
            get { return id; }
        }

        public bool Evaluate(EvaluationContext evaluationContext, ref object state)
        {
            //Debugger.Launch();
            // podesava se custom principal i prosledjuje se WindowsIdentity

            object obj;
            if (!evaluationContext.Properties.TryGetValue("Identities", out obj))
                return false;

            List<WindowsIdentity> identities = obj as List<WindowsIdentity>;
            if (obj == null || identities.Count <= 0)
            {
                ServiceClientLoggerKomunikacija.Factory.NeuspesnaAutentifikacija(identities[0].Name.Split(',', ';')[0].Split('=')[1]);
                return false;
            }

            /*List<IIdentity> identities = obj as List<IIdentity>;
            if (obj == null || identities.Count <= 0)
            {
                ServiceClientLoggerKomunikacija.Factory.NeuspesnaAutentifikacija(identities[0].Name.Split(',', ';')[0].Split('=')[1]);
                return false;
            }*/

            //ServiceClientLoggerKomunikacija.Factory.UspesnaAutentifikacija(identities[0].Name.Split(',', ';')[0].Split('=')[1]);
            ServiceClientLoggerKomunikacija.Factory.UspesnaAutentifikacija(identities[0].Name.Split('=')[0].Split(',', ';')[1]);

            evaluationContext.Properties["Principal"] = new CustomPrincipal(identities[0]);
            return true;
        }
    }
}
