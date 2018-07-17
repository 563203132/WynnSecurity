using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Linq;
using System.Text.RegularExpressions;

namespace WynnSecurity.DataAccess.Extensions
{
    public static class ModelBuilderExtension
    {
        public static void SetPluralizingTableNameConvention(this ModelBuilder modelBuilder)
        {
            foreach (IMutableEntityType entity in modelBuilder.Model.GetEntityTypes())
            {
                Regex underscoreRegex = new Regex(@"((?<=.)[A-Z][a-zA-Z]*)|((?<=[a-zA-Z])\d+)");
                entity.Relational().TableName = underscoreRegex.Replace(entity.DisplayName(), @"$1$2").ToLower();
            }
        }

        public static void SetOneToManyCascadeDeleteConvention(this ModelBuilder modelBuilder)
        {
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }
    }
}
