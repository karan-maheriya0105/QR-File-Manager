using BrochureAPI.DTOs;

namespace BrochureAPI.Interfaces
{
    public interface IClientFormService
    {
        Task<IEnumerable<ClientFormDto>> GetAllClientFormsAsync();
        Task<ClientFormDto?> GetClientFormByIdAsync(string id);
        Task<IEnumerable<ClientFormDto>> GetClientFormsByCategoryAsync(string categoryId);
        Task<ClientFormDto> CreateClientFormAsync(CreateClientFormDto createClientFormDto);
        Task<bool> DeleteClientFormAsync(string id);
    }
} 