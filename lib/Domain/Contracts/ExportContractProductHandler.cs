using Bonjour.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using OfficeOpenXml;

namespace Bonjour.Domain.Contracts;

public class ExportContractProductHandler : IRequestHandler<ExportContractProductRequest, byte[]>
{
    private readonly ApplicationDbContext dbContext;
    private readonly IStringLocalizer<SharedResources> localizer;
    public ExportContractProductHandler(ApplicationDbContext dbContext, IStringLocalizer<SharedResources> localizer)
    {
        this.dbContext = dbContext;
        this.localizer = localizer;
    }

    public async Task<byte[]> Handle(ExportContractProductRequest request, CancellationToken cancellationToken)
    {
        ExcelPackage.License.SetNonCommercialPersonal("Bonjour");
        var _shipmentProducts = await dbContext.ShipmentProducts.Join(dbContext.Products,
         t1 => t1.ProductId,
         t2 => t2.Id,
         (t1, t2) => new { ShipmentProduct = t1, Product = t2 })
         .Join(dbContext.Contracts, t => t.Product.ContractId, t3 => t3.Id, (t, t3) => new { t.ShipmentProduct, t.Product, Contract = t3 })
         .Join(dbContext.Shipments, t => t.ShipmentProduct.ShipmentId, t4 => t4.Id, (t, t4) => new { t.ShipmentProduct, t.Product, t.Contract, Shipment = t4 })
         .Where(m => m.Contract.Id == request.ContractId).Select(m => new
         {
             Contract = m.Contract.Name,
             m.Product.Category,
             m.Product.Name,
             m.ShipmentProduct.Loaded,
             m.ShipmentProduct.Unloaded,
             m.Product.Weight,
             m.Shipment.Departure
         }).ToListAsync();
        using (var package = new ExcelPackage())
        {
            var worksheet = package.Workbook.Worksheets.Add("Products");
            // Add headers to the first row
            worksheet.Cells[1, 1].Value = localizer["Contract"];
            worksheet.Cells[1, 2].Value = localizer["Category"];
            worksheet.Cells[1, 3].Value = localizer["Name"];
            worksheet.Cells[1, 4].Value = localizer["Loaded"];
            worksheet.Cells[1, 5].Value = localizer["LoadedWeight"];
            worksheet.Cells[1, 6].Value = localizer["Unloaded"];
            worksheet.Cells[1, 7].Value = localizer["UnloadedWeight"];
            worksheet.Cells[1, 8].Value = localizer["Departure"];
            int row = 2;
            foreach (var _shipmentProduct in _shipmentProducts)
            {
                // Add data to subsequent rows
                worksheet.Cells[row, 1].Value = _shipmentProduct.Contract;
                worksheet.Cells[row, 2].Value = _shipmentProduct.Category;
                worksheet.Cells[row, 3].Value = _shipmentProduct.Name;
                worksheet.Cells[row, 4].Value = _shipmentProduct.Loaded;
                worksheet.Cells[row, 5].Value = _shipmentProduct.Weight * _shipmentProduct.Loaded;
                worksheet.Cells[row, 6].Value = _shipmentProduct.Unloaded;
                worksheet.Cells[row, 7].Value = _shipmentProduct.Weight * _shipmentProduct.Unloaded;
                worksheet.Cells[row, 8].Value = _shipmentProduct.Departure.ToString("yyyy-MM-dd HH:mm:ss");
                row++;
            }
            // Auto-fit columns for better readability
            worksheet.Cells.AutoFitColumns();
            return package.GetAsByteArray();
        }
    }
}