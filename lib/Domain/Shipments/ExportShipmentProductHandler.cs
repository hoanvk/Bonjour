using Bonjour.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using OfficeOpenXml;

namespace Bonjour.Domain.Shipments;

public class ExportShipmentProductHandler : IRequestHandler<ExportShipmentProductRequest, byte[]>
{
    private readonly ApplicationDbContext dbContext;
    private readonly IStringLocalizer<SharedResources> localizer;
    public ExportShipmentProductHandler(ApplicationDbContext dbContext, IStringLocalizer<SharedResources> localizer)
    {
        this.dbContext = dbContext;
        this.localizer = localizer;
    }

    public async Task<byte[]> Handle(ExportShipmentProductRequest request, CancellationToken cancellationToken)
    {
        ExcelPackage.License.SetNonCommercialPersonal("Bonjour");
        var _shipmentProducts = await dbContext.ShipmentProducts.Join(dbContext.Products,
         t1 => t1.ProductId,
         t2 => t2.Id,
         (t1, t2) => new { ShipmentProduct = t1, Product = t2 }).Where(m => m.ShipmentProduct.ShipmentId == request.ShipmentId).Select(m => new
         {
             m.Product.Code,
             m.Product.Name,
             m.ShipmentProduct.Loaded,
             m.ShipmentProduct.Unloaded,
             m.Product.Weight
         }).ToListAsync();
        using (var package = new ExcelPackage())
        {
            var worksheet = package.Workbook.Worksheets.Add("Products");
            // Add headers to the first row
            worksheet.Cells[1, 1].Value = localizer["Code"];
            worksheet.Cells[1, 2].Value = localizer["Name"];
            worksheet.Cells[1, 3].Value = localizer["Loaded"];
            worksheet.Cells[1, 4].Value = localizer["LoadedWeight"];
            worksheet.Cells[1, 5].Value = localizer["Unloaded"];
            worksheet.Cells[1, 6].Value = localizer["UnloadedWeight"];
            int row = 2;
            foreach (var _shipmentProduct in _shipmentProducts)
            {
                // Add data to subsequent rows
                worksheet.Cells[row, 1].Value = _shipmentProduct.Code;
                worksheet.Cells[row, 2].Value = _shipmentProduct.Name;
                worksheet.Cells[row, 3].Value = _shipmentProduct.Loaded;
                worksheet.Cells[row, 4].Value = _shipmentProduct.Weight * _shipmentProduct.Loaded;
                worksheet.Cells[row, 5].Value = _shipmentProduct.Unloaded;
                worksheet.Cells[row, 6].Value = _shipmentProduct.Weight * _shipmentProduct.Unloaded;
                row++;
            }
            // Auto-fit columns for better readability
            worksheet.Cells.AutoFitColumns();
            return package.GetAsByteArray();
        }
    }
}