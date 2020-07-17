namespace DVVLBD.Models.dvvlBD
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class DM_ChuyenMon
    {
        [Key]
        public int ID_ChuyenMon { get; set; }

        [StringLength(300)]
        public string TenChuyenMon { get; set; }

        public string MoTa { get; set; }

        public int? ID_Parent { get; set; }
    }
}
