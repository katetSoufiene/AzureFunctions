using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace FunctionAppStorageTable
{
    public class PersonTableEntity : TableEntity
    {
        public DateTime CreatedTime { get; set; } 
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }


}