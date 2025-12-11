using System.Runtime.CompilerServices;

namespace OneClicker.WindowBehavior;

public static class TransparencyHelper
{
    private class OpacityState
    {
        public Form Form = null!;
        public bool IsActive;
        public double InactiveOpacity;
        public List<Control> SubscribedControls = new();
    }

    private static readonly ConditionalWeakTable<Form, OpacityState> States = new();

    public static void AttachAutoOpacity(Form form, double inactiveOpacity)
    {
        ArgumentNullException.ThrowIfNull(form);

        if (States.TryGetValue(form, out _))
        {
            return;
        }

        var state = new OpacityState
        {
            Form = form,
            IsActive = true,
            InactiveOpacity = inactiveOpacity
        };

        States.Add(form, state);

        form.Activated += Form_Activated;
        form.Deactivate += Form_Deactivate;
        form.MouseEnter += Form_MouseEnter;
        form.MouseLeave += Form_MouseLeave;
        form.FormClosing += Form_FormClosing;
        form.HandleDestroyed += Form_HandleDestroyed;

        foreach (Control ctrl in form.Controls)
        {
            ctrl.MouseEnter += Control_MouseEnter;
            ctrl.MouseLeave += Control_MouseLeave;
            state.SubscribedControls.Add(ctrl);
        }

        UpdateOpacity(state);
    }

    public static void SetInactiveOpacity(Form form, double inactiveOpacity)
    {
        if (!States.TryGetValue(form, out var state))
        {
            return;
        }

        state.InactiveOpacity = inactiveOpacity;
        UpdateOpacity(state);
    }

    public static void UpdateOpacity(Form form)
    {
        if (States.TryGetValue(form, out var state))
        {
            UpdateOpacity(state);
        }
    }

    private static void UpdateOpacity(OpacityState state)
    {
        var form = state.Form;

        if (form.IsDisposed || !form.IsHandleCreated)
        {
            return;
        }

        bool mouseOver = false;

        try
        {
            mouseOver = form.ClientRectangle.Contains(form.PointToClient(Cursor.Position));
        }
        catch
        {
            return;
        }

        try
        {
            form.Opacity = (state.IsActive || mouseOver) ? 1.0 : state.InactiveOpacity;
        }
        catch
        {
        }
    }

    private static void Form_Activated(object? sender, EventArgs e)
    {
        if (sender is not Form form)
        {
            return;
        }

        if (!States.TryGetValue(form, out var state))
        {
            return;
        }

        if (form.IsDisposed || !form.IsHandleCreated)
        {
            return;
        }

        state.IsActive = true;
        UpdateOpacity(state);
    }

    private static void Form_Deactivate(object? sender, EventArgs e)
    {
        if (sender is not Form form)
        {
            return;
        }

        if (!States.TryGetValue(form, out var state))
        {
            return;
        }

        if (form.IsDisposed)
        {
            return;
        }

        if (!form.IsHandleCreated)
        {
            return;
        }

        state.IsActive = false;
        UpdateOpacity(state);
    }

    private static void Form_MouseEnter(object? sender, EventArgs e)
    {
        if (sender is not Form form)
        {
            return;
        }

        if (!States.TryGetValue(form, out var state))
        {
            return;
        }

        if (form.IsDisposed || !form.IsHandleCreated)
        {
            return;
        }

        UpdateOpacity(state);
    }

    private static void Form_MouseLeave(object? sender, EventArgs e)
    {
        if (sender is not Form form)
        {
            return;
        }

        if (!States.TryGetValue(form, out var state))
        {
            return;
        }

        if (form.IsDisposed || !form.IsHandleCreated)
        {
            return;
        }

        UpdateOpacity(state);
    }

    private static void Control_MouseEnter(object? sender, EventArgs e)
    {
        if (sender is not Control ctrl)
        {
            return;
        }

        var form = ctrl.FindForm();
        if (form is null)
        {
            return;
        }

        if (!States.TryGetValue(form, out var state))
        {
            return;
        }

        if (form.IsDisposed || !form.IsHandleCreated)
        {
            return;
        }

        UpdateOpacity(state);
    }

    private static void Control_MouseLeave(object? sender, EventArgs e)
    {
        if (sender is not Control ctrl)
        {
            return;
        }

        var form = ctrl.FindForm();
        if (form is null)
        {
            return;
        }

        if (!States.TryGetValue(form, out var state))
        {
            return;
        }

        if (form.IsDisposed || !form.IsHandleCreated)
        {
            return;
        }

        UpdateOpacity(state);
    }

    private static void Form_FormClosing(object? sender, FormClosingEventArgs e)
    {
        if (sender is not Form form)
        {
            return;
        }

        if (!States.TryGetValue(form, out var state))
        {
            return;
        }

        form.Activated -= Form_Activated;
        form.Deactivate -= Form_Deactivate;
        form.MouseEnter -= Form_MouseEnter;
        form.MouseLeave -= Form_MouseLeave;
        form.FormClosing -= Form_FormClosing;
        form.HandleDestroyed -= Form_HandleDestroyed;

        foreach (var ctrl in state.SubscribedControls)
        {
            try
            {
                ctrl.MouseEnter -= Control_MouseEnter;
                ctrl.MouseLeave -= Control_MouseLeave;
            }
            catch
            {
            }
        }

        state.SubscribedControls.Clear();
        States.Remove(form);
    }

    private static void Form_HandleDestroyed(object? sender, EventArgs e)
    {
    }
}
