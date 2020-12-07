using System;

namespace ServerlessFuncsV2
{
    public class User
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("n");
        public DateTime CreatedTime { get; set; } = DateTime.UtcNow;
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }
}
