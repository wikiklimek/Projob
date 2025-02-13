using System.Text.Json.Serialization;
using System.Text.Json;
using System.IO;
using System.Diagnostics;
using Avalonia.Rendering;
using System.Timers;
using FlightTrackerGUI;
using NetworkSourceSimulator;
using global::ProjOb;

namespace ProjOb
{
    public static class Interface
    {
        private static Lists _lists = new();
        private static MessageReader? _messageReader;
        private static FlightGUIListDataAdapter _flightsGUIListAdapter = new();
        private static FlightGUIListConverter _converterGUIFlightList = new();
        private static DecoratorUpdateFlightGUIListConverter _decoratorUpdateFlightGUIListConverter = new(_converterGUIFlightList);
        private static DecoratorUpdateLists _decoratorLists = _lists.MakeDecoratorUpdateObject(_decoratorUpdateFlightGUIListConverter);
        //private static DecoratorTransactionLists _decoratorTransactionLists = _lists.MakeDecoratorTransaction(_decoratorUpdateFlightGUIListConverter);
        public static void MyProgram(string pathMessages, string pathUpdates)
        {
            Thread thr = ReadFromConsole();
            ManageGUIFlight();
            SendingUpdatingTCP(pathMessages, pathUpdates);

            thr.Join();
        }
        public static void SendingUpdatingTCP(string pathMessages, string pathUpdates)
        {
            NetworkSourceSimulator.NetworkSourceSimulator TCPserverMessages = new(pathMessages, 1, 5);
            MessageReaderCreator(TCPserverMessages);

            NetworkSourceSimulator.NetworkSourceSimulator TCPserverUpdates = new(pathUpdates, 100, 200);
            UpdateDataCreator(TCPserverUpdates);

            TCPrunning(TCPserverMessages, TCPserverUpdates);
        }
        public static void MessageReaderCreator(NetworkSourceSimulator.NetworkSourceSimulator TCPserverMessages)
        {
            _messageReader = new(TCPserverMessages, _lists);
            _messageReader.DoHandler();
        }
        public static void UpdateDataCreator(NetworkSourceSimulator.NetworkSourceSimulator TCPserverUpdates)
        {
            TCPserverUpdates.OnIDUpdate += IDUpdate;
            TCPserverUpdates.OnPositionUpdate += PositionUpdate;
            TCPserverUpdates.OnContactInfoUpdate += ContactInfoUpdate;

        }
        public static void UndoHandelUpdateData(NetworkSourceSimulator.NetworkSourceSimulator TCPserverUpdates)
        {
            TCPserverUpdates.OnIDUpdate -= IDUpdate;
            TCPserverUpdates.OnPositionUpdate -= PositionUpdate;
            TCPserverUpdates.OnContactInfoUpdate -= ContactInfoUpdate;

        }
        public static Thread ManageGUIFlight()
        {
            Thread thr = ShowFlightTracker();
            UpdateFlightTracker();
            return thr;
        }

        public static void TCPrunning(NetworkSourceSimulator.NetworkSourceSimulator TCPserverMessages,
            NetworkSourceSimulator.NetworkSourceSimulator TCPserverUpdates)
        {
            new Thread(() =>
            {
                TCPserverMessages.Run();
                TCPserverUpdates.Run();
            })
            { IsBackground = true }.Start();
        }

        public static Thread ReadFromConsole()
        {
            Thread thr = new Thread(() =>
            {
                string? orderLine, order;
                string[] orderArray;
                Orders dictionaryOrders = new(_lists, _decoratorUpdateFlightGUIListConverter);
                for (; ; )
                {
                    try
                    {
                        orderLine = Console.ReadLine();

                        if (orderLine == null)
                            continue;

                        orderArray = orderLine.Split(' ');
                        order = orderArray[0];
                        dictionaryOrders.DoOrder(orderArray);
                        if (order == "exit")
                        {
                            _messageReader?.UndoHandler();
                            return;
                        }
                    }
                    catch (Exception ex) { Console.WriteLine($"Error reading {ex.Message}"); }
                }
            });
            thr.Start();
            return thr;
        }
        public static void JsonSerializationObjects1()
        {
            DateTime date = DateTime.Now;
            if (_messageReader != null)
                try
                {
                    lock (_lists) _lists.SerializeJson($"snapshot_{date.Hour}_{date.Minute}_{date.Second}.json");
                }
                catch (Exception ex) { Console.WriteLine($"Error reading {ex.Message}"); }

        }
        public static Thread ShowFlightTracker()
        {
            Thread thr = new Thread(() => FlightTrackerGUI.Runner.Run());
            thr.IsBackground = true;
            thr.Start();
            return thr;
        }

        public static void UpdateFlightTracker()
        {
            new Thread(() =>
            {
                var timer = new System.Timers.Timer(1000);
                timer.Elapsed += UpdateGUIFlightsHandler;
                timer.AutoReset = true;
                timer.Enabled = true;
            })
            { IsBackground = true }.Start();
        }
        public static void UpdateGUIFlightsHandler(object? sender, ElapsedEventArgs a)
        {
            lock (_lists)
                lock (_converterGUIFlightList)
                    _flightsGUIListAdapter.UpdateFlights(_lists.FlightToGUI(a.SignalTime, _converterGUIFlightList));

            FlightTrackerGUI.Runner.UpdateGUI(_flightsGUIListAdapter);
        }

        public static void GetNewsOverwiew()
        {
            lock (_lists)
            {
                NewsGenerator newsGenerator = _lists.FromIReportablesToGenerator(NewMedias());
                string? news = newsGenerator.GenerateNextNews();
                while (news != null)
                {
                    Console.WriteLine(news);
                    news = newsGenerator.GenerateNextNews();
                }
            }

            for (int i = 0; i < 2; i++)
                Console.WriteLine();
        }

        public static List<Media> NewMedias()
        {
            return new List<Media>
            {
                new Television("Telewizja Abelowa"),
                new Television("Kanał TV-tensor"),
                new Radio("Radio Kwantyfikator"),
                new Radio("Radio Shmem"),
                new Newspaper("Gazeta Kategoryczna"),
                new Newspaper("Dziennik Politechniczny")
            };
        }
        public static void IDUpdate(object sender, IDUpdateArgs args)
        {
            lock (_lists)
                lock (_converterGUIFlightList)
                    _decoratorLists.IDUpdate(sender, args);
        }

        public static void ContactInfoUpdate(object sender, ContactInfoUpdateArgs args)
        {
            lock (_lists)
                lock (_converterGUIFlightList)
                    _decoratorLists.ContactInfoUpdate(sender, args);
        }

        public static void PositionUpdate(object sender, PositionUpdateArgs args)
        {
            lock (_lists)
                lock (_converterGUIFlightList)
                    _decoratorLists.PositionUpdate(sender, args);
        }

    }
}

