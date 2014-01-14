using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Remoting;
using System.Web.Http;
using Iris.Web.Scaling.Azure.ActionFilters;
using Iris.Web.Scaling.Azure.Models;
using Iris.Lib.Utils;

namespace Iris.Web.Scaling.Azure.Controllers
{
    public class ServiceController : ApiController
    {
        /// <summary>Allan Alderman 12-31-2013
        /// This method starts a VM within Azure
        /// </summary>
        /// <returns>Http OK result if successful; throwst.</returns>
        [RequiresCommandHandlerAuthorization]
        [HttpPost]
        public HttpResponseMessage CreateService(ServiceCommandDto serviceCommand)
        {
            try
            {
                //verify arguments
                if (serviceCommand == null)
                {
                    throw new ArgumentNullException("serviceCommand");
                }
                if(string.IsNullOrEmpty(serviceCommand.SubscriptionId))
                {
                    throw new ArgumentException("SubscriptionId must be supplied.");
                }
                if (string.IsNullOrEmpty(serviceCommand.ServiceLocation))
                {
                    throw new ArgumentException("Service Location must be supplied.");
                }
               
                //construct the REST call to create the cloud service
                var uri = String.Format("https://management.core.windows.net/{0}/services/hostedservices",
                    serviceCommand.SubscriptionId);

                //Construct the parameters that Azure will use to configure the cloud service.  Note that the cloud service
                //will be given the same name as the server.
                var configParams = new Dictionary<string, string>();
                configParams["SERVICENAME"] = serviceCommand.ServerName;
                configParams["SERVICELABEL"] = StringEncoder.Base64Encode(serviceCommand.ServerName);
                configParams["SERVICELOCATION"] = serviceCommand.ServiceLocation;

                //insert the parameters into the configuration XML
                var configurationXML = configParams.Keys.Aggregate(
                    SmapiConstants.ServiceConfigurationXML,
                    (xml, settingName) => xml.Replace(SmapiCommandHandlerControllerUtils.FormatKeyAsParameter(settingName), 
                        configParams[settingName]));

                //Issue the request to create the cloud service
                var responseMsg = SmapiCommandHandlerControllerUtils.IssueAzureCommand(uri,
                    "POST",
                    configurationXML);

                //fetch the response and check to be sure it was what we expected
                if (responseMsg == null)
                {
                    throw new ServerException(
                        "The call to Create the cloud service did not complete.  Null response returned.");
                }
                //HttpStatusCode.Created is the only acceptable response
                if (responseMsg.StatusCode != HttpStatusCode.Created)
                {
                    throw new ServerException(
                        "The call to Create the cloud service did not complete.  Error message returned:  " +
                        responseMsg.ReasonPhrase);
                }
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError,
                    ex.ToString()));
            }
        }



    }
}
