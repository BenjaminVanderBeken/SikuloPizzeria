namespace SikuloPizzeria.Core.DTOs;

public sealed class DashboardStatsDto
{
public int ProduitsActifs { get; set; }
public int ClientsActifs { get; set; }
public int CommandesTotales { get; set; }
public int CommandesDuJour { get; set; }
public int CommandesEnAttente { get; set; }
public decimal ChiffreAffairesPaye { get; set; }
public int IngredientsActifs { get; set; }
public int Compositions { get; set; }
}
