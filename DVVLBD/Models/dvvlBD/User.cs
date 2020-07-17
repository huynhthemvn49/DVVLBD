namespace DVVLBD.Models.dvvlBD
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class User
    {
        [Key]
        public int ID_User { get; set; }

        [StringLength(100)]
        public string UserName { get; set; }

        [StringLength(100)]
        public string PassWord { get; set; }

        [StringLength(100)]
        public string Email { get; set; }

        [StringLength(100)]
        public string TenHienThi { get; set; }

        public DateTime? NgayTao { get; set; }

        public int Quyen { get; set; }
    }
}
