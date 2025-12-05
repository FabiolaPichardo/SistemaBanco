using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ClosedXML.Excel;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace SistemaBanco
{
    public static class ExportHelper
    {

        public static void ExportarExcel(DataTable datos, string tituloReporte, string nombreArchivo)
        {
            try
            {
                SaveFileDialog saveDialog = new SaveFileDialog
                {
                    Filter = "Archivo Excel (*.xlsx)|*.xlsx",
                    FileName = $"{nombreArchivo}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx",
                    Title = "Exportar a Excel"
                };

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    using (var workbook = new XLWorkbook())
                    {
                        var worksheet = workbook.Worksheets.Add("Datos");

                        worksheet.Cell(1, 1).Value = tituloReporte;
                        worksheet.Cell(1, 1).Style.Font.Bold = true;
                        worksheet.Cell(1, 1).Style.Font.FontSize = 16;
                        worksheet.Range(1, 1, 1, datos.Columns.Count).Merge();

                        worksheet.Cell(2, 1).Value = $"Generado: {DateTime.Now:dd/MM/yyyy HH:mm:ss}";
                        worksheet.Cell(3, 1).Value = $"Usuario: {FormLogin.NombreUsuario}";
                        worksheet.Cell(4, 1).Value = $"Total de registros: {datos.Rows.Count}";

                        for (int i = 0; i < datos.Columns.Count; i++)
                        {
                            worksheet.Cell(6, i + 1).Value = datos.Columns[i].ColumnName;
                            worksheet.Cell(6, i + 1).Style.Font.Bold = true;
                            worksheet.Cell(6, i + 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#1e40af");
                            worksheet.Cell(6, i + 1).Style.Font.FontColor = XLColor.White;
                        }

                        for (int i = 0; i < datos.Rows.Count; i++)
                        {
                            for (int j = 0; j < datos.Columns.Count; j++)
                            {
                                var valor = datos.Rows[i][j];
                                var cell = worksheet.Cell(i + 7, j + 1);

                                if (valor is decimal || valor is double || valor is float)
                                {
                                    cell.Value = Convert.ToDouble(valor);
                                    cell.Style.NumberFormat.Format = "#,##0.00";
                                }
                                else if (valor is DateTime)
                                {
                                    cell.Value = (DateTime)valor;
                                    cell.Style.DateFormat.Format = "dd/MM/yyyy";
                                }
                                else
                                {
                                    cell.Value = valor.ToString();
                                }
                            }
                        }

                        worksheet.Columns().AdjustToContents();

                        var rango = worksheet.Range(6, 1, datos.Rows.Count + 6, datos.Columns.Count);
                        rango.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        rango.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                        workbook.SaveAs(saveDialog.FileName);
                    }

                    MessageBox.Show($"Archivo Excel generado exitosamente en:\n{saveDialog.FileName}\n\nPuede abrirlo directamente con Excel.",
                        "Exportación Exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al exportar a Excel:\n{ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void ExportarWord(DataTable datos, string tituloReporte, string nombreArchivo)
        {
            try
            {
                SaveFileDialog saveDialog = new SaveFileDialog
                {
                    Filter = "Documento Word (*.docx)|*.docx",
                    FileName = $"{nombreArchivo}_{DateTime.Now:yyyyMMdd_HHmmss}.docx",
                    Title = "Exportar a Word"
                };

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    using (WordprocessingDocument wordDocument = WordprocessingDocument.Create(saveDialog.FileName, WordprocessingDocumentType.Document))
                    {
                        MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();
                        mainPart.Document = new Document();
                        Body body = mainPart.Document.AppendChild(new Body());

                        Paragraph titulo = body.AppendChild(new Paragraph());
                        Run runTitulo = titulo.AppendChild(new Run());
                        runTitulo.AppendChild(new Text(tituloReporte));
                        RunProperties propsTitulo = runTitulo.InsertAt(new RunProperties(), 0);
                        propsTitulo.AppendChild(new Bold());
                        propsTitulo.AppendChild(new FontSize() { Val = "32" });

                        body.AppendChild(new Paragraph(new Run(new Text($"Generado: {DateTime.Now:dd/MM/yyyy HH:mm:ss}"))));
                        body.AppendChild(new Paragraph(new Run(new Text($"Usuario: {FormLogin.NombreUsuario}"))));
                        body.AppendChild(new Paragraph(new Run(new Text($"Total de registros: {datos.Rows.Count}"))));
                        body.AppendChild(new Paragraph(new Run(new Text(""))));

                        Table table = new Table();

                        TableProperties tblProp = new TableProperties(
                            new TableBorders(
                                new TopBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },
                                new BottomBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },
                                new LeftBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },
                                new RightBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },
                                new InsideHorizontalBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },
                                new InsideVerticalBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 }
                            )
                        );
                        table.AppendChild(tblProp);

                        TableRow headerRow = new TableRow();
                        foreach (DataColumn column in datos.Columns)
                        {
                            TableCell cell = new TableCell();
                            Paragraph para = new Paragraph(new Run(new Text(column.ColumnName)));
                            RunProperties runProps = para.Elements<Run>().First().InsertAt(new RunProperties(), 0);
                            runProps.AppendChild(new Bold());
                            cell.Append(para);
                            headerRow.Append(cell);
                        }
                        table.Append(headerRow);

                        foreach (DataRow row in datos.Rows)
                        {
                            TableRow dataRow = new TableRow();
                            foreach (var item in row.ItemArray)
                            {
                                TableCell cell = new TableCell();
                                string valor = item?.ToString() ?? "";
                                if (item is decimal || item is double)
                                {
                                    valor = string.Format("{0:N2}", item);
                                }
                                else if (item is DateTime)
                                {
                                    valor = ((DateTime)item).ToString("dd/MM/yyyy");
                                }
                                cell.Append(new Paragraph(new Run(new Text(valor))));
                                dataRow.Append(cell);
                            }
                            table.Append(dataRow);
                        }

                        body.Append(table);

                        body.AppendChild(new Paragraph(new Run(new Text(""))));
                        body.AppendChild(new Paragraph(new Run(new Text("© 2025 Módulo Banco - Documento Confidencial"))));

                        mainPart.Document.Save();
                    }

                    MessageBox.Show($"Documento Word generado exitosamente en:\n{saveDialog.FileName}\n\nPuede abrirlo directamente con Word.",
                        "Exportación Exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al exportar a Word:\n{ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void ExportarPDF(DataTable datos, string tituloReporte, string nombreArchivo)
        {
            try
            {
                SaveFileDialog saveDialog = new SaveFileDialog
                {
                    Filter = "Archivo HTML (*.html)|*.html",
                    FileName = $"{nombreArchivo}_{DateTime.Now:yyyyMMdd_HHmmss}.html",
                    Title = "Exportar a PDF (se abrirá en navegador)"
                };

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    string html = GenerarHTML(datos, tituloReporte);
                    File.WriteAllText(saveDialog.FileName, html);
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(saveDialog.FileName) { UseShellExecute = true });

                    MessageBox.Show("Archivo HTML generado. Se abrirá en su navegador.\nDesde ahí puede guardarlo como PDF usando Ctrl+P.",
                        "Exportación Exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al exportar a PDF:\n{ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static string GenerarHTML(DataTable datos, string tituloReporte)
        {
            var html = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <title>{tituloReporte}</title>
    <style>
        body {{ font-family: 'Segoe UI', Arial, sans-serif; margin: 40px; }}
        h1 {{ color: #1e40af; text-align: center; }}
        .info {{ background: #f3f4f6; padding: 15px; margin: 20px 0; border-radius: 8px; }}
        table {{ width: 100%; border-collapse: collapse; margin: 20px 0; }}
        th {{ background: #1e40af; color: white; padding: 12px; text-align: left; }}
        td {{ padding: 10px; border-bottom: 1px solid #e5e7eb; }}
        tr:nth-child(even) {{ background: #f9fafb; }}
        .footer {{ text-align: center; margin-top: 30px; color: #6b7280; font-size: 12px; }}
        @media print {{
            .no-print {{ display: none; }}
        }}
    </style>
</head>
<body>
    <h1>🏦 {tituloReporte}</h1>
    <div class='info'>
        <strong>Usuario:</strong> {FormLogin.NombreUsuario}<br>
        <strong>Fecha de generación:</strong> {DateTime.Now:dd/MM/yyyy HH:mm:ss}<br>
        <strong>Total de registros:</strong> {datos.Rows.Count}
    </div>
    <table>
        <tr>";

            foreach (DataColumn column in datos.Columns)
            {
                html += $"<th>{column.ColumnName}</th>";
            }
            html += "</tr>";

            foreach (DataRow row in datos.Rows)
            {
                html += "<tr>";
                foreach (var item in row.ItemArray)
                {
                    string valor = item?.ToString() ?? "";
                    if (item is decimal || item is double)
                    {
                        valor = string.Format("${0:N2}", item);
                    }
                    else if (item is DateTime)
                    {
                        valor = ((DateTime)item).ToString("dd/MM/yyyy");
                    }
                    html += $"<td>{valor}</td>";
                }
                html += "</tr>";
            }

            html += @"
    </table>
    <div class='footer'>
        © 2025 Módulo Banco - Documento Confidencial
    </div>
</body>
</html>";

            return html;
        }
    }
}
