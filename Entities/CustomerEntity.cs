using Microsoft.WindowsAzure.Storage.Table;

namespace TableStorageSample.Entities
{
    public class CustomerEntity : TableEntity
    {
        public CustomerEntity(string lastName, string firstName)
        {
            //Tables are partitioned to support load balancing across storage nodes. 
            //A table's entities are organized by partition. 
            //A partition is a consecutive range of entities possessing the same partition key value. 
            //The partition key is a unique identifier for the partition within a given table
            PartitionKey = lastName;

            //The row key is a unique identifier for an entity within a given partition
            RowKey = firstName;
        }

        public CustomerEntity() { }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }
    }
}
