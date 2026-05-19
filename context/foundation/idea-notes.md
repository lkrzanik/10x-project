## 10xFotowoltaika - MVP

### Główny problem
Długoterminowa analiza temperatury i wigotności pomieszczenia oraz warunków pogodowych w celu zaproponowania optymalnego zestawu paneli fotowoltaicznych, inwertera i magazynu energii.

### Najmniejszy zestaw funkcjonalności
- Aplikacja webowa z funcją logowania użytkownika
- Wprowadzanie danych polega na imporcie pliku CSV wygenerowanego przez urządzenie pomiarowe zlokalizowane w pomieszczeniu; dane to odczyt temperatury i wilgotności w stałych interwałach czasowych (co 30 minut); dane mają z góry zdefiniowany format
- Aplikacja pozwala zaimportować dane pogodowe dla danej lokalizacji z pliku CSV pobranego ręcznie z zewnętrznego serwisu pogodowego; dane pogodowe powinny zawierać informację o temperaturze, wilgotności i zachmurzeniu całkowitym; dane mają z góry zdefiniowany format
- Aplikacja automatycznie koreluje zaimportowane dane z urządzenia pomiarowego z zewnętrznymi danymi pogodowymi; dane pogodowe mogą być zapisane w innych interwałach czasowych niż dane pomiarowe z urządzenia, ale dotyczą tych samych dni,
- Do każego z rekordów danych pomiarowych użykownik może dodać własne uwagi w formie tekstowej, które mogą zawierać np. opis anomalii
- Na podstawie zebranych danych użykownik może w dowolnym momencie wyświetlić wykres zależności między danymi z urządzenia pomiarowego i danymi pogodowymi
- Aplikacja wyróżnia ekstremalne odczyty z urządzenia pomiarowego i wyświetla je w widocznym miejscu; kryteria ekstremów określa się jako wartości min/max w ustawieniach aplikacji
- Aplikacja wygeneruje raport przy pomocy AI, który będzie zawierał odpowiedź - pomieszczenie nadaje się/nie nadaje się, co dyskwalifikuje pomieszczenie, zaproponuje optymalną technologię zestawu inwertera z magazynem energii; raport będzie w formacie PDF

### Co NIE wchodzi w zakres MVP
- Zaawansowana edycja daych
- Obsługa wielu lokalizacji, w których dokonywane są pomiary
- Obsługa wielu użytkowników
- Analiza zapotrzebowania na energię i ilość potrzebnych paneli fotowoltaicznych

### Kryteria sukcesu
- wszytkie dane z urządzenia pomiarowego są skorelowane z danymi pogodowymi,
- zgromadzone dane pozwalają na wygenerowanie raportu zawierającego propzycję optymalnej technologii zestawu inwertera i magazynu energii
