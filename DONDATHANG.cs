//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace QuanLyThietBiDienTu6
{
    using System;
    using System.Collections.Generic;
    
    public partial class DONDATHANG
    {
        public DONDATHANG()
        {
            this.CHITIETDONTHANGs = new HashSet<CHITIETDONTHANG>();
        }
    
        public int MaDonHang { get; set; }
        public Nullable<bool> Dathanhtoan { get; set; }
        public Nullable<bool> Tinhtranggiaohang { get; set; }
        public Nullable<System.DateTime> Ngaydat { get; set; }
        public Nullable<System.DateTime> Ngaygiao { get; set; }
        public Nullable<int> MaKH { get; set; }
        public string MaAdmin { get; set; }
    
        public virtual Admin Admin { get; set; }
        public virtual ICollection<CHITIETDONTHANG> CHITIETDONTHANGs { get; set; }
        public virtual KHACHHANG KHACHHANG { get; set; }
    }
}
