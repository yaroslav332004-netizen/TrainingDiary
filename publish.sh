#!/usr/bin/env bash
set -euo pipefail

cd "$(dirname "$0")"

for rid in win-x64 osx-arm64 linux-x64; do
  echo "== $rid"
  for project in TrainingDiary.App TrainingDiary.Desktop; do
    # Кожна оболонка — у власний каталог: дві самодостатні збірки
    # в одному каталозі перезаписали б файли середовища виконання.
    dotnet publish "$project" -c Release -r "$rid" \
      --self-contained true -o "publish/$rid/$project"
  done
done

# Каталоги змінилися — право на виконання ставимо вже в нових.
chmod +x publish/*/TrainingDiary.App/diary \
         publish/*/TrainingDiary.Desktop/diary-ui 2>/dev/null || true
