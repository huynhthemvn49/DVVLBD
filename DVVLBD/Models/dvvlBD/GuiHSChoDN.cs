namespace DVVLBD.Models.dvvlBD
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("GuiHSChoDN")]
    public class GuiHSChoDN
    {
        [Key]
        public int ID_GuiHSChoDN { get; set; }

        public int ID_UserNTV { get; set; }

        public int ID_DoanhNghiep { get; set; }

        public int ID_HSTuyenDung { get; set; }

        public string TinNhanNTV { get; set; }

        public int DaXem { get; set; }

        public int TinhTrang { get; set; }

        public DateTime? NgayTao { get; set; }

        [ForeignKey("ID_DoanhNghiep")]
        public virtual DoanhNghiep DoanhNghiep { get; set; }
    }
}