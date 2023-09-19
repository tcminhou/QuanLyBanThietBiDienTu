using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.IO;
using System.Data;

namespace QuanLyThietBiDienTu6.Controllers
{
    public class AdminController : Controller
    {
        WebDienTuEntities da = new WebDienTuEntities();
        private bool IsStrongPassword(string password)
        {
            if (password.Length < 8)
                return false;

            bool hasUppercase = false;
            bool hasLowercase = false;
            bool hasDigit = false;

            foreach (char c in password)
            {
                if (char.IsUpper(c))
                    hasUppercase = true;
                else if (char.IsLower(c))
                    hasLowercase = true;
                else if (char.IsDigit(c))
                    hasDigit = true;

                if (hasUppercase && hasLowercase && hasDigit)
                    return true;
            }

            return false;
        }
        public ActionResult DangNhap(FormCollection collection)
        {
            if (Session["AdminID"] != null)
                return RedirectToAction("Index");
            else
            {
                if (ModelState.IsValid)
                {
                    var tenDN = collection["TenDN"];
                    var matKhau = collection["Matkhau"];
                    if (KTUser(tenDN, matKhau))
                    {
                        Admin k = da.Admins.FirstOrDefault(n => n.UserAdmin == tenDN &&
                                                                    n.PassAdmin == matKhau);
                        
                        FormsAuthentication.SetAuthCookie(k.UserAdmin, false);
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("", "");
                       
                    }
                }
                return View();
            }

        }
        public ActionResult DangXuat()
        {
            FormsAuthentication.SignOut();
            Session.Abandon(); // it will clear the session at the end of request

            return RedirectToAction("Index", "Home");
        }
        private bool KTUser(string Email, string Password)
        {

            bool isValid = false;
            Admin User = da.Admins.FirstOrDefault(u => u.UserAdmin == Email&&u.PassAdmin==Password);
                if (User != null)
                {
                    
                        Session["AdminID"] = User.UserAdmin;
                        Session["HoTenAd"] = User.HoTen;
                        isValid = true;
                    
                }

            return isValid;
        }
        
        public ActionResult EditOrder(int id)
        {
            if (Session["AdminID"] == null)
                return RedirectToAction("Index", "Home");
            ViewData["KH"] = new SelectList(da.KHACHHANGs, "MaKH", "HoTen");
      
            DONDATHANG p = da.DONDATHANGs.FirstOrDefault(s => s.MaDonHang == id);
            
            return View(p);
        }

        //Xử lý cập nhật SP
        [HttpPost]
        public ActionResult EditOrder(int id,FormCollection collection)
        {
            try
            {
               
                DONDATHANG d = da.DONDATHANGs.First(s => s.MaDonHang == id);
                var tt= Request.Form["tt"];
                var gh = Request.Form["gh"];
                if (tt.Contains("true"))
                    d.Dathanhtoan = true;
                else
                    d.Dathanhtoan = false;

                if (gh.Contains("true"))
                    d.Tinhtranggiaohang = true;
                else
                    d.Tinhtranggiaohang = false;
                var NgayGiao = String.Format("{0:MM/dd/yyyy}", collection["NgayGiaoHang"]);
                d.Ngaygiao = DateTime.Parse(NgayGiao);


                
                
                
               
                
                da.SaveChanges();

                return RedirectToAction("ListOrder");
            }
            catch
            {
                return View();
            }
        }
        // GET: Admin
        
        public ActionResult DeleteOrder(int id, FormCollection collection)
        {
            try
            {
                DONDATHANG c = da.DONDATHANGs.Single(s => s.MaDonHang == id);
                if (c != null)
                {
                    da.DONDATHANGs.Remove(c);
                    da.SaveChanges();
                    return RedirectToAction("ListOrder");
                }
                else
                {
                    return RedirectToAction("ListOrder");
                }
            }
            catch
            {
                return RedirectToAction("ListOrder");
            }
            // TODO: Add delete logic here
         


        }
     
