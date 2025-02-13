using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using NetworkSourceSimulator;
using Tmds.DBus.Protocol;
namespace ProjOb
{
    internal class Logs
    {
        private string _path;
        private DateTime _date;
        public Logs(DateTime nowTime) => SetNewFile(nowTime);
        private void SetNewFile(DateTime nowTime)
        {
            _date = nowTime;
            _path = CreateFile(nowTime);
            using (StreamWriter sr = File.CreateText(_path))
            {
                sr.WriteLine($"\t\tLog of the day: {nowTime.ToString("yyyy-MM-dd")}");
                sr.WriteLine("\t\tModifications that has been applied to the data in this program");
                sr.WriteLine(" ");
                sr.WriteLine("..........................................................................................");
                sr.WriteLine(" ");
                sr.WriteLine(" ");
            }
        }
        private string CreateFile(DateTime nowTime)
        {
            int quantity = 0;
            string path = $"Log from {nowTime.ToString("yyyy-MM-dd")}.json";
            if (File.Exists(path))
                do
                    path = $"Log from {nowTime.ToString("yyyy-MM-dd")}({++quantity}).json";
                while (File.Exists(path));

            return path;
        }
        private void UpdateDay(DateTime newDate)
        {
            if (newDate.Day != _date.Day)
                SetNewFile(newDate);
        }

        public void LogMessage(string message)
        {
            UpdateDay(DateTime.Now);
            try
            {
                using (StreamWriter sr = File.AppendText(_path))
                {
                    sr.WriteLine(message);
                    sr.Flush();
                }
            }
            catch (Exception ex) { }
        }

        public void LogMessageUpdateID(ulong oldID, ulong newID, bool success)
        {
            UpdateDay(DateTime.Now);
            try
            {
                using (StreamWriter sr = File.AppendText(_path))
                {
                    sr.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " the following changes has been made");
                    if (success)
                        sr.WriteLine($"Object with an ID {oldID} changed succesfully it's ID to {newID}");
                    else
                        sr.WriteLine($"Object with an ID {oldID} failed to change it's ID to {newID}");
                    sr.Flush();
                }
            }
            catch (Exception ex) { }
        }
        public void LogMessageUpdatePosition(PositionUpdateArgs oldData, PositionUpdateArgs newData, bool success)
        {
            UpdateDay(DateTime.Now);
            try
            {
                using (StreamWriter sr = File.AppendText(_path))
                {
                    sr.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " the following changes has been made");
                    if (success)
                    {
                        sr.WriteLine($"Object with an ID {oldData.ObjectID} changed succesfully it's Position:");
                        sr.WriteLine($"\t     AMSL: from {oldData.AMSL} to {newData.AMSL}");
                        sr.WriteLine($"\tLongitude: from {oldData.Longitude} to {newData.Longitude}");
                        sr.WriteLine($"\t Latitude: from {oldData.Latitude} to {newData.Latitude}");
                    }
                    else
                    {
                        sr.WriteLine($"Object with an ID {newData.ObjectID} failed to change it's Position:");
                        sr.WriteLine($"\t     AMSL: to {newData.AMSL}");
                        sr.WriteLine($"\tLongitude: to {newData.Longitude}");
                        sr.WriteLine($"\t Latitude: to {newData.Latitude}");
                    }
                    sr.Flush();
                }
            }
            catch (Exception ex) { }
        }

        public void LogMessageUpdateContactInfo(ContactInfoUpdateArgs oldData, ContactInfoUpdateArgs newData, bool success)
        {
            UpdateDay(DateTime.Now);
            try
            {
                using (StreamWriter sr = File.AppendText(_path))
                {
                    sr.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " the following changes has been made");
                    if (success)
                    {
                        sr.WriteLine($"Object with an ID {newData.ObjectID} changed succesfully it's Contact Info:");
                        sr.WriteLine($"\tEmail Address: from {oldData.EmailAddress} to {newData.EmailAddress}");
                        sr.WriteLine($"\t Phone Number: from {oldData.PhoneNumber} to {newData.PhoneNumber}");
                    }
                    else
                    {
                        sr.WriteLine($"Object with an ID {newData.ObjectID} failed to change it's Contact Info:");
                        sr.WriteLine($"\tEmail Address: to {newData.EmailAddress}");
                        sr.WriteLine($"\t Phone Number: to {newData.PhoneNumber}");
                    }
                    sr.Flush();
                }
            }
            catch (Exception ex) { }
        }

    }
}
