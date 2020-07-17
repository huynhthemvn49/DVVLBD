namespace DVVLBD.Models.dvvlBD
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class NguoiTimViec_HS
    {
        [Key]
        public int ID_HSXinViec { get; set; }

        public int? ID_NTV { get; set; }

        [StringLength(300)]
        public string TenHSXinViec { get; set; }

        public int? ID_ChucDanhMongMuon { get; set; }

        public int? ID_MucLuongMongMuon { get; set; }

        [Column(TypeName = "date")]
        public DateTime? NgayBatDau { get; set; }

        public string MoTa { get; set; }

        public int? TinhTrangHS { get; set; }

        public int? LuotXem { get; set; }

        public int? ID_NganhNgheMM { get; set; }

        public int? ID_LoaiHinhDNMM { get; set; }

        public int? ID_NoiLamViecMM { get; set; }

        public int? ID_ThoiGianLamViecMM { get; set; }

        public int? KinhNghiem { get; set; }

        public DateTime? NgayHSHetHan { get; set; }

        public DateTime? NgayTao { get; set; }

        public int? NguoiTao { get; set; }

        public DateTime? NgayCapNhat { get; set; }

        public int? NguoiCapNhat { get; set; }

        public string CV { get; set; }

        [ForeignKey("ID_NTV")]
        public virtual NguoiTimViec NguoiTimViec { get; set; }

        [ForeignKey("ID_NganhNgheMM")]
        public virtual DM_NganhDaoTao DM_NganhDaoTao { get; set; }

        [ForeignKey("ID_ChucDanhMongMuon")]
        public virtual DM_ChucDanh DM_ChucDanh { get; set; }

        [ForeignKey("ID_NganhNgheMM")]
        public virtual DM_NganhKinhDoanh DM_NganhKinhDoanh { get; set; }

        [ForeignKey("ID_MucLuongMongMuon")]
        public virtual DM_MucLuong DM_MucLuong { get; set; }

        [ForeignKey("ID_ThoiGianLamViecMM")]
        public virtual DM_ThoiGianLamViec DM_ThoiGianLamViec { get; set; }

        [ForeignKey("ID_LoaiHinhDNMM")]
        public virtual DM_LoaiHinhDoanhNghiep DM_LoaiHinhDoanhNghiep { get; set; }
    }
}
