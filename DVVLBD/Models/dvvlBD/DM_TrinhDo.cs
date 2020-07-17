namespace DVVLBD.Models.dvvlBD
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class DM_TrinhDo
    {
        [Key]
        public int ID_TrinhDo { get; set; }

        [StringLength(200)]
        public string TenTrinhDo { get; set; }

        public string MoTa { get; set; }

        public virtual ICollection<NguoiTimViec> NguoiTimViecs { get; set; }

        public virtual ICollection<DoanhNghiep_HS> DoanhNghiep_HS { get; set; }
    }
}
