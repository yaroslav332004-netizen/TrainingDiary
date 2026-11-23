namespace TrainingDiary.Desktop.ViewModels;

public class TextInput : AnswerInput
{
    private string text = "";

    public TextInput(string prompt) => Prompt = prompt;

    public string Prompt { get; }

    public string Text
    {
        get => text;
        // Очищене поле може передати null — CalculateLoad його не чекає.
        set { if (Set(ref text, value ?? "")) Raise(nameof(IsReady)); }
    }

    public override bool IsReady => !string.IsNullOrWhiteSpace(Text);

    public override string Given => Text;
}
