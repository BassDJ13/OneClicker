namespace BassDJPopup.Settings;

public interface IAppSettings
{
    static abstract IAppSettings Instance { get; }
    Color BackColor { get; set; }
    Color ButtonColor { get; set; }
    string FolderPath { get; set; }
    int Height { get; set; }
    Color TriangleColor { get; set; }
    int Width { get; set; }
    int X { get; set; }
    int Y { get; set; }

    //bool Equals(object? obj);
    //bool Equals(IAppSettings? other);
    //int GetHashCode();
    //string ToString();
}