﻿using repo_nha_hang_com_ga_BE.Models.Common.Models.Request;

namespace repo_nha_hang_com_ga_BE.Models.Requests.DonViTinh;

public class RequestSearchDonViTinh : PagingParameterModel
{
    public string? tenDonViTinh { get; set; }
}