using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KsWare.Text.Json {

	partial class DefaultObjectConverter/*Reader*/ {

		public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
			switch (reader.TokenType) {
				case JsonTokenType.Comment: reader.Read(); return null; //TODO read comment as typeToConvert
				case JsonTokenType.Null: return ReadNull(ref reader, typeToConvert, options);
				case JsonTokenType.False: case JsonTokenType.True: return ReadBoolean(ref reader, typeToConvert, options);
				case JsonTokenType.Number: return ReadNumber(ref reader, typeToConvert, options);
				case JsonTokenType.String: return ReadString(ref reader, typeToConvert, options);
				case JsonTokenType.StartArray: return ReadArray(ref reader, typeToConvert, options);
				case JsonTokenType.StartObject: return ReadObject(ref reader, typeToConvert, options);
				// case JsonTokenType.PropertyName: break;
				default: throw new JsonException();
			}
		}

		private object ReadNull(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
			reader.Read();
			var converter = options.GetConverter(typeToConvert); // TODO use converter
			var value = GetDefault(typeToConvert);
			return value;
		}

		private object ReadBoolean(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
			bool value;
			switch (reader.TokenType) {
				case JsonTokenType.False: value = false; reader.Read(); break;
				case JsonTokenType.True: value = true; reader.Read(); break;
				default: throw new JsonException();
			}
			var converter = options.GetConverter(typeToConvert); // TODO use converter
			return System.Convert.ChangeType(value,typeToConvert);
		}

		private object ReadNumber(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
			var value = reader.GetString(); reader.Read();
			var converter = options.GetConverter(typeToConvert); // TODO use converter
			var typeCode = Type.GetTypeCode(typeToConvert);
			switch (typeCode) {
				// case TypeCode.Object: {
				// 	//TODO create instance
				// } break;
				default:
					return System.Convert.ChangeType(value,typeToConvert);
			}
		}

		private object ReadString(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
			var value = reader.GetString(); reader.Read();

			var converter = options.GetConverter(typeToConvert); // TODO use converter

			if(typeToConvert == typeof(string)) return value;
			if(typeToConvert == typeof(object)) return value;
			if(typeToConvert == typeof(DateTime)) return DateTime.Parse(value);
			if(typeToConvert == typeof(DateOnly)) return DateOnly.Parse(value);
			if(typeToConvert == typeof(TimeOnly)) return TimeOnly.Parse(value);
			if(typeToConvert == typeof(TimeSpan)) return TimeSpan.Parse(value);
			return System.Convert.ChangeType(value,typeToConvert);
		}

		private object ReadArray(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
			reader.ReadStartArray();

			var elementType = GetElementType(typeToConvert);

			var instance = Activator.CreateInstance(typeToConvert); // TODO revise temporary code

			var list = new ArrayList();
			var dic = new Dictionary<string, object>();

			while (reader.TokenType != JsonTokenType.EndArray) {
				switch (reader.TokenType) {
					case JsonTokenType.Comment: reader.Read(); break;// TODO store comment
					case JsonTokenType.PropertyName: {
						var propertyName = reader.ReadPropertyName();
						var value = Read(ref reader, elementType, options); // recursive call
						dic.Add(propertyName, value);
					} break;

					case JsonTokenType.None:
					case JsonTokenType.EndObject: throw new JsonException();

					default: {
						var value = Read(ref reader, elementType, options); // recursive call
						list.Add(value);
					} break;
				}
			}

			// DON'T reader.ReadEndArray();
			return instance;
		}

		private Type GetElementType(Type typeToConvert) {
			if (typeToConvert.IsArray) return typeToConvert.GetElementType() ?? throw new ArgumentException();

			//TODO validate the strategy to detect the element type

			if (typeToConvert.IsGenericType) {
				// TODO this will not check if the type really a list/collection
				if (typeToConvert.GenericTypeArguments.Length == 1) return typeToConvert.GenericTypeArguments[0];
				throw new ArgumentException();
			}
			// assuming that each object implementing IEnumerable has items
			var enumerableType = typeToConvert.GetInterfaces().FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>));
			if (enumerableType != null) return enumerableType.GenericTypeArguments[0];
			if (typeToConvert.GetInterface(nameof(IEnumerable)) != null) return typeof(object);
			// TODO inconclusive:  MyList : List<TypeA>, IEnumerable<TypeB>

			return typeof(object); // throw exception?
		}

		private object ReadObject(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
			var obj = CreateInstanceFrom(ref reader, typeToConvert, options);
			var t = obj.GetType();
			reader.ReadStartObject();
			
			var members = GetMembersForDeserialize(t, obj, options); // or use typeToConvert ??
			var extensionData = GetExtensionData(members);

			while (reader.TokenType != JsonTokenType.EndArray) {
				var propertyName = reader.ReadPropertyName();
				var member = FindMember(propertyName, members, options);
				var value = Read(ref reader, member.MemberType ?? typeof(object), options); // recursive call
				if (member == null) 
					extensionData.Add(propertyName,value);
				else 
					member.SetValue(value);
			}

			// DON'T reader.ReadEndObject();
			return obj;
		}

		private IDictionary<string, object> GetExtensionData(IEnumerable<Member> members) {
			var m = members.FirstOrDefault(m => m.IsExtensionData);
			if (m == null) return new Dictionary<string, object>();
			if (m.Value != null) return (IDictionary<string, object>) m.Value;
			var dic = new Dictionary<string, object>();
			m.SetValue(dic);
			return dic;
		}

		private Member FindMember(string propertyName, IEnumerable<Member> members, JsonSerializerOptions options) {
			// TODO continue here
			return null;
		}

		private IList<Member> GetMembersForDeserialize(Type type, object instance, JsonSerializerOptions options) {
			if (type == null) throw new ArgumentNullException(nameof(type));
			if (instance == null) throw new ArgumentNullException(nameof(instance));
			var result = new List<Member>();

			var memberInfos = type.GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
				.Where(mi => mi.MemberType is MemberTypes.Field or MemberTypes.Property); //TODO
			foreach (var mi in memberInfos) {
				var member = new Member(mi, instance);
				switch (mi.MemberType) {
					case MemberTypes.Field:
						var fi = (FieldInfo) mi;
						if (options.IncludeFields == false) continue;
						if (options.IgnoreReadOnlyFields && IsReadOnly(fi)) continue;
						member.Value = fi.GetValue(instance);
						member.MemberType = fi.FieldType;
						break;
					case MemberTypes.Property:
						var pi = (PropertyInfo) mi;
						if (options.IgnoreReadOnlyProperties && IsReadOnly(pi)) continue;
						member.Value = pi.GetValue(instance);
						member.MemberType = pi.PropertyType;
						// When applied to a property, indicates that non-public getters and setters can be used for serialization and deserialization. Non-public properties are not supported.
						var includePrivate = mi.GetCustomAttribute<JsonIncludeAttribute>() != null; //TODO
						break;
					default:
						continue;
				}

				if (IsExcluded(member, options)) continue;
				// member.Converter = mi.GetCustomAttribute<JsonConverterAttribute>()?.CreateConverter(member.MemberType); //TODO cache
				member.IsExtensionData = mi.GetCustomAttribute<JsonExtensionDataAttribute>() != null; //TODO check member type is dictionary
				member.NumberHandling = mi.GetCustomAttribute<JsonNumberHandlingAttribute>()?.Handling;
				member.Name = mi.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name ?? ConvertName(mi.Name, options);
				member.Order = mi.GetCustomAttribute<JsonPropertyOrderAttribute>()?.Order ?? 0;
				
				result.Add(member);
			}
			var order = 0;
			result.ForEach(m => m.OriginalOrder = order++);
			result.Sort(MemberSort);

			return result;
		}

		protected virtual object CreateInstanceFrom(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
			if (typeToConvert.IsInterface) 
				typeToConvert = ResolveTypeforInterface(ref reader, typeToConvert, options);

			return Activator.CreateInstance(typeToConvert); 
		}

		protected virtual Type ResolveTypeforInterface(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
			// OPTIONAL use a TypeResolver (DefaultObjectConverter.TypeResolver property)

			var tmpReader = reader;
			tmpReader.ReadStartObject();
			if(tmpReader.TryReadProperty<string>(out var propertyName, out var propertyValue, options)) {
				if (propertyValue.Equals("@AQTN", StringComparison.OrdinalIgnoreCase)) {
					var t = Type.GetType(propertyValue, true);
					return t;
				}
			}
			throw new JsonException();
		}
	}
}
