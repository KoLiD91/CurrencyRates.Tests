# language: pl
Właściwość: Pobieranie miesięcznych kursów walut
    Jako użytkownik systemu
    Chcę mieć dostęp do historycznych kursów walut dla wybranego miesiąca
    Aby móc analizować zmiany wartości walut w okresie miesięcznym

Scenariusz: Pobieranie kursów waluty dla konkretnego miesiąca
    Mając określoną walutę "USD"
    I określony rok "2004"
    I określony miesiąc "11"
    Gdy pobieram kursy miesięczne
    Wtedy odpowiedź z kursami miesięcznymi jest poprawna
    I lista kursów miesięcznych zawiera prawidłowe dane
    I każdy kurs w liście jest większy od 0

Scenariusz: Próba pobrania kursów dla nieprawidłowego miesiąca
    Mając określoną walutę "USD"
    I określony rok "2004"
    I określony miesiąc "13"
    Gdy pobieram kursy miesięczne
    Wtedy system zwraca błąd niepoprawnego miesiąca