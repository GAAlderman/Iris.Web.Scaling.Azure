using System;
using System.Diagnostics;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml.Linq;

namespace Iris.Web.Scaling.Azure.Controllers
{
    public class SmapiCommandHandlerControllerUtils
    {

        private static bool _isDebug = false;

        /// <summary>Allan Alderman 1-12-2014
        /// This method formats keys for processign as parameters in XML files.  In IRIS, parameters in
        /// XML files are uppercase and have '%' before and after.  Example: '%VMNAME%.
        /// </summary>
        /// <param name="key">The key to format</param>
        /// <returns>The key formatted as a parameter in an XML file.</returns>
        public static string FormatKeyAsParameter(string key)
        {
            var param = key.ToUpper();
            if (!param.StartsWith("%")) param = "%" + param;
            if (!param.EndsWith("%")) param += "%";
            return param;
        }

        //namespaces used by XML
        public static readonly XNamespace AzureNamespace = "http://schemas.microsoft.com/windowsazure";
        public static readonly XNamespace XmlSchema = "http://www.w3.org/2001/XMLSchema-instance";
        public static readonly string AzureVmRestApiUri = "https://management.core.windows.net/";
        private static HttpClient _http;

        /// <summary>
        /// Allan Alderman 1-10-2014
        /// Assembles a VM Managment client, attaching the correct management certificate.
        /// </summary>
        /// <param name="thumbprint">The thumbprint of the certificate to attach to the command.</param>
        /// <returns>An HTTP client that is ready and authorized to issue commands to Azure's REST API.</returns>
        public static HttpClient CreateVMManagementClient(string thumbprint)
        {
            if (_http != null) return _http;
            var handler = new WebRequestHandler();
            handler.ClientCertificates.Add(ConstructX509Certificate());
            _http = new HttpClient(handler) {BaseAddress = new Uri(AzureVmRestApiUri)};
            _http.DefaultRequestHeaders.Add("x-ms-version", "2013-08-01");
            return _http;
        }


        /// <summary>
        /// Allan Alderman 1-10-2014
        /// Pass in certThumbprint to azure commands
        /// Executes a command against the Azure Management API, which is a REST interface for managing Azure.
        /// </summary>
        /// <param name="uri">The URI to call</param>
        /// <param name="method">The method to use for the invocation.  May be "POST", "PUT", "GET", or "DELETE"</param>
        /// <param name="thumbprint">The thumbprint of the certificate to attach to the command.</param>
        /// <param name="requestBodyXML">The XML to pass to the REST service.  This can be an empty string.</param>
        /// <returns>The HttpResponse returned by the REST call.</returns>
        public static HttpResponseMessage IssueAzureCommand(string uri, string method, string thumbprint,
            string requestBodyXML = "")
        {
            //construct the HTTP request
            var http = CreateVMManagementClient(thumbprint);
            HttpContent content = new StringContent(requestBodyXML, Encoding.UTF8, "application/xml");

            //issue the request and return the result
            switch (method.ToUpper())
            {
                case "POST":
                    return http.PostAsync(uri, content).Result;

                case "PUT":
                    return http.PutAsync(uri, content).Result;

                case "GET":
                    return http.GetAsync(uri).Result;

                case "DELETE":
                    return http.DeleteAsync(uri).Result;

                default:
                    throw new ArgumentException("method");
            }
        }

        /// <summary>
        /// Allan Alderman 1-10-2014
        /// Constructs a X.509 Certificate from a byte array containing the pfx
        /// </summary>
        /// <returns>The X509 certificate</returns>
        private static X509Certificate2 ConstructX509Certificate()
        {
            return new X509Certificate2(Convert.FromBase64String(SmapiConstants.ManagementCertificate));
        }

        /// <summary>
        /// Allan Alderman 1-12-2014
        /// Fetches a X.509 Certificate from the local machine store using its thumbprint
        /// </summary>
        /// <param name="thumbprint">The thumbprint of the certificate to fetch.</param>
        /// <returns>The X509 certificate.  Throws an exception if the certificate is not found.</returns>
        public static X509Certificate2 FetchX509Certificate(string thumbprint)
        {
            // Try to open the store.
            SetDebugState();
            var certStore = _isDebug
                ? new X509Store(StoreName.My, StoreLocation.CurrentUser)
                : new X509Store(StoreName.My, StoreLocation.LocalMachine);
            certStore.Open(OpenFlags.ReadOnly);

            // Find the certificate that matches the thumbprint.
            var certCollection = certStore.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, false);
            certStore.Close();

            // Check to see if our certificate was added to the collection. If no, throw an error, if yes, return the cert
            if (0 == certCollection.Count)
            {
                throw new Exception(string.Format("Could not find certificate with thumbprint {0}", thumbprint));
            }
            return certCollection[0];
        }

        [Conditional("DEBUG")]
        private static void SetDebugState()
        {
            _isDebug = true;
        }

    }
}