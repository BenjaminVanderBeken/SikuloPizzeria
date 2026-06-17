-- ============================================================================
-- SIKULO PIZZERIA - 02 - CREATION DES TABLES
-- SGBD cible : MySQL 8+
-- Le script peut être réexécuté : les tables existantes sont supprimées.
-- ============================================================================

USE sikulo_pizzeria;
SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

DROP TABLE IF EXISTS logs_activites;
DROP TABLE IF EXISTS details_supplements;
DROP TABLE IF EXISTS details_commandes;
DROP TABLE IF EXISTS commandes;
DROP TABLE IF EXISTS promotions;
DROP TABLE IF EXISTS utilisateurs;
DROP TABLE IF EXISTS horaires_ouverture;
DROP TABLE IF EXISTS clients;
DROP TABLE IF EXISTS supplements;
DROP TABLE IF EXISTS composition_produits;
DROP TABLE IF EXISTS ingredients;
DROP TABLE IF EXISTS variantes_produits;
DROP TABLE IF EXISTS produits;
DROP TABLE IF EXISTS categories;

SET FOREIGN_KEY_CHECKS = 1;

-- --------------------------------------------------------------------------
-- 1. Catégories
-- --------------------------------------------------------------------------
CREATE TABLE categories (
    id INT UNSIGNED NOT NULL AUTO_INCREMENT,
    nom VARCHAR(100) NOT NULL,
    description TEXT NULL,
    ordre_affichage INT NOT NULL DEFAULT 0,
    actif BOOLEAN NOT NULL DEFAULT TRUE,
    date_creation TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    date_modification TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
        ON UPDATE CURRENT_TIMESTAMP,

    CONSTRAINT pk_categories PRIMARY KEY (id),
    CONSTRAINT uq_categories_nom UNIQUE (nom),
    CONSTRAINT ck_categories_ordre CHECK (ordre_affichage >= 0)
) ENGINE = InnoDB;

-- --------------------------------------------------------------------------
-- 2. Produits
-- La catégorie définit le type du produit. Les produits sont désactivés plutôt
-- que supprimés afin de conserver l'historique des commandes.
-- --------------------------------------------------------------------------
CREATE TABLE produits (
    id INT UNSIGNED NOT NULL AUTO_INCREMENT,
    categorie_id INT UNSIGNED NOT NULL,
    nom VARCHAR(150) NOT NULL,
    description TEXT NULL,
    prix_base DECIMAL(10, 2) NOT NULL,
    prix_promotion DECIMAL(10, 2) NULL,
    image_url VARCHAR(500) NULL,
    permet_supplement BOOLEAN NOT NULL DEFAULT FALSE,
    actif BOOLEAN NOT NULL DEFAULT TRUE,
    populaire BOOLEAN NOT NULL DEFAULT FALSE,
    ordre_affichage INT NOT NULL DEFAULT 0,
    date_creation TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    date_modification TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
        ON UPDATE CURRENT_TIMESTAMP,

    CONSTRAINT pk_produits PRIMARY KEY (id),
    CONSTRAINT fk_produits_categories
        FOREIGN KEY (categorie_id) REFERENCES categories(id)
        ON UPDATE CASCADE ON DELETE RESTRICT,
    CONSTRAINT uq_produits_categorie_nom UNIQUE (categorie_id, nom),
    CONSTRAINT ck_produits_prix_base CHECK (prix_base >= 0),
    CONSTRAINT ck_produits_prix_promotion
        CHECK (prix_promotion IS NULL OR prix_promotion >= 0),
    CONSTRAINT ck_produits_ordre CHECK (ordre_affichage >= 0),

    INDEX idx_produits_categorie (categorie_id),
    INDEX idx_produits_actif (actif),
    INDEX idx_produits_populaire (populaire)
) ENGINE = InnoDB;

