using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace ServerlessFuncs
{
    public class User
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("n");
        public DateTime CreatedTime { get; set; } = DateTime.UtcNow;
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }

    public class UserCreateModel
    {
        public string Name { get; set; }
    }

    public class UserUpdateModel
    {
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }

    public class UserTableEntity : TableEntity
    {
        public DateTime CreatedTime { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }

    public static class Mappings
    {
        public static UserTableEntity ToTableEntity(this User user)
        {
            return new UserTableEntity()
            {
                PartitionKey = "USER",
                RowKey = user.Id,
                CreatedTime = user.CreatedTime,
                IsActive = user.IsActive,
                Name = user.Name
            };
        }

        public static User ToUser(this UserTableEntity user)
        {
            return new User()
            {
                Id = user.RowKey,
                CreatedTime = user.CreatedTime,
                IsActive = user.IsActive,
                Name = user.Name
            };
        }

    }
}
