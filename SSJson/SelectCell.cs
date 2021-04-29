// Written by Tanner Holladay, Noah Carlson, Abbey Nelson, Sergio Remigio, Travis Schnider, Jimmy Glasscock for CS 3505 on April 28, 2021
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSJson
{
    [JsonObject(MemberSerialization.OptIn)]
    public class SelectCell
    {
        [JsonProperty(PropertyName = "requestType")]
        private string requestType;

        [JsonProperty(PropertyName = "cellName")]
        private string cellName;

        public SelectCell()
        {
            requestType = "selectCell";
            cellName = "";
        }

        public void setCellName(string name)
        {
            cellName = name;
        }
    }
}
