-- ============================================================================
-- SIKULO PIZZERIA - 04 - DONNEES DE TEST
-- A exécuter après 01, 02 et 03.
-- ============================================================================

USE sikulo_pizzeria;

START TRANSACTION;

-- --------------------------------------------------------------------------
-- Catégories
-- --------------------------------------------------------------------------
INSERT INTO categories (nom, description, ordre_affichage) VALUES
('PIZZAS AL TAGLIO', 'Pizzas à la part et spécialités siciliennes', 1),
('TAVOLA CALDA', 'Plats chauds traditionnels italiens', 2),
('PANINI', 'Sandwiches italiens à la focaccia maison', 3),
('A PARTAGER', 'Plateaux et assortiments sur commande', 4),
('DOLCI', 'Desserts siciliens', 5),
('DRINKS', 'Boissons chaudes et froides', 6);

-- --------------------------------------------------------------------------
-- Produits
-- --------------------------------------------------------------------------
INSERT INTO produits (
    categorie_id, nom, description, prix_base, permet_supplement,
    populaire, ordre_affichage
) VALUES
((SELECT id FROM categories WHERE nom = 'PIZZAS AL TAGLIO'),
 'Margherita', 'Tomate, mozzarella et basilic', 8.50, TRUE, TRUE, 1),
((SELECT id FROM categories WHERE nom = 'PIZZAS AL TAGLIO'),
 'Diavola', 'Tomate, mozzarella et salami piquant', 9.50, TRUE, TRUE, 2),
((SELECT id FROM categories WHERE nom = 'TAVOLA CALDA'),
 'Lasagne maison', 'Lasagne à la sauce bolognaise', 12.00, FALSE, TRUE, 1),
((SELECT id FROM categories WHERE nom = 'TAVOLA CALDA'),
 'Arancini', 'Boulettes de riz siciliennes farcies', 4.00, TRUE, FALSE, 2),
((SELECT id FROM categories WHERE nom = 'PANINI'),
 'Panini Prosciutto', 'Focaccia, jambon italien et mozzarella', 8.50, TRUE, FALSE, 1),
((SELECT id FROM categories WHERE nom = 'DOLCI'),
 'Cannolo siciliano', 'Pâtisserie croustillante à la ricotta', 4.50, FALSE, TRUE, 1),
((SELECT id FROM categories WHERE nom = 'DOLCI'),
 'Tiramisu', 'Dessert italien au café et mascarpone', 5.50, FALSE, TRUE, 2),
((SELECT id FROM categories WHERE nom = 'DRINKS'),
 'Coca-Cola', 'Boisson gazeuse', 2.50, FALSE, FALSE, 1),
((SELECT id FROM categories WHERE nom = 'DRINKS'),
 'Eau pétillante', 'Eau minérale pétillante', 2.00, FALSE, FALSE, 2);

-- --------------------------------------------------------------------------
-- Variantes
-- --------------------------------------------------------------------------
INSERT INTO variantes_produits (produit_id, nom, prix, ordre_affichage) VALUES
((SELECT id FROM produits WHERE nom = 'Margherita'), 'PETITE', 8.50, 1),
((SELECT id FROM produits WHERE nom = 'Margherita'), 'MOYENNE', 10.50, 2),
((SELECT id FROM produits WHERE nom = 'Margherita'), 'GRANDE', 13.00, 3),
((SELECT id FROM produits WHERE nom = 'Diavola'), 'PETITE', 9.50, 1),
((SELECT id FROM produits WHERE nom = 'Diavola'), 'MOYENNE', 11.50, 2),
((SELECT id FROM produits WHERE nom = 'Diavola'), 'GRANDE', 14.00, 3),
((SELECT id FROM produits WHERE nom = 'Coca-Cola'), '33 CL', 2.50, 1),
((SELECT id FROM produits WHERE nom = 'Coca-Cola'), '50 CL', 3.50, 2);

-- --------------------------------------------------------------------------
-- Ingrédients
-- --------------------------------------------------------------------------
INSERT INTO ingredients (
    nom, type, stock_actuel, unite_mesure, prix_unitaire, allergenes
) VALUES
('Sauce tomate', 'SAUCE', 20000, 'g', 0.01, NULL),
('Mozzarella', 'FROMAGE', 10000, 'g', 0.02, 'Lactose'),
('Basilic', 'LEGUME', 500, 'g', 0.03, NULL),
('Salami piquant', 'VIANDE', 5000, 'g', 0.03, NULL),
('Pâte à pizza', 'ACCOMPAGNEMENT', 100, 'portion', 1.20, 'Gluten'),
('Sauce bolognaise', 'SAUCE', 10000, 'g', 0.02, NULL),
('Pâtes à lasagne', 'ACCOMPAGNEMENT', 100, 'portion', 0.80, 'Gluten, œufs'),
('Ricotta', 'FROMAGE', 5000, 'g', 0.02, 'Lactose'),
('Mascarpone', 'FROMAGE', 5000, 'g', 0.03, 'Lactose'),
('Café', 'AUTRE', 3000, 'g', 0.02, NULL);

