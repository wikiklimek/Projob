using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Mapsui.Utilities;
using NetworkSourceSimulator;
using Tmds.DBus.Protocol;

namespace ProjOb
{
    public class DecoratorUpdateLists
    {
        private readonly DecoratorUpdateFlightGUIListConverter _decoratorflGUIlc;
        private readonly Lists _lists;
        private readonly Dictionary<UInt64, Object1> _listObjects;
        private readonly Dictionary<UInt64, Plane> _listPlane;
        private readonly Dictionary<UInt64, Crew> _listCrew;
        private readonly Dictionary<UInt64, Passenger> _listPassenger;
        private readonly Dictionary<UInt64, Cargo> _listCargo;
        private readonly Dictionary<UInt64, CargoPlane> _listCargoPlane;
        private readonly Dictionary<UInt64, PassengerPlane> _listPassengerPlane;
        private readonly Dictionary<UInt64, Airport> _listAirport;
        private readonly  Dictionary<UInt64, Flight> _listFlight;
        private readonly Logs _log;

        private readonly UpdatingDataDictionary _updatingDataDictionary;
        public DecoratorUpdateLists(Lists lists, Dictionary<UInt64, Object1> listObjects, Dictionary<UInt64, Plane> listPlane, Dictionary<UInt64, Crew> listCrew,
            Dictionary<UInt64, Passenger> listPassenger, Dictionary<UInt64, Cargo> listCargo,
            Dictionary<UInt64, CargoPlane> listCargoPlane, Dictionary<UInt64, PassengerPlane> listPassengerPlane,
            Dictionary<UInt64, Airport> listAirport, Dictionary<UInt64, Flight> listFlight, DecoratorUpdateFlightGUIListConverter decoratorflGUIlc)
        {
            _listObjects = listObjects;
            _listPlane = listPlane;
            _listCrew = listCrew;
            _listPassenger = listPassenger;
            _listCargo = listCargo;
            _listAirport = listAirport;
            _listCargoPlane = listCargoPlane;
            _listPassengerPlane = listPassengerPlane;
            _listFlight = listFlight;
            _lists = lists;
            _updatingDataDictionary = new(this);
            _log = new(DateTime.Now);
            _decoratorflGUIlc = decoratorflGUIlc;
        }

        public void PositionUpdate(object sender, PositionUpdateArgs args)
        {
            string? str;
            if ((str = IfHasObject(args.ObjectID)) != null)
                _updatingDataDictionary.UpdatePosition(str, args, _decoratorflGUIlc);
            else
            {
                _log.LogMessageUpdatePosition(new PositionUpdateArgs(), args, false);
                _log.LogMessage("Error: There is no such a key");
            }
        }

        public void ContactInfoUpdate(object sender, ContactInfoUpdateArgs args)
        {
            string? str;
            if ((str = IfHasObject(args.ObjectID)) != null)
                _updatingDataDictionary.UpdateContactInfo(str, args);
            else
            {
                _log.LogMessageUpdateContactInfo(new ContactInfoUpdateArgs(), args, false);
                _log.LogMessage("Error: There is no such a key");
            }
        }
        public void IDUpdate(object sender, IDUpdateArgs args)
        {
            string? str;
            if ((str = IfHasObject(args.ObjectID)) != null)
            {
                if (IfHasObject(args.NewObjectID) == null)
                {
                    _updatingDataDictionary.UpdateID(str, args.ObjectID, args.NewObjectID, _decoratorflGUIlc);
                    _log.LogMessageUpdateID(args.ObjectID, args.NewObjectID, true);
                }
                else
                {
                    _log.LogMessageUpdateID(args.ObjectID, args.NewObjectID, false);
                    _log.LogMessage("Error: The key already exists");
                }
            }
            else
            {
                _log.LogMessageUpdateID(args.ObjectID, args.NewObjectID, false);
                _log.LogMessage("Error: There is no such a key");
            }
        }

