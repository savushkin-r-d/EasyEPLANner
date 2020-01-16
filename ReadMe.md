# EasyEplanner - Open Source

### Репозиторий
Данный репозиторий (хранилище) представляет собой проект с открытым исходным кодом - EasyEplanner. Мы не только работаем над проектом, но и решаем различные задачи связанные с жизнью и развитием проекта.

### EasyEplanner

<img src="docs/user_manual/images/EasyEplannerPreview.png">

Надстройка EasyEPlanner разработана как Add-In к EPLAN, на данный момент используется версия EPLAN 2.8. Надстройка используется при разработке проектов в EPLAN и позволяет автоматизировать работу инженера по автоматизации, а так же инженера-программиста, который описывает проект на языке программирования LUA. С помощью EasyEPlanner описываются технологические объекты (Танк, Бойлер и др.), операции этих объектов, шаги операций, устанавливаются ограничения для операций, а так же множество других свойств технологического объекта. В конечном итоге, EasyEPlanner генерирует LUA файлы, которые загружаются в контроллер. В данный момент поддерживаются контроллеры следующих производителей:

1. [Phoenix Contact - PLCNext](https://github.com/plcnext);
2. [WAGO - PFC200](https://github.com/WAGO).

### Пользовательская документация
По [этой](docs/user_manual/readme.md) ссылке вы можете найти последнюю версию пользовательской документации к проекту.

###  Contributing
If you want to contribute to the development of our project then  check out [how to do it better](docs/contributing.md) before you start.


### Feedback
If you wont to contact with us you can use some ways:
* [Google group](https://groups.google.com/forum/#!forum/easyeplanner);
* Channel in [Slack](https://slack.com) - easyeplanner.slack.com.


### Code of conduct
We [using](docs/CODE_OF_CONDUCT.md)
standart behavior rules (communication), provided by the GitHub service.


### Code style
We use C# and LUA programming languages for development. We have [own set of agreements](docs/codestyle.md), which you must follow to make the code convenient and readable.


### License
The project is licensed under [MIT](LICENSE.txt) license.
