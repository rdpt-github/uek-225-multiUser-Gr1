
# Dokumentation
#### Sven Leuenberger | Jonas Lüthi | Lars Büttler


### 1. Aufbau

Der Aufbau und somit die Architektur der Applikation entspricht am nächsten der Onion-Struktur. Die Schichten Applikation / Services, Entities und der Datenbank Zugriff sind in verschiedene Subprojekte aufgeteilt.

#### 1.1. Datenzugriffsschicht

Diese Schciht enthält die Repositories, welche für den direkten Zugriff auf die Datenbank und die Durchführung der Transaktionen verantwortlich ist.

#### 1.2. Models

In der Modelle-Schicht befinden sich die verschiedenen Entities und deren Definitionen.

#### 1.3. Web

Im Web-Projekt befindet sich die eigentliche Applikationsschicht sowie auch die Restschnittstellen. Die verschiedenen Services verarbeiten die Logik und die Controller sind für die Kommunikation zwischen dem Frontend und Backend verantwortlich.

#### 1.4. Testing

Im Testing-Projekt wird die Logik sowie auch die Integration mithilfe von Unit-Tests getestet.

Ebenso wird für den BookingsController ein Lasttest eingesetzt. Dafür haben wir wie von den Unterlagen empfohlen NBomber verwendet. Beim Lasttests wird so viele Requests für verschiedene zufällig erstellte Transaktionen abgesendet wie möglich. Es werden aktuell ein bisschen weniger als 80 Requests pro Sekunde erreicht und die Requests laufen alle Erfolgreich durch.

### 2. Starten

Die Applikation wird wie folgt gestartet:

- Im Terminal in den Docker-Compose Ordner navigieren und den docker-compose up Befehl eingeben. (Achtung: Docker muss installiert und gestartet sein.)

- L-Bank.Web starten.

- Im Terminal in den frontend-Ordner navigieren und das Frontend mit ng serve starten.

### 3. Probleme

Ein grosses Problem bei Multiuser-Applikationen ist, dass gleichzeitige Zugriffe auf die Datenbank von mehreren Personen zu Fehlern führen kann. Dies zeigt sich in etwa so, dass Beispielsweise Geld abgebucht werden kann, obwohl das Gelb parallel bereits abgebucht wurde und so der Kontostand in den Negtivbereich rutscht.

#### 3.1 Lösung

Die Lösung dieses Problems ist, die verschiedenen Datenbankzugriffe jeweils in einer Transaktion zu machen. Für unser Beispiel eignet sich dabei das Isolationlevel Serializable. Dies bewirkt, dass die zuerst gestartete Transaktion abgeschlossen wird und anschliessend erst die zweite Transaktion gestartet wird. Dies bewirkt, dass es zu keiner Überschreibung bei den Verbuchungen und somit bei den Datenbankzugriffen kommen kann. Um zu garantieren, dass die Transaktion auch wirklich durchgeführt wird, wurde die Transaktion in einer Do-While-Schlaufe platziert. Dies löst das Problem, dass die Transaktion nochmals versucht wird, führte jedoch bei den Tests zu Problemen. Dies lösten wir so, dass wir einen Boolean implementierten, welcher false ist, wenn keine passende Entity gefunden wurde und so die der Loop mithilfe dieses Bools unterbrochen werden kann. Bei unseren Zusatzfeatures wurde die Problematik gleich angegangen und gelöst.

