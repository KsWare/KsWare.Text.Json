using System.Text.Json;

namespace KsWare.Text.Json {

	internal static class Helper {

		public static JsonSerializerOptions Clone(this JsonSerializerOptions o) {
			var n = new JsonSerializerOptions {
				NumberHandling = o.NumberHandling,
				PropertyNamingPolicy = o.PropertyNamingPolicy,
				AllowTrailingCommas = o.AllowTrailingCommas,
				DefaultBufferSize = o.DefaultBufferSize,
				DefaultIgnoreCondition = o.DefaultIgnoreCondition,
				DictionaryKeyPolicy = o.DictionaryKeyPolicy,
				Encoder = o.Encoder,
				IgnoreReadOnlyFields = o.IgnoreReadOnlyFields,
				IgnoreReadOnlyProperties = o.IgnoreReadOnlyProperties,
				IncludeFields = o.IncludeFields,
				MaxDepth = o.MaxDepth,
				PropertyNameCaseInsensitive = o.PropertyNameCaseInsensitive,
				ReadCommentHandling = o.ReadCommentHandling,
				ReferenceHandler = o.ReferenceHandler,
				UnknownTypeHandling = o.UnknownTypeHandling,
				WriteIndented = o.WriteIndented
			};
			foreach (var c in o.Converters) n.Converters.Add(c);
			return n;
		}

	}
}
