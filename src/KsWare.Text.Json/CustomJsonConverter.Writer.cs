using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KsWare.Text.Json; 

public partial class CustomJsonConverter<T> : JsonConverter<T> {
	
	public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options) {
		if (value == null) { writer.WriteNullValue(); return; }
		var declaringType = typeof(T);
		var runtimeType = value.GetType();

		var members = GetMembersForSerialize(runtimeType, value, options);
		writer.WriteStartObject();
		foreach (var member in members) WriteMember(member, options);
		writer.WriteEndObject();
	}

	protected virtual void WriteMember(Member member, JsonSerializerOptions options) {
		
	}

	protected virtual IEnumerable<Member> GetMembersForSerialize(Type type, object? value, JsonSerializerOptions options) {
		var result = new List<Member>();
		if (value == null) return result;
		type = value.GetType();
		var members = type.GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
			.Where(mi => mi.MemberType == MemberTypes.Field || mi.MemberType == MemberTypes.Property); //TODO
		foreach (var mi in members) {
			var member = new Member(mi);
			switch (mi.MemberType) {
				case MemberTypes.Field:
					var fi = (FieldInfo) mi;
					if (options.IncludeFields == false) continue;
					if (options.IgnoreReadOnlyFields && IsReadOnly(fi)) continue;
					member.Value = fi.GetValue(value);
					member.MemberType = fi.FieldType;
					break;
				case MemberTypes.Property:
					var pi = (PropertyInfo) mi;
					if (options.IgnoreReadOnlyProperties && IsReadOnly(pi)) continue;
					member.Value = pi.GetValue(value);
					member.MemberType = pi.PropertyType;
					// When applied to a property, indicates that non-public getters and setters can be used for serialization and deserialization. Non-public properties are not supported.
					var includePrivate = mi.GetCustomAttribute<JsonIncludeAttribute>()!=null; // TODO
					break;
				default:
					continue;
			}

			if (IsExcluded(member, options)) continue;
			member.Converter = DetermineConverter(type, value?.GetType(), member.MemberInfo, options);
			member.IsExtensionData = mi.GetCustomAttribute<JsonExtensionDataAttribute>() != null; //TODO check member type is dictionary
			member.NumberHandling = mi.GetCustomAttribute<JsonNumberHandlingAttribute>()?.Handling;
			member.Name = mi.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name ?? ConvertName(mi.Name, options);
			member.Order = mi.GetCustomAttribute<JsonPropertyOrderAttribute>()?.Order ?? 0;
				
			result.Add(member);
		}
		var order = 0;
		result.ForEach(m => m.OriginalOrder = order++);
		result.Sort((Comparison<Member>) MemberSort);

		return result;
	}

	protected virtual JsonConverter DetermineConverter(Type? parentClassType, Type runtimePropertyType, MemberInfo? memberInfo, JsonSerializerOptions options)
		=> Helper.DetermineConverter(parentClassType, runtimePropertyType, memberInfo, options);
	

	protected int MemberSort(Member a, Member b) {
		var v = a.Order.CompareTo(b.Order);
		return v!=0 ? v : a.OriginalOrder.CompareTo(b.OriginalOrder);
	}

	protected string ConvertName(string name, JsonSerializerOptions options) {
		return options.PropertyNamingPolicy?.ConvertName(name) ?? name;
	}

}