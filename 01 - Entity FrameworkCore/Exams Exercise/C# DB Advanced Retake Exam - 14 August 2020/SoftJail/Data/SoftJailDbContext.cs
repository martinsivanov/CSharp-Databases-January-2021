namespace SoftJail.Data
{
	using Microsoft.EntityFrameworkCore;
    using SoftJail.Data.Models;

    public class SoftJailDbContext : DbContext
	{
		public SoftJailDbContext()
		{
		}

		public SoftJailDbContext(DbContextOptions options)
			: base(options)
		{
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			if (!optionsBuilder.IsConfigured)
			{
				optionsBuilder
					.UseSqlServer(Configuration.ConnectionString);
			}
		}
		public DbSet<Cell> Cells { get; set; }
		public DbSet<Department> Departments { get; set; }
		public DbSet<Mail> Mails { get; set; }
		public DbSet<Officer> Officers { get; set; }
		public DbSet<OfficerPrisoner> OfficersPrisoners { get; set; }
		public DbSet<Prisoner> Prisoners { get; set; }

		protected override void OnModelCreating(ModelBuilder builder)
		{
			builder.Entity<OfficerPrisoner>(entity =>
			{
				entity.HasKey(op => new
				{
					op.OfficerId,
					op.PrisonerId
				});
				//entity.HasOne(x => x.Prisoner)
				//.WithMany(c => c.PrisonerOfficers)
				//.HasForeignKey(y => y.PrisonerId)
				//.OnDelete(DeleteBehavior.Restrict);
			});
		}
	}
}