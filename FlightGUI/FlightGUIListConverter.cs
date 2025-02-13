using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using BruTile.Wms;
using Mapsui.Projections;
using NetworkSourceSimulator;

namespace ProjOb
{
    public class FlightGUIListConverter
    {
        private Dictionary<UInt64, Flight>? _flights;
        private readonly List<FlightGUIAdapter> _adapterFlightGUIs = [];
        private readonly FlightGUIConverter _converterGUI = new();
        private readonly List<UInt64> _ifFlies = [];
        public List<FlightGUIAdapter> GetNewFlightGUI { get { return _adapterFlightGUIs; } }
        public List<UInt64> GetPlanes { get { return _ifFlies; } }
        public FlightGUIListConverter() { }

        public void AddAndConvertNew(Dictionary<UInt64, Flight> flights, DateTime NowTime)
        {
            AddFlights(flights);
            ConvertList(NowTime);
        }

        private void AddFlights(Dictionary<UInt64, Flight> flights) => _flights = flights;

        private void ConvertList(DateTime NowTime)
        {
            if (_flights == null)
                return;

            bool add = false;
            FlightGUIAdapter? flGUIold = null;
            DateTime nowToSeconds = new(NowTime.Year, NowTime.Month, NowTime.Day, NowTime.Hour, NowTime.Minute, NowTime.Second);

            //usuniete samoloty, potem zrob usuniete lotniska itd
            
            foreach (KeyValuePair<ulong, Flight> fl in _flights)
                //jezeli lot jest aktualny
                if (!FlightGUIConverter.IfHasTargetOriginPlane(fl.Value) && (flGUIold = _adapterFlightGUIs.Find(p => p.ID == fl.Key)) != null)
                {
                    _adapterFlightGUIs.Remove(flGUIold);
                    _ifFlies.Remove(fl.Value.PlaneID);
                }
                else if (_converterGUI.IfActualFlight(fl.Value, nowToSeconds))
                {
                    flGUIold = null;

                    //jezeli tego lotu nie ma na liscie lotow GUI i ten samolot nie leci innym lotem
                    if ((flGUIold = _adapterFlightGUIs.Find(p => p.ID == fl.Key)) == null && !_ifFlies.Contains(fl.Value.PlaneID))
                    {
                        flGUIold = new(fl.Key);
                        add = true;
                    }
                    else add = false;

                    if (flGUIold == null)
                        continue;

                    //Zmieniamy do GUI ten lot tylko jezeli reset sie udal (czyli jezeli aktualny), dodajemy tylko jest to "nowy" lot
                    if (_converterGUI.Reset(fl.Value, flGUIold, nowToSeconds, add) && _converterGUI.ToGUI(nowToSeconds) && add)
                    {
                        _adapterFlightGUIs.Add(_converterGUI.GetNewFlightGUI!);
                        _ifFlies.Add(fl.Value.PlaneID);
                    }

                }
                //jezeli lot jest nieaktualny i jest pokazywany na naszej liscie lotow GUI
                else if ((flGUIold = _adapterFlightGUIs.Find(p => p.ID == fl.Key)) != null)
                {
                    //Console.WriteLine("USUNAC");
                    _adapterFlightGUIs.Remove(flGUIold);
                    _ifFlies.Remove(fl.Value.PlaneID);
                }


        }

    }


}
