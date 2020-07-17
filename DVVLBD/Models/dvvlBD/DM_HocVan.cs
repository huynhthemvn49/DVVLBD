namespace DVVLBD.Models.dvvlBD
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DM_HocVan")]
    public partial class DM_HocVan
    {
        [Key]
        public int ID_HocVan { get; set; }

        [StringLength(50)]
        public string TenHocVan { get; set; }

        public string MoTa { get; set; }

        public virtual ICollection<NguoiTimViec> NguoiTimViec { get; set; }
    }
}
