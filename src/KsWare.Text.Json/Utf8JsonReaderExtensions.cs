using System.Text.Json;

namespace KsWare.Text.Json;

public static class Utf8JsonReaderExtensions {

	public static T ReadValue<T>(this ref Utf8JsonReader reader, JsonSerializerOptions options) {
		var value = JsonSerializer.Deserialize<T>(ref reader, options);
		reader.Read();
		return value;
	}

	public static bool TryReadProperty<T>(this ref Utf8JsonReader reader, out string propertyName, out T value, JsonSerializerOptions options) {
		if (reader.TokenType != JsonTokenType.PropertyName) {
			//throw new JsonException($"PropertyName expected but {reader.TokenType} found.");
			propertyName = null;
			value = default;
			return false;
		}
		propertyName = reader.GetString();
		reader.Read();
		value = JsonSerializer.Deserialize<T>(ref reader, options);
		return true;
	}

	public static bool TryReadPropertyName(this ref Utf8JsonReader reader, out string propertyName) {
		if (reader.TokenType != JsonTokenType.PropertyName) {
			//throw new JsonException($"PropertyName expected but {reader.TokenType} found.");
			propertyName = null;
			return false;
		}
		propertyName = reader.GetString();
		reader.Read();
		return true;
	}

	public static string ReadPropertyName(this ref Utf8JsonReader reader) {
		if (reader.TokenType != JsonTokenType.PropertyName)
			throw new JsonException($"PropertyName expected but {reader.TokenType} found.");
			
		var propertyName = reader.GetString();
		reader.Read();
		return propertyName;
	}

	public static void ReadStartObject(this ref Utf8JsonReader reader) => ReadToken(ref reader, JsonTokenType.StartObject);
	public static void ReadEndObject(this ref Utf8JsonReader reader) => ReadToken(ref reader, JsonTokenType.EndObject);
	public static void ReadStartArray(this ref Utf8JsonReader reader) => ReadToken(ref reader, JsonTokenType.StartArray);
	public static void ReadEndArray(this ref Utf8JsonReader reader) => ReadToken(ref reader, JsonTokenType.EndArray);
	
	public static void ReadToken(this ref Utf8JsonReader reader, JsonTokenType expectedJsonTokenType) {
		if (reader.TokenType != expectedJsonTokenType)
			throw new JsonException($"{expectedJsonTokenType} expected but {reader.TokenType} found.");
		reader.Read();
	}

}