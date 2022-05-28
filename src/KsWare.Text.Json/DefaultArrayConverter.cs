using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KsWare.Text.Json {

	public class DefaultArrayConverter : JsonConverter<IEnumerable> {

		public static DefaultArrayConverter Default = new DefaultArrayConverter();

		public override IEnumerable? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
			throw new NotImplementedException();
		}

		public override void Write(Utf8JsonWriter writer, IEnumerable value, JsonSerializerOptions options) {
			writer.WriteStartArray();

			foreach (var item in value) {
				JsonSerializer.Serialize(writer, item, options);
			}

			writer.WriteEndArray();
		}

	}
}
