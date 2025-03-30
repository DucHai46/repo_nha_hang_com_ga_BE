using repo_nha_hang_com_ga_BE.Models.Common;

namespace repo_nha_hang_com_ga_BE.Models.MongoDB;

public class ThucDon : BaseMongoDb
{
    public string? tenThucDon { get; set; }
    public List<LoaiMonAnMenu>? loaiMonAns { get; set; }
    public List<ComboMenu>? combos { get; set; }
}

public class LoaiMonAnMenu
{
    public string? id { get; set; }
    public string? tenLoai { get; set; }
    public List<MonAnMenu>? monAns { get; set; }
    public string? moTa { get; set; }
}

public class ComboMenu
{
    public string? id { get; set; }
    public string? tenCombo { get; set; }
    public List<MonAnMenu>? monAns { get; set; }
    public string? hinhAnh { get; set; }
    public string? giaTien { get; set; }
    public string? moTa { get; set; }
}


