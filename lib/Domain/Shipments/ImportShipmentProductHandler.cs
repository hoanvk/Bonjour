using Bonjour.Domain.Helpers;
using Bonjour.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace Bonjour.Domain.Shipments;

public class ImportShipmentProductHandler : IRequestHandler<ImportShipmentProductRequest, int>
{
    private readonly ApplicationDbContext dbContext;
    private readonly ILogger<ImportShipmentProductHandler> logger;
    public ImportShipmentProductHandler(ApplicationDbContext dbContext, ILogger<ImportShipmentProductHandler> logger)
    {
        this.dbContext = dbContext;
        this.logger = logger;
    }
    public async Task<int> Handle(ImportShipmentProductRequest request, CancellationToken cancellationToken)
    {
        string filePath = request.FilePath;
        ExcelPackage.License.SetNonCommercialPersonal("Bonjour");

        FileInfo fileInfo = new FileInfo(filePath);
        int success = 0;
        try
        {
            using (ExcelPackage package = new ExcelPackage(fileInfo))
            {
                // Get the first worksheet
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                // Determine the range of used cells
                int startRow = worksheet.Dimension.Start.Row;
                int endRow = worksheet.Dimension.End.Row;
                int startCol = worksheet.Dimension.Start.Column;
                int endCol = worksheet.Dimension.End.Column;

                var _shipmentProducts = await dbContext.ShipmentProducts.Where(p => p.ShipmentId == request.ShipmentId).ToListAsync();
                if (_shipmentProducts != null && _shipmentProducts.Any())
                {
                    dbContext.ShipmentProducts.RemoveRange(_shipmentProducts);
                }
                // Loop through rows (assuming first row is header)
                for (int rowNum = startRow + 1; rowNum <= endRow; rowNum++)
                {
                    var _product = await dbContext.Products.FirstOrDefaultAsync(m => m.Code == worksheet.Cells[rowNum, startCol].Text);
                    if (_product == null)
                    {
                        throw new ValidationException("Code", "Product not found");
                    }
                    var _shipmentProduct = new ShipmentProduct();
                    // Assuming columns are in a specific order: Name, Age, City
                    _shipmentProduct.Loaded = int.Parse(worksheet.Cells[rowNum, startCol + 1].Text);
                    _shipmentProduct.Unloaded = int.Parse(worksheet.Cells[rowNum, startCol + 2].Text);
                    _shipmentProduct.ShipmentId = request.ShipmentId;
                    dbContext.ShipmentProducts.Update(_shipmentProduct);
                    success++;
                }
                await dbContext.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error when save to db");
        }
        fileInfo.Delete();
        return success;
    }
}