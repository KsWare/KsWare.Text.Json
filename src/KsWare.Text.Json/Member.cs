using System.Reflection;
using System.Text.Json.Serialization;

internal class Member {

	private readonly MemberInfo _mi;
	private readonly object _instance;

	public Member(MemberInfo mi) {
		_mi = mi;
		Name = _mi.Name;
	}

	public Member(MemberInfo mi, object instance) {
		_mi = mi;
		_instance = instance;
		Name = _mi.Name;
	}

	public string Name { get; set; }

	public MemberInfo MemberInfo => _mi;

	public object? Value { get; set; }
	public Type? MemberType { get; set; }
	public int Order { get; set; }
	public JsonNumberHandling? NumberHandling { get; set; }
	public JsonConverter Converter { get; set; }
	public int OriginalOrder { get; set; }
	public bool IsExtensionData { get; set; }

	public void SetValue(object value) {
		if (_instance == null) throw new InvalidOperationException("Instance is null.");
		if (_mi is FieldInfo fi) fi.SetValue(_instance, value);
		else if (_mi is PropertyInfo pi) pi.SetValue(_instance, value);
		throw new InvalidOperationException();
	}

}