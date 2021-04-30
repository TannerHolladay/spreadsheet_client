// Written by Tanner Holladay, Noah Carlson, Abbey Nelson, Sergio Remigio, Travis Schnider, Jimmy Glasscock for CS 3505 on April 28, 2021

using Newtonsoft.Json;

namespace SSJson

{
    /// <summary>
    ///     Class for Deserializing a Json Edit message to an object
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class EditCell
    {
        [JsonProperty(PropertyName = "cellName")]
        private string _cellName;

        [JsonProperty(PropertyName = "contents")]
        private string _contents;

        [JsonProperty(PropertyName = "requestType")]
        private string _requestType;

        public EditCell()
        {
            _requestType = "editCell";
            _cellName = "";
            _contents = "";
        }

        public void SetCellName(string name)
        {
            _cellName = name;
        }

        public void SetContents(string value)
        {
            _contents = value;
        }
    }
}