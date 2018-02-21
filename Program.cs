namespace TableStorageSample
{
    class Program
    {
        static void Main(string[] args)
        {
            var peopleTableReference = TableOperations.CreateTable();

            //EntityOperations.AddEntityToTable(peopleTableReference);

            //EntityOperations.InsertBatchOperation(peopleTableReference);

            //EntityOperations.RetrieveRangeOfEntities(peopleTableReference);

            //EntityOperations.ReplaceEntity(peopleTableReference);

            //EntityOperations.InsertOrReplaceEntity(peopleTableReference);

            //EntityOperations.RetrieveSingleEntity(peopleTableReference, "Jones", "Fred");

            //EntityOperations.QuerySubSetOfEntityProperties(peopleTableReference);

            //EntityOperations.DeleteEntity(peopleTableReference);

            //EntityOperations.RetrieveAllEntities(peopleTableReference);

            //TableOperations.DeleteTable(peopleTableReference);

            EntityOperations.RetrieveEntitiesInPagesAsync(peopleTableReference);
        }
    }
}
