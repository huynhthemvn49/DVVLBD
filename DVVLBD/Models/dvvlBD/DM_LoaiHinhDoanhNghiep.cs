namespace DVVLBD.Models.dvvlBD
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class DM_LoaiHinhDoanhNghiep
    {
        [Key]
        public int ID_LoaiDoanhNghiep { get; set; }

        [StringLength(200)]
        public string TenLoaiDoanhNghiep { get; set; }

        public string MoTa { get; set; }

        public virtual ICollection<NguoiTimViec_HS> NguoiTimViec_HS { get; set; }

        public virtual ICollection<DoanhNghiep> DoanhNghiep { get; set; }
    }
}
