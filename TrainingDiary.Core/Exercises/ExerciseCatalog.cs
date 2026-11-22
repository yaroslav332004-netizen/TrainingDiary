using System.Reflection;

namespace TrainingDiary.Exercises;

public record ExerciseKindEntry(string Kind, string Title, Type Type);

public class ExerciseCatalog
{
    private readonly Dictionary<string, ExerciseKindEntry> byKind =
        new(StringComparer.OrdinalIgnoreCase);

    /// <summary>Каталог із різновидами самого ядра. Готовий одразу.</summary>
    public static ExerciseCatalog Default { get; } = CreateDefault();

    private static ExerciseCatalog CreateDefault()
    {
        var catalog = new ExerciseCatalog();
        catalog.Add(typeof(Exercise).Assembly);
        return catalog;
    }

    public IReadOnlyCollection<ExerciseKindEntry> Kinds => byKind.Values;

    /// <summary>Переглядає збірку й додає всі типи вправ із неї.</summary>
    public int Add(Assembly assembly)
    {
        var added = 0;

        foreach (var type in TypesOf(assembly))
        {
            if (type.IsAbstract || !typeof(Exercise).IsAssignableFrom(type))
                continue;

            var attribute =
                type.GetCustomAttribute<ExerciseKindAttribute>(inherit: false);

            if (attribute is null)
                continue;

            // Каталог створює вправи конструктором без параметрів.
            if (type.GetConstructor(Type.EmptyTypes) is null)
                throw new InvalidOperationException(
                    $"Класу {type.FullName} потрібен відкритий " +
                    "конструктор без параметрів.");

            byKind[attribute.Kind] =
                new ExerciseKindEntry(attribute.Kind, attribute.Title, type);
            added++;
        }

        return added;
    }

    public Exercise? Create(string kind) =>
        byKind.TryGetValue(kind, out var entry)
            ? (Exercise)Activator.CreateInstance(entry.Type)!
            : null;

    private static IEnumerable<Type> TypesOf(Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException ex)
        {
            // Частину типів завантажити не вдалося — беремо ті, що вдалося.
            return ex.Types.Where(t => t is not null)!;
        }
    }
}
