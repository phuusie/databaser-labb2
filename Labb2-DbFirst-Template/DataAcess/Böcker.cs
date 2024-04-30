namespace Labb2_DbFirst_Template.DataAcess;

public partial class Böcker
{
    public string Isbn13 { get; set; } = null!;

    public string? Titel { get; set; }

    public string? Språk { get; set; }

    public decimal? Pris { get; set; }

    public DateOnly? Utgivningsdatum { get; set; }

    public virtual ICollection<BokDetaljer> BokDetaljers { get; set; } = new List<BokDetaljer>();

    public virtual ICollection<LagerSaldo> LagerSaldos { get; set; } = new List<LagerSaldo>();
}
