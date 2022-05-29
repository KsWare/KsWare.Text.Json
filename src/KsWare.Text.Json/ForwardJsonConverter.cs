using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace KsWare.Text.Json {

	internal class ForwardJsonConverter : JsonConverter<object> {

		public override bool CanConvert(Type typeToConvert) => true;

		public override object? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
			if (typeToConvert == typeof(bool)) return JsonMetadataServices.BooleanConverter.Read(ref reader, typeToConvert, options);
			if (typeToConvert == typeof(byte)) return JsonMetadataServices.ByteConverter.Read(ref reader, typeToConvert, options);
			if (typeToConvert == typeof(byte[])) return JsonMetadataServices.ByteArrayConverter.Read(ref reader, typeToConvert, options);
			if (typeToConvert == typeof(char)) return JsonMetadataServices.CharConverter.Read(ref reader, typeToConvert, options);
			if (typeToConvert == typeof(DateTime)) return JsonMetadataServices.DateTimeConverter.Read(ref reader, typeToConvert, options);
			if (typeToConvert == typeof(DateTimeOffset)) return JsonMetadataServices.DateTimeOffsetConverter.Read(ref reader, typeToConvert, options);
			if (typeToConvert == typeof(double)) return JsonMetadataServices.DoubleConverter.Read(ref reader, typeToConvert, options);
			if (typeToConvert == typeof(decimal)) return JsonMetadataServices.DecimalConverter.Read(ref reader, typeToConvert, options);
			if (typeToConvert == typeof(Guid)) return JsonMetadataServices.GuidConverter.Read(ref reader, typeToConvert, options);
			if (typeToConvert == typeof(short)) return JsonMetadataServices.Int16Converter.Read(ref reader, typeToConvert, options);
			if (typeToConvert == typeof(int)) return JsonMetadataServices.Int32Converter.Read(ref reader, typeToConvert, options);
			if (typeToConvert == typeof(long)) return JsonMetadataServices.Int64Converter.Read(ref reader, typeToConvert, options);
			// if (typeToConvert == typeof(JsonElement)) return new JsonElementConverter());
			// if (typeToConvert == typeof(JsonDocument());)) return new JsonDocumentConverter());
			if (typeToConvert == typeof(object)) return JsonMetadataServices.ObjectConverter.Read(ref reader, typeToConvert, options);
			if (typeToConvert == typeof(sbyte)) return JsonMetadataServices.SByteConverter.Read(ref reader, typeToConvert, options);
			if (typeToConvert == typeof(float)) return JsonMetadataServices.SingleConverter.Read(ref reader, typeToConvert, options);
			if (typeToConvert == typeof(string)) return JsonMetadataServices.StringConverter.Read(ref reader, typeToConvert, options);
			if (typeToConvert == typeof(TimeSpan)) return JsonMetadataServices.TimeSpanConverter.Read(ref reader, typeToConvert, options);
			if (typeToConvert == typeof(ushort)) return JsonMetadataServices.UInt16Converter.Read(ref reader, typeToConvert, options);
			if (typeToConvert == typeof(uint)) return JsonMetadataServices.UInt32Converter.Read(ref reader, typeToConvert, options);
			if (typeToConvert == typeof(ulong)) return JsonMetadataServices.UInt64Converter.Read(ref reader, typeToConvert, options);
			if (typeToConvert == typeof(Uri)) return JsonMetadataServices.UriConverter.Read(ref reader, typeToConvert, options);
			if (typeToConvert == typeof(Version)) return JsonMetadataServices.VersionConverter.Read(ref reader, typeToConvert, options);
			return ForwardReflectionJsonConverter.Default.Read(ref reader, typeToConvert, options);
		}

		public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options) {
			var typeToConvert = value.GetType();
			if (typeToConvert == typeof(bool)) JsonMetadataServices.BooleanConverter.Write(writer, (bool) value, options);
			else if (typeToConvert == typeof(byte)) JsonMetadataServices.ByteConverter.Write(writer, (byte) value, options);
			else if (typeToConvert == typeof(byte[])) JsonMetadataServices.ByteArrayConverter.Write(writer, (byte[]) value, options);
			else if (typeToConvert == typeof(char)) JsonMetadataServices.CharConverter.Write(writer, (char) value, options);
			else if (typeToConvert == typeof(DateTime)) JsonMetadataServices.DateTimeConverter.Write(writer, (DateTime) value, options);
			else if (typeToConvert == typeof(DateTimeOffset)) JsonMetadataServices.DateTimeOffsetConverter.Write(writer, (DateTimeOffset)value, options);
			else if (typeToConvert == typeof(double)) JsonMetadataServices.DoubleConverter.Write(writer, (double) value, options);
			else if (typeToConvert == typeof(decimal)) JsonMetadataServices.DecimalConverter.Write(writer, (decimal) value, options);
			else if (typeToConvert == typeof(Guid)) JsonMetadataServices.GuidConverter.Write(writer, (Guid) value, options);
			else if (typeToConvert == typeof(short)) JsonMetadataServices.Int16Converter.Write(writer, (short) value, options);
			else if (typeToConvert == typeof(int)) JsonMetadataServices.Int32Converter.Write(writer, (int) value, options);
			else if (typeToConvert == typeof(long)) JsonMetadataServices.Int64Converter.Write(writer, (long) value, options);
			// else if (typeToConvert == typeof(JsonElement)) new JsonElementConverter());
			// else if (typeToConvert == typeof(JsonDocument());)) new JsonDocumentConverter());
			else if (typeToConvert == typeof(object)) JsonMetadataServices.ObjectConverter.Write(writer, value, options);
			else if (typeToConvert == typeof(sbyte)) JsonMetadataServices.SByteConverter.Write(writer, (sbyte) value, options);
			else if (typeToConvert == typeof(float)) JsonMetadataServices.SingleConverter.Write(writer, (float) value, options);
			else if (typeToConvert == typeof(string)) JsonMetadataServices.StringConverter.Write(writer, (string) value, options);
			else if (typeToConvert == typeof(TimeSpan)) JsonMetadataServices.TimeSpanConverter.Write(writer, (TimeSpan) value, options);
			else if (typeToConvert == typeof(ushort)) JsonMetadataServices.UInt16Converter.Write(writer, (ushort) value, options);
			else if (typeToConvert == typeof(uint)) JsonMetadataServices.UInt32Converter.Write(writer, (uint) value, options);
			else if (typeToConvert == typeof(ulong)) JsonMetadataServices.UInt64Converter.Write(writer, (ulong) value, options);
			else if (typeToConvert == typeof(Uri)) JsonMetadataServices.UriConverter.Write(writer, (Uri) value, options);
			else if (typeToConvert == typeof(Version)) JsonMetadataServices.VersionConverter.Write(writer, (Version) value, options);
			ForwardReflectionJsonConverter.Default.Write(writer, (Version) value, options);
		}

	}
}
