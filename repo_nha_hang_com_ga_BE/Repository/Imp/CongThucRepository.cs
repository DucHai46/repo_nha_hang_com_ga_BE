using AutoMapper;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using repo_nha_hang_com_ga_BE.Models.Common;
using repo_nha_hang_com_ga_BE.Models.Common.Models.Respond;
using repo_nha_hang_com_ga_BE.Models.Common.Paging;
using repo_nha_hang_com_ga_BE.Models.Common.Respond;
using repo_nha_hang_com_ga_BE.Models.MongoDB;
using repo_nha_hang_com_ga_BE.Models.Requests.CongThuc;
using repo_nha_hang_com_ga_BE.Models.Responds.CongThuc;

namespace repo_nha_hang_com_ga_BE.Repository.Imp;

public class CongThucRepository : ICongThucRepository
{
    private readonly IMongoCollection<CongThuc> _collection;
    private readonly IMapper _mapper;

    public CongThucRepository(IOptions<MongoDbSettings> settings, IMapper mapper)
    {
        var mongoClientSettings = settings.Value;
        var client = new MongoClient(mongoClientSettings.Connection);
        var database = client.GetDatabase(mongoClientSettings.DatabaseName);
        _collection = database.GetCollection<CongThuc>("CongThuc");
        _mapper = mapper;
    }

    public async Task<RespondAPIPaging<List<CongThucRespond>>> GetAllCongThucs(RequestSearchCongThuc request)
    {
        try
        {
            var collection = _collection;

            var filter = Builders<CongThuc>.Filter.Empty;
            filter &= Builders<CongThuc>.Filter.Eq(x => x.isDelete, false);

            if (!string.IsNullOrEmpty(request.tenCongThuc))
            {
                filter &= Builders<CongThuc>.Filter.Regex(x => x.tenCongThuc, new BsonRegularExpression($".*{request.tenCongThuc}.*"));
            }

            var projection = Builders<CongThuc>.Projection
                .Include(x => x.Id)
                .Include(x => x.tenCongThuc)
                .Include(x => x.nguyenLieus)
                .Include(x => x.moTa)
                .Include(x => x.hinhAnh);

            var findOptions = new FindOptions<CongThuc, CongThucRespond>
            {
                Projection = projection
            };

            if (request.IsPaging)
            {
                long totalRecords = await collection.CountDocumentsAsync(filter);

                int totalPages = (int)Math.Ceiling((double)totalRecords / request.PageSize);

                int currentPage = request.PageNumber;
                if (currentPage < 1) currentPage = 1;
                if (currentPage > totalPages) currentPage = totalPages;

                findOptions.Skip = (currentPage - 1) * request.PageSize;
                findOptions.Limit = request.PageSize;

                var cursor = await collection.FindAsync(filter, findOptions);
                var congThucs = await cursor.ToListAsync();

                var pagingDetail = new PagingDetail(currentPage, request.PageSize, totalRecords);
                var pagingResponse = new PagingResponse<List<CongThucRespond>>
                {
                    Paging = pagingDetail,
                    Data = congThucs
                };

                return new RespondAPIPaging<List<CongThucRespond>>(
                    ResultRespond.Succeeded,
                    data: pagingResponse
                );
            }
            else
            {
                var cursor = await collection.FindAsync(filter, findOptions);
                var congThucs = await cursor.ToListAsync();

                return new RespondAPIPaging<List<CongThucRespond>>(
                    ResultRespond.Succeeded,
                    data: new PagingResponse<List<CongThucRespond>>
                    {
                        Data = congThucs,
                        Paging = new PagingDetail(1, congThucs.Count, congThucs.Count)
                    }
                );
            }
        }
        catch (Exception ex)
        {
            return new RespondAPIPaging<List<CongThucRespond>>(
                ResultRespond.Error,
                message: ex.Message
            );
        }
    }

