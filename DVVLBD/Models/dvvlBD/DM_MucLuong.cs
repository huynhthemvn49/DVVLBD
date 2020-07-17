namespace DVVLBD.Models.dvvlBD
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class DM_MucLuong
    {
        [Key]
        public int ID_MucLuong { get; set; }

        [StringLength(100)]
        public string TenMucLuong { get; set; }

        public string MoTa { get; set; }

        public virtual ICollection<DoanhNghiep_HS> DoanhNghiep_HS { get; set; }

        public virtual ICollection<NguoiTimViec_HS> NguoiTimViec_HS { get; set; }
    }
}
