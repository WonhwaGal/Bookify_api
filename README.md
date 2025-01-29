<h1>Bookify_api</h1>
<h5>February 2024</h5>

This service allows users to register/login, as well as add apartments or search for available apartments, create bookings and post reviews.

<img align="center" width="100%" src="/readme_resources/boo1.png">

<h3>Includes:</h3>

* **MSSql database** for users, apartments, bookings and reviews with **concurrency checks**;
* **Microsoft Identity** and a separate Identity database;
* Docker compose;
* Controller-based API;
* Implemented logging (**Serilog**), caching (**Redis**), **HealthCheck** and **Quartz**;
* **Service Integration** of test-mail and test-sms;
* **Patterns:**  Repository, Fail fast principle **design*;*
* Custom filters and complex sql requests;

<br><h4>Swagger interface adjusted for better perception:</h4>
<img align="center" width="100%" src="/readme_resources/boo2.png">

<br><h4>HealthCheck:</h4>
<img align="center" width="100%" src="/readme_resources/boo3_health.png">
<img align="center" width="100%" src="/readme_resources/boo3_health1.png">

<br><h4>Sql request example:</h4>
<img align="center" width="80%" src="/readme_resources/bookify_sql.png">
