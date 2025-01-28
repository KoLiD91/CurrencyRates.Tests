# language: pl
Właściwość: Pobieranie rocznych kursów walut
    Jako użytkownik systemu
    Chcę mieć dostęp do historycznych kursów walut dla wybranego roku
    Aby móc analizować zmiany wartości walut w okresie rocznym

Scenariusz: Pobieranie kursów waluty dla konkretnego roku
    Mając wskazaną walutę "USD"
    I wskazany rok "2022"
    Gdy pobieram kursy roczne
    Wtedy odpowiedź z kursami rocznymi jest udana
    I lista kursów rocznych jest prawidłowa
    I wszystkie kursy roczne są większe od 0

Scenariusz: Próba pobrania kursów dla nieprawidłowego roku
    Mając wskazaną walutę "USD"
    I wskazany rok "2099"
    Gdy pobieram kursy roczne
    Wtedy system zwraca błąd nieprawidłowego roku