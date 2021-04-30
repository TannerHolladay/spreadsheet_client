// Written by Tanner Holladay, Noah Carlson, Abbey Nelson, Sergio Remigio, Travis Schnider, Jimmy Glasscock for CS 3505 on April 28, 2021
using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace SSJson
{

    /// <summary>
    /// Class for Deserializing a Json selection message to an object
    /// </summary>
    public class CellSelected
    {
        [JsonProperty(PropertyName = "cellName")]
        private string _cellName;

        [JsonProperty(PropertyName = "selector")]
        private int _clientID;

        [JsonProperty(PropertyName = "selectorName")]
        private string _clientName;

        public string GetCellName()
        {
            return _cellName;
        }

        public int GetClientID()
        {
            return _clientID;
        }

        public string GetClientName()
        {
            return _clientName;
        }


    }
}
