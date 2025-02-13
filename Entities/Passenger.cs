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
    [JsonDerivedType(typeof(Passenger), typeDiscriminator: "P")]
    public class Passenger : Person
    {
        public override string? Value { get; set; }
        public string Class { get; set; }
        public UInt64 Miles { get; set; }
        public Passenger(UInt64 ID, string Name, UInt16 Age, string Phone, string Email, string Class, UInt64 Miles)
            : base(ID, Name, Age, Phone, Email)
        {
            this.Class = Class;
            this.Miles = Miles;
            this.Value = "P";
        }
        public Passenger() : base()
        {
            this.Class = "default";
            this.Miles = 0;
            this.Value = "P";
        }
    }
}
