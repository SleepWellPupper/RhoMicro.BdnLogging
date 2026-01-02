// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.BdnLogging;

using System.Text;

internal readonly record struct LiveMessageProgress(
    String? Name,
    ConsoleColor? PendingForegroundColor,
    ConsoleColor? PendingBackgroundColor,
    ConsoleColor? CompletedForegroundColor,
    ConsoleColor? CompletedBackgroundColor,
    Single ProgressRatio)
{
    public static LiveMessageProgress Default => new(
        Name: null,
        CompletedForegroundColor: ConsoleColor.Black,
        CompletedBackgroundColor: ConsoleColor.DarkGreen,
        PendingForegroundColor: ConsoleColor.DarkGreen,
        PendingBackgroundColor: (ConsoleColor)(-1),
        ProgressRatio: 0);

    public static LiveMessageProgress Create(BenchmarkState? benchmarkState)
    {
        if (benchmarkState is not
            {
                Name: [_, ..] name,
                Method: var method,
                ProgressRatio: var progress,
                TotalCount: var totalCount,
                CompletedCount: var completedCount,
                Eta: var eta
            })
        {
            return Default with { Name = "Processing" };
        }

        var nameBuilder = new StringBuilder();

        nameBuilder.Append($"Finished {completedCount}/{totalCount} ({progress:P}), ETA: {eta}, ");

        if (method is not [])
        {
            nameBuilder.Append($"Running {name}.{method}");
        }
        else
        {
            nameBuilder.Append($"Processing {name}");
        }

        var result = Default with { Name = nameBuilder.ToString(), ProgressRatio = progress };

        return result;
    }
}
