﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KsWare.Text.Json; 

public partial class CustomJsonConverter<T> : JsonConverter<T> {

	public bool SupportDefaultValueAttribute { get; set; }

}