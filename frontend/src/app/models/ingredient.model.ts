export interface Ingredient {
  id: number;
  nom: string;
  type: string;
  stockActuel: number;
  uniteMesure: string;
  prixUnitaire: number | null;
  allergenes: string | null;
  actif: boolean;
}

export interface CreateIngredientDto {
  nom: string;
  type: string;
  stockActuel: number;
  uniteMesure: string;
  prixUnitaire: number | null;
  allergenes: string | null;
}

export interface UpdateIngredientDto extends CreateIngredientDto {
  actif: boolean;
}

export const TYPES_INGREDIENT = [
  'SAUCE',
  'VIANDE',
  'FROMAGE',
  'LEGUME',
  'ACCOMPAGNEMENT',
  'AUTRE',
] as const;