using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace GrpcCore.DB.Database.WanjuDao
{
    [Table(nameof(t_employee))]
    public class t_employee
    {
        public int Id { get; set; }
        public int? MerchantId { get; set; }
        public string Uuid { get; set; }
        public string LoginId { get; set; }
        public string Name { get; set; }
        public string EmployeeNameAcronym { get; set; }
        public string MobileNo { get; set; }
        public string Password { get; set; }
        public int? Gender { get; set; }
        public string Introduction { get; set; }
        public int? IsSeller { get; set; }
        public int? IsBoss { get; set; }
        public int? Status { get; set; }
        public string JobNo { get; set; }
        public string HeadImage { get; set; }
        public string WeChatOpenId { get; set; }
        public string QrcodeUrl { get; set; }
        public decimal? Rating { get; set; }
        public int? AllowLoginBackstage { get; set; }
        public int? IsPreinstall { get; set; }
        public int? IsPartTime { get; set; }
        public string Imuuid { get; set; }
        public string JpGuid { get; set; }
        public int? GiveRank { get; set; }
        public int? BkMerchantPayFee { get; set; }
        public int? OnlineBookingDefault { get; set; }
        public decimal? GiveAmountLimit { get; set; }
        public int? CardVersion { get; set; }
        public string LastTransCode { get; set; }
        public decimal? Disaccount { get; set; }
        public int? IsSystem { get; set; }
        public decimal? UsedGiveAmount { get; set; }
        public int? GiveRatio { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime LastModifyDate { get; set; }
    }
}
