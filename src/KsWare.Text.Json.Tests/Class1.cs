using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KsWare.Text.Json.Tests;

internal class Class1 {

	[Test]
	public void GetElementType() {
		Console.WriteLine($"{typeof(string[]).GetElementType()}");
		Console.WriteLine($"{typeof(Array).GetElementType()}");
		Console.WriteLine($"{typeof(IEnumerable<int>).GetElementType()}");
	}

	[TestCase(";Comment")]
	public void ReadComment(string json) {
		var org =  JsonSerializer.Serialize(";Comment");
		var sut =  JsonSerializer.Serialize(";Comment", new JsonSerializerOptions{Converters = { DefaultObjectConverter.Default }});
		Assert.That(sut, Is.EqualTo(org));
	}

	[Test]
	public void GetConverter() {
		var options = new JsonSerializerOptions();
		var converter = options.GetConverter(typeof(object)); // internal System.Text.Json.Serialization.Converters.ObjectConverter
		WriteLine(converter.GetType().FullName);
		WriteLine($"  >"+converter.GetType().BaseType.FullName);

		converter = new JsonSerializerOptions().GetConverter(typeof(Array));
		WriteLine(converter.GetType().FullName);
		WriteLine($"  >"+converter.GetType().BaseType.FullName);

		converter = new JsonSerializerOptions().GetConverter(typeof(byte[]));
		WriteLine(converter.GetType().FullName);
		WriteLine($"  >"+converter.GetType().BaseType.FullName);

		converter = new JsonSerializerOptions().GetConverter(typeof(int));
		WriteLine(converter.GetType().FullName);
		WriteLine($"  >"+converter.GetType().BaseType.FullName);
		var c2 = (JsonConverter<int>) converter;
	}

	[Test]
	public void ForwardJsonConverterReadTest() {
		var sut=JsonSerializer.Deserialize("42",typeof(int), new JsonSerializerOptions {Converters = {new ForwardReflectionJsonConverter()}});
		Assert.That(sut, Is.EqualTo(42));
	}

	[Test]
	public void ForwardJsonConverterWriteTest() {
		var sut=JsonSerializer.Serialize(1, new JsonSerializerOptions {Converters = {new ForwardReflectionJsonConverter()}});
		Assert.That(sut, Is.EqualTo("1"));
	}


	private void WriteLine(string? s) {
		Console.WriteLine(s);
		Debug.WriteLine(s);
	}

}



