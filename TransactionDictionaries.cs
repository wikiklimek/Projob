using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BruTile.Wmts.Generated;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ProjOb
{

    public enum DisplayType
    {
        VariableName = 0,
        VariableValue = 1,
        Lines = 2
    }

    public class TransactionDictionaries
    {
        private readonly DecoratorTransactionLists _decoratorTransactionLists;
        private readonly DecoratorUpdateFlightGUIListConverter _decoratorUpdateFlightGUIListConverter;
        public Dictionary<string, Action> transactions;
        public Dictionary<string, Action<string[]>> displays;
        public Dictionary<string, Action<string[]>> updates;
        public Dictionary<string, Action<string[]>> deletes;
        public Dictionary<string, Action<string[]>> adds;
        public Dictionary<string, Func<float, float, bool>> precicateFloats;
        public Dictionary<string, Func<uint, uint, bool>> precicateUints;
        public Dictionary<string, Func<DateTime, DateTime, bool>> precicateDate;
        public Dictionary<string, Func<string, string, bool>> precicateString;

        //crew
        public Dictionary<string, uint> crewDictionary;
        public Dictionary<string, Func<Crew, string>> crewDictionaryDisplay;
        public Dictionary<string, Action<Crew, string[]>> crewDictionaryUpdate;
        public Dictionary<string, Func<Crew, string[], bool>> crewDictionaryPredicates;

        //passenger
        public Dictionary<string, uint> passengerDictionary;
        public Dictionary<string, Func<Passenger, string>> passengerDictionaryDisplay;
        public Dictionary<string, Action<Passenger, string[]>> passengerDictionaryUpdate;
        public Dictionary<string, Func<Passenger, string[], bool>> passengerDictionaryPredicates;

        //cargo
        public Dictionary<string, uint> cargoDictionary;
        public Dictionary<string, Func<Cargo, string>> cargoDictionaryDisplay;
        public Dictionary<string, Action<Cargo, string[]>> cargoDictionaryUpdate;
        public Dictionary<string, Func<Cargo, string[], bool>> cargoDictionaryPredicates;

        //airport
        public Dictionary<string, uint> airportDictionary;
        public Dictionary<string, Func<Airport, string>> airportDictionaryDisplay;
        public Dictionary<string, Action<Airport, string[]>> airportDictionaryUpdate;
        public Dictionary<string, Func<Airport, string[], bool>> airportDictionaryPredicates;

        //Plane
        public Dictionary<string, Func<Plane, string[], bool>> planeDictionaryPredicates;
        public Dictionary<string, Action<Plane, string>> planeDictionaryUpdate;

        //passengerPlane
        public Dictionary<string, uint> passengerPlaneDictionary;
        public Dictionary<string, Func<PassengerPlane, string>> passengerPlaneDictionaryDisplay;
        public Dictionary<string, Action<PassengerPlane, string[]>> passengerPlaneDictionaryUpdate;
        public Dictionary<string, Func<PassengerPlane, string[], bool>> passengerPlaneDictionaryPredicates;

        //cargoPlane
        public Dictionary<string, uint> cargoPlaneDictionary;
        public Dictionary<string, Func<CargoPlane, string>> cargoPlaneDictionaryDisplay;
        public Dictionary<string, Action<CargoPlane, string[]>> cargoPlaneDictionaryUpdate;
        public Dictionary<string, Func<CargoPlane, string[], bool>> cargoPlaneDictionaryPredicates;

        //flights
        public Dictionary<string, uint> flightsDictionary;
        public Dictionary<string, Func<Flight, string>> flightsDictionaryDisplay;
        public Dictionary<string, Action<Flight, string[]>> flightsDictionaryUpdate;
        public Dictionary<string, Func<Flight, string[], bool>> flightsDictionaryPredicates;

        //WorldPosition
        public Dictionary<string, Action<Flight, string>> worldPositionFlightsDictionaryUpdate;
        public Dictionary<string, Action<Airport, string>> worldPositionAirportDictionaryUpdate;
        public Dictionary<string, Func<WorldPosition, string[], bool>> worldPositionDictionaryPredicates;


        public Dictionary<string, Func<string, bool>> _dictionaryStructCorrectness;
        public Dictionary<string, int> numberDictionaryDisplay;

        public TransactionDictionaries(DecoratorTransactionLists decoratorTransactionLists, Dictionary<string, Action> transactions, DecoratorUpdateFlightGUIListConverter decoratorUpdateFlightGUIListConverter)
        {
            _decoratorTransactionLists = decoratorTransactionLists;
            this.transactions = transactions;
            _decoratorUpdateFlightGUIListConverter = decoratorUpdateFlightGUIListConverter;


            displays = NewDictionaryDisplays();
            deletes = NewDictionaryDeletes();
            updates = NewDictionaryUpdates();
            adds = NewDictionaryAdds();

            //Predicates
            {
                precicateFloats = new()
                {
                    {"<", (float a, float b) =>  a < b  },
                    {">", (float a, float b) =>  a > b },
                    {"=", (float a, float b) =>  a == b },
                    {"!=", (float a, float b) =>  a != b },
                    {">=", (float a, float b) =>  a >= b },
                    {"<=", (float a, float b) =>  a <= b }
                };

                precicateUints = new()
                {
                    {"<", (uint a, uint b) =>  a < b  },
                    {">", (uint a, uint b) =>  a > b },
                    {"=", (uint a, uint b) =>  a == b },
                    {"!=", (uint a, uint b) =>  a != b },
                    {">=", (uint a, uint b) =>  a >= b },
                    {"<=", (uint a, uint b) =>  a <= b }
                };

                precicateDate = new()
                {
                    {"<", (DateTime a, DateTime b) =>  a < b  },
                    {">", (DateTime a, DateTime b) =>  a > b },
                    {"=", (DateTime a, DateTime b) =>  a == b },
                    {"!=", (DateTime a, DateTime b) =>  a != b },
                    {">=", (DateTime a, DateTime b) =>  a >= b },
                    {"<=", (DateTime a, DateTime b) =>  a <= b }
                };

                precicateString = new()
                {
                    {"=", (string a, string b) =>  a == b },
                    {"!=", (string a, string b) =>  a != b }
                };
            }

            //WorldPosition
            {
                worldPositionFlightsDictionaryUpdate = new()
                    {
                         { "Lat", (Flight fl, string str) =>
                            {
                                fl.Latitude = float.Parse(str);
                                _decoratorUpdateFlightGUIListConverter.UpdateLat(fl.ID, float.Parse(str));
                            }
                         },

                         { "Long", (Flight fl, string str) =>
                            {
                                fl.Longitute = float.Parse(str);
                                _decoratorUpdateFlightGUIListConverter.UpdateLong(fl.ID, float.Parse(str));
                            }
                         }
                    };
                worldPositionAirportDictionaryUpdate = new()
                {
                     { "Lat", (Airport a, string str) => { a.Latitude = float.Parse(str); } },
                     { "Long", (Airport a, string str) => { a.Longitude = float.Parse(str); } }
                };
                worldPositionDictionaryPredicates = new()
                {
                     { "Lat", (WorldPosition w, string[] array) => { return Floats((float)(w.Latitude), float.Parse(array[1]), array[0]); } },
                     { "Long", (WorldPosition w, string[] array) => { return Floats((float)(w.Longitude), float.Parse(array[1]), array[0]); } }
                };

            }

            {

                numberDictionaryDisplay = new()
                {
                    { "ID",               8 },
                    { "Origin",           8},
                    { "Target",           8 },
                    { "TakeOffTime",      22},
                    { "LandingTime",      22},
                    { "WorldPosition",    25 },
                    { "AMSL",             12},
                    { "Plane",            8},
                    { "Serial",           8},
                    { "CountryCode",      15},
                    { "Model",            23},
                    { "MaxLoad",          11},
                    { "FirstClassSize",   20},
                    { "BusinessClassSize",20},
                    { "EconomyClassSize", 20},
                    { "Name",             28},
                    { "Code",             8},
                    { "Country",          10},
                    { "Weight",           8},
                    { "Description",      25},
                    { "Practise",         10},
                    { "Role",             8},
                    { "Age",              5},
                    { "Phone",            15},
                    { "Email",            40},
                    { "Class",            7},
                    { "Miles",            8}

                };
            }


            //Crew
            {
                crewDictionary = new()
                {
                    { "ID", 0 },
                    { "Name", 1 },
                    { "Age", 2 },
                    { "Phone", 3 },
                    { "Email", 4 },
                    { "Practise", 5 },
                    { "Role", 6 }
                };
                crewDictionaryDisplay = new()
                {
                    {"ID", (Crew crew) => crew.ID.ToString() },
                    {"Name", (Crew crew) => crew.Name.ToString() },
                    {"Age", (Crew crew) => crew.Age.ToString() },
                    {"Phone", (Crew crew) => crew.Phone.ToString() },
                    {"Email", (Crew crew) => crew.Email.ToString() },
                    {"Practise", (Crew crew) => crew.Practice.ToString() },
                    {"Role", (Crew crew) => crew.Role.ToString() }
                };
                crewDictionaryPredicates = new()
                {
                    //dwa elementy w string -> znak operacji oraz wartość
                    { "ID", (Crew crew, string[] array) => { return Uints((uint)crew.ID, uint.Parse(array[1]), array[0]); } },
                    { "Name", (Crew crew, string[] array) => { return Strings(crew.Name, array[1], array[0]); } },
                    { "Age", (Crew crew, string[] array) => { return Uints(crew.Age, uint.Parse(array[1]), array[0]); } },
                    { "Phone", (Crew crew, string[] array) => { return Strings(crew.Phone, array[1], array[0]); } },
                    { "Email", (Crew crew, string[] array) => { return Strings(crew.Email, array[1], array[0]); } },
                    { "Practise", (Crew crew, string[] array) => { return Uints(crew.Practice, uint.Parse(array[1]), array[0]); } },
                    { "Role", (Crew crew, string[] array) => { return Strings(crew.Role, array[1], array[0]); } }
                };

                crewDictionaryUpdate = new()
                {
                    { "ID", (Crew crew, string[] str) =>  _decoratorTransactionLists.decoratorUpdateLists.ChangeIDCrew(crew.ID, ulong.Parse(str[0])) },
                    { "Name", (Crew crew, string[] str) => { crew.Name = str[0]; } },
                    { "Age", (Crew crew, string[] str) => { crew.Age = ushort.Parse(str[0]); } },
                    { "Phone", (Crew crew, string[] str) => { crew.Phone = str[0]; } },
                    { "Email", (Crew crew, string[] str) => { crew.Email = str[0]; } },
                    { "Practise", (Crew crew, string[] str) => { crew.Practice = ushort.Parse(str[0]); } },
                    { "Role", (Crew crew, string[] str) => { crew.Role = str[0]; } }
                };
            }

            //Passenger
            {
                passengerDictionary = new()
                {
                    { "ID", 0 },
                    { "Name", 1 },
                    { "Age", 2 },
                    { "Phone", 3 },
                    { "Email", 4 },
                    { "Class", 5 },
                    { "Miles", 6 }
                };
                passengerDictionaryDisplay = new()
                {
                    {"ID", (Passenger p) => p.ID.ToString() },
                    {"Name", (Passenger p) => p.Name.ToString() },
                    {"Age", (Passenger p) => p.Age.ToString() },
                    {"Phone", (Passenger p) => p.Phone.ToString() },
                    {"Email", (Passenger p) => p.Email.ToString() },
                    {"Class", (Passenger p) => p.Class.ToString() },
                    {"Miles", (Passenger p) => p.Miles.ToString() }
                };
                passengerDictionaryPredicates = new()
                {
                    //dwa elementy w string -> znak operacji oraz wartość
                    { "ID", (Passenger p, string[] array) => { return Uints((uint)p.ID, uint.Parse(array[1]), array[0]); } },
                    { "Name", (Passenger p, string[] array) => { return Strings(p.Name, array[1], array[0]); } },
                    { "Age", (Passenger p, string[] array) => { return Uints(p.Age, uint.Parse(array[1]), array[0]); } },
                    { "Phone", (Passenger p, string[] array) => { return Strings(p.Phone, array[1], array[0]); } },
                    { "Email", (Passenger p, string[] array) => { return Strings(p.Email, array[1], array[0]); } },
                    { "Class", (Passenger p, string[] array) => { return Strings(p.Class, array[1], array[0]); } },
                    { "Miles", (Passenger p, string[] array) => { return Uints((uint)p.Miles, uint.Parse(array[1]), array[0]); } }
                };

                passengerDictionaryUpdate = new()
                {
                    { "ID", (Passenger p, string[] str) =>  _decoratorTransactionLists.decoratorUpdateLists.ChangeIDPassenger(p.ID, ulong.Parse(str[0])) },
                    { "Name", (Passenger p, string[] str) => { p.Name = str[0]; } },
                    { "Age", (Passenger p, string[] str) => {p.Age = ushort.Parse(str[0]); } },
                    { "Phone", (Passenger p, string[] str) => {  p.Phone = str[0]; } },
                    { "Email", (Passenger p, string[] str) => { p.Email = str[0]; } },
                    { "Class", (Passenger p, string[] str) => { p.Class = str[0]; } },
                    { "Miles", (Passenger p, string[] str) => { p.Miles = ulong.Parse(str[0]); } }
                };
            }


            //Cargo
            {
                cargoDictionary = new()
                {
                    { "ID", 0 },
                    { "Weight", 1 },
                    { "Code", 2 },
                    { "Description", 3 }
                };
                cargoDictionaryDisplay = new()
                {
                    {"ID", (Cargo ca) => ca.ID.ToString() },
                    {"Weight", (Cargo ca) => ca.Weight.ToString() },
                    {"Code", (Cargo ca) => ca.Code.ToString() },
                    {"Description", (Cargo ca) => ca.Description.ToString() }
                };
                cargoDictionaryPredicates = new()
                {
                    //dwa elementy w string -> znak operacji oraz wartość
                    { "ID", (Cargo ca, string[] array) => { return Uints((uint)ca.ID, uint.Parse(array[1]), array[0]); } },
                    { "Weight", (Cargo ca, string[] array) => { return Floats(ca.Weight, float.Parse(array[1]), array[0]); } },
                    { "Code", (Cargo ca, string[] array) => { return Strings(ca.Code, array[1], array[0]); } },
                    { "Description", (Cargo ca, string[] array) => { return Strings(ca.Description, array[1], array[0]); } }
                };
                cargoDictionaryUpdate = new()
                {
                    { "ID", (Cargo ca, string[] str) => _decoratorTransactionLists.decoratorUpdateLists.ChangeIDCargo(ca.ID, ulong.Parse(str[0]))},
                    { "Weight", (Cargo ca, string[] str) =>{ca.Weight = float.Parse(str[0]); } },
                    { "Code", (Cargo ca, string[] str) =>{ ca.Code = str[0]; } },
                    { "Description", (Cargo ca, string[] str) =>{ ca.Description = str[0]; } }
                };
            }

            //Airport
            {
                airportDictionary = new()
                {
                    { "ID", 0 },
                    { "Name", 1 },
                    { "Code", 2 },
                    { "WorldPosition", 3 },
                    { "AMSL", 4 },
                    { "Country", 5 }
                };
                airportDictionaryDisplay = new()
                {
                    {"ID", (Airport a) => a.ID.ToString()},
                    {"Name", (Airport a) => a.Name.ToString()},
                    {"Code", (Airport a) => a.Code.ToString()},
                    {"WorldPosition", (Airport a) => "{"+ $"{a.Longitude}, {a.Latitude}" +"}" },
                    {"AMSL", (Airport a) => a.AMSL.ToString() },
                    {"Country", (Airport a) => a.ISO.ToString() }
                };
                airportDictionaryPredicates = new()
                {
                    //dwa elementy w string -> znak operacji oraz wartość
                    { "ID", (Airport a, string[] array) => { return Uints((uint)a.ID, uint.Parse(array[1]), array[0]); } },
                    { "Name", (Airport a, string[] array) => { return Strings(a.Name, array[1], array[0]); } },
                    { "Code", (Airport a, string[] array) => { return Strings(a.Code, array[1], array[0]); } },
                    { "WorldPosition", (Airport a, string[] array) => worldPositionDictionaryPredicates[array[2]].Invoke(new WorldPosition(a.Latitude, a.Longitude), array)},
                    { "AMSL", (Airport a, string[] array) => { return Floats(a.AMSL, float.Parse(array[1]), array[0]); } },
                    { "Country", (Airport a, string[] array) => { return Strings(a.ISO, array[1], array[0]); } }
                };
                airportDictionaryUpdate = new()
                {
                    { "ID", (Airport a, string[] str) => _decoratorTransactionLists.decoratorUpdateLists.ChangeIDAirport(a.ID, ulong.Parse(str[0])) },
                    { "Name", (Airport a, string[] str) =>{ a.Name = str[0]; } },
                    { "Code", (Airport a, string[] str) =>{ a.Code = str[0]; } },
                    { "WorldPosition",(Airport a, string[] array) => worldPositionAirportDictionaryUpdate[array[1]].Invoke(a, array[0])},
                    { "AMSL", (Airport a, string[] str) =>{ a.AMSL = float.Parse(str[0]); } },
                    { "Country", (Airport a, string[] str) =>{ a.ISO = str[0]; } }
                };
            }

            //Plane
            {
                planeDictionaryPredicates = new()
                {
                    //dwa elementy w string -> znak operacji oraz wartość
                    { "ID", (Plane cp, string[] array) => { return Uints((uint)cp.ID, uint.Parse(array[1]), array[0]); } },
                    { "Serial", (Plane cp, string[] array) => { return Strings(cp.Serial, array[1], array[0]); } },
                    { "CountryCode", (Plane cp, string[] array) => { return Strings(cp.Country, array[1], array[0]); } },
                    { "Model", (Plane cp, string[] array) => { return Strings(cp.Model, array[1], array[0]); } }
                };

                planeDictionaryUpdate = new()
                {
                    { "ID", (Plane pp, string str) =>  pp.ID = ulong.Parse(str) },
                    { "Serial", (Plane pp, string str) =>{ pp.Serial = str; } },
                    { "CountryCode", (Plane pp, string str) =>{ pp.Country = str; } },
                    { "Model", (Plane pp, string str) =>{pp.Model = str; } }
                };
            }

            //PassengerPlane 
            {
                passengerPlaneDictionary = new()
                {
                    { "ID", 0 },
                    { "Serial", 1 },
                    { "CountryCode", 2 },
                    { "Model", 3 },
                    { "FirstClassSize", 4 },
                    { "BusinessClassSize", 5 },
                    { "EconomyClassSize", 6 }
                };
                passengerPlaneDictionaryDisplay = new()
                {
                    {"ID", (PassengerPlane pp) => pp.ID.ToString() },
                    {"Serial", (PassengerPlane pp) => pp.Serial.ToString()},
                    {"CountryCode", (PassengerPlane pp) => pp.Country.ToString() },
                    {"Model", (PassengerPlane pp) => pp.Model.ToString()},
                    {"FirstClassSize", (PassengerPlane pp) => pp.FirstClassSize.ToString() },
                    {"BusinessClassSize", (PassengerPlane pp) => pp.BusinessClassSize.ToString() },
                    {"EconomyClassSize", (PassengerPlane pp) => pp.EconomyClassSize.ToString() }
                };
                passengerPlaneDictionaryPredicates = new()
                {
                    //dwa elementy w string -> znak operacji oraz wartość
                    { "ID", (PassengerPlane pp, string[] array) => { return Uints((uint)pp.ID, uint.Parse(array[1]), array[0]); } },
                    { "Serial", (PassengerPlane pp, string[] array) => { return Strings(pp.Serial, array[1], array[0]); } },
                    { "CountryCode", (PassengerPlane pp, string[] array) => { return Strings(pp.Country, array[1], array[0]); } },
                    { "Model", (PassengerPlane pp, string[] array) => { return Strings(pp.Model, array[1], array[0]); } },
                    { "FirstClassSize", (PassengerPlane pp, string[] array) => { return Uints(pp.FirstClassSize, uint.Parse(array[1]), array[0]); } },
                    { "BusinessClassSize", (PassengerPlane pp, string[] array) => { return Uints(pp.BusinessClassSize, uint.Parse(array[1]), array[0]); } },
                    { "EconomyClassSize", (PassengerPlane pp, string[] array) => { return Uints(pp.EconomyClassSize, uint.Parse(array[1]), array[0]); } }
                };
                passengerPlaneDictionaryUpdate = new()
                {
                    { "ID", (PassengerPlane pp, string[] str) => _decoratorTransactionLists.decoratorUpdateLists.ChangeIDPassengerPlane(pp.ID, ulong.Parse(str[0]))},
                    { "Serial", (PassengerPlane pp, string[] str) =>{ pp.Serial = str[0]; } },
                    { "CountryCode", (PassengerPlane pp, string[] str) =>{ pp.Country = str[0]; } },
                    { "Model", (PassengerPlane pp, string[] str) =>{pp.Model = str[0]; } },
                    { "FirstClassSize", (PassengerPlane pp, string[] str) =>{pp.FirstClassSize = ushort.Parse(str[0]); } },
                    { "BusinessClassSize", (PassengerPlane pp, string[] str) =>{pp.BusinessClassSize = ushort.Parse(str[0]); } },
                    { "EconomyClassSize", (PassengerPlane pp, string[] str) =>{pp.EconomyClassSize = ushort.Parse(str[0]); } }
                };
            }


            //CargoPlane 
            {
                cargoPlaneDictionary = new()
                {
                    { "ID", 0 },
                    { "Serial", 1 },
                    { "CountryCode", 2 },
                    { "Model", 3 },
                    { "MaxLoad", 4 }
                };
                cargoPlaneDictionaryDisplay = new()
                {
                    {"ID", (CargoPlane cp) => cp.ID.ToString()},
                    {"Serial", (CargoPlane cp) => cp.Serial.ToString()},
                    {"CountryCode", (CargoPlane cp) => cp.Country.ToString()},
                    {"Model", (CargoPlane cp) => cp.Model.ToString() },
                    {"MaxLoad", (CargoPlane cp) => cp.MaxLoad.ToString()}
                };
                cargoPlaneDictionaryPredicates = new()
                {
                    //dwa elementy w string -> znak operacji oraz wartość
                    { "ID", (CargoPlane cp, string[] array) => { return Uints((uint)cp.ID, uint.Parse(array[1]), array[0]); } },
                    { "Serial", (CargoPlane cp, string[] array) => { return Strings(cp.Serial, array[1], array[0]); } },
                    { "CountryCode", (CargoPlane cp, string[] array) => { return Strings(cp.Country, array[1], array[0]); } },
                    { "Model", (CargoPlane cp, string[] array) => { return Strings(cp.Model, array[1], array[0]); } },
                    { "MaxLoad", (CargoPlane cp, string[] array) => { return Floats(cp.MaxLoad, float.Parse(array[1]), array[0]); } }
                };
                cargoPlaneDictionaryUpdate = new()
                {
                    { "ID", (CargoPlane cp, string[] str) => _decoratorTransactionLists.decoratorUpdateLists.ChangeIDCargoPlane(cp.ID, ulong.Parse(str[0]))},
                    { "Serial", (CargoPlane cp, string[] str) =>{ cp.Serial = str[0]; } },
                    { "CountryCode", (CargoPlane cp, string[] str) =>{ cp.Country = str[0]; } },
                    { "Model", (CargoPlane cp, string[] str) =>{ cp.Model = str[0]; } },
                    { "MaxLoad", (CargoPlane cp, string[] str) =>{ cp.MaxLoad = float.Parse(str[0]); } }
                };
            }

            //Flights
            {
                flightsDictionary = new()
                {
                    { "ID", 0 },
                    { "Origin", 1 },
                    { "Target", 2 },
                    { "TakeOffTime", 3 },
                    { "LandingTime", 4 },
                    { "WorldPosition", 5 },
                    { "AMSL", 6 },
                    { "Plane", 7 }
                };
                flightsDictionaryDisplay = new()
                {
                    {"ID", (Flight fl) => fl.ID.ToString() },
                    {"Origin", (Flight fl) => fl.OriginID.ToString() }, //popraw
                    {"Target", (Flight fl) => fl.TargetID.ToString() }, //popraw
                    {"TakeOffTime", (Flight fl) => fl.TakeoffTime.ToString() },
                    { "LandingTime", (Flight fl) => fl.LandingTime.ToString() },
                    {"WorldPosition", (Flight fl) => "{" + $"{fl.Latitude}, {fl.Longitute}" + "}"},
                    {"AMSL", (Flight fl) => fl.AMSL.ToString() },
                    {"Plane", (Flight fl) => fl.PlaneID.ToString() } //popraw
                };

                flightsDictionaryPredicates = new()
                {
                    //dwa elementy w string -> znak operacji oraz wartość
                    { "ID", (Flight fl, string[] array) => { return Uints((uint)fl.ID, uint.Parse(array[1]), array[0]); } },
                    { "Origin", (Flight fl, string[] array) => airportDictionaryPredicates[array[2]].Invoke(fl.Origin!, [array[0], array[1], array[3]]) },
                    { "Target", (Flight fl, string[] array) => airportDictionaryPredicates[array[2]].Invoke(fl.Target!, [array[0], array[1], array[3]]) },
                    { "TakeOffTime", (Flight fl, string[] array) => { return Date(fl.TakeoffTime, DateTime.Parse(array[1]), array[0]); } },
                    { "LandingTime", (Flight fl, string[] array) => { return Date(fl.LandingTime, DateTime.Parse(array[1]), array[0]); } },
                    { "WorldPosition", (Flight fl, string[] array) => worldPositionDictionaryPredicates[array[2]].Invoke(new WorldPosition(fl.Latitude, fl.Longitute), array) },
                    { "AMSL", (Flight fl, string[] array) => { return Floats(fl.AMSL, float.Parse(array[1]), array[0]); } },
                    { "Plane", (Flight fl, string[] array) => planeDictionaryPredicates[array[2]].Invoke(fl.Plane!, array) }
                };

                flightsDictionaryUpdate = new()
                {
                    //dwa elementy w string -> jaka wartosc przypisac oraz ktory element struktury updateujemy 
                    { "ID", (Flight fl, string[] array) =>
                        {
                            _decoratorUpdateFlightGUIListConverter.UpdateID(fl.ID, ulong.Parse(array[0]));
                            _decoratorTransactionLists.decoratorUpdateLists.ChangeIDFlight(fl.ID, ulong.Parse(array[0]));
                        }
                    },
                    { "Origin", (Flight fl, string[] array) => airportDictionaryUpdate[array[1]].Invoke(fl.Origin!, [array[0], array[2]]) },
                    { "Target", (Flight fl, string[] array) => airportDictionaryUpdate[array[1]].Invoke(fl.Target!, [array[0], array[2]]) },
                    { "TakeOffTime", (Flight fl, string[] array) => { fl.TakeoffTime = DateTime.Parse(array[0]); } },
                    { "LandingTime", (Flight fl, string[] array) => { fl.LandingTime = DateTime.Parse(array[0]); } },
                    { "WorldPosition", (Flight fl, string[] array) => worldPositionFlightsDictionaryUpdate[array[1]].Invoke(fl, array[0]) },
                    { "AMSL", (Flight fl, string[] array) => { fl.AMSL = float.Parse(array[0]); } },
                    { "Plane", (Flight fl, string[] array) =>  planeDictionaryUpdate[array[1]].Invoke(fl.Plane!, array[0]) }
                };


            }

            {
                _dictionaryStructCorrectness = new()
                {
                     { "Origin", (string array) => airportDictionaryPredicates.ContainsKey(array)  },
                     { "Target", (string array) =>  airportDictionaryPredicates.ContainsKey(array)  },
                     { "WorldPosition", (string array) =>  worldPositionDictionaryPredicates.ContainsKey(array) },
                     { "Plane", (string array) =>  planeDictionaryPredicates.ContainsKey(array) }
                };

            }

        }

        private static string Lines(int number) => "-+".PadLeft(number, '-');

        public static string MyLine(string str, DisplayType t, int number, string key) => 
            t == DisplayType.VariableName ? key.PadRight(number - 2, ' ') + " |" : (t == DisplayType.VariableValue ? (str + " |").PadLeft(number, ' ') : Lines(number));
        
        private bool Uints(uint a, uint b, string op)
        {
            if (precicateUints.TryGetValue(op, out Func<uint, uint, bool>? func))
                return func.Invoke(a, b);
            else
                return false;
        }
        private bool Strings(string a, string b, string op)
        {
            if (precicateString.TryGetValue(op, out Func<string, string, bool>? func))
                return func.Invoke(a, b);
            else
                return false;
        }
        private bool Floats(float a, float b, string op)
        {
            if (precicateFloats.TryGetValue(op, out Func<float, float, bool>? func))
                return func.Invoke(a, b);
            else
                return false;
        }
        private bool Date(DateTime a, DateTime b, string op)
        {
            if (precicateDate.TryGetValue(op, out Func<DateTime, DateTime, bool>? func))
                return func.Invoke(a, b);
            else
                return false;
        }

        private Dictionary<string, Action<string[]>> NewDictionaryDisplays()
        {
            return new()
            {
                {"Crew",  _decoratorTransactionLists.DisplayCrew},
                {"Passenger",  _decoratorTransactionLists.DisplayPassenger},
                {"Cargo",  _decoratorTransactionLists.DisplayCargo},
                {"Airport",  _decoratorTransactionLists.DisplayAirport},
                {"PassengerPlane",  _decoratorTransactionLists.DisplayPassengerPlane},
                {"CargoPlane",  _decoratorTransactionLists.DisplayCargoPlane},
                {"Flight",  _decoratorTransactionLists.DisplayFlights}
            };
        }

        private Dictionary<string, Action<string[]>> NewDictionaryUpdates()
        {
            return new()
            {
                {"Crew",  _decoratorTransactionLists.UpdateCrew},
                {"Passenger",  _decoratorTransactionLists.UpdatePassenger},
                {"Cargo",  _decoratorTransactionLists.UpdateCargo},
                {"Airport",  _decoratorTransactionLists.UpdateAirport},
                {"PassengerPlane",  _decoratorTransactionLists.UpdatePassengerPlane},
                {"CargoPlane",  _decoratorTransactionLists.UpdateCargoPlane},
                {"Flight",  _decoratorTransactionLists.UpdateFlight}
            };
        }
        private Dictionary<string, Action<string[]>> NewDictionaryAdds()
        {
            return new()
            {
                {"Crew",  _decoratorTransactionLists.AddCrew},
                {"Passenger",  _decoratorTransactionLists.AddPassenger},
                {"Cargo",  _decoratorTransactionLists.AddCargo},
                {"Airport",  _decoratorTransactionLists.AddAirport},
                {"PassengerPlane",  _decoratorTransactionLists.AddPassengerPlane},
                {"CargoPlane",  _decoratorTransactionLists.AddCargoPlane},
                {"Flight",  _decoratorTransactionLists.AddFlight}
            };
        }
        private Dictionary<string, Action<string[]>> NewDictionaryDeletes()
        {
            return new()
            {
                {"Crew",  _decoratorTransactionLists.DeleteCrew},
                {"Passenger",  _decoratorTransactionLists.DeletePassenger},
                {"Cargo",  _decoratorTransactionLists.DeleteCargo},
                {"Airport",  _decoratorTransactionLists.DeleteAirport},
                {"PassengerPlane",  _decoratorTransactionLists.DeletePassengerPlane},
                {"CargoPlane",  _decoratorTransactionLists.DeleteCargoPlane},
                {"Flight",  _decoratorTransactionLists.DeleteFlight}
            };
        }

        public bool ContainsKeyTransactions(string key) => transactions.ContainsKey(key);
        public bool ContainsKeyDisplays(string key) => displays.ContainsKey(key);
        public bool ContainsKeyUpdates(string key) => updates.ContainsKey(key);
        public bool ContainsKeyDeletes(string key) => deletes.ContainsKey(key);
        public bool ContainsKeyAdds(string key) => adds.ContainsKey(key);

    }


}
