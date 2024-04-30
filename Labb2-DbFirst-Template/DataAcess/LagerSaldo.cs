namespace Labb2_DbFirst_Template.DataAcess;

public partial class LagerSaldo
{
    public string ButikId { get; set; } = null!;

    public string Isbn { get; set; } = null!;

    public int? Antal { get; set; }

    public virtual Butiker Butik { get; set; } = null!;

    public virtual Böcker IsbnNavigation { get; set; } = null!;
}
