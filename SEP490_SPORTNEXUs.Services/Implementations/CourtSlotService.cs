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
    public class CourtSlotService : ICourtSlotService
    {
        private readonly ICourtSlotRepository _slotRepo;
        private readonly ICourtRepository _courtRepo;
        private readonly IFacilityRepository _facilityRepo;

        public CourtSlotService(ICourtSlotRepository slotRepo, ICourtRepository courtRepo, IFacilityRepository facilityRepo)
        {
            _slotRepo = slotRepo;
            _courtRepo = courtRepo;
            _facilityRepo = facilityRepo;
        }

        public async Task<ApiResponse<object?>> GenerateCourtSlotsAsync(Guid courtId, Guid ownerId, GenerateCourtSlotsRequest request)
        {
            var court = await _courtRepo.GetByIdAsync(courtId);
            if (court == null) return new ApiResponse<object?> { StatusCode = 404, Message = "Court not found", Data = null };
            
            var facility = await _facilityRepo.GetByIdAsync(court.FacilityId);
            if (facility == null || facility.OwnerId != ownerId) return new ApiResponse<object?> { StatusCode = 403, Message = "Forbidden", Data = null };

            var newSlots = new List<CourtSlot>();

            // Normalize dates to UTC start of day to avoid timezone confusion during loop
            DateTime currentDate = request.DateFrom.Date;
            DateTime endDate = request.DateTo.Date;

            while (currentDate <= endDate)
            {
                TimeSpan currentTime = request.OpenTime;
                while (currentTime.Add(TimeSpan.FromMinutes(request.DurationMinutes)) <= request.CloseTime)
                {
                    // Create DateTime as UTC
                    DateTime startDateTime = DateTime.SpecifyKind(currentDate.Add(currentTime), DateTimeKind.Utc);
                    DateTime endDateTime = startDateTime.AddMinutes(request.DurationMinutes);

                    // Optional: check overlap per slot or just assume owner knows what they are doing.
                    // For performance, we can skip individual overlap checks if we clear existing slots, 
                    // or we just do a bulk overlap check.
                    
                    newSlots.Add(new CourtSlot
                    {
                        Id = Guid.NewGuid(),
                        CourtId = courtId,
                        StartTime = startDateTime,
                        EndTime = endDateTime,
                        Price = request.Price,
                        Status = Repositories.Enums.CourtSlotStatus.Available
                    });

                    currentTime = currentTime.Add(TimeSpan.FromMinutes(request.DurationMinutes));
                }
                currentDate = currentDate.AddDays(1);
            }

            if (newSlots.Any())
            {
                await _slotRepo.BulkInsertSlotsAsync(newSlots);
            }

            return new ApiResponse<object?> { StatusCode = 201, Message = $"Generated {newSlots.Count} slots", Data = null };
        }

        public async Task<ApiResponse<IEnumerable<CourtSlotResponse>>> GetTimeGridAsync(Guid courtId, DateTime date)
        {
            // Convert to UTC range for the given day
            DateTime startOfDay = DateTime.SpecifyKind(date.Date, DateTimeKind.Utc);
            DateTime endOfDay = startOfDay.AddDays(1).AddTicks(-1);

            var slots = await _slotRepo.GetSlotsByDateRangeAsync(courtId, startOfDay, endOfDay);
            
            var mapped = slots.Select(s => new CourtSlotResponse
            {
                Id = s.Id,
                CourtId = s.CourtId,
                StartTime = s.StartTime,
                EndTime = s.EndTime,
                Price = s.Price,
                Status = s.Status.ToString()
            });

            if (!mapped.Any()) return new ApiResponse<IEnumerable<CourtSlotResponse>> { StatusCode = 200, Message = "Không có ca trống nào trong ngày này", Data = mapped };
            return new ApiResponse<IEnumerable<CourtSlotResponse>> { StatusCode = 200, Message = "Success", Data = mapped };
        }

        public async Task<ApiResponse<CourtSlotResponse?>> UpdateSlotStatusAsync(Guid slotId, Guid ownerId, UpdateSlotStatusRequest request)
        {
            var slot = await _slotRepo.GetByIdAsync(slotId);
            if (slot == null) return new ApiResponse<CourtSlotResponse?> { StatusCode = 404, Message = "Slot not found", Data = null };

            var court = await _courtRepo.GetByIdAsync(slot.CourtId);
            var facility = await _facilityRepo.GetByIdAsync(court!.FacilityId);
            if (facility!.OwnerId != ownerId) return new ApiResponse<CourtSlotResponse?> { StatusCode = 403, Message = "Forbidden", Data = null };

            slot.Status = request.Status;
            if (request.Price.HasValue) slot.Price = request.Price.Value;

            _slotRepo.Update(slot);
            await _slotRepo.SaveChangesAsync();

            var res = new CourtSlotResponse
            {
                Id = slot.Id, CourtId = slot.CourtId, StartTime = slot.StartTime, EndTime = slot.EndTime, Price = slot.Price, Status = slot.Status.ToString()
            };
            return new ApiResponse<CourtSlotResponse?> { StatusCode = 200, Message = "Updated", Data = res };
        }
    }
}
