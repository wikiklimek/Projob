using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using FlightTrackerGUI;
using NetworkSourceSimulator;

namespace ProjOb
{

    internal class Program
    {
        static void Main(string[] args)
        {
            //Wybranie pliku i nazwy pliku
            string directory = Directory.GetCurrentDirectory();
            string pathMessages = Path.Combine(directory, "example_data.ftr");
            string pathUpdates = Path.Combine(directory, "example.ftre");

            //etap4
            Interface.MyProgram(pathMessages, pathUpdates);
        }

    }
}