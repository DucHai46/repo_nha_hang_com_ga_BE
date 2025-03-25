﻿using AutoMapper;
using repo_nha_hang_com_ga_BE.Models.Common.Models;
using repo_nha_hang_com_ga_BE.Models.MongoDB;
using repo_nha_hang_com_ga_BE.Models.Requests.CongThuc;
using repo_nha_hang_com_ga_BE.Models.Requests.DanhMucMonAn;
using repo_nha_hang_com_ga_BE.Models.Requests.DanhMucNguyenLieu;
using repo_nha_hang_com_ga_BE.Models.Requests.DonViTinh;
using repo_nha_hang_com_ga_BE.Models.Requests.KhuyenMai;
using repo_nha_hang_com_ga_BE.Models.Requests.LoaiKhuyenMai;
using repo_nha_hang_com_ga_BE.Models.Requests.LoaiMonAn;
using repo_nha_hang_com_ga_BE.Models.Requests.LoaiNguyenLieu;
using repo_nha_hang_com_ga_BE.Models.Requests.NguyenLieu;
using repo_nha_hang_com_ga_BE.Models.Responds.Common;
using repo_nha_hang_com_ga_BE.Models.Responds.CongThuc;
using repo_nha_hang_com_ga_BE.Models.Responds.DanhMucMonAn;
using repo_nha_hang_com_ga_BE.Models.Responds.DanhMucNguyenLieu;
using repo_nha_hang_com_ga_BE.Models.Responds.DonViTinh;
using repo_nha_hang_com_ga_BE.Models.Responds.KhuyenMai;
using repo_nha_hang_com_ga_BE.Models.Responds.LoaiKhuyenMai;
using repo_nha_hang_com_ga_BE.Models.Responds.LoaiMonAn;
using repo_nha_hang_com_ga_BE.Models.Responds.LoaiNguyenLieu;
using repo_nha_hang_com_ga_BE.Models.Responds.NguyenLieu;

namespace repo_nha_hang_com_ga_BE.Models.Common.Services;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        //Danh mục nguyên liệu
        CreateMap(typeof(DanhMucNguyenLieu), typeof(DanhMucNguyenLieuRespond));
        CreateMap(typeof(RequestAddDanhMucNguyenLieu), typeof(DanhMucNguyenLieu));
        CreateMap(typeof(RequestUpdateDanhMucNguyenLieu), typeof(DanhMucNguyenLieu));


        //Loại nguyên liệu
        CreateMap(typeof(LoaiNguyenLieu), typeof(LoaiNguyenLieuRespond));
        CreateMap(typeof(RequestAddLoaiNguyenLieu), typeof(LoaiNguyenLieu));
        CreateMap(typeof(RequestUpdateLoaiNguyenLieu), typeof(LoaiNguyenLieu));

        //Đơn vị tính
        CreateMap(typeof(DonViTinh), typeof(DonViTinhRespond));
        CreateMap(typeof(RequestAddDonViTinh), typeof(DonViTinh));
        CreateMap(typeof(RequestUpdateDonViTinh), typeof(DonViTinh));

        //Nguyên liệu
        CreateMap(typeof(NguyenLieu), typeof(NguyenLieuRespond));
        CreateMap(typeof(RequestAddNguyenLieu), typeof(NguyenLieu));
        CreateMap(typeof(RequestUpdateNguyenLieu), typeof(NguyenLieu));

        //Danh mục món ăn
        CreateMap(typeof(DanhMucMonAn), typeof(DanhMucMonAnRespond));
        CreateMap(typeof(RequestAddDanhMucMonAn), typeof(DanhMucMonAn));
        CreateMap(typeof(RequestUpdateDanhMucMonAn), typeof(DanhMucMonAn));

        //Loại món ăn
        CreateMap(typeof(LoaiMonAn), typeof(LoaiMonAnRespond));
        CreateMap(typeof(RequestAddLoaiMonAn), typeof(LoaiMonAn));
        CreateMap(typeof(RequestUpdateLoaiMonAn), typeof(LoaiMonAn));

        //Công thức
        CreateMap(typeof(CongThuc), typeof(CongThucRespond));
        CreateMap(typeof(RequestAddCongThuc), typeof(CongThuc));
        CreateMap(typeof(RequestUpdateCongThuc), typeof(CongThuc));

        //NguyenLieuCongThuc
        CreateMap(typeof(NguyenLieuCongThuc), typeof(NguyenLieuCongThucRespond));

        //Khuyến mãi
        CreateMap(typeof(KhuyenMai), typeof(KhuyenMaiRespond));
        CreateMap(typeof(RequestAddKhuyenMai), typeof(KhuyenMai));
        CreateMap(typeof(RequestUpdateKhuyenMai), typeof(KhuyenMai));

        //Loại khuyến mãi
        CreateMap(typeof(LoaiKhuyenMai), typeof(LoaiKhuyenMaiRespond));
        CreateMap(typeof(RequestAddLoaiKhuyenMai), typeof(LoaiKhuyenMai));
        CreateMap(typeof(RequestUpdateLoaiKhuyenMai), typeof(LoaiKhuyenMai));
    }
}
