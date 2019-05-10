using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace TestAuthentification.Models
{
    public partial class A5dContext : DbContext
    {
        public A5dContext()
        {
        }

        public A5dContext(DbContextOptions<A5dContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Comments> Comments { get; set; }
        public virtual DbSet<EfmigrationsHistory> EfmigrationsHistory { get; set; }
        public virtual DbSet<Historymaintenance> Historymaintenance { get; set; }
        public virtual DbSet<Images> Images { get; set; }
        public virtual DbSet<Key> Key { get; set; }
        public virtual DbSet<Location> Location { get; set; }
        public virtual DbSet<Pole> Pole { get; set; }
        public virtual DbSet<Ride> Ride { get; set; }
        public virtual DbSet<RideUser> RideUser { get; set; }
        public virtual DbSet<Right> Right { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<Vehicle> Vehicle { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseMySql("server=mvinet.fr;port=3306;database=a5d;uid=a5d;password=pwtk@[gh$!7Z#&wX");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Comments>(entity =>
            {
                entity.HasKey(e => e.CommentId);

                entity.ToTable("COMMENTS");

                entity.HasIndex(e => e.CommentLocId)
                    .HasName("FK_COMMENT_LOC");

                entity.HasIndex(e => e.CommentUserId)
                    .HasName("FK_COMMENT_USER");

                entity.HasIndex(e => e.CommentVehId)
                    .HasName("FK_COMMENT_VEH");

                entity.Property(e => e.CommentId)
                    .HasColumnName("COMMENT_ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CommentDate)
                    .HasColumnName("COMMENT_DATE")
                    .HasColumnType("date");

                entity.Property(e => e.CommentLocId)
                    .HasColumnName("COMMENT_LOC_ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CommentText)
                    .IsRequired()
                    .HasColumnName("COMMENT_TEXT")
                    .HasColumnType("text");

                entity.Property(e => e.CommentUserId)
                    .HasColumnName("COMMENT_USER_ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CommentVehId)
                    .HasColumnName("COMMENT_VEH_ID")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.CommentLoc)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(d => d.CommentLocId)
                    .HasConstraintName("FK_COMMENT_LOC");

                entity.HasOne(d => d.CommentUser)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(d => d.CommentUserId)
                    .HasConstraintName("FK_COMMENT_USER");

                entity.HasOne(d => d.CommentVeh)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(d => d.CommentVehId)
                    .HasConstraintName("FK_COMMENT_VEH");
            });

            modelBuilder.Entity<EfmigrationsHistory>(entity =>
            {
                entity.HasKey(e => e.MigrationId);

                entity.ToTable("__EFMigrationsHistory");

                entity.Property(e => e.MigrationId).HasColumnType("varchar(95)");

                entity.Property(e => e.ProductVersion)
                    .IsRequired()
                    .HasColumnType("varchar(32)");
            });

            modelBuilder.Entity<Historymaintenance>(entity =>
            {
                entity.HasKey(e => e.HistId);

                entity.ToTable("HISTORYMAINTENANCE");

                entity.HasIndex(e => e.HistVehId)
                    .HasName("HIST_VEH_ID");

                entity.Property(e => e.HistId)
                    .HasColumnName("HIST_ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.HistCitygarage)
                    .IsRequired()
                    .HasColumnName("HIST_CITYGARAGE")
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.HistComment)
                    .IsRequired()
                    .HasColumnName("HIST_COMMENT")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.HistCpgarage)
                    .IsRequired()
                    .HasColumnName("HIST_CPGARAGE")
                    .HasColumnType("varchar(5)");

                entity.Property(e => e.HistDateendmaintenance)
                    .HasColumnName("HIST_DATEENDMAINTENANCE")
                    .HasColumnType("datetime");

                entity.Property(e => e.HistDatestartmaintenance)
                    .HasColumnName("HIST_DATESTARTMAINTENANCE")
                    .HasColumnType("datetime");

                entity.Property(e => e.HistReffacture)
                    .IsRequired()
                    .HasColumnName("HIST_REFFACTURE")
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.HistVehId)
                    .HasColumnName("HIST_VEH_ID")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.HistVeh)
                    .WithMany(p => p.Historymaintenance)
                    .HasForeignKey(d => d.HistVehId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("HISTORYMAINTENANCE_ibfk_1");
            });

            modelBuilder.Entity<Images>(entity =>
            {
                entity.HasKey(e => e.ImageId);

                entity.ToTable("IMAGES");

                entity.HasIndex(e => e.ImageVehId)
                    .HasName("IMAGE_VEH_ID");

                entity.Property(e => e.ImageId)
                    .HasColumnName("IMAGE_ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ImageUri)
                    .IsRequired()
                    .HasColumnName("IMAGE_URI")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.ImageVehId)
                    .HasColumnName("IMAGE_VEH_ID")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.ImageVeh)
                    .WithMany(p => p.Images)
                    .HasForeignKey(d => d.ImageVehId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_IMG_VEH_ID");
            });

            modelBuilder.Entity<Key>(entity =>
            {
                entity.ToTable("KEY");

                entity.HasIndex(e => e.KeyCarId)
                    .HasName("KEY_CAR_ID");

                entity.Property(e => e.KeyId)
                    .HasColumnName("KEY_ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.KeyAvailable)
                    .HasColumnName("KEY_AVAILABLE")
                    .HasColumnType("tinyint(1)");

                entity.Property(e => e.KeyCarId)
                    .HasColumnName("KEY_CAR_ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.KeyLocalisation)
                    .IsRequired()
                    .HasColumnName("KEY_LOCALISATION")
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.KeyStatus)
                    .IsRequired()
                    .HasColumnName("KEY_STATUS")
                    .HasColumnType("varchar(100)");

                entity.HasOne(d => d.KeyCar)
                    .WithMany(p => p.Key)
                    .HasForeignKey(d => d.KeyCarId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("KEY_ibfk_1");
            });

            modelBuilder.Entity<Location>(entity =>
            {
                entity.HasKey(e => e.LocId);

                entity.ToTable("LOCATION");

                entity.HasIndex(e => e.LocPoleIdend)
                    .HasName("FK_POLE_END_ID");

                entity.HasIndex(e => e.LocPoleIdstart)
                    .HasName("FK_POLE_START_ID");

                entity.HasIndex(e => e.LocUserId)
                    .HasName("FK_LOC_USER_ID");

                entity.HasIndex(e => e.LocVehId)
                    .HasName("FK_VEH_ID");

                entity.Property(e => e.LocId)
                    .HasColumnName("LOC_ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.LocDateendlocation)
                    .HasColumnName("LOC_DATEENDLOCATION")
                    .HasColumnType("datetime");

                entity.Property(e => e.LocDatestartlocation)
                    .HasColumnName("LOC_DATESTARTLOCATION")
                    .HasColumnType("datetime");

                entity.Property(e => e.LocPoleIdend)
                    .HasColumnName("LOC_POLE_IDEND")
                    .HasColumnType("int(11)");

                entity.Property(e => e.LocPoleIdstart)
                    .HasColumnName("LOC_POLE_IDSTART")
                    .HasColumnType("int(11)");

                entity.Property(e => e.LocState)
                    .HasColumnName("LOC_STATE")
                    .HasColumnType("tinyint(4)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.LocUserId)
                    .HasColumnName("LOC_USER_ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.LocVehId)
                    .HasColumnName("LOC_VEH_ID")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.LocPoleIdendNavigation)
                    .WithMany(p => p.LocationLocPoleIdendNavigation)
                    .HasForeignKey(d => d.LocPoleIdend)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_POLE_END_ID");

                entity.HasOne(d => d.LocPoleIdstartNavigation)
                    .WithMany(p => p.LocationLocPoleIdstartNavigation)
                    .HasForeignKey(d => d.LocPoleIdstart)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_POLE_START_ID");

                entity.HasOne(d => d.LocUser)
                    .WithMany(p => p.Location)
                    .HasForeignKey(d => d.LocUserId)
                    .HasConstraintName("FK_TEST");

                entity.HasOne(d => d.LocVeh)
                    .WithMany(p => p.Location)
                    .HasForeignKey(d => d.LocVehId)
                    .HasConstraintName("FK_VEH_ID");
            });

            modelBuilder.Entity<Pole>(entity =>
            {
                entity.ToTable("POLE");

                entity.Property(e => e.PoleId)
                    .HasColumnName("POLE_ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.PoleAddress)
                    .IsRequired()
                    .HasColumnName("POLE_ADDRESS")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.PoleCity)
                    .IsRequired()
                    .HasColumnName("POLE_CITY")
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.PoleCp)
                    .IsRequired()
                    .HasColumnName("POLE_CP")
                    .HasColumnType("varchar(5)");

                entity.Property(e => e.PoleName)
                    .IsRequired()
                    .HasColumnName("POLE_NAME")
                    .HasColumnType("varchar(100)");
            });

            modelBuilder.Entity<Ride>(entity =>
            {
                entity.ToTable("RIDE");

                entity.HasIndex(e => e.RideLocId)
                    .HasName("RIDE_LOC_ID");

                entity.HasIndex(e => e.RidePoleIdend)
                    .HasName("RIDE_POLE_IDEND");

                entity.HasIndex(e => e.RidePoleIdstart)
                    .HasName("RIDE_POLE_IDSTART");

                entity.Property(e => e.RideId)
                    .HasColumnName("RIDE_ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.RideHourstart)
                    .HasColumnName("RIDE_HOURSTART")
                    .HasColumnType("datetime");

                entity.Property(e => e.RideLocId)
                    .HasColumnName("RIDE_LOC_ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.RidePoleIdend)
                    .HasColumnName("RIDE_POLE_IDEND")
                    .HasColumnType("int(11)");

                entity.Property(e => e.RidePoleIdstart)
                    .HasColumnName("RIDE_POLE_IDSTART")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.RideLoc)
                    .WithMany(p => p.Ride)
                    .HasForeignKey(d => d.RideLocId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("RIDE_ibfk_1");

                entity.HasOne(d => d.RidePoleIdendNavigation)
                    .WithMany(p => p.RideRidePoleIdendNavigation)
                    .HasForeignKey(d => d.RidePoleIdend)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("RIDE_ibfk_3");

                entity.HasOne(d => d.RidePoleIdstartNavigation)
                    .WithMany(p => p.RideRidePoleIdstartNavigation)
                    .HasForeignKey(d => d.RidePoleIdstart)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("RIDE_ibfk_2");
            });

            modelBuilder.Entity<RideUser>(entity =>
            {
                entity.HasKey(e => new { e.RideId, e.UserId });

                entity.ToTable("RIDE_USER");

                entity.HasIndex(e => e.UserId)
                    .HasName("USER_ID");

                entity.Property(e => e.RideId)
                    .HasColumnName("RIDE_ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.UserId)
                    .HasColumnName("USER_ID")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.Ride)
                    .WithMany(p => p.RideUser)
                    .HasForeignKey(d => d.RideId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("RIDE_USER_ibfk_1");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.RideUser)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("RIDE_USER_ibfk_2");
            });

            modelBuilder.Entity<Right>(entity =>
            {
                entity.ToTable("RIGHT");

                entity.Property(e => e.RightId)
                    .HasColumnName("RIGHT_ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.RightLabel)
                    .IsRequired()
                    .HasColumnName("RIGHT_LABEL")
                    .HasColumnType("varchar(25)");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("USER");

                entity.HasIndex(e => e.UserEmail)
                    .HasName("USER_EMAIL")
                    .IsUnique();

                entity.HasIndex(e => e.UserPhone)
                    .HasName("USER_USER_TEL_uindex")
                    .IsUnique();

                entity.HasIndex(e => e.UserPoleId)
                    .HasName("USER_ibfk_2");

                entity.HasIndex(e => e.UserRightId)
                    .HasName("USER_RIGHT_ID");

                entity.Property(e => e.UserId)
                    .HasColumnName("USER_ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.UserEmail)
                    .IsRequired()
                    .HasColumnName("USER_EMAIL")
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.UserFirstname)
                    .HasColumnName("USER_FIRSTNAME")
                    .HasColumnType("varchar(25)");

                entity.Property(e => e.UserName)
                    .HasColumnName("USER_NAME")
                    .HasColumnType("varchar(25)");

                entity.Property(e => e.UserNumpermis)
                    .HasColumnName("USER_NUMPERMIS")
                    .HasColumnType("varchar(25)");

                entity.Property(e => e.UserPassword)
                    .IsRequired()
                    .HasColumnName("USER_PASSWORD")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.UserPhone)
                    .HasColumnName("USER_PHONE")
                    .HasColumnType("varchar(10)");

                entity.Property(e => e.UserPoleId)
                    .HasColumnName("USER_POLE_ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.UserRightId)
                    .HasColumnName("USER_RIGHT_ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.UserState)
                    .HasColumnName("USER_STATE")
                    .HasColumnType("tinyint(2)")
                    .HasDefaultValueSql("'1'");

                entity.HasOne(d => d.UserPole)
                    .WithMany(p => p.User)
                    .HasForeignKey(d => d.UserPoleId)
                    .HasConstraintName("USER_ibfk_2");

                entity.HasOne(d => d.UserRight)
                    .WithMany(p => p.User)
                    .HasForeignKey(d => d.UserRightId)
                    .HasConstraintName("USER_ibfk_1");
            });

            modelBuilder.Entity<Vehicle>(entity =>
            {
                entity.HasKey(e => e.VehId);

                entity.ToTable("VEHICLE");

                entity.HasIndex(e => e.VehPoleId)
                    .HasName("FK_VEHICLE_POLE");

                entity.Property(e => e.VehId)
                    .HasColumnName("VEH_ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.VehBrand)
                    .IsRequired()
                    .HasColumnName("VEH_BRAND")
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.VehColor)
                    .IsRequired()
                    .HasColumnName("VEH_COLOR")
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.VehDatemec)
                    .HasColumnName("VEH_DATEMEC")
                    .HasColumnType("datetime");

                entity.Property(e => e.VehIsactive)
                    .HasColumnName("VEH_ISACTIVE")
                    .HasColumnType("bit(1)");

                entity.Property(e => e.VehKm).HasColumnName("VEH_KM");

                entity.Property(e => e.VehModel)
                    .IsRequired()
                    .HasColumnName("VEH_MODEL")
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.VehNumberplace)
                    .HasColumnName("VEH_NUMBERPLACE")
                    .HasColumnType("int(11)");

                entity.Property(e => e.VehPoleId)
                    .HasColumnName("VEH_POLE_ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.VehRegistration)
                    .IsRequired()
                    .HasColumnName("VEH_REGISTRATION")
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.VehState)
                    .HasColumnName("VEH_STATE")
                    .HasColumnType("tinyint(4)");

                entity.Property(e => e.VehTypeEssence)
                    .IsRequired()
                    .HasColumnName("VEH_TYPE_ESSENCE")
                    .HasColumnType("varchar(100)");

                entity.HasOne(d => d.VehPole)
                    .WithMany(p => p.Vehicle)
                    .HasForeignKey(d => d.VehPoleId)
                    .HasConstraintName("FK_VEHICLE_POLE");
            });
        }
    }
}
