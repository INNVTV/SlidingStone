using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlidingStone.TableEntities
{

    #region Interface

    /// <summary>
    /// Interface that allows for greater brevity in our DataAccess methods
    /// </summary>
    internal interface ILogTableEntity
    {
        string IPAddress { get; set; }
        string UserName { get; set; }
        string Activity { get; set; }
        string Description { get; set; }
        string Email { get; set; }
        string Company { get; set; }
        string ObjectID { get; set; }
        CloudTable cloudTable { get; set; }

    }

    #endregion

    #region Base Class

    /// <summary>
    /// Base class for all LogTableEntity Types
    /// </summary>
    abstract class LogTableEntity : TableEntity, ILogTableEntity
    {

        // Abstract properties (properties that are used for partition keys on LogTableEntity_ types)
        public abstract string IPAddress { get; set; }
        public abstract string UserName { get; set; }
        public abstract string Activity { get; set; }

        // Base Properties
        public string Description { get; set; }
        public string Email { get; set; }
        public string Company { get; set; }
        public string ObjectID { get; set; }
        public CloudTable cloudTable { get; set; }
    }

    #endregion

    #region Table Entities

    internal class LogTableEntity_IPAddress : LogTableEntity
    {
        public LogTableEntity_IPAddress()
        {
            RowKey = string.Format("{0:d19}+{1}", DateTime.MaxValue.Ticks - DateTime.UtcNow.Ticks, Guid.NewGuid().ToString("N"));
        }

        public override string IPAddress 
        {
            get { return PartitionKey; }
            set { PartitionKey = value; }
        }

        public override string UserName { get; set; }
        public override string Activity { get; set; }
               
    }

    internal class LogTableEntity_Activity : LogTableEntity
    {
        public LogTableEntity_Activity()
        {
            RowKey = string.Format("{0:d19}+{1}", DateTime.MaxValue.Ticks - DateTime.UtcNow.Ticks, Guid.NewGuid().ToString("N"));
        }

        public override string Activity  
        {
            get { return PartitionKey; }
            set { PartitionKey = value; }
        }

        public override string IPAddress { get; set; }
        public override string UserName { get; set; }

    }

    internal class LogTableEntity_Time : LogTableEntity
    {
        public LogTableEntity_Time()
        {
            PartitionKey = string.Format("{0:d19}+{1}", DateTime.MaxValue.Ticks - DateTime.UtcNow.Ticks, Guid.NewGuid().ToString("N"));
        }

        public override string UserName  
        {
            get { return RowKey; }
            set { RowKey = value; }
        }

        public override string IPAddress { get; set; }
        public override string Activity { get; set; }

    }

    #endregion

}
