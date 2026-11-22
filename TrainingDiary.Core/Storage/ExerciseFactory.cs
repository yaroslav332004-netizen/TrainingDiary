using TrainingDiary.Exercises;

namespace TrainingDiary.Storage;

internal class ExerciseFactory
{
    private readonly ExerciseCatalog catalog;

    public ExerciseFactory(ExerciseCatalog catalog) => this.catalog = catalog;

    // Перестав бути static: тепер користується полем catalog.
    public Exercise? FromRecord(ExerciseRecord record)
    {
        // Замість switch — запит до каталогу.
        var exercise = catalog.Create(record.Kind);

        if (exercise is null)
            return null;

        exercise.Id = record.Id;
        exercise.Name = record.Name;
        exercise.Category = record.Category;
        exercise.ReadData(record.Data);

        return exercise;
    }

    // ToRecord лишається static: каталог йому не потрібен.
    public static ExerciseRecord ToRecord(Exercise exercise)
    {
        var record = new ExerciseRecord
        {
            Id = exercise.Id,
            Kind = exercise.Kind,
            Name = exercise.Name,
            Category = exercise.Category
        };

        exercise.WriteData(record.Data);

        return record;
    }
}
