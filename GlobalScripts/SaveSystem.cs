using Godot;

public static class SaveSystem
{
    private static readonly string SavePath = "user://savegame.save";
    //C:\Users\ondre\AppData\Roaming\Godot\app_userdata\Zapoctova Hra\savegame.save

    public static void SaveInt(int value)
    {
        using var file = FileAccess.Open(SavePath, FileAccess.ModeFlags.Write);
        if (file != null)
        {
            file.Store32((uint)value);
        }
    }

    public static int LoadInt()
    {
        if (!FileAccess.FileExists(SavePath)) return 0;

        using var file = FileAccess.Open(SavePath, FileAccess.ModeFlags.Read);
        if (file != null)
        {
            return (int)file.Get32();
        }
        return 0;
    }
}