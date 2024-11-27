using System.Text.RegularExpressions;
using BenchmarkDotNet.Attributes;

namespace BenchmarkTest;

public partial class Benchmark
{
    private static readonly string[] StackTraceLines = """
                                                          at System.Text.RegularExpressions.RegexRunner.<CheckTimeout>g__ThrowRegexTimeout|25_0()
                                                          at System.Text.RegularExpressions.Generated.<RegexGenerator_g>F06D33C3F8C8C3FD257C1A1967E3A3BAC4BE9C8EC41CC9366C764C2205C68F0CE__GetFrameRegex_1.RunnerFactory.Runner.TryMatchAtCurrentPosition(ReadOnlySpan`1) in /_/artifacts/obj/Microsoft.Testing.Platform/Release/net8.0/System.Text.RegularExpressions.Generator/System.Text.RegularExpressions.Generator.RegexGenerator/RegexGenerator.g.cs:line 639
                                                          at System.Text.RegularExpressions.Generated.<RegexGenerator_g>F06D33C3F8C8C3FD257C1A1967E3A3BAC4BE9C8EC41CC9366C764C2205C68F0CE__GetFrameRegex_1.RunnerFactory.Runner.Scan(ReadOnlySpan`1) in /_/artifacts/obj/Microsoft.Testing.Platform/Release/net8.0/System.Text.RegularExpressions.Generator/System.Text.RegularExpressions.Generator.RegexGenerator/RegexGenerator.g.cs:line 537
                                                          at System.Text.RegularExpressions.Regex.ScanInternal(RegexRunnerMode, Boolean, String, Int32, RegexRunner, ReadOnlySpan`1, Boolean)
                                                          at System.Text.RegularExpressions.Regex.RunSingleMatch(RegexRunnerMode, Int32, String, Int32, Int32, Int32)
                                                          at System.Text.RegularExpressions.Regex.Match(String)
                                                          at Microsoft.Testing.Platform.OutputDevice.Terminal.TerminalTestReporter.AppendStackFrame(ITerminal, String) in /_/src/Platform/Microsoft.Testing.Platform/OutputDevice/Terminal/TerminalTestReporter.cs:line 650
                                                          at Microsoft.Testing.Platform.OutputDevice.Terminal.TerminalTestReporter.FormatStackTrace(ITerminal, FlatException[], Int32) in /_/src/Platform/Microsoft.Testing.Platform/OutputDevice/Terminal/TerminalTestReporter.cs:line 601
                                                          at Microsoft.Testing.Platform.OutputDevice.Terminal.TerminalTestReporter.RenderTestCompleted(ITerminal , String , String, String, String , TestOutcome, TimeSpan, FlatException[] , String, String, String, String) in /_/src/Platform/Microsoft.Testing.Platform/OutputDevice/Terminal/TerminalTestReporter.cs:line 517
                                                          at Microsoft.Testing.Platform.OutputDevice.Terminal.TerminalTestReporter.<>c__DisplayClass27_0.<TestCompleted>b__0(ITerminal terminal) in /_/src/Platform/Microsoft.Testing.Platform/OutputDevice/Terminal/TerminalTestReporter.cs:line 439
                                                          at Microsoft.Testing.Platform.OutputDevice.Terminal.TestProgressStateAwareTerminal.WriteToTerminal(Action`1) in /_/src/Platform/Microsoft.Testing.Platform/OutputDevice/Terminal/TestProgressStateAwareTerminal.cs:line 129
                                                          at Microsoft.Testing.Platform.OutputDevice.Terminal.TerminalTestReporter.TestCompleted(String , String, String, String, String , TestOutcome, TimeSpan, FlatException[] , String, String, String, String) in /_/src/Platform/Microsoft.Testing.Platform/OutputDevice/Terminal/TerminalTestReporter.cs:line 439
                                                          at Microsoft.Testing.Platform.OutputDevice.Terminal.TerminalTestReporter.TestCompleted(String , String, String, String, String , TestOutcome, TimeSpan, String, Exception, String, String, String, String) in /_/src/Platform/Microsoft.Testing.Platform/OutputDevice/Terminal/TerminalTestReporter.cs:line 386
                                                          at Microsoft.Testing.Platform.OutputDevice.TerminalOutputDevice.ConsumeAsync(IDataProducer, IData, CancellationToken) in /_/src/Platform/Microsoft.Testing.Platform/OutputDevice/TerminalOutputDevice.cs:line 458
                                                          at Microsoft.Testing.Platform.Messages.AsyncConsumerDataProcessor.ConsumeAsync() in /_/src/Platform/Microsoft.Testing.Platform/Messages/ChannelConsumerDataProcessor.cs:line 74
                                                          at Microsoft.Testing.Platform.Messages.AsyncConsumerDataProcessor.DrainDataAsync() in /_/src/Platform/Microsoft.Testing.Platform/Messages/ChannelConsumerDataProcessor.cs:line 146
                                                          at Microsoft.Testing.Platform.Messages.AsynchronousMessageBus.DrainDataAsync() in /_/src/Platform/Microsoft.Testing.Platform/Messages/AsynchronousMessageBus.cs:line 177
                                                          at Microsoft.Testing.Platform.Messages.MessageBusProxy.DrainDataAsync() in /_/src/Platform/Microsoft.Testing.Platform/Messages/MessageBusProxy.cs:line 39
                                                          at Microsoft.Testing.Platform.Hosts.CommonTestHost.NotifyTestSessionEndAsync(SessionUid, BaseMessageBus, ServiceProvider, CancellationToken) in /_/src/Platform/Microsoft.Testing.Platform/Hosts/CommonTestHost.cs:line 192
                                                          at Microsoft.Testing.Platform.Hosts.CommonTestHost.ExecuteRequestAsync(IPlatformOutputDevice, ITestSessionContext, ServiceProvider, BaseMessageBus, ITestFramework, ClientInfo) in /_/src/Platform/Microsoft.Testing.Platform/Hosts/CommonTestHost.cs:line 133
                                                          at Microsoft.Testing.Platform.Hosts.ConsoleTestHost.InternalRunAsync() in /_/src/Platform/Microsoft.Testing.Platform/Hosts/ConsoleTestHost.cs:line 85
                                                          at Microsoft.Testing.Platform.Hosts.ConsoleTestHost.InternalRunAsync() in /_/src/Platform/Microsoft.Testing.Platform/Hosts/ConsoleTestHost.cs:line 117
                                                          at Microsoft.Testing.Platform.Hosts.CommonTestHost.RunTestAppAsync(CancellationToken) in /_/src/Platform/Microsoft.Testing.Platform/Hosts/CommonTestHost.cs:line 106
                                                          at Microsoft.Testing.Platform.Hosts.CommonTestHost.RunAsync() in /_/src/Platform/Microsoft.Testing.Platform/Hosts/CommonTestHost.cs:line 34
                                                          at Microsoft.Testing.Platform.Hosts.CommonTestHost.RunAsync() in /_/src/Platform/Microsoft.Testing.Platform/Hosts/CommonTestHost.cs:line 72
                                                          at Microsoft.Testing.Platform.Builder.TestApplication.RunAsync() in /_/src/Platform/Microsoft.Testing.Platform/Builder/TestApplication.cs:line 244
                                                          at TestingPlatformEntryPoint.Main(String[]) in /_/TUnit.TestProject/obj/Release/net8.0/osx-x64/TestPlatformEntryPoint.cs:line 16
                                                          at TestingPlatformEntryPoint.<Main>(String[])
                                                       """.Split([Environment.NewLine], StringSplitOptions.RemoveEmptyEntries);

