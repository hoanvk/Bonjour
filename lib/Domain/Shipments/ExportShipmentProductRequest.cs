using MediatR;

namespace Bonjour.Domain.Shipments;

public record ExportShipmentProductRequest(int ShipmentId) : IRequest<byte[]>;