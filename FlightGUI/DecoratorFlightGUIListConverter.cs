using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkSourceSimulator;

namespace ProjOb
{
    public class DecoratorUpdateFlightGUIListConverter
    {
        private FlightGUIListConverter _flGUIlc;
        public DecoratorUpdateFlightGUIListConverter(FlightGUIListConverter flightGUIListConverter) => _flGUIlc = flightGUIListConverter;
        public void UpdateID(ulong oldID, ulong newID)
        {
            FlightGUIAdapter? fGUI;

            if ((fGUI = _flGUIlc.GetNewFlightGUI.Find(f => f.ID == oldID)) != null)
            {
                fGUI.ID = newID;
                _flGUIlc.GetPlanes.Remove(oldID);
                _flGUIlc.GetPlanes.Add(newID);
            }
        }
        
        public (bool, PositionUpdateArgs) UpdatePosition(PositionUpdateArgs args, ulong newID)
        {
            FlightGUIAdapter? fGUI;

            if ((fGUI = _flGUIlc.GetNewFlightGUI.Find(f => f.ID == newID)) != null)
            {
                PositionUpdateArgs oldData =
                    new()
                    {
                        AMSL = 0,
                        Latitude = Convert.ToSingle(fGUI.WorldPosition.Latitude),
                        Longitude = Convert.ToSingle(fGUI.WorldPosition.Longitude),
                        ObjectID = args.ObjectID
                    };

                fGUI.WorldPosition = new(args.Latitude, args.Longitude);
                return (true, oldData);
            }
            else return (false, new());
        }

        public void UpdateLat(ulong ID, float Lat)
        {
            FlightGUIAdapter? fGUI;
            if ((fGUI = _flGUIlc.GetNewFlightGUI.Find(f => f.ID == ID)) != null)
                fGUI.WorldPosition = new(Lat, fGUI.WorldPosition.Longitude);
        }

        public void UpdateLong(ulong ID, float Long)
        {
            FlightGUIAdapter? fGUI;
            if ((fGUI = _flGUIlc.GetNewFlightGUI.Find(f => f.ID == ID)) != null)
                fGUI.WorldPosition = new(fGUI.WorldPosition.Latitude, Long);
        }

    }
}