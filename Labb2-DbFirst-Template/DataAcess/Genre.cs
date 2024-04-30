namespace Labb2_DbFirst_Template.DataAcess;

public partial class Genre
{
    public int GenreId { get; set; }

    public string? Genre1 { get; set; }

    public virtual ICollection<BokDetaljer> BokDetaljers { get; set; } = new List<BokDetaljer>();
}
