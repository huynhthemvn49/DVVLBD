namespace DVVLBD.Models.dvvlBD
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class DM_NganhKinhDoanh
    {
        [Key]
        public int ID_NganhKinhDoanh { get; set; }

        [StringLength(200)]
        public string TenNganhKinhDoanh { get; set; }

        public string MoTa { get; set; }

        public int? ID_Parent { get; set; }

        public virtual ICollection<NguoiTimViec_HS> NguoiTimViec_HS { get; set; }

        public virtual ICollection<DoanhNghiep> DoanhNghiep { get; set; }

        public virtual ICollection<DoanhNghiep> DoanhNghiep_t { get; set; }
    }
}
