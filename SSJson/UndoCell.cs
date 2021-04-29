﻿// Written by Tanner Holladay, Noah Carlson, Abbey Nelson, Sergio Remigio, Travis Schnider, Jimmy Glasscock for CS 3505 on April 28, 2021
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSJson
{
    /// <summary>
    /// Class for Deserializing a undo message to an object
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class UndoCell
    {
        [JsonProperty(PropertyName = "requestType")]
        private string requestType;

        public UndoCell()
        {
            requestType = "undo";
        }

    }
}
