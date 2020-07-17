namespace DVVLBD.Models.dvvlBD
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class DM_TinhTrangHoSo
    {
        [Key]
        public int ID_TinhTrangHS { get; set; }

        [StringLength(50)]
        public string TenTinhTrangHS { get; set; }

        public string MoTa { get; set; }
    }
}
