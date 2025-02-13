using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mapsui.Projections;
using NetworkSourceSimulator;

namespace ProjOb
{
    public class FlightGUIConverter
    {
        private FlightGUIAdapter? _adapterFlightGUI;
        private Flight? _fl;
        public FlightGUIAdapter? GetNewFlightGUI { get { return _adapterFlightGUI; } }

        public FlightGUIConverter()
        {
            _fl = null;
            _adapterFlightGUI = null;
        }

        public bool Reset(Flight fl, FlightGUIAdapter flGUI, DateTime nowToSeconds, bool ifnewflight)
        {
            if (!IfHasTargetOriginPlane(fl))
                return false;

            _fl = fl;
            _adapterFlightGUI = flGUI;

            if (!ifnewflight)
                return true;

            if (!IfActualFlight(fl, nowToSeconds))
                return false;

            _adapterFlightGUI!.UpdateID(_fl!.ID);
            _adapterFlightGUI!.UpdatePosition(NewWorldPositionStart(nowToSeconds));

            return true;
        }
        private static void UpdateLandingTime(Flight fl)
        {
            if (fl!.TakeoffTime > fl.LandingTime)
            {
                if (DateTime.Now < fl.LandingTime)
                    fl.TakeoffTime = fl.TakeoffTime.AddDays(-1);
                else
                    fl.LandingTime = fl.LandingTime.AddDays(1);
            }
        }
        public bool IfActualFlight(Flight newFL, DateTime NowTime)
        {
            UpdateLandingTime(newFL);
            if (NowTime >= newFL.LandingTime || NowTime <= newFL.TakeoffTime)
                return false;
            return true;
        }
        public static bool IfHasTargetOriginPlane(Flight fl)
        {
            if (fl.Target == null || fl.Origin == null || fl.Plane == null)
                return false;
            return true;
        }

        public bool ToGUI(DateTime NowTime)
        {
            if (_fl == null || _adapterFlightGUI == null)
                return false;

            if (!IfActualFlight(this._fl, NowTime))
                return false;

            _adapterFlightGUI!.UpdatePosition(NewWorldPosition(NowTime));
            _adapterFlightGUI!.UpdateRotation(NewNewMapRotation());

            return true;

        }
        private WorldPosition NewWorldPosition(DateTime NowTime)
        {
            double NewLatitude, NewLongitude;
            TimeSpan timeNow = new TimeSpan(0, 0, 1);
            TimeSpan timeAll = (_fl!.LandingTime - NowTime) + timeNow;

            double DiffLatitude = Convert.ToDouble(_fl.Target!.Latitude! - _adapterFlightGUI!.WorldPosition.Latitude);
            double DiffLongitude = Convert.ToDouble(_fl.Target!.Longitude! - _adapterFlightGUI.WorldPosition.Longitude);

            NewLatitude = Convert.ToDouble(_adapterFlightGUI.WorldPosition.Latitude!) +
                DiffLatitude * (timeNow.TotalSeconds / timeAll.TotalSeconds);

            NewLongitude = Convert.ToDouble(_adapterFlightGUI.WorldPosition.Longitude!) +
                DiffLongitude * (timeNow.TotalSeconds / timeAll.TotalSeconds);

            return new WorldPosition(NewLatitude, NewLongitude);
        }
        private WorldPosition NewWorldPositionStart(DateTime NowTime)
        {
            double NewLatitude, NewLongitude;
            TimeSpan timeNow = NowTime - _fl!.TakeoffTime;
            TimeSpan timeAll = _fl.LandingTime - _fl.TakeoffTime;

            double DiffLatitude = Convert.ToDouble(_fl.Target!.Latitude! - _fl.Origin!.Latitude);
            double DiffLongitude = Convert.ToDouble(_fl.Target!.Longitude! - _fl.Origin!.Longitude);

            NewLatitude = Convert.ToDouble(_fl.Origin.Latitude!) +
                DiffLatitude * (timeNow.TotalSeconds / timeAll.TotalSeconds);

            NewLongitude = Convert.ToDouble(_fl.Origin.Longitude!) +
                DiffLongitude * (timeNow.TotalSeconds / timeAll.TotalSeconds);

            return new WorldPosition(NewLatitude, NewLongitude);
        }
        private double NewNewMapRotation()
        {
            double TargetX, TargetY, OriginX, OriginY;

            (TargetX, TargetY) = SphericalMercator.FromLonLat(Convert.ToDouble(_fl!.Target!.Longitude!), Convert.ToDouble(_fl.Target.Latitude!));
            (OriginX, OriginY) = SphericalMercator.FromLonLat(Convert.ToDouble(_adapterFlightGUI!.WorldPosition.Longitude), Convert.ToDouble(_adapterFlightGUI.WorldPosition.Latitude!));

            double NewMapCoord = Math.Atan((TargetY - OriginY) / (TargetX - OriginX));
            if ((TargetX - OriginX) < 0) NewMapCoord += Math.PI;
            else NewMapCoord += 2 * Math.PI;
            NewMapCoord -= Math.PI / 2;
            NewMapCoord *= -1;

            return NewMapCoord;
        }

    }
}
