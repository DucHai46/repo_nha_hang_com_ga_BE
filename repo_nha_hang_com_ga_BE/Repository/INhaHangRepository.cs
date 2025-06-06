﻿using repo_nha_hang_com_ga_BE.Models.Common.Models.Respond;
using repo_nha_hang_com_ga_BE.Models.Common.Respond;
using repo_nha_hang_com_ga_BE.Models.Requests.NhaHang;
using repo_nha_hang_com_ga_BE.Models.Responds.NhaHang;
using repo_nha_hang_com_ga_BE.Models.MongoDB;

namespace repo_nha_hang_com_ga_BE.Repository;

public interface INhaHangRepository
{
    Task<RespondAPIPaging<List<NhaHangRespond>>> GetAllNhaHangs(RequestSearchNhaHang request);
    Task<RespondAPI<NhaHangRespond>> GetNhaHangById(string id);
    Task<RespondAPI<NhaHangRespond>> CreateNhaHang(RequestAddNhaHang product);
    Task<RespondAPI<NhaHangRespond>> UpdateNhaHang(string id, RequestUpdateNhaHang product);
    Task<RespondAPI<string>> DeleteNhaHang(string id);
    Task<RespondAPI<GiaoDienNhaHang>> GetGiaoDienNhaHangById(string? id, bool? isActive);
    Task<RespondAPI<GiaoDienNhaHang>> CreateGiaoDienNhaHang(string id, RequestAddGiaoDienNhaHang request);
    Task<RespondAPI<GiaoDienNhaHang>> UpdateGiaoDienNhaHang(string id, RequestUpdateGiaoDienNhaHang request);
}