using MediatR;

namespace Bonjour.Domain.Contracts;

public record ImportContractProductRequest(int ContractId, string FilePath) : IRequest<int>;