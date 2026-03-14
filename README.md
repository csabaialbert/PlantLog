#PlantLog - Növénygondozási Napló
A PlantLog egy modern, webalapú alkalmazás, amely segít a növénykedvelőknek digitálisan nyilvántartani növényeiket, követni azok fejlődését és rendszerezni a gondozási feladatokat.

##Főbb funkciók
###Személyre szabott gyűjtemény: Felhasználónként elkülönített növénylista.

###Gondozási napló: Öntözések, átültetések és egyéb események visszamenőleges követése.

###Fajtaadatbázis: Több mint 150 előre feltöltött növényfaj gondozási útmutatóval.

###Képgaléria: Növényekhez és naplóbejegyzésekhez csatolható fotók (GridFS tárolással).

###Interaktív Dashboard: Gyors áttekintés a legutóbbi tevékenységekről.

##Technológiai stack
###A projekt különlegessége a hibrid adatkezelési architektúra:

####Frontend/Backend: ASP.NET Core Blazor (Interactive Server mode).

####Felhasználókezelés: MSSQL Server + ASP.NET Core Identity.

####Domain adatok: MongoDB (Növények, Naplóbejegyzések, Fajok).

####Médiatárolás: MongoDB GridFS (Nagy méretű bináris fájlok darabolt tárolása).

##Adatmodell
###Az alkalmazás polyglot persistence megközelítést alkalmaz:

###MSSQL: A strukturált, biztonságkritikus adatokhoz (autentikáció).

###MongoDB: A rugalmas, hierarchikus adatokhoz. A naplóbejegyzések a teljesítmény érdekében beágyazott objektumként (Embedded Documents) tárolódnak a növények dokumentumain belül.

##Telepítés és futtatás
###Adatbázisok előkészítése
Indíts el egy MongoDB példányt (alapértelmezett port: 27017).

###Győződj meg róla, hogy az MSSQL Server elérhető.

###Konfiguráció
Frissítsd az appsettings.json fájlban a Connection Stringeket.

###Adatok inicializálása
Az első futtatáskor a rendszer automatikusan létrehozza az SQL sémát (Migrations) és feltölti a MongoDB Species kollekcióját a mellékelt JSON fájlból.
