## 10xFotowoltaika - MVP

### Główny problem
Długoterminowa analiza temperatury i wigotności pomieszczenia oraz warunków pogodowych w celu zaproponowania optymalnego zestawu paneli fotowoltaicznych, inwertera i magazynu energii.

### Najmniejszy zestaw funkcjonalności
- Wprowadzanie danych polega na imporcie pliku CSV wygenerowanego przez urządzenie pomiarowe zlokalizowane w pomieszczeniu; dane to odczyt temperatury i wilgotności w stałych interwałach czasowych (co 30 minut)
- Aplikacja pozwala zaimportować dane pogodowe dla danej lokalizacji (z API zewnętrznego serwisu pogodowego lub zaimportowane z pliku CSV pobranego ręcznie z zewnętrznego serwisu pogodowego); dane pogodowe powinny zawierać temperaturę, wilgotność i zachmurzenie całkowite
- Aplikacja automatycznie koreluje zaimportowane dane z urządzenia pomiarowego z zewnętrznymi danymi pogodowymi; dane pogodowe mogą być zapisane w innych interwałach czasowych niż dane pomiarowe z urządzenia,
- Do każego z rekordów danych pomiarowych użykownik może dodać własne uwagi w formie tekstowej
- Na podstawie zebranych danych użykownik może w dowolnym momencie wyświetlić wykres zależności między danymi z urządzenia pomiarowego i danymi pogodowymi
- Aplikacja wyróżnia ekstremalne odczyty z urządzenia pomiarowego i wyświetla je w widocznym miejscu
- Aplikacja wygeneruje raport przy pomocy AI, który będzie zawierał odpowiedź - pomieszczenie nadaje się/nie nadaje się, co dyskwalifikuje pomieszczenie, zaproponuje optymalną technologię zestawu inwertera z magazynem energii

### Co NIE wchodzi w zakres MVP
- Zdalny dostęp (aplikacja nie musi być stroną internetową),
- Zaawansowana edycja daych nie jest konieczna,
- Obsługa wielu lokalizacji, w których dokonywane są pomiary
- Obsługa wielu użytkowników,
- Brak analizy zapotrzebowania na energię i ilości potrzebnych paneli fotowoltaicznych

### Kryteria sukcesu
- wszytkie dane z urządzenia pomiarowego są skorelowane z danymi pogodowymi,
- zgromadzone dane pozwalają na propzycję optymalnej technologii zestawu inwertera i magazynu energii 
