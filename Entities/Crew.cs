using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NetworkSourceSimulator;

namespace ProjOb
{
    

    [JsonPolymorphic]
    [JsonDerivedType(typeof(Crew), typeDiscriminator: "C")]
    public class Crew : Person
    {
        public override string? Value { get; set; }
        public UInt16 Practice { get; set; }
        public string Role { get; set; }
        public Crew(UInt64 ID, string Name, UInt16 Age, string Phone, string Email, UInt16 Practice, string Role)
            : base(ID, Name, Age, Phone, Email)
        {
            this.Role = Role;
            this.Practice = Practice;
            this.Value = "C";
        }
        public Crew(): base()
        {
            this.Role = "default";
            this.Practice = 0;
            this.Value = "C";
        }
    }

}