    public async Task<RespondAPI<CongThucRespond>> GetCongThucById(string id)
    {
        try
        {
            var congThuc = await _collection.Find(x => x.Id == id && x.isDelete == false).FirstOrDefaultAsync();

            if (congThuc == null)
            {
                return new RespondAPI<CongThucRespond>(
                    ResultRespond.NotFound,
                    "Không tìm thấy công thức với ID đã cung cấp."
                );
            }

            var congThucRespond = _mapper.Map<CongThucRespond>(congThuc);

            return new RespondAPI<CongThucRespond>(
                ResultRespond.Succeeded,
                "Lấy công thức thành công.",
                congThucRespond
            );
        }
        catch (Exception ex)
        {
            return new RespondAPI<CongThucRespond>(
                ResultRespond.Error,
                $"Đã xảy ra lỗi: {ex.Message}"
            );
        }
    }

    public async Task<RespondAPI<CongThucRespond>> CreateCongThuc(RequestAddCongThuc request)
    {
        try
        {
            CongThuc newCongThuc = _mapper.Map<CongThuc>(request);

            newCongThuc.createdDate = DateTimeOffset.UtcNow;
            newCongThuc.updatedDate = DateTimeOffset.UtcNow;
            newCongThuc.isDelete = false;
            // Thiết lập createdUser và updatedUser nếu có thông tin người dùng
            // newDanhMucMonAn.createdUser = currentUser.Id;
            // newDanhMucNguyenLieu.updatedUser = currentUser.Id;

            await _collection.InsertOneAsync(newCongThuc);

            var congThucRespond = _mapper.Map<CongThucRespond>(newCongThuc);

            return new RespondAPI<CongThucRespond>(
                ResultRespond.Succeeded,
                "Tạo công thức thành công.",
                congThucRespond
            );
        }
        catch (Exception ex)
        {
            return new RespondAPI<CongThucRespond>(
                ResultRespond.Error,
                $"Đã xảy ra lỗi khi tạo công thức: {ex.Message}"
            );
        }
    }

    public async Task<RespondAPI<CongThucRespond>> UpdateCongThuc(string id, RequestUpdateCongThuc request)
    {
        try
        {
            var filter = Builders<CongThuc>.Filter.Eq(x => x.Id, id);
            filter &= Builders<CongThuc>.Filter.Eq(x => x.isDelete, false);
            var congThuc = await _collection.Find(filter).FirstOrDefaultAsync();

            if (congThuc == null)
            {
                return new RespondAPI<CongThucRespond>(
                    ResultRespond.NotFound,
                    "Không tìm thấy công thức với ID đã cung cấp."
                );
            }

            _mapper.Map(request, congThuc);

            congThuc.updatedDate = DateTimeOffset.UtcNow;

            // Cập nhật người dùng nếu có thông tin
            // danhMucNguyenLieu.updatedUser = currentUser.Id;

            var updateResult = await _collection.ReplaceOneAsync(filter, congThuc);

            if (!updateResult.IsAcknowledged || updateResult.ModifiedCount == 0)
            {
                return new RespondAPI<CongThucRespond>(
                    ResultRespond.Error,
                    "Cập nhật công thức không thành công."
                );
            }

            var congThucRespond = _mapper.Map<CongThucRespond>(congThuc);

            return new RespondAPI<CongThucRespond>(
                ResultRespond.Succeeded,
                "Cập nhật công thức thành công.",
                congThucRespond
            );
        }
        catch (Exception ex)
        {
            return new RespondAPI<CongThucRespond>(
                ResultRespond.Error,
                $"Đã xảy ra lỗi khi cập nhật công thức: {ex.Message}"
            );
        }
    }

    public async Task<RespondAPI<string>> DeleteCongThuc(string id)
    {
        try
        {
            var existingCongThuc = await _collection.Find(x => x.Id == id && x.isDelete == false).FirstOrDefaultAsync();
            if (existingCongThuc == null)
            {
                return new RespondAPI<string>(
                    ResultRespond.NotFound,
                    "Không tìm thấy công thức để xóa."
                );
            }

            var deleteResult = await _collection.DeleteOneAsync(x => x.Id == id);

            if (deleteResult.DeletedCount == 0)
            {
                return new RespondAPI<string>(
                    ResultRespond.Error,
                    "Xóa công thức không thành công."
                );
            }

            return new RespondAPI<string>(
                ResultRespond.Succeeded,
                "Xóa công thức thành công.",
                id
            );
        }
        catch (Exception ex)
        {
            return new RespondAPI<string>(
                ResultRespond.Error,
                $"Đã xảy ra lỗi khi xóa công thức: {ex.Message}"
            );
        }
    }
}