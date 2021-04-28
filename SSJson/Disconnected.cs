using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSJson
{
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
