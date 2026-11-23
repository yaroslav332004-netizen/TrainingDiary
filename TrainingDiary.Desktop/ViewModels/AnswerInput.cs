using TrainingDiary.Exercises;

namespace TrainingDiary.Desktop.ViewModels;

public abstract class AnswerInput : ObservableBase
{
    // Кількість рядком — саме такий вигляд приймає Exercise.CalculateLoad.
    public abstract string Given { get; }

    // Чи є що перевіряти: від цього залежить кнопка «Зарахувати».
    public abstract bool IsReady { get; }

    // Переносить вибір користувача на саму вправу. Типово нічого не робить.
    public virtual void ApplyTo(Exercise exercise) { }

    public static AnswerInput For(Exercise exercise) => exercise switch
    {
        RepsExercise reps when reps.Variants.Count > 1
            => new RepsInput(reps),
        _ => new TextInput(exercise.Prompt)
    };
}
