export interface Categorie {
  id: number;
  nom: string;
  description: string | null;
  ordreAffichage: number;
  actif: boolean;
}

export interface CreateCategorieRequest {
  nom: string;
  description: string | null;
  ordreAffichage: number;
}

export interface UpdateCategorieRequest {
  nom: string;
  description: string | null;
  ordreAffichage: number;
  actif: boolean;
}