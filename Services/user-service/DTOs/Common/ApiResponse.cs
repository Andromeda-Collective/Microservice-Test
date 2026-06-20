
namespace User_Service.DTOs.Common;

public record ApiResponse<T>(
    T Data,
    string RequestId
);