using System.Text.Json;

namespace KsWare.Text.Json.Tests {

	public class Tests {

		[SetUp]
		public void Setup() {
		}

		[Test]
		public void Test1() {
			var options = new JsonSerializerOptions {
				Converters = {new DefaultObjectConverter()}
			};
			var json = JsonSerializer.Serialize(new MyClassA(), options);
			Console.WriteLine(json);
		}

		[Test]
		public void Test2() {
			var options = new JsonSerializerOptions {
				Converters = {new DefaultObjectConverter()}
			};
			var classA = new MyClassA {
				Int32 = 32,
				String = "String",
				Bool = true,
				Enum = MyEnum.Enum1,
				ByteArray = new byte[] {01, 02, 255}
			};
			var json = JsonSerializer.Serialize(classA, options);
			Console.WriteLine(json);
		}

		[Test]
		public void Test3() {
			var options = new JsonSerializerOptions {
				Converters = {new DefaultObjectConverter()}
			};
			var classB = new MyClassB {
				Int32 = 32,
				String = "String",
				Bool = true,
				Enum = MyEnum.Enum1,
				ByteArray = new byte[] {01, 02, 255},
				Struct = new MyStruct{Double = 3.14}
			};
			var json = JsonSerializer.Serialize(classB, options);
			Console.WriteLine(json);
		}

		[Test]
		public void Test3a() {
			var options = new JsonSerializerOptions {
				Converters = {new DefaultObjectConverter()}
			};
			var classB = new MyClassBa {
				Struct = new MyStruct{Double = 3.14}
			};
			var json = JsonSerializer.Serialize(classB, options);
			Console.WriteLine(json);
		}
	}

	public class MyClassA {

		public int Int32 { get; set; }
		public string String { get; set; }
		public bool Bool { get; set; }
		public MyEnum Enum { get; set; }
		public byte[] ByteArray { get; set; }

	}

	public class MyClassB : MyClassA {

		public MyStruct Struct { get; set; }

	}

	public class MyClassBa {

		public MyStruct Struct { get; set; }

	}

	public struct MyStruct {

		public double Double { get; set; }

	}

	public enum MyEnum {
		Enum0=0,
		Enum1=1,
	}
	public enum MyEnum1 {
		Enum1=1
	}
}