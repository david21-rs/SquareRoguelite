using System.Text.Json;

namespace TheAdventure;
// AI-generated
public record SaveData(int HighScore);

public class SaveDataException : Exception
{
    public SaveDataException(string message, Exception inner)
        : base(message, inner)
    {
    }
}

public static class SaveSystem
{
    private static readonly string SavePath = Path.Combine(AppContext.BaseDirectory, "save.json");

    public static SaveData Load()
    {
        if (!File.Exists(SavePath))
        {
            return new SaveData(0);
        }

        try
        {
            return JsonSerializer.Deserialize<SaveData>(File.ReadAllText(SavePath)) ?? new SaveData(0);
        }
        catch (Exception e) when (e is IOException or JsonException)
        {
            throw new SaveDataException("save.json exists but could not be read", e);
        }
    }

    public static void Save(SaveData data)
    {
        try
        {
            File.WriteAllText(SavePath, JsonSerializer.Serialize(data));
        }
        catch (IOException e)
        {
            throw new SaveDataException("save.json could not be written", e);
        }
    }
}
// end AI-generated