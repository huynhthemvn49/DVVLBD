using DVVLBD.Models.dvvlBD;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DVVLBD.Controllers
{
    public class TDController : Controller
    {
        dvvlBD dvvl = new dvvlBD();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult TD_Main()
        {
            Session["DanhSach"] = "";
            return View();
        }

        public ActionResult TimKiemTD()
        {
            ViewBag.KhuVuc = dvvl.DM_DiaChi.Where(th => th.ID_TuyenTren == 9).ToList();
            ViewBag.NganhNghe = dvvl.DM_NganhKinhDoanh.Where(th => th.ID_Parent == 0).ToList();
            return PartialView();
        }

        public ActionResult DanhSachTD()
        {
            if (Session["DanhSach"] == "NganhNghe")
            {
                int nnid = int.Parse(Session["NganhNgheID"].ToString());
                Session["SoLuongKQ"] = dvvl.DoanhNghiep_HS.Where(th => th.DoanhNghiep.ID_NganhKinhDoanh == nnid && th.TinhTrangHS == 2 && th.NgayHetHanHS != null && th.NgayHetHanHS > DateTime.Now)
                                                .Count();
            }
            else if (Session["DanhSach"] == "KhuVuc")
            {
                int kvid = int.Parse(Session["KhuVucID"].ToString());
                Session["SoLuongKQ"] = dvvl.DoanhNghiep_HS.Where(th => th.DoanhNghiep.ID_QuanHuyen == kvid && th.TinhTrangHS == 2 && th.NgayHetHanHS != null && th.NgayHetHanHS > DateTime.Now)
                                                .Count();
            }
            else if (Session["DanhSach"] == "TuKhoa")
            {
                string tk = Session["ChuoiTimKiem"].ToString();
                Session["SoLuongKQ"] = dvvl.DoanhNghiep_HS.Where(th => th.TieuDeHoSo.Contains(tk) || th.DoanhNghiep.TenDoanhNghiep.Contains(tk) && th.TinhTrangHS == 2 && th.NgayHetHanHS != null && th.NgayHetHanHS > DateTime.Now)
                                                .Count();
            }
            else
            {
                Session["SoLuongKQ"] = dvvl.DoanhNghiep_HS.Where(th => th.NgayHetHanHS != null && th.TinhTrangHS == 2 && th.NgayHetHanHS > DateTime.Now).Count();
            }
            return PartialView();
        }
        [HttpGet]
        public ActionResult TD_Default(int PageNo = 0, int pageSize = 5)
        {
            var model = dvvl.DoanhNghiep_HS.Where(th => th.NgayHetHanHS != null && th.TinhTrangHS == 2 && th.NgayHetHanHS > DateTime.Now)
                                            .OrderByDescending(th => th.NgayCapNhat)
                                            .Skip(PageNo * pageSize)
                                            .Take(pageSize)
                                            .ToList();
            return PartialView(model);
        }

        public ActionResult TaoHoSoTuyenDung()
        {
            ViewBag.ID_ChucDanh = new SelectList(dvvl.DM_ChucDanh.ToList(), "ID_ChucDanh", "TenChucDanh");
            ViewBag.ID_MucLuong = new SelectList(dvvl.DM_MucLuong.ToList(), "ID_MucLuong", "TenMucLuong");
            ViewBag.ID_ThoiGianLamViec = new SelectList(dvvl.DM_ThoiGianLamViec.ToList(), "ID_TGLamViec", "TenTGLamViec");
            ViewBag.YeuCauTrinhDo = new SelectList(dvvl.DM_TrinhDo.ToList(), "ID_TrinhDo", "TenTrinhDo");
            ViewBag.YeuCauChuyenNganh = new SelectList(dvvl.DM_NganhDaoTao.Where(th => th.ID_Parent == 0), "ID_NganhDaoTao", "TenNganhDaoTao");
            ViewBag.YeuCauNN = new SelectList(dvvl.DM_NghiepVu.Where(th => th.ID_Parent == 19), "ID_NghiepVu", "TenNghiepVu");
            ViewBag.YeuCauTH = new SelectList(dvvl.DM_NghiepVu.Where(th => th.ID_Parent == 12).OrderBy(th => th.ID_NghiepVu), "ID_NghiepVu", "TenNghiepVu");

            return PartialView();
        }
        [HttpPost]
        public ActionResult TaoHoSoTuyenDung(DoanhNghiep_HS model_dn_hs)
        {
            int userid = int.Parse(Session["UsrID"].ToString());
            var dnid = dvvl.DoanhNghieps.Where(th => th.ID_User == userid).Single();
            model_dn_hs.ID_DoanhNghiep = dnid.ID_DoanhNghiep;
            model_dn_hs.SoLuotXem = 0;
            model_dn_hs.TinhTrangHS = 1;
            model_dn_hs.ViecLamHapDan = 0;
            model_dn_hs.NgayTao = DateTime.Now;
            model_dn_hs.ID_NguoiTao = userid;
            model_dn_hs.NgayCapNhat = DateTime.Now;
            model_dn_hs.ID_NguoiCapNhat = userid;

            dvvl.DoanhNghiep_HS.Add(model_dn_hs);
            dvvl.SaveChanges();
            return RedirectToAction("HoSoTD", "Account");
        }

        public ActionResult TD_Detail(int id)
        {
            var model = dvvl.DoanhNghiep_HS.Find(id);
            ViewBag.bycty = dvvl.DoanhNghiep_HS.Where(th => th.ID_DoanhNghiep == model.ID_DoanhNghiep && th.TinhTrangHS == 2 && th.NgayHetHanHS != null && th.NgayHetHanHS >= DateTime.Now).ToList();
            model.SoLuotXem++;
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

        public ActionResult DSTDTheoNganhNghe(string id)
        {
            Session["DanhSach"] = "NganhNghe";
            Session["NganhNgheID"] = id;
            return PartialView("TD_Main");
        }

        public ActionResult KQTDTheoNganhNghe(string Id, int pageNo = 0, int pageSize = 5)
        {
            int iid = int.Parse(Id.ToString());
            var model = dvvl.DoanhNghiep_HS.Where(th => th.DoanhNghiep.ID_NganhKinhDoanh == iid && th.TinhTrangHS == 2 && th.NgayHetHanHS != null && th.NgayHetHanHS > DateTime.Now)
                                            .OrderByDescending(th => th.NgayCapNhat)
                                            .Skip(pageNo * pageSize)
                                            .Take(pageSize)
                                            .ToList();
            return PartialView("TD_Default", model);
        }

        public ActionResult DSTDTheoKhuVuc(string id, int PageSize = 5)
        {
            Session["DanhSach"] = "KhuVuc";
            Session["KhuVucID"] = id;
            return PartialView("TD_Main");
        }

        public ActionResult KQTDTheoKhuVuc(string Id, int pageNo = 0, int pageSize = 5)
        {
            int iid = int.Parse(Id.ToString());
            var model = dvvl.DoanhNghiep_HS.Where(th => th.DoanhNghiep.ID_QuanHuyen == iid && th.TinhTrangHS == 2 && th.NgayHetHanHS != null && th.NgayHetHanHS > DateTime.Now)
                                                .OrderByDescending(th => th.NgayCapNhat)
                                                .Skip(pageNo * pageSize)
                                                .Take(pageSize)
                                                .ToList();
            return PartialView("TD_Default", model);
        }

        public ActionResult DSTDTheoTuKhoa()
        {
            Session["DanhSach"] = "TuKhoa";
            Session["ChuoiTimKiem"] = Request.Form["ChuoiTimKiem"].ToString();
            Session["NganhNgheID"] = "";
            Session["KhuVucID"] = "";
            return PartialView("TD_Main");
        }

        public ActionResult KQTDTheoTuKhoa(string ChuoiTimKiem, int pageNo = 0, int pageSize = 5)
        {
            var model = dvvl.DoanhNghiep_HS.Where(th => th.TinhTrangHS==2 && (th.TieuDeHoSo.Contains(ChuoiTimKiem) || th.DoanhNghiep.TenDoanhNghiep.Contains(ChuoiTimKiem)) && th.NgayHetHanHS != null && th.NgayHetHanHS > DateTime.Now)
                                                .OrderByDescending(th => th.NgayCapNhat)
                                                .Skip(pageNo * pageSize)
                                                .Take(pageSize)
                                                .ToList();
            return PartialView("TD_Default", model);
        }

        public ActionResult HSTDLuu(int id, int ID_DoanhNghiep, string TenHoSo)
        {
            HSTD_Luu hs = new HSTD_Luu();
            var uID = Session["UsrID"];
            int userid = int.Parse(uID.ToString());

            var model = dvvl.HSTD_Luu.Where(th => th.ID_User == userid && th.ID_DoanhNghiep == ID_DoanhNghiep && th.ID_HSTuyenDung == id).ToList();
            if (model.Count() == 0)
            {
                hs.ID_User = int.Parse(uID.ToString());
                hs.ID_DoanhNghiep = ID_DoanhNghiep;
                hs.ID_HSTuyenDung = id;
                hs.TenHoSo = TenHoSo;
                hs.NgayTao = DateTime.Now;

                dvvl.HSTD_Luu.Add(hs);
                int i = dvvl.SaveChanges();
                if (i > 0)
                    TempData["testmsg"] = " Luu Thanh Cong !!! ";
                else
                    TempData["testmsg"] = " Luu Loi !!! ";
                return RedirectToAction("TD_Detail", "TD", new { Id = id });
            }
            else
            {
                TempData["testmsg"] = " Ho So Da Co Trong Kho Cua Ban !!! ";
                return RedirectToAction("TD_Detail", "TD", new { Id = id });
            }

        }

        public ActionResult GuiHSChoDN(int ID_HSTuyenDung, int ID_DoanhNghiep, string TinNhan)
        {
            GuiHSChoDN guihs = new GuiHSChoDN();
            var uID = Session["UsrID"];
            int userid = int.Parse(uID.ToString());

            guihs.ID_UserNTV = userid;
            guihs.ID_DoanhNghiep = ID_DoanhNghiep;
            guihs.ID_HSTuyenDung = ID_HSTuyenDung;
            guihs.TinNhanNTV = TinNhan;
            guihs.DaXem = 0;
            guihs.TinhTrang = 2;
            guihs.NgayTao = DateTime.Now;

            dvvl.GuiHSChoDNs.Add(guihs);
            int kt = dvvl.SaveChanges();
            if(kt>0)
            {
                var mail = dvvl.Database.SqlQuery<string>("select Email from [DVVL].[dbo].[Users] where [ID_User]=" + userid);
                string email = mail.First();
                var hoten = dvvl.Database.SqlQuery<string>("select hoten from [DVVL].[dbo].[NguoiTimViec] where [ID_User] = " + userid);
                string hovaten = hoten.First();
                Common.SentMail.Send(email, "Trung tâm DVVL Bình Dương thông báo ", "Bạn vừa nhận được 1 tin nhắn từ ứng viên tên <b>"+hovaten+"</b> với nội dung '<i>"+TinNhan+"</i>'. Xem thông tin ứng viên tại:" + "<br /><br />" +
                        "http://localhost:59954/NTV/ThongTinChiTietNTV/"+userid+"?ID_GuiHSChoDN="+guihs.ID_GuiHSChoDN);
            }

            return RedirectToAction("TD_Detail", "TD", new { Id = ID_HSTuyenDung });
        }

        public ActionResult ThongTinChiTietDN(int id, int ID_GuiHSChoNTV)
        {
            var model_hsgui = dvvl.GuiHSChoNTVs.Find(ID_GuiHSChoNTV);
            model_hsgui.DaXem += 1;
            int id_ntv = model_hsgui.ID_NTV;
            dvvl.Entry(model_hsgui).State = EntityState.Modified;
            dvvl.SaveChanges();

            var dem = dvvl.GuiHSChoNTVs.Where(th => th.ID_NTV == id_ntv && th.DaXem == 0 && th.TinhTrang == 2).ToList();

            if (dem.Count() == 0)
            {
                Session["TinNhanNTV"] = "OLD"; 
                Session["Dem"] = dem.Count(); 
            }
            else
            {
                Session["TinNhanNTV"] = "NEW";
                Session["Dem"] = dem.Count(); 
            }

            var iddn = dvvl.Database.SqlQuery<int>("select ID_DoanhNghiep from [DVVL].[dbo].[DoanhNghiep] where ID_User = " + id);
            int id_dn = iddn.First();
            var model = dvvl.DoanhNghieps.Find(id_dn);
            ViewBag.bycty = dvvl.DoanhNghiep_HS.Where(th => th.ID_DoanhNghiep == id_dn && th.TinhTrangHS == 2 && th.NgayHetHanHS != null && th.NgayHetHanHS >= DateTime.Now).OrderByDescending(th => th.ID_HSTuyenDung).ToList();
            
            return PartialView(model);
        }
    }
}
