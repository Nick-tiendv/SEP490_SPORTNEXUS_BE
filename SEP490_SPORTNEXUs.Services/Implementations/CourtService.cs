using SEP490_SPORTNEXUS_BE.Repositories.Entities.Facilities;
using SEP490_SPORTNEXUS_BE.Repositories.Repository;
using SEP490_SPORTNEXUS_BE.Services.IServices;
using SEP490_SPORTNEXUS_BE.Services.RequestModel;
using SEP490_SPORTNEXUS_BE.Services.ResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SEP490_SPORTNEXUS_BE.Services.Implementations
{
    public class CourtService : ICourtService
    {
        private readonly ICourtRepository _courtRepo;
        private readonly IFacilityRepository _facilityRepo;

        public CourtService(ICourtRepository courtRepo, IFacilityRepository facilityRepo)
        {
            _courtRepo = courtRepo;
            _facilityRepo = facilityRepo;
        }

        public async Task<ApiResponse<IEnumerable<CourtResponse>>> GetCourtsByFacilityAsync(Guid facilityId)
        {
            var courts = await _courtRepo.GetCourtsByFacilityAsync(facilityId);
            var mapped = courts.Select(c => new CourtResponse
            {
                Id = c.Id, FacilityId = c.FacilityId, CategoryId = c.CategoryId,
                CategoryName = c.Category?.Name ?? "", Name = c.Name, Status = c.Status, DefaultPrice = c.DefaultPrice
            });
            return new ApiResponse<IEnumerable<CourtResponse>> { StatusCode = 200, Message = "Success", Data = mapped };
        }

        public async Task<ApiResponse<CourtResponse?>> CreateCourtAsync(Guid facilityId, Guid ownerId, CreateCourtRequest request)
        {
            var facility = await _facilityRepo.GetByIdAsync(facilityId);
            if (facility == null) return new ApiResponse<CourtResponse?> { StatusCode = 404, Message = "Facility not found", Data = null };
            if (facility.OwnerId != ownerId) return new ApiResponse<CourtResponse?> { StatusCode = 403, Message = "Forbidden", Data = null };

            var court = new Court
            {
                Id = Guid.NewGuid(),
                FacilityId = facilityId,
                CategoryId = request.CategoryId,
                Name = request.Name,
                DefaultPrice = request.DefaultPrice
            };
            
            await _courtRepo.AddAsync(court);
            await _courtRepo.SaveChangesAsync();
            // Re-fetch to get category name
            var created = (await _courtRepo.GetCourtsByFacilityAsync(facilityId)).FirstOrDefault(c => c.Id == court.Id);
            
            return new ApiResponse<CourtResponse?> { StatusCode = 201, Message = "Created", Data = new CourtResponse
            {
                Id = created!.Id, FacilityId = created.FacilityId, CategoryId = created.CategoryId,
                CategoryName = created.Category?.Name ?? "", Name = created.Name, Status = created.Status, DefaultPrice = created.DefaultPrice
            }};
        }

        public async Task<ApiResponse<CourtResponse?>> UpdateCourtAsync(Guid courtId, Guid ownerId, UpdateCourtRequest request)
        {
            var court = await _courtRepo.GetByIdAsync(courtId);
            if (court == null) return new ApiResponse<CourtResponse?> { StatusCode = 404, Message = "Court not found", Data = null };
            
            var facility = await _facilityRepo.GetByIdAsync(court.FacilityId);
            if (facility == null || facility.OwnerId != ownerId) return new ApiResponse<CourtResponse?> { StatusCode = 403, Message = "Forbidden", Data = null };

            if (request.CategoryId.HasValue) court.CategoryId = request.CategoryId.Value;
            if (!string.IsNullOrEmpty(request.Name)) court.Name = request.Name;
            if (!string.IsNullOrEmpty(request.Status)) court.Status = request.Status;
            if (request.DefaultPrice.HasValue) court.DefaultPrice = request.DefaultPrice.Value;

            _courtRepo.Update(court);
            await _courtRepo.SaveChangesAsync();
            var updated = (await _courtRepo.GetCourtsByFacilityAsync(court.FacilityId)).FirstOrDefault(c => c.Id == courtId);

            return new ApiResponse<CourtResponse?> { StatusCode = 200, Message = "Updated", Data = new CourtResponse
            {
                Id = updated!.Id, FacilityId = updated.FacilityId, CategoryId = updated.CategoryId,
                CategoryName = updated.Category?.Name ?? "", Name = updated.Name, Status = updated.Status, DefaultPrice = updated.DefaultPrice
            }};
        }

        public async Task<ApiResponse<object?>> DeleteCourtAsync(Guid courtId, Guid ownerId)
        {
            var court = await _courtRepo.GetByIdAsync(courtId);
            if (court == null) return new ApiResponse<object?> { StatusCode = 404, Message = "Court not found", Data = null };
            
            var facility = await _facilityRepo.GetByIdAsync(court.FacilityId);
            if (facility == null || facility.OwnerId != ownerId) return new ApiResponse<object?> { StatusCode = 403, Message = "Forbidden", Data = null };

            _courtRepo.Delete(court);
            await _courtRepo.SaveChangesAsync();
            return new ApiResponse<object?> { StatusCode = 200, Message = "Deleted", Data = null };
        }
    }
}
