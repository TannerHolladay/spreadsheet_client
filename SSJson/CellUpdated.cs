// Written by Tanner Holladay, Noah Carlson, Abbey Nelson, Sergio Remigio, Travis Schnider, Jimmy Glasscock for CS 3505 on April 28, 2021
using Newtonsoft.Json;
using System;

namespace SSJson
{
    public class CellUpdated
    {

        [JsonProperty(PropertyName = "cellName")]
        private string cellName;

        [JsonProperty(PropertyName = "contents")]
        private string contents;

        public string getCellName()
        {
            return cellName;
        }

        public string getContents()
        {
            return contents;
        }

    }
}
