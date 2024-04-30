namespace Labb2_DbFirst_Template.DataAcess;

public partial class TitlarPerFörfattare
{
    public string Namn { get; set; } = null!;

    public int? Ålder { get; set; }

    public int? Titlar { get; set; }

    public decimal? Lagervärde { get; set; }
}
