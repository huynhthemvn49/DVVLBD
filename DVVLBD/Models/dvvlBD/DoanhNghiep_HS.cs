namespace DVVLBD.Models.dvvlBD
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.ModelConfiguration;
    using System.Data.Entity.Spatial;

    public partial class DoanhNghiep_HS
    {
        [Key]
        public int ID_HSTuyenDung { get; set; }

        public int? ID_DoanhNghiep { get; set; }

        [StringLength(300)]
        public string TieuDeHoSo { get; set; }

        public int? SoLuongTuyen { get; set; }

        public DateTime? NgayNhanHS { get; set; }

        public DateTime? NgayHetHanHS { get; set; }

        public DateTime? NgayDuTuyen { get; set; }

        [StringLength(500)]
        public string NoiNopHS { get; set; }

        [StringLength(500)]
        public string NoiDuTuyen { get; set; }

        public int? ID_MucLuong { get; set; }

        public int? ID_ThoiGianLamViec { get; set; }

        public int? ID_ChucDanh { get; set; }

        public string GiayToYeuCau { get; set; }

        public string QuyenLoi { get; set; }

        public int? YeuCauChuyenNganh { get; set; }

        public int? YeuCauTrinhDo { get; set; }

        public int YeuCauNamKinhNghiem { get; set; }

        public int? YeuCauGioiTinh { get; set; }

        public int? YeuCauDoTuoiTu { get; set; }

        public int? YeuCauDoTuoiDen { get; set; }

        public int? YeuCauTH { get; set; }

        public int? YeuCauNN { get; set; }

        public string MoTa { get; set; }

        public string GhiChu { get; set; }

        public int? SoLuotXem { get; set; }

        public int? TinhTrangHS { get; set; }

        public int? ViecLamHapDan { get; set; }

        public DateTime? NgayTao { get; set; }

        public int? ID_NguoiTao { get; set; }

        public DateTime? NgayCapNhat { get; set; }

        public int? ID_NguoiCapNhat { get; set; }

        [ForeignKey("ID_DoanhNghiep")]
        public virtual DoanhNghiep DoanhNghiep { get; set; }

        [ForeignKey("ID_MucLuong")]
        public virtual DM_MucLuong DM_MucLuong { get; set; }

        [ForeignKey("YeuCauChuyenNganh")]
        public virtual DM_NganhDaoTao DM_NganhDaoTao { get; set; }

        [ForeignKey("ID_ChucDanh")]
        public virtual DM_ChucDanh DM_ChucDanh { get; set; }

        [ForeignKey("ID_ThoiGianLamViec")]
        public virtual DM_ThoiGianLamViec DM_ThoiGianLamViec { get; set; }

        [ForeignKey("YeuCauTrinhDo")]
        public virtual DM_TrinhDo DM_TrinhDo { get; set; }

        [ForeignKey("YeuCauNN")]
        public virtual DM_NghiepVu DM_NghiepVuNN { get; set; }

        [ForeignKey("YeuCauTH")]
        public virtual DM_NghiepVu DM_NghiepVuTH { get; set; }
        public class DoanhNghiep_HSMapping : EntityTypeConfiguration<DoanhNghiep_HS>
        {
            public DoanhNghiep_HSMapping()
            {
                HasRequired(m => m.DM_NghiepVuNN).WithMany(m => m.DoanhNghiep_HS);
                HasRequired(m => m.DM_NghiepVuTH).WithMany(m => m.DoanhNghiep_HS_t);
            }
        }
    }
}
