using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NetworkSourceSimulator;

namespace ProjOb
{

    [JsonPolymorphic]
    [JsonDerivedType(typeof(PassengerPlane), typeDiscriminator: "PP")]
    public class PassengerPlane : Plane
    {
        public override string? Value { get; set; }
        public UInt16 FirstClassSize { get; set; }
        public UInt16 BusinessClassSize { get; set; }
        public UInt16 EconomyClassSize { get; set; }
        public PassengerPlane(UInt64 ID, string Serial, string Country, string Model, UInt16 FirstClassSize,
            UInt16 BusinessClassSize, UInt16 EconomyClassSize)
            : base(ID, Serial, Country, Model)
        {
            this.FirstClassSize = FirstClassSize;
            this.BusinessClassSize = BusinessClassSize;
            this.EconomyClassSize = EconomyClassSize;
            this.Value = "PP";
        }
        public PassengerPlane() : base()
        {
            this.FirstClassSize = 0;
            this.BusinessClassSize = 0;
            this.EconomyClassSize = 0;
            this.Value = "PP";
        }
        override public string Reported(IVisitor visitor) => visitor.VisitPassengerPlane(this);
    }

}
