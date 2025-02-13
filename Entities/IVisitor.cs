using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkSourceSimulator;


namespace ProjOb
{
    public interface IVisitor
    {
        public string VisitPassengerPlane(PassengerPlane passengerPlane);
        public string VisitCargoPlane(CargoPlane cargoPlane);
        public string VisitAirport(Airport airport);

    }
}
