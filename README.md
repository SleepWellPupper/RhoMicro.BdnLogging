# RhoMicro.BdnLogging

This is a less verbose logger implementation for `BenchmarkDotNet` that doesn't clutter your terminal.

## Licensing

This library is licensed to you under the MPL-2.0.

## Demo

<p align="center" width="100%">
<video src="https://github.com/user-attachments/assets/5fc01eab-00c1-48d6-9553-33cb95ba0bbe" width="80%" controls></video>
</p>

## Installation

Package Reference:

```xml

<ItemGroup>
    <PackageReference Include="RhoMicro.BdnLogging" Version="*"/>
</ItemGroup>
```

CLI:

```
dotnet add RhoMicro.BdnLogging
```

## How To Use

Use the provided `SpotlightConfig` that replaces the `BenchmarkDotNet.Loggers.ConsoleLogger` with the `SpotlightLogger`
implementation:

```cs
BenchmarkRunner.Run<Benchmark>(
    SpotlightConfig.Instance, 
    args 
);
```

Alternatively, use the `SpotlightLogger` directly:

```cs
BenchmarkRunner.Run<Benchmark>(
    new MyConfig().AddLogger(SpotlitLogger.Instance)
);
```
