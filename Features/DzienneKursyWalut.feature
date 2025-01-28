# language: pl
Właściwość: Pobieranie dziennych kursów walut
    Jako użytkownik systemu
    Chcę mieć dostęp do historycznych kursów walut dla konkretnego dnia
    Aby móc sprawdzać wartości walut w wybranym dniu

Scenariusz: Pobieranie kursu waluty dla konkretnego dnia
    Mając wybraną walutę "USD"
    I wybraną datę "2022-01-31"
    Gdy sprawdzam historyczny kurs dzienny
    Wtedy system zwraca poprawną odpowiedź
    I odpowiedź zawiera prawidłowe dane kursu historycznego
    I wartość kursu jest większa od 0

Scenariusz: Próba pobrania kursu dla nieprawidłowej daty
    Mając wybraną walutę "USD"
    I wybraną datę "2099-99-99"
    Gdy sprawdzam historyczny kurs dzienny
    Wtedy system zwraca błąd nieprawidłowego żądania