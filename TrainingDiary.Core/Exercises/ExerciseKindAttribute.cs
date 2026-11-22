namespace TrainingDiary.Exercises;

[AttributeUsage(AttributeTargets.Class,
                AllowMultiple = false, Inherited = false)]
public sealed class ExerciseKindAttribute : Attribute
{
    public ExerciseKindAttribute(string kind, string title)
    {
        Kind = kind;
        Title = title;
    }

    /// <summary>Коротка позначка, яка потрапляє у файл щоденника.</summary>
    public string Kind { get; }

    /// <summary>Назва різновиду для людини.</summary>
    public string Title { get; }
}
