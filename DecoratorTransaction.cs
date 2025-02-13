using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Avalonia.Controls;
using Newtonsoft.Json.Linq;

namespace ProjOb
{

    public enum TransactionType
    {
        DisplayUpdate = 4,
        Delete = 2
    }

    public class DecoratorTransactionLists
    {
        private readonly Transaction _transaction;
        private readonly DecoratorUpdateFlightGUIListConverter _decoratorflGUIlc;
        private readonly Lists _lists;
        private readonly Dictionary<UInt64, Object1> _listObjects;
        private readonly Dictionary<UInt64, Crew> _listCrew;
        private readonly Dictionary<UInt64, Passenger> _listPassenger;
        private readonly Dictionary<UInt64, Cargo> _listCargo;
        private readonly Dictionary<UInt64, CargoPlane> _listCargoPlane;
        private readonly Dictionary<UInt64, PassengerPlane> _listPassengerPlane;
        private readonly Dictionary<UInt64, Airport> _listAirport;
        private readonly Dictionary<UInt64, Flight> _listFlight;
        private TransactionDictionaries? _transactionDictionaries;
        public readonly DecoratorUpdateLists decoratorUpdateLists;

        public DecoratorTransactionLists(Lists lists, Dictionary<UInt64, Object1> listObjects, Dictionary<UInt64, Crew> listCrew,
            Dictionary<UInt64, Passenger> listPassenger, Dictionary<UInt64, Cargo> listCargo,
            Dictionary<UInt64, CargoPlane> listCargoPlane, Dictionary<UInt64, PassengerPlane> listPassengerPlane,
            Dictionary<UInt64, Airport> listAirport, Dictionary<UInt64, Flight> listFlight, DecoratorUpdateFlightGUIListConverter decoratorflGUIlc,
            TransactionDictionaries? transactionDictionaries, Transaction transaction)
        {
            _transaction = transaction;
            _listObjects = listObjects;
            _listCrew = listCrew;
            _listPassenger = listPassenger;
            _listCargo = listCargo;
            _listAirport = listAirport;
            _listCargoPlane = listCargoPlane;
            _listPassengerPlane = listPassengerPlane;
            _listFlight = listFlight;
            _lists = lists;
            _transactionDictionaries = transactionDictionaries;
            decoratorUpdateLists = _lists.MakeDecoratorUpdateObject(decoratorflGUIlc);
            _decoratorflGUIlc = decoratorflGUIlc;
        }
        public void AddTransactionDictionaries(TransactionDictionaries transactionDictionaries) => _transactionDictionaries = transactionDictionaries;

        
        private void Display<T>(string[] orderArray, Dictionary<ulong, T> listObject,
            Dictionary<string, Func<T, string>> listObjectDisplay, Dictionary<string, Func<T, string[], bool>> objectDictionaryPredicates)
            where T : class
        {
            if (_transactionDictionaries == null)
                return;

            if (!_transaction.CorrectConditionSyntax(orderArray, TransactionType.DisplayUpdate)
                || !_transaction.CorrectConditionVariables<T>(orderArray, TransactionType.DisplayUpdate, objectDictionaryPredicates))
                return;

            List<string> ifDisplay = Transaction.BoolDisplay<T>(listObjectDisplay, orderArray[1]);
            if (ifDisplay.Count == 0)
                return;


            StringBuilder sb = new StringBuilder();

            foreach (var variable in ifDisplay)
                sb.Append(TransactionDictionaries.MyLine(" ", DisplayType.VariableName, _transactionDictionaries.numberDictionaryDisplay[variable], variable));
            Console.WriteLine(sb.ToString().Remove(sb.ToString().Length - 1));

            sb.Clear();

            foreach (var variable in ifDisplay)
                sb.Append(TransactionDictionaries.MyLine(" ", DisplayType.Lines, _transactionDictionaries.numberDictionaryDisplay[variable], " "));
            Console.WriteLine(sb.ToString().Remove(sb.ToString().Length - 1));


            foreach (var pair in listObject.ToList())
                if (Transaction.Condition<T>(pair.Value, orderArray, TransactionType.DisplayUpdate, objectDictionaryPredicates))
                {
                    sb.Clear();
                    foreach (var variable in ifDisplay)
                        sb.Append(
                            TransactionDictionaries.MyLine(
                                listObjectDisplay[variable].Invoke(pair.Value), DisplayType.VariableValue, _transactionDictionaries.numberDictionaryDisplay[variable], variable));
                    Console.WriteLine(sb.ToString().Remove(sb.ToString().Length - 1));
                }


        }


