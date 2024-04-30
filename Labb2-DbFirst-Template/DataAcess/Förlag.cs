namespace Labb2_DbFirst_Template.DataAcess;

public partial class Förlag
{
    public int FörlagId { get; set; }

    public string? Namn { get; set; }

    public string? Webbadress { get; set; }

    public string? Stad { get; set; }

    public string? Land { get; set; }

    public virtual ICollection<BokDetaljer> BokDetaljers { get; set; } = new List<BokDetaljer>();
}