-- --------------------------------------------------------------------------
-- 3. Variantes de produits
-- Exemples : Petite, Moyenne, Grande, 33 cl, 50 cl.
-- --------------------------------------------------------------------------
CREATE TABLE variantes_produits (
    id INT UNSIGNED NOT NULL AUTO_INCREMENT,
    produit_id INT UNSIGNED NOT NULL,
    nom VARCHAR(50) NOT NULL,
    prix DECIMAL(10, 2) NOT NULL,
    actif BOOLEAN NOT NULL DEFAULT TRUE,
    ordre_affichage INT NOT NULL DEFAULT 0,

    CONSTRAINT pk_variantes_produits PRIMARY KEY (id),
    CONSTRAINT fk_variantes_produits_produits
        FOREIGN KEY (produit_id) REFERENCES produits(id)
        ON UPDATE CASCADE ON DELETE CASCADE,
    CONSTRAINT uq_variantes_produit_nom UNIQUE (produit_id, nom),
    CONSTRAINT ck_variantes_prix CHECK (prix >= 0),
    CONSTRAINT ck_variantes_ordre CHECK (ordre_affichage >= 0),

    INDEX idx_variantes_produit (produit_id),
    INDEX idx_variantes_actif (actif)
) ENGINE = InnoDB;

-- --------------------------------------------------------------------------
-- 4. Ingrédients
-- --------------------------------------------------------------------------
CREATE TABLE ingredients (
    id INT UNSIGNED NOT NULL AUTO_INCREMENT,
    nom VARCHAR(100) NOT NULL,
    type VARCHAR(30) NOT NULL,
    stock_actuel DECIMAL(10, 2) NOT NULL DEFAULT 0,
    unite_mesure VARCHAR(20) NOT NULL,
    prix_unitaire DECIMAL(10, 2) NULL,
    allergenes VARCHAR(255) NULL,
    actif BOOLEAN NOT NULL DEFAULT TRUE,
    date_creation TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    date_modification TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
        ON UPDATE CURRENT_TIMESTAMP,

    CONSTRAINT pk_ingredients PRIMARY KEY (id),
    CONSTRAINT uq_ingredients_nom UNIQUE (nom),
    CONSTRAINT ck_ingredients_type CHECK (
        type IN ('SAUCE', 'VIANDE', 'FROMAGE', 'LEGUME',
                 'ACCOMPAGNEMENT', 'AUTRE')
    ),
    CONSTRAINT ck_ingredients_stock CHECK (stock_actuel >= 0),
    CONSTRAINT ck_ingredients_prix
        CHECK (prix_unitaire IS NULL OR prix_unitaire >= 0),

    INDEX idx_ingredients_type (type),
    INDEX idx_ingredients_actif (actif)
) ENGINE = InnoDB;

