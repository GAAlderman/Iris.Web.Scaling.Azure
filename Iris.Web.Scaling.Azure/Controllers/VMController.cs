using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Remoting;
using System.Web.Http;
using Iris.Web.Scaling.Azure.ActionFilters;
using Iris.Web.Scaling.Azure.Models;

namespace Iris.Web.Scaling.Azure.Controllers
{
    public class VMController : ApiController
    {

        /// <summary>Allan Alderman 1-12-2013
        /// This method starts a VM within Azure
        /// </summary>
        /// <param name="vmCommand">VMCommand DTO specifying the command options.</param>
        /// <returns>HttpStatus.OK if successful, false if not.</returns>
        [ClientVerification]
        [HttpGet]
        public HttpResponseMessage StartVM(VmCommandDto vmCommand)
        {
            try
            {
                //the cert thumbprint must be passed as the Authorization-Token.
                //use it to fetch the cert to attach to the request.
                var thumbprint = Request.Headers.GetValues("Authorization-Token").First();

                //verify arguments
                if (vmCommand == null)
                {
                    throw new ArgumentNullException("vmCommand");
                }
                if (string.IsNullOrEmpty(vmCommand.SubscriptionId))
                {
                    throw new ArgumentException("SubscriptionId must be supplied.");
                }
                if (vmCommand.SSHPort == 0)
                {
                    throw new ArgumentException("SSHPort must be supplied.");
                }
                if (vmCommand.MirthWebPort == 0)
                {
                    throw new ArgumentException("Mirth Web Port must be supplied.");
                }
                if (vmCommand.MirthAdminPort == 0)
                {
                    throw new ArgumentException("Mirth Admin Port must be supplied.");
                }
                if (string.IsNullOrEmpty(vmCommand.VmName))
                {
                    throw new ArgumentException("VMName must be supplied.");
                }
                if (string.IsNullOrEmpty(vmCommand.VmRoleSize))
                {
                    throw new ArgumentException("VmRoleSize must be supplied.");
                }
                if (string.IsNullOrEmpty(vmCommand.VHDFileName))
                {
                    throw new ArgumentException("VHDFileName must be supplied.");
                }
                if (string.IsNullOrEmpty(vmCommand.VMUserName))
                {
                    throw new ArgumentException("VMUserName must be supplied.");
                }
                if (string.IsNullOrEmpty(vmCommand.VMPassword))
                {
                    throw new ArgumentException("VMPassword must be supplied.");
                }
                if (string.IsNullOrEmpty(vmCommand.SourceImage))
                {
                    throw new ArgumentException("SourceImage must be supplied.");
                }

                //construct the REST call to start the VM under the service created above
                var uri = String.Format(
                    "https://management.core.windows.net/{0}/services/hostedservices/{1}/deployments",
                    vmCommand.SubscriptionId,
                    vmCommand.VmName);

                //construct the parameters that Azure will use to configure the VM
                var configParams = new Dictionary<string, string>();
                configParams["VMNAME"] = vmCommand.VmName;
                configParams["VMROLESIZE"] = vmCommand.VmRoleSize;
                configParams["SSHPORT"] = vmCommand.SSHPort.ToString(CultureInfo.InvariantCulture);
                configParams["MIRTHWEBPORT"] = vmCommand.MirthWebPort.ToString(CultureInfo.InvariantCulture);
                configParams["MIRTHADMINPORT"] = vmCommand.MirthAdminPort.ToString(CultureInfo.InvariantCulture);
                configParams["VHDFILENAME"] = vmCommand.VHDFileName;
                configParams["USERNAME"] = vmCommand.VMUserName;
                configParams["PASSWORD"] = vmCommand.VMPassword;
                configParams["SOURCEIMAGE"] = vmCommand.SourceImage;

                //insert the parameters into the configuration XML
                var configurationXML = configParams.Keys.Aggregate(
                    SmapiConstants.VMConfigurationXML,
                    (xml, settingName) =>
                        xml.Replace(SmapiCommandHandlerControllerUtils.FormatKeyAsParameter(settingName),
                            configParams[settingName]));

                //Issue the request
                var responseMsg = SmapiCommandHandlerControllerUtils.IssueAzureCommand(uri,
                    "POST",
                    thumbprint,
                    configurationXML);

                //fetch the response and check to be sure it was what we expected
                if (responseMsg == null)
                {
                    throw new ServerException("The call to Create the VM did not complete.  Null response returned.");
                }
                //accepted is the only acceptable response
                if (responseMsg.StatusCode != HttpStatusCode.Accepted)
                {
                    throw new ServerException(
                        "The call to Create the VM  did not complete.  Error message returned:  " +
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


        /// <summary>Allan Alderman 1-12-2013
        /// This method stops a running VM and deletes associates services and media
        /// </summary>
        /// <param name="vmCommand">VMCommand DTO specifying the command options.</param>
        [ClientVerification]
        [HttpGet]
        public HttpResponseMessage StopVM(VmCommandDto vmCommand)
        {
            try
            {
                //the cert thumbprint must be passed as the Authorization-Token.
                //use it to fetch the cert to attach to the request.
                var thumbprint = Request.Headers.GetValues("Authorization-Token").First();

                //verify arguments
                if (vmCommand == null)
                {
                    throw new ArgumentNullException("vmCommand");
                }
                if (string.IsNullOrEmpty(vmCommand.SubscriptionId))
                {
                    throw new ArgumentException("SubscriptionId must be supplied.");
                }
                if (string.IsNullOrEmpty(vmCommand.VmName))
                {
                    throw new ArgumentException("VMName must be supplied.");
                }
                if (string.IsNullOrEmpty(vmCommand.SubscriptionId))
                {
                    throw new ArgumentException("SubscriptionId must be supplied.");
                }

                //make up the Azure command to stop the VM
                //var uri = String.Format("https://management.core.windows.net/{0}/services/hostedservices/{1}/deployments/{2}/roles/{3}?comp=media",
                var uri = String.Format(
                    "https://management.core.windows.net/{0}/services/hostedservices/{1}?comp=media",
                    vmCommand.SubscriptionId,
                    vmCommand.VmName);

                //Issue the request to create the cloud service
                var responseMsg = SmapiCommandHandlerControllerUtils.IssueAzureCommand(uri,
                    "DELETE",
                    thumbprint,
                    string.Empty);
                //if the status code is Accepted or not found, then consider the delete successful
                if (responseMsg.StatusCode != HttpStatusCode.Accepted &&
                    responseMsg.StatusCode != HttpStatusCode.NotFound)
                {
                    throw new ServerException("The call to Delete the VM did not complete.  Error message returned:" +
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