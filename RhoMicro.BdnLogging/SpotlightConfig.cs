// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.BdnLogging;

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

/// <summary>
/// Provides all configuration properties of the <see cref="DefaultConfig"/>, but
/// replaces the provided <see cref="ConsoleLogger"/> with a <see cref="SpotlitLogger"/> instance.
/// </summary>
public sealed class SpotlightConfig : IConfig
{
    private SpotlightConfig()
    {
    }

    /// <summary>
    /// Gets the singleton instance.
    /// </summary>
    public static SpotlightConfig Instance { get; } = new();

    /// <inheritdoc/>
    public IEnumerable<IColumnProvider> GetColumnProviders() => DefaultConfig.Instance.GetColumnProviders();

    /// <inheritdoc/>
    public IEnumerable<IExporter> GetExporters() => DefaultConfig.Instance.GetExporters();

    /// <inheritdoc/>
    public IEnumerable<ILogger> GetLoggers()
    {
        foreach (var logger in DefaultConfig.Instance.GetLoggers())
        {
            yield return logger is ConsoleLogger ? SpotlitLogger.Instance : logger;
        }
    }

    /// <inheritdoc/>
    public IEnumerable<IDiagnoser> GetDiagnosers() => DefaultConfig.Instance.GetDiagnosers();

    /// <inheritdoc/>
    public IEnumerable<IAnalyser> GetAnalysers() => DefaultConfig.Instance.GetAnalysers();

    /// <inheritdoc/>
    public IEnumerable<Job> GetJobs() => DefaultConfig.Instance.GetJobs();

    /// <inheritdoc/>
    public IEnumerable<IValidator> GetValidators() => DefaultConfig.Instance.GetValidators();

    /// <inheritdoc/>
    public IEnumerable<HardwareCounter> GetHardwareCounters() => DefaultConfig.Instance.GetHardwareCounters();

    /// <inheritdoc/>
    public IEnumerable<IFilter> GetFilters() => DefaultConfig.Instance.GetFilters();

    /// <inheritdoc/>
    public IEnumerable<BenchmarkLogicalGroupRule> GetLogicalGroupRules() =>
        DefaultConfig.Instance.GetLogicalGroupRules();

    /// <inheritdoc/>
    public IEnumerable<EventProcessor> GetEventProcessors() => DefaultConfig.Instance.GetEventProcessors();

    /// <inheritdoc/>
    public IEnumerable<IColumnHidingRule> GetColumnHidingRules() => DefaultConfig.Instance.GetColumnHidingRules();

    /// <inheritdoc/>
    public IOrderer? Orderer => DefaultConfig.Instance.Orderer;

    /// <inheritdoc/>
    public ICategoryDiscoverer? CategoryDiscoverer => DefaultConfig.Instance.CategoryDiscoverer;

    /// <inheritdoc/>
    public SummaryStyle SummaryStyle => DefaultConfig.Instance.SummaryStyle;

    /// <inheritdoc/>
    public ConfigUnionRule UnionRule => DefaultConfig.Instance.UnionRule;

    /// <inheritdoc/>
    public string ArtifactsPath => DefaultConfig.Instance.ArtifactsPath;

    /// <inheritdoc/>
    public CultureInfo? CultureInfo => DefaultConfig.Instance.CultureInfo;

    /// <inheritdoc/>
    public ConfigOptions Options =>
#if DEBUG
        DefaultConfig.Instance.Options | ConfigOptions.DisableOptimizationsValidator;
#else
        DefaultConfig.Instance.Options;
#endif

    /// <inheritdoc/>
    public TimeSpan BuildTimeout => DefaultConfig.Instance.BuildTimeout;

    /// <inheritdoc/>
    public WakeLockType WakeLock => DefaultConfig.Instance.WakeLock;

    /// <inheritdoc/>
    public IReadOnlyList<Conclusion> ConfigAnalysisConclusion => DefaultConfig.Instance.ConfigAnalysisConclusion;
}
