using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Diagnostics;
using NetworkSourceSimulator;

namespace ProjOb
{
    public class MessageReader
    {
        private Lists _lists;
        private FactoryDictionary _dictionaries;
        private NetworkSourceSimulator.NetworkSourceSimulator _TCPserver;
        public MessageReader(NetworkSourceSimulator.NetworkSourceSimulator TCPserver, Lists lists)
        {
            _TCPserver = TCPserver;
            _dictionaries = new FactoryDictionary();
            _lists = lists;
        }

        public void MessageReaderFunction(object sender, NewDataReadyArgs args)
        {
            Message message = _TCPserver.GetMessageAt(args.MessageIndex);
            IAbstractFactory? factory;

            //znacznik wiadomosci
            char[] value = {
                Convert.ToChar(message.MessageBytes[0]),
                Convert.ToChar(message.MessageBytes[1]),
                Convert.ToChar(message.MessageBytes[2])
            };

            //wyluskanie fabryki 
            string name = new string(value);
            bool success = _dictionaries.Dictionary.TryGetValue(name, out factory);

            //dodanie obiektu
            lock (_lists)
                if (success && factory != null)
                {
                    factory.CreateNew(message.MessageBytes);
                    factory.Add(ref _lists);
                }
        }

        public void DoHandler() => _TCPserver.OnNewDataReady += this.MessageReaderFunction;
        public void UndoHandler() => _TCPserver.OnNewDataReady -= this.MessageReaderFunction;
    }
}
