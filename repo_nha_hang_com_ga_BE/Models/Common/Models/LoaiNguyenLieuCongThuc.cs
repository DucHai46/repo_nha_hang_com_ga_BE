namespace repo_nha_hang_com_ga_BE.Models.Common.Models;

public class LoaiNguyenLieuCongThuc : IdName
{
    public List<NguyenLieuCongThuc>? nguyenLieus { get; set; }
    public string? ghiChu { get; set; }

}
