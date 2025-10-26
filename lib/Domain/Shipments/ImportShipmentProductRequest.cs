using MediatR;

namespace Bonjour.Domain.Shipments;

public record ImportShipmentProductRequest(int ShipmentId, string FilePath) : IRequest<int>;