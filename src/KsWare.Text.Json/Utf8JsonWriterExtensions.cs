using System.Text.Json;

namespace KsWare.Text.Json;

public static class Utf8JsonWriterExtensions {

	public static void WriteConditional(this Utf8JsonWriter writer, bool condition, string propertyName, object value, JsonSerializerOptions options) {
		if (!condition) return;
		writer.WritePropertyName(propertyName);
		JsonSerializer.Serialize(writer, value, options);
	}

}