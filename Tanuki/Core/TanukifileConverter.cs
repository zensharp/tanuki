using System;
using System.Collections.Generic;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

public class TanukiYamlTypeConvert<T> : IYamlTypeConverter
{
	public bool Accepts(Type type) => type == typeof(List<T>);

	public object ReadYaml(IParser parser, Type type)
	{
		if (parser.TryConsume<Scalar>(out var scalar))
		{
			return new List<T> { (T)Convert.ChangeType(scalar.Value, typeof(T)) };
		}
		else if (parser.TryConsume<SequenceStart>(out _))
		{
			var list = new List<T>();
			while (!parser.TryConsume<SequenceEnd>(out _))
			{
				if (parser.TryConsume<Scalar>(out var item))
					list.Add((T)Convert.ChangeType(item.Value, typeof(T)));
			}
			return list;
		}
		throw new InvalidOperationException("Unexpected YAML structure for List<" + typeof(T).Name + ">");
	}

	public void WriteYaml(IEmitter emitter, object? value, Type type)
	{
		var list = (List<T>)value!;
		if (list.Count == 1)
		{
			emitter.Emit(new Scalar(list[0]?.ToString() ?? ""));
		}
		else
		{
			emitter.Emit(new SequenceStart(null, null, false, SequenceStyle.Block));
			foreach (var item in list)
			{
				emitter.Emit(new Scalar(item?.ToString() ?? ""));
			}
			emitter.Emit(new SequenceEnd());
		}
	}
}
