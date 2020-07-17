using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;
using DVVLBD.Filters;
using DVVLBD.Models;
using System.IO;
using System.Drawing;
using System.Text;
using DVVLBD.Models.dvvlBD;
using System.Data.Entity;

namespace DVVLBD.Controllers
{
    //[Authorize]
    //[InitializeSimpleMembership]
    public class AccountController : Controller
    {
        dvvlBD dvvl = new dvvlBD();

        public ActionResult Login()
        {
            return PartialView("Login");
        }
        [HttpPost]
        public ActionResult Login(string Username, string Password)
        {
            var ID = dvvl.Database.SqlQuery<int>("select [ID_User] from [DVVL].[dbo].[Users] where [UserName]=N'" + Username + "'");
            if (ID.Count() == 0)
            {
                ModelState.AddModelError("", "Sai tên đăng nhập");
                return PartialView("Login");
            }
            else
            {
                int UserID = ID.First();
                Session["UsrID"] = ID.First();
                var user = dvvl.Users.Where(th => th.ID_User == UserID).Single();
                var matkhau = Common.MaHoaMD5.MD5Hash(Password);
                if (user.PassWord != matkhau)
                {
                    ModelState.AddModelError("", "Sai mật khẩu");
                    return PartialView("Login");
                }
                else
                {
                    Session["User"] = user.TenHienThi;
                    //kiểm tra quyền                    
                    var quyen = dvvl.Users.Where(th => th.ID_User == UserID).Single();

                    if (quyen.Quyen == 1)
                    {
                        Session["Quyen"] = "ADMIN";
                    }
                    else if (quyen.Quyen == 2)
                    {
                        Session["Quyen"] = "TD";
                        var iddn = dvvl.DoanhNghieps.Where(th => th.ID_User == UserID).Single();
                        int id_dn = iddn.ID_DoanhNghiep;
                        var dem = dvvl.GuiHSChoDNs.Where(th => th.ID_DoanhNghiep == id_dn && th.DaXem == 0 && th.TinhTrang == 2).ToList();
                        Session["Dem"] = dem.Count();
                        if (dem.Count() > 0)
                        {
                            Session["TinNhan"] = "NEW";
                        }
                        else
                        {
                            Session["TinNhan"] = "OLD";
                        }
                    }
                    else
                    {
                        Session["Quyen"] = "NTV";
                        var idntv = dvvl.NguoiTimViecs.Where(th => th.ID_User == UserID).Single();
                        int id_ntv = idntv.ID_NTV;
                        var dem = dvvl.GuiHSChoNTVs.Where(th => th.ID_NTV == id_ntv && th.DaXem == 0 && th.TinhTrang == 2).ToList();
                        Session["Dem"] = dem.Count();
                        if (dem.Count() > 0)
                        {
                            Session["TinNhanNTV"] = "NEW";
                        }
                        else
                        {
                            Session["TinNhanNTV"] = "OLD";
                        }
                    }
                }
            }
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Logoff()
        {
            if (Session.Count > 0)
            {
                if (Session["Quyen"] != null || Session["Quyen"].ToString() != "")
                {
                    Session.Remove("User");
                    Session.Remove("Quyen");
                    Session.Remove("TinNhan");
                    Session.Remove("TinNhanNTV");
                    Session.Remove("Dem");
                    Session.Remove("hhh");
                    Session.Remove("duyet");
                    Session.Remove("quanly");
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Register(string captcha, string Username, string password, string password2, string Email, string optradio)
        {
            if (captcha != null)
            {
                if (ModelState.IsValid)
                {
                    //kiểm tra captcha trước
                    string getcaptcha = Session["captchar"].ToString();
                    if (captcha == getcaptcha)
                    {
                        var user = dvvl.Users.Where(p => p.UserName == Username).ToList();

                        if (password != password2)
                        {
                            ModelState.AddModelError("", "Nhập lại mật khẩu không đúng !!!");
                        }
                        else if (user.Count == 0)
                        {
                            //lưu thông tin user vào session
                            string[] user_regiter = new string[4];
                            user_regiter[0] = Username;
                            user_regiter[1] = Email;
                            user_regiter[2] = Common.MaHoaMD5.MD5Hash(password);
                            user_regiter[3] = Username;
                            Session["User"] = user_regiter;

                            if (optradio == "ntv")
                            {
                                return RedirectToAction("NTV_Account", "Account");
                            }
                            else return RedirectToAction("TD_Account", "Account");
                        }
                        else { ModelState.AddModelError("", "User này đã tồn tại!!!"); }
                    }
                    else ViewData["captcha"] = "Điền chính xác dòng mã này!!";
                }
            }
            return PartialView("Register");
        }

        public ActionResult NTV_Account()
        {
            ViewBag.ID_TinhThanhPho = new SelectList(dvvl.DM_DiaChi.Where(th => th.ID_TuyenTren == 0), "ID_DiaChi", "TenDiaChi");
            ViewBag.ID_DanToc = new SelectList(dvvl.DM_DanToc.ToList(), "ID_DanToc", "TenDanToc");
            ViewBag.TonGiao = new SelectList(dvvl.DM_TonGiao.ToList(), "ID_TonGiao", "TenTonGiao");
            ViewBag.TrinhDoVanHoa = new SelectList(dvvl.DM_HocVan.ToList(), "ID_HocVan", "TenHocVan");
            ViewBag.TrinhdoChuyenMon = new SelectList(dvvl.DM_TrinhDo.ToList(), "ID_TrinhDo", "TenTrinhDo");
            ViewBag.TrinhDoNgoaiNgu = new SelectList(dvvl.DM_NghiepVu.Where(th => th.ID_Parent == 19).OrderBy(th => th.ID_NghiepVu), "ID_NghiepVu", "TenNghiepVu");
            ViewBag.TrinhDoTinHoc = new SelectList(dvvl.DM_NghiepVu.Where(th => th.ID_Parent == 12), "ID_NghiepVu", "TenNghiepVu");
            ViewBag.ChuyenNganh = new SelectList(dvvl.DM_NganhDaoTao.Where(th => th.ID_Parent == 0), "ID_NganhDaoTao", "TenNganhDaoTao");
            return PartialView("NTV_Account");
        }
        [HttpPost]
        public ActionResult NTV_Account(NguoiTimViec model_ntv)
        {
            if (Session["User"] != null)
            {
                //xử lý file ảnh
                var file = Request.Files["AnhDaiDien"];
                if (file.ContentLength > 0)
                {
                    var ten = file.FileName;
                    if (CheckFileType(ten))
                    {
                        var ext = ten.Substring(ten.LastIndexOf('.'));
                        ten = Guid.NewGuid() + ext;
                        model_ntv.AnhDaiDien = ten;
                        file.SaveAs(Server.MapPath("~/Images/NTV_Images/") + ten);
                    }
                    else
                    {
                        ModelState.AddModelError("", "Chỉ chấp nhận định dạng ảnh png/jpg/jpeg !!!");
                        ViewBag.ID_TinhThanhPho = new SelectList(dvvl.DM_DiaChi.Where(th => th.ID_TuyenTren == 0), "ID_DiaChi", "TenDiaChi");
                        ViewBag.ID_DanToc = new SelectList(dvvl.DM_DanToc.ToList(), "ID_DanToc", "TenDanToc");
                        ViewBag.TonGiao = new SelectList(dvvl.DM_TonGiao.ToList(), "ID_TonGiao", "TenTonGiao");
                        ViewBag.TrinhDoVanHoa = new SelectList(dvvl.DM_HocVan.ToList(), "ID_HocVan", "TenHocVan");
                        ViewBag.TrinhdoChuyenMon = new SelectList(dvvl.DM_TrinhDo.ToList(), "ID_TrinhDo", "TenTrinhDo");
                        ViewBag.TrinhDoNgoaiNgu = new SelectList(dvvl.DM_NghiepVu.Where(th => th.ID_Parent == 19).OrderBy(th => th.ID_NghiepVu), "ID_NghiepVu", "TenNghiepVu");
                        ViewBag.TrinhDoTinHoc = new SelectList(dvvl.DM_NghiepVu.Where(th => th.ID_Parent == 12), "ID_NghiepVu", "TenNghiepVu");
                        ViewBag.ChuyenNganh = new SelectList(dvvl.DM_NganhDaoTao.Where(th => th.ID_Parent == 0), "ID_NganhDaoTao", "TenNganhDaoTao");
                        return PartialView("NTV_Account");
                    }
                }
                else if (model_ntv.GioiTinh == 1)
                {
                    model_ntv.AnhDaiDien = "AvataNam.jpg";
                }
                else model_ntv.AnhDaiDien = "AvataNu.jpg";

                var user_dangky = new string[4];
                user_dangky = Session["User"] as string[];
                DVVLBD.Models.dvvlBD.User model_user = new User();
                model_user.UserName = user_dangky[0].ToString();
                model_user.Email = user_dangky[1].ToString();
                model_user.PassWord = user_dangky[2].ToString();
                model_user.TenHienThi = user_dangky[3].ToString();
                model_user.Quyen = 3;
                model_user.NgayTao = DateTime.Now;
                dvvl.Users.Add(model_user);
                dvvl.SaveChanges();

                model_ntv.ID_User = model_user.ID_User;

                model_ntv.Email = model_user.Email;
                model_ntv.NgayTao = DateTime.Now;
                model_ntv.NgayCapNhat = DateTime.Now;
                dvvl.NguoiTimViecs.Add(model_ntv);
                int kt2 = dvvl.SaveChanges();
                if (kt2 > 0)
                {
                    return PartialView("Login");
                }
                else
                {
                    return PartialView("Register");
                }
            }
            else
            {
                return PartialView("Register");
            }
        }

        public ActionResult TD_Account()
        {
            ViewBag.ID_TinhThanhPho = new SelectList(dvvl.DM_DiaChi.Where(th => th.ID_TuyenTren == 0), "ID_DiaChi", "TenDiaChi");
            ViewBag.ID_LoaiHinhDoanhNghiep = new SelectList(dvvl.DM_LoaiHinhDoanhNghiep.ToList(), "ID_LoaiDoanhNghiep", "TenLoaiDoanhNghiep");
            ViewBag.ID_NganhKinhDoanh = new SelectList(dvvl.DM_NganhKinhDoanh.Where(th => th.ID_Parent == 0), "ID_NganhKinhDoanh", "TenNganhKinhDoanh");
            return PartialView("TD_Account");
        }

        [HttpPost]
        public ActionResult TD_Account(DoanhNghiep model_dn)
        {
            if (Session["User"] != null)
            {
                //xử lý file ảnh
                var file = Request.Files["LoGo"];
                if (file.ContentLength > 0)
                {
                    var ten = file.FileName;
                    if (CheckFileType(ten))
                    {
                        var ext = ten.Substring(ten.LastIndexOf('.'));
                        ten = Guid.NewGuid() + ext;
                        model_dn.LoGo = ten;
                        file.SaveAs(Server.MapPath("~/Images/DN_Images/") + ten);
                    }
                    else
                    {
                        ModelState.AddModelError("", "Chỉ chấp nhận định dạng ảnh png/jpg/jpeg !!!");
                        ViewBag.ID_TinhThanhPho = new SelectList(dvvl.DM_DiaChi.Where(th => th.ID_TuyenTren == 0), "ID_DiaChi", "TenDiaChi");
                        ViewBag.ID_LoaiHinhDoanhNghiep = new SelectList(dvvl.DM_LoaiHinhDoanhNghiep.ToList(), "ID_LoaiDoanhNghiep", "TenLoaiDoanhNghiep");
                        ViewBag.ID_NganhKinhDoanh = new SelectList(dvvl.DM_NganhKinhDoanh.Where(th => th.ID_Parent == 0), "ID_NganhKinhDoanh", "TenNganhKinhDoanh");
                        return PartialView("NTV_Account");
                    }
                }
                else model_dn.LoGo = "iconlogo1.png";

                var user_dangky = new string[4];
                user_dangky = Session["User"] as string[];
                DVVLBD.Models.dvvlBD.User model_user = new User();
                model_user.UserName = user_dangky[0].ToString();
                model_user.Email = user_dangky[1].ToString();
                model_user.PassWord = user_dangky[2].ToString();
                model_user.TenHienThi = user_dangky[3].ToString();
                model_user.Quyen = 2;
                model_user.NgayTao = DateTime.Now;
                dvvl.Users.Add(model_user);
                dvvl.SaveChanges();

                model_dn.ID_User = model_user.ID_User;

                model_dn.NgayTao = DateTime.Now;
                model_dn.NgayCapNhat = DateTime.Now;
                dvvl.DoanhNghieps.Add(model_dn);
                int kt3 = dvvl.SaveChanges();
                if (kt3 > 0)
                {
                    return PartialView("Login");
                }
                else
                {
                    return PartialView("Register");
                }
            }
            else
            {
                return PartialView("Register");
            }
        }

        private bool CheckFileType(string FileName)
        {
            string ext = Path.GetExtension(FileName);
            if (ext.Equals(".png") || ext.Equals(".jpg") || ext.Equals(".jpeg"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool CheckFileTypePDF(string FileName)
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

        public ActionResult HuyenThiXa(int tinhthanhphoID)
        {
            var huyenthixa = dvvl.DM_DiaChi.Where(th => th.ID_TuyenTren == tinhthanhphoID).Select(th => new { ID_DiaChi = th.ID_DiaChi, TenDiaChi = th.TenDiaChi });

            return Json(huyenthixa, JsonRequestBehavior.AllowGet);
        }

        public ActionResult NganhNgheKD(int nganhID)
        {
            var nganhnghekd = dvvl.DM_NganhKinhDoanh.Where(th => th.ID_Parent == nganhID).Select(th => new { ID_NganhKinhDoanh = th.ID_NganhKinhDoanh, TenNganhKinhDoanh = th.TenNganhKinhDoanh });

            return Json(nganhnghekd, JsonRequestBehavior.AllowGet);
        }

        public ActionResult HoSoTD()
        {
            return PartialView("HoSoTD");
        }

        public ActionResult ChuyenMucTD()
        {
            return PartialView();
        }

        public ActionResult ThongTinTaiKhoanTD()
        {
            var uID = Session["UsrID"];
            if (uID != null)
            {
                ViewBag.TD = dvvl.Database.SqlQuery<DoanhNghiep>("select * from [DVVL].[dbo].[DoanhNghiep] where ID_User=" + uID);
                return PartialView();
            }
            else
            {
                return PartialView("Login");
            }
        }

        public ActionResult ThayDoiThongTinTaiKhoanDN()
        {
            var uID = Session["UsrID"];
            if (uID != null)
            {
                int id = int.Parse(uID.ToString());
                var model = dvvl.DoanhNghieps.Where(th => th.ID_User == id).Single();

                ViewBag.GTC = model.GioiThieuChung;
                ViewBag.ID_TinhThanhPho = new SelectList(dvvl.DM_DiaChi.Where(th => th.ID_TuyenTren == 0), "ID_DiaChi", "TenDiaChi", model.ID_TinhThanhPho);
                ViewBag.ID_QuanHuyen = new SelectList(dvvl.DM_DiaChi.Where(th => th.ID_TuyenTren == model.ID_TinhThanhPho), "ID_DiaChi", "TenDiaChi", model.ID_QuanHuyen);
                ViewBag.ID_PhuongXa = new SelectList(dvvl.DM_DiaChi.Where(th => th.ID_TuyenTren == model.ID_QuanHuyen), "ID_DiaChi", "TenDiaChi", model.ID_PhuongXa);
                ViewBag.ID_LoaiHinhDoanhNghiep = new SelectList(dvvl.DM_LoaiHinhDoanhNghiep.ToList(), "ID_LoaiDoanhNghiep", "TenLoaiDoanhNghiep", model.ID_LoaiHinhDoanhNghiep);
                ViewBag.ID_NganhKinhDoanh = new SelectList(dvvl.DM_NganhKinhDoanh.Where(th => th.ID_Parent == 0), "ID_NganhKinhDoanh", "TenNganhKinhDoanh", model.ID_NganhKinhDoanh);
                ViewBag.ID_NgheKinhDoanh = new SelectList(dvvl.DM_NganhKinhDoanh.Where(th => th.ID_Parent == model.ID_NganhKinhDoanh), "ID_NganhKinhDoanh", "TenNganhKinhDoanh", model.ID_NgheKinhDoanh);

                return View("ThayDoiThongTinTaiKhoanDN", model);
            }
            else
            {
                return PartialView("Login");
            }
        }
        [HttpPost]
        public ActionResult ThayDoiThongTinTaiKhoanDN(DoanhNghiep model_dn_change, DateTime? NgayThanhLap1)
        {

            var file = Request.Files["LoGo"];
            if (file.ContentLength > 0)
            {
                var ten = file.FileName;
                if (CheckFileType(ten))
                {
                    var ext = ten.Substring(ten.LastIndexOf('.'));
                    ten = Guid.NewGuid() + ext;
                    model_dn_change.LoGo = ten;
                    file.SaveAs(Server.MapPath("~/Images/DN_Images/") + ten);
                }
                else
                {
                    ModelState.AddModelError("", "Chỉ chấp nhận định dạng ảnh png/jpg/jpeg !!!");
                    var uID = Session["UsrID"];
                    if (uID == null)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        int id = int.Parse(uID.ToString());
                        var model = dvvl.DoanhNghieps.Where(th => th.ID_User == id).Single();
                        ViewBag.GTC = model.GioiThieuChung;
                        ViewBag.ID_TinhThanhPho = new SelectList(dvvl.DM_DiaChi.Where(th => th.ID_TuyenTren == 0), "ID_DiaChi", "TenDiaChi", model.ID_TinhThanhPho);
                        ViewBag.ID_QuanHuyen = new SelectList(dvvl.DM_DiaChi.Where(th => th.ID_TuyenTren == model.ID_TinhThanhPho), "ID_DiaChi", "TenDiaChi", model.ID_QuanHuyen);
                        ViewBag.ID_PhuongXa = new SelectList(dvvl.DM_DiaChi.Where(th => th.ID_TuyenTren == model.ID_QuanHuyen), "ID_DiaChi", "TenDiaChi", model.ID_PhuongXa);
                        ViewBag.ID_LoaiHinhDoanhNghiep = new SelectList(dvvl.DM_LoaiHinhDoanhNghiep.ToList(), "ID_LoaiDoanhNghiep", "TenLoaiDoanhNghiep", model.ID_LoaiHinhDoanhNghiep);
                        ViewBag.ID_NganhKinhDoanh = new SelectList(dvvl.DM_NganhKinhDoanh.Where(th => th.ID_Parent == 0), "ID_NganhKinhDoanh", "TenNganhKinhDoanh", model.ID_NganhKinhDoanh);
                        ViewBag.ID_NgheKinhDoanh = new SelectList(dvvl.DM_NganhKinhDoanh.Where(th => th.ID_Parent == model.ID_NganhKinhDoanh), "ID_NganhKinhDoanh", "TenNganhKinhDoanh", model.ID_NgheKinhDoanh);
                        return PartialView("ThayDoiThongTinTaiKhoanDN");
                    }
                }
            }
            else model_dn_change.LoGo = "TNThumbnail.jpg";

            if (NgayThanhLap1 != null)
            {
                model_dn_change.NgayThanhLap = NgayThanhLap1;
            }
            model_dn_change.NgayCapNhat = DateTime.Now;

            dvvl.Entry(model_dn_change).State = EntityState.Modified;

            dvvl.SaveChanges();
            return PartialView("HoSoTD");
        }

        public ActionResult HoSoNTV()
        {
            return PartialView("HoSoNTV");
        }

        public ActionResult ChuyenMucNTV()
        {
            return PartialView();
        }

        public ActionResult ThongTinTaiKhoanNTV()
        {
            var uID = Session["UsrID"];
            if (uID != null)
            {
                ViewBag.NTV = dvvl.Database.SqlQuery<NguoiTimViec>("select * from [DVVL].[dbo].[NguoiTimViec] where ID_User=" + uID);
                return PartialView();
            }
            else
            {
                return PartialView("Login");
            }
        }

        public ActionResult ThayDoiThongTinTaiKhoanNTV()
        {
            var uID = Session["UsrID"];
            if (uID != null)
            {
                int id = int.Parse(uID.ToString());
                var model = dvvl.NguoiTimViecs.Where(th => th.ID_User == id).Single();

                ViewBag.GioiTinh = model.GioiTinh;
                ViewBag.ID_TinhThanhPho = new SelectList(dvvl.DM_DiaChi.Where(th => th.ID_TuyenTren == 0), "ID_DiaChi", "TenDiaChi", model.ID_TinhThanhPho);
                ViewBag.ID_QuanHuyen = new SelectList(dvvl.DM_DiaChi.Where(th => th.ID_TuyenTren == model.ID_TinhThanhPho), "ID_DiaChi", "TenDiaChi", model.ID_QuanHuyen);
                ViewBag.ID_PhuongXa = new SelectList(dvvl.DM_DiaChi.Where(th => th.ID_TuyenTren == model.ID_QuanHuyen), "ID_DiaChi", "TenDiaChi", model.ID_PhuongXa);
                ViewBag.TinhTrangHonNhan = model.TinhTrangHonNhan;
                ViewBag.ID_DanToc = new SelectList(dvvl.DM_DanToc.ToList(), "ID_DanToc", "TenDanToc", model.ID_DanToc);
                ViewBag.TonGiao = new SelectList(dvvl.DM_TonGiao.ToList(), "ID_TonGiao", "TenTonGiao", model.TonGiao);
                ViewBag.TrinhDoVanHoa = new SelectList(dvvl.DM_HocVan.ToList(), "ID_HocVan", "TenHocVan", model.TrinhDoVanHoa);
                ViewBag.TrinhDoChuyenMon = new SelectList(dvvl.DM_TrinhDo.ToList(), "ID_TrinhDo", "TenTrinhDo", model.TrinhDoChuyenMon);
                ViewBag.TrinhDoNgoaiNgu = new SelectList(dvvl.DM_NghiepVu.Where(th => th.ID_Parent == 19).OrderBy(th => th.ID_NghiepVu), "ID_NghiepVu", "TenNghiepVu", model.TrinhDoNgoaiNgu);
                ViewBag.TrinhDoTinHoc = new SelectList(dvvl.DM_NghiepVu.Where(th => th.ID_Parent == 12), "ID_NghiepVu", "TenNghiepVu", model.TrinhDoNgoaiNgu);

                return PartialView("ThayDoiThongTinTaiKhoanNTV", model);
            }
            else
            {
                return PartialView("Login");
            }
        }
        [HttpPost]
        public ActionResult ThayDoiThongTinTaiKhoanNTV(NguoiTimViec model_ntv_change, DateTime? NgaySinh1)
        {
            var file = Request.Files["AnhDaiDien"];
            if (file.ContentLength > 0)
            {
                var ten = file.FileName;
                if (CheckFileType(ten))
                {
                    var ext = ten.Substring(ten.LastIndexOf('.'));
                    ten = Guid.NewGuid() + ext;
                    model_ntv_change.AnhDaiDien = ten;
                    file.SaveAs(Server.MapPath("~/Images/NTV_Images/") + ten);
                }
                else
                {
                    ModelState.AddModelError("", "Chỉ chấp nhận định dạng ảnh png/jpg/jpeg !!!");
                    var uID = Session["UsrID"];
                    if (uID == null)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        int id = int.Parse(uID.ToString());
                        var model = dvvl.NguoiTimViecs.Where(th => th.ID_User == id).Single();
                        ViewBag.GioiTinh = model.GioiTinh;
                        ViewBag.ID_TinhThanhPho = new SelectList(dvvl.DM_DiaChi.Where(th => th.ID_TuyenTren == 0), "ID_DiaChi", "TenDiaChi", model.ID_TinhThanhPho);
                        ViewBag.ID_QuanHuyen = new SelectList(dvvl.DM_DiaChi.Where(th => th.ID_TuyenTren == model.ID_TinhThanhPho), "ID_DiaChi", "TenDiaChi", model.ID_QuanHuyen);
                        ViewBag.ID_PhuongXa = new SelectList(dvvl.DM_DiaChi.Where(th => th.ID_TuyenTren == model.ID_QuanHuyen), "ID_DiaChi", "TenDiaChi", model.ID_PhuongXa);
                        ViewBag.TinhTrangHonNhan = model.TinhTrangHonNhan;
                        ViewBag.ID_DanToc = new SelectList(dvvl.DM_DanToc.ToList(), "ID_DanToc", "TenDanToc", model.ID_DanToc);
                        ViewBag.TonGiao = new SelectList(dvvl.DM_TonGiao.ToList(), "ID_TonGiao", "TenTonGiao", model.TonGiao);
                        ViewBag.TrinhDoVanHoa = new SelectList(dvvl.DM_HocVan.ToList(), "ID_HocVan", "TenHocVan", model.TrinhDoVanHoa);
                        ViewBag.TrinhDoChuyenMon = new SelectList(dvvl.DM_TrinhDo.ToList(), "ID_TrinhDo", "TenTrinhDo", model.TrinhDoChuyenMon);
                        ViewBag.TrinhDoNgoaiNgu = new SelectList(dvvl.DM_NghiepVu.Where(th => th.ID_Parent == 19).OrderBy(th => th.ID_NghiepVu), "ID_NghiepVu", "TenNghiepVu", model.TrinhDoNgoaiNgu);
                        ViewBag.TrinhDoTinHoc = new SelectList(dvvl.DM_NghiepVu.Where(th => th.ID_Parent == 12), "ID_NghiepVu", "TenNghiepVu", model.TrinhDoNgoaiNgu);
                        return PartialView("ThayDoiThongTinTaiKhoanNTV");
                    }
                }
            }
            else if (model_ntv_change.GioiTinh == 1)
            {
                model_ntv_change.AnhDaiDien = "AvataNam.jpg";
            }
            else model_ntv_change.AnhDaiDien = "AvataNu.jpg";

            if (NgaySinh1 != null)
            {
                model_ntv_change.NgaySinh = NgaySinh1;
            }

            model_ntv_change.NgayCapNhat = DateTime.Now;

            dvvl.Entry(model_ntv_change).State = EntityState.Modified;

            dvvl.SaveChanges();
            return PartialView("HoSoNTV");
        }

        public ActionResult HoSoTDCuaDN()
        {
            var uID = Session["UsrID"];
            var DNId = dvvl.Database.SqlQuery<int>("select ID_DoanhNghiep from [DVVL].[dbo].[DoanhNghiep] where ID_User=" + uID);
            ViewBag.DSHSbyDN = dvvl.Database.SqlQuery<DoanhNghiep_HS>("select * from [DVVL].[dbo].[DoanhNghiep_HS] where [ID_DoanhNghiep]=" + DNId.First() + " order by ID_HSTuyenDung desc");
            return PartialView();
        }

        public ActionResult ChinhSuaHSTD(int id)
        {
            var model = dvvl.DoanhNghiep_HS.Find(id);
            ViewBag.ID_ChucDanh = new SelectList(dvvl.DM_ChucDanh.ToList(), "ID_ChucDanh", "TenChucDanh", model.ID_ChucDanh);
            ViewBag.Mota = model.MoTa;
            ViewBag.QuyenLoi = model.QuyenLoi;
            ViewBag.ID_MucLuong = new SelectList(dvvl.DM_MucLuong.ToList(), "ID_MucLuong", "TenMucLuong", model.ID_MucLuong);
            ViewBag.ID_ThoiGianLamViec = new SelectList(dvvl.DM_ThoiGianLamViec.ToList(), "ID_TGLamViec", "TenTGLamViec", model.ID_ThoiGianLamViec);
            ViewBag.YeuCauTrinhDo = new SelectList(dvvl.DM_TrinhDo.ToList(), "ID_TrinhDo", "TenTrinhDo", model.YeuCauTrinhDo);
            ViewBag.YeuCauChuyenNganh = new SelectList(dvvl.DM_NganhDaoTao.Where(th => th.ID_Parent == 0), "ID_NganhDaoTao", "TenNganhDaoTao", model.YeuCauChuyenNganh);
            ViewBag.YeuCauNN = new SelectList(dvvl.DM_NghiepVu.Where(th => th.ID_Parent == 19), "ID_NghiepVu", "TenNghiepVu", model.YeuCauNN);
            ViewBag.YeuCauTH = new SelectList(dvvl.DM_NghiepVu.Where(th => th.ID_Parent == 12).OrderBy(th => th.ID_NghiepVu), "ID_NghiepVu", "TenNghiepVu", model.YeuCauTH);
            ViewBag.GioiTinh = model.YeuCauGioiTinh;
            ViewBag.GhiChu = model.GhiChu;
            ViewBag.GiayToYeuCau = model.GiayToYeuCau;
            return PartialView("ChinhSuaHSTD", model);
        }
        [HttpPost]
        public ActionResult ChinhSuaHSTD(DoanhNghiep_HS model, DateTime? NgayBatDauHS1, DateTime? NgayHetHanHS1, DateTime? NgayDuTuyen1)
        {
            model.NgayCapNhat = DateTime.Now;
            if (NgayBatDauHS1 != null)
            {
                model.NgayNhanHS = NgayBatDauHS1;
            }
            if (NgayHetHanHS1 != null)
            {
                model.NgayHetHanHS = NgayHetHanHS1;
            }
            if (NgayDuTuyen1 != null)
            {
                model.NgayDuTuyen = NgayDuTuyen1;
            }
            dvvl.Entry(model).State = EntityState.Modified;
            dvvl.SaveChanges();
            return PartialView("HoSoTD");
        }

        public ActionResult XoaHSTD(int id)
        {
            var model = dvvl.DoanhNghiep_HS.Find(id);
            dvvl.DoanhNghiep_HS.Remove(model);
            dvvl.SaveChanges();
            return PartialView("HoSoTD");
        }

        public ActionResult HoSoXVCuaNTV()
        {
            var uID = Session["UsrID"];
            var NTVId = dvvl.Database.SqlQuery<int>("select ID_NTV from [DVVL].[dbo].[NguoiTimViec] where ID_User=" + uID);
            ViewBag.DSHSbyNTV = dvvl.Database.SqlQuery<NguoiTimViec_HS>("select * from [DVVL].[dbo].[NguoiTimViec_HS] where [ID_NTV]=" + NTVId.First() + " order by ID_HSXinViec desc");
            return PartialView();
        }

        public ActionResult ChinhSuaHSNTV(int id)
        {
            var model = dvvl.NguoiTimViec_HS.Find(id);
            ViewBag.ID_ChucDanhMongMuon = new SelectList(dvvl.DM_ChucDanh.ToList(), "ID_ChucDanh", "TenChucDanh", model.ID_ChucDanhMongMuon);
            ViewBag.ID_MucLuongMongMuon = new SelectList(dvvl.DM_MucLuong.ToList(), "ID_MucLuong", "TenMucLuong", model.ID_MucLuongMongMuon);
            ViewBag.ID_NganhNgheMM = new SelectList(dvvl.DM_NganhKinhDoanh.Where(th => th.ID_Parent == 0), "ID_NganhKinhDoanh", "TenNganhKinhDoanh", model.ID_NganhNgheMM);
            ViewBag.ID_LoaiHinhDNMM = new SelectList(dvvl.DM_LoaiHinhDoanhNghiep.ToList(), "ID_LoaiDoanhNghiep", "TenLoaiDoanhNghiep", model.ID_LoaiHinhDNMM);
            ViewBag.ID_NoiLamViecMM = new SelectList(dvvl.DM_DiaChi.Where(th => th.ID_TuyenTren == 9), "ID_DiaChi", "TenDiaChi", model.ID_NoiLamViecMM);
            ViewBag.ID_ThoiGianLamViecMM = new SelectList(dvvl.DM_ThoiGianLamViec.ToList(), "ID_TGLamViec", "TenTGLamViec", model.ID_ThoiGianLamViecMM);
            ViewBag.MoTa = model.MoTa;
            return PartialView("ChinhSuaHSNTV", model);
        }
        [HttpPost]
        public ActionResult ChinhSuaHSNTV(NguoiTimViec_HS model, DateTime? NgayBatDau1)
        {
            var file = Request.Files["CV"];
            if (file.ContentLength > 0)
            {
                var ten = file.FileName;
                if (CheckFileTypePDF(ten))
                {
                    var ext = ten.Substring(ten.LastIndexOf('.'));
                    ten = Guid.NewGuid() + ext;
                    model.CV = ten;
                    file.SaveAs(Server.MapPath("~/Images/CV/") + ten);
                }
            }
            if (NgayBatDau1 != null)
            {
                model.NgayBatDau = NgayBatDau1;
            }
            model.NgayCapNhat = DateTime.Now;

            dvvl.Entry(model).State = EntityState.Modified;
            dvvl.SaveChanges();
            return PartialView("HoSoNTV");
        }

        public ActionResult XoaHSNTV(int id)
        {
            var model = dvvl.NguoiTimViec_HS.Find(id);
            dvvl.NguoiTimViec_HS.Remove(model);
            dvvl.SaveChanges();
            return PartialView("HoSoNTV");
        }

        public ActionResult QuanLyHoSoCuaDN()
        {
            int uID = int.Parse(Session["UsrID"].ToString());
            var DNid = dvvl.Database.SqlQuery<int>("select ID_DoanhNghiep from [DVVL].[dbo].[DoanhNghiep] where [ID_User]=" + uID);
            int idDN = DNid.First();

            ViewBag.HSGuiChoDN = dvvl.Database.SqlQuery<GuiHSChoDN>("select * from [DVVL].[dbo].[GuiHSChoDN] where [ID_DoanhNghiep]=" + idDN + " and [TinhTrang]=2 order by ID_GuiHSChoDN desc");
            ViewBag.HSNhan = dvvl.Database.SqlQuery<GuiHSChoNTV>("select * from [DVVL].[dbo].[GuiHSChoNTV] where [ID_UserDN]=" + uID + " and [TinhTrang]=2 order by ID_GuiHSChoNTV desc");
            ViewBag.HSDaLuu = dvvl.Database.SqlQuery<HSNTV_Luu>("select * from [DVVL].[dbo].[HSNTV_Luu] where [ID_User]=" + uID + " order by ID_HSNTVLuu desc");

            return PartialView();
        }

        public ActionResult XoaHSLuuDN(int id)
        {
            var model = dvvl.HSNTV_Luu.Find(id);
            dvvl.HSNTV_Luu.Remove(model);
            dvvl.SaveChanges();
            Session["hhh"] = "hhh";
            return PartialView("HoSoTD");
        }

        public ActionResult QuanLyHoSoCuaNTV()
        {
            int uID = int.Parse(Session["UsrID"].ToString());
            var NTVid = dvvl.Database.SqlQuery<int>("select ID_NTV from [DVVL].[dbo].[NguoiTimViec] where [ID_User]=" + uID);
            int idNTV = NTVid.First();

            ViewBag.HSGuiChoNTV = dvvl.Database.SqlQuery<GuiHSChoNTV>("select * from [DVVL].[dbo].[GuiHSChoNTV] where [ID_NTV]=" + idNTV + " and [TinhTrang]=2 order by ID_GuiHSChoNTV desc");
            ViewBag.HSNhan = dvvl.Database.SqlQuery<GuiHSChoDN>("select * from [DVVL].[dbo].[GuiHSChoDN] where [ID_UserNTV]=" + uID + " and [TinhTrang]=2 order by ID_GuiHSChoDN desc");
            ViewBag.HSDaLuu = dvvl.Database.SqlQuery<HSTD_Luu>("select * from [DVVL].[dbo].[HSTD_Luu] where [ID_User]=" + uID + " order by ID_HSTDLuu desc");
            return PartialView();
        }

        public ActionResult XoaHSLuuNTV(int id)
        {
            var model = dvvl.HSTD_Luu.Find(id);
            dvvl.HSTD_Luu.Remove(model);
            dvvl.SaveChanges();
            Session["hhh1"] = "hhh1";
            return PartialView("HoSoNTV");
        }

        public ActionResult XoaTinNhanDenDN(int id)
        {
            var model = dvvl.GuiHSChoDNs.Find(id);
            dvvl.GuiHSChoDNs.Remove(model);
            dvvl.SaveChanges();
            Session["hhh"] = "sss";

            return PartialView("HoSoTD");
        }

        public ActionResult XoaTinNhanDiDN(int id)
        {
            var model = dvvl.GuiHSChoNTVs.Find(id);
            dvvl.GuiHSChoNTVs.Remove(model);
            dvvl.SaveChanges();
            Session["hhh"] = "ttt";

            return PartialView("HoSoTD");
        }

        public ActionResult XoaTinNhanDenNTV(int id)
        {
            var model = dvvl.GuiHSChoNTVs.Find(id);
            dvvl.GuiHSChoNTVs.Remove(model);
            dvvl.SaveChanges();
            Session["hhh1"] = "sss1";

            return PartialView("HoSoNTV");
        }

        public ActionResult XoaTinNhanDiNTV(int id)
        {
            var model = dvvl.GuiHSChoDNs.Find(id);
            dvvl.GuiHSChoDNs.Remove(model);
            dvvl.SaveChanges();
            Session["hhh1"] = "ttt1";

            return PartialView("HoSoNTV");
        }

        public ActionResult QuanLyAdmin()
        {
            Session.Remove("quanly");
            return PartialView("QuanLyAdmin");
        }

        public ActionResult ChuyenMucAdmin()
        {
            return PartialView();
        }

        public ActionResult QuanLyNguoiDung()
        {
            ViewBag.NguoiDung = dvvl.Users.OrderByDescending(th => th.ID_User).Take(5).ToList();
            var demUser = dvvl.Users.ToList();
            int i = demUser.Count();
            int ii = i / 5;
            Session["ii"] = ii;

            return PartialView("QuanLyNguoiDung");
        }

        public ActionResult QuanLyNguoiDungSkip(int pageNo, int pageSize)
        {
            ViewBag.NguoiDung = dvvl.Users.OrderByDescending(th => th.ID_User).Skip(pageNo * pageSize).Take(pageSize).ToList();
            return PartialView("ListAccount");
        }

        public ActionResult User_Detail(int id)
        {
            var model = dvvl.Users.Find(id);

            return PartialView(model);
        }

        public ActionResult XoaUser(int id)
        {
            var model = dvvl.Users.Find(id);
            dvvl.Users.Remove(model);
            dvvl.SaveChanges();
            return PartialView("QuanLyAdmin");
        }
        
        ////////quản lý doanh nghiệp/////////
        
        public ActionResult QuanLyDoanhNghiep()
        {
            ViewBag.DoanhNghiep = dvvl.DoanhNghiep_HS.Where(th => th.TinhTrangHS == 2).OrderByDescending(th => th.NgayCapNhat).Take(5).ToList();
            var demDN = dvvl.DoanhNghiep_HS.Where(th => th.TinhTrangHS == 2).ToList();
            int i = demDN.Count();
            int ii = i / 5;
            Session["demdn"] = ii;

            ViewBag.DuyetHoSo = dvvl.DoanhNghiep_HS.Where(th => th.TinhTrangHS == 1).OrderByDescending(th => th.ID_HSTuyenDung).Take(5).ToList();
            var demDNduyet = dvvl.DoanhNghiep_HS.Where(th => th.TinhTrangHS == 1).ToList();
            int demdnduyet1 = demDNduyet.Count();
            Session["demduyet"] = demdnduyet1 / 5;

            ViewBag.DuyetTinNhan = dvvl.GuiHSChoDNs.Where(th => th.TinhTrang == 2).OrderByDescending(th => th.NgayTao).Take(5).ToList();
            var demTNDN = dvvl.GuiHSChoDNs.Where(th => th.TinhTrang == 2).ToList();
            int demtndn1 = demTNDN.Count();
            Session["demtndn"] = demtndn1 / 5;

            return PartialView();
        }

        public ActionResult QuanLyDoanhNghiepSkip(int pageNo, int pageSize)
        {
            ViewBag.DoanhNghiep = dvvl.DoanhNghiep_HS.Where(th => th.TinhTrangHS == 2).OrderByDescending(th => th.NgayCapNhat).Skip(pageNo * pageSize).Take(pageSize).ToList();
            return PartialView("ListDoanhNghiep");
        }

        public ActionResult QuanLyDuyetDoanhNghiepSkip(int pageNo, int pageSize)
        {
            ViewBag.DuyetHoSo = dvvl.DoanhNghiep_HS.Where(th => th.TinhTrangHS == 1).OrderByDescending(th => th.ID_HSTuyenDung).Skip(pageNo * pageSize).Take(pageSize).ToList();
           
            return PartialView("ListDuyetDN");
        }

        public ActionResult QuanLyDuyetTNDNSkip(int pageNo, int pageSize)
        {
            ViewBag.DuyetTinNhan = dvvl.GuiHSChoDNs.Where(th => th.TinhTrang == 2).OrderByDescending(th => th.NgayTao).Skip(pageNo * pageSize).Take(pageSize).ToList();
            
            return PartialView("ListDuyetTNDN");
        }
        
        public ActionResult DuyetHoSo(int id)
        {
            var model = dvvl.DoanhNghiep_HS.Find(id);
            model.TinhTrangHS = 2;
            model.NgayCapNhat = DateTime.Now;
            dvvl.Entry(model).State = EntityState.Modified;
            string iddn = model.ID_DoanhNghiep.ToString();
            var iduser = dvvl.Database.SqlQuery<int>("select ID_User from [DVVL].[dbo].[DoanhNghiep] where [ID_DoanhNghiep]=" + iddn);
            var mail = dvvl.Database.SqlQuery<string>("select Email from [DVVL].[dbo].[Users] where [ID_User]=" + iduser.First());
            string email = mail.First();
            Common.SentMail.Send(email, "TTDVVL thông báo kiểm duyệt", "Thông tin Tuyển dụng của bạn đã được duyệt. Xem chi tiết tại:" + "<br /><br />" +
                    "http://localhost:59954/TD/TD_Detail/" + model.ID_HSTuyenDung);
            try
            {
                dvvl.SaveChanges();
                TempData["th"] = "tb";
                Session["duyet"] = "ok";
                Session["quanly"] = "dn";
                return PartialView("QuanLyAdmin");
            }
            catch
            {
                TempData["th"] = "bt";
                Session["duyet"] = "ok";
                Session["quanly"] = "dn";
                return PartialView("QuanLyAdmin");
            }
        }

        public ActionResult HuyDuyetHoSo(int id)
        {
            var model = dvvl.DoanhNghiep_HS.Find(id);
            model.TinhTrangHS = 3;
            model.NgayCapNhat = DateTime.Now;
            dvvl.Entry(model).State = EntityState.Modified;
            string iddn = model.ID_DoanhNghiep.ToString();
            var iduser = dvvl.Database.SqlQuery<int>("select ID_User from [DVVL].[dbo].[DoanhNghiep] where [ID_DoanhNghiep]=" + iddn);
            var mail = dvvl.Database.SqlQuery<string>("select Email from [DVVL].[dbo].[Users] where [ID_User]=" + iduser.First());
            string email = mail.First();
            Common.SentMail.Send(email, "TTDVVL thông báo kiểm duyệt", "Thông tin Tuyển dụng của bạn với tên hồ sơ: [" + model.TieuDeHoSo + "] đã bị hủy do không đạt yêu cầu");
            try
            {
                dvvl.SaveChanges();
                TempData["th"] = "tb";
                Session["duyet"] = "ok";
                Session["quanly"] = "dn";
                return PartialView("QuanLyAdmin");
            }
            catch
            {
                TempData["th"] = "bt";
                Session["duyet"] = "ok";
                Session["quanly"] = "dn";
                return PartialView("QuanLyAdmin");
            }
        }

        public ActionResult DuyetTatCaDN()
        {
            var model = dvvl.DoanhNghiep_HS.Where(th => th.TinhTrangHS == 1).OrderByDescending(th => th.ID_HSTuyenDung).ToList();
            foreach (var th in model)
            {
                th.TinhTrangHS = 2;
                th.NgayCapNhat = DateTime.Now;
                dvvl.Entry(th).State = EntityState.Modified;
                string iddn = th.ID_DoanhNghiep.ToString();
                var iduser = dvvl.Database.SqlQuery<int>("select ID_User from [DVVL].[dbo].[DoanhNghiep] where [ID_DoanhNghiep]=" + iddn);
                var mail = dvvl.Database.SqlQuery<string>("select Email from [DVVL].[dbo].[Users] where [ID_User]=" + iduser.First());
                string email = mail.First();
                Common.SentMail.Send(email, "TTDVVL thông báo kiểm duyệt", "Thông tin Tuyển dụng của bạn đã được duyệt. Xem chi tiết tại:" + "<br /><br />" +
                        "http://localhost:59954/TD/TD_Detail/" + th.ID_HSTuyenDung);
            }
            try
            {
                dvvl.SaveChanges();
                TempData["th"] = "tb";
                Session["duyet"] = "ok";
                Session["quanly"] = "dn";
                return PartialView("QuanLyAdmin");
            }
            catch
            {
                TempData["th"] = "bt";
                Session["duyet"] = "ok";
                Session["quanly"] = "dn";
                return PartialView("QuanLyAdmin");
            }
        }

        public ActionResult HuyDuyetTatCaDN()
        {
            var model = dvvl.DoanhNghiep_HS.Where(th => th.TinhTrangHS == 1).OrderByDescending(th => th.ID_HSTuyenDung).ToList();
            foreach (var th in model)
            {
                th.TinhTrangHS = 3;
                th.NgayCapNhat = DateTime.Now;
                dvvl.Entry(th).State = EntityState.Modified;
                string iddn = th.ID_DoanhNghiep.ToString();
                var iduser = dvvl.Database.SqlQuery<int>("select ID_User from [DVVL].[dbo].[DoanhNghiep] where [ID_DoanhNghiep]=" + iddn);
                var mail = dvvl.Database.SqlQuery<string>("select Email from [DVVL].[dbo].[Users] where [ID_User]=" + iduser.First());
                string email = mail.First();
                Common.SentMail.Send(email, "TTDVVL thông báo kiểm duyệt", "Thông tin Tuyển dụng của bạn với tên hồ sơ: [" + th.TieuDeHoSo + "] đã bị hủy do không đạt yêu cầu");
            
            }
            try
            {
                dvvl.SaveChanges();
                TempData["th"] = "tb";
                Session["duyet"] = "ok";
                Session["quanly"] = "dn";
                return PartialView("QuanLyAdmin");
            }
            catch
            {
                TempData["th"] = "bt";
                Session["duyet"] = "ok";
                Session["quanly"] = "dn";
                return PartialView("QuanLyAdmin");
            }
        }

        //public ActionResult DuyetTinNhan(int id)
        //{
        //    var model = dvvl.GuiHSChoDNs.Find(id);
        //    model.TinhTrang = 2;
        //    dvvl.Entry(model).State = EntityState.Modified;
        //    try
        //    {
        //        dvvl.SaveChanges();
        //        Session["duyet"] = "tn";
        //        Session["quanly"] = "dn";
        //        return PartialView("QuanLyAdmin");
        //    }
        //    catch
        //    {
        //        Session["duyet"] = "tn";
        //        Session["quanly"] = "dn";
        //        return PartialView("QuanLyAdmin");
        //    }
        //}

        public ActionResult HuyDuyetTinNhan(int id)
        {
            var model = dvvl.GuiHSChoDNs.Find(id);
            dvvl.GuiHSChoDNs.Remove(model);
            //dvvl.Entry(model).State = EntityState.Modified;

            try
            {
                dvvl.SaveChanges();
                Session["duyet"] = "tn";
                Session["quanly"] = "dn";
                return PartialView("QuanLyAdmin");
            }
            catch
            {
                Session["duyet"] = "tn";
                Session["quanly"] = "dn";
                return PartialView("QuanLyAdmin");
            }
        }

        //public ActionResult DuyetTatCaTNDN()
        //{
        //    var model = dvvl.GuiHSChoDNs.Where(th => th.TinhTrang == 1).OrderByDescending(th => th.NgayTao).ToList();
        //    foreach (var th in model)
        //    {
        //        th.TinhTrang = 2;
        //        dvvl.Entry(th).State = EntityState.Modified;
        //    }
        //    try
        //    {
        //        dvvl.SaveChanges();
        //        Session["duyet"] = "tn";
        //        Session["quanly"] = "dn";
        //        return PartialView("QuanLyAdmin");
        //    }
        //    catch
        //    {
        //        Session["duyet"] = "tn";
        //        Session["quanly"] = "dn";
        //        return PartialView("QuanLyAdmin");
        //    }
        //}

        //public ActionResult HuyDuyetTatCaTNDN()
        //{
        //    var model = dvvl.GuiHSChoDNs.Where(th => th.TinhTrang == 1).OrderByDescending(th => th.NgayTao).ToList();
        //    foreach (var th in model)
        //    {
        //        th.TinhTrang = 3;
        //        dvvl.Entry(th).State = EntityState.Modified;
        //    }
        //    try
        //    {
        //        dvvl.SaveChanges();
        //        Session["duyet"] = "tn";
        //        Session["quanly"] = "dn";
        //        return PartialView("QuanLyAdmin");
        //    }
        //    catch
        //    {
        //        Session["duyet"] = "tn";
        //        Session["quanly"] = "dn";
        //        return PartialView("QuanLyAdmin");
        //    }
        //}

        ////////quản lý người tìm việc/////////

        public ActionResult QuanLyNguoiTimViec()
        {
            ViewBag.NguoiTimViec = dvvl.NguoiTimViec_HS.Where(th => th.TinhTrangHS == 2).OrderByDescending(th => th.NgayCapNhat).Take(5).ToList();
            var demNTV = dvvl.NguoiTimViec_HS.ToList();
            int i = demNTV.Count();
            int ii = i / 5;
            Session["demntv"] = ii;

            ViewBag.DuyetHoSoNTV = dvvl.NguoiTimViec_HS.Where(th => th.TinhTrangHS == 1).OrderByDescending(th => th.ID_HSXinViec).Take(5).ToList();
            var demNTVduyet = dvvl.NguoiTimViec_HS.Where(th => th.TinhTrangHS == 1).ToList();
            int demntvduyet1 = demNTVduyet.Count();
            Session["demduyetntv"] = demntvduyet1 / 5;

            ViewBag.DuyetTinNhanNTV = dvvl.GuiHSChoNTVs.Where(th => th.TinhTrang == 2).OrderByDescending(th => th.NgayTao).Take(5).ToList();
            var demTNNTV = dvvl.GuiHSChoNTVs.Where(th => th.TinhTrang == 2).ToList();
            int demtnntv1 = demTNNTV.Count();
            Session["demtnntv"] = demtnntv1 / 5;
            return PartialView();
        }

        public ActionResult QuanLyNguoiTimViecSkip(int pageNo, int pageSize)
        {
            ViewBag.NguoiTimViec = dvvl.NguoiTimViec_HS.Where(th => th.TinhTrangHS == 2).OrderByDescending(th => th.NgayCapNhat).Skip(pageNo * pageSize).Take(pageSize).ToList();
            return PartialView("ListNguoiTimViec");
        }

        public ActionResult QuanLyDuyetNguoiTimViecSkip(int pageNo, int pageSize)
        {
            ViewBag.DuyetHoSoNTV = dvvl.NguoiTimViec_HS.Where(th => th.TinhTrangHS == 1).OrderByDescending(th => th.ID_HSXinViec).Skip(pageNo * pageSize).Take(pageSize).ToList();

            return PartialView("ListDuyetNTV");
        }

        public ActionResult QuanLyDuyetTNNTVSkip(int pageNo, int pageSize)
        {
            ViewBag.DuyetTinNhanNTV = dvvl.GuiHSChoNTVs.Where(th => th.TinhTrang == 2).OrderByDescending(th => th.NgayTao).Skip(pageNo * pageSize).Take(pageSize).ToList();

            return PartialView("ListDuyetTNNTV");
        }

        public ActionResult DuyetHoSoNTV(int id)
        {
            var model = dvvl.NguoiTimViec_HS.Find(id);
            model.TinhTrangHS = 2;
            model.NgayCapNhat = DateTime.Now;
            dvvl.Entry(model).State = EntityState.Modified;
            string idntv = model.ID_NTV.ToString();
            var iduser = dvvl.Database.SqlQuery<int>("select ID_User from [DVVL].[dbo].[NguoiTimViec] where [ID_NTV]=" + idntv);
            var mail = dvvl.Database.SqlQuery<string>("select Email from [DVVL].[dbo].[Users] where [ID_User]=" + iduser.First());
            string email = mail.First();
            Common.SentMail.Send(email, "TTDVVL thông báo kiểm duyệt", "Thông tin Tuyển dụng của bạn đã được duyệt. Xem chi tiết tại:" + "<br /><br />" +
                    "http://localhost:59954/NTV/NTV_Detail/" + model.ID_HSXinViec);
            try
            {
                dvvl.SaveChanges();
                TempData["th"] = "tb";
                Session["duyet"] = "okntv";
                Session["quanly"] = "ntv";
                return PartialView("QuanLyAdmin");
            }
            catch
            {
                TempData["th"] = "bt";
                Session["duyet"] = "okntv";
                Session["quanly"] = "ntv";
                return PartialView("QuanLyAdmin");
            }
        }

        public ActionResult HuyDuyetHoSoNTV(int id)
        {
            var model = dvvl.NguoiTimViec_HS.Find(id);
            model.TinhTrangHS = 3;
            model.NgayCapNhat = DateTime.Now;
            dvvl.Entry(model).State = EntityState.Modified;
            string idntv = model.ID_NTV.ToString();
            var iduser = dvvl.Database.SqlQuery<int>("select ID_User from [DVVL].[dbo].[NguoiTimViec] where [ID_NTV]=" + idntv);
            var mail = dvvl.Database.SqlQuery<string>("select Email from [DVVL].[dbo].[Users] where [ID_User]=" + iduser.First());
            string email = mail.First();
            Common.SentMail.Send(email, "TTDVVL thông báo kiểm duyệt", "Thông tin Xin việc của bạn với tên hồ sơ: [" + model.TenHSXinViec + "] đã bị hủy do không đạt yêu cầu");
            try
            {
                dvvl.SaveChanges();
                TempData["th"] = "tb";
                Session["duyet"] = "okntv";
                Session["quanly"] = "ntv";
                return PartialView("QuanLyAdmin");
            }
            catch
            {
                TempData["th"] = "bt";
                Session["duyet"] = "okntv";
                Session["quanly"] = "ntv";
                return PartialView("QuanLyAdmin");
            }
        }

        public ActionResult DuyetTatCaNTV()
        {
            var model = dvvl.NguoiTimViec_HS.Where(th => th.TinhTrangHS == 1).OrderByDescending(th => th.ID_HSXinViec).ToList();
            foreach (var th in model)
            {
                th.TinhTrangHS = 2;
                th.NgayCapNhat = DateTime.Now;
                dvvl.Entry(th).State = EntityState.Modified;
                string idntv = th.ID_NTV.ToString();
                var iduser = dvvl.Database.SqlQuery<int>("select ID_User from [DVVL].[dbo].[NguoiTimViec] where [ID_NTV]=" + idntv);
                var mail = dvvl.Database.SqlQuery<string>("select Email from [DVVL].[dbo].[Users] where [ID_User]=" + iduser.First());
                string email = mail.First();
                Common.SentMail.Send(email, "TTDVVL thông báo kiểm duyệt", "Thông tin Tuyển dụng của bạn đã được duyệt. Xem chi tiết tại:" + "<br /><br />" +
                        "http://localhost:59954/NTV/NTV_Detail/" + th.ID_HSXinViec);
            }
            try
            {
                dvvl.SaveChanges();
                TempData["th"] = "tb";
                Session["duyet"] = "okntv";
                Session["quanly"] = "ntv";
                return PartialView("QuanLyAdmin");
            }
            catch
            {
                TempData["th"] = "bt";
                Session["duyet"] = "okntv";
                Session["quanly"] = "ntv";
                return PartialView("QuanLyAdmin");
            }
        }

        public ActionResult HuyDuyetTatCaNTV()
        {
            var model = dvvl.NguoiTimViec_HS.Where(th => th.TinhTrangHS == 1).OrderByDescending(th => th.ID_HSXinViec).ToList();
            foreach (var th in model)
            {
                th.TinhTrangHS = 3;
                th.NgayCapNhat = DateTime.Now;
                dvvl.Entry(th).State = EntityState.Modified;
                string idntv = th.ID_NTV.ToString();
                var iduser = dvvl.Database.SqlQuery<int>("select ID_User from [DVVL].[dbo].[NguoiTimViec] where [ID_NTV]=" + idntv);
                var mail = dvvl.Database.SqlQuery<string>("select Email from [DVVL].[dbo].[Users] where [ID_User]=" + iduser.First());
                string email = mail.First();
                Common.SentMail.Send(email, "TTDVVL thông báo kiểm duyệt", "Thông tin Xin việc của bạn với tên hồ sơ: [" + th.TenHSXinViec + "] đã bị hủy do không đạt yêu cầu");
            
            }
            try
            {
                dvvl.SaveChanges();
                TempData["th"] = "tb";
                Session["duyet"] = "okntv";
                Session["quanly"] = "ntv";
                return PartialView("QuanLyAdmin");
            }
            catch
            {
                TempData["th"] = "bt";
                Session["duyet"] = "okntv";
                Session["quanly"] = "ntv";
                return PartialView("QuanLyAdmin");
            }
        }

        //public ActionResult DuyetTinNhanNTV(int id)
        //{
        //    var model = dvvl.GuiHSChoNTVs.Find(id);
        //    model.TinhTrang = 2;
        //    dvvl.Entry(model).State = EntityState.Modified;
        //    try
        //    {
        //        dvvl.SaveChanges();
        //        Session["duyet"] = "tnntv";
        //        Session["quanly"] = "ntv";
        //        return PartialView("QuanLyAdmin");
        //    }
        //    catch
        //    {
        //        Session["duyet"] = "tnntv";
        //        Session["quanly"] = "ntv";
        //        return PartialView("QuanLyAdmin");
        //    }
        //}

        public ActionResult HuyDuyetTinNhanNTV(int id)
        {
            var model = dvvl.GuiHSChoNTVs.Find(id);
            dvvl.GuiHSChoNTVs.Remove(model);
            //model.TinhTrang = 3;
            //dvvl.Entry(model).State = EntityState.Modified;

            try
            {
                dvvl.SaveChanges();
                Session["duyet"] = "tnntv";
                Session["quanly"] = "ntv";
                return PartialView("QuanLyAdmin");
            }
            catch
            {
                Session["duyet"] = "tnntv";
                Session["quanly"] = "ntv";
                return PartialView("QuanLyAdmin");
            }
        }

        //public ActionResult DuyetTatCaTNNTV()
        //{
        //    var model = dvvl.GuiHSChoNTVs.Where(th => th.TinhTrang == 1).OrderByDescending(th => th.NgayTao).ToList();
        //    foreach (var th in model)
        //    {
        //        th.TinhTrang = 2;
        //        dvvl.Entry(th).State = EntityState.Modified;
        //    }
        //    try
        //    {
        //        dvvl.SaveChanges();
        //        Session["duyet"] = "tnntv";
        //        Session["quanly"] = "ntv";
        //        return PartialView("QuanLyAdmin");
        //    }
        //    catch
        //    {
        //        Session["duyet"] = "tnntv";
        //        Session["quanly"] = "ntv";
        //        return PartialView("QuanLyAdmin");
        //    }
        //}

        //public ActionResult HuyDuyetTatCaTNNTV()
        //{
        //    var model = dvvl.GuiHSChoNTVs.Where(th => th.TinhTrang == 1).OrderByDescending(th => th.NgayTao).ToList();
        //    foreach (var th in model)
        //    {
        //        th.TinhTrang = 3;
        //        dvvl.Entry(th).State = EntityState.Modified;
        //    }
        //    try
        //    {
        //        dvvl.SaveChanges();
        //        Session["duyet"] = "tnntv";
        //        Session["quanly"] = "ntv";
        //        return PartialView("QuanLyAdmin");
        //    }
        //    catch
        //    {
        //        Session["duyet"] = "tnntv";
        //        Session["quanly"] = "ntv";
        //        return PartialView("QuanLyAdmin");
        //    }
        //}

        public ActionResult QuanLyThongKe()
        {
            ViewBag.DoanhNghiep = dvvl.DoanhNghieps.Count();
            ViewBag.DoanhNghiep_HS = dvvl.DoanhNghiep_HS.Count(th => th.TinhTrangHS == 2);
            ViewBag.NguoiTimViec = dvvl.NguoiTimViecs.Count();
            ViewBag.NguoiTimViec_HS = dvvl.NguoiTimViec_HS.Count(th => th.TinhTrangHS == 2);
         
            //ViewBag.ThongKeHSDN = dvvl.Database.SqlQuery<ThongKeHSTD>("select a.[ID_HSTuyenDung], a.[TieuDeHoSo], c.LuotLuu, e.LuotNhan from [DVVL].[dbo].[DoanhNghiep_HS] a " +
            //        "left join (select b.ID_HSTuyenDung,COUNT(b.[ID_HSTuyenDung]) LuotLuu from [DVVL].[dbo].[GuiHSChoDN] b group by [ID_HSTuyenDung]) c " +
            //        "on a.[ID_HSTuyenDung] = c.[ID_HSTuyenDung] " +
            //        "left join (select d.[ID_HSTuyenDung],COUNT(d.[ID_HSTuyenDung]) LuotNhan from [DVVL].[dbo].[HSTD_Luu] d group by [ID_HSTuyenDung]) e " +
            //        "on a.[ID_HSTuyenDung] = e.[ID_HSTuyenDung]");
            ViewBag.ThongKeHSDN = dvvl.DoanhNghiep_HS.Where(th => th.TinhTrangHS == 2).Select(th => new ThongKeHSTD
            {
                ID_HSTuyenDung = th.ID_HSTuyenDung,
                TieuDeHoSo = th.TieuDeHoSo,
                LuotLuu = dvvl.HSTD_Luu.Where(t => t.ID_HSTuyenDung == th.ID_HSTuyenDung).Count(),
                LuotNhan = dvvl.GuiHSChoDNs.Where(t => t.ID_HSTuyenDung == th.ID_HSTuyenDung).Count()
            }).OrderByDescending(th=>th.ID_HSTuyenDung).Take(5).ToList();

            var demHSDN = dvvl.DoanhNghiep_HS.Where(th => th.TinhTrangHS == 2).Select(th => new ThongKeHSTD
            {
                ID_HSTuyenDung = th.ID_HSTuyenDung,
                TieuDeHoSo = th.TieuDeHoSo,
                LuotLuu = dvvl.HSTD_Luu.Where(t => t.ID_HSTuyenDung == th.ID_HSTuyenDung).Count(),
                LuotNhan = dvvl.GuiHSChoDNs.Where(t => t.ID_HSTuyenDung == th.ID_HSTuyenDung).Count()
            }).OrderByDescending(th => th.ID_HSTuyenDung).ToList();

            int i = demHSDN.Count();
            int ii = i / 5;
            Session["demhsdn"] = ii;


            ViewBag.ThongKeHSNTV = dvvl.NguoiTimViec_HS.Where(th => th.TinhTrangHS == 2).Select(th => new ThongKeHSNTV
            {
                ID_HSXinViec = th.ID_HSXinViec,
                TenHSXinViec = th.TenHSXinViec,
                LuotLuu = dvvl.HSNTV_Luu.Where(t => t.ID_HSXinViec == th.ID_HSXinViec).Count(),
                LuotNhan = dvvl.GuiHSChoNTVs.Where(t => t.ID_HSXinViec == th.ID_HSXinViec).Count()
            }).OrderByDescending(th => th.ID_HSXinViec).Take(5).ToList();

            var demHSNTV = dvvl.NguoiTimViec_HS.Where(th => th.TinhTrangHS == 2).Select(th => new ThongKeHSNTV
            {
                ID_HSXinViec = th.ID_HSXinViec,
                TenHSXinViec = th.TenHSXinViec,
                LuotLuu = dvvl.HSNTV_Luu.Where(t => t.ID_HSXinViec == th.ID_HSXinViec).Count(),
                LuotNhan = dvvl.GuiHSChoNTVs.Where(t => t.ID_HSXinViec == th.ID_HSXinViec).Count()
            }).OrderByDescending(th => th.ID_HSXinViec).ToList();

            int itv = demHSNTV.Count();
            int iitv = itv / 5;
            Session["demhsntv"] = iitv;

            return PartialView();
        }

        public ActionResult QuanLyThongKeDNSkip(int pageNo, int pageSize)
        {
            ViewBag.ThongKeHSDN = dvvl.DoanhNghiep_HS.Where(th => th.TinhTrangHS == 2).Select(th => new ThongKeHSTD
            {
                ID_HSTuyenDung = th.ID_HSTuyenDung,
                TieuDeHoSo = th.TieuDeHoSo,
                LuotLuu = dvvl.HSTD_Luu.Where(t => t.ID_HSTuyenDung == th.ID_HSTuyenDung).Count(),
                LuotNhan = dvvl.GuiHSChoDNs.Where(t => t.ID_HSTuyenDung == th.ID_HSTuyenDung).Count()
            }).OrderByDescending(th => th.ID_HSTuyenDung).Skip(pageNo * pageSize).Take(pageSize).ToList();

            return PartialView("ListThongKeDN");
        }

        public ActionResult QuanLyThongKeNTVSkip(int pageNo, int pageSize)
        {
            ViewBag.ThongKeHSNTV = dvvl.NguoiTimViec_HS.Where(th => th.TinhTrangHS == 2).Select(th => new ThongKeHSNTV
            {
                ID_HSXinViec = th.ID_HSXinViec,
                TenHSXinViec = th.TenHSXinViec,
                LuotLuu = dvvl.HSNTV_Luu.Where(t => t.ID_HSXinViec == th.ID_HSXinViec).Count(),
                LuotNhan = dvvl.GuiHSChoNTVs.Where(t => t.ID_HSXinViec == th.ID_HSXinViec).Count()
            }).OrderByDescending(th => th.ID_HSXinViec).Skip(pageNo * pageSize).Take(pageSize).ToList();

            return PartialView("ListThongKeNTV");
        }

        public ActionResult CV_NTV(int id)
        {
            var model = dvvl.NguoiTimViec_HS.Find(id);
            return PartialView(model);
        }

        public FileResult GetCaptchaImage()
        {
            //get Random text
            StringBuilder randomText = new StringBuilder();
            string alphabets = "012345679ACEFGHKLMNPRSWXZabcdefghijkhlmnopqrstuvwxyz";
            Random r = new Random();
            for (int j = 0; j <= 5; j++)
            {
                randomText.Append(alphabets[r.Next(alphabets.Length)]);
            }
            Session["captchar"] = randomText.ToString();

            string text = randomText.ToString();

            //first, create a dummy bitmap just to get a graphics object
            Image img = new Bitmap(1, 1);
            Graphics drawing = Graphics.FromImage(img);

            Font font = new Font("Arial", 15);
            //measure the string to see how big the image needs to be
            SizeF textSize = drawing.MeasureString(text, font);

            //free up the dummy image and old graphics object
            img.Dispose();
            drawing.Dispose();

            //create a new image of the right size
            img = new Bitmap((int)textSize.Width + 40, (int)textSize.Height + 20);
            drawing = Graphics.FromImage(img);

            Color backColor = Color.SeaShell;
            Color textColor = Color.Red;
            //paint the background
            drawing.Clear(backColor);

            //create a brush for the text
            Brush textBrush = new SolidBrush(textColor);

            drawing.DrawString(text, font, textBrush, 20, 10);

            drawing.Save();

            font.Dispose();
            textBrush.Dispose();
            drawing.Dispose();

            MemoryStream ms = new MemoryStream();
            img.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            img.Dispose();

            return File(ms.ToArray(), "image/png");
        }

    }
}
