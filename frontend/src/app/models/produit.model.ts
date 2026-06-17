export interface VarianteProduit {
  id: number;
  produitId: number;
  nom: string;
  prix: number;
  actif: boolean;
  ordreAffichage: number;
}

export interface VarianteProduitRequest {
  id?: number | null;
  nom: string;
  prix: number;
  actif: boolean;
  ordreAffichage: number;
}

export interface Produit {
  id: number;
  categorieId: number;
  categorieNom: string;
  nom: string;
  description: string | null;
  prixBase: number;
  prixPromotion: number | null;
  imageUrl: string | null;
  permetSupplement: boolean;
  actif: boolean;
  populaire: boolean;
  ordreAffichage: number;
  variantes: VarianteProduit[];
}

export interface CreateProduitDto {
  categorieId: number;
  nom: string;
  description: string | null;
  prixBase: number;
  prixPromotion: number | null;
  imageUrl: string | null;
  permetSupplement: boolean;
  populaire: boolean;
  ordreAffichage: number;
  variantes: VarianteProduitRequest[];
}

export interface UpdateProduitDto extends CreateProduitDto {
  actif: boolean;
}

