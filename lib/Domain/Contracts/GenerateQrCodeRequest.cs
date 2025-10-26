using MediatR;

namespace Bonjour.Domain.Contracts;

public record GenerateQrCodeRequest(int ContractId) : IRequest<int>;