using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace SSJson
{
    public class CellSelected
    {
        [JsonProperty(PropertyName = "cellName")]
        private string cellName;

        [JsonProperty(PropertyName = "selector")]
        private int clientID;

        [JsonProperty(PropertyName = "selectorName")]
        private string clientName;

        public string getCellName()
        {
            return cellName;
        }

        public int getClientID()
        {
            return clientID;
        }

        public string getClientName()
        {
            return clientName;
        }


    }
}
