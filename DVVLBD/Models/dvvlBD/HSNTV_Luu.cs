namespace DVVLBD.Models.dvvlBD
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public class HSNTV_Luu
    {
        [Key]
        public int ID_HSNTVLuu { get; set; }

        public int ID_User { get; set; }

        public int ID_NTV { get; set; }

        public int ID_HSXinViec { get; set; }

        public string TenHoSo { get; set; }

        public DateTime? NgayTao { get; set; }
    }
}