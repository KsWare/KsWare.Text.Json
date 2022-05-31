using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KsWare.Text.Json.Tests; 

public class Int1Converter : JsonConverter<int> {

	public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
		reader.Read();
		return 1;
	}

	public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options) {
		writer.WriteNumberValue(1);
	}

}

public class Int2Converter : JsonConverter<int> {

	public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
		reader.Read();
		return 2;
	}

	public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options) {
		writer.WriteNumberValue(2);
	}

}