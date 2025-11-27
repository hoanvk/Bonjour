using MediatR;

namespace Bonjour.Domain.Contracts;

public record ExportContractProductRequest(int ContractId) : IRequest<byte[]>;