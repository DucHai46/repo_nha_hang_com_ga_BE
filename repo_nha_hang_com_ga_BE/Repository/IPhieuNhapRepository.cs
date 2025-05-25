using repo_nha_hang_com_ga_BE.Models.Common.Models.Respond;
using repo_nha_hang_com_ga_BE.Models.Common.Respond;
using repo_nha_hang_com_ga_BE.Models.MongoDB;
using repo_nha_hang_com_ga_BE.Models.Requests.PhieuNhap;
using repo_nha_hang_com_ga_BE.Models.Responds.PhieuNhap;

namespace repo_nha_hang_com_ga_BE.Repository;

public interface IPhieuNhapRepository
{
    Task<RespondAPIPaging<List<PhieuNhapRespond>>> GetAllPhieuNhaps(RequestSearchPhieuNhap request);
    Task<RespondAPI<PhieuNhapRespond>> GetPhieuNhapById(string id);
    Task<RespondAPI<PhieuNhapRespond>> CreatePhieuNhap(RequestAddPhieuNhap product);
    Task<RespondAPI<string>> DeletePhieuNhap(string id);
}