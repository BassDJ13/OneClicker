using System.Diagnostics;

namespace BassTween;

public static class Tween
{
    public static async Task AnimateAsync(
        double durationMs,
        Easing easing,
        Action<double> onUpdate,
        CancellationToken token = default)
    {
        if (durationMs <= 0)
        {
            onUpdate(1.0);
            return;
        }

        var sw = Stopwatch.StartNew();
        while (sw.ElapsedMilliseconds < durationMs && !token.IsCancellationRequested)
        {
            double time = sw.ElapsedMilliseconds / durationMs;
            double eased = ApplyEasing(time, easing);
            onUpdate(eased);
            await Task.Delay(1000/60, token);
        }

        onUpdate(1.0); // finalize
    }

    private static double ApplyEasing(double time, Easing easing)
    {
        time = Math.Clamp(time, 0, 1);

        return easing switch
        {
            Easing.Linear => time,
            Easing.EaseIn => time * time,
            Easing.EaseOut => 1 - Math.Pow(1 - time, 2),
            Easing.EaseInOut => time < 0.5
                ? 2 * time * time
                : 1 - Math.Pow(-2 * time + 2, 2) / 2,

            Easing.SineIn => 1 - Math.Cos((time * Math.PI) / 2),
            Easing.SineOut => Math.Sin((time * Math.PI) / 2),
            Easing.SineInOut => -(Math.Cos(Math.PI * time) - 1) / 2,

            _ => time
        };
    }
}
