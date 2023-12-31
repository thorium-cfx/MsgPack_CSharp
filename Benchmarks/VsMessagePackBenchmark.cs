﻿using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using System;
using System.Collections.Generic;

namespace MsgPack
{
	public class VsMessagePackBenchmark
	{
		public const int Iterations = 1_000_000;

		[GlobalSetup]
		public void Setup()
		{
			MsgPackRegistry.GetOrCreateSerializerMethod(typeof(Dictionary<string, string>));
			MsgPackRegistry.GetOrCreateSerializerMethod(typeof(string[]));
		}

		[Benchmark(Description = "thorium Serialize(int)", OperationsPerInvoke = 5 * Iterations)]
		public void Custom_SerializeInt()
		{
			for (int i = 0; i < Iterations; i++)
			{
				new MsgPackSerializer().Serialize(1);
				new MsgPackSerializer().Serialize(-1);
				new MsgPackSerializer().Serialize(-2);
				new MsgPackSerializer().Serialize(-128);
				new MsgPackSerializer().Serialize(-129);
			}
		}

		[Benchmark(Description = "MessagePack Serialize<int>(int)", OperationsPerInvoke = 5 * Iterations)]
		public void MessagePack_SerializeInt()
		{
			for (int i = 0; i < Iterations; i++)
			{
				MessagePack.MessagePackSerializer.Serialize(1);
				MessagePack.MessagePackSerializer.Serialize(-1);
				MessagePack.MessagePackSerializer.Serialize(-2);
				MessagePack.MessagePackSerializer.Serialize(-128);
				MessagePack.MessagePackSerializer.Serialize(-129);
			}
		}

		[Benchmark(Description = "thorium Serialize((object)Dictionary<string, string>)", OperationsPerInvoke = Iterations)]
		public void Custom_SerializeDictionaryStringStringAsObject()
		{
			for (int i = 0; i < Iterations; i++)
			{
				MsgPackSerializer serializer = new MsgPackSerializer();

				serializer.Serialize(new Dictionary<string, string>
				{
					{ "hello0", "item0" },
					{ "hello1", "item1" },
					{ "hello2", "item2" },
					{ "hello3", "item3" },
				});
			}
		}

		[Benchmark(Description = "MessagePack Serialize<object>(Dictionary<string, string>)", OperationsPerInvoke = Iterations)]
		public void MessagePack_SerializeDictionaryStringStringAsObject()
		{
			for (int i = 0; i < Iterations; i++)
			{
				MessagePack.MessagePackSerializer.Serialize<object>(new Dictionary<string, string>
				{
					{ "hello0", "item0" },
					{ "hello1", "item1" },
					{ "hello2", "item2" },
					{ "hello3", "item3" },
				});
			}
		}

		[Benchmark(Description = "thorium Serialize((object)Dictionary<int, string>)", OperationsPerInvoke = Iterations)]
		public void Custom_SerializeDictionaryStringIntAsObject()
		{
			for (int i = 0; i < Iterations; i++)
			{
				MsgPackSerializer serializer = new MsgPackSerializer();

				serializer.Serialize(new Dictionary<int, string>
				{
					{ -2, "item0" },
					{ 200_000, "item1" },
					{ 1234, "item2" },
					{ -9000, "item3" },
				});
			}
		}

		[Benchmark(Description = "MessagePack Serialize<object>(Dictionary<int, string>)", OperationsPerInvoke = Iterations)]
		public void MessagePack_SerializeDictionaryStringIntAsObject()
		{
			for (int i = 0; i < Iterations; i++)
			{
				MessagePack.MessagePackSerializer.Serialize<object>(new Dictionary<int, string>
				{
					{ -2, "item0" },
					{ 200_000, "item1" },
					{ 1234, "item2" },
					{ -9000, "item3" },
				});
			}
		}

		[Benchmark(Description = "thorium Serialize((object)string[])", OperationsPerInvoke = Iterations)]
		public void Custom_SerializeArrayStringAsObject()
		{
			for (int i = 0; i < Iterations; i++)
			{
				MsgPackSerializer serializer = new MsgPackSerializer();

				serializer.Serialize(new string[]
				{
					"hello0",
					"hello1",
					"hello2",
					"hello3",
					"hello4",
					"hello5",
				});
			}
		}

		[Benchmark(Description = "MessagePack Serialize<object>(string[])", OperationsPerInvoke = Iterations)]
		public void MessagePack_SerializeArrayStringAsObject()
		{
			for (int i = 0; i < Iterations; i++)
			{
				MessagePack.MessagePackSerializer.Serialize<object>(new string[]
				{
					"hello0",
					"hello1",
					"hello2",
					"hello3",
					"hello4",
					"hello5",
				});
			}
		}

		static void Main()
		{
			//var config = DefaultConfig.Instance.AddJob(Job.Default.WithRuntime(new MonoRuntime("Mono", "C:\\Program Files\\Mono\\bin\\mono.exe")).WithIterationCount(20));
			var config = DefaultConfig.Instance.AddJob(Job.Default.WithIterationCount(20));

			var summary = BenchmarkRunner.Run<VsMessagePackBenchmark>(config);
			Console.WriteLine(summary);

			//(new Benchmark()).SerializeDictionaryStringStringAsObject2();

			Console.ReadLine();
		}
	}
}
