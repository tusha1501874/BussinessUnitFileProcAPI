namespace BussinessUnitFileProcAPI.Models;
public class Acl
    {
        public List<ReadUser> ReadUsers { get; set; } = new();
        public List<ReadGroup> ReadGroups { get; set; } = new();
    }