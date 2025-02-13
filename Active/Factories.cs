using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkSourceSimulator;

namespace ProjOb
{
    public interface IAbstractFactory
    {
        Object1 Create(string[] buf);
        Object1 CreateNew(byte[] buf);
        void Add(ref Lists lists);

    }
    public class FactoryCrew : IAbstractFactory
    {
        private Crew? _object1;
        public Object1 Create(string[] buf)
        {
            _object1 = new Crew(ulong.Parse(buf[0]), buf[1], ushort.Parse(buf[2]), buf[3], buf[4], ushort.Parse(buf[5]), buf[6]);
            return _object1;
        }
        public Object1 CreateNew(byte[] buf)
        {
            UInt64 ID = BitConverter.ToUInt64(buf, 7);
            UInt16 NL = BitConverter.ToUInt16(buf, 15);
            char[] Name = new char[NL];
            for(int i = 0; i < NL; i++) Name[i] = Convert.ToChar(buf[17 + i]); 
            UInt16 Age = BitConverter.ToUInt16(buf, 17 + NL);
            char[] PhoneNumber = new char[12]; 
            for (int i = 0; i < 12; i++) PhoneNumber[i] = Convert.ToChar(buf[19 + NL + i]); 
            UInt16 EL = BitConverter.ToUInt16(buf, 31 + NL);
            char[] EmailAddress = new char[EL];
            for (int i = 0; i < EL; i++) EmailAddress[i] = Convert.ToChar(buf[33 + NL + i]); 
            UInt16 Practise = BitConverter.ToUInt16(buf, 33 + NL + EL);
            char[] Role = { Convert.ToChar(buf[35 + NL + EL]) };
            _object1 = new Crew(ID, new string(Name), Age, new string(PhoneNumber), new string(EmailAddress), Practise, new string(Role));
            return _object1;
        }
        public void Add(ref Lists lists)
        {
            if (_object1 != null) lists.Crew = _object1;
            _object1 = null;
        }
    }
    public class FactoryPassenger : IAbstractFactory
    {
        private Passenger? _object1;
        public Object1 Create(string[] buf)
        {
            _object1 = new Passenger(ulong.Parse(buf[0]), buf[1], ushort.Parse(buf[2]), buf[3], buf[4], buf[5], ulong.Parse(buf[6]));
            return _object1;
        }
        public Object1 CreateNew(byte[] buf)
        {
            UInt64 ID = BitConverter.ToUInt64(buf, 7);
            UInt16 NL = BitConverter.ToUInt16(buf, 15);
            char[] Name = new char[NL];
            for (int i = 0; i < NL; i++) Name[i] = Convert.ToChar(buf[17 + i]);
            UInt16 Age = BitConverter.ToUInt16(buf, 17 + NL);
            char[] PhoneNumber = new char[12];
            for (int i = 0; i < 12; i++) PhoneNumber[i] = Convert.ToChar(buf[19 + NL + i]);
            UInt16 EL = BitConverter.ToUInt16(buf, 31 + NL);
            char[] EmailAddress = new char[EL];
            for (int i = 0; i < EL; i++) EmailAddress[i] = Convert.ToChar(buf[33 + NL + i]); 
            char[] Class = { Convert.ToChar(buf[33 + NL + EL]) };
            UInt64 Miles = BitConverter.ToUInt64(buf, 34 + NL + EL);
            
            _object1 = new Passenger(ID, new string(Name), Age, new string(PhoneNumber), new string(EmailAddress), new string(Class), Miles);
            return _object1;
        }
        public void Add(ref Lists lists)
        {
            if (_object1 != null) lists.Passenger = _object1;
            _object1 = null;
        }
    }
    public class FactoryCargo : IAbstractFactory
    {
        private Cargo? _object1;
        public Object1 Create(string[] buf)
        {
            _object1 = new Cargo(ulong.Parse(buf[0]), float.Parse(buf[1], CultureInfo.InvariantCulture.NumberFormat), buf[2], buf[3]);
            return _object1;
        }
        public Object1 CreateNew(byte[] buf)
        {
            UInt64 ID = BitConverter.ToUInt64(buf, 7);
            Single Weight = BitConverter.ToSingle(buf, 15);
            char[] Code = new char[6];
            for (int i = 0; i < 6; i++) Code[i] = Convert.ToChar(buf[19 + i]);
            UInt16 DL = BitConverter.ToUInt16(buf, 25);
            char[] Description = new char[DL];
            for (int i = 0; i < DL; i++) Description[i] = Convert.ToChar(buf[27 + i]);

            _object1 = new Cargo(ID, Weight, new string(Code), new string(Description));
            return _object1;
        }
        public void Add(ref Lists lists)
        {
            if (_object1 != null) lists.Cargo = _object1;
            _object1 = null;
        }
    }
    public class FactoryCargoPlane : IAbstractFactory
    {
        private CargoPlane? _object1;
        public Object1 Create(string[] buf)
        {
            _object1 = new CargoPlane(ulong.Parse(buf[0]), buf[1], buf[2], buf[3], float.Parse(buf[4], CultureInfo.InvariantCulture.NumberFormat));
            return _object1;
        }
        public Object1 CreateNew(byte[] buf)
        {
            UInt64 ID = BitConverter.ToUInt64(buf, 7);
            char[] Serial = new char[10];
            for (int i = 0; i < 10; i++) Serial[i] = Convert.ToChar(buf[15 + i]);
            char[] ISO = new char[3];
            for (int i = 0; i < 3; i++) ISO[i] = Convert.ToChar(buf[25 + i]);
            UInt16 ML = BitConverter.ToUInt16(buf, 28);
            char[] Model = new char[ML];
            for (int i = 0; i < ML; i++) Model[i] = Convert.ToChar(buf[30 + i]);
            Single MaxLoad = BitConverter.ToSingle(buf, 30 + ML);

            _object1 = new CargoPlane(ID, new string(Serial[0..5]), new string(ISO), new string(Model), MaxLoad);
            return _object1;
        }
        public void Add(ref Lists lists)
        {
            if (_object1 != null) lists.CargoPlane = _object1;
            _object1 = null;
        }
    }
    public class FactoryPassengerPlane : IAbstractFactory
    {
        private PassengerPlane? _object1;
        public Object1 Create(string[] buf)
        {
            return new PassengerPlane(ulong.Parse(buf[0]), buf[1], buf[2], buf[3], ushort.Parse(buf[4]), ushort.Parse(buf[5]), ushort.Parse(buf[6]));
        }
        public Object1 CreateNew(byte[] buf)
        {
            UInt64 ID = BitConverter.ToUInt64(buf, 7);
            char[] Serial = new char[10];
            for (int i = 0; i < 10; i++) Serial[i] = Convert.ToChar(buf[15 + i]);
            char[] ISO = new char[3];
            for (int i = 0; i < 3; i++) ISO[i] = Convert.ToChar(buf[25 + i]);
            UInt16 ML = BitConverter.ToUInt16(buf, 28);
            char[] Model = new char[ML];
            for (int i = 0; i < ML; i++) Model[i] = Convert.ToChar(buf[30 + i]);

            UInt16 FirstClassSize = BitConverter.ToUInt16(buf, 30 + ML);
            UInt16 BusinessClassSize = BitConverter.ToUInt16(buf, 32 + ML);
            UInt16 EconomyClassSize = BitConverter.ToUInt16(buf, 34 + ML);

            _object1 = new PassengerPlane(ID, new string(Serial[0..5]), new string(ISO), new string(Model), FirstClassSize, BusinessClassSize, EconomyClassSize);
            return _object1;
        }
        public void Add(ref Lists lists)
        {
            if (_object1 != null) lists.PassengerPlane = _object1;
            _object1 = null;
        }
    }
    public class FactoryAirport : IAbstractFactory
    {
        private Airport? _object1;
        public Object1 Create(string[] buf)
        {
            _object1 = new Airport(ulong.Parse(buf[0]), buf[1], buf[2], float.Parse(buf[3], CultureInfo.InvariantCulture.NumberFormat), float.Parse(buf[4], CultureInfo.InvariantCulture.NumberFormat), float.Parse(buf[5], CultureInfo.InvariantCulture.NumberFormat), buf[6]);
            return _object1;
        }
        public Object1 CreateNew(byte[] buf)
        {
            UInt64 ID = BitConverter.ToUInt64(buf, 7);
            UInt16 NL = BitConverter.ToUInt16(buf, 15);
            char[] Name = new char[NL];
            for (int i = 0; i < NL; i++) Name[i] = Convert.ToChar(buf[17 + i]);
            char[] Code = new char[3];
            for (int i = 0; i < 3; i++) Code[i] = Convert.ToChar(buf[17 + NL + i]);
            Single Longitude = BitConverter.ToSingle(buf, 20 + NL);
            Single Latitude = BitConverter.ToSingle(buf, 24 + NL);
            Single AMSL = BitConverter.ToSingle(buf, 28 + NL);
            char[] ISO = new char[3];
            for (int i = 0; i < 3; i++) ISO[i] = Convert.ToChar(buf[32 + NL + i]);

            _object1 = new Airport(ID, new string(Name), new string(Code), Longitude, Latitude, AMSL, new string(ISO));
            return _object1;
        }
        public void Add(ref Lists lists)
        {
            if (_object1 != null) lists.Airport = _object1;
            _object1 = null;
        }
    }
    public class FactoryFlight : IAbstractFactory
    {
        private Flight? _object1;
        public Object1 Create(string[] buf)
        {
            ulong[] tab1 = buf[9].Substring(1, buf[9].Length - 2).Split(';').Select(x => ulong.Parse(x)).ToArray();
            ulong[] tab2 = buf[10].Substring(1, buf[10].Length - 2).Split(';').Select(x => ulong.Parse(x)).ToArray();
            _object1 = new Flight
                (ulong.Parse(buf[0]), ulong.Parse(buf[1]), ulong.Parse(buf[2]), 
                new DateTime(10000 * Convert.ToInt64(buf[3]) + DateTime.UnixEpoch.Ticks), 
                new DateTime(10000 * Convert.ToInt64(buf[4]) + DateTime.UnixEpoch.Ticks), 
                float.Parse(buf[5], CultureInfo.InvariantCulture.NumberFormat), 
                float.Parse(buf[6], CultureInfo.InvariantCulture.NumberFormat), 
                float.Parse(buf[7], CultureInfo.InvariantCulture.NumberFormat), 
                ulong.Parse(buf[8]), tab1, tab2);
            return _object1;
        }
        public Object1 CreateNew(byte[] buf)
        {
            UInt64 ID = BitConverter.ToUInt64(buf, 7);
            UInt64 OriginID = BitConverter.ToUInt64(buf, 15);
            UInt64 TargetID = BitConverter.ToUInt64(buf, 23);
            DateTime TakeoffTime = new DateTime(10000 * BitConverter.ToInt64(buf, 31) + DateTime.UnixEpoch.Ticks);
            DateTime LandingTime = new DateTime(10000 * BitConverter.ToInt64(buf, 39) + DateTime.UnixEpoch.Ticks);
            UInt64 PlaneID = BitConverter.ToUInt64(buf, 47);
            UInt16 CC = BitConverter.ToUInt16(buf, 55);
            UInt64[] Crew = new UInt64[CC];
            for (int i = 0; i < CC; i++) Crew[i] = BitConverter.ToUInt64(buf,57 + 8 * i);
            UInt16 PCC = BitConverter.ToUInt16(buf, 57 + 8 * CC);
            UInt64[] PasCarg = new UInt64[PCC];
            for (int i = 0; i < PCC; i++) PasCarg[i] = BitConverter.ToUInt64(buf, 59 + 8 * CC + 8 * i);

            _object1 = new Flight(ID, OriginID, TargetID, TakeoffTime, LandingTime, PlaneID, Crew, PasCarg);
            return _object1;
        }
        public void Add(ref Lists lists)
        {
            if(_object1 != null)
            {
                _object1.Origin = lists.GetByIDAirport(_object1.OriginID);
                _object1.Target = lists.GetByIDAirport(_object1.TargetID);
                _object1.Latitude = _object1.Origin!.Latitude;
                _object1.Longitute = _object1.Origin!.Longitude;
                _object1.Plane = lists.GetByIDPlane(_object1.PlaneID);


                lists.Flight = _object1;
            }
            _object1 = null;
        }
    }

}
