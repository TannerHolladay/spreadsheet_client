using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSJson
{
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
