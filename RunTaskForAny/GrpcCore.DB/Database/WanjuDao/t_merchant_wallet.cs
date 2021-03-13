using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace GrpcCore.DB.Database.WanjuDao
{
    [Table(nameof(t_merchant_wallet))]
    public class t_merchant_wallet
    {
        public int Id { get; set; }
        public int? MerchantId { get; set; }
        public decimal? Balance { get; set; }
        public decimal? GiveBalance { get; set; }
        public string PayPassword { get; set; }
        public decimal? TotalChargeAmount { get; set; }
        public decimal? TotalGiveAmount { get; set; }
        public string Uuid { get; set; }
        public string LastTransCode { get; set; }

        public DateTime? CreateDate { get; set; }
        public DateTime LastModifyDate { get; set; }


    }
}
