using Microsoft.EntityFrameworkCore;

namespace Labb2_DbFirst_Template.DataAcess;

public partial class BokhandelContext : DbContext
{
    public BokhandelContext()
    {
    }

    public BokhandelContext(DbContextOptions<BokhandelContext> options)
        : base(options)
    {
    }

    public virtual DbSet<BokDetaljer> BokDetaljers { get; set; }

    public virtual DbSet<Butiker> Butikers { get; set; }

    public virtual DbSet<Böcker> Böckers { get; set; }

    public virtual DbSet<Författare> Författares { get; set; }

    public virtual DbSet<Förlag> Förlags { get; set; }

    public virtual DbSet<Genre> Genres { get; set; }

    public virtual DbSet<LagerSaldo> LagerSaldos { get; set; }

    public virtual DbSet<TitlarPerFörfattare> TitlarPerFörfattares { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=PHU;Initial Catalog=Bokhandel;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BokDetaljer>(entity =>
        {
            entity.HasKey(e => new { e.Isbn, e.FörfattareId, e.GenreId }).HasName("PK__BokDetal__037AA2A333B8E0C8");

            entity.ToTable("BokDetaljer", "Bok");

            entity.Property(e => e.Isbn)
                .HasMaxLength(13)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("ISBN");
            entity.Property(e => e.FörfattareId).HasColumnName("FörfattareID");
            entity.Property(e => e.GenreId).HasColumnName("GenreID");
            entity.Property(e => e.FörlagId).HasColumnName("FörlagID");

            entity.HasOne(d => d.Författare).WithMany(p => p.BokDetaljers)
                .HasForeignKey(d => d.FörfattareId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BokDetalj__Förfa__52593CB8");

            entity.HasOne(d => d.Förlag).WithMany(p => p.BokDetaljers)
                .HasForeignKey(d => d.FörlagId)
                .HasConstraintName("FK__BokDetalj__Förla__534D60F1");

            entity.HasOne(d => d.Genre).WithMany(p => p.BokDetaljers)
                .HasForeignKey(d => d.GenreId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BokDetalj__Genre__5441852A");

            entity.HasOne(d => d.IsbnNavigation).WithMany(p => p.BokDetaljers)
                .HasForeignKey(d => d.Isbn)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BokDetalje__ISBN__5165187F");
        });

        modelBuilder.Entity<Butiker>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Butiker__3214EC2718D2EF94");

            entity.ToTable("Butiker", "Butik");

            entity.Property(e => e.Id)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasDefaultValueSql("('BT'+format(NEXT VALUE FOR [Butik].[ButikerSequence],'0000'))")
                .HasColumnName("ID");
            entity.Property(e => e.Adress)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Butiksnamn)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Land)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Postadress)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Stad)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Böcker>(entity =>
        {
            entity.HasKey(e => e.Isbn13).HasName("PK__Böcker__3BF79E03185C9BED");

            entity.ToTable("Böcker", "Bok");

            entity.Property(e => e.Isbn13)
                .HasMaxLength(13)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("ISBN13");
            entity.Property(e => e.Pris).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Språk)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Titel)
                .HasMaxLength(200)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Författare>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Författa__3214EC07B064339B");

            entity.ToTable("Författare", "Bok");

            entity.Property(e => e.Efternamn)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Förnamn)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Förlag>(entity =>
        {
            entity.HasKey(e => e.FörlagId).HasName("PK__Förlag__DE6A852CEEA396C2");

            entity.ToTable("Förlag", "Bok");

            entity.HasIndex(e => e.Namn, "idx_Unique_FörlagNamn").IsUnique();

            entity.Property(e => e.FörlagId).HasColumnName("FörlagID");
            entity.Property(e => e.Land)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Namn)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Stad)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Webbadress)
                .HasMaxLength(150)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Genre>(entity =>
        {
            entity.HasKey(e => e.GenreId).HasName("PK__Genre__0385055EEFAA5A06");

            entity.ToTable("Genre", "Bok");

            entity.Property(e => e.GenreId).HasColumnName("GenreID");
            entity.Property(e => e.Genre1)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Genre");
        });

        modelBuilder.Entity<LagerSaldo>(entity =>
        {
            entity.HasKey(e => new { e.ButikId, e.Isbn }).HasName("PK__LagerSal__1191B89418615704");

            entity.ToTable("LagerSaldo", "Butik");

            entity.Property(e => e.ButikId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("ButikID");
            entity.Property(e => e.Isbn)
                .HasMaxLength(13)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("ISBN");

            entity.HasOne(d => d.Butik).WithMany(p => p.LagerSaldos)
                .HasForeignKey(d => d.ButikId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LagerSald__Butik__68487DD7");

            entity.HasOne(d => d.IsbnNavigation).WithMany(p => p.LagerSaldos)
                .HasForeignKey(d => d.Isbn)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LagerSaldo__ISBN__693CA210");
        });

        modelBuilder.Entity<TitlarPerFörfattare>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("TitlarPerFörfattare");

            entity.Property(e => e.Lagervärde).HasColumnType("decimal(38, 2)");
            entity.Property(e => e.Namn)
                .HasMaxLength(101)
                .IsUnicode(false);
        });
        modelBuilder.HasSequence("ButikerSequence", "Butik");

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
