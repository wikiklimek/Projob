using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkSourceSimulator;

namespace ProjOb
{
    public class FlightGUIAdapter
    {
        private ulong _id;
        private double _mcr;
        private WorldPosition _worldPosition;
        public ulong ID { get { return _id; } set { _id = value; } }
        public WorldPosition WorldPosition { get { return _worldPosition; } set { _worldPosition = value; } }
        public double MapCoordRotation { get { return _mcr; } }
        public FlightGUIAdapter() => Update(0, new WorldPosition(), 0);
        public FlightGUIAdapter(ulong newID) => Update(newID, new WorldPosition(), 0);
        public FlightGUIAdapter(ulong ID, WorldPosition worldPosition, double mapCoordRotation) => Update(ID, worldPosition, mapCoordRotation);
        public void Update(ulong newID, WorldPosition worldPosition, double mapCoordRotation)
        {
            UpdateID(newID);
            UpdatePosition(worldPosition);
            UpdateRotation(mapCoordRotation);
        }
        public void UpdatePosition(WorldPosition worldPosition) => _worldPosition = worldPosition;
        public void UpdateRotation(double mapCoordRotation) => _mcr = mapCoordRotation;
        public void UpdateID(ulong newID) => _id = newID;
    }

}
