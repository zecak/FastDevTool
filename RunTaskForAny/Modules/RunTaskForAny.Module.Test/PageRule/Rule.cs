using HtmlAgilityPack;
using RunTaskForAny.Common.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RunTaskForAny.Module.Test.PageRule
{
    public class Rule
    {
        DataTable dataTable = new DataTable();
        public string FunctionSeparator { get { return "$$"; } }
        public string KeySeparator { get { return "::"; } }
        public string ValueSeparator { get { return "="; } }
        public string LeftSeparator { get { return "["; } }
        public string RightSeparator { get { return "]"; } }
        public AcquisitionRule Config { get; set; }
        public Rule(AcquisitionRule acquisitionRule)
        {
            Config = acquisitionRule;
            foreach (var keyValue in Config.RowRules)
            {
                dataTable.Columns.Add(new DataColumn(keyValue.Key));
            }
        }

        public List<string> GetPageValue()
        {
            string url = Config.Url;
            var rulesindex = Config.RowRules.FindIndex(kv => kv.Key == "{内容}");
            List<KeyValue> kvspage = new List<KeyValue>();
            if (rulesindex == -1) { return null; }
            else
            {
                for (int i = 0; i < Config.RowRules.Count; i++)
                {
                    if (i > rulesindex)
                    {
                        kvspage.Add(Config.RowRules[i]);
                    }
                }
            }

            NSoup.Nodes.Document doc = null;
            try
            {
                doc = NSoup.NSoupClient.Parse(HttpTool.AjaxGet(url));
            }
            catch (Exception ex)
            {
                return null;
            }

            List<string> l = new List<string>();

            var duan = doc;

            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(doc.ToString());
            HtmlNode rootNode = document.DocumentNode;

            foreach (var rr in kvspage)
            {
                if (!rr.Value.StartsWith(LeftSeparator))
                {
                    HtmlNode node = null;
                    var valrs1 = rr.Value.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    var nodes = rootNode.SelectNodes(valrs1[0]);
                    if (nodes == null)
                    {
                        l.Add("");
                        break;
                    }
                    node = nodes.FirstOrDefault();

                    foreach (var v in valrs1)
                    {
                        if (v == "{$Text}")
                        {
                            l.Add(node.InnerText.Trim());
                            break;

                        }
                        if (v == "{$Html}")
                        {
                            l.Add(node.OuterHtml.Trim());
                            break;

                        }
                        if (v == "{$IHtml}")
                        {
                            l.Add(node.InnerHtml.Trim());
                            break;

                        }
                        if (v.StartsWith("{$Attr=") && v.EndsWith("}"))
                        {
                            var valtemp = v.Substring(1, v.Length - 2).Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                            if (valtemp.Length >= 2)
                            {
                                l.Add(node.Attributes[valtemp[1]].Value.Trim());
                                break;
                            }
                        }
                        if (v.StartsWith("{$Down=") && v.EndsWith("}"))
                        {
                            if (node == null) { l.Add(""); break; }
                            var valtemp = v.Substring(1, v.Length - 2).Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                            if (valtemp.Length >= 2)
                                if (!string.IsNullOrWhiteSpace(node.Attributes[valtemp[1]].Value))
                                {
                                    Uri baseUri = new Uri(Config.Url);
                                    Uri absoluteUri = new Uri(baseUri, node.Attributes[valtemp[1]].Value);

                                    //l.Add(absoluteUri.AbsoluteUri);

                                    var filename = DownFile(absoluteUri, System.IO.Path.Combine(Config.DataPath, Config.Name, Config.DownPath));
                                    l.Add(filename);
                                    break;
                                }
                        }
                        if (v.StartsWith("{$Downs=") && v.EndsWith("}"))
                        {
                            var valtemp = v.Substring(1, v.Length - 2).Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                            if (valtemp.Length >= 2)
                            {
                                var dirname = string.Empty;
                                if (valtemp.Length == 3)
                                {
                                    var dirindex = Config.RowRules.FindIndex(r => r.Key == valtemp[2]) - rulesindex - 1;
                                    if (dirindex >= 0 && dirindex < l.Count)
                                    {
                                        dirname = FilterFileNamePath(l[dirindex]);
                                    }
                                }
                                List<Uri> uris = new List<Uri>();
                                string links = string.Empty;
                                foreach (var n in nodes)
                                {
                                    if (!string.IsNullOrWhiteSpace(n.Attributes[valtemp[1]].Value))
                                    {
                                        Uri baseUri = new Uri(Config.Url);
                                        Uri absoluteUri = new Uri(baseUri, n.Attributes[valtemp[1]].Value);
                                        var filename = System.IO.Path.Combine(Config.DataPath, Config.Name, Config.DownPath, dirname, absoluteUri.Segments[absoluteUri.Segments.Length - 1]);
                                        links += filename + ";";
                                        uris.Add(absoluteUri);
                                    }
                                }
                                if (uris.Count > 0)
                                {
                                    var filenames = DownFiles(uris, System.IO.Path.Combine(Config.DataPath, Config.Name, Config.DownPath, dirname));
                                    l.Add(links);
                                    break;
                                }
                            }
                        }

                    }
                }
                else
                {
                    #region 默认规则

                    NSoup.Nodes.Element row1 = null;
                    NSoup.Select.Elements rowst1 = null;
                    var valrs1 = rr.Value.Split(new string[] { FunctionSeparator }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var v in valrs1)
                    {
                        if (v.Contains(LeftSeparator + "Tag" + KeySeparator))
                        {
                            var valtemp = v.Substring(1, v.Length - 2).Split(new string[] { KeySeparator }, StringSplitOptions.RemoveEmptyEntries);
                            if (valtemp.Length >= 2)
                            {
                                if (rowst1 == null)
                                {
                                    rowst1 = duan.GetElementsByTag(valtemp[1]);
                                }
                                else
                                {
                                    if (row1 != null)
                                    {
                                        rowst1 = row1.GetElementsByTag(valtemp[1]);
                                    }
                                }
                            }
                        }
                        if (v.Contains(LeftSeparator + "Attr" + KeySeparator))
                        {
                            var valtemp = v.Substring(1, v.Length - 2).Split(new string[] { KeySeparator }, StringSplitOptions.RemoveEmptyEntries);
                            if (valtemp.Length >= 2)
                            {
                                valtemp = valtemp[1].Split(new string[] { ValueSeparator }, StringSplitOptions.RemoveEmptyEntries);
                                if (valtemp.Length >= 2)
                                {
                                    if (rowst1 == null)
                                    {
                                        rowst1 = duan.GetElementsByAttributeValue(valtemp[0], valtemp[1]);
                                    }
                                    else
                                    {
                                        if (row1 != null)
                                        {
                                            rowst1 = row1.GetElementsByAttributeValue(valtemp[0], valtemp[1]);
                                        }
                                    }
                                }
                            }
                        }
                        if (v.Contains(LeftSeparator + "RowIndex" + KeySeparator))
                        {
                            continue;
                        }
                        if (v.Contains(LeftSeparator + "Index" + KeySeparator))
                        {
                            var valtemp = v.Substring(1, v.Length - 2).Split(new string[] { KeySeparator }, StringSplitOptions.RemoveEmptyEntries);
                            if (valtemp.Length >= 2)
                            {
                                int indext = Convert.ToInt32(valtemp[1]);
                                if (indext >= rowst1.Count) { break; }
                                if (indext < 0)
                                {
                                    row1 = rowst1[rowst1.Count + indext];
                                }
                                else
                                {
                                    row1 = rowst1[indext];
                                }
                            }
                        }
                        if (v.Contains(LeftSeparator + "FIndex" + RightSeparator))
                        {
                            row1 = rowst1.First;
                        }
                        if (v.Contains(LeftSeparator + "LIndex" + RightSeparator))
                        {
                            row1 = rowst1.Last;
                        }
                        if (v.Contains(LeftSeparator + "Prev" + RightSeparator))
                        {
                            row1 = row1.PreviousElementSibling;
                        }
                        if (v.Contains(LeftSeparator + "Next" + RightSeparator))
                        {
                            row1 = row1.NextElementSibling;
                        }
                        if (v.Contains(LeftSeparator + "Parent" + RightSeparator))
                        {
                            row1 = row1.Parent;
                        }
                        if (v.Contains(LeftSeparator + "Child" + RightSeparator))
                        {
                            rowst1 = row1.Children;
                        }

                        if (v.Contains(LeftSeparator + "Text" + RightSeparator) && row1 != null)
                        {
                            l.Add(row1.Text().Trim());
                            break;
                        }
                        if (v.Contains(LeftSeparator + "Attr" + KeySeparator) && row1 != null)
                        {
                            var valtemp = v.Substring(1, v.Length - 2).Split(new string[] { KeySeparator }, StringSplitOptions.RemoveEmptyEntries);
                            if (valtemp.Length >= 2)
                            {
                                l.Add(row1.Attributes[valtemp[1]]);
                                break;
                            }
                        }
                        if (v.Contains(LeftSeparator + "Html" + RightSeparator) && row1 != null)
                        {
                            l.Add(row1.Html().Trim());
                            break;

                        }
                        if (v.Contains(LeftSeparator + "IHtml" + RightSeparator) && row1 != null)
                        {
                            l.Add(row1.Children.Html().Trim());
                            break;

                        }
                        if (v.Contains(LeftSeparator + "Link" + RightSeparator) && row1 != null)
                        {
                            var link = row1.Attr("href");

                            Uri baseUri = new Uri(Config.Url);
                            Uri absoluteUri = new Uri(baseUri, link);

                            l.Add(absoluteUri.AbsoluteUri);
                            break;
                        }
                        if (v.Contains(LeftSeparator + "PrevText" + RightSeparator) && row1 != null)
                        {
                            var text = row1.PreviousSibling.Attr("text").Trim();

                            l.Add(text);
                            break;

                        }
                        if (v.Contains(LeftSeparator + "NextText" + RightSeparator) && row1 != null)
                        {
                            var text = row1.NextSibling.Attr("text").Trim();

                            l.Add(text);
                            break;

                        }
                        if (v.Contains(LeftSeparator + "GetAttr" + KeySeparator) && row1 != null)
                        {
                            var valtemp = v.Substring(1, v.Length - 2).Split(new string[] { KeySeparator }, StringSplitOptions.RemoveEmptyEntries);
                            if (valtemp.Length >= 2)
                            {
                                var text = row1.Attr(valtemp[1]).Trim();

                                l.Add(text);
                                break;

                            }
                        }

                        if (v.Contains(LeftSeparator + "Clear" + KeySeparator) && row1 != null)
                        {
                            var valtemp = v.Substring(1, v.Length - 2).Split(new string[] { KeySeparator }, StringSplitOptions.RemoveEmptyEntries);
                            if (valtemp.Length >= 2)
                            {
                                valtemp = valtemp[1].Split(new string[] { ValueSeparator }, StringSplitOptions.RemoveEmptyEntries);
                                if (valtemp.Length >= 2)
                                {
                                    var tv = row1.Attr(valtemp[0]).Replace(valtemp[1], "");
                                    row1.Attr(valtemp[0], tv);
                                }
                            }

                        }

                        if (v.Contains(LeftSeparator + "DownLink" + RightSeparator) && row1 != null)
                        {
                            var link = row1.Attr("href");
                            if (!string.IsNullOrWhiteSpace(link))
                            {
                                Uri baseUri = new Uri(Config.Url);
                                Uri absoluteUri = new Uri(baseUri, link);

                                //l.Add(absoluteUri.AbsoluteUri);

                                var filename = DownFile(absoluteUri, System.IO.Path.Combine(Config.DataPath, Config.Name, Config.DownPath));
                                l.Add(filename);

                                break;
                            }
                        }
                        if (v.Contains(LeftSeparator + "DownPic" + RightSeparator) && row1 != null)
                        {
                            var link = row1.Attr("src");
                            if (!string.IsNullOrWhiteSpace(link))
                            {
                                Uri baseUri = new Uri(Config.Url);
                                Uri absoluteUri = new Uri(baseUri, link);

                                //l.Add(absoluteUri.AbsoluteUri);

                                var filename = DownFile(absoluteUri, System.IO.Path.Combine(Config.DataPath, Config.Name, Config.DownPath));
                                l.Add(filename);
                                break;
                            }
                        }
                        if (v.Contains(LeftSeparator + "Down" + KeySeparator) && row1 != null)
                        {
                            var valtemp = v.Substring(1, v.Length - 2).Split(new string[] { KeySeparator }, StringSplitOptions.RemoveEmptyEntries);
                            if (valtemp.Length >= 2)
                            {
                                var link = row1.Attr(valtemp[1]);
                                if (!string.IsNullOrWhiteSpace(link))
                                {
                                    Uri baseUri = new Uri(Config.Url);
                                    Uri absoluteUri = new Uri(baseUri, link);

                                    //l.Add(absoluteUri.AbsoluteUri);

                                    var filename = DownFile(absoluteUri, System.IO.Path.Combine(Config.DataPath, Config.Name, Config.DownPath));
                                    l.Add(filename);
                                    break;
                                }
                            }
                        }
                        if (v.Contains(LeftSeparator + "Downs" + KeySeparator) && rowst1 != null && rowst1.Count > 0)
                        {
                            var valtemp = v.Substring(1, v.Length - 2).Split(new string[] { KeySeparator }, StringSplitOptions.RemoveEmptyEntries);
                            if (valtemp.Length >= 2)
                            {
                                var dirname = string.Empty;
                                if (valtemp.Length == 3)
                                {
                                    var dirindex = Config.RowRules.FindIndex(r => r.Key == valtemp[2]) - rulesindex - 1;
                                    if (dirindex >= 0 && dirindex < l.Count)
                                    {
                                        dirname = FilterFileNamePath(l[dirindex]);
                                    }
                                }
                                List<Uri> uris = new List<Uri>();
                                string links = string.Empty;
                                foreach (var r in rowst1)
                                {
                                    if (!string.IsNullOrWhiteSpace(r.Attr(valtemp[1])))
                                    {
                                        Uri baseUri = new Uri(Config.Url);
                                        Uri absoluteUri = new Uri(baseUri, r.Attr(valtemp[1]));
                                        var filename = System.IO.Path.Combine(Config.DataPath, Config.Name, Config.DownPath, dirname, absoluteUri.Segments[absoluteUri.Segments.Length - 1]);
                                        links += filename + ";";
                                        uris.Add(absoluteUri);
                                    }
                                }
                                if (uris.Count > 0)
                                {
                                    var filenames = DownFiles(uris, System.IO.Path.Combine(Config.DataPath, Config.Name, Config.DownPath, dirname));
                                    l.Add(links);
                                    break;
                                }


                            }
                        }
                    }
                    #endregion
                }
            }

            return l;

        }

        public DataRow GetRowValue(NSoup.Nodes.Element duan, int index)
        {
            DataRow dataRow = dataTable.NewRow();
            List<string> l = new List<string>();
            //var ruleslistindex = Config.RowRules.FindIndex(kv => kv.Key == "{列表}");
            var rulesindex = Config.RowRules.FindIndex(kv => kv.Key == "{内容}");
            List<KeyValue> kvslist = new List<KeyValue>();
            if (rulesindex == -1) { kvslist = Config.RowRules; }
            else
            {
                for (int i = 0; i < Config.RowRules.Count; i++)
                {
                    if (i <= rulesindex)
                    {
                        kvslist.Add(Config.RowRules[i]);
                    }
                }
            }
            #region 默认规则(取列表)

            foreach (var rr in kvslist)
            {
                NSoup.Nodes.Element row1 = null;
                NSoup.Select.Elements rowst1 = null;
                var valrs1 = rr.Value.Split(new string[] { FunctionSeparator }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var v in valrs1)
                {
                    if (v.Contains(LeftSeparator + "Tag" + KeySeparator))
                    {
                        var valtemp = v.Substring(1, v.Length - 2).Split(new string[] { KeySeparator }, StringSplitOptions.RemoveEmptyEntries);
                        if (valtemp.Length >= 2)
                        {
                            if (rowst1 == null)
                            {
                                rowst1 = duan.GetElementsByTag(valtemp[1]);
                            }
                            else
                            {
                                if (row1 != null)
                                {
                                    rowst1 = row1.GetElementsByTag(valtemp[1]);
                                }
                            }
                        }
                    }
                    if (v.Contains(LeftSeparator + "Attr" + KeySeparator))
                    {
                        var valtemp = v.Substring(1, v.Length - 2).Split(new string[] { KeySeparator }, StringSplitOptions.RemoveEmptyEntries);
                        if (valtemp.Length >= 2)
                        {
                            valtemp = valtemp[1].Split(new string[] { ValueSeparator }, StringSplitOptions.RemoveEmptyEntries);
                            if (valtemp.Length >= 2)
                            {
                                if (rowst1 == null)
                                {
                                    rowst1 = duan.GetElementsByAttributeValue(valtemp[0], valtemp[1]);
                                }
                                else
                                {
                                    if (row1 != null)
                                    {
                                        rowst1 = row1.GetElementsByAttributeValue(valtemp[0], valtemp[1]);
                                    }
                                }
                            }
                        }
                    }
                    if (v.Contains(LeftSeparator + "RowIndex" + KeySeparator))
                    {
                        var valtemp = v.Substring(1, v.Length - 2).Split(new string[] { KeySeparator }, StringSplitOptions.RemoveEmptyEntries);
                        if (valtemp.Length >= 2)
                        {
                            if (index >= rowst1.Count) { break; }
                            row1 = rowst1[index];
                        }
                    }
                    if (v.Contains(LeftSeparator + "Index" + KeySeparator))
                    {
                        var valtemp = v.Substring(1, v.Length - 2).Split(new string[] { KeySeparator }, StringSplitOptions.RemoveEmptyEntries);
                        if (valtemp.Length >= 2)
                        {
                            int indext = Convert.ToInt32(valtemp[1]);
                            if (indext >= rowst1.Count) { break; }
                            if (indext < 0)
                            {
                                row1 = rowst1[rowst1.Count + indext];
                            }
                            else
                            {
                                row1 = rowst1[indext];
                            }

                        }
                    }
                    if (v.Contains(LeftSeparator + "FIndex" + RightSeparator))
                    {
                        row1 = rowst1.First;
                    }
                    if (v.Contains(LeftSeparator + "LIndex" + RightSeparator))
                    {
                        row1 = rowst1.Last;
                    }
                    if (v.Contains(LeftSeparator + "Prev" + RightSeparator))
                    {
                        row1 = row1.PreviousElementSibling;
                    }
                    if (v.Contains(LeftSeparator + "Next" + RightSeparator))
                    {
                        row1 = row1.NextElementSibling;
                    }
                    if (v.Contains(LeftSeparator + "Parent" + RightSeparator))
                    {
                        row1 = row1.Parent;
                    }
                    if (v.Contains(LeftSeparator + "Child" + RightSeparator))
                    {
                        rowst1 = row1.Children;
                    }

                    if (v.Contains(LeftSeparator + "Text" + RightSeparator) && row1 != null)
                    {
                        l.Add(row1.Text().Trim());
                        break;
                    }
                    if (v.Contains(LeftSeparator + "Html" + RightSeparator) && row1 != null)
                    {
                        l.Add(row1.Html().Trim());
                        break;
                    }
                    if (v.Contains(LeftSeparator + "IHtml" + RightSeparator) && row1 != null)
                    {
                        l.Add(row1.Children.Html().Trim());
                        break;
                    }
                    if (v.Contains(LeftSeparator + "Link" + RightSeparator) && row1 != null)
                    {
                        var link = row1.Attr("href");
                        Uri baseUri = new Uri(Config.Url);
                        Uri absoluteUri = new Uri(baseUri, link);

                        l.Add(absoluteUri.AbsoluteUri);
                        break;
                    }
                    if (v.Contains(LeftSeparator + "PrevText" + RightSeparator) && row1 != null)
                    {
                        var text = row1.PreviousSibling.Attr("text").Trim();
                        l.Add(text);
                        break;
                    }
                    if (v.Contains(LeftSeparator + "NextText" + RightSeparator) && row1 != null)
                    {
                        var text = row1.NextSibling.Attr("text").Trim();

                        l.Add(text);
                        break;

                    }
                    if (v.Contains(LeftSeparator + "GetAttr" + KeySeparator) && row1 != null)
                    {
                        var valtemp = v.Substring(1, v.Length - 2).Split(new string[] { KeySeparator }, StringSplitOptions.RemoveEmptyEntries);
                        if (valtemp.Length >= 2)
                        {
                            var text = row1.Attr(valtemp[1]).Trim();

                            l.Add(text);
                            break;

                        }
                    }

                    if (v.Contains(LeftSeparator + "Clear" + KeySeparator) && row1 != null)
                    {
                        var valtemp = v.Substring(1, v.Length - 2).Split(new string[] { KeySeparator }, StringSplitOptions.RemoveEmptyEntries);
                        if (valtemp.Length >= 2)
                        {
                            valtemp = valtemp[1].Split(new string[] { ValueSeparator }, StringSplitOptions.RemoveEmptyEntries);
                            if (valtemp.Length >= 2)
                            {
                                var tv = row1.Attr(valtemp[0]).Replace(valtemp[1], "");
                                row1.Attr(valtemp[0], tv);
                            }
                        }

                    }

                    if (v.Contains(LeftSeparator + "DownLink" + RightSeparator) && row1 != null)
                    {
                        var link = row1.Attr("href");
                        if (!string.IsNullOrWhiteSpace(link))
                        {
                            Uri baseUri = new Uri(Config.Url);
                            Uri absoluteUri = new Uri(baseUri, link);

                            //l.Add(absoluteUri.AbsoluteUri);

                            var filename = DownFile(absoluteUri, System.IO.Path.Combine(Config.DataPath, Config.Name, Config.DownPath));
                            l.Add(filename);
                            break;
                        }
                    }
                    if (v.Contains(LeftSeparator + "DownPic" + RightSeparator) && row1 != null)
                    {
                        var link = row1.Attr("src");
                        if (!string.IsNullOrWhiteSpace(link))
                        {
                            Uri baseUri = new Uri(Config.Url);
                            Uri absoluteUri = new Uri(baseUri, link);

                            // l.Add(absoluteUri.AbsoluteUri);

                            var filename = DownFile(absoluteUri, System.IO.Path.Combine(Config.DataPath, Config.Name, Config.DownPath));
                            l.Add(filename);
                            break;
                        }
                    }
                    if (v.Contains(LeftSeparator + "Down" + KeySeparator) && row1 != null)
                    {
                        var valtemp = v.Substring(1, v.Length - 2).Split(new string[] { KeySeparator }, StringSplitOptions.RemoveEmptyEntries);
                        if (valtemp.Length >= 2)
                        {
                            var link = row1.Attr(valtemp[1]);
                            if (!string.IsNullOrWhiteSpace(link))
                            {
                                Uri baseUri = new Uri(Config.Url);
                                Uri absoluteUri = new Uri(baseUri, link);

                                // l.Add(absoluteUri.AbsoluteUri);

                                var filename = DownFile(absoluteUri, System.IO.Path.Combine(Config.DataPath, Config.Name, Config.DownPath));
                                l.Add(filename);
                                break;
                            }
                        }
                    }
                    if (v.Contains(LeftSeparator + "Downs" + KeySeparator) && rowst1 != null && rowst1.Count > 0)
                    {
                        var valtemp = v.Substring(1, v.Length - 2).Split(new string[] { KeySeparator }, StringSplitOptions.RemoveEmptyEntries);
                        if (valtemp.Length >= 2)
                        {
                            var dirname = string.Empty;
                            if (valtemp.Length == 3)
                            {
                                var dirindex = Config.RowRules.FindIndex(r => r.Key == valtemp[2]);
                                if (dirindex >= 0 && dirindex < l.Count)
                                {
                                    dirname = FilterFileNamePath(l[dirindex]);
                                }
                            }

                            List<Uri> uris = new List<Uri>();
                            string links = string.Empty;
                            foreach (var r in rowst1)
                            {
                                if (!string.IsNullOrWhiteSpace(r.Attr(valtemp[1])))
                                {
                                    Uri baseUri = new Uri(Config.Url);
                                    Uri absoluteUri = new Uri(baseUri, r.Attr(valtemp[1]));

                                    var filename = System.IO.Path.Combine(Config.DataPath, Config.Name, Config.DownPath, dirname, absoluteUri.Segments[absoluteUri.Segments.Length - 1]);

                                    links += filename + ";";

                                    uris.Add(absoluteUri);
                                }
                            }
                            if (uris.Count > 0)
                            {

                                var filenames = DownFiles(uris, System.IO.Path.Combine(Config.DataPath, Config.Name, Config.DownPath, dirname));
                                l.Add(links);
                                break;
                            }


                        }
                    }
                }


            }

            #endregion

            //if (l.Count > 0)
            //{
            //    var pagevalues = GetPageValue(l.Last(), arule, out info2);
            //    if (pagevalues != null)
            //    {
            //        l.AddRange(pagevalues);
            //    }
            //}
            dataRow.ItemArray = l.ToArray();
            return dataRow;

        }

        string FilterFileNamePath(string filenamepath)
        {
            string illegal = filenamepath;
            string regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            Regex r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
            illegal = r.Replace(illegal, "");
            return illegal;
        }

        public Uri GetNextUrl(NSoup.Nodes.Element duan)
        {
            Uri uri = null;
            if (Config.NextPageRules == null) { return null; }
            if (string.IsNullOrWhiteSpace(Config.NextPageRules.Value)) { return null; }
            NSoup.Nodes.Element row1 = null;
            NSoup.Select.Elements rowst1 = null;
            var valrs1 = Config.NextPageRules.Value.Split(new string[] { FunctionSeparator }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var v in valrs1)
            {
                if (v.Contains(LeftSeparator + "Tag" + KeySeparator))
                {
                    var valtemp = v.Substring(1, v.Length - 2).Split(new string[] { KeySeparator }, StringSplitOptions.RemoveEmptyEntries);
                    if (valtemp.Length >= 2)
                    {
                        if (rowst1 == null)
                        {
                            rowst1 = duan.GetElementsByTag(valtemp[1]);
                        }
                        else
                        {
                            if (row1 != null)
                            {
                                rowst1 = row1.GetElementsByTag(valtemp[1]);
                            }
                        }
                    }
                }
                if (v.Contains(LeftSeparator + "Attr" + KeySeparator))
                {
                    var valtemp = v.Substring(1, v.Length - 2).Split(new string[] { KeySeparator }, StringSplitOptions.RemoveEmptyEntries);
                    if (valtemp.Length >= 2)
                    {
                        valtemp = valtemp[1].Split(new string[] { ValueSeparator }, StringSplitOptions.RemoveEmptyEntries);
                        if (valtemp.Length >= 2)
                        {
                            if (rowst1 == null)
                            {
                                rowst1 = duan.GetElementsByAttributeValue(valtemp[0], valtemp[1]);
                            }
                            else
                            {
                                if (row1 != null)
                                {
                                    rowst1 = row1.GetElementsByAttributeValue(valtemp[0], valtemp[1]);
                                }
                            }
                        }
                    }
                }
                if (v.Contains(LeftSeparator + "Index" + KeySeparator))
                {
                    var valtemp = v.Substring(1, v.Length - 2).Split(new string[] { KeySeparator }, StringSplitOptions.RemoveEmptyEntries);
                    if (valtemp.Length >= 2)
                    {
                        int indext = Convert.ToInt32(valtemp[1]);
                        if (indext >= rowst1.Count) { break; }
                        if (indext < 0)
                        {
                            row1 = rowst1[rowst1.Count + indext];
                        }
                        else
                        {
                            row1 = rowst1[indext];
                        }
                    }
                }
                if (v.Contains(LeftSeparator + "FIndex" + RightSeparator))
                {
                    row1 = rowst1.First;
                }
                if (v.Contains(LeftSeparator + "LIndex" + RightSeparator))
                {
                    row1 = rowst1.Last;
                }
                if (v.Contains(LeftSeparator + "Prev" + RightSeparator))
                {
                    row1 = row1.PreviousElementSibling;
                }
                if (v.Contains(LeftSeparator + "Next" + RightSeparator))
                {
                    row1 = row1.NextElementSibling;
                }
                if (v.Contains(LeftSeparator + "Parent" + RightSeparator))
                {
                    row1 = row1.Parent;
                }
                if (v.Contains(LeftSeparator + "Child" + RightSeparator))
                {
                    rowst1 = row1.Children;
                }
                if (v.Contains(LeftSeparator + "Link" + RightSeparator) && row1 != null)
                {
                    var link = row1.Attr("href");
                    if (!string.IsNullOrWhiteSpace(link))
                    {
                        uri = new Uri(new Uri(Config.Url), link);
                        break;
                    }
                }

            }

            return uri;

        }

        public NSoup.Select.Elements GetElements(NSoup.Nodes.Element doc)
        {
            var duan = GetElementsFirstRow(doc);
            if (duan == null) { return null; }
            var list = duan.Parent.Children;
            return list;
        }

        /// <summary>
        /// 获取列表中的第一个元素
        /// </summary>
        /// <param name="duan"></param>
        /// <returns></returns>
        public NSoup.Nodes.Element GetElementsFirstRow(NSoup.Nodes.Element doc)
        {
            var duan = GetElementsFirst(doc);
            if (duan == null) { return null; }
            NSoup.Nodes.Element row = null;
            NSoup.Nodes.Element row1 = null;
            NSoup.Select.Elements rowst1 = null;
            var index = 0;
            var valrs1 = Config.RowRules[0].Value.Split(new string[] { FunctionSeparator }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var v in valrs1)
            {
                if (v.Contains(LeftSeparator + "Tag" + KeySeparator))
                {
                    var valtemp = v.Substring(1, v.Length - 2).Split(new string[] { KeySeparator }, StringSplitOptions.RemoveEmptyEntries);
                    if (valtemp.Length >= 2)
                    {
                        if (rowst1 == null)
                        {
                            rowst1 = duan.GetElementsByTag(valtemp[1]);
                        }
                        else
                        {
                            if (row1 != null)
                            {
                                rowst1 = row1.GetElementsByTag(valtemp[1]);
                            }
                        }
                    }
                }
                if (v.Contains(LeftSeparator + "Attr" + KeySeparator))
                {
                    var valtemp = v.Substring(1, v.Length - 2).Split(new string[] { KeySeparator }, StringSplitOptions.RemoveEmptyEntries);
                    if (valtemp.Length >= 2)
                    {
                        valtemp = valtemp[1].Split(new string[] { ValueSeparator }, StringSplitOptions.RemoveEmptyEntries);
                        if (valtemp.Length >= 2)
                        {
                            if (rowst1 == null)
                            {
                                rowst1 = duan.GetElementsByAttributeValue(valtemp[0], valtemp[1]);
                            }
                            else
                            {
                                if (row1 != null)
                                {
                                    rowst1 = row1.GetElementsByAttributeValue(valtemp[0], valtemp[1]);
                                }
                            }
                        }
                    }
                }
                if (v.Contains(LeftSeparator + "RowIndex" + KeySeparator))
                {
                    var valtemp = v.Substring(1, v.Length - 2).Split(new string[] { KeySeparator }, StringSplitOptions.RemoveEmptyEntries);
                    if (valtemp.Length >= 2)
                    {
                        index = Convert.ToInt32(valtemp[1]);
                        row1 = rowst1[index];
                        if (row == null) { row = row1; return row; }
                    }
                }
                if (v.Contains(LeftSeparator + "Index" + KeySeparator))
                {
                    var valtemp = v.Substring(1, v.Length - 2).Split(new string[] { KeySeparator }, StringSplitOptions.RemoveEmptyEntries);
                    if (valtemp.Length >= 2)
                    {
                        int indext = Convert.ToInt32(valtemp[1]);
                        if (indext < 0)
                        {
                            row1 = rowst1[rowst1.Count + indext];
                        }
                        else
                        {
                            row1 = rowst1[indext];
                        }
                    }
                }
                if (v.Contains(LeftSeparator + "FIndex" + RightSeparator))
                {
                    row1 = rowst1.First;
                }
                if (v.Contains(LeftSeparator + "LIndex" + RightSeparator))
                {
                    row1 = rowst1.Last;
                }
                if (v.Contains(LeftSeparator + "Prev" + RightSeparator))
                {
                    row1 = row1.PreviousElementSibling;
                }
                if (v.Contains(LeftSeparator + "Next" + RightSeparator))
                {
                    row1 = row1.NextElementSibling;
                }
                if (v.Contains(LeftSeparator + "Parent" + RightSeparator))
                {
                    row1 = row1.Parent;
                }
                if (v.Contains(LeftSeparator + "Child" + RightSeparator))
                {
                    rowst1 = row1.Children;
                }
            }
            return row;
        }

        NSoup.Nodes.Element GetElementsFirst(NSoup.Nodes.Element doc)
        {
            var duan = doc.GetElementsByAttributeValue(Config.ParagraphRules.Key, Config.ParagraphRules.Value).First;
            return duan;
        }


        string DownFile(Uri uri, string filepath)
        {
            System.IO.Directory.CreateDirectory(filepath);
            var name = uri.Segments[uri.Segments.Length - 1];
            var filename = System.IO.Path.Combine(filepath, name);
            if (!System.IO.File.Exists(filename))
            {
                try
                {
                    System.Net.WebClient myWebClient = new System.Net.WebClient();
                    myWebClient.DownloadFile(uri, filename);
                }
                catch (Exception ex)
                {
                    Common.Helper.Tool.Log.Error(ex);
                    return null;
                }
            }
            return name;
        }

        List<string> DownFiles(List<Uri> uris, string filepath)
        {
            List<string> list = new List<string>();
            System.IO.Directory.CreateDirectory(filepath);
            System.Net.WebClient myWebClient = new System.Net.WebClient();
            foreach (var uri in uris)
            {
                var name = uri.Segments[uri.Segments.Length - 1];
                var filename = System.IO.Path.Combine(filepath, name);
                if (!System.IO.File.Exists(filename))
                {
                    try
                    {
                        myWebClient.DownloadFile(uri, filename);
                        list.Add(name);
                    }
                    catch (Exception ex)
                    {
                        list.Add(null);
                        Common.Helper.Tool.Log.Error(ex);
                    }
                }

            }
            return list;
        }
    }
}