        private IEnumerable<T> Update<T>(string[] orderArray, Dictionary<ulong, T> listObject, Dictionary<string, Action<T, string[]>> objectDictionaryUpdate,
            Dictionary<string, Func<T, string[], bool>> objectDictionaryPredicates)
            where T : class
        {
            if (!_transaction.BoolUpdate(orderArray[3], out (string property, string value, string structField, string structStructFiled)[] updateArray)
                || !_transaction.CorrectConditionSyntax(orderArray, TransactionType.DisplayUpdate))
                return[];

            if (!_transaction.IfCorrectNewID(updateArray)) //dodawnaie istniejacegi ID
                return [];

            IEnumerable<T> enumerable = listObject.ToList().Where(
                pair => Transaction.Condition<T>(pair.Value, orderArray, TransactionType.DisplayUpdate, objectDictionaryPredicates)).Select(pair => pair.Value);

            foreach (var TObject in enumerable)
                    for (int i = 0; i < updateArray.Length; i++)
                objectDictionaryUpdate[updateArray[i].property].Invoke(
                   TObject, [updateArray[i].value, updateArray[i].structField, updateArray[i].structStructFiled]);

            return enumerable;
        }

        private static void AddPart2<T>(string[] orderArray, Dictionary<string, Action<T, string[]>> objectDictionaryUpdate, T object1,
            (string property, string value, string structField, string structStructFiled)[] updateArray)
            where T : Object1, new()
        {
            for (int i = 0; i < updateArray.Length; i++)
                objectDictionaryUpdate[updateArray[i].property].Invoke(
                   object1, [updateArray[i].value, updateArray[i].structField, updateArray[i].structStructFiled]);
        }

        private T? AddPart1<T>(string[] orderArray, out (string property, string value, string structField, string structStructFiled)[] updateArray)
            where T : Object1, new()
        {
            if (_transaction.BoolUpdate(orderArray[3], out updateArray))
            {
                if (!_transaction.IfCorrectNewID(updateArray)) //dodawnaie istniejacegi ID
                    return null;

                var object1 = new T();

                ulong ID = Convert.ToUInt64(_listObjects.Count) + 1;
                while (_listObjects.ContainsKey(ID))
                    ID++;
                object1.ID = ID;

                return object1;
            }
            else return null;
        }

        private IEnumerable<T> Delete<T>(string[] orderArray, Dictionary<ulong, T> listObject, Dictionary<string, Func<T, string[], bool>> dictionaryPredicates)
        {
            if (!_transaction.CorrectConditionSyntax(orderArray, TransactionType.Delete))
                return [];

            return listObject.ToList().Where(
                pair => Transaction.Condition<T>(pair.Value, orderArray, TransactionType.Delete, dictionaryPredicates)).Select(pair => pair.Value);
        }

        public void DisplayCrew(string[] orderArray)
        {
            if (_transactionDictionaries == null)
                return;

            Display<Crew>(orderArray, _listCrew, _transactionDictionaries.crewDictionaryDisplay,
                _transactionDictionaries.crewDictionaryPredicates);

        }

        public void DisplayPassenger(string[] orderArray)
        {
            if (_transactionDictionaries == null)
                return;

            Display<Passenger>(orderArray, _listPassenger, _transactionDictionaries.passengerDictionaryDisplay,
                _transactionDictionaries.passengerDictionaryPredicates);
        }
        public void DisplayCargo(string[] orderArray)
        {
            if (_transactionDictionaries == null)
                return;

            Display<Cargo>(orderArray, _listCargo,  _transactionDictionaries.cargoDictionaryDisplay,
                _transactionDictionaries.cargoDictionaryPredicates);
        }

        public void DisplayAirport(string[] orderArray)
        {
            if (_transactionDictionaries == null)
                return;

            Display<Airport>(orderArray, _listAirport,  _transactionDictionaries.airportDictionaryDisplay,
                _transactionDictionaries.airportDictionaryPredicates);
        }

