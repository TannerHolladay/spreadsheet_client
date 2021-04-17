using Newtonsoft.Json;
using System;

namespace SSJson
{
    public class CellUpdated
    {
        //[JsonProperty(PropertyName = "messageType")]
        //private string messageType;

        [JsonProperty(PropertyName = "cellName")]
        private string cellName;

        [JsonProperty(PropertyName = "contents")]
        private string contents;

        public CellUpdated()
        {
            cellName = "";
            contents = "";
        }

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
