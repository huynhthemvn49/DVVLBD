namespace DVVLBD.Models.dvvlBD
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.ModelConfiguration;
    using System.Data.Entity.Spatial;

    [Table("NguoiTimViec")]
    public partial class NguoiTimViec
    {
        [Key]
        public int ID_NTV { get; set; }

        public int? ID_User { get; set; }

        [StringLength(200)]
        public string HoTen { get; set; }

        public string AnhDaiDien { get; set; }

        [Column(TypeName = "date")]
        public DateTime? NgaySinh { get; set; }

        [StringLength(500)]
        public string DiaChi { get; set; }

        public int? ID_TinhThanhPho { get; set; }

        public int? ID_QuanHuyen { get; set; }

        public int? ID_PhuongXa { get; set; }

        public int? GioiTinh { get; set; }

        public int? TonGiao { get; set; }

        public int? TinhTrangHonNhan { get; set; }

        [StringLength(50)]
        public string CMND { get; set; }

        public int? TrinhDoVanHoa { get; set; }

        public int? TrinhDoNgoaiNgu { get; set; }

        public int? TrinhDoTinHoc { get; set; }

        public int? TrinhDoChuyenMon { get; set; }

        public int? ChuyenNganh { get; set; }

        [StringLength(50)]
        public string DienThoai { get; set; }

        [StringLength(50)]
        public string Email { get; set; }

        public int? ID_DanToc { get; set; }

        public DateTime? NgayTao { get; set; }

        public DateTime? NgayCapNhat { get; set; }

        [ForeignKey("ID_QuanHuyen")]
        public virtual DM_DiaChi DM_DiaChi { get; set; }

        [ForeignKey("TrinhDoChuyenMon")]
        public virtual DM_TrinhDo DM_TrinhDo { get; set; }

        [ForeignKey("ChuyenNganh")]
        public virtual DM_NganhDaoTao DM_NganhDaoTao { get; set; }

        [ForeignKey("TrinhDoVanHoa")]
        public virtual DM_HocVan DM_HocVan { get; set; }

        [ForeignKey("TrinhDoNgoaiNgu")]
        public virtual DM_NghiepVu DM_NghiepVuNN { get; set; }

        [ForeignKey("TrinhDoTinHoc")]
        public virtual DM_NghiepVu DM_NghiepVuTH { get; set; }
        public class NguoiTimViecMapping : EntityTypeConfiguration<NguoiTimViec>
        {
            public NguoiTimViecMapping()
            {
                HasRequired(m => m.DM_NghiepVuNN).WithMany(m => m.NguoiTimViecs);
                HasRequired(m => m.DM_NghiepVuTH).WithMany(m => m.NguoiTimViecs_t);
            }
        }

        [ForeignKey("ID_DanToc")]
        public virtual DM_DanToc DM_DanToc { get; set; }

        [ForeignKey("TonGiao")]
        public virtual DM_TonGiao DM_TonGiao { get; set; }

        public virtual ICollection<GuiHSChoNTV> GuiHSChoNTV { get; set; }
    }
}
