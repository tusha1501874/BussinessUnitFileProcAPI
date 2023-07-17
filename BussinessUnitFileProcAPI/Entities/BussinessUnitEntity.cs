using BussinessUnitFileProcAPI.Models;
using BussinessUnitFileProcAPI.Models.Utilities;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace BussinessUnitFileProcAPI.Entities;
public class BussinessUnitEntity : TableEntity
    {
        public BussinessUnitEntity()
        {
            PartitionKey = BusinessUnit;
            RowKey = Guid.NewGuid().ToString();
        }
        public string BatchID { get; set; }
        public string BusinessUnit { get; set; }

        [EntityJsonPropertyConverter]
        public Acl acl { get; set; }

        [EntityJsonPropertyConverter]
        public List<Attributes> Attributes { get; set; }
        public DateTime expiryDate { get; set; }

        [EntityJsonPropertyConverter]
        public List<Models.File> Files { get; set; }

        public override IDictionary<string, EntityProperty> WriteEntity(OperationContext operationContext)
        {
            IDictionary<string, EntityProperty> results = base.WriteEntity(operationContext);
            EntityJsonPropertyConverter.Serialize(this, results);
            return results;
        }

        public override void ReadEntity(IDictionary<string, EntityProperty> properties, OperationContext operationContext)
        {
            base.ReadEntity(properties, operationContext);
            EntityJsonPropertyConverter.Deserialize(this, properties);
        }
    }