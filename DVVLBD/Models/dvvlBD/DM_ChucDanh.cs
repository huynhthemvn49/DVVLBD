namespace DVVLBD.Models.dvvlBD
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class DM_ChucDanh
    {
        [Key]
        public int ID_ChucDanh { get; set; }

        [StringLength(2000)]
        public string TenChucDanh { get; set; }

        public string MoTa { get; set; }

        public virtual ICollection<NguoiTimViec_HS> NguoiTimViec_HS { get; set; }

        public virtual ICollection<DoanhNghiep_HS> DoanhNghiep_HS { get; set; }
    }
}
