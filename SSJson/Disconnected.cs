// Written by Tanner Holladay, Noah Carlson, Abbey Nelson, Sergio Remigio, Travis Schnider, Jimmy Glasscock for CS 3505 on April 28, 2021
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSJson
{
    /// <summary>
    /// Class for Deserializing a client disconnection message to an object
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Disconnected
    {

        [JsonProperty(PropertyName = "user")]
        private int userID;

        public int getUserID()
        {
            return userID;
        }

    }
}
