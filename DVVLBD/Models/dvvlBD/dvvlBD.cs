namespace DVVLBD.Models.dvvlBD
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class dvvlBD : DbContext
    {
        public dvvlBD()
            : base("name=dvvlBD")
        {
        }

        public virtual DbSet<DM_ChucDanh> DM_ChucDanh { get; set; }
        public virtual DbSet<DM_ChuyenMon> DM_ChuyenMon { get; set; }
        public virtual DbSet<DM_DanToc> DM_DanToc { get; set; }
        public virtual DbSet<DM_DiaChi> DM_DiaChi { get; set; }
        public virtual DbSet<DM_HocVan> DM_HocVan { get; set; }
        public virtual DbSet<DM_LoaiHinhDoanhNghiep> DM_LoaiHinhDoanhNghiep { get; set; }
        public virtual DbSet<DM_MucLuong> DM_MucLuong { get; set; }
        public virtual DbSet<DM_NganhDaoTao> DM_NganhDaoTao { get; set; }
        public virtual DbSet<DM_NganhKinhDoanh> DM_NganhKinhDoanh { get; set; }
        public virtual DbSet<DM_NghiepVu> DM_NghiepVu { get; set; }
        public virtual DbSet<DM_ThoiGianLamViec> DM_ThoiGianLamViec { get; set; }
        public virtual DbSet<DM_TinhTrangHoSo> DM_TinhTrangHoSo { get; set; }
        public virtual DbSet<DM_TonGiao> DM_TonGiao { get; set; }
        public virtual DbSet<DM_TrinhDo> DM_TrinhDo { get; set; }
        public virtual DbSet<DoanhNghiep> DoanhNghieps { get; set; }
        public virtual DbSet<DoanhNghiep_HS> DoanhNghiep_HS { get; set; }
        public virtual DbSet<GopY> GopYs { get; set; }
        public virtual DbSet<GuiHSChoDN> GuiHSChoDNs { get; set; }
        public virtual DbSet<GuiHSChoNTV> GuiHSChoNTVs { get; set; }
        public virtual DbSet<HSNTV_Luu> HSNTV_Luu { get; set; }
        public virtual DbSet<HSTD_Luu> HSTD_Luu { get; set; }
        public virtual DbSet<NguoiTimViec> NguoiTimViecs { get; set; }
        public virtual DbSet<NguoiTimViec_HS> NguoiTimViec_HS { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GopY>()
                .Property(e => e.SoDienThoai)
                .IsUnicode(false);

            modelBuilder.Entity<NguoiTimViec>()
                .Property(e => e.CMND)
                .IsUnicode(false);

            modelBuilder.Entity<NguoiTimViec>()
                .Property(e => e.DienThoai)
                .IsUnicode(false);

            modelBuilder.Entity<NguoiTimViec>()
                .Property(e => e.Email)
                .IsUnicode(false);

            modelBuilder.Configurations.Add(new NguoiTimViec.NguoiTimViecMapping());
            modelBuilder.Configurations.Add(new DoanhNghiep_HS.DoanhNghiep_HSMapping());
            modelBuilder.Configurations.Add(new DoanhNghiep.DoanhNghiepMapping());
        }
    }
}
