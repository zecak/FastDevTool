using GrpcCore.DB.Database.WanjuDao;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GrpcCore.DB.Database
{
    public class WanjuOfflineContext : DbContext
    {
        public virtual DbSet<t_merchant> Merchants { get; set; }
        public virtual DbSet<t_merchant_wallet> MerchantWallets { get; set; }
        public virtual DbSet<t_employee> Employees { get; set; }
        public virtual DbSet<t_employee_wallet> EmployeeWallets { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL(Helper.Setting.ConnectionString_WanjuOffline);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
