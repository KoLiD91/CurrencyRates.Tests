# language: pl
Właściwość: Pobieranie listy dostępnych walut
    Jako użytkownik systemu
    Chcę mieć dostęp do listy wszystkich dostępnych walut
    Aby móc sprawdzić, dla których walut mogę pobierać kursy

Scenariusz: Pobieranie listy wszystkich dostępnych walut
    Gdy pobieram listę dostępnych walut
    Wtedy odpowiedź z listą walut jest udana
    I lista zawiera podstawowe waluty
    I każda waluta ma wymagane pola

Scenariusz: Sprawdzanie szczegółów konkretnych walut
    Gdy pobieram listę dostępnych walut
    Wtedy odpowiedź z listą walut jest udana
    I lista zawiera walutę "USD" o nazwie "dolar amerykański"
    I lista zawiera walutę "EUR" o nazwie "euro"
    I wszystkie waluty mają typ tabeli "A"