using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace KsWare.Text.Json;

public class ForwardReflectionJsonConverter : JsonConverter<object> {

	private JsonSerializerOptions defaultOptions=new JsonSerializerOptions();

	public static readonly ForwardReflectionJsonConverter Default = new ForwardReflectionJsonConverter();

	public override bool CanConvert(Type typeToConvert) => true; // catch all

	public override object? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
		var converter = defaultOptions.GetConverter(typeToConvert);
		var ct = converter.GetType();
		var method = ct.GetMethod("Read", BindingFlags.Instance | BindingFlags.Public);
		
		//var p = new object[] {reader, typeToConvert, options}; // does not work because Utf8JsonReader is a ref struct 
		//return method.Invoke(c,p);

		var parameter = new[] {
			Expression.Parameter(typeof(Utf8JsonReader), "a"),
			Expression.Parameter(typeof(Type), "b"),
			Expression.Parameter(typeof(JsonSerializerOptions), "c")
		};
		var call = Expression.Call(Expression.Constant(converter), method, parameter); 
		var convert = Expression.Convert(call, typeof(object));
		var expression = Expression.Lambda<readDelegate>(convert, parameter);
		var func = expression.Compile();
		var result = func(reader, typeToConvert, options);
		return result;
	}

	delegate object readDelegate(Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options);
	

	public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options) {
		var converter = defaultOptions.GetConverter(value.GetType());
		var ct = converter.GetType();
		var method = ct.GetMethod("Write", BindingFlags.Instance | BindingFlags.Public);
		
		var parameter = new object[] {writer, value, options};
		method.Invoke(converter, parameter);

		// //TODO some conversion is missing here
		// var parameter = new[] {
		// 	Expression.Parameter(typeof(Utf8JsonWriter), "a"),
		// 	Expression.Parameter(typeof(object), "b"),
		// 	Expression.Parameter(typeof(JsonSerializerOptions), "c")
		// };
		// var call = Expression.Call(Expression.Constant(converter), method, parameter);
		// var expression = Expression.Lambda<writedDelegate>(call, parameter);
		// var func = expression.Compile();
		// func(writer, value, options);
	}

	delegate void writedDelegate(Utf8JsonWriter writer, object value, JsonSerializerOptions options);

}