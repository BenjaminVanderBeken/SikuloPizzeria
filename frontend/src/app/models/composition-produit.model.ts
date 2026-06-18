export interface CompositionProduit {
id: number;
produitId: number;
produitNom: string;
ingredientId: number;
ingredientNom: string;
quantite: number;
unite: string;
ordreAffichage: number;
}

export interface CreateCompositionProduitDto {
produitId: number;
ingredientId: number;
quantite: number;
unite: string;
ordreAffichage: number;
}
