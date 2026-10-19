# Лабораторная: LINQ (сцена Bestiary)

## Кратко
Нужно было по кнопкам делать выборки существ сцены `Bestiary` через LINQ: подсвечивать красным нужных врагов на 3 секунды, а в заданиях 4–5 сортировать врагов и выводить список в консоль. Сделаны `PaintAll` с занятия и 5 заданий, у каждого своя кнопка. Всё проверено в Play Mode.

## Как запустить и проверить
Открыть `Assets/1.LINQ/Scenes/Bestiary.unity` → Play. Кнопки — столбиком в правом верхнем углу.

| Кнопка | Что должно произойти |
|---|---|
| Покрасить всех | все 12 существ (вместе с гномами-союзниками) становятся красными и остаются такими |
| Летающие, HP > 50 | на 3 с краснеют `Icon38`, `flowers`, `mushrooms2`, потом белые |
| Летающие грибы | на 3 с краснеет `mushrooms2` |
| Первый враг | на 3 с краснеет `mushrooms3` |
| По здоровью (лог) | в консоли список врагов от 200 до 5 |
| По типу и HP (лог) | в консоли сначала Ground, потом Air, внутри по возрастанию HP |

Перед каждым выделением цвет у всех сбрасывается, поэтому после «Покрасить всех» любая другая кнопка оставит красными только своих.

Существа сцены (по ним проверялись выборки):

| Имя | Класс | Type | HP |
|---|---|---|---|
| mushrooms1 / mushrooms3 | Mushrooms | Ground | 100 |
| mushrooms4 | Mushrooms | Ground | 30 |
| mushrooms2 | Mushrooms | Air | 100 |
| flowers (1) / flowers (2) | Flowers | Ground | 100 |
| flowers | Flowers | Air | 100 |
| Icon39 | Sects | Ground | 200 |
| Icon38 | Sects | Air | 100 |
| Icon42 | Sects | Air | 5 |
| Icon44 / Icon44 (1) | Friends (союзник) | Ground | 50 / 100 |

Порядок массива `_entities` (так его вернул `FindObjectsOfType`): `Icon44, Icon44 (1), mushrooms3, Icon38, flowers (2), mushrooms4, flowers, flowers (1), mushrooms1, Icon39, Icon42, mushrooms2`.

## Связь с занятием
Взято с занятия (таймкоды — по конспекту `_notes/lessons/linq.md`):
- `PaintAll` — дословно как на занятии: `Select → ToList → ForEach(PaintRed)` (27:45–30:23).
- Отбор только врагов — `Where(e => e.GetComponent<Enemy>())`, как в `ShootAtAllEnemies` (05:39–08:56).
- `ForEach` со ссылкой на метод, а не с лямбдой: `ForEach(PaintRed)`, `ForEach(Highlight)`, `ForEach(LogHealth)` (09:01–10:42).
- `FirstOrDefault()` + `if (e)` вместо `First()` (21:27–23:41).
- `OrderBy` для сортировки (16:27–19:30).
- Кнопки: `Button - TextMeshPro` в `Canvas`, `On Click () → Runtime Only → EnemySelector → метод` (30:25–32:30).

Добавлено сверх занятия:
- **Корутина `Flash` с `WaitForSeconds(3f)`**: по заданию выделение длится 3 секунды. Сделано по образцу готового `Entity.Flash()` преподавателя.
- **`ResetAll()` со `StopAllCoroutines()`**: по заданию перед новым выделением цвет сбрасывается у всех. Остановка корутин нужна, чтобы таймер прошлого нажатия не вернул белый цвет раньше времени новому выделению.
- **`Reverse` и `ThenBy`** — их требует задание, на занятии их не было.
- **`Debug.Log`** — вывод в консоль требует задание (на занятии его не было).
- Кнопки стали меньше, чем на занятии (масштаб 1.3 вместо ~2.16, ширина 200, шрифт 20). Иначе шесть кнопок не помещаются в углу и налезают на `Icon38`.

## Шаг 0. PaintAll с занятия
**Что сделали:** заполнили пустой `PaintAll()`. Существующую кнопку подписали «Покрасить всех» (она уже была привязана к `PaintAll`).
**Где:** `Assets/1.LINQ/Scripts/EnemySelector.cs`, `PaintAll()`; объект `Canvas/Button`.
**Как это работает:**
```csharp
_entities
    .Select(e => e.GetComponent<SpriteRenderer>())   // Entity → SpriteRenderer
    .ToList()                                        // ForEach есть только у List
    .ForEach(PaintRed);                              // ссылка на метод, без скобок
```
**Ключевые понятия:**
- *Select* — превращает каждый элемент в другой тип, количество элементов не меняется.
- *ToList()* — LINQ возвращает `IEnumerable` (ленивую последовательность), а `ForEach` — метод `List`, поэтому сначала нужен список.
- *Ссылка на метод* — `ForEach(PaintRed)` передаёт сам метод, а `ForEach` вызывает его для каждого элемента.

