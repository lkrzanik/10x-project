## 10xFotowoltaika - MVP

### Główny problem
Długoterminowa analiza temperatury i wigotności pomieszczenia oraz warunków pogodowych w celu zaproponowania optymalnego zestawu paneli fotowoltaicznych, inwertera i magazynu energii.

### Najmniejszy zestaw funkcjonalności
- Wprowadzanie danych polega na imporcie pliku CSV wygenerowanego przez urządzenie pomiarowe zlokalizowane w pomieszczeniu; dane to odczyt temperatury i wilgotności w stałych interwałach czasowych (co 30 minut)
- Aplikacja automatycznie koreluje zaimportowane dane z danymi pogodowymi pobranymi z zewnętrznego serwisu; przy synchronizacji dane zapisywane są w aplikacji 
- Użykownik może do każego z rekordów pomiarowych dodać własne uwagi w formie tekstowej
- Na podstawie zebranych danych użykownik może w dowolnym momencie wyświetlić wykres zależności między wprowadzonymi danymi z urządzenia pomiarowego i danymi pobranymi z zewnętrznego serwisu pogodowego
- Aplikacja wyróżnia ekstremalne odczyty i wyświetla je w widocznym miejscu
- Aplikacja wygeneruje raport przy pomocy AI, który będzie zawierał odpowiedź - pomieszczenie nadaje się/nie nadaje się, co dyskwalifikuje pomieszczenie, zaproponuje optymalną technologię zestawu inwertera z magazynem energii

### Co NIE wchodzi w zakres MVP
- Zdalny dostęp (aplikacja nie musi być stroną internetową),
- Zaawansowana edycja daych,
- Obsługa wielu lokalizacji, w których dokonywane są pomiary,
- Obsługa wielu użytkowników,
- Brak analizy zapotrzebowania na energię i ilości potrzebnych paneli fotowoltaicznych

### Kryteria sukcesu
- wszytkie dane z urządzenia pomiarowego są skorelowane z danymi pogodowymi,
- zgromadzone dane pozwalają na propzycję optymalnego zestawu paneli fotowoltaicznych, inwertera i magazynu energii 
