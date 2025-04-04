﻿using repo_nha_hang_com_ga_BE.Models.Common.Models;
using repo_nha_hang_com_ga_BE.Models.Common.Models.Request;

namespace repo_nha_hang_com_ga_BE.Models.Requests.NguyenLieu;

public class RequestSearchNguyenLieu : PagingParameterModel
{
    public string? tenNguyenLieu { get; set; }

    public DateTimeOffset? hanSuDung { get; set; }

    public string? loaiNguyenLieuId { get; set; }

    public string? donViTinhId { get; set; }
}