namespace DVVLBD.Models.dvvlBD
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ThongKeHSNTV
    {
        public int ID_HSXinViec { get; set; }

        [StringLength(300)]
        public string TenHSXinViec { get; set; }

        public int? LuotLuu { get; set; }

        public int? LuotNhan { get; set; }
    }
}