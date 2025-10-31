namespace BassDJPopup.WindowBehavior;

public class TopMostHelper
{
    private readonly IWindowPositioner _positioner;

    public TopMostHelper(IWindowPositioner positioner)
    {
        _positioner = positioner;
    }

    public void HandleMessage(Form form, ref Message m)
    {
        const int WM_ACTIVATE = 0x0006;
        const int WA_INACTIVE = 0;

        if (m.Msg == WM_ACTIVATE && m.WParam.ToInt32() == WA_INACTIVE)
        {
            _positioner.SetTopMost(form.Handle);
        }
    }

    public void KeepOnTop(nint handle)
    {
        _positioner.SetTopMost(handle);
    }
}
