using NPOI.OpenXmlFormats.Wordprocessing;
using NPOI.XWPF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RunTaskForAny.Common.WPFPrint
{
    public class PrintHelper
    {
        /// <summary>
        /// 多品一单
        /// </summary>
        /// <param name="printerName"></param>
        /// <param name="wordFilePath"></param>
        /// <param name="model"></param>
        /// <param name="datalist"></param>
        /// <param name="printNumber"></param>
        public static void PrintDoc(string printerName, string wordFilePath, List<KeyValuePair<string, string>> model, List<List<KeyValuePair<string, string>>> datalist, string printNumber = "")
        {
            PrintDocForDataList(printerName, wordFilePath, model, new List<List<List<KeyValuePair<string, string>>>>() { datalist }, printNumber);
        }

        public static void PrintDocForDataList(string printerName, string wordFilePath, List<KeyValuePair<string, string>> model, List<List<List<KeyValuePair<string, string>>>> datalists, string printNumber = "")
        {
            if (!System.IO.File.Exists(wordFilePath))
            {
                return;
            }
            FileStream fs = new FileStream(wordFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            XWPFDocument doc = new XWPFDocument(fs);

            model.Add(new KeyValuePair<string, string>("{PrintNumber}", printNumber));

            WPFPrint.PrintHelper.ReplaceContent(doc, model, datalists);

            var document = WPFPrint.PrintHelper.XWPFDocumentToFlowDocument(doc);

            if (string.IsNullOrWhiteSpace(printerName))
            {
                var printDialog = new PrintDialog();
                IDocumentPaginatorSource dps = document;
                printDialog.PrintDocument(dps.DocumentPaginator, "Document_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + DateTime.Now.GetHashCode());
            }
            else
            {
                var printers = new LocalPrintServer().GetPrintQueues();
                var selectedPrinter = printers.FirstOrDefault(p => p.Name == printerName);
                var printDialog = new PrintDialog();
                printDialog.PrintQueue = selectedPrinter;
                IDocumentPaginatorSource dps = document;
                printDialog.PrintDocument(dps.DocumentPaginator, "Document_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + DateTime.Now.GetHashCode());
            }


        }

        /// <summary>
        /// 一品一单
        /// </summary>
        /// <param name="printerName"></param>
        /// <param name="wordFilePath"></param>
        /// <param name="model"></param>
        /// <param name="data"></param>
        /// <param name="printNumber"></param>
        public static void PrintDoc(string printerName, string wordFilePath, List<KeyValuePair<string, string>> model, List<KeyValuePair<string, string>> data, string printNumber = "")
        {
            PrintDoc(printerName, wordFilePath, model, new List<List<KeyValuePair<string, string>>>() { data }, printNumber);
        }

        public static bool PrintDoc(string printerName, string wordFilePath)
        {
            try
            {
                if (!System.IO.File.Exists(wordFilePath))
                {
                    return false;
                }
                FileStream fs = new FileStream(wordFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                XWPFDocument doc = new XWPFDocument(fs);
                var document = WPFPrint.PrintHelper.XWPFDocumentToFlowDocument(doc);
                if (string.IsNullOrWhiteSpace(printerName))
                {
                    var printDialog = new PrintDialog();
                    IDocumentPaginatorSource dps = document;
                    printDialog.PrintDocument(dps.DocumentPaginator, "Document_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + DateTime.Now.GetHashCode());
                }
                else
                {
                    var printers = new LocalPrintServer().GetPrintQueues();
                    var selectedPrinter = printers.FirstOrDefault(p => p.Name == printerName);
                    var printDialog = new PrintDialog();
                    printDialog.PrintQueue = selectedPrinter;
                    IDocumentPaginatorSource dps = document;
                    printDialog.PrintDocument(dps.DocumentPaginator, "Document_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + DateTime.Now.GetHashCode());
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public static void ReplaceContent(XWPFDocument doc, List<KeyValuePair<string, string>> model, List<List<List<KeyValuePair<string, string>>>> datalists)
        {
            var elements = doc.BodyElements;
            var table_index = 0;
            foreach (var element in elements)
            {
                if (element.ElementType == BodyElementType.PARAGRAPH)
                {
                    var par = element as XWPFParagraph;
                    if (par != null)
                    {
                        foreach (var m in model)
                        {
                            if (par.Text.Contains(m.Key))
                            {
                                par.ReplaceText(par.Text, par.Text.Replace(m.Key, m.Value));
                            }
                        }
                    }

                }
                else if (element.ElementType == BodyElementType.TABLE)
                {
                    var table = element as XWPFTable;
                    if (table != null && table.Rows.Count > 0)
                    {
                        var datalist = datalists[table_index];

                        var fuzhi_row_index = table.Rows.Count - 1;
                        if (datalist.Count > 1)
                        {
                            foreach (var data in datalist)
                            {
                                for (int i = 1; i < data.Count; i++)
                                {
                                    table.AddRow(table.Rows[fuzhi_row_index]);
                                }
                            }
                        }
                        for (int i = 0; i < fuzhi_row_index; i++)
                        {
                            var row = table.Rows[i];
                            var row_cells = row.GetTableCells();
                            for (int j = 0; j < row_cells.Count; j++)
                            {
                                var cell = row_cells[j];
                                var c_pars = cell.Paragraphs;
                                foreach (var c_par in c_pars)
                                {
                                    foreach (var m in model)
                                    {
                                        if (c_par.Text.Contains(m.Key))
                                        {
                                            c_par.ReplaceText(c_par.Text, c_par.Text.Replace(m.Key, m.Value));
                                        }
                                    }
                                }

                            }
                        }
                        for (int i = fuzhi_row_index; i < table.Rows.Count; i++)
                        {
                            var row = table.Rows[i];
                            var row_cells = row.GetTableCells();
                            for (int j = 0; j < row_cells.Count; j++)
                            {
                                var cell = row_cells[j];
                                var c_pars = cell.Paragraphs;
                                foreach (var c_par in c_pars)
                                {
                                    var m2 = datalist[i - fuzhi_row_index][j];
                                    if (c_par.Text.Contains(m2.Key))
                                    {
                                        c_par.ReplaceText(c_par.Text, c_par.Text.Replace(m2.Key, m2.Value));
                                    }
                                }

                            }

                        }

                        table_index++;

                    }
                }

            }

        }


        public static FlowDocument XWPFDocumentToFlowDocument(XWPFDocument xWPFDocument)
        {
            XWPFDocument doc = xWPFDocument;
            FlowDocument document = new FlowDocument();
            document.PageWidth = doc.Document.body.sectPr.pgSz.w / 15d;
            var elements = doc.BodyElements;
            foreach (var element in elements)
            {
                if (element.ElementType == BodyElementType.PARAGRAPH)
                {
                    var par = element as XWPFParagraph;
                    if (par != null)
                    {
                        var fd_paragraph = PrintHelper.XWPFParagraphToParagraph(par);
                        if (fd_paragraph != null)
                        {
                            document.Blocks.Add(fd_paragraph);
                        }
                    }

                }
                else if (element.ElementType == BodyElementType.TABLE)
                {
                    var table = element as XWPFTable;
                    if (table != null && table.Rows.Count > 0)
                    {

                        var borders = doc.Document.body.GetTblArray(0).Items1;//第一个表格样式

                        //for (int b = borders.Count - 1; b >= 0; b--)//边框样式顺序,NPOI框架疑似获取反了
                        //{
                        //    var border = ((borders[b] as CT_Row).Items[0] as CT_Tc).tcPr.tcBorders;

                        //}

                        var fd_table = new Table();
                        fd_table.Margin = new Thickness(0);
                        fd_table.CellSpacing = 0;
                        var cells = table.Rows[table.Rows.Count - 1].GetTableCells();
                        for (int i = 0; i < cells.Count; i++)
                        {
                            var w = double.Parse(cells[i].GetCTTc().tcPr.tcW.w) / 15d;
                            fd_table.Columns.Add(new TableColumn() { Width = new GridLength(w) });
                        }
                        var fd_tableRowGroup = new TableRowGroup();
                        fd_table.RowGroups.Add(fd_tableRowGroup);
                        for (int i = 0; i < table.Rows.Count; i++)
                        {
                            var row = table.Rows[i];
                            var row_cells = row.GetTableCells();
                            var fd_row = new TableRow();

                            var border = ((borders[table.Rows.Count - 1 - i] as CT_Row).Items[0] as CT_Tc).tcPr.tcBorders;//边框样式

                            if(borders.Count-1<i)
                            {
                                border = ((borders[0] as CT_Row).Items[0] as CT_Tc).tcPr.tcBorders;//边框样式
                            }

                            for ( int j = 0; j < row_cells.Count; j++)
                            {
                                var cell = row_cells[j];

                                var c_pars = cell.Paragraphs;

                                var fd_cell = new TableCell();
                                fd_cell.Padding = new Thickness(7,0,7,0);
                                if (row_cells.Count != cells.Count)
                                {
                                    fd_cell.ColumnSpan = cells.Count;
                                }

                                SetBorder(border, fd_cell);

                                foreach (var c_par in c_pars)
                                {
                                    var fd_paragraph = PrintHelper.XWPFParagraphToParagraph(c_par);
                                    if (fd_paragraph != null)
                                    {
                                        fd_cell.Blocks.Add(fd_paragraph);
                                    }
                                }


                                fd_row.Cells.Add(fd_cell);
                            }

                            fd_tableRowGroup.Rows.Add(fd_row);
                        }

                        document.Blocks.Add(fd_table);
                    }
                }

            }

            return document;
        }

        /// <summary>
        /// 厘米转像素
        /// </summary>
        /// <param name="cm"></param>
        /// <returns></returns>
        public static double CmToPx(double cm)
        {
            return (96d / 2.54d) * cm;
        }

        static Paragraph XWPFParagraphToParagraph(XWPFParagraph xWPFParagraph)
        {
            var par = xWPFParagraph;
            if (par != null)
            {
                var fd_paragraph = new Paragraph();
                fd_paragraph.Margin = new Thickness(0);

                SetBorder(par.BorderBottom, new Thickness(0, 0, 0, 1), fd_paragraph);
                SetBorder(par.BorderLeft, new Thickness(1, 0, 0, 0), fd_paragraph);
                SetBorder(par.BorderRight, new Thickness(0, 0, 1, 0), fd_paragraph);
                SetBorder(par.BorderTop, new Thickness(0, 1, 0, 0), fd_paragraph);

                switch (par.Alignment)
                {
                    case ParagraphAlignment.LEFT:
                        fd_paragraph.TextAlignment = System.Windows.TextAlignment.Left;
                        break;
                    case ParagraphAlignment.CENTER:
                        fd_paragraph.TextAlignment = System.Windows.TextAlignment.Center;
                        break;
                    case ParagraphAlignment.RIGHT:
                        fd_paragraph.TextAlignment = System.Windows.TextAlignment.Right;
                        break;
                    case ParagraphAlignment.BOTH:
                        fd_paragraph.TextAlignment = System.Windows.TextAlignment.Justify;
                        break;
                    default:
                        break;
                }

                foreach (var run in par.Runs)
                {
                    var fd_run = new Run(run.GetText(0));
                    fd_run.FontFamily = new FontFamily(run.FontFamily);
                    if (run.FontSize > 0)
                    {
                        fd_run.FontSize = ((double)run.FontSize / 72d) * 96d;
                        if (double.IsNaN(fd_paragraph.LineHeight))
                        {
                            fd_paragraph.LineHeight = fd_run.FontSize * 1.5d;
                        }
                    }

                    var c = run.GetColor();
                    if (!string.IsNullOrEmpty(c))
                    {
                        Color color = (Color)ColorConverter.ConvertFromString("#" + c);
                        fd_run.Foreground = new SolidColorBrush(color);
                    }
                    if (run.IsBold)
                    {
                        fd_run.FontWeight = FontWeights.Bold;
                    }
                    if (run.IsItalic)
                    {
                        fd_run.FontStyle = FontStyles.Italic;
                    }

                    if (run.Underline != UnderlinePatterns.None)
                    {
                        {
                            //var tds = new TextDecorationCollection();
                            // var td = new TextDecoration();
                            //td.PenOffset = 4;
                            //tds.Add(td);
                            //Pen pen = new Pen(new SolidColorBrush(Colors.Black), 1);
                            //pen.DashStyle = DashStyles.DashDotDot;
                            //td.Pen = pen;
                            //fd_paragraph.TextDecorations = tds;
                        }
                        fd_run.TextDecorations = TextDecorations.Underline;
                    }

                    var xwpfPictureList = run.GetEmbeddedPictures();
                    if (xwpfPictureList != null && xwpfPictureList.Count > 0)
                    {
                        foreach (var pic in xwpfPictureList)
                        {
                            var fd_img = new Image();
                            var pic_data = ByteArrayToBitmapImage(pic.GetPictureData().Data);
                            fd_img.Source = pic_data;
                            fd_paragraph.Inlines.Add(fd_img);
                        }
                    }

                    fd_paragraph.Inlines.Add(fd_run);


                }

                return (fd_paragraph);
            }
            else
            {
                return null;
            }
        }

        static void SetBorder(Borders borders, Thickness thickness, Paragraph fd_paragraph)
        {
            if (borders != Borders.None)
            {
                Pen pen = new Pen();
                pen.DashStyle = DashStyles.Solid;
                switch (borders)
                {
                    case Borders.Dotted:
                        pen.DashStyle = DashStyles.Dot;
                        break;
                    case Borders.Dashed:
                        pen.DashStyle = DashStyles.Dash;
                        break;
                    case Borders.DotDash:
                        pen.DashStyle = DashStyles.DashDot;
                        break;
                    case Borders.DotDotDash:
                        pen.DashStyle = DashStyles.DashDotDot;
                        break;
                    default:
                        break;
                }

                var db = new DrawingBrush(new GeometryDrawing(new SolidColorBrush(Colors.Black), pen, new GeometryGroup() { Children = new GeometryCollection() { new RectangleGeometry(new Rect(0, 0, 1, 1)), new RectangleGeometry(new Rect(1, 1, 1, 1)) } }));
                db.TileMode = TileMode.Tile;
                db.Viewport = new Rect(0, 0, 1, 1);
                db.ViewportUnits = BrushMappingMode.Absolute;
                fd_paragraph.BorderBrush = db;
                fd_paragraph.BorderThickness = thickness;

            }
        }

        static void SetBorder(CT_TcBorders border, TableCell tableCell)
        {
            if (border != null)
            {
                var bottom = 0;
                var left = 0;
                var right = 0;
                var top = 0;
                if (border?.bottom != null)
                {
                    bottom = 1;
                }
                if (border?.left != null)
                {
                    left = 1;
                }
                if (border?.right != null)
                {
                    right = 1;
                }
                if (border?.top != null)
                {
                    top = 1;
                }

                tableCell.BorderBrush = new SolidColorBrush(Colors.Black);
                tableCell.BorderThickness = new Thickness(left, top, right, bottom);

            }
        }

        public static BitmapImage ByteArrayToBitmapImage(byte[] byteArray)
        {
            BitmapImage bmp = null;

            try
            {
                bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.StreamSource = new MemoryStream(byteArray);
                bmp.EndInit();
            }
            catch
            {
                bmp = null;
            }

            return bmp;

        }
    }
}
