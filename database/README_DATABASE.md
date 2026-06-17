# Base de données Sikulo Pizzeria

## SGBD cible

- MySQL 8 ou une version ultérieure
- Jeu de caractères : `utf8mb4`
- Moteur des tables : `InnoDB`

## Ordre d'exécution

Exécuter les scripts dans l'ordre suivant :

```text
01-create-database.sql
02-create-tables.sql
03-create-views.sql
04-insert-test-data.sql
```

Exemple en ligne de commande PowerShell :

```powershell
mysql -u root -p < .\01-create-database.sql
mysql -u root -p < .\02-create-tables.sql
mysql -u root -p < .\03-create-views.sql
mysql -u root -p < .\04-insert-test-data.sql
```

## Modèle retenu

Le modèle contient 14 tables :

1. `categories`
2. `produits`
3. `variantes_produits`
4. `ingredients`
5. `composition_produits`
6. `supplements`
7. `clients`
8. `promotions`
9. `commandes`
10. `details_commandes`
11. `details_supplements`
12. `horaires_ouverture`
13. `utilisateurs`
14. `logs_activites`

Les vues disponibles sont :

- `v_commandes_actives`
- `v_ventes_jour`
- `v_produits_populaires`
- `v_clients_statistiques`

## Principales corrections par rapport au script initial

- ajout de `variantes_produits` pour ne plus stocker plusieurs tailles dans une chaîne ;
- ajout de la relation `commandes.promotion_id` ;
- correction de `allerenes` en `allergenes` ;
- remplacement des suppressions dangereuses par `ON DELETE RESTRICT` ;
- ajout du statut de commande `TERMINEE` ;
- ajout de contraintes de validation sur les prix, quantités, dates et statuts ;
- ajout d'une contrainte d'unicité dans `details_supplements` ;
- calcul des statistiques clients dans une vue plutôt que dans des colonnes dupliquées ;
- conservation des prix appliqués dans les lignes de commande pour protéger l'historique.

## Utilisation avec Dapper

Les requêtes SQL seront placées uniquement dans les repositories du projet
`Infrastructure`. Les contrôleurs de l'API ne devront jamais ouvrir directement
une connexion MySQL et ne devront contenir aucune requête SQL.
