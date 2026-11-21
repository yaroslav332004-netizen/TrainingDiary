using TrainingDiary.Storage;

Console.OutputEncoding = System.Text.Encoding.UTF8;

// Без аргументу беремо щоденник із каталогу даних користувача — на будь-якій системі.
var file = args.Length > 0 ? args[0] : DiaryLocation.DiaryFile;

if (!File.Exists(file))
{
    Console.WriteLine($"Файлу щоденника немає: {file}");
    return 1;
}

var exercises = await new JsonExerciseStorage(file).LoadAsync();

Console.WriteLine($"Вправ: {exercises.Count}");

foreach (var group in exercises.GroupBy(e => e.Category).OrderBy(g => g.Key))
    Console.WriteLine($" {group.Key}: {group.Count()}");

return 0;
