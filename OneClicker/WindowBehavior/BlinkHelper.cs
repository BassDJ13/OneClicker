using BassTween;

namespace OneClicker.WindowBehavior;

public static class BlinkHelper
{
    public static async Task BlinkAsync(Action<double> onUpdate)
    {
        for (int i = 0; i < 2; i++)
        {
            await Tween.AnimateAsync(100, Easing.EaseOut, v => onUpdate(v));
            await Tween.AnimateAsync(300, Easing.EaseOut, v => onUpdate(1 - v));
        }
    }
}

