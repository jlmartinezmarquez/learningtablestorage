using Microsoft.WindowsAzure.Storage.Table;
using System;
using TableStorageSample.Entities;

namespace TableStorageSample
{
    public class EntityOperations
    {
        public static void DeleteEntity(CloudTable table)
        {
            var result = RetrieveSingleEntity(table, "Smith", "Ben");

            // Assign the result to a CustomerEntity.
            var deleteEntity = (CustomerEntity)result.Result;

            // Create the Delete TableOperation.
            if (deleteEntity != null)
            {
                var deleteOperation = TableOperation.Delete(deleteEntity);
                table.Execute(deleteOperation);
                Console.WriteLine("Entity deleted");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine("Could not retrieve the entity.");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }

        public static void QuerySubSetOfEntityProperties(CloudTable table)
        {
            // Define the query, and select only the Email property.
            var projectionQuery = new TableQuery<DynamicTableEntity>().Select(new string[] { "Email" });

            // Define an entity resolver to work with the entity after retrieval.
            EntityResolver<string> resolver = (partitionKey, rowKey, timeStamp, properties, etag) => properties.ContainsKey("Email") ? properties["Email"].StringValue : null;

            foreach (string projectedEmail in table.ExecuteQuery(projectionQuery, resolver, null, null))
            {
                Console.WriteLine(projectedEmail);
            }
            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        public static void InsertOrReplaceEntity(CloudTable table)
        {
            // Create a customer entity.
            CustomerEntity customer3 = new CustomerEntity("Jones", "Fred")
            {
                Email = "Fred@contoso.com",
                PhoneNumber = "425-555-0106"
            };

            // Create the TableOperation object that inserts the customer entity.
            TableOperation insertOperation = TableOperation.Insert(customer3);

            // Execute the operation.
            table.Execute(insertOperation);

            // Create another customer entity with the same partition key and row key.
            // We've already created a 'Fred Jones' entity and saved it to the
            // 'people' table, but here we're specifying a different value for the
            // PhoneNumber property.
            CustomerEntity customer4 = new CustomerEntity("Jones", "Fred")
            {
                Email = "Fred@contoso.com",
                PhoneNumber = "07581078721"
            };

            // Create the InsertOrReplace TableOperation.
            var insertOrReplaceOperation = TableOperation.InsertOrReplace(customer4);

            // Execute the operation. Because a 'Fred Jones' entity already exists in the
            // 'people' table, its property values will be overwritten by those in this
            // CustomerEntity. If 'Fred Jones' didn't already exist, the entity would be
            // added to the table.
            table.Execute(insertOrReplaceOperation);
        }

        public static void ReplaceEntity(CloudTable table)
        {
            // Create a retrieve operation that takes a customer entity.
            var retrieveOperation = TableOperation.Retrieve<CustomerEntity>("Smith", "Ben");

            // Execute the retrieve operation.
            var retrievedResult = table.Execute(retrieveOperation);

            var updateEntity = (CustomerEntity)retrievedResult.Result;

            if (updateEntity != null)
            {
                // Change the phone number.
                updateEntity.PhoneNumber = "91654879622";

                // Create the Replace TableOperation.
                var updateOperation = TableOperation.Replace(updateEntity);

                // Execute the operation.
                table.Execute(updateOperation);

                Console.WriteLine("Entity updated.");
            }
            else
            {
                Console.WriteLine("Entity could not be retrieved.");
            }
        }

        public static TableResult RetrieveSingleEntity(CloudTable table, string partitionKey, string rowKey)
        {
            var retrieveOperation = TableOperation.Retrieve<CustomerEntity>(partitionKey, rowKey);

            // Execute the retrieve operation.
            var retrievedResult = table.Execute(retrieveOperation);

            // Print the phone number of the result.
            if (retrievedResult.Result != null) Console.WriteLine(((CustomerEntity)retrievedResult.Result).PhoneNumber);
            else Console.WriteLine("The phone number could not be retrieved.");

            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();

            return retrievedResult;
        }

        public static void RetrieveAllEntities(CloudTable table)
        {
            var query = new TableQuery<CustomerEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "Smith"));

            DisplayResults(table, query);
        }

        public static void DisplayResults(CloudTable table, TableQuery<CustomerEntity> query)
        {
            // Loop through the results, displaying information about the entity.
            foreach (CustomerEntity entity in table.ExecuteQuery(query))
            {
                Console.WriteLine("{0}, {1}\t{2}\t{3}", entity.PartitionKey, entity.RowKey,
                    entity.Email, entity.PhoneNumber);
            }

            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        public static void RetrieveRangeOfEntities(CloudTable table)
        {
            // Create the table query.
            var rangeQuery = new TableQuery<CustomerEntity>().Where(
                TableQuery.CombineFilters(
                    TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "Smith"),
                    TableOperators.And,
                    TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.LessThan, "E")
                    )
                );

            DisplayResults(table, rangeQuery);
        }

        public static void InsertBatchOperation(CloudTable table)
        {
            // Create the batch operation.
            var batchOperation = new TableBatchOperation();

            // Create a customer entity and add it to the table.
            var customer1 = new CustomerEntity("Smith", "Jeff")
            {
                Email = "Jeff@contoso.com",
                PhoneNumber = "425-555-0104"
            };

            // Create another customer entity and add it to the table.
            var customer2 = new CustomerEntity("Smith", "Ben")
            {
                Email = "Ben@contoso.com",
                PhoneNumber = "425-555-0102"
            };

            // Add both customer entities to the batch insert operation.
            batchOperation.Insert(customer1);
            batchOperation.Insert(customer2);

            // Execute the batch operation.
            table.ExecuteBatch(batchOperation);
        }

        public static void AddEntityToTable(CloudTable table)
        {
            // Create a new customer entity.
            var customer1 = new CustomerEntity("Harp", "Walter")
            {
                Email = "Walter@contoso.com",
                PhoneNumber = "425-555-0101"
            };

            // Create the TableOperation object that inserts the customer entity.
            var insertOperation = TableOperation.Insert(customer1);

            table.Execute(insertOperation);  //Add entity to table
        }

        public static async void RetrieveEntitiesInPagesAsync(CloudTable table)
        {
            // Initialize a default TableQuery to retrieve all the entities in the table.
            var tableQuery = new TableQuery<CustomerEntity>();

            // Initialize the continuation token to null to start from the beginning of the table.
            TableContinuationToken continuationToken = null;

            do
            {
                // Retrieve a segment (up to 1,000 entities).
                TableQuerySegment<CustomerEntity> tableQueryResult = await table.ExecuteQuerySegmentedAsync(tableQuery, continuationToken);

                // Assign the new continuation token to tell the service where to
                // continue on the next iteration (or null if it has reached the end).
                continuationToken = tableQueryResult.ContinuationToken;

                // Print the number of rows retrieved.
                Console.WriteLine("Rows retrieved {0}", tableQueryResult.Results.Count);
            }
            while (continuationToken != null);

            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}
