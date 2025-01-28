# language: pl
Właściwość: Pobieranie kwartalnych kursów walut
    Jako użytkownik systemu
    Chcę mieć dostęp do historycznych kursów walut dla wybranego kwartału
    Aby móc analizować zmiany wartości walut w okresie kwartalnym

Scenariusz: Pobieranie kursów waluty dla konkretnego kwartału
    Mając zdefiniowaną walutę "USD"
    I zdefiniowany rok "2024"
    I zdefiniowany kwartał "1"
    Gdy pobieram kursy kwartalne
    Wtedy odpowiedź z kursami kwartalnymi jest udana
    I lista kursów kwartalnych jest prawidłowa
    I wszystkie kursy kwartalne są większe od 0

Scenariusz: Próba pobrania kursów dla nieprawidłowego kwartału
    Mając zdefiniowaną walutę "USD"
    I zdefiniowany rok "2024"
    I zdefiniowany kwartał "5"
    Gdy pobieram kursy kwartalne
    Wtedy system zwraca błąd nieprawidłowego kwartału