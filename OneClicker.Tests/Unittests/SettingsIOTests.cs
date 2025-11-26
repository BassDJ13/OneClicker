using OneClicker.Settings.Ini;
using NSubstitute;
using BassCommon.FileSystem;
using PluginContracts;

namespace OneClicker.Tests.Unittests;

public class SettingsIOTests
{
    [Test]
    public void Save_Should_Write_All_Settings_To_FileSystem()
    {
        // Arrange
        var fakeFs = Substitute.For<IFileSystem>();
        var settings = new AppSettings
        {
            FolderPath = @"C:\TestFolder",
            X = 123,
            Y = 456,
            WidgetSize = 40,
            BackColor = Color.Red,
            ButtonColor = Color.Green,
            TriangleColor = Color.Blue
        };

        var sut = new IniSettingsStorage("config.ini", settings, fakeFs);

        // Act
        sut.Save();

        // Assert
        fakeFs.Received(1).WriteAllLines(
            "config.ini",
            Arg.Is<IEnumerable<string>>(lines =>
                lines.Contains("Folder=C:\\TestFolder") &&
                lines.Contains("X=123") &&
                lines.Contains("Y=456") &&
                lines.Contains("WidgetSize=40") &&
                lines.Contains($"BackColor=#{Color.Red.R:X2}{Color.Red.G:X2}{Color.Red.B:X2}")
            ));
    }

    [Test]
    public void Load_Should_Apply_Values_From_FileSystem()
    {
        // Arrange
        var fakeFs = Substitute.For<IFileSystem>();
        fakeFs.Exists(Arg.Any<string>()).Returns(true);
        fakeFs.ReadAllLines(Arg.Any<string>()).Returns(new[]
        {
            "Folder=C:\\TestFolder",
            "X=10",
            "Y=20",
            "WidgetSize=30",
            "BackColor=#FF0000",
            "ButtonColor=#00FF00",
            "TriangleColor=#0000FF"
        });

        var settings = new AppSettings();
        var sut = new IniSettingsStorage("config.ini", settings, fakeFs);

        // Act
        sut.Load();

        // Assert
        Assert.That(settings.FolderPath, Is.EqualTo(@"C:\TestFolder"));
        Assert.That(settings.X, Is.EqualTo(10));
        Assert.That(settings.Y, Is.EqualTo(20));
        Assert.That(settings.WidgetSize, Is.EqualTo(30));
        Assert.That(settings.BackColor.ToArgb(), Is.EqualTo(Color.FromArgb(255, 0, 0).ToArgb()));
        Assert.That(settings.ButtonColor.ToArgb(), Is.EqualTo(Color.FromArgb(0, 255, 0).ToArgb()));
        Assert.That(settings.TriangleColor.ToArgb(), Is.EqualTo(Color.FromArgb(0, 0, 255).ToArgb()));
    }
}
