using System.ComponentModel;

namespace OneClicker.WindowBehavior;

public static class TransparencyHelper
{
    public static void AttachAutoOpacity(Form form, double inactiveOpacity = 0.7)
    {
        ArgumentNullException.ThrowIfNull(form);

        bool isActive = true;

        void SafeUpdate() => Update(form, isActive, inactiveOpacity);

        form.Activated += (_, _) => { isActive = true; SafeUpdate(); };
        form.Deactivate += (_, _) => { isActive = false; SafeUpdate(); };
        form.MouseEnter += (_, _) => SafeUpdate();
        form.MouseLeave += (_, _) => SafeUpdate();

        foreach (Control ctrl in form.Controls)
        {
            ctrl.MouseEnter += (_, _) => SafeUpdate();
            ctrl.MouseLeave += (_, _) => SafeUpdate();
        }

        // Handle closing/disposal to prevent callbacks after it's gone
        form.FormClosing += (_, _) => form.HandleDestroyed -= OnHandleDestroyed;
        form.HandleDestroyed += OnHandleDestroyed;

        // Initialize once
        SafeUpdate();

        void OnHandleDestroyed(object? sender, EventArgs e)
        {
            // No-op: event unsubscribed during closing
        }
    }

    private static void Update(Form form, bool isActive, double inactiveOpacity)
    {
        // skip updates when handle or form is gone
        if (form.IsDisposed || !form.IsHandleCreated)
        {
            return;
        }

        try
        {
            bool mouseOver = false;
            if (form.IsHandleCreated)
            {
                mouseOver = form.ClientRectangle.Contains(form.PointToClient(Cursor.Position));
            }

            form.Opacity = (isActive || mouseOver) ? 1.0 : inactiveOpacity;
        }
        catch (ObjectDisposedException)
        {
            // ignore — form is closing
        }
        catch (Win32Exception)
        {
            // ignore invalid handle errors during shutdown
        }
    }
}