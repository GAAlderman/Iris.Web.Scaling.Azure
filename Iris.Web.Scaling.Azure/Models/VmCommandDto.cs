using System;

namespace Iris.Web.Scaling.Azure.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class VmCommandDto : IDataTransferObject
    {
        /// <summary>
        /// 
        /// </summary>
        public string SubscriptionId { get; set; }
         
        /// <summary>
        /// 
        /// </summary>
        public string VmName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string VmRoleSize { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public int SSHPort { get; set; }
 
        /// <summary>
        /// 
        /// </summary>
        public int MirthWebPort { get; set; }
 
        /// <summary>
        /// 
        /// </summary>
        public int MirthAdminPort { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string VHDFileName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string VMUserName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string VMPassword { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string SourceImage { get; set; }

        public VmCommandDto(string subscriptionId, string vmName)
        {
            SubscriptionId = subscriptionId;
            VmName = vmName;
            VmRoleSize = string.Empty;
            SSHPort = 0;
            MirthWebPort = 0;
            MirthAdminPort = 0;
            VHDFileName = string.Empty;
            VMUserName = string.Empty;
            VMPassword = string.Empty;
            SourceImage = string.Empty;
        }

    }
}