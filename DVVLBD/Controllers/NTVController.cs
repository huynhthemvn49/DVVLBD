using DVVLBD.Models.dvvlBD;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DVVLBD.Controllers
{
    public class NTVController : Controller
    {
        dvvlBD dvvl = new dvvlBD();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult NTV_Main()
        {
            Session["DanhSachNTV"] = "";
            return View();
        }

        public ActionResult TimKiemNTV()
        {
            ViewBag.KhuVuc = dvvl.DM_DiaChi.Where(th => th.ID_TuyenTren == 9).ToList();
            ViewBag.NganhNghe = dvvl.DM_NganhDaoTao.Where(th => th.ID_Parent == 0).ToList();
            return PartialView();
        }

        public ActionResult DanhSachNTV()
        {
            if (Session["DanhSachNTV"] == "NganhNgheNTV")
            {
                int nnid = int.Parse(Session["NganhNgheNTVid"].ToString());
                Session["SLNTV"] = dvvl.NguoiTimViec_HS.Where(th => th.NguoiTimViec.DM_NganhDaoTao.ID_NganhDaoTao == nnid && th.NgayHSHetHan != null && th.NgayHSHetHan > DateTime.Now)
                                                .Count();
            }
            else if (Session["DanhSachNTV"] == "KhuVucNTV")
            {
                int kvid = int.Parse(Session["KhuVucNTVid"].ToString());
                Session["SLNTV"] = dvvl.NguoiTimViec_HS.Where(th => th.NguoiTimViec.ID_QuanHuyen == kvid && th.NgayHSHetHan != null && th.NgayHSHetHan > DateTime.Now)
                                                .Count();
            }
            else if (Session["DanhSachNTV"] == "ChuoiTimKiem")
            {
                string tk = Session["ChuoiTimKiem"].ToString();
                Session["SLNTV"] = dvvl.NguoiTimViec_HS.Where(th => th.TenHSXinViec.Contains(tk) || th.NguoiTimViec.HoTen.Contains(tk) && th.NgayHSHetHan != null && th.NgayHSHetHan > DateTime.Now)
                                                .Count();
            }
            else
            {
                Session["SLNTV"] = dvvl.NguoiTimViec_HS.Where(th => th.NgayHSHetHan != null && th.NgayHSHetHan > DateTime.Now).Count();
            }
            return PartialView();
        }
        [HttpGet]
        public ActionResult NTV_Default(int PageNo = 0, int pageSize = 5)
        {
            var model = dvvl.NguoiTimViec_HS.Where(th => th.NgayHSHetHan != null && th.NgayHSHetHan > DateTime.Now && th.TinhTrangHS == 2)
                                            .OrderByDescending(th => th.NgayCapNhat)
                                            .Skip(PageNo * pageSize)
                                            .Take(pageSize)
                                            .ToList();
            return PartialView(model);
        }

        private bool CheckFileType(string FileName)
        {
            string ext = Path.GetExtension(FileName);
            if (ext.Equals(".png") || ext.Equals(".jpg") || ext.Equals(".jpeg") || ext.Equals(".pdf"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public ActionResult TaoHoSoTimViec()
        {
            ViewBag.ID_ChucDanhMongMuon = new SelectList(dvvl.DM_ChucDanh.ToList(), "ID_ChucDanh", "TenChucDanh");
            ViewBag.ID_MucLuongMongMuon = new SelectList(dvvl.DM_MucLuong.ToList(), "ID_MucLuong", "TenMucLuong");
            ViewBag.ID_NganhNgheMM = new SelectList(dvvl.DM_NganhKinhDoanh.Where(th => th.ID_Parent == 0), "ID_NganhKinhDoanh", "TenNganhKinhDoanh");
            ViewBag.ID_LoaiHinhDNMM = new SelectList(dvvl.DM_LoaiHinhDoanhNghiep.ToList(), "ID_LoaiDoanhNghiep", "TenLoaiDoanhNghiep");
            ViewBag.ID_NoiLamViecMM = new SelectList(dvvl.DM_DiaChi.Where(th => th.ID_TuyenTren == 9), "ID_DiaChi", "TenDiaChi");
            ViewBag.ID_ThoiGianLamViecMM = new SelectList(dvvl.DM_ThoiGianLamViec.ToList(), "ID_TGLamViec", "TenTGLamViec");

            return PartialView();
        }
        [HttpPost]
        public ActionResult TaoHoSoTimViec(NguoiTimViec_HS model_ntv_hs)
        {
            int userid = int.Parse(Session["UsrID"].ToString());
            var ntvid = dvvl.NguoiTimViecs.Where(th => th.ID_User == userid).Single();

            var file = Request.Files["CV"];
            if (file.ContentLength > 0)
            {
                var ten = file.FileName;
                if (CheckFileType(ten))
                {
                    var ext = ten.Substring(ten.LastIndexOf('.'));
                    ten = Guid.NewGuid() + ext;
                    model_ntv_hs.CV = ten;
                    file.SaveAs(Server.MapPath("~/Images/CV/") + ten);
                }
                else
                {
                    model_ntv_hs.CV = "khongcocv.pdf";
                }
            }
            else
            {
                model_ntv_hs.CV = "khongcocv.pdf";
            }
            model_ntv_hs.ID_NTV = ntvid.ID_NTV;
            model_ntv_hs.TinhTrangHS = 1;
            model_ntv_hs.LuotXem = 0;
            model_ntv_hs.NgayTao = DateTime.Now;
            DateTime now = DateTime.Now;
            model_ntv_hs.NgayHSHetHan = now.AddMonths(2);
            model_ntv_hs.NgayCapNhat = DateTime.Now;
            model_ntv_hs.NguoiTao = userid;
            model_ntv_hs.NguoiCapNhat = userid;

            dvvl.NguoiTimViec_HS.Add(model_ntv_hs);
            dvvl.SaveChanges();
            return RedirectToAction("HoSoNTV", "Account");
        }

        public ActionResult NTV_Detail(int Id)
        {
            var model = dvvl.NguoiTimViec_HS.Find(Id);
            model.LuotXem++;
            ViewBag.HSkhac = dvvl.NguoiTimViec_HS.Where(th => th.ID_NTV == model.ID_NTV && th.NgayHSHetHan != null && th.NgayHSHetHan >= DateTime.Now).OrderByDescending(th => th.ID_HSXinViec).ToList();
            dvvl.Entry(model).State = EntityState.Modified;
            try
            {
                dvvl.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                TempData["message"] = "Lỗi: " + ex;
            }
            return PartialView(model);
        }

        public ActionResult DSNTVTheoNganhNghe(string id)
        {
            Session["DanhSachNTV"] = "NganhNgheNTV";
            Session["NganhNgheNTVid"] = id;
            return PartialView("NTV_Main");
        }
        public ActionResult KQNTVTheoNganhNghe(string Id, int pageNo = 0, int pageSize = 5)
        {
            int iid = int.Parse(Id.ToString());
            var model = dvvl.NguoiTimViec_HS.Where(th => th.NguoiTimViec.ChuyenNganh == iid && th.NgayHSHetHan != null && th.NgayHSHetHan > DateTime.Now)
                                            .OrderByDescending(th => th.NgayCapNhat)
                                            .Skip(pageNo * pageSize)
                                            .Take(pageSize)
                                            .ToList();
            return PartialView("NTV_Default", model);
        }

        public ActionResult DSNTVTheoKhuVuc(string id, int PageSize = 5)
        {
            Session["DanhSachNTV"] = "KhuVucNTV";
            Session["KhuVucNTVid"] = id;
            return PartialView("NTV_Main");
        }

        public ActionResult KQNTVTheoKhuVuc(string Id, int pageNo = 0, int pageSize = 5)
        {
            int iid = int.Parse(Id.ToString());
            var model = dvvl.NguoiTimViec_HS.Where(th => th.NguoiTimViec.ID_QuanHuyen == iid && th.NgayHSHetHan != null && th.NgayHSHetHan > DateTime.Now)
                                                .OrderByDescending(th => th.NgayCapNhat)
                                                .Skip(pageNo * pageSize)
                                                .Take(pageSize)
                                                .ToList();
            return PartialView("NTV_Default", model);
        }

        public ActionResult DSNTVTheoTuKhoa()
        {
            Session["DanhSachNTV"] = "ChuoiTimKiemNTV";
            Session["ChuoiTimKiemNTV"] = Request.Form["ChuoiTimKiemNTV"].ToString();
            Session["NganhNgheNTVid"] = "";
            Session["KhuVucNTVid"] = "";
            return PartialView("NTV_Main");
        }

        public ActionResult KQNTVTheoTuKhoa(string ChuoiTimKiemNTV, int pageNo = 0, int pageSize = 5)
        {
            var model = dvvl.NguoiTimViec_HS.Where(th => th.TenHSXinViec.Contains(ChuoiTimKiemNTV) || th.NguoiTimViec.HoTen.Contains(ChuoiTimKiemNTV) && th.NgayHSHetHan != null && th.NgayHSHetHan > DateTime.Now)
                                                .OrderByDescending(th => th.NgayCapNhat)
                                                .Skip(pageNo * pageSize)
                                                .Take(pageSize)
                                                .ToList();
            return PartialView("NTV_Default", model);
        }

        public ActionResult HSNTVLuu(int id, int ID_NTV, string TenHoSo)
        {
            HSNTV_Luu hs = new HSNTV_Luu();
            var uID = Session["UsrID"];
            int userid = int.Parse(uID.ToString());

            var model = dvvl.HSNTV_Luu.Where(th => th.ID_User == userid && th.ID_NTV == ID_NTV && th.ID_HSXinViec == id).ToList();
            if (model.Count() == 0)
            {
                hs.ID_User = int.Parse(uID.ToString());
                hs.ID_NTV = ID_NTV;
                hs.ID_HSXinViec = id;
                hs.TenHoSo = TenHoSo;
                hs.NgayTao = DateTime.Now;

                dvvl.HSNTV_Luu.Add(hs);
                int i = dvvl.SaveChanges();
                if (i > 0)
                    TempData["testmsg"] = " Luu Thanh Cong !!! ";
                else
                    TempData["testmsg"] = " Luu Loi !!! ";
                return RedirectToAction("NTV_Detail", "NTV", new { Id = id });
            }
            else
            {
                TempData["testmsg"] = " Ho So Da Co Trong Kho Cua Ban !!! ";
                return RedirectToAction("NTV_Detail", "NTV", new { Id = id });
            }
        }

        public ActionResult GuiHSChoNTV(int ID_HSXinViec, int ID_NTV, string TinNhan)
        {
            GuiHSChoNTV guihs = new GuiHSChoNTV();
            var uID = Session["UsrID"];
            int userid = int.Parse(uID.ToString());

            guihs.ID_UserDN = userid;
            guihs.ID_NTV = ID_NTV;
            guihs.ID_HSXinViec = ID_HSXinViec;
            guihs.TinNhanDN = TinNhan;
            guihs.DaXem = 0;
            guihs.TinhTrang = 2;
            guihs.NgayTao = DateTime.Now;

            dvvl.GuiHSChoNTVs.Add(guihs);
            int kt = dvvl.SaveChanges();
            if (kt > 0)
            {
                var mail = dvvl.Database.SqlQuery<string>("select Email from [DVVL].[dbo].[Users] where [ID_User]=" + userid);
                string email = mail.First();
                var tendn = dvvl.Database.SqlQuery<string>("select TenDoanhNghiep from [DVVL].[dbo].[DoanhNghiep] where [ID_User] = " + userid);
                string tendoanhnghiep = tendn.First();
                Common.SentMail.Send(email, "Trung tâm DVVL Bình Dương thông báo ", "Bạn vừa nhận được 1 tin nhắn từ doanh nghiệp tên <b>" + tendoanhnghiep + "</b> với nội dung '<i>" + TinNhan + "</i>'. Xem thông tin doanh nghiệp tại:" + "<br /><br />" +
                        "http://localhost:59954/TD/ThongTinChiTietDN/" + userid + "?ID_GuiHSChoNTV=" + guihs.ID_GuiHSChoNTV);
            }

            return RedirectToAction("NTV_Detail", "NTV", new { Id = ID_HSXinViec });
        }

        public ActionResult ThongTinChiTietNTV(int id, int ID_GuiHSChoDN)
        {
            var model_hsgui = dvvl.GuiHSChoDNs.Find(ID_GuiHSChoDN);
            model_hsgui.DaXem += 1;
            int id_dn = model_hsgui.ID_DoanhNghiep;
            dvvl.Entry(model_hsgui).State = EntityState.Modified;
            dvvl.SaveChanges();

            var dem = dvvl.GuiHSChoDNs.Where(th => th.ID_DoanhNghiep == id_dn && th.DaXem == 0 && th.TinhTrang == 2).ToList();

            if (dem.Count() == 0)
            {
                Session["TinNhan"] = "OLD";
                Session["Dem"] = dem.Count();
            }
            else
            {
                Session["TinNhan"] = "NEW";
                Session["Dem"] = dem.Count();
            }
            var idntv = dvvl.Database.SqlQuery<int>("select ID_NTV from [DVVL].[dbo].[NguoiTimViec] where ID_User = " + id);
            int id_ntv = idntv.First();
            var model = dvvl.NguoiTimViecs.Find(id_ntv);
            ViewBag.HSkhac = dvvl.NguoiTimViec_HS.Where(th => th.ID_NTV == id_ntv && th.NgayHSHetHan != null && th.NgayHSHetHan >= DateTime.Now).OrderByDescending(th => th.ID_HSXinViec).ToList();
            return PartialView(model);
        }
    }
}