        public void DisplayPassengerPlane(string[] orderArray)
        {
            if (_transactionDictionaries == null)
                return;

            Display<PassengerPlane>(orderArray, _listPassengerPlane,  _transactionDictionaries.passengerPlaneDictionaryDisplay,
                _transactionDictionaries.passengerPlaneDictionaryPredicates);
        }

        public void DisplayCargoPlane(string[] orderArray)
        {
            if (_transactionDictionaries == null)
                return;

            Display<CargoPlane>(orderArray, _listCargoPlane,  _transactionDictionaries.cargoPlaneDictionaryDisplay,
                _transactionDictionaries.cargoPlaneDictionaryPredicates);
        }

        public void DisplayFlights(string[] orderArray)
        {
            if (_transactionDictionaries == null)
                return;

            Display<Flight>(orderArray, _listFlight,  _transactionDictionaries.flightsDictionaryDisplay,
                _transactionDictionaries.flightsDictionaryPredicates);
        }


        public void UpdateCrew(string[] orderArray)
        {
            if (_transactionDictionaries == null)
                return;

            Update<Crew>(orderArray, _listCrew, _transactionDictionaries.crewDictionaryUpdate, _transactionDictionaries.crewDictionaryPredicates);
        }
        public void UpdatePassenger(string[] orderArray)
        {
            if (_transactionDictionaries == null)
                return;

            Update<Passenger>(orderArray, _listPassenger, _transactionDictionaries.passengerDictionaryUpdate, _transactionDictionaries.passengerDictionaryPredicates);
        }
        public void UpdateCargo(string[] orderArray)
        {
            if (_transactionDictionaries == null)
                return;

            Update<Cargo>(orderArray, _listCargo, _transactionDictionaries.cargoDictionaryUpdate, _transactionDictionaries.cargoDictionaryPredicates);
        }
        public void UpdateAirport(string[] orderArray)
        {
            if (_transactionDictionaries == null)
                return;

            Update<Airport>(orderArray, _listAirport, _transactionDictionaries.airportDictionaryUpdate, _transactionDictionaries.airportDictionaryPredicates);
        }
        public void UpdatePassengerPlane(string[] orderArray)
        {
            if (_transactionDictionaries == null)
                return;

            Update<PassengerPlane>(orderArray, _listPassengerPlane, _transactionDictionaries.passengerPlaneDictionaryUpdate, _transactionDictionaries.passengerPlaneDictionaryPredicates);
        }
        public void UpdateCargoPlane(string[] orderArray)
        {
            if (_transactionDictionaries == null)
                return;

            Update<CargoPlane>(orderArray, _listCargoPlane, _transactionDictionaries.cargoPlaneDictionaryUpdate, _transactionDictionaries.cargoPlaneDictionaryPredicates);
        }
        public void UpdateFlight(string[] orderArray)
        {
            if (_transactionDictionaries == null)
                return;

            Update<Flight>(orderArray, _listFlight, _transactionDictionaries.flightsDictionaryUpdate, _transactionDictionaries.flightsDictionaryPredicates);
        }


        public void DeleteCrew(string[] orderArray)
        {
            if (_transactionDictionaries == null)
                return;

            foreach (var o in Delete<Crew>(orderArray, _listCrew, _transactionDictionaries.crewDictionaryPredicates))
                _lists.RemoveCrew = o;
        }


