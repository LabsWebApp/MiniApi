﻿namespace MiniApi.Models.Entities.Events;

public record struct NumberInfo(int Code, double Ratio)
{
    public static bool TryParse(string input, out NumberInfo? info)
    {
        info = default;
        var splitArray = input.Split('|', 2);

        if (splitArray.Length != 2) return false;

        if (!int.TryParse(splitArray[0], out var code)) return false;
        if (!double.TryParse(splitArray[1], NumberStyles.Any, CultureInfo.InvariantCulture, out var ratio))
            return false;

        info = new NumberInfo(code, ratio);
        return true;
    }

    public static async ValueTask<NumberInfo?> BindAsync(HttpContext context, ParameterInfo param) =>
        await Task.Run(() =>
        {
            var input = context.GetRouteValue(param.Name!) as string ?? string.Empty;
            return TryParse(input, out var info) ? info : default;
        });
}