#language: pl
Właściwość: Operacje na bazie danych dla kursów walut
    Jako użytkownik systemu
    Chcę prawidłowo zapisywać i odczytywać kursy walut z bazy danych
    Aby zapewnić spójność i dostępność danych historycznych

Scenariusz: Zapisywanie nowego kursu waluty w bazie danych
    Mając nowy kurs waluty "NOK"
    I wartość kursu 4.0124
    I datę kursu "2024-01-24"
    Gdy zapisuję kurs w bazie danych
    Wtedy kurs zostaje poprawnie zapisany
    I mogę odczytać ten sam kurs z bazy

Scenariusz: Próba zapisania istniejącego kursu waluty
    Mając istniejący kurs waluty "EUR" z dnia "2025-01-23"
    Gdy próbuję zapisać ten sam kurs ponownie
    Wtedy operacja kończy się niepowodzeniem