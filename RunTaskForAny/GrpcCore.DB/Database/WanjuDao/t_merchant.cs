using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace GrpcCore.DB.Database.WanjuDao
{
    [Table(nameof(t_merchant))]
    public class t_merchant
    {
        public int Id { get; set; }
        public int? TradeId { get; set; }
        public int? RegionId { get; set; }
        public string Name { get; set; }
        public string Brief { get; set; }
        public string Alias { get; set; }
        public int? Status { get; set; }
        public string Tel1 { get; set; }
        public string Tel2 { get; set; }
        public int? HasPark { get; set; }
        public string ParkInfo { get; set; }
        public string BusinessLicense { get; set; }
        public int? PerCapitaConsumption { get; set; }
        public string Logo { get; set; }
        public string Poster { get; set; }
        public decimal? OverallScore { get; set; }
        public int? ChooseSite { get; set; }
        public decimal? Longitude { get; set; }
        public decimal? Latitude { get; set; }
        public string Address { get; set; }
        public string PlaneImage { get; set; }
        public decimal? PlatformDivide { get; set; }
        public string ContactsName { get; set; }
        public int? MustMinConsumption { get; set; }
        public string BusinessEndTime { get; set; }
        public string ConsumeTime { get; set; }
        public int? MemberDisabled { get; set; }
        public int? MerchantOwnPay { get; set; }
        public int? CooperationMode { get; set; }
        public decimal? BookingFee { get; set; }
        public int? WhetherFee { get; set; }
        public int? FeeType { get; set; }
        public int? NoBkAfterComsume { get; set; }
        public string Seotitle { get; set; }
        public string Seokeywords { get; set; }
        public string ContactsTel { get; set; }
        public string SceneQrCode { get; set; }
        public string Uuid { get; set; }
        public string LastTransCode { get; set; }

        public DateTime? CreateDate { get; set; }
        public DateTime LastModifyDate { get; set; }

    }
}
