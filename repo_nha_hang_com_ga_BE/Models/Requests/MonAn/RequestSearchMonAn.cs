using repo_nha_hang_com_ga_BE.Models.Common.Models.Request;

namespace repo_nha_hang_com_ga_BE.Models.Requests.MonAn;

public class RequestSearchMonAn : PagingParameterModel
{
    public string? tenMonAn { get; set; }

    public string? tenLoaiMonAn { get; set; }

    public string? tenCongThuc { get; set; }

    public string? giaTien { get; set; }
}
