# SikuloPizzeria

Projet Angular et .NET realise dans le cadre du cours de developpement web.

SikuloPizzeria est une application de gestion pour une pizzeria. Elle permet de gerer les categories, les produits, les variantes, les ingredients, les compositions produits-ingredients, les clients, les commandes et les statistiques principales.

---

## 1. Technologies utilisees

### Backend

* ASP.NET Core
* .NET SDK 10.0.301
* Clean Architecture
* Dapper
* MySql.Data
* MySQL 9.1.0
* API REST
* Pattern Repository
* Services metier

Le projet n'utilise pas Entity Framework.

L'acces aux donnees est realise exclusivement avec Dapper dans les repositories du projet Infrastructure.

### Frontend

* Angular 21.2.17
* Angular CLI 21.2.15
* Node.js v24.14.1
* npm 11.11.0
* TypeScript 5.9.3
* HttpClient
* Reactive Forms
* Routing Angular
* Services Angular
* Signals Angular
* Nouvelle syntaxe Angular : @if et @for

Le projet n'utilise pas NgRx, Redux ou une autre bibliotheque externe de gestion d'etat.

L'etat de l'application est gere uniquement dans les services Angular.

---

## 2. Architecture du projet

Le backend respecte une Clean Architecture en couches avec une separation claire entre API, Core et Infrastructure.

Structure generale :

```
SikuloPizzeria
|
|-- backend
|   |-- SikuloPizzeria.API
|   |   |-- Controllers
|   |   |-- Program.cs
|   |   |-- appsettings.json
|   |
|   |-- SikuloPizzeria.Core
|   |   |-- DTOs
|   |   |-- Entities
|   |   |-- Exceptions
|   |   |-- Interfaces
|   |   |-- Services
|   |
|   |-- SikuloPizzeria.Infrastructure
|       |-- Data
|       |-- Repositories
|
|-- frontend
|   |-- public
|   |   |-- images
|   |-- src
|       |-- app
|           |-- models
|           |-- pages
|           |-- services
|           |-- app.routes.ts
|           |-- app.html
|
|-- database
|   |-- 01-create-database.sql
|   |-- 02-create-tables.sql
|   |-- 03-create-views.sql
|   |-- 04-insert-test-data.sql
|   |-- README_DATABASE.md
|
|-- SikuloPizzeria.sln
|-- consignes.md
|-- global.json
|-- .gitignore
|-- README.md
```

---

## 3. Role des projets backend

### SikuloPizzeria.API

Le projet API contient les controleurs ASP.NET Core.

Les controleurs recoivent les requetes HTTP et appellent les services du Core.

Les controleurs ne contiennent pas de requetes SQL.

### SikuloPizzeria.Core

Le projet Core contient la logique principale de l'application :

* entities
* DTOs
* interfaces
* services metier
* exceptions

Le Core ne depend pas de l'Infrastructure.

Il definit les contrats utilises par l'application.

### SikuloPizzeria.Infrastructure

Le projet Infrastructure contient l'acces aux donnees :

* repositories
* requetes SQL
* utilisation de Dapper
* connexion MySQL

Toutes les communications avec la base de donnees passent par les repositories.

---

## 4. Flux applicatif

Parcours general d'une donnee :

```
Angular Component
-> Angular Service
-> API Controller
-> Core Service
-> Repository Interface
-> Infrastructure Repository
-> Dapper
-> MySQL
-> retour vers Repository
-> retour vers Core Service
-> retour vers API Controller
-> retour vers Angular Service
-> retour vers Angular Component
-> affichage dans l'interface
```

Exemple avec les commandes :

```
CommandesPage
-> CommandeService Angular
-> CommandesController
-> CommandeService Core
-> ICommandeRepository
-> CommandeRepository
-> Dapper
-> tables commandes, details_commandes, produits, clients
```

Cette organisation permet de separer clairement les responsabilites.

---

## 5. Fonctionnalites principales

### Dashboard

* Affichage des statistiques principales
* Nombre de produits actifs
* Nombre de clients actifs
* Nombre total de commandes
* Nombre de commandes du jour
* Nombre de commandes en attente
* Chiffre d'affaires paye
* Nombre d'ingredients actifs
* Nombre de compositions produits-ingredients

