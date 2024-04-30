using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Data;

byte[] Export(string title, DataTable dt)
{
    return Document.Create(container =>
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4.Portrait());
            page.Margin(1, Unit.Centimetre);
            page.PageColor(Colors.White);
            page.DefaultTextStyle(x => x.FontSize(8));

            page.Header().AlignCenter().Text(title).SemiBold().FontSize(24).FontColor("#0F2854");

            page.Content()
                .PaddingVertical(0.5f, Unit.Centimetre)
                .Table(table =>
                {
                    IContainer DefaultCellStyle(IContainer container, string backgroundColor, bool alignCenter = false)
                    {
                        container = container
                            .Border(1)
                            .BorderColor(Colors.Grey.Lighten1)
                            .Background(backgroundColor)
                            .PaddingVertical(3)
                            .PaddingHorizontal(3);

                        if (alignCenter)
                        {
                            container = container
                                .AlignCenter()
                                .AlignMiddle();
                        }

                        return container;
                    }

                    table.ColumnsDefinition(columns =>
                    {
                        foreach (var column in dt.Columns)
                        {
                            columns.RelativeColumn(allowShrink: true, allowGrow: true);
                        }
                    });

                    table.ExtendLastCellsToTableBottom();

                    foreach (DataColumn column in dt.Columns)
                    {
                        table.Header(header => header.Cell().Element(HeaderCellStyle).Text(column.ColumnName).Bold());
                    }

                    foreach (DataRow row in dt.Rows)
                    {
                        foreach (DataColumn column in dt.Columns)
                        {
                            table.Cell().Element(CellStyle).Text(row[column]);
                        }
                    }

                    IContainer CellStyle(IContainer container) => DefaultCellStyle(container, Colors.White).ShowOnce();
                    IContainer HeaderCellStyle(IContainer container) => DefaultCellStyle(container, Colors.Grey.Lighten3);
                });

            page.Footer()
                .AlignCenter()
                .Text(x =>
                {
                    x.Span("Seite ");
                    x.CurrentPageNumber();
                    x.Span(" / ");
                    x.TotalPages();
                });
        });
    })
        .GeneratePdf();
}
QuestPDF.Settings.License = LicenseType.Community;
DataTable dt = new DataTable();
dt.Columns.Add(new DataColumn("Firstname"));
dt.Columns.Add(new DataColumn("Lastname"));
dt.Columns.Add(new DataColumn("City"));
dt.Columns.Add(new DataColumn("ZipCode"));
dt.Columns.Add(new DataColumn("Street"));
dt.Columns.Add(new DataColumn("Username"));
dt.Columns.Add(new DataColumn("Company"));
for (int i = 0; i < 100; i++)
{
    var row = dt.NewRow();
    var faker = new Bogus.Faker();
    row[0] = faker.Person.FirstName;
    row[1] = faker.Person.LastName;
    row[2] = faker.Person.Address.City;
    row[3] = faker.Person.Address.ZipCode;
    row[4] = faker.Person.Address.Street;
    row[5] = faker.Person.UserName;
    row[6] = faker.Person.Company.Name;
    dt.Rows.Add(row);
}
File.WriteAllBytes("New.pdf", Export("Test", dt));

