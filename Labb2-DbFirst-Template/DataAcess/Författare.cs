namespace Labb2_DbFirst_Template.DataAcess;

public partial class Författare
{
    public int Id { get; set; }

    public string? Förnamn { get; set; }

    public string? Efternamn { get; set; }

    public DateOnly? Födelsedatum { get; set; }

    public virtual ICollection<BokDetaljer> BokDetaljers { get; set; } = new List<BokDetaljer>();
}
