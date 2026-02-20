# Benutzerhandbuch – Hytale Translation Helper

## 1) Zweck der Software
Der **Hytale Translation Helper** hilft dir beim Bearbeiten von `.lang`-Dateien in einem Sprachordner-Setup wie:

- `Languages/de-DE/items.lang`
- `Languages/en-US/items.lang`
- `Languages/fr-FR/items.lang`

Die App zeigt dir pro Key alle Sprachen nebeneinander, markiert fehlende Übersetzungen und unterstützt Import/Export über JSON.

---

## 2) Schnellstart

1. App starten.
2. **Datei → Languages-Ordner wählen...**
3. Falls mehrere Dateien vorhanden sind: **Datei → Sprachdatei wählen...** (z. B. `items.lang`).
4. Keys im Baum links auswählen und rechts pro Sprache bearbeiten.
5. Mit **Datei → Speichern** speichern.

---

## 3) Oberfläche im Überblick

### Links: Key-Baum
- Hier siehst du die hierarchischen Keys (durch `.` getrennt).
- Rechtsklick auf einen Knoten bietet:
  - **Pfad bis hierhin kopieren**
  - **Neuen Eintrag hier hinzufügen**

### Oben: Such-/Filterleiste
- **Suche/Filter**: filtert Keys nach Text.
- **Nur fehlende**: zeigt nur Keys mit fehlenden Übersetzungen.
- **Filter**: Filter anwenden.
- **Nächstes fehlendes**: springt zum nächsten unvollständigen Key.
- **Auto-Übersetzung**: versucht fehlende Felder für den aktuellen Key automatisch zu füllen.

### Rechts: Sprach-Editoren
- Für jede Sprache ein Textfeld.
- Bis 5 Sprachen werden gleichmäßig verteilt angezeigt.
- Ab mehr als 5 Sprachen wird der Bereich scrollbar.

### Unten: Statuszeile
- Zeigt u. a. Anzahl Sprachen, Keys und fehlende Übersetzungen.

---

## 4) Menüfunktionen

## Datei
- **Languages-Ordner wählen...**
- **Sprachdatei wählen...**
- **Neue Sprache anlegen...**
- **Liste aktualisieren**
- **Speichern**
- **JSON importieren...**
- **JSON exportieren...**
- **Beenden**

## Bearbeiten
- **Key hinzufügen**
- **Key löschen**
- **Nächstes fehlendes Feld**
- **Pfad bis hierhin kopieren**

## Werkzeuge
- **Auto-Übersetzen (aktueller Key)**

## Ansicht
- **Baum-Breite umschalten**

## Sprache
- **Deutsch / English** (UI-Sprache)

---

## 5) Neue Sprache anlegen (wichtig)
Mit **Datei → Neue Sprache anlegen...** kannst du im aktuell gewählten Languages-Ordner eine neue Sprache erzeugen.

### Eingaben im Dialog
- **Neue Sprache (Ordnername)**, z. B. `it-IT`
- **Default/Fallback-Sprache (optional)**
  - z. B. `en-US`
  - oder **(Keine - nur Keys übernehmen)**

### Verhalten
- Die App erstellt den neuen Sprachordner.
- Sie erstellt darin **alle vorhandenen `.lang`-Dateien**, die in den anderen Sprachen existieren.
- Wenn eine Fallback-Sprache gewählt ist:
  - Werte werden aus der Fallback-Datei übernommen.
- Wenn **Keine** gewählt ist:
  - nur vorhandene Keys werden angelegt,
  - Werte bleiben leer (`key = `), damit du manuell übersetzen kannst.

---

## 6) JSON Import/Export

### Export
**Datei → JSON exportieren...**
- Exportiert die aktuell geladene Sprachdatei über alle Sprachen in eine JSON-Datei.

### Import
**Datei → JSON importieren...**
- Zeigt vorab eine Validierung/Vorschau.
- Danach kannst du wählen:
  - **Ersetzen** (bestehende Daten komplett ersetzen)
  - **Zusammenführen** (merge)

---

## 7) Tastenkürzel
- **F3**: nächstes fehlendes Feld
- **Ctrl + F**: Suchfeld fokussieren
- **Entf** (im Baum): ausgewählten Key löschen
- **Enter** (im Suchfeld): Filter anwenden

---

## 8) Speicherung & Konfiguration
Die App speichert den letzten Zustand lokal neben der EXE:

- `config/lfe.config`

Darin werden u. a. gespeichert:
- letzter Languages-Ordner
- zuletzt gewählte `.lang`-Datei
- UI-Sprache (DE/EN)

---

## 9) Hinweise zur Auto-Übersetzung
Die Auto-Übersetzung nutzt externe Dienste (z. B. LibreTranslate, optional Fallback).

- Sende keine vertraulichen Inhalte an öffentliche Endpunkte.
- Für volle Kontrolle besser eigene/self-hosted Instanz verwenden.

---

## 10) Typische Probleme

### „Keine .lang-Datei gefunden/ausgewählt“
- Prüfe, ob im Sprachordner tatsächlich `.lang`-Dateien liegen.
- Wähle die Datei manuell über **Datei → Sprachdatei wählen...**.

### „Languages-Ordner nicht gefunden“
- Pfad prüfen und erneut wählen.

### Sprache anlegen schlägt fehl
- Prüfe ungültige Zeichen im neuen Ordnernamen.
- Prüfe Schreibrechte im Zielordner.

---

## 11) Lizenz / Nutzung
Die Anwendung ist für **private, nicht-kommerzielle Nutzung** vorgesehen.
