🩺 Détection du Diabète de Type 2 — Microservices .NET

📌 Objectif
Ce projet détecte automatiquement le risque de diabète de type 2 à partir des données patients et notes médicales. 
Il est basé sur une architecture microservices ASP.NET Core, sécurisée par JWT, avec un frontend Razor et déployée avec Docker Compose.

📦 Architecture
PatientService – Microservice avec SQL Server
NotesService – Microservice avec MongoDB
RiskAssessmentService – Évalue le risque de diabète (aucune base)
IdentityService – Authentification et JWT
OcelotGateway – API Gateway
FrontEndApp – Razor Pages UI

🚀 Déploiement local (via Docker Compose)
1. ✅ Prérequis
.NET 8 SDK
Docker
Git
MongoDB Compass (Tutoriel: https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-mongo-app?view=aspnetcore-7.0&tabs=visual-studio)

2. 📁 Cloner le projet
git clone https://github.com/talla761/OcelotAPIGatewayInMicroservice.git
cd OcelotAPIGatewayInMicroservice

3. 🛠️ Configuration des bases de données
SQL Server (pour PatientService) / Il existe une classe Dataseeder qui ajouter les 4 patients de tests si base de donnée est vide au lancement de l'application

Configuration dans appsettings.json du service :

{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=OCP10_DiabeteApp;Trusted_Connection=True;MultipleActiveResultSets=true;Encrypt=True; TrustServerCertificate=True"
  },
  "AllowedHosts": "*",
  "Jwt": {
    "Key": "ThisIsA32CharactersLongSecretKey!!",
    "Issuer": "http://localhost:7001",
    "Audience": "http://localhost:7001"
  }
}
Le conteneur Docker de SQL Server est défini dans docker-compose.yml.

MongoDB (pour NotesService)
Configuration dans appsettings.json :

"MongoDbSettings": {
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "NotesDatabase": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "OCP10_DiabeteApp",
    "NotesCollectionName": "Notes"
  },
  "AllowedHosts": "*",
  "Jwt": {
    "Key": "ThisIsA32CharactersLongSecretKey!!",
    "Issuer": "http://localhost:7001",
    "Audience": "http://localhost:7001"
  }
}
La base est automatiquement créée au lancement du service.

4. 🔐 Configuration de l’authentification (IdentityService)
Utilise Entity Framework Core avec une base SQL Server

Gère la création et l’authentification des utilisateurs via JWT

Exemple de appsettings.json :

"ConnectionStrings": {
  "DefaultConnection": "Server=sqlserver,1433;Database=IdentityDb;User=sa;Password=Your_password123;"
},
"JwtSettings": {
  "Key": "super_secret_jwt_key",
  "Issuer": "IdentityService",
  "Audience": "ApiUsers",
  "DurationInMinutes": 60
}

5. 🧩 Lancer les services (via Docker Compose)

docker-compose build
docker-compose up -d

Les services démarrés :

http://localhost:8000 → Frontend Razor (via Ocelot)

http://localhost:8001 → Ocelot Gateway

http://localhost:8002 → Identity API

http://localhost:8003 → Patient API

http://localhost:8004 → Notes API

http://localhost:8005 → Risk Assessment API

6. 👤 Création d’un utilisateur (via Identity)
Utiliser un outil comme Postman ou Swagger :

POST http://localhost:8002/api/auth/register
Content-Type: application/json

{
  "username": "admin",
  "password": "Admin123!",
  "email": "admin@example.com"
}
Connectez-vous pour récupérer le JWT :

POST /api/auth/login

7. 🌐 Accéder à l'application Razor
Une fois connecté, l'application utilise le token JWT pour appeler les microservices via l’API Gateway Ocelot.

Frontend : http://localhost:8000

🧪 Tests rapides
Créer un utilisateur sur le menu (Bouton S'inscrire)
Connecter vous Bouton Connexion)
Créer un patient via l’interface Razor

Ajouter une note médicale a ce patient

Lancer l’évaluation du risque : Dans la liste des patients, selectionner un patient puis cliquer sur son bouton voir.
                                Puis vous aurez les details du patient, ses notes et son evaluation de risque que se met a jour a chaque fois en fonction de ses notes
                                (Ajouter une notes du patient avec des mots clés. Ne pas oublier que l'age et le sexe du patient sont prises en considération)

🌱 Green Code
Traitement asynchrone (async/await)

Docker Compose évite les installations manuelles

API Gateway limite les appels directs aux services

Pas de surcharge mémoire sur l’analyse des notes

📁 Structure du projet
.
├── docker-compose.yml
├── FrontEndApp/
├── Gateway/
├── IdentityService/
├── PatientService/
├── NotesService/
├── RiskAssessmentService/
🧹 Commandes utiles
docker-compose down – arrêter tous les services

docker system prune – nettoyer les images/conteneurs inutilisés

docker logs <nom_du_service> – consulter les logs


♻️ Suggestions pour appliquer le Green Code
1. Limiter les appels réseau entre microservices
➕ Pourquoi : Moins de trafic réseau = moins de consommation CPU & réseau.
✅ Action :
Utiliser un cache en mémoire dans le service d’évaluation du risque pour éviter d’interroger plusieurs fois PatientService ou NotesService.
Grouper les appels quand possible (ex: récupérer toutes les notes en une seule requête).

2. Optimiser les requêtes base de données
✅ SQL Server (PatientService) :
Ne récupérer que les colonnes nécessaires.
Utiliser des indexes si les recherches sont fréquentes sur des colonnes précises (Id, Nom, etc.).

✅ MongoDB (NotesService) :
Indexer le champ patientId pour éviter des scans complets.
Éviter les champs inutiles ou très lourds dans chaque document.

3. Réduction des démarrages inutiles
✅ N’expose aucune route inutile ou de test dans les microservices.
✅ Ne démarre que les microservices nécessaires à l’environnement (ex: désactiver le frontend en environnement de test back-end pur).

4. Éviter la duplication de données
✅ Dans le service d’évaluation du risque, ne pas dupliquer les objets patients ou notes. Utilise DTO légers ou références (IDs uniquement).

5. Alléger le Frontend (Razor Pages)
Supprimer les assets inutiles (JS, CSS, images non utilisés).
Minimiser le nombre d’appels HTTP déclenchés au chargement.
Utiliser le Lazy Loading pour les sections non critiques.

6. Optimiser les jetons JWT
✅ Ne pas inclure de claims trop volumineux.
✅ Réduire la durée de vie des tokens en dev (et éviter leur régénération abusive).

7. Alléger les images Docker
Utiliser l’image .NET 8 ASP.NET slim si possible :

FROM mcr.microsoft.com/dotnet/aspnet:8.0-slim AS base

Utiliser le multi-stage build pour ne pas embarquer le SDK dans l’image finale :

FROM build AS publish
FROM mcr.microsoft.com/dotnet/aspnet:8.0-slim AS final

8. Surveiller les métriques
Intégrer un outil léger comme Prometheus ou Application Insights (avec échantillonnage), pour détecter les parties du système trop sollicitées ou trop lentes.

9. Adopter l'asynchrone systématiquement
Toute méthode qui appelle un service distant ou une base doit être en async/await pour ne pas bloquer inutilement les threads (et réduire l’usage CPU).

10. Documentation claire pour éviter les surcoûts humains
Un README.md clair et concis évite que chaque nouvel utilisateur ou développeur passe 2h à comprendre le déploiement → moins de machines démarrées inutilement pour "essayer de comprendre".
