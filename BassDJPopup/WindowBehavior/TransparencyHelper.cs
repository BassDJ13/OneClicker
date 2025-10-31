namespace BassDJPopup.WindowBehavior;

public static class TransparencyHelper
{
    public static void AttachAutoOpacity(Form form, double inactiveOpacity = 0.75)
    {
        if (form is null)
            throw new ArgumentNullException(nameof(form));

        bool isActive = true;

        // Attach event handlers
        form.Activated += (_, _) => { isActive = true; Update(form, isActive, inactiveOpacity); };
        form.Deactivate += (_, _) => { isActive = false; Update(form, isActive, inactiveOpacity); };
        form.MouseEnter += (_, _) => Update(form, isActive, inactiveOpacity);
        form.MouseLeave += (_, _) => Update(form, isActive, inactiveOpacity);

        // Also handle child controls
        foreach (Control ctrl in form.Controls)
        {
            ctrl.MouseEnter += (_, _) => Update(form, isActive, inactiveOpacity);
            ctrl.MouseLeave += (_, _) => Update(form, isActive, inactiveOpacity);
        }

        // Initialize once
        Update(form, isActive, inactiveOpacity);
    }

    private static void Update(Form form, bool isActive, double inactiveOpacity)
    {
        if (form.IsDisposed) return;

        bool mouseOver = form.ClientRectangle.Contains(form.PointToClient(Cursor.Position));
        form.Opacity = isActive || mouseOver ? 1.0 : inactiveOpacity;
    }
}
