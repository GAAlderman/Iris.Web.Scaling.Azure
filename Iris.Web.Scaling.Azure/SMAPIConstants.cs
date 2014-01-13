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
                          <VMUserName>%USERNAME%</VMUserName> 
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

    }
}