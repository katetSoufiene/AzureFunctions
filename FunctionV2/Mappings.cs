namespace ServerlessFuncsV2
{
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
