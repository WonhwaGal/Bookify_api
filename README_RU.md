<h1>Bookify_api</h1>
<h5>Февраль 2024</h5>

Веб-сервис, предоставляющий пользователям возможность зарегистрироваться/авторизоваться, а также добавлять апартаменты или осуществлять поиск доступных апартаментов по нескольким параметрам, создавать бронирования и оставлять отзывы.

<img align="center" width="100%" src="/readme_resources/boo1.png">

<h3>Содержит:</h3>

* База данных **MSSql** для пользователей, апартаментов, бронирований и отзывов (**с проверкой параллелизма)**;
* Авторизация через **Microsoft Identity** и отдельная база данных под Identity;
* Контейнерная разработка;
* Веб-API контроллеры;
* Имплементированы сервисы логирования (**Serilog**), кеширования (**Redis**), проверки работоспособности **HealthCheck** и джобов **Quartz**;
* Игтеграция (**Service Integration**) тестовых sms и mail сервисов;
* **Паттерны:**  Репозиторий, Принцип быстрого отказа;
* Кастомные фильтры и sql-запросы различной сложности;

<br><h4>Интерфейс Swagger настроен для лучшего восприятия:</h4>
<img align="center" width="100%" src="/readme_resources/boo2.png">

<br><h4>HealthCheck сервис:</h4>
<img align="center" width="100%" src="/readme_resources/boo3_health.png">
<img align="center" width="100%" src="/readme_resources/boo3_health1.png">

<br><h4>Пример sql-запроса:</h4>
<img align="center" width="80%" src="/readme_resources/bookify_sql.png">
