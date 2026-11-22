using TrainingDiary.Exercises;

namespace TrainingDiary.Exercises.Interval;

/// <summary>Вправа на інтервали: рахуємо раунди, у навантаження йде лише робочий час.</summary>
[ExerciseKind("interval", "На інтервали")]
public class IntervalExercise : Exercise
{
    /// <summary>Скільки балів дає хвилина роботи.</summary>
    public double PointsPerMinute { get; set; }

    /// <summary>Тривалість роботи в одному раунді, с.</summary>
    public double WorkSeconds { get; set; }

    /// <summary>Тривалість відпочинку між раундами, с (у навантаження не входить).</summary>
    public double RestSeconds { get; set; }

    public IntervalExercise() { }

    public IntervalExercise(int id, string name, double pointsPerMinute,
                            double workSeconds, double restSeconds)
        : base(id, name)
    {
        PointsPerMinute = pointsPerMinute;
        WorkSeconds = workSeconds;
        RestSeconds = restSeconds;
    }

    public override double? CalculateLoad(string given)
    {
        // Раундів має бути ціле невід’ємне число.
        if (!TryParseNumber(given, out var rounds) || rounds < 0 || rounds != Math.Floor(rounds))
            return null;

        return rounds * WorkSeconds / 60 * PointsPerMinute;
    }

    public override string LoadRule =>
        $"{PointsPerMinute:0.##} б. за хвилину роботи; раунд: {WorkSeconds:0.##} с роботи, {RestSeconds:0.##} с відпочинку";

    public override string Prompt => "Кількість раундів";

    public override void WriteData(IDictionary<string, string> data)
    {
        data["pointsPerMinute"] = WriteNumber(PointsPerMinute);
        data["workSeconds"] = WriteNumber(WorkSeconds);
        data["restSeconds"] = WriteNumber(RestSeconds);
    }

    public override void ReadData(IReadOnlyDictionary<string, string> data)
    {
        PointsPerMinute = ReadNumber(data, "pointsPerMinute");
        WorkSeconds = ReadNumber(data, "workSeconds");
        RestSeconds = ReadNumber(data, "restSeconds");
    }
}
