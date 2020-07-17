namespace DVVLBD.Models.dvvlBD
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class GopY
    {
        [Key]
        public int ID_GopY { get; set; }

        [StringLength(100)]
        public string HoTen { get; set; }

        [StringLength(50)]
        public string SoDienThoai { get; set; }

        [StringLength(100)]
        public string Email { get; set; }

        public string NoiDungGopY { get; set; }

        public System.DateTime NgayTao { get; set; }
    }
}
