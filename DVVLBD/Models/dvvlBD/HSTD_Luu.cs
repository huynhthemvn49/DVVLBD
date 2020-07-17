using System;
namespace DVVLBD.Models.dvvlBD
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public class HSTD_Luu
    {
        [Key]
        public int ID_HSTDLuu { get; set; }

        public int ID_User { get; set; }

        public int ID_DoanhNghiep { get; set; }

        public int ID_HSTuyenDung { get; set; }

        public string TenHoSo { get; set; }

        public DateTime? NgayTao { get; set; }
    }
}