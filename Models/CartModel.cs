using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLyThietBiDienTu6.Models
{
    public class CartModel
    {
        WebDienTuEntities da = new WebDienTuEntities();
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public string AnhBia { get; set; }
        public decimal Total { get { return UnitPrice * Quantity; } }
        public CartModel(int id)
        {
            SANPHAM sp = da.SANPHAMs.FirstOrDefault(s => s.MaSP == id);
            ProductID = sp.MaSP;
            ProductName = sp.TenSP;
            AnhBia = sp.AnhSP;
            UnitPrice = (decimal)sp.Giaban;
            Quantity = 1;
        }
    }
}