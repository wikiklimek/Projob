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
    [JsonDerivedType(typeof(Airport), typeDiscriminator: "AI")]
    public class Airport : Object1, IReportable
    {
        public override string? Value { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public Single Longitude { get; set; }
        public Single Latitude { get; set; }
        public Single AMSL { get; set; }
        public string ISO { get; set; }
        public Airport(UInt64 ID, string Name, string Code, Single Longitude, Single Latitude, Single AMSL, string ISO) : base(ID)
        {
            this.Name = Name;
            this.Code = Code;
            this.Longitude = Longitude;
            this.Latitude = Latitude;
            this.AMSL = AMSL;
            this.ISO = ISO;
            this.Value = "AI";
        }
        public Airport() : base()
        {
            this.Name = "default";
            this.Code = "default";
            this.Longitude = 0;
            this.Latitude = 0;
            this.AMSL = 0;
            this.ISO = "default";
            this.Value = "AI";
        }
        public string Reported(IVisitor visitor) => visitor.VisitAirport(this);
    }

}