**Коммит:** `282ef95` LINQ: PaintAll с занятия

## Задание 1. Выделить всех летающих, у кого HP больше 50
**Что сделали:** метод `PaintAirStrong()` и кнопка «Летающие, HP > 50».
**Где:** `EnemySelector.PaintAirStrong()`, `ResetAll()`, `Highlight()`, `Flash()`; объект `Canvas/ButtonAirStrong`.
**Как это работает:**
```csharp
ResetAll();                                       // всех в белый, старые таймеры стоп
_entities
    .Where(e => e.GetComponent<Enemy>())          // только враги (без Friends)
    .Where(e => e.Type == Entity.EntityType.Air)  // летающие
    .Where(e => e.health > 50)                    // HP > 50
    .Select(e => e.GetComponent<SpriteRenderer>())
    .ToList()
    .ForEach(Highlight);                          // красный на 3 секунды
```
```csharp
private IEnumerator Flash(SpriteRenderer spriteRenderer)
{
    PaintRed(spriteRenderer);
    yield return new WaitForSeconds(3f);   // пауза, кадры идут дальше
    ResetColor(spriteRenderer);
}
```
Условия записаны отдельными `Where`, а не одним длинным, как советовал преподаватель («не нагромождать большим условием»). `Icon42` летает, но у него HP 5, поэтому он не выделяется.
**Ключевые понятия:**
- *Where* — оставляет только элементы, для которых условие истинно.
- *`GetComponent<Enemy>()` в условии* — у Unity-объекта есть неявное приведение к `bool`: если компонента нет, это `false`. `Mushrooms`, `Flowers`, `Sects` наследуются от `Enemy`, поэтому находятся по нему.
- *Корутина* — метод с `IEnumerator`, который можно приостановить через `yield return` и не останавливать при этом игру. Запускается `StartCoroutine`.

**Что делалось вручную:** возврат белого через 3 с проверил студент в Unity (когда окно Unity не в фокусе, игровое время стояло).
**Коммит:** `4b0d580` LINQ: задание 1 — выделение летающих с HP больше 50

## Задание 2. Выделить всех летающих грибочков
**Что сделали:** метод `PaintAirMushrooms()` и кнопка «Летающие грибы».
**Где:** `EnemySelector.PaintAirMushrooms()`; объект `Canvas/ButtonAirMushrooms`.
**Как это работает:**
```csharp
_entities
    .Where(e => e.GetComponent<Mushrooms>())      // только грибы (они уже враги)
    .Where(e => e.Type == Entity.EntityType.Air)  // летающие
    .Select(e => e.GetComponent<SpriteRenderer>())
    .ToList()
    .ForEach(Highlight);
```
Летающий гриб в сцене один — `mushrooms2`.
**Ключевые понятия:**
- *Отбор по конкретному классу* — `GetComponent<Mushrooms>()` находит только этот класс-наследник, отдельная проверка на `Enemy` не нужна.

**Коммит:** `260f9a3` LINQ: задание 2 — выделение летающих грибов

## Задание 3. Выделить первый элемент в массиве (First)
**Что сделали:** метод `PaintFirst()` и кнопка «Первый враг».
**Где:** `EnemySelector.PaintFirst()`; объект `Canvas/ButtonFirst`.
**Как это работает:**
```csharp
var e = _entities
    .Where(e => e.GetComponent<Enemy>())
    .FirstOrDefault();          // первый подходящий или null
if (e)                          // если врагов нет — ничего не делаем
    Highlight(e.GetComponent<SpriteRenderer>());
```
Первые два элемента массива — союзники `Icon44`, их пропускает `Where`, поэтому выделяется `mushrooms3`.
**Ключевые понятия:**
- *First()* — возвращает первый элемент. Если последовательность пуста, выбрасывает исключение `InvalidOperationException`.
- *FirstOrDefault()* — то же, но для пустой последовательности возвращает `null` (значение по умолчанию). Вместе с `if (e)` это безопасно, так и сделано на занятии.

**Коммит:** `b07b628` LINQ: задание 3 — выделение первого врага в массиве

