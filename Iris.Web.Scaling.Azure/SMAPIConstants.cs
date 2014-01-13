using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Iris.Web.Scaling.Azure
{
    public class SmapiConstants
    {
        public const string ServiceConfigurationXML = @"<?xml version=""1.0"" encoding=""utf-8""?>
                <CreateHostedService xmlns=""http://schemas.microsoft.com/windowsazure"">
                  <ServiceName>%SERVICENAME%</ServiceName>
                  <Label>%SERVICELABEL%</Label>  
                  <Location>%SERVICELOCATION%</Location>
                </CreateHostedService>";


        public const string VMConfigurationXML = @"<Deployment xmlns=""http://schemas.microsoft.com/windowsazure"" xmlns:i=""http://www.w3.org/2001/XMLSchema-instance"">
                  <!--description and format: http://msdn.microsoft.com/en-us/library/jj157194.aspx -->
                  <Name>%VMNAME%</Name>
                  <DeploymentSlot>Production</DeploymentSlot>
                  <Label>%VMNAME%</Label>      
                  <RoleList> 
                  <Role> 
                  <RoleName>%VMNAME%</RoleName>
                  <RoleType>PersistentVMRole</RoleType>      
                      <ConfigurationSets>
                        <ConfigurationSet i:type=""LinuxProvisioningConfigurationSet"">
                          <ConfigurationSetType>LinuxProvisioningConfiguration</ConfigurationSetType>
                          <HostName>%VMNAME%</HostName>
                          <UserName>%USERNAME%</UserName> 
                          <UserPassword>%PASSWORD%</UserPassword>           
                        </ConfigurationSet>        
                        <ConfigurationSet> 
                          <ConfigurationSetType>NetworkConfiguration</ConfigurationSetType>
                          <InputEndpoints>
                             <InputEndpoint>
			                  <LocalPort>8443</LocalPort>
			                  <Name>MirthAdmin</Name>
			                  <Port>%MIRTHADMINPORT%</Port>
			                  <Protocol>tcp</Protocol>
			                </InputEndpoint>
			                <InputEndpoint>
			                  <LocalPort>8080</LocalPort>
			                  <Name>MirthWeb</Name>
			                  <Port>%MIRTHWEBPORT%</Port>
			                  <Protocol>tcp</Protocol>
			                </InputEndpoint>
			                <InputEndpoint>
			                  <LocalPort>22</LocalPort>
			                  <Name>SSH</Name>
			                  <Port>%SSHPORT%</Port>
			                  <Protocol>tcp</Protocol>
			                </InputEndpoint>
                          </InputEndpoints>       
                        </ConfigurationSet>
                      </ConfigurationSets>  	
                      <OSVirtualHardDisk>
                        <HostCaching>ReadWrite</HostCaching> 
                        <MediaLink>%VHDFILENAME%</MediaLink>
                        <SourceImageName>%SOURCEIMAGE%</SourceImageName>
                      </OSVirtualHardDisk>  
	                  <RoleSize>%VMROLESIZE%</RoleSize>
		                </Role>
	                  </RoleList>  
	                </Deployment>";

        public const string ManagementCertificate =
            "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";
    }
}