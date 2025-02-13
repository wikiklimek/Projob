using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkSourceSimulator;

namespace ProjOb
{
    public class FlightGUIListDataAdapter : FlightsGUIData
    {
        private List<FlightGUIAdapter> _flightGUIAdapters;
        public FlightGUIListDataAdapter() => _flightGUIAdapters = [];
        public void UpdateFlights(List<FlightGUIAdapter> _flGUIa) => _flightGUIAdapters = _flGUIa;
        public override int GetFlightsCount() => _flightGUIAdapters != null ? _flightGUIAdapters.Count : 0;
        public override ulong GetID(int index) => _flightGUIAdapters[index].ID;
        public override WorldPosition GetPosition(int index) => _flightGUIAdapters[index].WorldPosition;
        public override double GetRotation(int index) => _flightGUIAdapters[index].MapCoordRotation;

    }
}
