using repo_nha_hang_com_ga_BE.Repository;
using repo_nha_hang_com_ga_BE.Repository.Imp;

namespace repo_nha_hang_com_ga_BE.Models.Common.Services
{
    public static class MongoDbServiceExtensions
    {
        public static IServiceCollection AddMongoDbServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Cấu hình settings từ appsettings.json
            services.Configure<MongoDbSettings>(configuration.GetSection("MongoDbSettings"));

            // Đăng ký repository
            services.AddSingleton<IDanhMucNguyenLieuRepository, DanhMucNguyenLieuRepository>();
            services.AddSingleton<IDanhMucMonAnRepository, DanhMucMonAnRepository>();
            services.AddSingleton<ILoaiNguyenLieuRepository, LoaiNguyenLieuRepository>();
            services.AddSingleton<IDonViTinhRepository, DonViTinhRepository>();
            services.AddSingleton<INguyenLieuRepository, NguyenLieuRepository>();
            services.AddSingleton<ILoaiMonAnRepository, LoaiMonAnRepository>();
            services.AddSingleton<ICongThucRepository, CongThucRepository>();
            services.AddSingleton<IKhuyenMaiRepository, KhuyenMaiRepository>();
            services.AddSingleton<ILoaiKhuyenMaiRepository, LoaiKhuyenMaiRepository>();

            return services;
        }
    }
}