using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace ManagerAplikacije
{
    public class CustomPrincipal : IPrincipal
    {
        /*WindowsIdentity windowsIdentity = null;

        public CustomPrincipal(WindowsIdentity identity)
        {
            windowsIdentity = identity;
        }

        public IIdentity Identity
        {
            get { return windowsIdentity; }
        }

        public bool IsInRole(string role)
        {
            foreach (IdentityReference group in this.windowsIdentity.Groups)
            {
                SecurityIdentifier sid = (SecurityIdentifier)group.Translate(typeof(SecurityIdentifier));
                var name = sid.Translate(typeof(NTAccount));
                string groupName = Formatter.ParseName(name.ToString());
                string permissions = RBACConfig.ResourceManager.GetString(groupName);
                if (permissions != null && permissions.Contains(role))
                {
                    return true;
                }
            }
            return false;

            
        }*/

        IIdentity identity;

        public CustomPrincipal(IIdentity identity)
        {
            this.identity = identity;
        }

        public IIdentity Identity
        {
            get { return identity; }
        }

        public bool IsInRole(string permission)
        {
            //string imeGrupe = identity.Name.Split(',', ';')[1].Split('=')[1];
            string imeGrupe = identity.Name.Split('=')[1].Split(',', ';')[1];

            string permissions = RBACConfig.ResourceManager.GetString(imeGrupe);

            if(permissions != null && permissions.Contains(permission))
            {
                return true;
            }

            return false;
        }
    }
}
