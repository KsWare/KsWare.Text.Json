using System;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace KsWare.Text.Json; 

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

	public static JsonConverter DetermineConverter(Type? parentClassType, Type runtimePropertyType, MemberInfo? memberInfo, JsonSerializerOptions options) {
		// member.MemberInfo.GetCustomAttribute<JsonConverterAttribute>()?.CreateConverter(member.MemberType);
		JsonConverter converter = null!;

		// Priority 1: attempt to get converter from JsonConverterAttribute on property.
		if (memberInfo != null) {
			Debug.Assert(parentClassType != null);

			JsonConverterAttribute? converterAttribute = (JsonConverterAttribute?)
				GetAttributeThatCanHaveMultiple(parentClassType!, typeof(JsonConverterAttribute), memberInfo);

			if (converterAttribute != null) {
				converter = GetConverterFromAttribute(converterAttribute, typeToConvert: runtimePropertyType,
					classTypeAttributeIsOn: parentClassType!, memberInfo, options);
			}
		}

		if (converter == null) {
			converter = GetConverterInternal(runtimePropertyType, options);
			Debug.Assert(converter != null);
		}

		if (converter is JsonConverterFactory factory) {
			converter = factory.CreateConverter(runtimePropertyType, options);

			// A factory cannot return null; GetConverterInternal checked for that.
			Debug.Assert(converter != null);
		}

		// TODO check
		// // User has indicated that either:
		// //   a) a non-nullable-struct handling converter should handle a nullable struct type or
		// //   b) a nullable-struct handling converter should handle a non-nullable struct type.
		// // User should implement a custom converter for the underlying struct and remove the unnecessary CanConvert method override.
		// // The serializer will automatically wrap the custom converter with NullableConverter<T>.
		// //
		// // We also throw to avoid passing an invalid argument to setters for nullable struct properties,
		// // which would cause an InvalidProgramException when the generated IL is invoked.
		// if (runtimePropertyType.IsValueType && converter.IsValueType &&
		//     (runtimePropertyType.IsNullableOfT() ^ converter.TypeToConvert.IsNullableOfT())) {
		// 	ThrowHelper.ThrowInvalidOperationException_ConverterCanConvertMultipleTypes(runtimePropertyType,
		// 		converter);
		// }

		return converter;
	}

	private static JsonConverter GetConverterInternal(Type typeToConvert, JsonSerializerOptions options) {
		Debug.Assert(typeToConvert != null);

		JsonConverter converter = null!;
		// if (_converters.TryGetValue(typeToConvert, out JsonConverter? converter)) {
		// 	Debug.Assert(converter != null);
		// 	return converter;
		// }

		// Priority 1: If there is a JsonSerializerContext, fetch the converter from there.
		// converter = _context?.GetTypeInfo(typeToConvert)?.PropertyInfoForTypeInfo?.ConverterBase;
		// TODO Sorry we can't support _context due internal restrictions

		// Priority 2: Attempt to get custom converter added at runtime.
		// Currently there is not a way at runtime to override the [JsonConverter] when applied to a property.
		foreach (JsonConverter item in options.Converters) {
			if (item.CanConvert(typeToConvert)) {
				converter = item;
				break;
			}
		}

		// Priority 3: Attempt to get converter from [JsonConverter] on the type being converted.
		if (converter == null) {
			JsonConverterAttribute? converterAttribute = (JsonConverterAttribute?)
				GetAttributeThatCanHaveMultiple(typeToConvert, typeof(JsonConverterAttribute));

			if (converterAttribute != null) {
				converter = GetConverterFromAttribute(converterAttribute, typeToConvert: typeToConvert,
					classTypeAttributeIsOn: typeToConvert, memberInfo: null, options);
			}
		}

		// Priority 4: Attempt to get built-in converter.
		// if (converter == null) {
		// 	if (s_defaultSimpleConverters == null || s_defaultFactoryConverters == null) {
		// 		// (De)serialization using serializer's options-based methods has not yet occurred, so the built-in converters are not rooted.
		// 		// Even though source-gen code paths do not call this method <i.e. JsonSerializerOptions.GetConverter(Type)>, we do not root all the
		// 		// built-in converters here since we fetch converters for any type included for source generation from the binded context (Priority 1).
		// 		Debug.Assert(s_defaultSimpleConverters == null);
		// 		Debug.Assert(s_defaultFactoryConverters == null);
		// 		ThrowHelper.ThrowNotSupportedException_BuiltInConvertersNotRooted(typeToConvert);
		// 		return null!;
		// 	}
		//
		// 	if (s_defaultSimpleConverters.TryGetValue(typeToConvert, out JsonConverter? foundConverter)) {
		// 		converter = foundConverter;
		// 	}
		// 	else {
		// 		foreach (JsonConverter item in s_defaultFactoryConverters) {
		// 			if (item.CanConvert(typeToConvert)) {
		// 				converter = item;
		// 				break;
		// 			}
		// 		}
		//
		// 		// Since the object and IEnumerable converters cover all types, we should have a converter.
		// 		Debug.Assert(converter != null);
		// 	}
		// }

		// Allow redirection for generic types or the enum converter.
		if (converter is JsonConverterFactory factory) {
			// converter = factory.GetConverterInternal(typeToConvert, this);
			converter = factory.CreateConverter(typeToConvert, options);

			// A factory cannot return null; GetConverterInternal checked for that.
			Debug.Assert(converter != null);
		}

		// Type converterTypeToConvert = converter.TypeToConvert;
		Type converterTypeToConvert = Helper.TypeToConvert(converter);

		// if (!converterTypeToConvert.IsAssignableFromInternal(typeToConvert)
		//     && !typeToConvert.IsAssignableFromInternal(converterTypeToConvert)) {
		// 	ThrowHelper.ThrowInvalidOperationException_SerializationConverterNotCompatible(converter.GetType(),
		// 		typeToConvert);
		// }

		// // Only cache the value once (de)serialization has occurred since new converters can be added that may change the result.
		// if (_haveTypesBeenCreated) {
		// 	// A null converter is allowed here and cached.
		//
		// 	// Ignore failure case here in multi-threaded cases since the cached item will be equivalent.
		// 	_converters.TryAdd(typeToConvert, converter);
		// }

		return converter;
	}

	private static Attribute? GetAttributeThatCanHaveMultiple(Type classType, Type attributeType,
		MemberInfo memberInfo) {
		object[] attributes = memberInfo.GetCustomAttributes(attributeType, inherit: false);
		return GetAttributeThatCanHaveMultiple(attributeType, classType, memberInfo, attributes);
	}

	private static Attribute? GetAttributeThatCanHaveMultiple(Type classType, Type attributeType) {
		object[] attributes = classType.GetCustomAttributes(attributeType, inherit: false);
		return GetAttributeThatCanHaveMultiple(attributeType, classType, null, attributes);
	}

	private static Attribute? GetAttributeThatCanHaveMultiple(Type attributeType, Type classType,
		MemberInfo? memberInfo, object[] attributes) {
		if (attributes.Length == 0) return null;

		if (attributes.Length == 1) return (Attribute) attributes[0];

		// ThrowHelper.ThrowInvalidOperationException_SerializationDuplicateAttribute(attributeType, classType, memberInfo);
		throw new InvalidOperationException("Duplicate attribute.");
		return default;
	}

	private static JsonConverter GetConverterFromAttribute(JsonConverterAttribute converterAttribute,
		Type typeToConvert, Type classTypeAttributeIsOn, MemberInfo? memberInfo, JsonSerializerOptions options) {
		JsonConverter? converter;

		Type? type = converterAttribute.ConverterType;
		if (type == null) {
			// Allow the attribute to create the converter.
			converter = converterAttribute.CreateConverter(typeToConvert);
			if (converter == null) {
				// ThrowHelper.ThrowInvalidOperationException_SerializationConverterOnAttributeNotCompatible(classTypeAttributeIsOn, memberInfo, typeToConvert);
				throw new InvalidOperationException("Converter on attribute is not compatible.");
			}
		}
		else {
			ConstructorInfo? ctor = type.GetConstructor(Type.EmptyTypes);
			if (!typeof(JsonConverter).IsAssignableFrom(type) || ctor == null || !ctor.IsPublic) {
				// ThrowHelper.ThrowInvalidOperationException_SerializationConverterOnAttributeInvalid(classTypeAttributeIsOn, memberInfo);
				throw new InvalidOperationException("Converter on attribute is invalid.");
			}

			converter = (JsonConverter) Activator.CreateInstance(type)!;
		}

		Debug.Assert(converter != null);
		if (!converter.CanConvert(typeToConvert)) {
			Type? underlyingType = Nullable.GetUnderlyingType(typeToConvert);
			if (underlyingType != null && converter.CanConvert(underlyingType)) {
				if (converter is JsonConverterFactory converterFactory) {
					converter = converterFactory.CreateConverter(underlyingType, options);
				}

				// Allow nullable handling to forward to the underlying type's converter.
				// return NullableConverterFactory.CreateValueConverter(underlyingType, converter);
				options.GetConverter(typeToConvert); //<== TODO implement more exact if possible
			}

			// ThrowHelper.ThrowInvalidOperationException_SerializationConverterOnAttributeNotCompatible(classTypeAttributeIsOn, memberInfo, typeToConvert);
			throw new InvalidOperationException("Converter on attribute is not compatible.");
		}

		return converter;
	}

	private static Type TypeToConvert(JsonConverter converter) {
		// replacement for converter.TypeToConvert which is internal (.NET6)
		var t = converter.GetType();
		while (t != null && !t.IsGenericType && t != typeof(object) &&
		       t.GetGenericTypeDefinition() != typeof(JsonConverter<>)) {
			t = t.BaseType;
		}
		if (t == null) throw new Exception();
		return t.GenericTypeArguments[0];
	}

}