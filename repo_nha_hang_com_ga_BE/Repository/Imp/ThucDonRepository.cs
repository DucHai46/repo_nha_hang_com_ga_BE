using AutoMapper;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using repo_nha_hang_com_ga_BE.Models.Common;
using repo_nha_hang_com_ga_BE.Models.Common.Models.Respond;
using repo_nha_hang_com_ga_BE.Models.Common.Paging;
using repo_nha_hang_com_ga_BE.Models.Common.Respond;
using repo_nha_hang_com_ga_BE.Models.MongoDB;
using repo_nha_hang_com_ga_BE.Models.Requests.ThucDon;
using repo_nha_hang_com_ga_BE.Models.Responds.ThucDon;

namespace repo_nha_hang_com_ga_BE.Repository.Imp;

public class ThucDonRepository : IThucDonRepository
{
    private readonly IMongoCollection<ThucDon> _collection;
    private readonly IMapper _mapper;

    public ThucDonRepository(IOptions<MongoDbSettings> settings, IMapper mapper)
    {
        var mongoClientSettings = settings.Value;
        var client = new MongoClient(mongoClientSettings.Connection);
        var database = client.GetDatabase(mongoClientSettings.DatabaseName);
        _collection = database.GetCollection<ThucDon>("ThucDon");
        _mapper = mapper;
    }

