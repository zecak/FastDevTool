using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace GrpcCore.DB.Database.WanjuDao
{
    [Table(nameof(t_employee_wallet))]
    public class t_employee_wallet
    {
        public int Id { get; set; }
        public int? EmployeeId { get; set; }
        public decimal? Balance { get; set; }
        public decimal? GiveBalance { get; set; }
        public string PayPassword { get; set; }
        public decimal? TotalChargeAmount { get; set; }
        public decimal? TotalGiveAmount { get; set; }
        public int? AllowPayForAnthor { get; set; }
        public int? NonePasswordPay { get; set; }
        public string Uuid { get; set; }
        public string LastTransCode { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime LastModifyDate { get; set; }


        public virtual t_employee Employee { get; set; }
    }
}