-- --------------------------------------------------------------------------
-- Composition de quelques produits
-- --------------------------------------------------------------------------
INSERT INTO composition_produits (
    produit_id, ingredient_id, quantite, unite, ordre_affichage
) VALUES
((SELECT id FROM produits WHERE nom = 'Margherita'),
 (SELECT id FROM ingredients WHERE nom = 'Pâte à pizza'), 1, 'portion', 1),
((SELECT id FROM produits WHERE nom = 'Margherita'),
 (SELECT id FROM ingredients WHERE nom = 'Sauce tomate'), 120, 'g', 2),
((SELECT id FROM produits WHERE nom = 'Margherita'),
 (SELECT id FROM ingredients WHERE nom = 'Mozzarella'), 100, 'g', 3),
((SELECT id FROM produits WHERE nom = 'Margherita'),
 (SELECT id FROM ingredients WHERE nom = 'Basilic'), 5, 'g', 4),
((SELECT id FROM produits WHERE nom = 'Diavola'),
 (SELECT id FROM ingredients WHERE nom = 'Pâte à pizza'), 1, 'portion', 1),
((SELECT id FROM produits WHERE nom = 'Diavola'),
 (SELECT id FROM ingredients WHERE nom = 'Sauce tomate'), 120, 'g', 2),
((SELECT id FROM produits WHERE nom = 'Diavola'),
 (SELECT id FROM ingredients WHERE nom = 'Mozzarella'), 100, 'g', 3),
((SELECT id FROM produits WHERE nom = 'Diavola'),
 (SELECT id FROM ingredients WHERE nom = 'Salami piquant'), 70, 'g', 4),
((SELECT id FROM produits WHERE nom = 'Lasagne maison'),
 (SELECT id FROM ingredients WHERE nom = 'Sauce bolognaise'), 250, 'g', 1),
((SELECT id FROM produits WHERE nom = 'Lasagne maison'),
 (SELECT id FROM ingredients WHERE nom = 'Pâtes à lasagne'), 1, 'portion', 2);

-- --------------------------------------------------------------------------
-- Suppléments
-- --------------------------------------------------------------------------
INSERT INTO supplements (nom, type, prix) VALUES
('Fromage supplémentaire', 'FROMAGE', 1.50),
('Légumes supplémentaires', 'LEGUME', 0.50),
('Viande supplémentaire', 'VIANDE', 2.00),
('Sauce supplémentaire', 'SAUCE', 0.50);

-- --------------------------------------------------------------------------
-- Clients
-- --------------------------------------------------------------------------
INSERT INTO clients (
    nom, prenom, email, telephone,
    adresse_rue, adresse_numero, adresse_code_postal, adresse_ville,
    notes, client_vip
) VALUES
('Dupont', 'Marie', 'marie.dupont@example.com', '+32 470 11 22 33',
 'Rue de la Paix', '12', '1000', 'Bruxelles', NULL, TRUE),
('Martin', 'Lucas', 'lucas.martin@example.com', '+32 471 44 55 66',
 'Avenue Louise', '120', '1050', 'Ixelles', 'Préférence sans oignons', FALSE),
('Peeters', 'Sophie', 'sophie.peeters@example.com', '+32 472 77 88 99',
 'Chaussée de Louvain', '45', '1030', 'Schaerbeek', NULL, FALSE);

-- --------------------------------------------------------------------------
-- Promotion
-- --------------------------------------------------------------------------
INSERT INTO promotions (
    code, description, type_reduction, valeur_reduction,
    montant_minimum, date_debut, date_fin, nombre_utilisations_max
) VALUES
('BIENVENUE10', '10 % de réduction à partir de 20 euros',
 'POURCENTAGE', 10.00, 20.00, '2026-01-01', '2026-12-31', 500);

-- --------------------------------------------------------------------------
-- Horaires d'ouverture
-- --------------------------------------------------------------------------
INSERT INTO horaires_ouverture (
    jour_semaine, heure_ouverture, heure_fermeture, ouvert, notes
) VALUES
('LUNDI', '11:00:00', '22:00:00', TRUE, NULL),
('MARDI', '11:00:00', '22:00:00', TRUE, NULL),
('MERCREDI', '11:00:00', '22:00:00', TRUE, NULL),
('JEUDI', '11:00:00', '22:00:00', TRUE, NULL),
('VENDREDI', '11:00:00', '23:00:00', TRUE, NULL),
('SAMEDI', '12:00:00', '23:00:00', TRUE, NULL),
('DIMANCHE', '12:00:00', '22:00:00', TRUE, NULL);

