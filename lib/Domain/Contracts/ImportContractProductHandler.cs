using Bonjour.Domain.Helpers;
using Bonjour.Domain.Products;
using Bonjour.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using QRCoder;

namespace Bonjour.Domain.Contracts;

public class ImportContractProductHandler : IRequestHandler<ImportContractProductRequest, int>
{
    private readonly ApplicationDbContext dbContext;
    private readonly ILogger<ImportContractProductHandler> logger;
    private readonly IMediator mediator;
    public ImportContractProductHandler(ApplicationDbContext dbContext, ILogger<ImportContractProductHandler> logger, IMediator mediator)
    {
        this.dbContext = dbContext;
        this.logger = logger;
        this.mediator = mediator;
    }
    public async Task<int> Handle(ImportContractProductRequest request, CancellationToken cancellationToken)
    {
        string filePath = request.FilePath;
        int id = request.ContractId;
        ExcelPackage.License.SetNonCommercialPersonal("Bonjour");
        int success = 0;
        FileInfo fileInfo = new FileInfo(filePath);

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

                // Loop through rows (assuming first row is header)
                for (int rowNum = startRow + 1; rowNum <= endRow; rowNum++)
                {
                    Product product = new Product();
                    // Assuming columns are in a specific order: Name, Age, City
                    product.Code = worksheet.Cells[rowNum, startCol].Text;
                    product.Name = worksheet.Cells[rowNum, startCol + 1].Text;
                    product.Quantity = int.Parse(worksheet.Cells[rowNum, startCol + 2].Text);
                    product.Delivery = 0;
                    product.ContractId = id;
                    dbContext.Products.Add(product);
                    for (int i = 0; i < product.Quantity; i++)
                    {
                        var _productDetail = new ProductDetails()
                        {
                            Product = product,
                            SequenceNo = i + 1,
                            ShortId = ShortIdGenerator.Generate(12),
                            Status = ProductStatus.RESERVED.Code,
                            CreatedAt = DateTime.UtcNow,
                        };
                        dbContext.ProductDetails.Add(_productDetail);
                        success++;
                    }
                }

                await dbContext.SaveChangesAsync();
                await mediator.Send(new GenerateQrCodeRequest(id));
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error importing file");
            throw new ValidationException("Id", "Import file failed");
        }
        fileInfo.Delete();
        return success;
    }
}