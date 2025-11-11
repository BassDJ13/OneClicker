using BassTween;

namespace OneClicker.WindowBehavior;

public class BlinkHelper
{
    private bool _isBlinking;

    public async Task BlinkAsync(Action<double> onUpdate)
    {
        if (_isBlinking)
        {
            return;
        }

        _isBlinking = true;
        try
        {
            for (int i = 0; i < 2; i++)
            {
                await Tween.AnimateAsync(100, Easing.EaseOut, v => onUpdate(v));
                await Tween.AnimateAsync(300, Easing.EaseOut, v => onUpdate(1 - v));
            }
        }
        finally
        {
            _isBlinking = false;
        }
    }
}
