using BassDJPopup.WindowBehavior;
using NSubstitute;

namespace BassDJPopup.Tests.Unittests;

public class TopMostHelperTests
{
    [Test]
    public void HandleMessage_WhenInactive_ShouldSetTopMost()
    {
        var positioner = Substitute.For<IWindowPositioner>();
        var helper = new TopMostHelper(positioner);
        var form = new Form();

        var msg = Message.Create(form.Handle, 0x0006, nint.Zero, nint.Zero); // WM_ACTIVATE, WA_INACTIVE

        helper.HandleMessage(form, ref msg);

        positioner.Received(1).SetTopMost(form.Handle);
    }

    [Test]
    public void HandleMessage_WhenActive_ShouldNotSetTopMost()
    {
        var positioner = Substitute.For<IWindowPositioner>();
        var helper = new TopMostHelper(positioner);
        var form = new Form();

        var msg = Message.Create(form.Handle, 0x0006, new nint(1), nint.Zero); // WM_ACTIVATE, active

        helper.HandleMessage(form, ref msg);

        positioner.DidNotReceive().SetTopMost(Arg.Any<nint>());
    }
}
