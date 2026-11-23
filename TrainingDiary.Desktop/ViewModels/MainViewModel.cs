using System.ComponentModel;
using TrainingDiary.Exercises;
using TrainingDiary.Ordering;
using TrainingDiary.Plugins;
using TrainingDiary.Storage;
using TrainingDiary.Workout;

namespace TrainingDiary.Desktop.ViewModels;

public class MainViewModel : ObservableBase
{
    private readonly string diaryFile;

    private WorkoutSession? session;
    private IReadOnlyList<Exercise> exercises = Array.Empty<Exercise>();
    private int index = -1;

    private string status = "Читаю щоденник…";
    private string title = "";
    private string rule = "";
    private string feedback = "";
    private string summary = "";
    private bool hasExercise;
    private bool answered;
    private AnswerInput? input;
    private bool? lastCorrect;
    private int done;

    public MainViewModel(string diaryFile)
    {
        this.diaryFile = diaryFile;

        CheckCommand = new RelayCommand(
            Check, () => HasExercise && !Answered && Input is { IsReady: true });
        NextCommand = new RelayCommand(Next, () => HasExercise && Answered);
    }

    public string Status
    {
        get => status;
        private set => Set(ref status, value);
    }

    public string Title
    {
        get => title;
        private set => Set(ref title, value);
    }

    public string Rule
    {
        get => rule;
        private set => Set(ref rule, value);
    }

    public string Feedback
    {
        get => feedback;
        private set => Set(ref feedback, value);
    }

    public string Summary
    {
        get => summary;
        private set => Set(ref summary, value);
    }

    public bool HasExercise
    {
        get => hasExercise;
        private set { if (Set(ref hasExercise, value)) RefreshCommands(); }
    }

    public bool Answered
    {
        get => answered;
        private set { if (Set(ref answered, value)) RefreshCommands(); }
    }

    public AnswerInput? Input
    {
        get => input;
        private set
        {
            var old = input;

            if (!Set(ref input, value))
                return;

            if (old is not null)
                old.PropertyChanged -= OnInputChanged;

            if (value is not null)
                value.PropertyChanged += OnInputChanged;

            RefreshCommands();
        }
    }

    // Щось обрали чи набрали — «Зарахувати» має перечитати CanExecute.
    private void OnInputChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(AnswerInput.IsReady))
            CheckCommand.RaiseCanExecuteChanged();
    }

    public bool IsCorrect => lastCorrect == true;
    public bool IsWrong => lastCorrect == false;

    public int Total => exercises.Count;
    public int Done { get => done; private set => Set(ref done, value); }

    // Обидві ознаки обчислюються з lastCorrect — сповіщаємо про обидві.
    private void SetResult(bool? correct)
    {
        lastCorrect = correct;
        Raise(nameof(IsCorrect));
        Raise(nameof(IsWrong));
    }

    public RelayCommand CheckCommand { get; }
    public RelayCommand NextCommand { get; }

    private void RefreshCommands()
    {
        CheckCommand.RaiseCanExecuteChanged();
        NextCommand.RaiseCanExecuteChanged();
    }

    // Читає щоденник. Викликають після того, як вікно вже показано.
    public async Task LoadAsync()
    {
        try
        {
            PluginLoader.LoadInto(ExerciseCatalog.Default);

            var storage = new JsonExerciseStorage(diaryFile);

            // Ознака першого запуску — відсутній файл, а не порожній перелік.
            if (!File.Exists(diaryFile))
                await storage.SaveAsync(SampleExerciseStorage.Exercises());

            session = new WorkoutSession(storage, new ByCategoryOrder());
            exercises = await session.ExercisesAsync();
            Raise(nameof(Total));

            Next();
        }
        catch (ExerciseStorageException ex)
        {
            Fail(ex.Message, "Виправте або видаліть файл щоденника.");
        }
        catch (Exception ex)
        {
            Fail("Не вдалося підготувати тренування.", ex.Message);
        }
    }

    private void Check()
    {
        if (session is null || Input is null)
            return;

        var exercise = exercises[index];

        // Уся перевірка — у ядрі. Тут лише запис результату до стану вікна.
        Input.ApplyTo(exercise);
        var load = session.Record(exercise, Input.Given);

        if (load is null)
        {
            Feedback = "Не вдалося розпізнати кількість — вправу пропущено.";
            SetResult(false);
        }
        else
        {
            Feedback = $"Зараховано: {load:0.##} балів.";
            SetResult(true);
        }

        Done = index + 1;
        Answered = true;
    }

    private void Next()
    {
        index++;

        SetResult(null);
        Feedback = "";
        Answered = false;

        if (index >= exercises.Count)
        {
            Finish();
            return;
        }

        var exercise = exercises[index];

        HasExercise = true;
        Status = $"Вправа {index + 1} з {exercises.Count} · {exercise.Category}";
        Title = exercise.Name;
        Rule = exercise.LoadRule;
        Input = AnswerInput.For(exercise);
    }

    private void Finish()
    {
        HasExercise = false;
        Input = null;
        Status = "Тренування завершено";

        if (session is null)
            return;

        var result = session.Summary;

        Summary = $"Виконано {result.Done}, пропущено {result.Skipped} · " +
                  $"навантаження {result.TotalLoad:0.##} б., " +
                  $"у середньому {result.AverageLoad:0.##}.";
    }

    private void Fail(string reason, string detail)
    {
        HasExercise = false;
        Status = reason;
        Feedback = detail;
    }
}
