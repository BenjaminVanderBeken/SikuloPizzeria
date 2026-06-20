export interface Categorie {
  id: number;
  nom: string;
  description: string | null;
  ordreAffichage: number;
  actif: boolean;
}

export interface CreateCategorieDto {
  nom: string;
  description: string | null;
  ordreAffichage: number;
}

export interface UpdateCategorieDto {
  nom: string;
  description: string | null;
  ordreAffichage: number;
  actif: boolean;
}