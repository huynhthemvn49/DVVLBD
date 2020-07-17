namespace DVVLBD.Models.dvvlBD
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class DM_DiaChi
    {
        [Key]
        public int ID_DiaChi { get; set; }

        [StringLength(100)]
        public string TenDiaChi { get; set; }

        public string MoTa { get; set; }

        public int ID_TuyenTren { get; set; }

        public virtual ICollection<DoanhNghiep> DoanhNghieps { get; set; }

        public virtual ICollection<NguoiTimViec> NguoiTimViecs { get; set; }
    }
}
