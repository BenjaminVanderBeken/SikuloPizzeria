# Consignes et verification du projet

Ce fichier reprend les points eliminatoires importants pour la remise du projet.

## Points eliminatoires

| Situation | Consequence | Etat du projet |
| --- | --- | --- |
| ZIP absent sur Moodle | 0 directement | A deposer manuellement sur Moodle si demande |
| Depot GitHub absent | 0 directement | A pousser sur GitHub avant la remise |
| Application non fonctionnelle | 0 directement | Frontend compile, API testee sur les endpoints principaux |
| README absent | 0 directement | README.md present a la racine |
| Projet impossible a lancer en suivant le README | 0 directement | Instructions de lancement presentes dans README.md |
| Utilisation d'Entity Framework | 0 directement | Entity Framework non utilise |
| Absence de Clean Architecture | 0 directement | Backend separe en API, Core et Infrastructure |
| Utilisation d'une librairie de gestion d'etat externe | 0 directement | Pas de NgRx, Redux ou librairie equivalente |

## Architecture attendue

Le projet suit une separation en couches :

```text
Frontend Angular
-> Services Angular
-> API ASP.NET Core
-> Services metier Core
-> Interfaces Repository
-> Repositories Infrastructure
-> Dapper
-> MySQL
```

## Technologies principales

- Angular
- ASP.NET Core
- Clean Architecture
- Dapper
- MySQL
- Reactive Forms
- HttpClient
- Services Angular
- Signals Angular

## Verification avant remise

Commandes utiles :

```powershell
dotnet build .\SikuloPizzeria.sln
cd .\frontend
npm run build
npm test -- --watch=false
```

Le projet ne doit pas contenir dans le depot GitHub :

```text
node_modules
bin
obj
dist
.vs
*.zip
```

Ces elements sont ignores par `.gitignore`.
