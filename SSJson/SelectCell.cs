// Written by Tanner Holladay, Noah Carlson, Abbey Nelson, Sergio Remigio, Travis Schnider, Jimmy Glasscock for CS 3505 on April 28, 2021

using Newtonsoft.Json;

namespace SSJson
{
    /// <summary>
    ///     Class for serializing a selection message to an object
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class SelectCell
    {
        [JsonProperty(PropertyName = "cellName")]
        private string _cellName;

        [JsonProperty(PropertyName = "requestType")]
        private string _requestType;

        public SelectCell()
        {
            _requestType = "selectCell";
            _cellName = "";
        }

        public void SetCellName(string name)
        {
            _cellName = name;
        }
    }
}