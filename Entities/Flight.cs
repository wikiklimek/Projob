using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.ComponentModel.Design;
using Mapsui.Projections;
using NetworkSourceSimulator;

namespace ProjOb
{
    

    [JsonPolymorphic]
    [JsonDerivedType(typeof(Flight), typeDiscriminator: "FL")]
    public class Flight : Object1
    {
        public override string? Value { get; set; }
        public UInt64 OriginID { get; set; }
        public UInt64 TargetID { get; set; }
        public DateTime TakeoffTime { get; set; }
        public DateTime LandingTime { get; set; }
        public Single Longitute { get; set; }
        public Single Latitude { get; set; }
        public Single AMSL { get; set; }
        public UInt64 PlaneID { get; set; }
        public UInt64[]? Crew { get; set; }
        public UInt64[]? Load { get; set; }
        [JsonIgnore]
        public Airport? Origin { get; set; }
        [JsonIgnore]
        public Airport? Target { get; set; }
        [JsonIgnore]
        public Plane? Plane { get; set; }
        public Flight(UInt64 ID, UInt64 OriginID, UInt64 TargetID, DateTime TakeoffTime, DateTime LandingTime, Single Longitute, Single Latitude, Single AMSL, UInt64 PlaneID, UInt64[] Crew, UInt64[] Load) : base(ID)
        {
            Value = "FL";
            this.OriginID = OriginID;
            this.TargetID = TargetID;
            this.TakeoffTime = TakeoffTime;
            this.LandingTime = LandingTime;
            this.Longitute = Longitute;
            this.Latitude = Latitude;
            this.AMSL = AMSL;
            this.PlaneID = PlaneID;
            this.Crew = Crew;
            this.Load = Load;
        }
        public Flight(UInt64 ID, UInt64 OriginID, UInt64 TargetID, DateTime TakeoffTime, DateTime LandingTime, UInt64 PlaneID, UInt64[] Crew, UInt64[] PassCarg) : base(ID)
        {
            Value = "FL";
            this.OriginID = OriginID;
            this.TargetID = TargetID;
            this.TakeoffTime = TakeoffTime;
            this.LandingTime = LandingTime;
            this.PlaneID = PlaneID;
            this.Crew = Crew;
            this.Load = PassCarg;
            this.Longitute = 0;
            this.Latitude = 0;
            this.AMSL = 0;
        }
        public Flight() : base()
        {
            Value = "FL";
            this.OriginID = 0;
            this.TargetID = 1;
            this.TakeoffTime = DateTime.Now;
            this.LandingTime = DateTime.Now.AddMinutes(60);
            this.PlaneID = 10;
            this.Crew = [];
            this.Load = [];
            this.Longitute = 0;
            this.Latitude = 0;
            this.AMSL = 0;
        }

    }


}
