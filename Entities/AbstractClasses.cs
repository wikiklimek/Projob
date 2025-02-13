using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Linq;
using NetworkSourceSimulator;

namespace ProjOb
{

    [JsonPolymorphic]
    [JsonDerivedType(typeof(Crew), typeDiscriminator: "C")]
    [JsonDerivedType(typeof(Passenger), typeDiscriminator: "P")]
    [JsonDerivedType(typeof(Cargo), typeDiscriminator: "CA")]
    [JsonDerivedType(typeof(CargoPlane), typeDiscriminator: "CP")]
    [JsonDerivedType(typeof(PassengerPlane), typeDiscriminator: "PP")]
    [JsonDerivedType(typeof(Airport), typeDiscriminator: "AI")]
    [JsonDerivedType(typeof(Flight), typeDiscriminator: "FL")]
    abstract public class Object1
    {
        public UInt64 ID { get; set; }
        public virtual string? Value { get; set; }
        public Object1() 
        {
            ID = UInt64.MaxValue;
        }
        public Object1(UInt64 ID) { this.ID = ID; }
    }

    [JsonPolymorphic]
    [JsonDerivedType(typeof(Person))]

    abstract public class Person : Object1
    {
        public string Name { get; set; }
        public UInt16 Age { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public Person(UInt64 ID, string Name, UInt16 Age, string Phone, string Email) : base(ID)
        {
            this.Name = Name;
            this.Age = Age;
            this.Phone = Phone;
            this.Email = Email;
        }
        public Person() : base()
        {
            this.Name = "default";
            this.Age = 0;
            this.Phone = "000-000-000";
            this.Email = "email@email.com";
        }

    }


    [JsonPolymorphic]
    [JsonDerivedType(typeof(Plane))]
    abstract public class Plane : Object1, IReportable
    {
        public string Serial { get; set; }
        public string Country { get; set; }
        public string Model { get; set; }
        public Plane(UInt64 ID, string Serial, string Country, string Model) : base(ID)
        {
            this.Serial = Serial;
            this.Country = Country;
            this.Model = Model;

        }
        public Plane() : base()
        {
            this.Serial = "default";
            this.Country = "defaut";
            this.Model = "default";

        }
        abstract public string Reported(IVisitor Visitor);
    }

}




