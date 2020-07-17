namespace DVVLBD.Models.dvvlBD
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("GuiHSChoNTV")]
    public class GuiHSChoNTV
    {
        [Key]
        public int ID_GuiHSChoNTV { get; set; }

        public int ID_UserDN { get; set; }

        public int ID_NTV { get; set; }

        public int ID_HSXinViec { get; set; }

        public string TinNhanDN { get; set; }

        public int DaXem { get; set; }

        public int TinhTrang { get; set; }

        public DateTime? NgayTao { get; set; }

        [ForeignKey("ID_NTV")]
        public virtual NguoiTimViec NguoiTimViec { get; set; }
    }
}