using Microsoft.Azure; // Namespace for CloudConfigurationManager
using Microsoft.WindowsAzure.Storage; // Namespace for StorageAccounts
using Microsoft.WindowsAzure.Storage.Table;

namespace TableStorageSample
{
    public static class TableOperations
    {
        public static CloudTable CreateTable()
        {
            // Parse the connection string and return a reference to the storage account.
            var cloudConfigurationManagerSetting = CloudConfigurationManager.GetSetting("StorageConnectionString");
            var storageAccount = CloudStorageAccount.Parse(cloudConfigurationManagerSetting);

            // Create a CloudTableClient object from the storage account.
            // This object is the root object for all operations on the 
            // table service for this particular account.
            var tableClient = storageAccount.CreateCloudTableClient();

            // Retrieve a reference to the table.
            var cloudTable = tableClient.GetTableReference("people");
            cloudTable.CreateIfNotExists();

            return cloudTable;
        }

        public static void DeleteTable(CloudTable table)
        {
            table.DeleteIfExists();
        }
    }
}