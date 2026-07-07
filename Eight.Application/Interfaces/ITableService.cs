using Eight.Application.DTOs.Table;
using Eight.Domain.Enums;

namespace Eight.Application.Interfaces;

public interface ITableService
{
    Task<List<TableResponse>> GetByVenueAsync(Guid venueId);
    Task<TableResponse> CreateAsync(TableRequest request);
    Task<TableResponse> UpdateAsync(Guid id, TableRequest request);
    Task SetStatusAsync(Guid id, TableStatus status); 
    Task DeleteAsync(Guid id);

}