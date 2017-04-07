using Newtonsoft.Json;
using System;

namespace IEvangelist.DocumentDb.Models
{
    public class Person : BaseDocument
    {
        public string FirstName { get; set; }
        
        public string LastName { get; set; }
        
        public DateTime DateOfBirth { get; set; }
        
        public Sex Sex { get; set; }
    }

    public enum Sex
    {
        Male, Female
    }
}