    public async Task<RespondAPIPaging<List<ThucDonRespond>>> GetAllThucDons(RequestSearchThucDon request)
    {
        try
        {
            var collection = _collection;

            var filter = Builders<ThucDon>.Filter.Empty;
            filter &= Builders<ThucDon>.Filter.Eq(x => x.isDelete, false);

            if (!string.IsNullOrEmpty(request.tenThucDon))
            {
                filter &= Builders<ThucDon>.Filter.Regex(x => x.tenThucDon, new BsonRegularExpression($".*{request.tenThucDon}.*"));
            }

            if (!string.IsNullOrEmpty(request.loaiMonAnId))
            {
                filter &= Builders<ThucDon>.Filter.AnyEq("loaiMonAns.Id", request.loaiMonAnId);
            }

            if (!string.IsNullOrEmpty(request.comboId))
            {
                filter &= Builders<ThucDon>.Filter.ElemMatch(x => x.combos, Builders<ComboMenu>.Filter.Eq(y => y.Id, request.comboId));
            }

            if (request.trangThai.HasValue)
            {
                filter &= Builders<ThucDon>.Filter.Eq(x => x.trangThai, request.trangThai);
            }

            var projection = Builders<ThucDon>.Projection
                .Include(x => x.Id)
                .Include(x => x.tenThucDon)
                .Include(x => x.loaiMonAns)
                .Include(x => x.combos)
                .Include(x => x.trangThai);

            var findOptions = new FindOptions<ThucDon, ThucDonRespond>
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
                var thucDons = await cursor.ToListAsync();

                var pagingDetail = new PagingDetail(currentPage, request.PageSize, totalRecords);
                var pagingResponse = new PagingResponse<List<ThucDonRespond>>
                {
                    Paging = pagingDetail,
                    Data = thucDons
                };

                return new RespondAPIPaging<List<ThucDonRespond>>(
                    ResultRespond.Succeeded,
                    data: pagingResponse
                );
            }
            else
            {
                var cursor = await collection.FindAsync(filter, findOptions);
                var bans = await cursor.ToListAsync();

                return new RespondAPIPaging<List<ThucDonRespond>>(
                    ResultRespond.Succeeded,
                    data: new PagingResponse<List<ThucDonRespond>>
                    {
                        Data = bans,
                        Paging = new PagingDetail(1, bans.Count, bans.Count)
                    }
                );
            }
        }
        catch (Exception ex)
        {
            return new RespondAPIPaging<List<ThucDonRespond>>(
                ResultRespond.Error,
                message: ex.Message
            );
        }
    }

    public async Task<RespondAPI<ThucDonRespond>> GetThucDonById(string id)
    {
        try
        {
            var thucDon = await _collection.Find(x => x.Id == id && x.isDelete == false).FirstOrDefaultAsync();

            if (thucDon == null)
            {
                return new RespondAPI<ThucDonRespond>(
                    ResultRespond.NotFound,
                    "Không tìm thấy thực đơn với ID đã cung cấp."
                );
            }

            var thucDonRespond = _mapper.Map<ThucDonRespond>(thucDon);

            return new RespondAPI<ThucDonRespond>(
                ResultRespond.Succeeded,
                "Lấy thực đơn thành công.",
                thucDonRespond
            );
        }
        catch (Exception ex)
        {
            return new RespondAPI<ThucDonRespond>(
                ResultRespond.Error,
                $"Đã xảy ra lỗi: {ex.Message}"
            );
        }
    }

    public async Task<RespondAPI<ThucDonRespond>> CreateThucDon(RequestAddThucDon request)
    {
        try
        {
            ThucDon newThucDon = _mapper.Map<ThucDon>(request);

            newThucDon.createdDate = DateTimeOffset.UtcNow;
            newThucDon.updatedDate = DateTimeOffset.UtcNow;
            newThucDon.isDelete = false;
            // Thiết lập createdUser và updatedUser nếu có thông tin người dùng
            // newDanhMucMonAn.createdUser = currentUser.Id;
            // newDanhMucNguyenLieu.updatedUser = currentUser.Id;

            await _collection.InsertOneAsync(newThucDon);

            var thucDonRespond = _mapper.Map<ThucDonRespond>(newThucDon);

            return new RespondAPI<ThucDonRespond>(
                ResultRespond.Succeeded,
                "Tạo thực đơn thành công.",
                thucDonRespond
            );
        }
        catch (Exception ex)
        {
            return new RespondAPI<ThucDonRespond>(
                ResultRespond.Error,
                $"Đã xảy ra lỗi khi tạo thực đơn: {ex.Message}"
            );
        }
    }

    public async Task<RespondAPI<ThucDonRespond>> UpdateThucDon(string id, RequestUpdateThucDon request)
    {
        try
        {
            var filter = Builders<ThucDon>.Filter.Eq(x => x.Id, id);
            filter &= Builders<ThucDon>.Filter.Eq(x => x.isDelete, false);
            var thucDon = await _collection.Find(filter).FirstOrDefaultAsync();

            if (thucDon == null)
            {
                return new RespondAPI<ThucDonRespond>(
                    ResultRespond.NotFound,
                    "Không tìm thấy thực đơn với ID đã cung cấp."
                );
            }

            _mapper.Map(request, thucDon);

            thucDon.updatedDate = DateTimeOffset.UtcNow;

            // Cập nhật người dùng nếu có thông tin
            // danhMucNguyenLieu.updatedUser = currentUser.Id;

            var updateResult = await _collection.ReplaceOneAsync(filter, thucDon);

            if (!updateResult.IsAcknowledged || updateResult.ModifiedCount == 0)
            {
                return new RespondAPI<ThucDonRespond>(
                    ResultRespond.Error,
                    "Cập nhật thực đơn không thành công."
                );
            }

            var thucDonRespond = _mapper.Map<ThucDonRespond>(thucDon);

            return new RespondAPI<ThucDonRespond>(
                ResultRespond.Succeeded,
                "Cập nhật thực đơn thành công.",
                thucDonRespond
            );
        }
        catch (Exception ex)
        {
            return new RespondAPI<ThucDonRespond>(
                ResultRespond.Error,
                $"Đã xảy ra lỗi khi cập nhật thực đơn: {ex.Message}"
            );
        }
    }

    public async Task<RespondAPI<string>> DeleteThucDon(string id)
    {
        try
        {
            var existingThucDon = await _collection.Find(x => x.Id == id && x.isDelete == false).FirstOrDefaultAsync();
            if (existingThucDon == null)
            {
                return new RespondAPI<string>(
                    ResultRespond.NotFound,
                    "Không tìm thấy thực đơn để xóa."
                );
            }

            var deleteResult = await _collection.DeleteOneAsync(x => x.Id == id);

            if (deleteResult.DeletedCount == 0)
            {
                return new RespondAPI<string>(
                    ResultRespond.Error,
                    "Xóa thực đơn không thành công."
                );
            }

            return new RespondAPI<string>(
                ResultRespond.Succeeded,
                "Xóa thực đơn thành công.",
                id
            );
        }
        catch (Exception ex)
        {
            return new RespondAPI<string>(
                ResultRespond.Error,
                $"Đã xảy ra lỗi khi xóa thực đơn: {ex.Message}"
            );
        }
    }
}