namespace repo_nha_hang_com_ga_BE.Models.Common.Models;

public class LoaiNguyenLieuCongThuc
{
    public IdName? loaiNguyenLieu { get; set; }
    public List<NguyenLieuCongThuc>? nguyenLieus { get; set; }
    public string? ghiChu { get; set; }

}
