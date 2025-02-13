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
    [JsonDerivedType(typeof(Cargo), typeDiscriminator: "CA")]
    public class Cargo : Object1
    {
        public override string? Value { get; set; }
        public Single Weight { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public Cargo(ulong ID, Single Weight, string Code, string Description)
            : base(ID)
        {
            this.Weight = Weight;
            this.Code = Code;
            this.Description = Description;
            this.Value = "CA";
        }

        public Cargo() : base()
        {
            this.Weight = 0;
            this.Code = "default";
            this.Description = "default";
            this.Value = "CA";
        }
    }

}
