﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSJson
{
    [JsonObject(MemberSerialization.OptIn)]
    public class UndoCell
    {
        [JsonProperty(PropertyName = "requestType")]
        private string requestType;

        public UndoCell()
        {
            requestType = "undo";
        }

    }
}
