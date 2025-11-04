using System.Text.Json;
using System.Text.Json.Serialization;

namespace OneClicker.Settings.Json;

public class JsonColorConverter : JsonConverter<Color>
{
    public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var hex = reader.GetString() ?? "#000000";
        try 
        { 
            return ColorTranslator.FromHtml(hex);
        }
        catch 
        { 
            return Color.Black;
        }
    }

    public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options)
    {
        writer.WriteStringValue($"#{value.R:X2}{value.G:X2}{value.B:X2}");
    }
}
