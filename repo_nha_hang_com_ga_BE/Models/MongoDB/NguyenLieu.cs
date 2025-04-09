using repo_nha_hang_com_ga_BE.Models.Common;
using repo_nha_hang_com_ga_BE.Models.Common.Models;

namespace repo_nha_hang_com_ga_BE.Models.MongoDB;

public class NguyenLieu : BaseMongoDb
{
    public string? tenNguyenLieu { get; set; }
    public DateTimeOffset? hanSuDung { get; set; }
    public string? moTa { get; set; }
    public IdName? loaiNguyenLieu { get; set; }
    public IdName? donViTinh { get; set; }

    public IdName? tuDo { get; set; }
}

