using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;

namespace QuanLyThietBiDienTu6.Controllers
{
    public class HomeController : Controller
    {
        WebDienTuEntities da = new WebDienTuEntities();
        public ActionResult Index(int? page)
        {
            if (page == null)
                page = 1;
            var ds = da.SANPHAMs.OrderBy(s => s.MaSP);
            int pageSize = 6;
            int pageNumber = (page ?? 1);// nếu page = null thì lấy giá trị 1 cho biến pageNumber.
            return View(ds.ToPagedList(pageNumber, pageSize));
        }

        //Hiển thị chi tiết sp
        public ActionResult ProductDetail(int id)
        {
            SANPHAM p = da.SANPHAMs.FirstOrDefault(s => s.MaSP == id);
            ViewBag.Id = id;
            return View(p);
        }

        //Xem sản phẩm theo danh mục
        public ActionResult ProductType(int id)
        {
            ViewBag.ID = id;
            List<SANPHAM> ds = da.SANPHAMs.Where(s => s.MaLoai == id).ToList();
            return View(ds);
        }
    }
}