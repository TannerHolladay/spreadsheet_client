// Written by Tanner Holladay, Noah Carlson, Abbey Nelson, Sergio Remigio, Travis Schnider, Jimmy Glasscock for CS 3505 on April 28, 2021
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSJson
{
    /// <summary>
    /// Class for Deserializing a server shutdown message to an object
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class ServerShutdownError
    {

        [JsonProperty(PropertyName = "message")]
        private string message;

        public string getMessage()
        {
            return message;
        }

    }
}
