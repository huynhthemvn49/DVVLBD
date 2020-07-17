namespace DVVLBD.Models.dvvlBD
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.ModelConfiguration;
    using System.Data.Entity.Spatial;

    [Table("DoanhNghiep")]
    public partial class DoanhNghiep
    {
        [Key]
        public int ID_DoanhNghiep { get; set; }

        public int ID_User { get; set; }

        [StringLength(500)]
        public string TenDoanhNghiep { get; set; }

        [StringLength(500)]
        public string LoGo { get; set; }

        [StringLength(300)]
        public string Website { get; set; }

        public string GioiThieuChung { get; set; }

        [Column(TypeName = "date")]
        public DateTime? NgayThanhLap { get; set; }

        [StringLength(100)]
        public string NguoiLienHe { get; set; }

        [StringLength(100)]
        public string ChucVuNguoiLienHe { get; set; }

        [StringLength(50)]
        public string DienThoai { get; set; }

        [StringLength(50)]
        public string Fax { get; set; }

        [StringLength(50)]
        public string Email { get; set; }

        public int? ID_TinhThanhPho { get; set; }

        public int? ID_QuanHuyen { get; set; }

        public int? ID_PhuongXa { get; set; }

        [StringLength(500)]
        public string DiaChi { get; set; }

        public int? ID_LoaiHinhDoanhNghiep { get; set; }

        public int? ID_NganhKinhDoanh { get; set; }

        public int? ID_NgheKinhDoanh { get; set; }

        public DateTime? NgayTao { get; set; }

        public DateTime? NgayCapNhat { get; set; }

        [ForeignKey("ID_QuanHuyen")]
        public virtual DM_DiaChi DM_DiaChi { get; set; }

        [ForeignKey("ID_LoaiHinhDoanhNghiep")]
        public virtual DM_LoaiHinhDoanhNghiep DM_LoaiHinhDoanhNghiep { get; set; }

        [ForeignKey("ID_NganhKinhDoanh")]
        public virtual DM_NganhKinhDoanh DM_NganhKinhDoanh { get; set; }

        [ForeignKey("ID_NgheKinhDoanh")]
        public virtual DM_NganhKinhDoanh DM_NgheKinhDoanh { get; set; }

        public virtual ICollection<GuiHSChoDN> GuiHSChoDN { get; set; }

        public class DoanhNghiepMapping : EntityTypeConfiguration<DoanhNghiep>
        {
            public DoanhNghiepMapping()
            {
                HasRequired(m => m.DM_NganhKinhDoanh).WithMany(m => m.DoanhNghiep);
                HasRequired(m => m.DM_NgheKinhDoanh).WithMany(m => m.DoanhNghiep_t);
            }
        }
    }
}
