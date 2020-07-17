namespace DVVLBD.Models.dvvlBD
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class DM_DanToc
    {
        [Key]
        public int ID_DanToc { get; set; }

        [StringLength(50)]
        public string TenDanToc { get; set; }

        public string MoTa { get; set; }

        public virtual ICollection<NguoiTimViec> NguoiTimViecs { get; set; }
    }
}
