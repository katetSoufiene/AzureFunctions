using System;

namespace FunctionAppStorageTable
{
    public class Person
    {
        public DateTime CreatedTime { get; set; } = DateTime.UtcNow;
        public string Name { get; set; }
        public string Id { get; set; } = Guid.NewGuid().ToString("n");
        public bool IsActive { get; set; }
    }

    public static class Mappings
    {
        public static PersonTableEntity PersonTableEntity(this Person person)
        {
            return new PersonTableEntity()
            {
                PartitionKey = "Person",
                RowKey = person.Id,
                Name = person.Name,
                IsActive = person.IsActive,
                CreatedTime = person.CreatedTime,
            };
        }

        public static Person ToPerson(this PersonTableEntity personTableEntity)
        {
            return new Person()
            {
                Id = personTableEntity.RowKey,
                Name = personTableEntity.Name,
                IsActive = personTableEntity.IsActive,
                CreatedTime = personTableEntity.CreatedTime,
            };
        }
    }
}