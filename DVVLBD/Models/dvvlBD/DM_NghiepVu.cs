namespace DVVLBD.Models.dvvlBD
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class DM_NghiepVu
    {
        [Key]
        public int ID_NghiepVu { get; set; }

        [StringLength(200)]
        public string TenNghiepVu { get; set; }

        public string MoTa { get; set; }

        public int ID_Parent { get; set; }

        public virtual ICollection<NguoiTimViec> NguoiTimViecs { get; set; }

        public virtual ICollection<NguoiTimViec> NguoiTimViecs_t { get; set; }

        public virtual ICollection<DoanhNghiep_HS> DoanhNghiep_HS { get; set; }

        public virtual ICollection<DoanhNghiep_HS> DoanhNghiep_HS_t { get; set; }
    }
}
