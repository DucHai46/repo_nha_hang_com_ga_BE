using AutoMapper;
using repo_nha_hang_com_ga_BE.Models.MongoDB;
using repo_nha_hang_com_ga_BE.Models.Requests.DanhMucNguyenLieu;
using repo_nha_hang_com_ga_BE.Models.Requests.DonViTinh;
using repo_nha_hang_com_ga_BE.Models.Requests.LoaiNguyenLieu;
using repo_nha_hang_com_ga_BE.Models.Requests.NguyenLieu;
using repo_nha_hang_com_ga_BE.Models.Responds.DanhMucNguyenLieu;
using repo_nha_hang_com_ga_BE.Models.Responds.DonViTinh;
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
    }
}
