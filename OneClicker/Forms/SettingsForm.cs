using OneClicker.Settings;

namespace OneClicker.Forms;

public class SettingsForm : Form
{
    public SettingsForm()
    {
        var settings = AppSettings.Instance;
        Text = "OneClicker Settings v1.0.1";
        FormBorderStyle = FormBorderStyle.FixedDialog;
        StartPosition = FormStartPosition.CenterParent;
        ClientSize = new Size(368, 212);
        MaximizeBox = false;
        MinimizeBox = false;

        // --- Folder ---
        var lblFolder = new Label { Text = "Folder:", Left = 10, Top = 20, Width = 50 };
        var txtFolder = new TextBox { Left = 70, Top = 18, Width = 220, Text = settings.FolderPath };
        var btnBrowse = new Button { Text = "Browse...", Left = 290, Top = 18, Width = 65 };
        var btnOpen = new Button { Text = "Open in explorer", Left = 70, Top = 44, Width = 110 };

        btnBrowse.Click += (s, e) =>
        {
            using var dialog = new FolderBrowserDialog
            {
                Description = "Select folder to display",
                SelectedPath = txtFolder.Text
            };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                txtFolder.Text = dialog.SelectedPath;
            }
        };

        btnOpen.Click += (s, e) =>
        {
            if (Directory.Exists(txtFolder.Text))
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(txtFolder.Text) { UseShellExecute = true });
            }
        };

        // --- Colors ---
        var lblBack = new Label { Text = "Header:", Left = 10, Top = 90, Width = 60 };
        var lblButton = new Label { Text = "Button:", Left = 10, Top = 115, Width = 60 };
        var lblTriangle = new Label { Text = "Arrow:", Left = 10, Top = 140, Width = 60 };

        var btnBack = new Button { Left = 70, Top = 85, Width = 22, BackColor = settings.BackColor };
        var btnButton = new Button { Left = 70, Top = 110, Width = 22, BackColor = settings.ButtonColor };
        var btnTriangle = new Button { Left = 70, Top = 135, Width = 22, BackColor = settings.TriangleColor };

        btnBack.Click += (s, e) => PickColor(btnBack);
        btnButton.Click += (s, e) => PickColor(btnButton);
        btnTriangle.Click += (s, e) => PickColor(btnTriangle);

        // --- Size ---
        var lblWidth = new Label { Text = "Width:", Left = 175, Top = 90, Width = 50 };
        var numWidth = new NumericUpDown { Left = 230, Top = 87, Width = 60, Minimum = 8, Maximum = 960, Value = Math.Max(8, settings.Width) };
        var lblHeight = new Label { Text = "Height:", Left = 175, Top = 115, Width = 50 };
        var numHeight = new NumericUpDown { Left = 230, Top = 112, Width = 60, Minimum = 12, Maximum = 540, Value = Math.Max(12, settings.Height) };

        // --- OK/Cancel ---
        var okBtn = new Button { Text = "Save", Left = 226, Width = 60, Top = 175, DialogResult = DialogResult.OK };
        var cancelBtn = new Button { Text = "Cancel", Left = 296, Width = 60, Top = 175, DialogResult = DialogResult.Cancel };
        AcceptButton = okBtn;
        CancelButton = cancelBtn;

        Controls.AddRange(new Control[]
        {
            lblFolder, txtFolder, btnBrowse, btnOpen,
            lblBack, lblButton, lblTriangle,
            btnBack, btnButton, btnTriangle,
            lblWidth, numWidth,
            lblHeight, numHeight,
            okBtn, cancelBtn
        });

        // --- Handle OK ---
        okBtn.Click += (s, e) =>
        {
            if (!Directory.Exists(txtFolder.Text))
            {
                MessageBox.Show("Selected folder does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                DialogResult = DialogResult.None;
                return;
            }

            settings.FolderPath = txtFolder.Text;
            settings.BackColor = btnBack.BackColor;
            settings.ButtonColor = btnButton.BackColor;
            settings.TriangleColor = btnTriangle.BackColor;
            settings.Width = (int)numWidth.Value;
            settings.Height = (int)numHeight.Value;
        };
    }

    private static void PickColor(Button btn)
    {
        var cd = new ColorDialog
        {
            Color = btn.BackColor
        };

        if (cd.ShowDialog() == DialogResult.OK)
        {
            btn.BackColor = cd.Color;
        }
    }
}
