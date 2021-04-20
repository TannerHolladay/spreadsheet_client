using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSJson
{
    [JsonObject(MemberSerialization.OptIn)]
    public class EditCell
    {
        [JsonProperty(PropertyName = "requestType")]
        private string requestType;

        [JsonProperty(PropertyName = "cellName")]
        private string cellName;

        [JsonProperty(PropertyName = "contents")]
        private string contents;

        public EditCell()
        {
            requestType = "editCell";
            cellName = "";
            contents = "";
        }

        public void setCellName(string name)
        {
            cellName = name;
        }

        public void setContents(string value)
        {
            contents = value;
        }

    }
}
