# EasyEPLANner - Open Source

[![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=savushkin-r-d_EasyEPLANner&metric=sqale_rating)](https://sonarcloud.io/summary/new_code?id=savushkin-r-d_EasyEPLANner)
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=savushkin-r-d_EasyEPLANner&metric=security_rating)](https://sonarcloud.io/summary/new_code?id=savushkin-r-d_EasyEPLANner)
[![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=savushkin-r-d_EasyEPLANner&metric=reliability_rating)](https://sonarcloud.io/summary/new_code?id=savushkin-r-d_EasyEPLANner)

[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=savushkin-r-d_EasyEPLANner&metric=bugs)](https://sonarcloud.io/summary/new_code?id=savushkin-r-d_EasyEPLANner)
[![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=savushkin-r-d_EasyEPLANner&metric=code_smells)](https://sonarcloud.io/summary/new_code?id=savushkin-r-d_EasyEPLANner)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=savushkin-r-d_EasyEPLANner&metric=coverage)](https://sonarcloud.io/summary/new_code?id=savushkin-r-d_EasyEPLANner)

>[!TIP]
>Русский вариант находится [здесь](../ru/readme.md). \
>Deutsche Readme ist [hier](../de/readme.md). \
>English version is [here](../../ReadMe.md)


### Рэпазіторый

Гэты рэпазіторый уяўляе сабой праект з адкрытым зыходным кодам - ​​EasyEPLANner. Мы не толькі працуем над праектам, але і вырашаем розныя задачы, звязаныя з жыццём і развіццём праекта.

### EPLAN

[EPLAN Electric P8](https://www.eplan-software.com/solutions/eplan-electric-p8/) — модульнае і маштабіруемае рашэнне для электратэхнічнага праектавання, аўтаматычнага стварэння праектнай і працоўнай дакументацыі.

[EPLAN API Help](https://www.eplan.help/en-us/Infoportal/Content/api/2023/index.html) — апісанне і прыклады карыстання API EPLAN.

Прыклады па аўтаматызацыі EPLAN Electric P8 на C# можна знайсці ў
[Suplanus](https://github.com/Suplanus).

### EasyEPLANner

<img src="../user_manual/images/EasyEplannerPreview.png">

Надналадка EasyEPLANner распрацавана як Add-In к EPLAN, на дадзены момант выкарыстоўваецца версія EPLAN 2.9. Надналадка выкарыстоўваецца пры распрацоке праектаў у EPLAN і дазваляе автоматизировать аўтаматызаваць працу інжынэра па аўтаматызацыі, і інжынэра-распрацоўніка, які апісвае праект на LUA.

З дапамогай EasyEPLANner апісваюцца тэхналагічныя аб'екты (Танк, Бойлер и інш.), аперацыі гэтых аб'ектаў, шаги апераций, усталююцца абмежаванні для аперацый, і мноства іншых уласцівасцяў тэхналагічнага аб'екта. У выніку EasyEPLANner генеруе LUA файлы, якія загружаюцца у кантролер. У даддзены момнат падтрымліваюцца кантролеры наступных вытворцаў:

1. [Phoenix Contact - PLCNext](https://github.com/plcnext);
2. [WAGO - PFC200](https://github.com/WAGO).

### Як сабраць
Вы можаце кланіраваць рэпазіторый гэтай камандай:

```bash
git clone --recurse-submodules https://github.com/savushkin-r-d/EasyEPLANner.git
```

але, калі вы скланіравалі без ініцыялізацыі і абнаўлення падмодуляў, спрабуйце наступнае:

```bash
git submodule update --init --recursive
```

Віншуем, цяпер вы можаце сабраць гэта рашэнне (_калі у вас ёсць дадатак EPLAN P8_).

### Карыстацкая дакументацыя

Па [гэтай](../user_manual/ReadMe.md) спасылцы вы можаце знайсці апошнюю версію карыстацкай дакументацыі да праекта.

### Садзейнічанне (Contributing)

Калі вы хаціце пасадзейнічаць у распрацоўцы нашага праекта, то перад гэтым азнаёмцесь з тым, [як лепш гэта зрабіць](../contributing.md).

### Зваротная сувязь (Feedback)

Калі вы хаціце звязацца з намі, можаце карыстацца Slack:

* Канал ў [Slack](https://slack.com) - EasyEPLANner.slack.com.

### Нормы паводзін (Code of conduct)

Мы [выкарыстоўваем](../CODE_OF_CONDUCT.md) стандартныя нормы поводзін, якія дае сервіс GitHub.

### Правила распрацощцы (Code style)

Для распрацоўцы выкарыстоўваеца C# і Lua. У нас ёсць [уласны набор пагадненняў](../codestyle.md), якога трэба прытлімлівацца, каб код быў зручным і чытаемым. 
### Ліцэнзія

Праект лінцэнзіраван пад [MIT](../../LICENSE.txt) ліцэнзіей.
