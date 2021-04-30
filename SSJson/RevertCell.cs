// Written by Tanner Holladay, Noah Carlson, Abbey Nelson, Sergio Remigio, Travis Schnider, Jimmy Glasscock for CS 3505 on April 28, 2021using Newtonsoft.Json;

using Newtonsoft.Json;

namespace SSJson
{
    /// <summary>
    ///     Class for serializing a revert message to an object
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class RevertCell
    {
        [JsonProperty(PropertyName = "cellName")]
        private string _cellName;

        [JsonProperty(PropertyName = "requestType")]
        private string _requestType;

        public RevertCell()
        {
            _requestType = "revertCell";
            _cellName = "";
        }

        public void SetCellName(string name)
        {
            _cellName = name;
        }
    }
}