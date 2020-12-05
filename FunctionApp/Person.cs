using System;

namespace FunctionApp
{
    public class Person
    {
        public DateTime CreatedTime { get; set; } = DateTime.UtcNow;      
        public string Name { get;  set; }
        public string Id { get; set; } = Guid.NewGuid().ToString("n");
        public bool IsActive { get;  set; }
    }
}