### Categories

* Affichage des categories
* Ajout d'une categorie
* Modification d'une categorie
* Desactivation ou reactivation
* Suppression securisee

### Produits

* Affichage des produits
* Ajout d'un produit
* Modification d'un produit
* Desactivation ou reactivation
* Suppression securisee
* Association a une categorie
* Gestion du prix
* Gestion du type de produit

### Variantes

* Affichage des variantes
* Association d'une variante a un produit
* Gestion du prix supplementaire
* Utilisation lors de la creation d'une commande

### Ingredients

* Affichage des ingredients
* Ajout d'un ingredient
* Modification d'un ingredient
* Desactivation ou reactivation
* Suppression securisee

### Compositions produits-ingredients

* Association d'un produit avec un ingredient
* Definition d'une quantite
* Definition d'une unite
* Affichage avec jointures entre produits, ingredients et compositions
* Suppression d'une ligne de composition

### Clients

* Affichage des clients
* Ajout d'un client
* Modification d'un client
* Desactivation ou reactivation
* Suppression securisee

### Commandes

* Creation d'une commande
* Vente comptoir ou client enregistre
* Ajout de plusieurs produits
* Choix des variantes
* Calcul automatique des sous-totaux
* Calcul de la TVA
* Calcul des frais de livraison
* Calcul du total
* Gestion du statut de la commande
* Gestion du paiement
* Annulation d'une commande
* Suppression definitive d'une commande avec confirmation
* Creation transactionnelle avec Dapper

---

## 6. Prerequis

Avant de lancer le projet, installer :

* .NET SDK 10.0.301
* Node.js v24.14.1
* npm 11.11.0
* Angular CLI 21.2.15
* MySQL 9.1.0
* Visual Studio Code
* WampServer avec MySQL 9.1.0

Verifier la version de .NET :

```
dotnet --version
```

Version utilisee pendant le developpement :

```
10.0.301
```

Verifier la version de Node.js :

```
node -v
```

Version utilisee pendant le developpement :

```
v24.14.1
```

Verifier la version de npm :

```
npm -v
```

Version utilisee pendant le developpement :

```
11.11.0
```

Verifier Angular :

```
npx ng version
```

Versions utilisees pendant le developpement :

```
Angular CLI : 21.2.15
Angular     : 21.2.17
TypeScript  : 5.9.3
```

Verifier MySQL :

```
mysql --version
```

Version utilisee pendant le developpement :

```
MySQL 9.1.0
```

Si la commande mysql n'est pas reconnue, il faut ajouter MySQL au PATH ou utiliser le chemin complet vers mysql.exe selon l'installation locale de MySQL.

---

## 7. Installation du projet

Apres avoir clone le depot GitHub ou extrait le fichier ZIP, ouvrir un terminal a la racine du projet.

La racine du projet est le dossier qui contient le fichier :

```
SikuloPizzeria.sln
```

Restaurer les dependances .NET :

```
dotnet restore .\SikuloPizzeria.sln
```

Compiler le backend :

```
dotnet build .\SikuloPizzeria.sln
```

Installer les dependances Angular :

```
cd .\frontend
npm install
```

Compiler le frontend :

```
npm run build
```

Lancer les tests frontend :

```
npm test -- --watch=false
```

Revenir a la racine du projet :

```
cd ..
```

---

## 8. Base de donnees

Le projet a ete teste avec WampServer et MySQL.

Avant d'executer les scripts SQL, il faut demarrer WampServer et verifier que l'icone WampServer est verte. Cela indique que les services necessaires, dont MySQL, sont actifs.


Le projet utilise une base de donnees MySQL nommee :

```
sikulo_pizzeria
```

Les scripts SQL sont disponibles dans le dossier :

```
database
```

Les scripts doivent etre executes dans cet ordre :

```
01-create-database.sql
02-create-tables.sql
03-create-views.sql
04-insert-test-data.sql
```

Depuis la racine du projet, creer la base de donnees :

```
mysql -u root -h 127.0.0.1 -P 3306 < .\database\01-create-database.sql
```

Creer les tables :

