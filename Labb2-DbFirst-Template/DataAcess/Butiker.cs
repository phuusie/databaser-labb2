namespace Labb2_DbFirst_Template.DataAcess;

public partial class Butiker
{
    public string Id { get; set; } = null!;

    public string? Butiksnamn { get; set; }

    public string? Adress { get; set; }

    public string? Postadress { get; set; }

    public string? Stad { get; set; }

    public string? Land { get; set; }

    public virtual ICollection<LagerSaldo> LagerSaldos { get; set; } = new List<LagerSaldo>();
}