-- --------------------------------------------------------------------------
-- 5. Composition des produits (table d'association Produit - Ingrédient)
-- --------------------------------------------------------------------------
CREATE TABLE composition_produits (
    id INT UNSIGNED NOT NULL AUTO_INCREMENT,
    produit_id INT UNSIGNED NOT NULL,
    ingredient_id INT UNSIGNED NOT NULL,
    quantite DECIMAL(10, 2) NOT NULL,
    unite VARCHAR(20) NOT NULL,
    ordre_affichage INT NOT NULL DEFAULT 0,

    CONSTRAINT pk_composition_produits PRIMARY KEY (id),
    CONSTRAINT fk_composition_produits_produits
        FOREIGN KEY (produit_id) REFERENCES produits(id)
        ON UPDATE CASCADE ON DELETE CASCADE,
    CONSTRAINT fk_composition_produits_ingredients
        FOREIGN KEY (ingredient_id) REFERENCES ingredients(id)
        ON UPDATE CASCADE ON DELETE RESTRICT,
    CONSTRAINT uq_composition_produit_ingredient
        UNIQUE (produit_id, ingredient_id),
    CONSTRAINT ck_composition_quantite CHECK (quantite > 0),
    CONSTRAINT ck_composition_ordre CHECK (ordre_affichage >= 0),

    INDEX idx_composition_produit (produit_id),
    INDEX idx_composition_ingredient (ingredient_id)
) ENGINE = InnoDB;

-- --------------------------------------------------------------------------
-- 6. Suppléments
-- --------------------------------------------------------------------------
CREATE TABLE supplements (
    id INT UNSIGNED NOT NULL AUTO_INCREMENT,
    nom VARCHAR(100) NOT NULL,
    type VARCHAR(30) NOT NULL,
    prix DECIMAL(10, 2) NOT NULL,
    actif BOOLEAN NOT NULL DEFAULT TRUE,
    date_creation TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    date_modification TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
        ON UPDATE CURRENT_TIMESTAMP,

    CONSTRAINT pk_supplements PRIMARY KEY (id),
    CONSTRAINT uq_supplements_nom UNIQUE (nom),
    CONSTRAINT ck_supplements_type CHECK (
        type IN ('FROMAGE', 'LEGUME', 'VIANDE', 'SAUCE', 'AUTRE')
    ),
    CONSTRAINT ck_supplements_prix CHECK (prix >= 0),

    INDEX idx_supplements_type (type),
    INDEX idx_supplements_actif (actif)
) ENGINE = InnoDB;

-- --------------------------------------------------------------------------
-- 7. Clients
-- Les statistiques d'achat sont calculées par une vue afin d'éviter les
-- données dupliquées ou désynchronisées.
-- --------------------------------------------------------------------------
CREATE TABLE clients (
    id INT UNSIGNED NOT NULL AUTO_INCREMENT,
    nom VARCHAR(100) NOT NULL,
    prenom VARCHAR(100) NULL,
    email VARCHAR(150) NULL,
    telephone VARCHAR(25) NULL,
    adresse_rue VARCHAR(200) NULL,
    adresse_numero VARCHAR(20) NULL,
    adresse_boite VARCHAR(10) NULL,
    adresse_code_postal VARCHAR(10) NULL,
    adresse_ville VARCHAR(100) NULL,
    adresse_pays VARCHAR(50) NOT NULL DEFAULT 'Belgique',
    notes TEXT NULL,
    client_vip BOOLEAN NOT NULL DEFAULT FALSE,
    actif BOOLEAN NOT NULL DEFAULT TRUE,
    date_creation TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    date_modification TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
        ON UPDATE CURRENT_TIMESTAMP,

    CONSTRAINT pk_clients PRIMARY KEY (id),
    CONSTRAINT uq_clients_email UNIQUE (email),

    INDEX idx_clients_nom_prenom (nom, prenom),
    INDEX idx_clients_telephone (telephone),
    INDEX idx_clients_actif (actif)
) ENGINE = InnoDB;

-- --------------------------------------------------------------------------
-- 8. Promotions
-- --------------------------------------------------------------------------
CREATE TABLE promotions (
    id INT UNSIGNED NOT NULL AUTO_INCREMENT,
    code VARCHAR(20) NOT NULL,
    description TEXT NULL,
    type_reduction VARCHAR(20) NOT NULL DEFAULT 'POURCENTAGE',
    valeur_reduction DECIMAL(10, 2) NOT NULL,
    montant_minimum DECIMAL(10, 2) NULL,
    date_debut DATE NOT NULL,
    date_fin DATE NOT NULL,
    nombre_utilisations_max INT UNSIGNED NULL,
    nombre_utilisations_actuelles INT UNSIGNED NOT NULL DEFAULT 0,
    actif BOOLEAN NOT NULL DEFAULT TRUE,
    date_creation TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    date_modification TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
        ON UPDATE CURRENT_TIMESTAMP,

    CONSTRAINT pk_promotions PRIMARY KEY (id),
    CONSTRAINT uq_promotions_code UNIQUE (code),
    CONSTRAINT ck_promotions_type CHECK (
        type_reduction IN ('POURCENTAGE', 'MONTANT_FIXE')
    ),
    CONSTRAINT ck_promotions_valeur CHECK (
        valeur_reduction > 0
        AND (type_reduction <> 'POURCENTAGE' OR valeur_reduction <= 100)
    ),
    CONSTRAINT ck_promotions_minimum
        CHECK (montant_minimum IS NULL OR montant_minimum >= 0),
    CONSTRAINT ck_promotions_dates CHECK (date_fin >= date_debut),
    CONSTRAINT ck_promotions_utilisations CHECK (
        nombre_utilisations_max IS NULL
        OR nombre_utilisations_actuelles <= nombre_utilisations_max
    ),

    INDEX idx_promotions_dates (date_debut, date_fin),
    INDEX idx_promotions_actif (actif)
) ENGINE = InnoDB;

-- --------------------------------------------------------------------------
-- 9. Commandes
-- --------------------------------------------------------------------------
CREATE TABLE commandes (
    id INT UNSIGNED NOT NULL AUTO_INCREMENT,
    numero_commande VARCHAR(25) NOT NULL,
    client_id INT UNSIGNED NULL,
    promotion_id INT UNSIGNED NULL,
    date_commande TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    date_livraison_prevue DATETIME NULL,
    date_livraison_reelle DATETIME NULL,
    type_commande VARCHAR(20) NOT NULL DEFAULT 'A_EMPORTER',
    statut VARCHAR(25) NOT NULL DEFAULT 'EN_ATTENTE',
    sous_total DECIMAL(10, 2) NOT NULL,
    tva_montant DECIMAL(10, 2) NOT NULL DEFAULT 0,
    frais_livraison DECIMAL(10, 2) NOT NULL DEFAULT 0,
    reduction_montant DECIMAL(10, 2) NOT NULL DEFAULT 0,
    montant_total DECIMAL(10, 2) NOT NULL,
    mode_paiement VARCHAR(25) NOT NULL DEFAULT 'EN_ATTENTE',
    statut_paiement VARCHAR(20) NOT NULL DEFAULT 'NON_PAYEE',
    notes_client TEXT NULL,
    notes_cuisine TEXT NULL,
    demandes_speciales TEXT NULL,
    date_modification TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
        ON UPDATE CURRENT_TIMESTAMP,

    CONSTRAINT pk_commandes PRIMARY KEY (id),
    CONSTRAINT uq_commandes_numero UNIQUE (numero_commande),
    CONSTRAINT fk_commandes_clients
        FOREIGN KEY (client_id) REFERENCES clients(id)
        ON UPDATE CASCADE ON DELETE SET NULL,
    CONSTRAINT fk_commandes_promotions
        FOREIGN KEY (promotion_id) REFERENCES promotions(id)
        ON UPDATE CASCADE ON DELETE SET NULL,
    CONSTRAINT ck_commandes_type CHECK (
        type_commande IN ('SUR_PLACE', 'A_EMPORTER', 'LIVRAISON')
    ),
    CONSTRAINT ck_commandes_statut CHECK (
        statut IN ('EN_ATTENTE', 'CONFIRMEE', 'EN_PREPARATION', 'PRETE',
                   'LIVREE', 'TERMINEE', 'ANNULEE')
    ),
    CONSTRAINT ck_commandes_mode_paiement CHECK (
        mode_paiement IN ('ESPECES', 'CARTE', 'VIREMENT', 'EN_ATTENTE')
    ),
    CONSTRAINT ck_commandes_statut_paiement CHECK (
        statut_paiement IN ('NON_PAYEE', 'PAYEE', 'REMBOURSEE')
    ),
    CONSTRAINT ck_commandes_montants CHECK (
        sous_total >= 0
        AND tva_montant >= 0
        AND frais_livraison >= 0
        AND reduction_montant >= 0
        AND montant_total >= 0
    ),
    CONSTRAINT ck_commandes_dates_livraison CHECK (
        date_livraison_reelle IS NULL
        OR date_livraison_prevue IS NULL
        OR date_livraison_reelle >= date_commande
    ),

    INDEX idx_commandes_client (client_id),
    INDEX idx_commandes_promotion (promotion_id),
    INDEX idx_commandes_date (date_commande),
    INDEX idx_commandes_statut (statut),
    INDEX idx_commandes_paiement (statut_paiement),
    INDEX idx_commandes_type (type_commande)
) ENGINE = InnoDB;

-- --------------------------------------------------------------------------
-- 10. Détails des commandes
-- prix_unitaire garde le prix appliqué au moment de la vente.
-- --------------------------------------------------------------------------
CREATE TABLE details_commandes (
    id INT UNSIGNED NOT NULL AUTO_INCREMENT,
    commande_id INT UNSIGNED NOT NULL,
    produit_id INT UNSIGNED NOT NULL,
    variante_id INT UNSIGNED NULL,
    quantite INT UNSIGNED NOT NULL,
    prix_unitaire DECIMAL(10, 2) NOT NULL,
    prix_total DECIMAL(10, 2) NOT NULL,
    nom_variante VARCHAR(50) NULL,
    notes_particulieres TEXT NULL,
    cout_supplements DECIMAL(10, 2) NOT NULL DEFAULT 0,

    CONSTRAINT pk_details_commandes PRIMARY KEY (id),
    CONSTRAINT fk_details_commandes_commandes
        FOREIGN KEY (commande_id) REFERENCES commandes(id)
        ON UPDATE CASCADE ON DELETE CASCADE,
    CONSTRAINT fk_details_commandes_produits
        FOREIGN KEY (produit_id) REFERENCES produits(id)
        ON UPDATE CASCADE ON DELETE RESTRICT,
    CONSTRAINT fk_details_commandes_variantes
        FOREIGN KEY (variante_id) REFERENCES variantes_produits(id)
        ON UPDATE CASCADE ON DELETE SET NULL,
    CONSTRAINT ck_details_commandes_quantite CHECK (quantite > 0),
    CONSTRAINT ck_details_commandes_prix CHECK (
        prix_unitaire >= 0 AND prix_total >= 0 AND cout_supplements >= 0
    ),

    INDEX idx_details_commandes_commande (commande_id),
    INDEX idx_details_commandes_produit (produit_id),
    INDEX idx_details_commandes_variante (variante_id)
) ENGINE = InnoDB;

-- --------------------------------------------------------------------------
-- 11. Suppléments appliqués aux détails de commande
-- --------------------------------------------------------------------------
CREATE TABLE details_supplements (
    id INT UNSIGNED NOT NULL AUTO_INCREMENT,
    detail_commande_id INT UNSIGNED NOT NULL,
    supplement_id INT UNSIGNED NOT NULL,
    quantite INT UNSIGNED NOT NULL DEFAULT 1,
    prix_unitaire DECIMAL(10, 2) NOT NULL,

    CONSTRAINT pk_details_supplements PRIMARY KEY (id),
    CONSTRAINT fk_details_supplements_details
        FOREIGN KEY (detail_commande_id) REFERENCES details_commandes(id)
        ON UPDATE CASCADE ON DELETE CASCADE,
    CONSTRAINT fk_details_supplements_supplements
        FOREIGN KEY (supplement_id) REFERENCES supplements(id)
        ON UPDATE CASCADE ON DELETE RESTRICT,
    CONSTRAINT uq_details_supplements_detail_supplement
        UNIQUE (detail_commande_id, supplement_id),
    CONSTRAINT ck_details_supplements_quantite CHECK (quantite > 0),
    CONSTRAINT ck_details_supplements_prix CHECK (prix_unitaire >= 0),

    INDEX idx_details_supplements_detail (detail_commande_id),
    INDEX idx_details_supplements_supplement (supplement_id)
) ENGINE = InnoDB;

-- --------------------------------------------------------------------------
-- 12. Horaires d'ouverture
-- --------------------------------------------------------------------------
CREATE TABLE horaires_ouverture (
    id INT UNSIGNED NOT NULL AUTO_INCREMENT,
    jour_semaine VARCHAR(10) NOT NULL,
    heure_ouverture TIME NULL,
    heure_fermeture TIME NULL,
    ouvert BOOLEAN NOT NULL DEFAULT TRUE,
    notes VARCHAR(200) NULL,

    CONSTRAINT pk_horaires_ouverture PRIMARY KEY (id),
    CONSTRAINT uq_horaires_jour UNIQUE (jour_semaine),
    CONSTRAINT ck_horaires_jour CHECK (
        jour_semaine IN ('LUNDI', 'MARDI', 'MERCREDI', 'JEUDI',
                         'VENDREDI', 'SAMEDI', 'DIMANCHE')
    ),
    CONSTRAINT ck_horaires_heures CHECK (
        ouvert = FALSE
        OR (heure_ouverture IS NOT NULL
            AND heure_fermeture IS NOT NULL
            AND heure_fermeture > heure_ouverture)
    )
) ENGINE = InnoDB;

-- --------------------------------------------------------------------------
-- 13. Utilisateurs / membres du personnel
-- --------------------------------------------------------------------------
CREATE TABLE utilisateurs (
    id INT UNSIGNED NOT NULL AUTO_INCREMENT,
    nom VARCHAR(100) NOT NULL,
    prenom VARCHAR(100) NULL,
    email VARCHAR(150) NOT NULL,
    telephone VARCHAR(25) NULL,
    password_hash VARCHAR(255) NOT NULL,
    role VARCHAR(20) NOT NULL DEFAULT 'ACCUEIL',
    actif BOOLEAN NOT NULL DEFAULT TRUE,
    date_creation TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    date_dernier_acces TIMESTAMP NULL,

    CONSTRAINT pk_utilisateurs PRIMARY KEY (id),
    CONSTRAINT uq_utilisateurs_email UNIQUE (email),
    CONSTRAINT ck_utilisateurs_role CHECK (
        role IN ('ADMIN', 'MANAGER', 'CUISINE', 'LIVRAISON', 'ACCUEIL')
    ),

    INDEX idx_utilisateurs_role (role),
    INDEX idx_utilisateurs_actif (actif)
) ENGINE = InnoDB;

-- --------------------------------------------------------------------------
-- 14. Journal des activités
-- --------------------------------------------------------------------------
CREATE TABLE logs_activites (
    id BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
    utilisateur_id INT UNSIGNED NULL,
    type_action VARCHAR(40) NOT NULL,
    description TEXT NULL,
    donnees_json JSON NULL,
    reference_type VARCHAR(50) NULL,
    reference_id INT UNSIGNED NULL,
    date_action TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT pk_logs_activites PRIMARY KEY (id),
    CONSTRAINT fk_logs_activites_utilisateurs
        FOREIGN KEY (utilisateur_id) REFERENCES utilisateurs(id)
        ON UPDATE CASCADE ON DELETE SET NULL,
    CONSTRAINT ck_logs_activites_type CHECK (
        type_action IN (
            'CONNEXION', 'DECONNEXION',
            'COMMANDE_CREEE', 'COMMANDE_MODIFIEE',
            'COMMANDE_CONFIRMEE', 'COMMANDE_ANNULEE',
            'PRODUIT_AJOUTE', 'PRODUIT_MODIFIE', 'PRODUIT_DESACTIVE',
            'CLIENT_AJOUTE', 'CLIENT_MODIFIE', 'AUTRE'
        )
    ),

    INDEX idx_logs_utilisateur (utilisateur_id),
    INDEX idx_logs_type_action (type_action),
    INDEX idx_logs_date_action (date_action),
    INDEX idx_logs_reference (reference_type, reference_id)
) ENGINE = InnoDB;
