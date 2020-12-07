using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace ServerlessFuncsV2
{
    public class UserTableEntity : TableEntity
    {
        public DateTime CreatedTime { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }
}
