// SPDX-License-Identifier: MPL-2.0

using System;
using System.Collections.Generic;
using System.Globalization;
using BenchmarkDotNet.Analysers;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.EventProcessors;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Filters;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Validators;

public sealed class FocusConfig(FocusLogger focusLogger) : IConfig
{
    public static FocusConfig Default { get; } = new(FocusLogger.Default);

    public IEnumerable<IColumnProvider> GetColumnProviders() => DefaultConfig.Instance.GetColumnProviders();

    public IEnumerable<IExporter> GetExporters() => DefaultConfig.Instance.GetExporters();

    public IEnumerable<ILogger> GetLoggers()
    {
        foreach (var logger in DefaultConfig.Instance.GetLoggers())
        {
            yield return logger is ConsoleLogger ? focusLogger : logger;
        }
    }

    public IEnumerable<IDiagnoser> GetDiagnosers() => DefaultConfig.Instance.GetDiagnosers();

    public IEnumerable<IAnalyser> GetAnalysers() => DefaultConfig.Instance.GetAnalysers();

    public IEnumerable<Job> GetJobs() => DefaultConfig.Instance.GetJobs();

    public IEnumerable<IValidator> GetValidators() => DefaultConfig.Instance.GetValidators();

    public IEnumerable<HardwareCounter> GetHardwareCounters() => DefaultConfig.Instance.GetHardwareCounters();

    public IEnumerable<IFilter> GetFilters() => DefaultConfig.Instance.GetFilters();

    public IEnumerable<BenchmarkLogicalGroupRule> GetLogicalGroupRules() =>
        DefaultConfig.Instance.GetLogicalGroupRules();

    public IEnumerable<EventProcessor> GetEventProcessors() => DefaultConfig.Instance.GetEventProcessors();

    public IEnumerable<IColumnHidingRule> GetColumnHidingRules() => DefaultConfig.Instance.GetColumnHidingRules();

    public IOrderer? Orderer => DefaultConfig.Instance.Orderer;

    public ICategoryDiscoverer? CategoryDiscoverer => DefaultConfig.Instance.CategoryDiscoverer;

    public SummaryStyle SummaryStyle => DefaultConfig.Instance.SummaryStyle;

    public ConfigUnionRule UnionRule => DefaultConfig.Instance.UnionRule;

    public string ArtifactsPath => DefaultConfig.Instance.ArtifactsPath;

    public CultureInfo? CultureInfo => DefaultConfig.Instance.CultureInfo;

    public ConfigOptions Options => DefaultConfig.Instance.Options;

    public TimeSpan BuildTimeout => DefaultConfig.Instance.BuildTimeout;
    public WakeLockType WakeLock => Default.WakeLock;

    public IReadOnlyList<Conclusion> ConfigAnalysisConclusion => DefaultConfig.Instance.ConfigAnalysisConclusion;
}
