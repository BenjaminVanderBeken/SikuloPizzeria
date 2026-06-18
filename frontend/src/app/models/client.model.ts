export interface Client {
id: number;
nom: string;
prenom: string | null;
email: string | null;
telephone: string | null;
adresseRue: string | null;
adresseNumero: string | null;
adresseBoite: string | null;
adresseCodePostal: string | null;
adresseVille: string | null;
adressePays: string;
notes: string | null;
clientVip: boolean;
actif: boolean;
dateCreation: string;
dateModification: string;
}

export interface CreateClientDto {
nom: string;
prenom: string | null;
email: string | null;
telephone: string | null;
adresseRue: string | null;
adresseNumero: string | null;
adresseBoite: string | null;
adresseCodePostal: string | null;
adresseVille: string | null;
adressePays: string;
notes: string | null;
clientVip: boolean;
}

export interface UpdateClientDto extends CreateClientDto {
actif: boolean;
}
//frontend 1