```
mysql -u root -h 127.0.0.1 -P 3306 sikulo_pizzeria < .\database\02-create-tables.sql
```

Creer les vues :

```
mysql -u root -h 127.0.0.1 -P 3306 sikulo_pizzeria < .\database\03-create-views.sql
```

Inserer les donnees de test :

```
mysql -u root -h 127.0.0.1 -P 3306 sikulo_pizzeria < .\database\04-insert-test-data.sql
```

Verifier que les tables existent :

```
mysql -u root -h 127.0.0.1 -P 3306 sikulo_pizzeria -e "SHOW TABLES;"
```

Si MySQL demande un mot de passe, utiliser l'option -p.

Exemple :

```
mysql -u root -p -h 127.0.0.1 -P 3306 < .\database\01-create-database.sql
```

Si la commande mysql n'est pas reconnue, utiliser le chemin complet vers mysql.exe.

Exemple sous Windows :

```
& "C:\Program Files\MySQL\MySQL Server 9.5\bin\mysql.exe" -u root -h 127.0.0.1 -P 3306 < .\database\01-create-database.sql
```

---

## 9. Configuration du backend

Le fichier de configuration se trouve ici :

```
backend/SikuloPizzeria.API/appsettings.json
```

La chaine de connexion doit pointer vers MySQL.

Exemple sans mot de passe :

```
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=127.0.0.1;Port=3306;Database=sikulo_pizzeria;User ID=root;Password=;"
  }
}
```

Si MySQL utilise un mot de passe, modifier la partie Password=.

Exemple :

```
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=127.0.0.1;Port=3306;Database=sikulo_pizzeria;User ID=root;Password=motdepasse;"
  }
}
```

Aucune variable d'environnement obligatoire n'est necessaire pour lancer le projet.

La configuration principale se fait dans appsettings.json.

---

## 10. Lancement du backend

Depuis la racine du projet, lancer l'API :

```
dotnet run --project .\backend\SikuloPizzeria.API\SikuloPizzeria.API.csproj --urls http://localhost:5000
```

L'API est disponible a l'adresse :

```
http://localhost:5000
```

Tester l'API :

```
Invoke-RestMethod http://localhost:5000/api/dashboard/stats | ConvertTo-Json -Depth 5
```

Endpoints principaux :

```
GET /api/dashboard/stats
GET /api/categories
GET /api/produits
GET /api/ingredients
GET /api/composition-produits
GET /api/clients
GET /api/commandes
DELETE /api/commandes/{id}/definitif
```

---

## 11. Lancement du frontend

Ouvrir un deuxieme terminal.

Depuis la racine du projet, se placer dans le dossier frontend :

```
cd .\frontend
```

Lancer Angular :

```
npm start -- --port 4200
```

L'application est disponible a l'adresse :

```
http://localhost:4200
```

Pages disponibles :

```
/dashboard
/categories
/produits
/ingredients
/compositions
/clients
/commandes
```

---

## 12. Ordre de lancement conseille

1. Demarrer MySQL.
2. Creer la base de donnees avec les scripts SQL si elle n'existe pas encore.
3. Lancer le backend ASP.NET Core.
4. Lancer le frontend Angular.
5. Ouvrir le navigateur sur http://localhost:4200.

Terminal 1, depuis la racine :

```
dotnet run --project .\backend\SikuloPizzeria.API\SikuloPizzeria.API.csproj --urls http://localhost:5000
```

Terminal 2, depuis la racine :

```
cd .\frontend
npm start -- --port 4200
```

---

## 13. Comptes de test

L'application ne necessite pas d'authentification.

Aucun identifiant n'est necessaire pour tester l'application.

Les donnees de demonstration sont inserees par le script :

```
database/04-insert-test-data.sql
```

---

## 14. Verifications finales

Verifier les packages backend :

```
dotnet list .\SikuloPizzeria.sln package
```

Le projet doit afficher Dapper et MySql.Data.

Entity Framework ne doit pas apparaitre.

Compiler le backend :

```
dotnet build .\SikuloPizzeria.sln
```

Compiler le frontend :

```
cd .\frontend
npm run build
```

Lancer les tests frontend :

```
npm test -- --watch=false
```