    public static readonly string[] AOTStacktraceLines = """
                                                          at BenchmarkTest.ExceptionThrower.<RandomSwitcher>d__7.MoveNext() + 0x42f
                                                       --- End of stack trace from previous location ---
                                                          at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw() + 0x20
                                                          at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task) + 0xb2
                                                          at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task, ConfigureAwaitOptions) + 0x4b
                                                          at BenchmarkTest.ExceptionThrower.<Nested4>d__5.MoveNext() + 0x9d
                                                       --- End of stack trace from previous location ---
                                                          at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw() + 0x20
                                                          at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task) + 0xb2
                                                          at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task, ConfigureAwaitOptions) + 0x4b
                                                          at BenchmarkTest.ExceptionThrower.<RandomSwitcher>d__7.MoveNext() + 0x341
                                                       --- End of stack trace from previous location ---
                                                          at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw() + 0x20
                                                          at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task) + 0xb2
                                                          at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task, ConfigureAwaitOptions) + 0x4b
                                                          at BenchmarkTest.ExceptionThrower.<Nested1>d__2.MoveNext() + 0x9d
                                                       --- End of stack trace from previous location ---
                                                          at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw() + 0x20
                                                          at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task) + 0xb2
                                                          at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task, ConfigureAwaitOptions) + 0x4b
                                                          at BenchmarkTest.ExceptionThrower.<RandomSwitcher>d__7.MoveNext() + 0x17f
                                                       --- End of stack trace from previous location ---
                                                          at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw() + 0x20
                                                          at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task) + 0xb2
                                                          at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task, ConfigureAwaitOptions) + 0x4b
                                                          at BenchmarkTest.ExceptionThrower.<Nested3>d__4.MoveNext() + 0x9d
                                                       --- End of stack trace from previous location ---
                                                          at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw() + 0x20
                                                          at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task) + 0xb2
                                                          at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task, ConfigureAwaitOptions) + 0x4b
                                                          at BenchmarkTest.ExceptionThrower.<RandomSwitcher>d__7.MoveNext() + 0x2ab
                                                       --- End of stack trace from previous location ---
                                                          at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw() + 0x20
                                                          at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task) + 0xb2
                                                          at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task, ConfigureAwaitOptions) + 0x4b
                                                          at BenchmarkTest.ExceptionThrower.<Nested1>d__2.MoveNext() + 0x9d
                                                       --- End of stack trace from previous location ---
                                                          at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw() + 0x20
                                                          at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task) + 0xb2
                                                          at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task, ConfigureAwaitOptions) + 0x4b
                                                          at BenchmarkTest.ExceptionThrower.<RandomSwitcher>d__7.MoveNext() + 0x17f
                                                       --- End of stack trace from previous location ---
                                                          at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw() + 0x20
                                                          at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task) + 0xb2
                                                          at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task, ConfigureAwaitOptions) + 0x4b
                                                          at BenchmarkTest.ExceptionThrower.<Nested4>d__5.MoveNext() + 0x9d
                                                       --- End of stack trace from previous location ---
                                                          at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw() + 0x20
                                                          at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task) + 0xb2
                                                          at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task, ConfigureAwaitOptions) + 0x4b
                                                          at BenchmarkTest.ExceptionThrower.<RandomSwitcher>d__7.MoveNext() + 0x341
                                                       --- End of stack trace from previous location ---
                                                          at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw() + 0x20
                                                          at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task) + 0xb2
                                                          at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task, ConfigureAwaitOptions) + 0x4b
                                                          at BenchmarkTest.ExceptionThrower.<Nested2>d__3.MoveNext() + 0x9d
                                                       --- End of stack trace from previous location ---
                                                          at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw() + 0x20
                                                          at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task) + 0xb2
                                                          at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task, ConfigureAwaitOptions) + 0x4b
                                                          at BenchmarkTest.ExceptionThrower.<RandomSwitcher>d__7.MoveNext() + 0x215
                                                       --- End of stack trace from previous location ---
                                                          at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw() + 0x20
                                                          at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task) + 0xb2
                                                          at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task, ConfigureAwaitOptions) + 0x4b
                                                          at BenchmarkTest.ExceptionThrower.<Nested3>d__4.MoveNext() + 0x9d
                                                       --- End of stack trace from previous location ---
                                                          at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw() + 0x20
                                                          at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task) + 0xb2
                                                          at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task, ConfigureAwaitOptions) + 0x4b
                                                          at BenchmarkTest.ExceptionThrower.<RandomSwitcher>d__7.MoveNext() + 0x2ab
                                                       --- End of stack trace from previous location ---
                                                          at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw() + 0x20
                                                          at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task) + 0xb2
                                                          at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task, ConfigureAwaitOptions) + 0x4b
                                                          at BenchmarkTest.ExceptionThrower.<Nested3>d__4.MoveNext() + 0x9d
                                                       --- End of stack trace from previous location ---
                                                          at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw() + 0x20
                                                          at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task) + 0xb2
                                                          at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task, ConfigureAwaitOptions) + 0x4b
                                                          at BenchmarkTest.ExceptionThrower.<RandomSwitcher>d__7.MoveNext() + 0x2ab
                                                       --- End of stack trace from previous location ---
                                                          at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw() + 0x20
                                                          at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task) + 0xb2
                                                          at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task, ConfigureAwaitOptions) + 0x4b
                                                          at BenchmarkTest.ExceptionThrower.<Nested1>d__2.MoveNext() + 0x9d
                                                       --- End of stack trace from previous location ---
                                                          at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw() + 0x20
                                                          at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task) + 0xb2
                                                          at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task, ConfigureAwaitOptions) + 0x4b
                                                          at BenchmarkTest.ExceptionThrower.<RandomSwitcher>d__7.MoveNext() + 0x17f
                                                       --- End of stack trace from previous location ---
                                                          at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw() + 0x20
                                                          at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task) + 0xb2
                                                          at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task, ConfigureAwaitOptions) + 0x4b
                                                          at BenchmarkTest.ExceptionThrower.<Nested2>d__3.MoveNext() + 0x9d
                                                       --- End of stack trace from previous location ---
                                                          at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw() + 0x20
                                                          at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task) + 0xb2
                                                          at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task, ConfigureAwaitOptions) + 0x4b
                                                          at BenchmarkTest.ExceptionThrower.<RandomSwitcher>d__7.MoveNext() + 0x215
                                                       --- End of stack trace from previous location ---
                                                          at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw() + 0x20
                                                          at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task) + 0xb2
                                                          at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task, ConfigureAwaitOptions) + 0x4b
                                                          at BenchmarkTest.ExceptionThrower.<Nested4>d__5.MoveNext() + 0x9d
                                                       --- End of stack trace from previous location ---
                                                          at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw() + 0x20
                                                          at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task) + 0xb2
                                                          at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task, ConfigureAwaitOptions) + 0x4b
                                                          at BenchmarkTest.ExceptionThrower.<RandomSwitcher>d__7.MoveNext() + 0x341
                                                       --- End of stack trace from previous location ---
                                                          at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw() + 0x20
                                                          at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task) + 0xb2
                                                          at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task, ConfigureAwaitOptions) + 0x4b
                                                          at BenchmarkTest.ExceptionThrower.<Nested4>d__5.MoveNext() + 0x9d
                                                       --- End of stack trace from previous location ---
                                                          at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw() + 0x20
                                                          at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task) + 0xb2
                                                          at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task, ConfigureAwaitOptions) + 0x4b
                                                          at BenchmarkTest.ExceptionThrower.<RandomSwitcher>d__7.MoveNext() + 0x341
                                                       --- End of stack trace from previous location ---
                                                          at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw() + 0x20
                                                          at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task) + 0xb2
                                                          at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task, ConfigureAwaitOptions) + 0x4b
                                                          at BenchmarkTest.ExceptionThrower.<Nested1>d__2.MoveNext() + 0x9d
                                                       --- End of stack trace from previous location ---
                                                          at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw() + 0x20
                                                          at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task) + 0xb2
                                                          at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task, ConfigureAwaitOptions) + 0x4b
                                                          at BenchmarkTest.ExceptionThrower.<RandomSwitcher>d__7.MoveNext() + 0x17f
                                                       --- End of stack trace from previous location ---
                                                          at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw() + 0x20
                                                          at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task) + 0xb2
                                                          at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task, ConfigureAwaitOptions) + 0x4b
                                                          at BenchmarkTest.ExceptionThrower.<Nested1>d__2.MoveNext() + 0x9d
                                                       --- End of stack trace from previous location ---
                                                          at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw() + 0x20
                                                          at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task) + 0xb2
                                                          at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task, ConfigureAwaitOptions) + 0x4b
                                                          at BenchmarkTest.ExceptionThrower.<RandomSwitcher>d__7.MoveNext() + 0x17f
                                                       --- End of stack trace from previous location ---
                                                          at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw() + 0x20
                                                          at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task) + 0xb2
                                                          at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task, ConfigureAwaitOptions) + 0x4b
                                                          at BenchmarkTest.ExceptionThrower.<Nested1>d__2.MoveNext() + 0x9d
                                                       --- End of stack trace from previous location ---
                                                          at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw() + 0x20
                                                          at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task) + 0xb2
                                                          at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task, ConfigureAwaitOptions) + 0x4b
                                                          at BenchmarkTest.ExceptionThrower.<RandomSwitcher>d__7.MoveNext() + 0x17f
                                                       --- End of stack trace from previous location ---
                                                          at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw() + 0x20
                                                          at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task) + 0xb2
                                                          at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task, ConfigureAwaitOptions) + 0x4b
                                                          at BenchmarkTest.ExceptionThrower.<Nested2>d__3.MoveNext() + 0x9d
                                                       --- End of stack trace from previous location ---
                                                          at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw() + 0x20
                                                          at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task) + 0xb2
                                                          at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task, ConfigureAwaitOptions) + 0x4b
                                                          at BenchmarkTest.ExceptionThrower.<RandomSwitcher>d__7.MoveNext() + 0x215
                                                       --- End of stack trace from previous location ---
                                                          at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw() + 0x20
                                                          at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task) + 0xb2
                                                          at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task, ConfigureAwaitOptions) + 0x4b
                                                          at BenchmarkTest.ExceptionThrower.<Nested3>d__4.MoveNext() + 0x9d
                                                       --- End of stack trace from previous location ---
                                                          at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw() + 0x20
                                                          at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task) + 0xb2
                                                          at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task, ConfigureAwaitOptions) + 0x4b
                                                          at BenchmarkTest.ExceptionThrower.<RandomSwitcher>d__7.MoveNext() + 0x2ab
                                                       --- End of stack trace from previous location ---
                                                          at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw() + 0x20
                                                          at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task) + 0xb2
                                                          at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task, ConfigureAwaitOptions) + 0x4b
                                                          at BenchmarkTest.ExceptionThrower.<Nested1>d__2.MoveNext() + 0x9d
                                                       --- End of stack trace from previous location ---
                                                          at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw() + 0x20
                                                          at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task) + 0xb2
                                                          at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task, ConfigureAwaitOptions) + 0x4b
                                                          at BenchmarkTest.ExceptionThrower.<RandomSwitcher>d__7.MoveNext() + 0x17f
                                                       --- End of stack trace from previous location ---
                                                          at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw() + 0x20
                                                          at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task) + 0xb2
                                                          at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task, ConfigureAwaitOptions) + 0x4b
                                                          at BenchmarkTest.ExceptionThrower.<Nested3>d__4.MoveNext() + 0x9d
                                                       --- End of stack trace from previous location ---
                                                          at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw() + 0x20
                                                          at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task) + 0xb2
                                                          at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task, ConfigureAwaitOptions) + 0x4b
                                                          at BenchmarkTest.ExceptionThrower.<RandomSwitcher>d__7.MoveNext() + 0x2ab
                                                       --- End of stack trace from previous location ---
                                                          at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw() + 0x20
                                                          at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task) + 0xb2
                                                          at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task, ConfigureAwaitOptions) + 0x4b
                                                          at BenchmarkTest.ExceptionThrower.<Nested1>d__2.MoveNext() + 0x9d
                                                       --- End of stack trace from previous location ---
                                                          at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw() + 0x20
                                                          at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task) + 0xb2
                                                          at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task, ConfigureAwaitOptions) + 0x4b
                                                          at BenchmarkTest.ExceptionThrower.<GetExceptionWithStacktrace>d__1.MoveNext() + 0xad
                                                          at BenchmarkTest.ExceptionThrower.<RandomSwitcher>d__7.MoveNext() + 0x42f
                                                       --- End of stack trace from previous location ---
                                                          at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw() + 0x20
                                                          at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task) + 0xb2
                                                          at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task, ConfigureAwaitOptions) + 0x4b
                                                          at BenchmarkTest.ExceptionThrower.<Nested4>d__5.MoveNext() + 0x9d
                                                       --- End of stack trace from previous location ---
                                                          at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw() + 0x20
                                                          at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task) + 0xb2
                                                          at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task, ConfigureAwaitOptions) + 0x4b
                                                          at BenchmarkTest.ExceptionThrower.<RandomSwitcher>d__7.MoveNext() + 0x341
                                                       --- End of stack trace from previous location ---
                                                          at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw() + 0x20
                                                          at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task) + 0xb2
                                                          at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task, ConfigureAwaitOptions) + 0x4b
                                                          at BenchmarkTest.ExceptionThrower.<Nested1>d__2.MoveNext() + 0x9d
                                                       --- End of stack trace from previous location ---
                                                          at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw() + 0x20
                                                          at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task) + 0xb2
                                                          at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task, ConfigureAwaitOptions) + 0x4b
                                                          at BenchmarkTest.ExceptionThrower.<RandomSwitcher>d__7.MoveNext() + 0x17f
                                                       --- End of stack trace from previous location ---
                                                          at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw() + 0x20
                                                          at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task) + 0xb2
                                                          at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task, ConfigureAwaitOptions) + 0x4b
                                                          at BenchmarkTest.ExceptionThrower.<Nested3>d__4.MoveNext() + 0x9d
                                                       --- End of stack trace from previous location ---
                                                          at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw() + 0x20
                                                          at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task) + 0xb2
                                                          at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task, ConfigureAwaitOptions) + 0x4b
                                                          at BenchmarkTest.ExceptionThrower.<RandomSwitcher>d__7.MoveNext() + 0x2ab
                                                       --- End of stack trace from previous location ---
                                                          at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw() + 0x20
                                                          at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task) + 0xb2
                                                          at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task, ConfigureAwaitOptions) + 0x4b
                                                          at BenchmarkTest.ExceptionThrower.<Nested1>d__2.MoveNext() + 0x9d
                                                       --- End of stack trace from previous location ---
                                                          at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw() + 0x20
                                                          at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task) + 0xb2
                                                          at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task, ConfigureAwaitOptions) + 0x4b
                                                          at BenchmarkTest.ExceptionThrower.<RandomSwitcher>d__7.MoveNext() + 0x17f
                                                       --- End of stack trace from previous location ---
                                                          at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw() + 0x20
                                                          at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task) + 0xb2
                                                          at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task, ConfigureAwaitOptions) + 0x4b
                                                          at BenchmarkTest.ExceptionThrower.<Nested4>d__5.MoveNext() + 0x9d
                                                       --- End of stack trace from previous location ---
                                                          at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw() + 0x20
                                                          at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task) + 0xb2
                                                          at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task, ConfigureAwaitOptions) + 0x4b
                                                          at BenchmarkTest.ExceptionThrower.<RandomSwitcher>d__7.MoveNext() + 0x341
                                                       --- End of stack trace from previous location ---
                                                          at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw() + 0x20
                                                          at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task) + 0xb2
                                                          at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task, ConfigureAwaitOptions) + 0x4b
                                                          at BenchmarkTest.ExceptionThrower.<Nested2>d__3.MoveNext() + 0x9d
                                                       --- End of stack trace from previous location ---
                                                          at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw() + 0x20
                                                          at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task) + 0xb2
                                                          at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task, ConfigureAwaitOptions) + 0x4b
                                                          at BenchmarkTest.ExceptionThrower.<RandomSwitcher>d__7.MoveNext() + 0x215
                                                       --- End of stack trace from previous location ---
                                                          at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw() + 0x20
                                                          at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task) + 0xb2
                                                          at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task, ConfigureAwaitOptions) + 0x4b
                                                          at BenchmarkTest.ExceptionThrower.<Nested3>d__4.MoveNext() + 0x9d
                                                       --- End of stack trace from previous location ---
                                                          at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw() + 0x20
                                                          at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task) + 0xb2
                                                          at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task, ConfigureAwaitOptions) + 0x4b
                                                          at BenchmarkTest.ExceptionThrower.<RandomSwitcher>d__7.MoveNext() + 0x2ab
                                                       --- End of stack trace from previous location ---
                                                          at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw() + 0x20
                                                          at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task) + 0xb2
                                                          at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task, ConfigureAwaitOptions) + 0x4b
                                                          at BenchmarkTest.ExceptionThrower.<Nested3>d__4.MoveNext() + 0x9d
                                                       --- End of stack trace from previous location ---
                                                          at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw() + 0x20
                                                          at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task) + 0xb2
                                                          at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task, ConfigureAwaitOptions) + 0x4b
                                                          at BenchmarkTest.ExceptionThrower.<RandomSwitcher>d__7.MoveNext() + 0x2ab
                                                       --- End of stack trace from previous location ---
                                                          at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw() + 0x20
                                                          at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task) + 0xb2
                                                          at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task, ConfigureAwaitOptions) + 0x4b
                                                          at BenchmarkTest.ExceptionThrower.<Nested1>d__2.MoveNext() + 0x9d
                                                       --- End of stack trace from previous location ---
                                                          at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw() + 0x20
                                                          at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task) + 0xb2
                                                          at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task, ConfigureAwaitOptions) + 0x4b
                                                          at BenchmarkTest.ExceptionThrower.<RandomSwitcher>d__7.MoveNext() + 0x17f
                                                       --- End of stack trace from previous location ---
                                                          at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw() + 0x20
                                                          at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task) + 0xb2
                                                          at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task, ConfigureAwaitOptions) + 0x4b
                                                          at BenchmarkTest.ExceptionThrower.<Nested2>d__3.MoveNext() + 0x9d
                                                       --- End of stack trace from previous location ---
                                                          at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw() + 0x20
                                                          at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task) + 0xb2
                                                          at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task, ConfigureAwaitOptions) + 0x4b
                                                          at BenchmarkTest.ExceptionThrower.<RandomSwitcher>d__7.MoveNext() + 0x215
                                                       --- End of stack trace from previous location ---
                                                          at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw() + 0x20
                                                          at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task) + 0xb2
                                                          at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task, ConfigureAwaitOptions) + 0x4b
                                                          at BenchmarkTest.ExceptionThrower.<Nested4>d__5.MoveNext() + 0x9d
                                                       --- End of stack trace from previous location ---
                                                          at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw() + 0x20
                                                          at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task) + 0xb2
                                                          at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task, ConfigureAwaitOptions) + 0x4b
                                                          at BenchmarkTest.ExceptionThrower.<RandomSwitcher>d__7.MoveNext() + 0x341
                                                       --- End of stack trace from previous location ---
                                                          at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw() + 0x20
                                                          at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task) + 0xb2
                                                          at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task, ConfigureAwaitOptions) + 0x4b
                                                          at BenchmarkTest.ExceptionThrower.<Nested4>d__5.MoveNext() + 0x9d
                                                       --- End of stack trace from previous location ---
                                                          at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw() + 0x20
                                                          at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task) + 0xb2
                                                          at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task, ConfigureAwaitOptions) + 0x4b
                                                          at BenchmarkTest.ExceptionThrower.<RandomSwitcher>d__7.MoveNext() + 0x341
                                                       --- End of stack trace from previous location ---
                                                          at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw() + 0x20
                                                          at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task) + 0xb2
                                                          at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task, ConfigureAwaitOptions) + 0x4b
                                                          at BenchmarkTest.ExceptionThrower.<Nested1>d__2.MoveNext() + 0x9d
                                                       --- End of stack trace from previous location ---
                                                          at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw() + 0x20
                                                          at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task) + 0xb2
                                                          at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task, ConfigureAwaitOptions) + 0x4b
                                                          at BenchmarkTest.ExceptionThrower.<RandomSwitcher>d__7.MoveNext() + 0x17f
                                                       --- End of stack trace from previous location ---
                                                          at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw() + 0x20
                                                          at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task) + 0xb2
                                                          at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task, ConfigureAwaitOptions) + 0x4b
                                                          at BenchmarkTest.ExceptionThrower.<Nested1>d__2.MoveNext() + 0x9d
                                                       --- End of stack trace from previous location ---
                                                          at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw() + 0x20
                                                          at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task) + 0xb2
                                                          at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task, ConfigureAwaitOptions) + 0x4b
                                                          at BenchmarkTest.ExceptionThrower.<RandomSwitcher>d__7.MoveNext() + 0x17f
                                                       --- End of stack trace from previous location ---
                                                          at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw() + 0x20
                                                          at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task) + 0xb2
                                                          at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task, ConfigureAwaitOptions) + 0x4b
                                                          at BenchmarkTest.ExceptionThrower.<Nested1>d__2.MoveNext() + 0x9d
                                                       --- End of stack trace from previous location ---
                                                          at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw() + 0x20
                                                          at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task) + 0xb2
                                                          at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task, ConfigureAwaitOptions) + 0x4b
                                                          at BenchmarkTest.ExceptionThrower.<RandomSwitcher>d__7.MoveNext() + 0x17f
                                                       --- End of stack trace from previous location ---
                                                          at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw() + 0x20
                                                          at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task) + 0xb2
                                                          at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task, ConfigureAwaitOptions) + 0x4b
                                                          at BenchmarkTest.ExceptionThrower.<Nested2>d__3.MoveNext() + 0x9d
                                                       --- End of stack trace from previous location ---
                                                          at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw() + 0x20
                                                          at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task) + 0xb2
                                                          at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task, ConfigureAwaitOptions) + 0x4b
                                                          at BenchmarkTest.ExceptionThrower.<RandomSwitcher>d__7.MoveNext() + 0x215
                                                       --- End of stack trace from previous location ---
                                                          at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw() + 0x20
                                                          at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task) + 0xb2
                                                          at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task, ConfigureAwaitOptions) + 0x4b
                                                          at BenchmarkTest.ExceptionThrower.<Nested3>d__4.MoveNext() + 0x9d
                                                       --- End of stack trace from previous location ---
                                                          at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw() + 0x20
                                                          at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task) + 0xb2
                                                          at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task, ConfigureAwaitOptions) + 0x4b
                                                          at BenchmarkTest.ExceptionThrower.<RandomSwitcher>d__7.MoveNext() + 0x2ab
                                                       --- End of stack trace from previous location ---
                                                          at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw() + 0x20
                                                          at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task) + 0xb2
                                                          at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task, ConfigureAwaitOptions) + 0x4b
                                                          at BenchmarkTest.ExceptionThrower.<Nested1>d__2.MoveNext() + 0x9d
                                                       --- End of stack trace from previous location ---
                                                          at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw() + 0x20
                                                          at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task) + 0xb2
                                                          at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task, ConfigureAwaitOptions) + 0x4b
                                                          at BenchmarkTest.ExceptionThrower.<RandomSwitcher>d__7.MoveNext() + 0x17f
                                                       --- End of stack trace from previous location ---
                                                          at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw() + 0x20
                                                          at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task) + 0xb2
                                                          at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task, ConfigureAwaitOptions) + 0x4b
                                                          at BenchmarkTest.ExceptionThrower.<Nested3>d__4.MoveNext() + 0x9d
                                                       --- End of stack trace from previous location ---
                                                          at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw() + 0x20
                                                          at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task) + 0xb2
                                                          at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task, ConfigureAwaitOptions) + 0x4b
                                                          at BenchmarkTest.ExceptionThrower.<RandomSwitcher>d__7.MoveNext() + 0x2ab
                                                       --- End of stack trace from previous location ---
                                                          at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw() + 0x20
                                                          at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task) + 0xb2
                                                          at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task, ConfigureAwaitOptions) + 0x4b
                                                          at BenchmarkTest.ExceptionThrower.<Nested1>d__2.MoveNext() + 0x9d
                                                       --- End of stack trace from previous location ---
                                                          at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw() + 0x20
                                                          at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task) + 0xb2
                                                          at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task, ConfigureAwaitOptions) + 0x4b
                                                          at BenchmarkTest.ExceptionThrower.<GetExceptionWithStacktrace>d__1.MoveNext() + 0xad 
                                                       """.Split([Environment.NewLine], System.StringSplitOptions.RemoveEmptyEntries);
    
