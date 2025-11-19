namespace BassCommon.FileSystem;

public interface IFileSystem
{
    bool Exists(string path);
    string[] ReadAllLines(string path);
    void WriteAllLines(string path, IEnumerable<string> lines);
}