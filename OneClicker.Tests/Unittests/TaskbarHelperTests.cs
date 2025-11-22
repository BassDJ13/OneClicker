using OneClicker.WindowBehavior;
using NSubstitute;

namespace OneClicker.Tests.Unittests;

public class TaskbarHelperTests
{
    [Test]
    public void KeepInWorkArea_ShouldClampFormInsideWorkingArea()
    {
        var screenProvider = Substitute.For<IScreenProvider>();
        screenProvider.GetWorkingArea(Arg.Any<Form>())
                      .Returns(new Rectangle(0, 0, 100, 100));

        var helper = new WindowLocationHelper(screenProvider);
        var form = new Form
        {
            FormBorderStyle = FormBorderStyle.None,
            Width = 20,
            Height = 20,
            Left = 95,
            Top = 95
        };

        helper.KeepInWorkArea(form);

        Assert.That(form.Left, Is.EqualTo(80)); // 100 - 20
        Assert.That(form.Top, Is.EqualTo(80));  // 100 - 20
    }

    [Test]
    public void DockAboveTaskbar_ShouldPositionBottomRightOfWorkingArea()
    {
        var screenProvider = Substitute.For<IScreenProvider>();
        screenProvider.GetWorkingArea(Arg.Any<Form>())
                      .Returns(new Rectangle(0, 0, 200, 200));

        var helper = new WindowLocationHelper(screenProvider);
        var form = new Form
        {
            FormBorderStyle = FormBorderStyle.None,
            Width = 20,
            Height = 20 
        };

        form.Location = helper.GetDockedPosition(screenProvider.GetWorkingArea(form), form.Size, PluginContracts.DockPosition.BottomRight);

        Assert.That(form.Left, Is.EqualTo(200 - 20 - 16));
        Assert.That(form.Top, Is.EqualTo(200 - 20 - 4));
    }
}
