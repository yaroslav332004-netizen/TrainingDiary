## ЛР 7

- У `TrainingDiary.App.csproj` додано `<RuntimeIdentifiers>win-x64;osx-arm64;linux-x64</RuntimeIdentifiers>` і явне `<InvariantGlobalization>false</InvariantGlobalization>` (це типове значення, пишу лише щоб було видно, що режим без правил мов вимкнено навмисно).
- Файл щоденника перенесено з каталогу застосунку до каталогу даних користувача: `DiaryLocation` у бібліотеці (`Environment.GetFolderPath(SpecialFolder.ApplicationData)` плюс `TrainingDiary`). У Windows це `%APPDATA%\TrainingDiary`, у Linux `~/.config/TrainingDiary`, а в macOS `~/Library/Application Support/TrainingDiary` (це показав `--info` на GitHub Actions; у `~/.config` на macOS файл не знайшовся). Каталог застосунку не годиться: у Program Files звичайний користувач писати не може, а під час оновлення вміст каталогу замінюють.
- `PlatformInfo` лежить у каталозі `Platform` (не `Environment`: інакше простір імен затінив би клас `System.Environment`, і `Environment.GetFolderPath` перестав би збиратися). Тому в `Describe` написано повне `System.Environment.NewLine`. Звіт складає ядро, а рамку для нього малює оболонка.
- Ключ `--info` обробляється одразу після `Console.OutputEncoding`, інакше у Windows звіт вивівся б у неправильному кодуванні. Заголовок сеансу тепер містить назву системи (`OsName`).
- У бібліотеку додано відкритий клас `DiaryLocation`, тому підняв `Version` у `TrainingDiary.Core.csproj` до `1.1.0` і оновив версію в `tools/DiaryReport/DiaryReport.csproj`. Якби лишилася `1.0.0`, NuGet узяв би з кешу старий пакет без `DiaryLocation`, і другий застосунок не зібрався б (CS0103): я на цьому й спіткнувся під час перевірки.
- `publish.sh` і `publish.ps1` публікують під три RID командою `dotnet publish ... --self-contained true`. Вихідний текст при цьому не змінюється: тричі запускається та сама команда з різним RID.

### Виведення `--info` на трьох системах

Windows і macOS — з GitHub Actions (кроки «Відомості про систему (--info)» в задачах `build`), Linux — мій власний запуск.

| | Linux (Ubuntu 24.04) | Windows (GitHub Actions) | macOS (GitHub Actions) |
|---|---|---|---|
| Операційна система | Ubuntu 24.04.5 LTS | Microsoft Windows 10.0.26100 | macOS 26.7.2 |
| Архітектура процесора | X64 | X64 | Arm64 |
| RID | ubuntu.24.04-x64 | win-x64 | osx-arm64 |
| Версія .NET | .NET 10.0.14 | .NET 10.0.15 | .NET 10.0.15 |
| Роздільник шляхів | `/` | `\` | `/` |
| Каталог даних | `/root/.config/TrainingDiary` | `C:\Users\runneradmin\AppData\Roaming\TrainingDiary` | `/Users/runner/Library/Application Support/TrainingDiary` |

Каталог даних — те, що повертає `Environment.GetFolderPath(ApplicationData)`, і він різний: у Windows це `AppData\Roaming`, у Linux `~/.config`, у macOS `~/Library/Application Support`. Саме на macOS я помилився: у перевірці збирання я вказав шлях `~/.config/TrainingDiary/diary.json`, і другий застосунок його не знайшов (збирання на macOS стало червоним). Після цього я прибрав шлях із перевірки: `tools/DiaryReport` тепер бере `DiaryLocation.DiaryFile` з бібліотеки.

### Розміри збірок

Беруться з кроку «Розміри каталогів» у задачі `publish` в Actions (`du -sh publish/*`).

| Збірка | Розмір |
|---|---|
| win-x64, самодостатня | 79M |
| osx-arm64, самодостатня | 85M |
| linux-x64, самодостатня | 81M |
| linux-x64, залежна | 1,3M |

Різниця між самодостатньою й залежною збіркою — це середовище виконання .NET, яке лежить поруч із застосунком: близько 80 МБ проти 1,3 МБ. Залежна доречна, коли .NET уже є на цільовій машині (економія місця, оновлення середовища окремо), самодостатня — коли встановлювати .NET не можна чи не хочеться.

### Запуск на іншій системі

Самодостатні збірки під `win-x64`, `osx-arm64` і `linux-x64` створює задача `publish` у GitHub Actions одним скриптом `publish.sh` з одного вихідного тексту; їх можна завантажити з розділу Artifacts запуску «Build». Ключ `--info` я перевірив у задачах `build` на Windows, macOS і Linux (дані в таблиці вище): застосунок запускається й показує систему та каталог даних на всіх трьох.

Опубліковані файли з Artifacts на окремих машинах я не запускав, тому перешкод запуску з власного досвіду не маю. Типові перешкоди (право на виконання `chmod +x` у Linux і macOS, карантин macOS, бібліотека ICU у мінімальних образах Linux) описані в `README.md` за документацією.