        public ActionResult DeleteOD(int id, FormCollection collection)
        {
            try {
                CHITIETDONTHANG c = da.CHITIETDONTHANGs.First(s => s.MaSP == id);
                if (c!= null)
                {
                    da.CHITIETDONTHANGs.Remove(c);
                    da.SaveChanges();
                    return RedirectToAction("ListOrder");
                }
                else
                {
                    return RedirectToAction("ListOrder");
                }
            }
            catch
            {
                return View();
            }
            // TODO: Add delete logic here
        

        }

        public ActionResult Index()
        {
            if (Session["AdminID"] == null)
                return RedirectToAction("Index","Home");
            ViewBag.XinChaoAd = Session["HoTenAd"];
            return View();
        }
        [HttpGet]
        public ActionResult DangKy()
        {           
            if (Session["AdminID"] == null)
                return RedirectToAction("Index","Home");
            return View();
        }
        [HttpPost]
        public ActionResult DangKy(FormCollection collection,Admin kh)
        {
            if (Session["AdminID"] != null)
                return RedirectToAction("Index");
            else
            {
                var HoTen = collection["HoTen"];
                var TenDN = collection["TenDN"];
                var MK = collection["MK"];
                var MKLai = collection["MKLai"];
                var MaAdmin = collection["MaAdmin"];
             
                Admin k = da.Admins.FirstOrDefault(s => s.UserAdmin == TenDN);
                Admin k1 = da.Admins.FirstOrDefault(s => s.MaAdmin == MaAdmin);

                if (String.IsNullOrEmpty(HoTen))
                {
                    ViewData["Loi1"] = "Ho ten khong duoc de trong";
                }
                else if (String.IsNullOrEmpty(TenDN) || k != null)
                {
                    ViewData["Loi2"] = "Tên đăng nhập không được để trống hoặc trùng";
                }
                else if (String.IsNullOrEmpty(MK) || !IsStrongPassword(MK))
                {
                    ViewData["Loi3"] = "Mật khẩu không được để trống và phải đủ 8 ký tự bao gồm chữ hoa, thường,số";
                }
                else if (String.IsNullOrEmpty(MKLai) || MKLai != MK)
                {
                    ViewData["Loi4"] = "Mật khẩu nhập lại không được để trống và phải giống mật khẩu";
                }
                else if (k1!=null)
                {
                    ViewData["Loi5"] = "Mã admin đã tồn tại";
                }
                else
                {
                    kh.MaAdmin = MaAdmin;
                    kh.HoTen = HoTen;
                    kh.UserAdmin = TenDN;
                    kh.PassAdmin = MK;
                
                    da.Admins.Add(kh);
                    da.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View();
            }

        }

        //Xem danh sách sp
        public ActionResult ListProducts()
        {
            if (Session["AdminID"] == null)
                return RedirectToAction("Index","Home");
            IEnumerable<SANPHAM> ds = da.SANPHAMs.OrderByDescending(s => s.MaSP);
            return View(ds);
        }
        public ActionResult ListOrderDetail(int id)
        {
            if (Session["AdminID"] == null)
                return RedirectToAction("Index", "Home");
            IEnumerable<CHITIETDONTHANG> ds = da.CHITIETDONTHANGs.Where(s=>s.MaDonHang==id);
            foreach(var item in ds)
            {
                if (item.DONDATHANG.Dathanhtoan == true)
                    ViewBag.ThanhToan = "Đã thanh toán";
                else
                    ViewBag.ThanhToan = "Chưa thanh toán";
                if (item.DONDATHANG.Tinhtranggiaohang == true)
                    ViewBag.GiaoHang = "Đã giao hàng";
                else
                    ViewBag.GiaoHang = "Chưa giao hàng";
            }
          
            return View(ds);
        }
        public ActionResult ListOrder()
        {
            if (Session["AdminID"] == null)
                return RedirectToAction("Index","Home");
            IEnumerable<DONDATHANG> ds = da.DONDATHANGs.OrderByDescending(s => s.MaDonHang);
             
            return View(ds);
        }
        public ActionResult ListCus()
        {
            if (Session["AdminID"] == null)
                return RedirectToAction("Index", "Home");
            IEnumerable<KHACHHANG> ds = da.KHACHHANGs.OrderByDescending(s => s.MaKH);
            return View(ds);
        }
        public ActionResult EditCus(int id)
        {
            if (Session["AdminID"] == null)
                return RedirectToAction("Index", "Home");
            KHACHHANG p = da.KHACHHANGs.FirstOrDefault(s => s.MaKH == id);
            return View(p);
        }
        [HttpPost]
        public ActionResult EditCus(int id, FormCollection collection)
        {


            try
            {
                KHACHHANG p = da.KHACHHANGs.First(s => s.MaKH == id);
                var HoTen = collection["HoTen"];

                var MK = collection["MK"];

                var DiaChi = collection["DC"];
                var DienThoai = collection["DT"];
                var NgaySinh = String.Format("{0:MM/dd/yyyy}", collection["NgaySinh"]);
                p.HoTen = HoTen;
                p.Matkhau = MK;
                p.DiachiKH = DiaChi;
                p.DienthoaiKH = DienThoai;
                p.Ngaysinh = DateTime.Parse(NgaySinh);

                da.SaveChanges();

                return RedirectToAction("ListCus");
            }
            catch (Exception)
            {

                return View();
            }
            
           
        }
        public ActionResult ListAd()
        {
            if (Session["AdminID"] == null)
                return RedirectToAction("Index", "Home");
            IEnumerable<Admin> ds = da.Admins.OrderByDescending(s => s.MaAdmin);
            return View(ds);
        }
        public ActionResult EditAd(string id)
        {
            if (Session["AdminID"] == null)
                return RedirectToAction("Index", "Home");
          
            Admin p = da.Admins.First(s => s.MaAdmin == id);
            return View(p);
        }
        [HttpPost]
        public ActionResult EditAd(string id, FormCollection collection)
        {


            try
            {
              
                Admin p = da.Admins.First(s => s.MaAdmin == id);
                var HoTen = collection["HoTen"];

                var MK = collection["MK"];

                var TK = collection["TK"];
                Admin k = da.Admins.FirstOrDefault(s => s.UserAdmin == TK);

                if (k != null)
                {
                    ViewData["Loi2"] = "Tài khoản không được trùng";
                }
                else
                {
                    p.HoTen = HoTen;
                    p.PassAdmin = MK;
                    p.UserAdmin = TK;


                    da.SaveChanges();
                }


                return RedirectToAction("ListAd");
            }
            catch (Exception)
            {

                return View();
            }


        }
        // POST: Product/Delete/5
        public ActionResult DeleteAd(string id)
        {


            try
            {
              
                Admin p = da.Admins.First(s => s.MaAdmin == id);
                if (p != null)
                {
                    da.Admins.Remove(p);
                    da.SaveChanges();
                    return RedirectToAction("ListAd");
                }
                else
                {
                    return RedirectToAction("ListAd");
                }
                //Thiet lap thuoc tinh
            }
            catch (Exception)
            {

                return RedirectToAction("ListCus");
            }






        }
        public ActionResult DeleteCus(int id)
        {


            try
            {
                KHACHHANG c = da.KHACHHANGs.Single(s => s.MaKH == id);
                if (c != null)
                {
                    da.KHACHHANGs.Remove(c);
                    da.SaveChanges();
                    return RedirectToAction("ListCus");
                }
                else
                {
                    return RedirectToAction("ListCus");
                }
                //Thiet lap thuoc tinh
            }
            catch (Exception)
            {

                return RedirectToAction("ListCus");
            }


          



        }
        //Giao diện thêm sp
        public ActionResult CreateProduct()
        {
            if (Session["AdminID"] == null)
                return RedirectToAction("Index","Home");

            ViewData["NCC"] = new SelectList(da.NHACUNGCAPs, "MaNCC", "TenNCC");
            ViewData["LSP"] = new SelectList(da.LoaiSPs, "MaLoaiSP", "TenLoaiSP");
            return View();

        }

        //Xử lý thêm SP
        [HttpPost]
        public ActionResult CreateProduct(SANPHAM sanpham, FormCollection collection) //FormCollection collection: toàn bộ dữ liệu có trên View
        {
            try
            {
                //Tạo mới 1 SP
                SANPHAM p = new SANPHAM();

                //Thiết lập các thuộc tính cho SP
                p = sanpham;

                //Thêm ảnh
                if (sanpham.ImageUpload != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(sanpham.ImageUpload.FileName); //lấy tên file ko có phần mở rộng
                    string extension = Path.GetExtension(sanpham.ImageUpload.FileName);//lấy phần mở rộng
                    fileName = fileName + extension;
                    p.AnhSP = fileName;
                    sanpham.ImageUpload.SaveAs(Path.Combine(Server.MapPath("../Content/Images/"), fileName));

                }
                else
                    p.AnhSP = "loi-hinh-anh.jpg";

                //Chưa có NCC là LSP
                //Gán giá trị MaNCC, MaLSP
                p.MaLoai = int.Parse(collection["LSP"]);
                p.MaNCC = int.Parse(collection["NCC"]);

                // Thêm SP vào bảng SanPham
                da.SANPHAMs.Add(p);

                //Cập nhật thay đổi db
                da.SaveChanges();

                //Hiển thị DSSP
                return RedirectToAction("ListProducts");

            }
            catch//chưa xử lý bắt lỗi
            {
                return View();
            }
        }

        //Giao diện SP muốn sửa
        public ActionResult EditProduct(int id)
        {
            if (Session["AdminID"] == null)
                return RedirectToAction("Index","Home");
            ViewData["NCC"] = new SelectList(da.NHACUNGCAPs, "MaNCC", "TenNCC");
            ViewData["LSP"] = new SelectList(da.LoaiSPs, "MaLoaiSP", "TenLoaiSP");
            SANPHAM p = da.SANPHAMs.FirstOrDefault(s => s.MaSP == id);
            return View(p);
        }

        //Xử lý cập nhật SP
        [HttpPost]
        public ActionResult EditProduct(SANPHAM sanpham, FormCollection collection)
        {
            try
            {
                // Xác định SP cần sửa trong da
                SANPHAM p = da.SANPHAMs.First(s => s.MaSP == sanpham.MaSP);

                //Thực hiện cập nhật da
                p.TenSP = sanpham.TenSP;
                p.Giaban = sanpham.Giaban;
                p.Soluongton = sanpham.Soluongton;

                //Chưa có NCC là LSP
                //Gán giá trị MaNCC, MaLSP
                p.MaLoai = int.Parse(collection["LSP"]);
                p.MaNCC = int.Parse(collection["NCC"]);

                //Cập nhật ảnh
                if (sanpham.ImageUpload != null)
                {                    
                    string fileName = Path.GetFileNameWithoutExtension(sanpham.ImageUpload.FileName); //lấy tên file ko có phần mở rộng
                    string extension = Path.GetExtension(sanpham.ImageUpload.FileName);//lấy phần mở rộng
                    fileName = fileName + extension;
                    p.AnhSP = fileName;
                    sanpham.ImageUpload.SaveAs(Path.Combine(Server.MapPath("~/Content/Images/"), fileName));
                    
                }
                //else
                //    p.AnhSP = "loi-hinh-anh.jpg"; //giữ nguyên hình ảnh ban đầu

                //Lưu xuống da
                da.SaveChanges();

                return RedirectToAction("ListProducts");
            }
            catch//chưa xử lý bắt lỗi
            {
                return View();
            }
        }

        //Giao diện SP muốn xóa
        public ActionResult DeleteProduct(int id)
        {
            // Xác định SP cần xóa trong da
            SANPHAM p = da.SANPHAMs.FirstOrDefault(s => s.MaSP == id);
            return View(p);
        }

        //Xử lý xóa SP
        [HttpPost]
        public ActionResult DeleteProduct(int id, FormCollection collection)
        {
            try
            {
                // Xác định SP cần xóa trong da
                SANPHAM p = da.SANPHAMs.FirstOrDefault(s => s.MaSP == id);
                //Thực hiện xóa
                da.SANPHAMs.Remove(p);
                //Cập nhật thay đổi da
                da.SaveChanges();
                return RedirectToAction("ListProducts");
            }
            catch//chưa xử lý bắt lỗi
            {
                return View();
            }
        }
    }
}
