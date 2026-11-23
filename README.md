# TrainingDiary — Щоденник тренувань

Тема проєкту: **«Щоденник тренувань»** (тема №16 з переліку). Застосунок веде облік вправ трьох видів — на час, на повторення й на дистанцію — і для кожної обчислює навантаження.

Це навчальний проєкт з дисципліни «Крос-платформне програмування» (ЖДУ імені Івана Франка). Він збирається й запускається у Windows, macOS і Linux.

## Структура

- `TrainingDiary.Core/` — бібліотека класів: вправи, порядок перебору, сховище, ядро сеансу. Без жодного коду для виведення.
- `TrainingDiary.App/` — консольна оболонка на Spectre.Console.
- `TrainingDiary.Desktop/` — віконна оболонка на Avalonia UI над тим самим ядром.
- `plugins/TrainingDiary.Exercises.Interval/` — складник із четвертим різновидом (вправа на інтервали). Застосунок на нього не посилається.
- `tools/DiaryReport/` — другий застосунок, який підключає бібліотеку пакетом NuGet, а не проєктом.

## Як запустити

Потрібен .NET SDK 10.

    dotnet run --project TrainingDiary.App

Відомості про систему й фактичні шляхи (каталог застосунку, каталог даних):

    dotnet run --project TrainingDiary.App -- --info

Файл даних лежить у каталозі даних користувача: `%APPDATA%\TrainingDiary` у Windows, `~/.config/TrainingDiary` у Linux; у macOS `~/Library/Application Support/TrainingDiary`.

## Дві оболонки над одним ядром

Ядро (`TrainingDiary.Core`) не знає ні про консоль, ні про вікно, і заради віконної оболонки в ньому не змінено жодного рядка. Консольна й віконна оболонки обидві звертаються до тих самих класів: `WorkoutSession`, `Exercise` та його різновидів. Вправи перевіряє й навантаження рахує ядро, тому результати в обох оболонках збігаються. Оболонка відповідає лише за вигляд і за введення.

- Консоль: `dotnet run --project TrainingDiary.App`
- Вікно: `dotnet run --project TrainingDiary.Desktop`

У віконній оболонці стан описано моделлю подання (`ViewModels/`, без жодного типу Avalonia), розмітку `MainWindow.axaml` з’єднано з нею прив’язками, а кнопки — з командами. Кожен різновид вправи має власний спосіб введення, який розмітка добирає шаблоном даних за типом: вправа на повторення — список варіантів і поле кількості, решта вправ — поле введення. Код вікна `MainWindow.axaml.cs` містить лише конструктор.

**Знімки:** *[додати знімок вікна й знімок консолі]*

Для віконної оболонки в Linux потрібні системні бібліотеки: `libx11-6`, `libice6`, `libsm6`, `libfontconfig1`.

## Різновиди вправ і складники

Різновиди позначено атрибутом `[ExerciseKind("позначка", "назва")]`, а каталог знаходить їх рефлексією. Показати відомі різновиди й те, з якої збірки кожен узявся:

    dotnet run --project TrainingDiary.App -- --kinds

Щоб установити складник, зберіть його й скопіюйте `.dll` у каталог `plugins` поруч із застосунком (ядро й застосунок перезбирати не треба).

macOS і Linux:

    dotnet build TrainingDiary.App -c Debug
    dotnet build plugins/TrainingDiary.Exercises.Interval -c Debug
    mkdir -p TrainingDiary.App/bin/Debug/net10.0/plugins
    cp plugins/TrainingDiary.Exercises.Interval/bin/Debug/net10.0/TrainingDiary.Exercises.Interval.dll \
       TrainingDiary.App/bin/Debug/net10.0/plugins/
    dotnet run --project TrainingDiary.App --no-build -- --kinds

Windows (PowerShell):

    dotnet build TrainingDiary.App -c Debug
    dotnet build plugins/TrainingDiary.Exercises.Interval -c Debug
    $target = "TrainingDiary.App/bin/Debug/net10.0/plugins"
    New-Item -ItemType Directory -Force -Path $target
    Copy-Item plugins/TrainingDiary.Exercises.Interval/bin/Debug/net10.0/TrainingDiary.Exercises.Interval.dll -Destination $target
    dotnet run --project TrainingDiary.App --no-build -- --kinds

## Публікація

Один вихідний текст — три збірки. Самодостатня збірка не потребує встановленого .NET.

    ./publish.sh        # macOS і Linux
    ./publish.ps1       # Windows (PowerShell)

Кожна оболонка публікується у власний каталог: `publish/<RID>/TrainingDiary.App` і `publish/<RID>/TrainingDiary.Desktop`.

| RID | Для кого | Консоль | Вікно |
|---|---|---|---|
| `win-x64` | Windows, 64-розрядні процесори | `publish/win-x64/TrainingDiary.App/diary.exe` | `publish/win-x64/TrainingDiary.Desktop/diary-ui.exe` |
| `osx-arm64` | macOS на процесорах Apple | `publish/osx-arm64/TrainingDiary.App/diary` | `publish/osx-arm64/TrainingDiary.Desktop/diary-ui` |
| `linux-x64` | Linux, 64-розрядні процесори | `publish/linux-x64/TrainingDiary.App/diary` | `publish/linux-x64/TrainingDiary.Desktop/diary-ui` |

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
