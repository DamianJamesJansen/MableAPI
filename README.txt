Het project is gemaakt in .Net 9.0. 
Als je dotnet geinstalleerd hebt kan je in de terminal van het project in de submap MableAPI 'dotnet run' uitvoeren

De tests zijn uit te voeren door in de map MableAPI.Tests het commando 'dotnet test' uit te voeren

Als je het project runt is de makkelijkste manier om de endpoints te checken via:
localhost:5013/swagger/index.html (dit is een UI)
Maar je kan ook via postman de endpoints testen.

Alle endpoints behalve de login endpoint zijn beveiligd met een token, die token krijg je door de login endpoint aan te roepen
de credentials zijn: 'username' en 'password'. Heel origineel, ik weet het :)

Alle andere endpoints hebben een geldig JWT token nodig. run ze zonder en je krijgt een error. Je kan in de swagger ui de token opgeven in de Authorize button, rechts boven in
of als je postman gebruikt of curl. de header toevoegen: Authorization: Bearer <paste token>

Op de swagger pagina kan je bij elke call "Try it out" klikken aan de rechter kant. Alle benodigdheden/voorbeeld data voor een call staat al klaar daar

De database staat in de MableAPI/MableAPI map nadat het project 1 keer opgestart is.

Voor extra requirements heb ik gekozen voor het groeperen per categorie en heb ik dat gecombineerd met het classificeren van producten.
Voor het derde punt heb ik gekozen voor het favoriten van items. Je kan ook een lijst met favorieten opvragen.

EntityFrameworkCore is gebruikt om de data van de database te gebruiken. Zelf had ik daar nog geen ervaring mee, maar het lijkt wel op XPO voor devexpress
Het maakt voor makkelijke communicatie met data uit een database. Hierdoor heb je niet altijd pure sql nodig om data op te vragen. al kan dat wel.

Bij sommige comments laat ik zien dat ik dit in een groter of langer project anders zou doen, zoals authenticatie(Hardcoded login gegevens) en het gebruik van materiaal van een artikel
