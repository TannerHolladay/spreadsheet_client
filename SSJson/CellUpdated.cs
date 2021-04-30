// Written by Tanner Holladay, Noah Carlson, Abbey Nelson, Sergio Remigio, Travis Schnider, Jimmy Glasscock for CS 3505 on April 28, 2021

using Newtonsoft.Json;

namespace SSJson
{
    /// <summary>
    ///     Class for serializing a cell updated message to an object
    /// </summary>
    public class CellUpdated
    {
        [JsonProperty(PropertyName = "cellName")]
        private string _cellName;

        [JsonProperty(PropertyName = "contents")]
        private string _contents;

        public string GetCellName()
        {
            return _cellName;
        }

        public string GetContents()
        {
            return _contents;
        }
    }
}