namespace TrainingDiary.Storage;

/// <summary>
/// Де лежить щоденник користувача. Каталог застосунку для цього не годиться:
/// самодостатню збірку часто кладуть туди, куди звичайний користувач писати
/// не може, а під час оновлення вміст цього каталогу замінюють.
/// </summary>
public static class DiaryLocation
{
    /// <summary>
    /// Windows — %APPDATA%\TrainingDiary, Linux — ~/.config/TrainingDiary.
    /// У macOS — ~/Library/Application Support/TrainingDiary.
    /// </summary>
    public static string Folder => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "TrainingDiary");

    public static string DiaryFile => Path.Combine(Folder, "diary.json");
}
