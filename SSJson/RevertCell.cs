using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSJson
{
    [JsonObject(MemberSerialization.OptIn)]
    public class RevertCell
    {
        [JsonProperty(PropertyName = "requestType")]
        private string requestType;

        [JsonProperty(PropertyName = "cellName")]
        private string cellName;

        public RevertCell()
        {
            requestType = "revertCell";
            cellName = "";
        }

        public void setCellName(string name)
        {
            cellName = name;
        }
    }
}
