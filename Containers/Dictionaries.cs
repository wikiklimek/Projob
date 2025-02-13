using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using ShimSkiaSharp;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Timers;
using NetworkSourceSimulator;
using Avalonia.Controls;

namespace ProjOb
{
    public class FactoryDictionary
    {
        public Dictionary<string, IAbstractFactory> Dictionary;
        public FactoryDictionary()
        {
            Dictionary = new Dictionary<string, IAbstractFactory>
             {
                {"C", new FactoryCrew() },
                {"P", new FactoryPassenger() },
                {"CA", new FactoryCargo() },
                {"CP", new FactoryCargoPlane() },
                {"PP", new FactoryPassengerPlane() },
                {"AI", new FactoryAirport() },
                {"FL", new FactoryFlight() },
                {"NCR", new FactoryCrew() },
                {"NPA", new FactoryPassenger() },
                {"NCA", new FactoryCargo() },
                {"NCP", new FactoryCargoPlane() },
                {"NPP", new FactoryPassengerPlane() },
                {"NAI", new FactoryAirport() },
                {"NFL", new FactoryFlight() }
             };
        }
    }

    public class Orders
    {
        private Dictionary<string, Action> _dictionary;
        private Transaction _transaction;
        public Orders(Lists lists, DecoratorUpdateFlightGUIListConverter decoratorUpdateFlightGUIListConverter)
        {
            _transaction = new(lists, decoratorUpdateFlightGUIListConverter);
            _dictionary = new Dictionary<string, Action>
            {
                { "report", () => Interface.GetNewsOverwiew() },
                { "print", () => Interface.JsonSerializationObjects1() }
            };
        }

        private bool CorrectOrderNotTransaction(string order) => _dictionary.ContainsKey(order);
        private bool CorrectOrderTransaction(string order) => _transaction.CorrectOrder(order);
        public void DoOrder(string[] orderArray)
        {
            if (orderArray == null)
                return;

            if (CorrectOrderNotTransaction(orderArray[0]))
                _dictionary[orderArray[0]].Invoke();
            else if (CorrectOrderTransaction(orderArray[0]))
                _transaction.LoadAndExecuteTransaction(orderArray);
            else if (orderArray[0] != "exit")
                Console.WriteLine("Incorrect order");
        }

    }

    public class UpdatingDataDictionary
    {
        private DecoratorUpdateLists _decoratorLists;
        private DecoratorUpdateFlightGUIListConverter? _decoratorflGUIlc;
        private Dictionary<string, Action<ulong, ulong>> _dictionaryID;
        private Dictionary<string, Action<PositionUpdateArgs, DecoratorUpdateFlightGUIListConverter>> _dictionaryPosition;
        private Dictionary<string, Action<ContactInfoUpdateArgs>> _dictionaryContactInfo;
        public UpdatingDataDictionary(DecoratorUpdateLists decoratorLists)
        {
            this._decoratorLists = decoratorLists;
            _dictionaryID = new()
            {
                {"C", _decoratorLists.ChangeIDCrew },
                {"P", _decoratorLists.ChangeIDPassenger },
                {"CA", _decoratorLists.ChangeIDCargo },
                {"CP", _decoratorLists.ChangeIDCargoPlane },
                {"PP", _decoratorLists.ChangeIDPassengerPlane },
                {"AI", _decoratorLists.ChangeIDAirport },
                {"FL", (ulong oldID, ulong newID) =>
                    {
                        _decoratorLists.ChangeIDFlight(oldID, newID);
                        _decoratorflGUIlc?.UpdateID(oldID, newID);
                    }
                }
            };
            _dictionaryContactInfo = new()
            {
                {"C", _decoratorLists.ChangeContactInfoCrew },
                {"P", _decoratorLists.ChangeContactInfoPassenger }
            };
            _dictionaryPosition = new()
            {
                {"AI", _decoratorLists.ChangePositionAirport},
                {"FL", _decoratorLists.ChangePositionFlight },
                {"PP", _decoratorLists.ChangePositionPlane },
                {"CP", _decoratorLists.ChangePositionPlane }
            };
        }

        public void UpdateID(string type, ulong oldID, ulong newID, DecoratorUpdateFlightGUIListConverter decoratorflGUIlc)
        {
            _decoratorflGUIlc = decoratorflGUIlc;
            if (_dictionaryID.TryGetValue(type, out var func))
                func.Invoke(oldID, newID);
            else
            {/*Console.Writeline("Ten obiekt nie ma takiej zmiennej");*/}
        }

        public void UpdateContactInfo(string type, ContactInfoUpdateArgs args)
        {
            if (_dictionaryContactInfo.TryGetValue(type, out var func))
                func.Invoke(args);
            else
            {/*Console.Writeline("Ten obiekt nie ma takiej zmiennej");*/}
        }
        public void UpdatePosition(string type, PositionUpdateArgs args, DecoratorUpdateFlightGUIListConverter decoratorflGUIlc)
        {
            _decoratorflGUIlc = decoratorflGUIlc;
            if (_dictionaryPosition.TryGetValue(type, out var func))
                func.Invoke(args, decoratorflGUIlc);
            else
            {/*Console.Writeline("Ten obiekt nie ma takiej zmiennej");*/}
        }
    }


}


