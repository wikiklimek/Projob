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
    [JsonDerivedType(typeof(CargoPlane), typeDiscriminator: "CP")]
    public class CargoPlane : Plane
    {
        //private static string? _value = "CP";
        public override string? Value { get; set; }
        public Single MaxLoad { get; set; }
        public CargoPlane(UInt64 ID, string Serial, string Country, string Model, Single MaxLoad)
            : base(ID, Serial, Country, Model)
        {
            Value = "CP";
            this.MaxLoad = MaxLoad;
        }
        public CargoPlane() : base()
        {
            Value = "CP";
            this.MaxLoad = 0;
        }
        override public string Reported(IVisitor visitor) => visitor.VisitCargoPlane(this);
    }
}
