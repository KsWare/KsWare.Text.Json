using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KsWare.Text.Json; 

partial class CustomJsonConverter<T> /*Reader*/ {

	public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
		throw new NotImplementedException();
		// TODO continue here
	}



	protected bool IsExcluded(Member member, JsonSerializerOptions options) {
		var jsonIgnore = member.MemberInfo.GetCustomAttribute<JsonIgnoreAttribute>();
		var condition = jsonIgnore != null ? jsonIgnore.Condition : options.DefaultIgnoreCondition;
		switch (condition) {
			case JsonIgnoreCondition.Always: return true;
			case JsonIgnoreCondition.Never: return false;
			case JsonIgnoreCondition.WhenWritingNull: return IsReferenceType(member.MemberInfo) && member.Value == null;
			case JsonIgnoreCondition.WhenWritingDefault: return IsDefaultValue(member.MemberInfo, member.Value);
			default: return false;
		}
	}

	private bool IsDefaultValue(MemberInfo memberInfo, object value) {
		var returnType = GetReturnType(memberInfo);
		object defaultValue;
		var defaultValueAttribute = SupportDefaultValueAttribute ? memberInfo.GetCustomAttribute<DefaultValueAttribute>() : null;
		if (defaultValueAttribute != null) {
			defaultValue = defaultValueAttribute.Value;
		}
		else if(returnType.IsValueType) {
			defaultValue = Activator.CreateInstance(returnType);
		}
		else {
			defaultValue = null;
		}
		return Equals(value, defaultValue); // TODO verify 
	}

	private static Type GetReturnType(MemberInfo memberInfo) {
		if (memberInfo is FieldInfo fi) return fi.FieldType;
		if (memberInfo is PropertyInfo pi) return pi.PropertyType;
		throw new NotSupportedException();
	}

	private static bool IsReferenceType(MemberInfo memberInfo) {
		return GetReturnType(memberInfo).IsValueType == false;
	}

	protected static bool IsReadOnly(PropertyInfo propertyInfo) {
		if (propertyInfo.CanWrite) return true;
		return false;
	}

	protected static bool IsReadOnly(FieldInfo fieldInfo) {
		if (fieldInfo.IsInitOnly) return true; //???
		return false;
	}

}