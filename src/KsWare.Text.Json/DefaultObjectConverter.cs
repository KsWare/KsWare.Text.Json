using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace KsWare.Text.Json {

	// Because all useful things of System.Text.JsonConverter are internal we have to reinvent the wheel :-( 
	// But while we are at it, we might as well make a few things better
	// and for time/technical reasons, unfortunately, some things also worse and here you are asked to make this implementation better.

	public partial class DefaultObjectConverter: CustomJsonConverter<object> {

		public static readonly DefaultObjectConverter Default = new DefaultObjectConverter();

		// OPTIONAL use/create JsonDefaultValueAttribute

		public override bool CanConvert(Type typeToConvert) => true; // OPTIONAL include/exclude list
		
		public override void Write(Utf8JsonWriter writer, object? value, JsonSerializerOptions options) {
			var t = value != null ? value.GetType() : null;
			switch (value) {
				case null: writer.WriteNullValue(); break;

				case bool b: writer.WriteBooleanValue(b); break;

				case char c: writer.WriteStringValue(c.ToString()); break;
				case string s: writer.WriteStringValue(s); break;

				case decimal d: writer.WriteNumberValue(d); break;
				case long i8: writer.WriteNumberValue(i8); break;
				case int i4: writer.WriteNumberValue(i4); break;
				case short i2: writer.WriteNumberValue(i2); break;
				case byte i1: writer.WriteNumberValue(i1); break;
				case ulong u8: writer.WriteNumberValue(u8); break;
				case uint u4: writer.WriteNumberValue(u4); break;
				case ushort u2: writer.WriteNumberValue(u2); break;
				case sbyte s1: writer.WriteNumberValue(s1); break;
				case float f4: writer.WriteNumberValue(f4); break;
				case double f8:  writer.WriteNumberValue(f8); break;

				case DateTime dt: writer.WriteStringValue(Convert.ToString(dt, CultureInfo.InvariantCulture)); break;
				case DateOnly dx: writer.WriteStringValue(Convert.ToString(dx, CultureInfo.InvariantCulture)); break;
				case TimeOnly tx: writer.WriteStringValue(Convert.ToString(tx, CultureInfo.InvariantCulture)); break;
				case TimeSpan ts: writer.WriteStringValue(Convert.ToString(ts, CultureInfo.InvariantCulture)); break;

				case object o when t.IsEnum: writer.WriteStringValue($"{value}"); break;
				case object o when o is IEnumerable e: WriteArray(writer, e, options); break;

				default: WriteObject(writer, value, options); break;
			}
		}

		private void WriteArray(Utf8JsonWriter writer, IEnumerable enumerable, JsonSerializerOptions options) {
			writer.WriteStartArray();

			foreach (var item in enumerable) {
				JsonSerializer.Serialize(writer, item, options);
			}

			writer.WriteEndArray();
		}

		private void WriteObject(Utf8JsonWriter writer, object? value, JsonSerializerOptions options) {
			writer.WriteStartObject();

			if (value != null) {
				var t = value.GetType();
				// t.GetCustomAttribute<JsonSerializableAttribute>();
				var numberHandling = t.GetCustomAttribute<JsonNumberHandlingAttribute>()?.Handling ?? options.NumberHandling;
				var converter = t.GetCustomAttribute<JsonConverterAttribute>()?.CreateConverter(t);
				// t.GetCustomAttribute<JsonSourceGenerationOptionsAttribute>()
				
				foreach (var member in GetMembersForSerialize(typeof(object), value, options)) {
					WriteMember(writer, member, options);
				}
			}

			writer.WriteEndObject();
		}

		private void WriteMember(Utf8JsonWriter writer, Member member, JsonSerializerOptions options) {
			var t = member.Value != null ? member.Value.GetType() : null;
			switch (member.Value) {
				case null: writer.WriteNull(member.Name); break;

				case bool b: writer.WriteBoolean(member.Name, b); break;

				case char c: writer.WriteString(member.Name, c.ToString()); break;
				case string s: writer.WriteString(member.Name, s); break;

				case decimal d: writer.WriteNumber(member.Name, d); break;
				case long i8: writer.WriteNumber(member.Name, i8); break;
				case int i4: writer.WriteNumber(member.Name, i4); break;
				case short i2: writer.WriteNumber(member.Name, i2); break;
				case byte i1: writer.WriteNumber(member.Name, i1); break;
				case ulong u8: writer.WriteNumber(member.Name, u8); break;
				case uint u4: writer.WriteNumber(member.Name, u4); break;
				case ushort u2: writer.WriteNumber(member.Name, u2); break;
				case sbyte s1: writer.WriteNumber(member.Name, s1); break;
				case float f4: writer.WriteNumber(member.Name, f4); break;
				case double f8:  writer.WriteNumber(member.Name, f8); break;

				// simple types
				case DateTime dt: writer.WriteString(member.Name, Convert.ToString(dt, CultureInfo.InvariantCulture)); break;
				case DateOnly dx: writer.WriteString(member.Name, Convert.ToString(dx, CultureInfo.InvariantCulture)); break;
				case TimeOnly tx: writer.WriteString(member.Name, Convert.ToString(tx, CultureInfo.InvariantCulture)); break;
				case TimeSpan ts: writer.WriteString(member.Name, Convert.ToString(ts, CultureInfo.InvariantCulture)); break;
				// TODO use list of simple types, or more generic Convert.ToString/FromString

				case object when t.IsEnum: writer.WriteString(member.Name, $"{member.Value}"); break;
				case object o when o is IEnumerable e: DefaultArrayConverter.Default.Write(writer, e, options); break;
				default: Write(writer, member.Value, options); break;
			}
		}

		private static object GetDefault(Type type) {
			return type.IsValueType ? Activator.CreateInstance(type) : null;
		}

	}
}