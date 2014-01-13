using System;

namespace Iris.Web.Scaling.Azure.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class ServiceCommandDto : IDataTransferObject
    {
        /// <summary>
        /// 
        /// </summary>
        public string SubscriptionId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ServerName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ServiceLocation { get; set; }

    }
}