        public void ChangePositionFlight(PositionUpdateArgs args, DecoratorUpdateFlightGUIListConverter dec)
        {
            (bool changed, PositionUpdateArgs oldDataGUI) = dec.UpdatePosition(args, args.ObjectID);

            PositionUpdateArgs oldData = new()
            {
                AMSL = _listFlight[args.ObjectID].AMSL,
                Latitude = changed ? oldDataGUI.Latitude : _listFlight[args.ObjectID].Latitude,
                Longitude = changed ? oldDataGUI.Longitude : _listFlight[args.ObjectID].Longitute,
                ObjectID = args.ObjectID
            };  

            _listFlight[args.ObjectID].AMSL = args.AMSL;
            _listFlight[args.ObjectID].Latitude = args.Latitude;
            _listFlight[args.ObjectID].Longitute = args.Longitude;

            _log.LogMessageUpdatePosition(oldData, args, true);
        }
        public void ChangePositionAirport(PositionUpdateArgs args, DecoratorUpdateFlightGUIListConverter dec)
        {
            PositionUpdateArgs oldData = new()
            {
                AMSL = _listAirport[args.ObjectID].AMSL,
                Latitude = _listAirport[args.ObjectID].Latitude,
                Longitude = _listAirport[args.ObjectID].Longitude,
                ObjectID = args.ObjectID
            };

            _listAirport[args.ObjectID].AMSL = args.AMSL;
            _listAirport[args.ObjectID].Latitude = args.Latitude;
            _listAirport[args.ObjectID].Longitude = args.Longitude;

            _log.LogMessageUpdatePosition(oldData, args, true);
        }
        public void ChangePositionPlane(PositionUpdateArgs args, DecoratorUpdateFlightGUIListConverter dec)
        {
            foreach (var flight in _listFlight)
                if (flight.Value.PlaneID == args.ObjectID)
                {
                    (bool changed, PositionUpdateArgs oldData) = dec.UpdatePosition(args, flight.Key);
                    _log.LogMessageUpdatePosition(oldData, args, changed);
                }
        }
        private ContactInfoUpdateArgs MakeContactInfoUpdateArgs(Person person)
        {
            return new ContactInfoUpdateArgs()
            {
                ObjectID = person.ID,
                EmailAddress = person.Email,
                PhoneNumber = person.Phone
            };
        }

        public void ChangeContactInfoCrew(ContactInfoUpdateArgs args)
        {
            ContactInfoUpdateArgs oldData = MakeContactInfoUpdateArgs(_listCrew[args.ObjectID]);

            _listCrew[args.ObjectID].Email = args.EmailAddress;
            _listCrew[args.ObjectID].Phone = args.PhoneNumber;

            _log.LogMessageUpdateContactInfo(oldData, args, true);
        }
        public void ChangeContactInfoPassenger(ContactInfoUpdateArgs args)
        {
            ContactInfoUpdateArgs oldData = MakeContactInfoUpdateArgs(_listCrew[args.ObjectID]);

            _listPassenger[args.ObjectID].Email = args.EmailAddress;
            _listPassenger[args.ObjectID].Phone = args.PhoneNumber;

            _log.LogMessageUpdateContactInfo(oldData, args, true);
        }
        public string? IfHasObject(ulong ID) => _listObjects.TryGetValue(ID, out Object1? object1) ? object1.Value : null;
        
        public void ChangeIDCrew(ulong oldID, ulong newID)
        {
            Crew object1 = _listCrew[oldID];
            _lists.RemoveCrew = object1;
            object1.ID = newID;
            _lists.Crew = object1;
        }

        public void ChangeIDPassenger(ulong oldID, ulong newID)
        {
            Passenger object1 = _listPassenger[oldID];
            _lists.RemovePassenger = object1;
            object1.ID = newID;
            _lists.Passenger = object1;
        }
        public void ChangeIDCargo(ulong oldID, ulong newID)
        {
            Cargo object1 = _listCargo[oldID];
            _lists.RemoveCargo = object1;
            object1.ID = newID;
            _lists.Cargo = object1;
        }
        public void ChangeIDPassengerPlane(ulong oldID, ulong newID)
        {
            PassengerPlane object1 = _listPassengerPlane[oldID];
            _lists.RemovePassengerPlane = object1;
            object1.ID = newID;

            foreach (var flight in _listFlight)
                if (flight.Value.PlaneID == oldID)
                    flight.Value.PlaneID = newID;

            _lists.PassengerPlane = object1;
        }
        public void ChangeIDCargoPlane(ulong oldID, ulong newID)
        {
            CargoPlane object1 = _listCargoPlane[oldID];
            _lists.RemoveCargoPlane = object1;
            object1.ID = newID;

            foreach (var flight in _listFlight)
                if (flight.Value.PlaneID == oldID)
                    flight.Value.PlaneID = newID;

            _lists.CargoPlane = object1;
        }
        public void ChangeIDAirport(ulong oldID, ulong newID)
        {
            Airport object1 = _listAirport[oldID];
            _lists.RemoveAirport = object1;

            object1.ID = newID;

            foreach (var flight in _listFlight)
                if (flight.Value.OriginID == oldID)
                    flight.Value.OriginID = newID;
                else if (flight.Value.TargetID == oldID)
                    flight.Value.TargetID = newID;

            _lists.Airport = object1;
        }
        public void ChangeIDFlight(ulong oldID, ulong newID)
        {
            Flight object1 = _listFlight[oldID];
            _lists.RemoveFlight = object1;

            object1.ID = newID;
            _lists.Flight = object1;
        }

    }




}
