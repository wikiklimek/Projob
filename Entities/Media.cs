using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkSourceSimulator;

namespace ProjOb
{
    abstract public class Media : IVisitor
    {
        public string Name { get; set; }
        public Media(string name)
        {
            Name = name;
        }
        abstract public string VisitPassengerPlane(PassengerPlane passengerPlane);
        abstract public string VisitCargoPlane(CargoPlane cargoPlane);
        abstract public string VisitAirport(Airport airport);
    }

    public class Television : Media
    {
        public Television(string name) : base(name) { }
        override public string VisitPassengerPlane(PassengerPlane passengerPlane)
        {
            return $"<An image of {passengerPlane.Serial} passenger plane>";
        }
        override public string VisitCargoPlane(CargoPlane cargoPlane)
        {
            return $"<An image of {cargoPlane.Serial} cargo plane>";
        }
        override public string VisitAirport(Airport airport)
        {
            return $"<An image of {airport.Name} airport>";
        }

    }

    public class Radio : Media
    {
        public Radio(string name) : base(name) { }
        override public string VisitPassengerPlane(PassengerPlane passengerPlane)
        {
            return $"Reporting for {this.Name}, " +
                "Ladies and gentelmen, we’ve " +
                "just witnessed " +
                $"{passengerPlane.Serial} " +
                "take off.";
        }
        override public string VisitCargoPlane(CargoPlane cargoPlane)
        {
            return $"Reporting for {this.Name}, " +
                "Ladies and gentelmen, we are " +
                "seeing the " +
                $"{cargoPlane.Serial} " +
                "aircraft fly above us.";
        }
        override public string VisitAirport(Airport airport)
        {
            return $"Reporting for {this.Name}," +
                $"Ladies and gentelmen, we are " +
                $"at the {airport.Name} airport.";
        }

    }

    public class Newspaper : Media
    {
        public Newspaper(string name) : base(name) { }
        override public string VisitPassengerPlane(PassengerPlane passengerPlane)
        {
            return $"{this.Name} - " +
                $"Breaking news! {passengerPlane.Model} " +
                "aircraft loses EASA fails " +
                "certification after inspection of " +
                $"{passengerPlane.Serial}.";
        }
        override public string VisitCargoPlane(CargoPlane cargoPlane)
        {
            return $"{this.Name} - An " +
                "interview with the crew of " +
                $"{cargoPlane.Serial}.";
        }
        override public string VisitAirport(Airport airport)
        {
            return $"{this.Name} - A " +
                "report from the " +
                $"{airport.Name} airport, " +
                $"{airport.ISO}.";
        }

    }

}
