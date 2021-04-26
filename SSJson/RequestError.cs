using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSJson
{
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
