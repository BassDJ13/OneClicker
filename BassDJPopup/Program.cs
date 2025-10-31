using BassDJPopup.Forms;

namespace QuickFolderPopup;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.Run(new MainForm());
    }
}
