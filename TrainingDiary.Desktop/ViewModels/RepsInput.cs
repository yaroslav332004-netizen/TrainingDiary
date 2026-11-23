using TrainingDiary.Exercises;

namespace TrainingDiary.Desktop.ViewModels;

/// <summary>Вправа на повторення: обирають варіант виконання зі списку й вводять кількість.</summary>
public class RepsInput : AnswerInput
{
    private int selectedIndex;
    private string text = "";

    // Індексатор вправи нумерує варіанти від 1 — так їх і показуємо.
    public RepsInput(RepsExercise exercise)
    {
        Options = Enumerable.Range(1, exercise.Variants.Count)
            .Select(n => $"{n}. {exercise[n]}")
            .ToList();

        Prompt = exercise.Prompt;

        // Одразу позначаємо варіант, записаний у самій вправі.
        selectedIndex = Math.Clamp(exercise.Variant - 1, 0, Options.Count - 1);
    }

    public IReadOnlyList<string> Options { get; }

    public string Prompt { get; }

    public int SelectedIndex
    {
        get => selectedIndex;
        set { if (Set(ref selectedIndex, value)) Raise(nameof(IsReady)); }
    }

    public string Text
    {
        get => text;
        // Очищене поле може передати null — CalculateLoad його не чекає.
        set { if (Set(ref text, value ?? "")) Raise(nameof(IsReady)); }
    }

    public override bool IsReady =>
        SelectedIndex >= 0 && !string.IsNullOrWhiteSpace(Text);

    public override string Given => Text;

    // Список нумерує з 0, а варіанти вправи — з 1.
    public override void ApplyTo(Exercise exercise)
    {
        if (exercise is RepsExercise reps && SelectedIndex >= 0)
            reps.Variant = SelectedIndex + 1;
    }
}
