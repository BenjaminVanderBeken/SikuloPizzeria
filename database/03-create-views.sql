-- ============================================================================
-- SIKULO PIZZERIA - 03 - CREATION DES VUES
-- ============================================================================

USE sikulo_pizzeria;

DROP VIEW IF EXISTS v_clients_statistiques;
DROP VIEW IF EXISTS v_produits_populaires;
DROP VIEW IF EXISTS v_ventes_jour;
DROP VIEW IF EXISTS v_commandes_actives;

-- Commandes qui doivent encore être traitées par l'équipe.
CREATE VIEW v_commandes_actives AS
SELECT
    c.id,
    c.numero_commande,
    c.date_commande,
    c.type_commande,
    c.statut,
    c.statut_paiement,
    c.montant_total,
    c.client_id,
    TRIM(CONCAT_WS(' ', cl.prenom, cl.nom)) AS nom_client,
    p.code AS code_promotion
FROM commandes AS c
LEFT JOIN clients AS cl ON cl.id = c.client_id
LEFT JOIN promotions AS p ON p.id = c.promotion_id
WHERE c.statut IN (
    'EN_ATTENTE', 'CONFIRMEE', 'EN_PREPARATION', 'PRETE'
);

-- Résumé des ventes du jour. Les commandes annulées ne génèrent pas de vente.
CREATE VIEW v_ventes_jour AS
SELECT
    DATE(c.date_commande) AS date_vente,
    COUNT(CASE WHEN c.statut <> 'ANNULEE' THEN 1 END) AS nombre_commandes,
    COALESCE(SUM(
        CASE WHEN c.statut <> 'ANNULEE' THEN c.montant_total ELSE 0 END
    ), 0) AS montant_total_ventes,
    COALESCE(SUM(
        CASE WHEN c.statut <> 'ANNULEE' THEN c.tva_montant ELSE 0 END
    ), 0) AS tva_totale,
    COUNT(CASE WHEN c.statut IN ('LIVREE', 'TERMINEE') THEN 1 END)
        AS commandes_terminees
FROM commandes AS c
WHERE DATE(c.date_commande) = CURDATE()
GROUP BY DATE(c.date_commande);

-- Classement des produits selon les ventes non annulées.
CREATE VIEW v_produits_populaires AS
SELECT
    p.id,
    p.nom,
    cat.nom AS categorie,
    COUNT(CASE WHEN c.id IS NOT NULL THEN dc.id END) AS nombre_lignes_vente,
    COALESCE(SUM(
        CASE WHEN c.id IS NOT NULL THEN dc.quantite ELSE 0 END
    ), 0) AS quantite_totale,
    COALESCE(AVG(
        CASE WHEN c.id IS NOT NULL THEN dc.prix_unitaire END
    ), 0) AS prix_moyen
FROM produits AS p
INNER JOIN categories AS cat ON cat.id = p.categorie_id
LEFT JOIN details_commandes AS dc ON dc.produit_id = p.id
LEFT JOIN commandes AS c
    ON c.id = dc.commande_id
   AND c.statut <> 'ANNULEE'
WHERE p.actif = TRUE
GROUP BY p.id, p.nom, cat.nom;

-- Statistiques client calculées à partir des commandes, sans duplication dans
-- la table clients.
CREATE VIEW v_clients_statistiques AS
SELECT
    cl.id,
    cl.nom,
    cl.prenom,
    cl.email,
    cl.telephone,
    cl.client_vip,
    COUNT(CASE WHEN c.statut <> 'ANNULEE' THEN 1 END) AS nombre_commandes,
    COALESCE(SUM(
        CASE WHEN c.statut <> 'ANNULEE' THEN c.montant_total ELSE 0 END
    ), 0) AS montant_total_achete,
    MIN(CASE WHEN c.statut <> 'ANNULEE' THEN DATE(c.date_commande) END)
        AS date_premiere_commande,
    MAX(CASE WHEN c.statut <> 'ANNULEE' THEN DATE(c.date_commande) END)
        AS date_derniere_commande
FROM clients AS cl
LEFT JOIN commandes AS c ON c.client_id = cl.id
GROUP BY
    cl.id,
    cl.nom,
    cl.prenom,
    cl.email,
    cl.telephone,
    cl.client_vip;
