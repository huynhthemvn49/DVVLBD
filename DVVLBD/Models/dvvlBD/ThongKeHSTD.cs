namespace DVVLBD.Models.dvvlBD
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ThongKeHSTD
    {
        public int ID_HSTuyenDung { get; set; }

        [StringLength(300)]
        public string TieuDeHoSo { get; set; }

        public int? LuotLuu { get; set; }

        public int? LuotNhan { get; set; }
    }
}