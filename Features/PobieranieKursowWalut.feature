# language: pl
Właściwość: Pobieranie aktualnego kursu waluty
    Jako użytkownik systemu
    Chcę mieć możliwość sprawdzenia aktualnego kursu waluty
    Aby móc śledzić bieżące kursy walut

Scenariusz: Pobieranie aktualnego kursu dla prawidłowej waluty
    Mając kod waluty "USD"
    Gdy wysyłam zapytanie o aktualny kurs
    Wtedy odpowiedź powinna być udana
    I odpowiedź powinna zawierać prawidłowe dane kursu

Scenariusz: Pobieranie aktualnego kursu dla nieprawidłowej waluty
    Mając kod waluty "XXX"
    Gdy wysyłam zapytanie o aktualny kurs
    Wtedy system zwraca błąd serwera