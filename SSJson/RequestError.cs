// Written by Tanner Holladay, Noah Carlson, Abbey Nelson, Sergio Remigio, Travis Schnider, Jimmy Glasscock for CS 3505 on April 28, 2021
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSJson
{
    /// <summary>
    /// Class for Deserializing a RequestError message to an object
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class RequestError
    {
        [JsonProperty(PropertyName = "cellName")]
        private string cellName;

        [JsonProperty(PropertyName = "message")]
        private string message;

        public string getCellName()
        {
            return cellName;
        }

        public string getMessage()
        {
            return message;
        }

    }
}
