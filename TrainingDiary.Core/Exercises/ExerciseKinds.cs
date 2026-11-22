using System.Collections.Concurrent;
using System.Reflection;

namespace TrainingDiary.Exercises;

public static class ExerciseKinds
{
    // Пошук атрибута повільний, а відповідь для типу незмінна:
    // кожен тип перевіряємо один раз.
    private static readonly ConcurrentDictionary<Type, ExerciseKindAttribute>
        Known = new();

    public static ExerciseKindAttribute Describe(Type type) =>
        Known.GetOrAdd(type, Read);

    private static ExerciseKindAttribute Read(Type type)
    {
        var attribute =
            type.GetCustomAttribute<ExerciseKindAttribute>(inherit: false);

        if (attribute is null)
            throw new InvalidOperationException(
                $"До класу {type.Name} не додано атрибут [ExerciseKind].");

        return attribute;
    }

    public static string Of(Type type) => Describe(type).Kind;
}
