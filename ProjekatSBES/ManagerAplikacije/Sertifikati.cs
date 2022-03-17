using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ManagerAplikacije
{
    public class Sertifikati
    {
        public static X509Certificate2 GetCertificateFromStorage(StoreName storeName, StoreLocation storeLocation, string subjectName)
        {
            /*X509Certificate2 certificate = null;
            var certificateStore = new X509Store(storeName, storeLocation);

            certificateStore.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);

            var certificateCollection = certificateStore.Certificates.Find(X509FindType.FindBySubjectName, subjectName, true);

            certificateStore.Close();
            if (certificateCollection != null)
            {
                certificate = certificateCollection[0];
            }
            return certificate;*/

            X509Store store = new X509Store(storeName, storeLocation);
            store.Open(OpenFlags.ReadOnly);

            X509Certificate2Collection certCollection = store.Certificates.Find(X509FindType.FindBySubjectName, subjectName, false);

            /// Check whether the subjectName of the certificate is exactly the same as the given "subjectName"
            foreach (X509Certificate2 c in certCollection)
            {
                if (c.SubjectName.Name.Equals(string.Format("CN={0}", subjectName)))
                {
                    return c;
                }
            }

            return null; 
        }


        /// <summary>
        /// Get a certificate from file.		
        /// </summary>
        /// <param name="fileName"> .cer file name </param>
        /// <returns> The requested certificate. If no valid certificate is found, returns null. </returns>
        public static X509Certificate2 GetCertificateFromFile(string fileName)
        {
            X509Certificate2 certificate = null;


            return certificate;
        }

        /// <summary>
        /// Get a certificate from file.
        /// </summary>
        /// <param name="fileName">.pfx file name</param>
        /// <param name="pwd"> password for .pfx file</param>
        /// <returns>The requested certificate. If no valid certificate is found, returns null.</returns>
		public static X509Certificate2 GetCertificateFromFile(string fileName, SecureString pwd)
        {
            X509Certificate2 certificate = null;


            return certificate;
        }
    }
}