-- --------------------------------------------------------------------------
-- Commandes de démonstration
-- --------------------------------------------------------------------------
INSERT INTO commandes (
    numero_commande, client_id, promotion_id, date_commande, type_commande, statut,
    sous_total, tva_montant, frais_livraison, reduction_montant,
    montant_total, mode_paiement, statut_paiement, notes_client
) VALUES
('CMD-2026-00001',
 (SELECT id FROM clients WHERE email = 'marie.dupont@example.com'),
 NULL, CURRENT_TIMESTAMP, 'A_EMPORTER', 'EN_PREPARATION',
 14.50, 0.87, 0.00, 0.00, 15.37, 'EN_ATTENTE', 'NON_PAYEE',
 'Commande à préparer rapidement'),
('CMD-2026-00002',
 (SELECT id FROM clients WHERE email = 'lucas.martin@example.com'),
 NULL, CURRENT_TIMESTAMP, 'SUR_PLACE', 'TERMINEE',
 16.50, 0.99, 0.00, 0.00, 17.49, 'CARTE', 'PAYEE', NULL),
('CMD-2026-00003',
 (SELECT id FROM clients WHERE email = 'sophie.peeters@example.com'),
 (SELECT id FROM promotions WHERE code = 'BIENVENUE10'),
 CURRENT_TIMESTAMP, 'LIVRAISON', 'LIVREE',
 28.00, 1.51, 3.00, 2.80, 29.71, 'CARTE', 'PAYEE',
 'Sonner à la porte');

-- Ligne 1 : Margherita moyenne avec fromage supplémentaire.
INSERT INTO details_commandes (
    commande_id, produit_id, variante_id, quantite,
    prix_unitaire, prix_total, nom_variante, cout_supplements
) VALUES
((SELECT id FROM commandes WHERE numero_commande = 'CMD-2026-00001'),
 (SELECT id FROM produits WHERE nom = 'Margherita'),
 (SELECT vp.id
    FROM variantes_produits vp
    INNER JOIN produits p ON p.id = vp.produit_id
   WHERE p.nom = 'Margherita' AND vp.nom = 'MOYENNE'),
 1, 10.50, 10.50, 'MOYENNE', 1.50);

INSERT INTO details_supplements (
    detail_commande_id, supplement_id, quantite, prix_unitaire
) VALUES
((SELECT dc.id
    FROM details_commandes dc
    INNER JOIN commandes c ON c.id = dc.commande_id
   WHERE c.numero_commande = 'CMD-2026-00001'
     AND dc.produit_id = (SELECT id FROM produits WHERE nom = 'Margherita')),
 (SELECT id FROM supplements WHERE nom = 'Fromage supplémentaire'),
 1, 1.50);

-- Ligne 2 : Coca-Cola 33 cl.
INSERT INTO details_commandes (
    commande_id, produit_id, variante_id, quantite,
    prix_unitaire, prix_total, nom_variante, cout_supplements
) VALUES
((SELECT id FROM commandes WHERE numero_commande = 'CMD-2026-00001'),
 (SELECT id FROM produits WHERE nom = 'Coca-Cola'),
 (SELECT vp.id
    FROM variantes_produits vp
    INNER JOIN produits p ON p.id = vp.produit_id
   WHERE p.nom = 'Coca-Cola' AND vp.nom = '33 CL'),
 1, 2.50, 2.50, '33 CL', 0.00);

-- Commande 2 : lasagne et cannolo.
INSERT INTO details_commandes (
    commande_id, produit_id, quantite,
    prix_unitaire, prix_total, cout_supplements
) VALUES
((SELECT id FROM commandes WHERE numero_commande = 'CMD-2026-00002'),
 (SELECT id FROM produits WHERE nom = 'Lasagne maison'),
 1, 12.00, 12.00, 0.00),
((SELECT id FROM commandes WHERE numero_commande = 'CMD-2026-00002'),
 (SELECT id FROM produits WHERE nom = 'Cannolo siciliano'),
 1, 4.50, 4.50, 0.00);

-- Commande 3 : deux grandes Margherita avec viande supplémentaire.
INSERT INTO details_commandes (
    commande_id, produit_id, variante_id, quantite,
    prix_unitaire, prix_total, nom_variante, cout_supplements
) VALUES
((SELECT id FROM commandes WHERE numero_commande = 'CMD-2026-00003'),
 (SELECT id FROM produits WHERE nom = 'Margherita'),
 (SELECT vp.id
    FROM variantes_produits vp
    INNER JOIN produits p ON p.id = vp.produit_id
   WHERE p.nom = 'Margherita' AND vp.nom = 'GRANDE'),
 2, 13.00, 26.00, 'GRANDE', 2.00);

INSERT INTO details_supplements (
    detail_commande_id, supplement_id, quantite, prix_unitaire
) VALUES
((SELECT dc.id
    FROM details_commandes dc
    INNER JOIN commandes c ON c.id = dc.commande_id
   WHERE c.numero_commande = 'CMD-2026-00003'
     AND dc.produit_id = (SELECT id FROM produits WHERE nom = 'Margherita')),
 (SELECT id FROM supplements WHERE nom = 'Viande supplémentaire'),
 1, 2.00);

UPDATE promotions
SET nombre_utilisations_actuelles = 1
WHERE code = 'BIENVENUE10';

COMMIT;
