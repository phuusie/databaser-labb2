namespace Labb2_DbFirst_Template.DataAcess;

public partial class BokDetaljer
{
    public string Isbn { get; set; } = null!;

    public int FörfattareId { get; set; }

    public int? FörlagId { get; set; }

    public int GenreId { get; set; }

    public virtual Författare Författare { get; set; } = null!;

    public virtual Förlag? Förlag { get; set; }

    public virtual Genre Genre { get; set; } = null!;

    public virtual Böcker IsbnNavigation { get; set; } = null!;
}
