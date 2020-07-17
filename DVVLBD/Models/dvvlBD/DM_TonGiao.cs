namespace DVVLBD.Models.dvvlBD
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;


    public partial class DM_TonGiao
    {
        [Key]
        public int ID_TonGiao { get; set; }

        [StringLength(100)]
        public string TenTonGiao { get; set; }

        public string MoTa { get; set; }

        public virtual ICollection<NguoiTimViec> NguoiTimViecs { get; set; }
    }
}