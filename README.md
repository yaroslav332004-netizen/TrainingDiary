# TrainingDiary — Щоденник тренувань

Тема проєкту: **«Щоденник тренувань»** (тема №16 з переліку). Застосунок веде облік вправ трьох видів — на час, на повторення й на дистанцію — і для кожної обчислює навантаження.

Це навчальний проєкт з дисципліни «Крос-платформне програмування» (ЖДУ імені Івана Франка). Він збирається й запускається у Windows, macOS і Linux.

## Структура

- `TrainingDiary.Core/` — бібліотека класів: вправи, порядок перебору, сховище, ядро сеансу. Без жодного коду для виведення.
- `TrainingDiary.App/` — консольна оболонка на Spectre.Console.
- `tools/DiaryReport/` — другий застосунок, який підключає бібліотеку пакетом NuGet, а не проєктом.

## Як запустити

Потрібен .NET SDK 10.

    dotnet run --project TrainingDiary.App

Відомості про систему й фактичні шляхи (каталог застосунку, каталог даних):

    dotnet run --project TrainingDiary.App -- --info

Файл даних лежить у каталозі даних користувача: `%APPDATA%\TrainingDiary` у Windows, `~/.config/TrainingDiary` у Linux; у macOS `~/Library/Application Support/TrainingDiary`.

## Публікація

Один вихідний текст — три збірки. Самодостатня збірка не потребує встановленого .NET.

    ./publish.sh        # macOS і Linux
    ./publish.ps1       # Windows (PowerShell)

| RID | Для кого | Що запускати |
|---|---|---|
| `win-x64` | Windows, 64-розрядні процесори | `publish/win-x64/diary.exe` |
| `osx-arm64` | macOS на процесорах Apple | `publish/osx-arm64/diary` |
| `linux-x64` | Linux, 64-розрядні процесори | `publish/linux-x64/diary` |

Перешкоди під час запуску:

- **Linux і macOS: право на виконання.** Після пересилання архівом файл втрачає ознаку «виконуваний»: `chmod +x diary`.
- **macOS: карантин.** Файл, завантажений з мережі, не запуститься, доки не виконати `xattr -d com.apple.quarantine diary`.
- **Linux: бібліотека ICU.** У мінімальних образах її немає, і застосунок завершується ще до першого рядка коду. Установіть `libicu` (Debian/Ubuntu) або `icu-libs` (Alpine). `InvariantGlobalization` не вмикаю, щоб не втратити порівняння рядків за правилами української мови.

## Пакет бібліотеки

    dotnet pack TrainingDiary.Core -c Release -o ./packages
    dotnet run --project tools/DiaryReport

Без аргументу застосунок бере щоденник із каталогу даних користувача; інший файл можна вказати аргументом.

Локальне джерело пакетів описано в `nuget.config`. Після зміни бібліотеки збільшіть `Version` у `TrainingDiary.Core.csproj`, запакуйте знову й оновіть версію в `tools/DiaryReport/DiaryReport.csproj`.

## Карта назв

| Роль у кістяку | У прикладі (CardTrainer) | Мій варіант (TrainingDiary) |
|---|---|---|
| Основний тип | Card | Exercise |
| Різновид 1 | OpenAnswerCard | TimedExercise (на час) |
| Різновид 2 | ChoiceCard | RepsExercise (на повторення) |
| Різновид 3 | NumericCard | DistanceExercise (на дистанцію) |
| Спільна дія | Check(given) | CalculateLoad(amount) |
| Набір записів | List<Card> deck | List<Exercise> exercises |
| Сховище | ICardStorage | IExerciseStorage |
| Порядок перебору | IReviewStrategy | IExerciseOrder |
| Підсумок | SessionSummary | WorkoutSummary |
| Різновид 4 (ЛР 8) | TrueFalseCard | IntervalExercise (на інтервали) |