## Задание 4. Отсортировать по здоровью от большего к меньшему (OrderBy + Reverse), вывести имя и здоровье
**Что сделали:** метод `SortByHealth()` с выводом через `LogHealth`, кнопка «По здоровью (лог)».
**Где:** `EnemySelector.SortByHealth()`, `LogHealth()`; объект `Canvas/ButtonSortHealth`.
**Как это работает:**
```csharp
_entities
    .Where(e => e.GetComponent<Enemy>())
    .OrderBy(e => e.health)     // по возрастанию: 5, 30, 100…, 200
    .Reverse()                  // разворот: 200 … 5
    .ToList()
    .ForEach(LogHealth);        // Debug.Log(имя + ": " + здоровье)
```
Вывод в Play Mode:
```
Icon39: 200
mushrooms2: 100
mushrooms1: 100
flowers (1): 100
flowers: 100
flowers (2): 100
Icon38: 100
mushrooms3: 100
mushrooms4: 30
Icon42: 5
```
**Ключевые понятия:**
- *OrderBy* — сортирует по ключу по возрастанию. Сортировка *устойчивая*: элементы с одинаковым ключом остаются в исходном порядке массива.
- *Reverse* — разворачивает последовательность. Поэтому враги с HP 100 идут в порядке, обратном массиву (`mushrooms2` … `mushrooms3`).

**Коммит:** `1190fc1` LINQ: задание 4 — сортировка врагов по здоровью с выводом в консоль

## Задание 5. Отсортировать по типу (Ground, Air), потом по здоровью (ThenBy), вывести имя, тип и здоровье
**Что сделали:** метод `SortByTypeAndHealth()` с выводом через `LogTypeHealth`, кнопка «По типу и HP (лог)».
**Где:** `EnemySelector.SortByTypeAndHealth()`, `LogTypeHealth()`; объект `Canvas/ButtonSortTypeHealth`.
**Как это работает:**
```csharp
_entities
    .Where(e => e.GetComponent<Enemy>())
    .OrderBy(e => e.Type)       // enum: Ground = 0, Air = 1 → сначала Ground
    .ThenBy(e => e.health)      // внутри одного типа — по возрастанию HP
    .ToList()
    .ForEach(LogTypeHealth);
```
Вывод в Play Mode:
```
mushrooms4 (Ground): 30
mushrooms3 (Ground): 100
flowers (2) (Ground): 100
flowers (1) (Ground): 100
mushrooms1 (Ground): 100
Icon39 (Ground): 200
Icon42 (Air): 5
Icon38 (Air): 100
flowers (Air): 100
mushrooms2 (Air): 100
```
**Ключевые понятия:**
- *ThenBy* — дополнительная сортировка: применяется только к элементам с одинаковым ключом предыдущего `OrderBy`. Если вместо него написать второй `OrderBy`, первая сортировка потеряется.
- *Сортировка по enum* — enum сравнивается по числовому значению, порядок задаётся объявлением `Ground, Air` в `Entity`.

**Коммит:** `5a310be` LINQ: задание 5 — сортировка врагов по типу и здоровью с выводом в консоль (в нём же кнопки сужены, чтобы все шесть помещались)

## Вопросы на защите
1. **Почему перед `ForEach` нужен `ToList()`?** LINQ возвращает `IEnumerable`, а `ForEach` — метод класса `List<T>`.
2. **Чем `Select` отличается от `Where`?** `Select` меняет тип элементов и оставляет их все, `Where` фильтрует, тип не меняет.
3. **Почему `ForEach(PaintRed)` без скобок?** Мы не вызываем метод, а передаём ссылку на него. Вызывать его будет `ForEach`.
4. **Почему гномы не выделяются?** На них компонент `Friends`, а не наследник `Enemy`, поэтому `GetComponent<Enemy>()` возвращает `null` → `false`.
5. **`First()` или `FirstOrDefault()`?** `First()` падает с исключением на пустой выборке, `FirstOrDefault()` возвращает `null`, а `if (e)` это проверяет.
6. **Почему выделен `mushrooms3`, а не первый объект массива?** Первые два элемента — союзники, `Where` отбрасывает их раньше, чем `FirstOrDefault`.
7. **Как работает задержка в 3 секунды?** Корутина `Flash`: красит, `yield return new WaitForSeconds(3f)`, потом возвращает белый цвет. Игра в это время не останавливается.
8. **Зачем `StopAllCoroutines()` в `ResetAll()`?** Чтобы корутина от прошлого нажатия не вернула белый цвет раньше времени новому выделению.
9. **Чем `ThenBy` отличается от второго `OrderBy`?** `ThenBy` сортирует только внутри групп с одинаковым первым ключом, а второй `OrderBy` пересортировал бы всё заново.
10. **Почему одинаковые HP в задании 4 идут в «обратном» порядке?** `OrderBy` устойчивый, а `Reverse` разворачивает весь список, в том числе равные элементы.

## Что можно было бы сделать иначе
- В задании 4 вместо `OrderBy(...).Reverse()` — `OrderByDescending(e => e.health)`: результат тот же, но равные HP остались бы в порядке массива. Задание просило именно `Reverse`.
- Отбор врагов можно писать через `OfType<Enemy>()`, но на занятии использовался `Where(e => e.GetComponent<Enemy>())`.
- Три `Where` в задании 1 можно объединить в одно условие через `&&`. Отдельные строки читаются легче.