        public void DeletePassenger(string[] orderArray)
        {
            if (_transactionDictionaries == null)
                return;

            foreach (var o in Delete<Passenger>(orderArray, _listPassenger, _transactionDictionaries.passengerDictionaryPredicates))
                _lists.RemovePassenger = o;

        }
        public void DeleteCargo(string[] orderArray)
        {
            if (_transactionDictionaries == null)
                return;

            foreach (var o in Delete<Cargo>(orderArray, _listCargo, _transactionDictionaries.cargoDictionaryPredicates))
                _lists.RemoveCargo = o;
        }
        public void DeleteCargoPlane(string[] orderArray)
        {
            if (_transactionDictionaries == null)
                return;

            foreach (var o in Delete<CargoPlane>(orderArray, _listCargoPlane, _transactionDictionaries.cargoPlaneDictionaryPredicates))
                _lists.RemoveCargoPlane = o;
        }
        public void DeletePassengerPlane(string[] orderArray)
        {
            if (_transactionDictionaries == null)
                return;

            foreach (var o in Delete<PassengerPlane>(orderArray, _listPassengerPlane, _transactionDictionaries.passengerPlaneDictionaryPredicates))
                _lists.RemovePassengerPlane = o;
        }
        public void DeleteAirport(string[] orderArray)
        {
            if (_transactionDictionaries == null)
                return;

            foreach (var o in Delete<Airport>(orderArray, _listAirport, _transactionDictionaries.airportDictionaryPredicates))
                _lists.RemoveAirport = o;
        }
        public void DeleteFlight(string[] orderArray)
        {
            if (_transactionDictionaries == null)
                return;

            foreach (var o in Delete<Flight>(orderArray, _listFlight, _transactionDictionaries.flightsDictionaryPredicates))
                _lists.RemoveFlight = o;
        }
        public void AddCrew(string[] orderArray)
        {
            if (_transactionDictionaries == null)
                return;

            Crew? object1;
            if ((object1 = AddPart1<Crew>(orderArray, out (string property, string value, string structField, string structStructFiled)[] updateArray)) != null)
            {
                _lists.Crew = object1;
                AddPart2<Crew>(orderArray, _transactionDictionaries.crewDictionaryUpdate, object1, updateArray);
            }
        }
        public void AddPassenger(string[] orderArray)
        {
            if (_transactionDictionaries == null)
                return;

            Passenger? object1;
            if ((object1 = AddPart1<Passenger>(orderArray, out (string property, string value, string structField, string structStructFiled)[] updateArray)) != null)
            {
                _lists.Passenger = object1;
                AddPart2<Passenger>(orderArray, _transactionDictionaries.passengerDictionaryUpdate, object1, updateArray);
            }
        }
        public void AddCargo(string[] orderArray)
        {
            if (_transactionDictionaries == null)
                return;

            Cargo? object1;
            if ((object1 = AddPart1<Cargo>(orderArray, out (string property, string value, string structField, string structStructFiled)[] updateArray)) != null)
            {
                _lists.Cargo = object1;
                AddPart2<Cargo>(orderArray, _transactionDictionaries.cargoDictionaryUpdate, object1, updateArray);
            }
        }
        public void AddAirport(string[] orderArray)
        {
            if (_transactionDictionaries == null)
                return;

            Airport? object1;
            if ((object1 = AddPart1<Airport>(orderArray, out (string property, string value, string structField, string structStructFiled)[] updateArray)) != null)
            {
                _lists.Airport = object1;
                AddPart2<Airport>(orderArray, _transactionDictionaries.airportDictionaryUpdate, object1, updateArray);
            }
        }
        public void AddPassengerPlane(string[] orderArray)
        {
            if (_transactionDictionaries == null)
                return;

            PassengerPlane? object1;
            if ((object1 = AddPart1<PassengerPlane>(orderArray, out (string property, string value, string structField, string structStructFiled)[] updateArray)) != null)
            {
                _lists.PassengerPlane = object1;
                AddPart2<PassengerPlane>(orderArray, _transactionDictionaries.passengerPlaneDictionaryUpdate, object1, updateArray);
            }
        }
        public void AddCargoPlane(string[] orderArray)
        {
            if (_transactionDictionaries == null)
                return;

            CargoPlane? object1;
            if ((object1 = AddPart1<CargoPlane>(orderArray, out (string property, string value, string structField, string structStructFiled)[] updateArray)) != null)
            {
                _lists.CargoPlane = object1;
                AddPart2<CargoPlane>(orderArray, _transactionDictionaries.cargoPlaneDictionaryUpdate, object1, updateArray);
            }
        }
        public void AddFlight(string[] orderArray)
        {
            if (_transactionDictionaries == null)
                return;

            Flight? object1;
            if ((object1 = AddPart1<Flight>(orderArray, out (string property, string value, string structField, string structStructFiled)[] updateArray)) != null)
            {
                _lists.Flight = object1;
                AddPart2<Flight>(orderArray, _transactionDictionaries.flightsDictionaryUpdate, object1, updateArray);
            }
        }

    }
}

