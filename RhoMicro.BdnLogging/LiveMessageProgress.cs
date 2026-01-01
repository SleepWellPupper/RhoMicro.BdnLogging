// SPDX-License-Identifier: MPL-2.0

internal readonly record struct LiveMessageProgress(
    String? Name,
    ConsoleColor? PrimaryColor,
    ConsoleColor? SecondaryColor,
    Single ProgressRatio)
{
    public static LiveMessageProgress Default => new(
        Name: null,
        PrimaryColor: ConsoleColor.Green,
        SecondaryColor: ConsoleColor.Black,
        ProgressRatio: 0);

    public static LiveMessageProgress Create(BenchmarkState? benchmarkState)
    {
        if (benchmarkState is not
            {
                Name: var name,
                Method: var method,
                ProgressRatio: var progress,
                TotalCount: var totalCount,
                CompletedCount: var completedCount
            })
        {
            return Default;
        }

        var result = Default with
        {
            Name = method is []
                ? $"{name} ({completedCount}/{totalCount})"
                : $"{name}.{method} ({completedCount}/{totalCount})",
            ProgressRatio = progress
        };

        return result;
    }
}
