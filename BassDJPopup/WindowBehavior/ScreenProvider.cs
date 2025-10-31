namespace BassDJPopup.WindowBehavior;

public class ScreenProvider : IScreenProvider
{
    public Rectangle GetWorkingArea(Form form)
    {
        return Screen.FromHandle(form.Handle).WorkingArea;
    }
}
