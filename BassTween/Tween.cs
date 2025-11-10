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
            double t = sw.ElapsedMilliseconds / durationMs;
            double eased = ApplyEasing(t, easing);
            onUpdate(eased);
            await Task.Delay(1000/60, token);
        }

        onUpdate(1.0); // finalize
    }

    private static double ApplyEasing(double t, Easing easing)
    {
        t = Math.Clamp(t, 0, 1);

        return easing switch
        {
            Easing.Linear => t,
            Easing.EaseIn => t * t,
            Easing.EaseOut => 1 - Math.Pow(1 - t, 2),
            Easing.EaseInOut => t < 0.5
                ? 2 * t * t
                : 1 - Math.Pow(-2 * t + 2, 2) / 2,

            Easing.SineIn => 1 - Math.Cos((t * Math.PI) / 2),
            Easing.SineOut => Math.Sin((t * Math.PI) / 2),
            Easing.SineInOut => -(Math.Cos(Math.PI * t) - 1) / 2,

            _ => t
        };
    }
}
