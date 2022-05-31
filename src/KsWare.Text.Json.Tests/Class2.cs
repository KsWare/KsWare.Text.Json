using System.Drawing;

namespace KsWare.Text.Json.Tests; 

internal class Class2 {

	[Test]
	public void Test1() {
		var sut = JsonSerializer.Serialize(new MyClass());
		Assert.That(sut, Is.EqualTo("{\"MyInt\":1}"));

		// explicit converter has priority
		sut = JsonSerializer.Serialize(new MyClass(),new JsonSerializerOptions(){Converters = { new Int2Converter() }});
		Assert.That(sut, Is.EqualTo("{\"MyInt\":1}")); // Int2Converter used
	}


	internal class MyClass {

		[JsonConverter(typeof(Int1Converter))]
		public int MyInt { get; set; }

	}
}