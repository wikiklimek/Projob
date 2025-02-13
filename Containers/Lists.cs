using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.Text.Json;
using DynamicData;
using System.Collections;
using NetworkSourceSimulator;

namespace ProjOb
{
    public class Lists
    {
        private readonly Dictionary<UInt64, Object1> _listObjects;
        private readonly Dictionary<UInt64, Plane> _listPlane;
        private readonly Dictionary<UInt64, Crew> _listCrew;
        private readonly Dictionary<UInt64, Passenger> _listPassenger;
        private readonly Dictionary<UInt64, Cargo> _listCargo;
        private readonly Dictionary<UInt64, CargoPlane> _listCargoPlane;
        private readonly Dictionary<UInt64, PassengerPlane> _listPassengerPlane;
        private readonly Dictionary<UInt64, Airport> _listAirport;
        private readonly Dictionary<UInt64, Flight> _listFlight;
        private readonly List<IReportable> _listReportables;

        private (Object1 Obj, UInt64 ID) Object { set { _listObjects.Add(value.ID, value.Obj); } }
        private IReportable Reporables { set { _listReportables.Add(value); } }
        private Plane Plane { set { _listPlane.Add(value.ID, value); Object = (value, value.ID); Reporables = value; } }
        public Crew Crew { set { _listCrew.Add(value.ID, value); Object = (value, value.ID); } }
        public Passenger Passenger { set { _listPassenger.Add(value.ID, value); Object = (value, value.ID); } }
        public Cargo Cargo { set { _listCargo.Add(value.ID, value); Object = (value, value.ID); } }
        public CargoPlane CargoPlane { set { _listCargoPlane.Add(value.ID, value); Plane = value; } }
        public PassengerPlane PassengerPlane { set { _listPassengerPlane.Add(value.ID, value); Plane = value; } }
        public Airport Airport { set { _listAirport.Add(value.ID, value); Object = (value, value.ID); Reporables = value; } }
        public Flight Flight { set { _listFlight.Add(value.ID, value); Object = (value, value.ID); } }

        private Object1 RemoveObj { set { _listObjects.Remove(value.ID); } }
        private IReportable RemoveIReportable { set { _listReportables.Remove(value); } }
        private Plane RemovePlane
        {
            set
            {
                _listPlane.Remove(value.ID); RemoveObj = value;
                foreach (var pair in _listFlight.ToList())
                    if (pair.Value.PlaneID == value.ID)
                        pair.Value.Plane = null;
            }
        }
        public Crew RemoveCrew { set { _listCrew.Remove(value.ID); RemoveObj = value; } }
        public Passenger RemovePassenger { set { _listPassenger.Remove(value.ID); RemoveObj = value; } }
        public Cargo RemoveCargo { set { _listCargo.Remove(value.ID); RemoveObj = value; } }
        public CargoPlane RemoveCargoPlane { set { _listCargoPlane.Remove(value.ID); RemovePlane = value; RemoveIReportable = value; } }
        public PassengerPlane RemovePassengerPlane { set { _listPassengerPlane.Remove(value.ID); RemovePlane = value; RemoveIReportable = value; } }
        public Airport RemoveAirport
        {
            set
            {
                _listAirport.Remove(value.ID); RemoveObj = value; RemoveIReportable = value;
                foreach (var pair in _listFlight.ToList())
                    if (pair.Value.TargetID == value.ID)
                        pair.Value.Target = null;
                    else if (pair.Value.OriginID == value.ID)
                        pair.Value.Origin = null;

            }
        }
        public Flight RemoveFlight { set { _listFlight.Remove(value.ID); RemoveObj = value; } }

        public Lists()
        {
            _listObjects = new();
            _listPlane = new();
            _listCrew = new();
            _listPassenger = new();
            _listCargo = new();
            _listCargoPlane = new();
            _listPassengerPlane = new();
            _listAirport = new();
            _listFlight = new();
            _listReportables = new();
        }

        public bool ContainsID(ulong ID) => _listObjects.ContainsKey(ID);
        public Airport? GetByIDAirport(ulong ID) => _listAirport[ID];
        public Plane? GetByIDPlane(ulong ID) => _listPlane[ID];


        public List<FlightGUIAdapter> FlightToGUI(DateTime NowTime, FlightGUIListConverter flightGUIListConverter)
        {
            flightGUIListConverter.AddAndConvertNew(_listFlight, NowTime);
            return flightGUIListConverter.GetNewFlightGUI;
        }
        public void SerializeJson(string path)
        {
            using (StreamWriter sr = new StreamWriter(path))
                foreach (var obj in _listObjects.ToList())
                    sr.WriteLine(JsonSerializer.Serialize<Object1>(obj.Value));

        }
        public NewsGenerator FromIReportablesToGenerator(List<Media> _media) => new NewsGenerator(_media, _listReportables);

        public DecoratorUpdateLists MakeDecoratorUpdateObject(DecoratorUpdateFlightGUIListConverter decoratorflGUIlc) => new DecoratorUpdateLists(this,

            _listObjects, _listPlane, _listCrew, _listPassenger, _listCargo, _listCargoPlane, _listPassengerPlane,
            _listAirport, _listFlight, decoratorflGUIlc);

        public DecoratorTransactionLists MakeDecoratorTransaction(DecoratorUpdateFlightGUIListConverter decoratorflGUIlc, TransactionDictionaries? transactionDictionaries, Transaction transaction) =>
            new DecoratorTransactionLists(this,
            _listObjects, _listCrew, _listPassenger, _listCargo, _listCargoPlane, _listPassengerPlane,
            _listAirport, _listFlight, decoratorflGUIlc, transactionDictionaries, transaction);


    }


}

