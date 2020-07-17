using DVVLBD.Models.dvvlBD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DVVLBD.Controllers
{
    public class HomeController : Controller
    {
        dvvlBD dvvl = new dvvlBD();

        public ActionResult Index()
        {
            ViewBag.Message = "Trung tâm Dịch Vụ Việc Làm tỉnh Bình Dương";

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Về chúng tôi.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Hãy gửi gì đó cho chúng tôi !";
            return View();
        }

        public ActionResult LastestJob()
        {
            var model = dvvl.DoanhNghiep_HS.Where(th => th.TinhTrangHS == 2 && th.NgayHetHanHS != null && th.NgayHetHanHS > DateTime.Now).Take(5).OrderByDescending(th => th.NgayCapNhat).ToList();
            return PartialView("_TD_MoiNhat", model);
        }

        public ActionResult LastestCandidate()
        {
            var model = dvvl.NguoiTimViec_HS.Where(th => th.TinhTrangHS == 2 && th.NgayHSHetHan != null && th.NgayHSHetHan > DateTime.Now).Take(5).OrderByDescending(th => th.NgayCapNhat).ToList();

            return PartialView("_NTV_MoiNhat", model);
        }

        public ActionResult HighSalaryJob()
        {
            var model = dvvl.DoanhNghiep_HS.Where(th => th.ID_MucLuong >= 6 && th.TinhTrangHS == 2 && th.NgayHetHanHS != null && th.NgayHetHanHS > DateTime.Now).Take(5).OrderByDescending(th => th.NgayCapNhat).ToList();
            return PartialView("_TD_LuongCao", model);
        }

        public ActionResult TDTheoKV()
        {
            return PartialView("_TD_TheoKhuVuc");
        }

        public ActionResult LienHeGopY(GopY model_gopy, string customer_name, string email, string phone, string msg_content)
        {
            model_gopy.HoTen = customer_name;
            model_gopy.SoDienThoai = phone;
            model_gopy.Email = email;
            model_gopy.NoiDungGopY = msg_content;
            model_gopy.NgayTao = DateTime.Now;
            dvvl.GopYs.Add(model_gopy);
            int kt = dvvl.SaveChanges();
            if (kt > 0)
            {
                TempData["Data"] = "tc";
            }
            else
            {
                TempData["Data"] = "tb";
            }
            return View("Contact");
        }



    }
}