    private static string[] DynamicStacktraceLines { get; } = ExceptionThrower.GetExceptionWithStacktrace()
            .Result
            .StackTrace!
            .Split([Environment.NewLine], StringSplitOptions.RemoveEmptyEntries);
    
    [GeneratedRegex(@$"^   at ((?<code>.+) in (?<file>.+):line (?<line>\d+)|(?<code1>.+))$", RegexOptions.ExplicitCapture, 1000)]
    private static partial Regex OriginalRegex();
    
    [GeneratedRegex(@"^   at (?<code>.+\))( in (?<file>.+):line (?<line>\d+))?$", RegexOptions.ExplicitCapture, 1000)]
    private static partial Regex NewRegex();
    
    [GeneratedRegex(@"^   at (?<code>.+\))", RegexOptions.ExplicitCapture, 1000)]
    internal static partial Regex NewAOTRegex();

    [Benchmark]
    public void OriginalRegexBenchmark()
    {
        foreach (var stackTraceLine in StackTraceLines)
        {
            _ = OriginalRegex().Match(stackTraceLine);
        }
    }
    
    [Benchmark]
    public void OriginalRegexBenchmark_DynamicStacktrace()
    {
        foreach (var stackTraceLine in DynamicStacktraceLines)
        {
            _ = OriginalRegex().Match(stackTraceLine);
        }
    }
    
    [Benchmark]
    public void OriginalRegexBenchmark_AOTStacktrace()
    {
       // This actually fails matching
       foreach (var stackTraceLine in AOTStacktraceLines)
       {
          _ = OriginalRegex().Match(stackTraceLine);
       }
    }
    
    [Benchmark]
    public void NewRegexBenchmark()
    {
        foreach (var stackTraceLine in StackTraceLines)
        {
            _ = NewRegex().Match(stackTraceLine);
        }
    }
    
    [Benchmark]
    public void NewRegexBenchmark_DynamicStacktrace()
    {
        foreach (var stackTraceLine in DynamicStacktraceLines)
        {
            _ = NewRegex().Match(stackTraceLine);
        }
    }
    
    [Benchmark]
    public void NewRegexBenchmark_AOTStacktrace()
    {
       foreach (var stackTraceLine in AOTStacktraceLines)
       {
          _ = NewAOTRegex().Match(stackTraceLine);
       }
    }
}