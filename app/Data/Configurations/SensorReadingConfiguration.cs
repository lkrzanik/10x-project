using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using _10xPV.Models;

namespace _10xPV.Data.Configurations;

public class SensorReadingConfiguration : IEntityTypeConfiguration<SensorReading>
{
    public void Configure(EntityTypeBuilder<SensorReading> builder)
    {
        builder.ToTable("SensorReadings");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever();

        builder.Property(e => e.Timestamp).IsRequired();
        builder.HasIndex(e => e.Timestamp).IsUnique().HasDatabaseName("IX_SensorReadings_Timestamp");

        builder.Property(e => e.Temperature).HasColumnType("float");
        builder.Property(e => e.Humidity).HasColumnType("float");
    }
}