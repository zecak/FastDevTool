using HtmlAgilityPack;
using RunTaskForAny.Common.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RunTaskForAny.Module.Test.PageRule
{
    public class RuleHelper
    {
        public static List<string> GetRowValue(NSoup.Nodes.Element duan, AcquisitionRule arule, int index, out string info)
        {
            List<string> l = new List<string>();
            info = string.Empty;
            var rulesindex = arule.RowRules.FindIndex(kv => kv.Key == "{$内容链接}");
            List<KeyValue> kvslist = new List<KeyValue>();
            if (rulesindex == -1) { kvslist = arule.RowRules; }
            else
            {
                for (int i = 0; i < arule.RowRules.Count; i++)
                {
                    if (i <= rulesindex)
                    {
                        kvslist.Add(arule.RowRules[i]);
                    }
                }
            }
            #region 默认规则(取列表)

            foreach (var rr in kvslist)
            {
                NSoup.Nodes.Element row1 = null;
                NSoup.Select.Elements rowst1 = null;
                var valrs1 = rr.Value.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var v in valrs1)
                {
                    if (v.Contains("[标签名="))
                    {
                        var valtemp = v.Substring(1, v.Length - 2).Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
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
                    if (v.Contains("[属性名值="))
                    {
                        var valtemp = v.Substring(1, v.Length - 2).Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                        if (valtemp.Length >= 2)
                        {
                            valtemp = valtemp[1].Split(new string[] { "&" }, StringSplitOptions.RemoveEmptyEntries);
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
                    if (v.Contains("[行索引="))
                    {
                        var valtemp = v.Substring(1, v.Length - 2).Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                        if (valtemp.Length >= 2)
                        {
                            if (index >= rowst1.Count) { break; }
                            row1 = rowst1[index];
                        }
                    }
                    if (v.Contains("[索引="))
                    {
                        var valtemp = v.Substring(1, v.Length - 2).Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
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
                    if (v.Contains("[首索引]"))
                    {
                        row1 = rowst1.First;
                    }
                    if (v.Contains("[尾索引]"))
                    {
                        row1 = rowst1.Last;
                    }
                    if (v.Contains("[上元素]"))
                    {
                        row1 = row1.PreviousElementSibling;
                    }
                    if (v.Contains("[下元素]"))
                    {
                        row1 = row1.NextElementSibling;
                    }
                    if (v.Contains("[父元素]"))
                    {
                        row1 = row1.Parent;
                    }
                    if (v.Contains("[子元素]"))
                    {
                        rowst1 = row1.Children;
                    }

                    if (v.Contains("[内容]") && row1 != null)
                    {
                        //if (!string.IsNullOrWhiteSpace(row1.Text()))
                        {
                            info += " " + rr.Key + ":" + row1.Text().Trim();
                            l.Add(row1.Text().Trim());
                            break;
                        }
                    }
                    if (v.Contains("[源码]") && row1 != null)
                    {
                        //if (!string.IsNullOrWhiteSpace(row1.Html()))
                        {
                            info += " " + rr.Key + ":" + row1.Html().Trim();
                            l.Add(row1.Html().Trim());
                            break;
                        }
                    }
                    if (v.Contains("[内源码]") && row1 != null)
                    {
                        //if (!string.IsNullOrWhiteSpace(row1.Children.Html()))
                        {
                            info += " " + rr.Key + ":" + row1.Children.Html().Trim();
                            l.Add(row1.Children.Html().Trim());
                            break;
                        }
                    }
                    if (v.Contains("[链接]") && row1 != null)
                    {
                        var link = row1.Attr("href");
                        //if (!string.IsNullOrWhiteSpace(link))
                        {
                            Uri baseUri = new Uri(arule.Url);
                            Uri absoluteUri = new Uri(baseUri, link);

                            info += " " + rr.Key + ":" + absoluteUri.AbsoluteUri;
                            l.Add(absoluteUri.AbsoluteUri);
                            break;
                        }
                    }
                    if (v.Contains("[上节点内容]") && row1 != null)
                    {
                        var text = row1.PreviousSibling.Attr("text").Trim();
                        //if (!string.IsNullOrWhiteSpace(text))
                        {
                            info += " " + rr.Key + ":" + text;
                            l.Add(text);
                            break;
                        }
                    }
                    if (v.Contains("[下节点内容]") && row1 != null)
                    {
                        var text = row1.NextSibling.Attr("text").Trim();
                        //if (!string.IsNullOrWhiteSpace(text))
                        {
                            info += " " + rr.Key + ":" + text;
                            l.Add(text);
                            break;
                        }
                    }
                    if (v.Contains("[取属性值=") && row1 != null)
                    {
                        var valtemp = v.Substring(1, v.Length - 2).Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                        if (valtemp.Length >= 2)
                        {
                            var text = row1.Attr(valtemp[1]).Trim();
                            //if (!string.IsNullOrWhiteSpace(text))
                            {
                                info += " " + rr.Key + ":" + text;
                                l.Add(text);
                                break;
                            }
                        }
                    }

                    if (v.Contains("[清除=") && row1 != null)
                    {
                        var valtemp = v.Substring(1, v.Length - 2).Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                        if (valtemp.Length >= 2)
                        {
                            valtemp = valtemp[1].Split(new string[] { "&" }, StringSplitOptions.RemoveEmptyEntries);
                            if (valtemp.Length >= 2)
                            {
                                var tv = row1.Attr(valtemp[0]).Replace(valtemp[1], "");
                                row1.Attr(valtemp[0], tv);
                            }
                        }

                    }

                    if (v.Contains("[下载链接]") && row1 != null)
                    {
                        var link = row1.Attr("href");
                        if (!string.IsNullOrWhiteSpace(link))
                        {
                            Uri baseUri = new Uri(arule.Url);
                            Uri absoluteUri = new Uri(baseUri, link);

                            info += " " + rr.Key + ":" + absoluteUri.AbsoluteUri;
                            l.Add(absoluteUri.AbsoluteUri);

                            DownFile(absoluteUri, arule);

                            break;
                        }
                    }
                    if (v.Contains("[下载图片]") && row1 != null)
                    {
                        var link = row1.Attr("src");
                        if (!string.IsNullOrWhiteSpace(link))
                        {
                            Uri baseUri = new Uri(arule.Url);
                            Uri absoluteUri = new Uri(baseUri, link);

                            info += " " + rr.Key + ":" + absoluteUri.AbsoluteUri;
                            l.Add(absoluteUri.AbsoluteUri);

                            DownFile(absoluteUri, arule);

                            break;
                        }
                    }
                    if (v.Contains("[下载=") && row1 != null)
                    {
                        var valtemp = v.Substring(1, v.Length - 2).Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                        if (valtemp.Length >= 2)
                        {
                            var link = row1.Attr(valtemp[1]);
                            if (!string.IsNullOrWhiteSpace(link))
                            {
                                Uri baseUri = new Uri(arule.Url);
                                Uri absoluteUri = new Uri(baseUri, link);

                                info += " " + rr.Key + ":" + absoluteUri.AbsoluteUri;
                                l.Add(absoluteUri.AbsoluteUri);

                                DownFile(absoluteUri, arule);

                                break;
                            }
                        }
                    }
                    if (v.Contains("[下载集合=") && rowst1 != null && rowst1.Count > 0)
                    {
                        var valtemp = v.Substring(1, v.Length - 2).Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                        if (valtemp.Length >= 2)
                        {
                            var dirname = string.Empty;
                            if (valtemp.Length == 3)
                            {
                                var dirindex = arule.RowRules.FindIndex(r => r.Key == valtemp[2]);
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
                                    Uri baseUri = new Uri(arule.Url);
                                    Uri absoluteUri = new Uri(baseUri, r.Attr(valtemp[1]));

                                    var filename = System.IO.Path.Combine(arule.DataPath, arule.Name, arule.DownPath, dirname, absoluteUri.Segments[absoluteUri.Segments.Length - 1]);

                                    links += filename + ";";

                                    uris.Add(absoluteUri);
                                }
                            }
                            if (uris.Count > 0)
                            {
                                info += " " + rr.Key + ":" + links;
                                l.Add(links);
                                DownFiles(uris, arule, dirname);
                                break;
                            }


                        }
                    }
                }


            }

            #endregion

            if (l.Count > 0)
            {
                var info2 = string.Empty;
                var pagevalues = GetPageValue(l.Last(), arule, out info2);
                if (pagevalues != null)
                {
                    info += info2;
                    l.AddRange(pagevalues);
                }
            }

            return l;

        }

        public static void DownFile(Uri uri, AcquisitionRule arule)
        {
            var filepath = System.IO.Path.Combine(arule.DataPath, arule.Name, arule.DownPath);
            System.IO.Directory.CreateDirectory(filepath);
            var filename = System.IO.Path.Combine(filepath, uri.Segments[uri.Segments.Length - 1]);
            if (!System.IO.File.Exists(filename))
            {
                System.Net.WebClient myWebClient = new System.Net.WebClient();
                myWebClient.DownloadFile(uri, filename);
            }
        }

        public static void DownFiles(List<Uri> uris, AcquisitionRule arule, string dirname)
        {
            var filepath = System.IO.Path.Combine(arule.DataPath, arule.Name, arule.DownPath, dirname);
            System.IO.Directory.CreateDirectory(filepath);
            System.Net.WebClient myWebClient = new System.Net.WebClient();
            foreach (var uri in uris)
            {
                var filename = System.IO.Path.Combine(filepath, uri.Segments[uri.Segments.Length - 1]);
                if (!System.IO.File.Exists(filename))
                {
                    try
                    {
                        myWebClient.DownloadFile(uri, filename);
                    }
                    catch { }
                }
            }
        }

        public static List<string> GetPageValue(string url, AcquisitionRule arule, out string info)
        {
            info = "";
            var rulesindex = arule.RowRules.FindIndex(kv => kv.Key == "{$内容链接}");
            List<KeyValue> kvspage = new List<KeyValue>();
            if (rulesindex == -1) { return null; }
            else
            {
                for (int i = 0; i < arule.RowRules.Count; i++)
                {
                    if (i > rulesindex)
                    {
                        kvspage.Add(arule.RowRules[i]);
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
                if (!rr.Value.StartsWith("["))
                {
                    HtmlNode node = null;
                    var valrs1 = rr.Value.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    var nodes = rootNode.SelectNodes(valrs1[0]);
                    if (nodes == null)
                    {
                        info += " " + rr.Key + ":[空]";
                        l.Add("");
                        break;
                    }
                    node = nodes.FirstOrDefault();

                    foreach (var v in valrs1)
                    {
                        if (v == "{$Text}")
                        {
                            //if (!string.IsNullOrWhiteSpace(node.InnerText))
                            {
                                info += " " + rr.Key + ":" + node.InnerText.Trim();
                                l.Add(node.InnerText.Trim());
                                break;
                            }
                        }
                        if (v == "{$Html}")
                        {
                            //if (!string.IsNullOrWhiteSpace(node.OuterHtml))
                            {
                                info += " " + rr.Key + ":" + node.OuterHtml.Trim();
                                l.Add(node.OuterHtml.Trim());
                                break;
                            }
                        }
                        if (v == "{$IHtml}")
                        {
                            //if (!string.IsNullOrWhiteSpace(node.InnerHtml))
                            {
                                info += " " + rr.Key + ":" + node.InnerHtml.Trim();
                                l.Add(node.InnerHtml.Trim());
                                break;
                            }
                        }
                        if (v.StartsWith("{$Attr=") && v.EndsWith("}"))
                        {
                            var valtemp = v.Substring(1, v.Length - 2).Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                            if (valtemp.Length >= 2)
                            //if (!string.IsNullOrWhiteSpace(node.Attributes[valtemp[1]].Value))
                            {
                                info += " " + rr.Key + ":" + node.Attributes[valtemp[1]].Value.Trim();
                                l.Add(node.Attributes[valtemp[1]].Value.Trim());
                                break;
                            }
                        }
                        if (v.StartsWith("{$Down=") && v.EndsWith("}"))
                        {
                            if (node == null) { info += " " + rr.Key + ":[空]"; l.Add(""); break; }
                            var valtemp = v.Substring(1, v.Length - 2).Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                            if (valtemp.Length >= 2)
                                if (!string.IsNullOrWhiteSpace(node.Attributes[valtemp[1]].Value))
                                {
                                    Uri baseUri = new Uri(arule.Url);
                                    Uri absoluteUri = new Uri(baseUri, node.Attributes[valtemp[1]].Value);

                                    info += " " + rr.Key + ":" + absoluteUri.AbsoluteUri;
                                    l.Add(absoluteUri.AbsoluteUri);

                                    DownFile(absoluteUri, arule);
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
                                    var dirindex = arule.RowRules.FindIndex(r => r.Key == valtemp[2]) - rulesindex - 1;
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
                                        Uri baseUri = new Uri(arule.Url);
                                        Uri absoluteUri = new Uri(baseUri, n.Attributes[valtemp[1]].Value);
                                        var filename = System.IO.Path.Combine(arule.DataPath, arule.Name, arule.DownPath, dirname, absoluteUri.Segments[absoluteUri.Segments.Length - 1]);
                                        links += filename + ";";
                                        uris.Add(absoluteUri);
                                    }
                                }
                                if (uris.Count > 0)
                                {
                                    info += " " + rr.Key + ":" + links;
                                    l.Add(links);
                                    DownFiles(uris, arule, dirname);
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
                    var valrs1 = rr.Value.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var v in valrs1)
                    {
                        if (v.Contains("[标签名="))
                        {
                            var valtemp = v.Substring(1, v.Length - 2).Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
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
                        if (v.Contains("[属性名值="))
                        {
                            var valtemp = v.Substring(1, v.Length - 2).Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                            if (valtemp.Length >= 2)
                            {
                                valtemp = valtemp[1].Split(new string[] { "&" }, StringSplitOptions.RemoveEmptyEntries);
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
                        if (v.Contains("[行索引="))
                        {
                            continue;
                        }
                        if (v.Contains("[索引="))
                        {
                            var valtemp = v.Substring(1, v.Length - 2).Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
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
                        if (v.Contains("[首索引]"))
                        {
                            row1 = rowst1.First;
                        }
                        if (v.Contains("[尾索引]"))
                        {
                            row1 = rowst1.Last;
                        }
                        if (v.Contains("[上元素]"))
                        {
                            row1 = row1.PreviousElementSibling;
                        }
                        if (v.Contains("[下元素]"))
                        {
                            row1 = row1.NextElementSibling;
                        }
                        if (v.Contains("[父元素]"))
                        {
                            row1 = row1.Parent;
                        }
                        if (v.Contains("[子元素]"))
                        {
                            rowst1 = row1.Children;
                        }

                        if (v.Contains("[内容]") && row1 != null)
                        {
                            //if (!string.IsNullOrWhiteSpace(row1.Text()))
                            {
                                info += " " + rr.Key + ":" + row1.Text().Trim();
                                l.Add(row1.Text().Trim());
                                break;
                            }
                        }
                        if (v.Contains("[属性值=") && row1 != null)
                        {
                            var valtemp = v.Substring(1, v.Length - 2).Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                            if (valtemp.Length >= 2)
                            //if (!string.IsNullOrWhiteSpace(row1.Attributes[valtemp[1]]))
                            {
                                info += " " + rr.Key + ":" + row1.Attributes[valtemp[1]];
                                l.Add(row1.Attributes[valtemp[1]]);
                                break;
                            }
                        }
                        if (v.Contains("[源码]") && row1 != null)
                        {
                            //if (!string.IsNullOrWhiteSpace(row1.Html()))
                            {
                                info += " " + rr.Key + ":" + row1.Html().Trim();
                                l.Add(row1.Html().Trim());
                                break;
                            }
                        }
                        if (v.Contains("[内源码]") && row1 != null)
                        {
                            //if (!string.IsNullOrWhiteSpace(row1.Children.Html()))
                            {
                                info += " " + rr.Key + ":" + row1.Children.Html().Trim();
                                l.Add(row1.Children.Html().Trim());
                                break;
                            }
                        }
                        if (v.Contains("[链接]") && row1 != null)
                        {
                            var link = row1.Attr("href");
                            //if (!string.IsNullOrWhiteSpace(link))
                            {
                                Uri baseUri = new Uri(arule.Url);
                                Uri absoluteUri = new Uri(baseUri, link);

                                info += " " + rr.Key + ":" + absoluteUri.AbsoluteUri;
                                l.Add(absoluteUri.AbsoluteUri);
                                break;
                            }
                        }
                        if (v.Contains("[上节点内容]") && row1 != null)
                        {
                            var text = row1.PreviousSibling.Attr("text").Trim();
                            //if (!string.IsNullOrWhiteSpace(text))
                            {
                                info += " " + rr.Key + ":" + text;
                                l.Add(text);
                                break;
                            }
                        }
                        if (v.Contains("[下节点内容]") && row1 != null)
                        {
                            var text = row1.NextSibling.Attr("text").Trim();
                            //if (!string.IsNullOrWhiteSpace(text))
                            {
                                info += " " + rr.Key + ":" + text;
                                l.Add(text);
                                break;
                            }
                        }
                        if (v.Contains("[取属性值=") && row1 != null)
                        {
                            var valtemp = v.Substring(1, v.Length - 2).Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                            if (valtemp.Length >= 2)
                            {
                                var text = row1.Attr(valtemp[1]).Trim();
                                //if (!string.IsNullOrWhiteSpace(text))
                                {
                                    info += " " + rr.Key + ":" + text;
                                    l.Add(text);
                                    break;
                                }
                            }
                        }

                        if (v.Contains("[清除=") && row1 != null)
                        {
                            var valtemp = v.Substring(1, v.Length - 2).Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                            if (valtemp.Length >= 2)
                            {
                                valtemp = valtemp[1].Split(new string[] { "&" }, StringSplitOptions.RemoveEmptyEntries);
                                if (valtemp.Length >= 2)
                                {
                                    var tv = row1.Attr(valtemp[0]).Replace(valtemp[1], "");
                                    row1.Attr(valtemp[0], tv);
                                }
                            }

                        }

                        if (v.Contains("[下载链接]") && row1 != null)
                        {
                            var link = row1.Attr("href");
                            if (!string.IsNullOrWhiteSpace(link))
                            {
                                Uri baseUri = new Uri(arule.Url);
                                Uri absoluteUri = new Uri(baseUri, link);

                                info += " " + rr.Key + ":" + absoluteUri.AbsoluteUri;
                                l.Add(absoluteUri.AbsoluteUri);

                                DownFile(absoluteUri, arule);

                                break;
                            }
                        }
                        if (v.Contains("[下载图片]") && row1 != null)
                        {
                            var link = row1.Attr("src");
                            if (!string.IsNullOrWhiteSpace(link))
                            {
                                Uri baseUri = new Uri(arule.Url);
                                Uri absoluteUri = new Uri(baseUri, link);

                                info += " " + rr.Key + ":" + absoluteUri.AbsoluteUri;
                                l.Add(absoluteUri.AbsoluteUri);

                                DownFile(absoluteUri, arule);

                                break;
                            }
                        }
                        if (v.Contains("[下载=") && row1 != null)
                        {
                            var valtemp = v.Substring(1, v.Length - 2).Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                            if (valtemp.Length >= 2)
                            {
                                var link = row1.Attr(valtemp[1]);
                                if (!string.IsNullOrWhiteSpace(link))
                                {
                                    Uri baseUri = new Uri(arule.Url);
                                    Uri absoluteUri = new Uri(baseUri, link);

                                    info += " " + rr.Key + ":" + absoluteUri.AbsoluteUri;
                                    l.Add(absoluteUri.AbsoluteUri);

                                    DownFile(absoluteUri, arule);

                                    break;
                                }
                            }
                        }
                        if (v.Contains("[下载集合=") && rowst1 != null && rowst1.Count > 0)
                        {
                            var valtemp = v.Substring(1, v.Length - 2).Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                            if (valtemp.Length >= 2)
                            {
                                var dirname = string.Empty;
                                if (valtemp.Length == 3)
                                {
                                    var dirindex = arule.RowRules.FindIndex(r => r.Key == valtemp[2]) - rulesindex - 1;
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
                                        Uri baseUri = new Uri(arule.Url);
                                        Uri absoluteUri = new Uri(baseUri, r.Attr(valtemp[1]));
                                        var filename = System.IO.Path.Combine(arule.DataPath, arule.Name, arule.DownPath, dirname, absoluteUri.Segments[absoluteUri.Segments.Length - 1]);
                                        links += filename + ";";
                                        uris.Add(absoluteUri);
                                    }
                                }
                                if (uris.Count > 0)
                                {
                                    info += " " + rr.Key + ":" + links;
                                    l.Add(links);
                                    DownFiles(uris, arule, dirname);
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

        public static NSoup.Nodes.Element GetFirstRow(NSoup.Nodes.Element duan, AcquisitionRule arule, out int index)
        {
            NSoup.Nodes.Element row = null;
            NSoup.Nodes.Element row1 = null;
            NSoup.Select.Elements rowst1 = null;
            index = 0;
            var valrs1 = arule.RowRules[0].Value.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var v in valrs1)
            {
                if (v.Contains("[标签名="))
                {
                    var valtemp = v.Substring(1, v.Length - 2).Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
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
                if (v.Contains("[属性名值="))
                {
                    var valtemp = v.Substring(1, v.Length - 2).Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                    if (valtemp.Length >= 2)
                    {
                        valtemp = valtemp[1].Split(new string[] { "&" }, StringSplitOptions.RemoveEmptyEntries);
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
                if (v.Contains("[行索引="))
                {
                    var valtemp = v.Substring(1, v.Length - 2).Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                    if (valtemp.Length >= 2)
                    {
                        index = Convert.ToInt32(valtemp[1]);
                        row1 = rowst1[index];
                        if (row == null) { row = row1; return row; }
                    }
                }
                if (v.Contains("[索引="))
                {
                    var valtemp = v.Substring(1, v.Length - 2).Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
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
                if (v.Contains("[首索引]"))
                {
                    row1 = rowst1.First;
                }
                if (v.Contains("[尾索引]"))
                {
                    row1 = rowst1.Last;
                }
                if (v.Contains("[上元素]"))
                {
                    row1 = row1.PreviousElementSibling;
                }
                if (v.Contains("[下元素]"))
                {
                    row1 = row1.NextElementSibling;
                }
                if (v.Contains("[父元素]"))
                {
                    row1 = row1.Parent;
                }
                if (v.Contains("[子元素]"))
                {
                    rowst1 = row1.Children;
                }
                if (v.Contains("[内容]") && row1 != null)
                {
                    break;
                }
                if (v.Contains("[属性值=") && row1 != null)
                {
                    break;
                }
                if (v.Contains("[源码]") && row1 != null)
                {
                    break;
                }
                if (v.Contains("[内源码]") && row1 != null)
                {
                    break;
                }
                if (v.Contains("[链接]") && row1 != null)
                {
                    break;
                }
                if (v.Contains("[上节点内容]") && row1 != null)
                {
                    break;
                }
                if (v.Contains("[下节点内容]") && row1 != null)
                {
                    break;
                }
                if (v.Contains("[取属性值=") && row1 != null)
                {
                    break;
                }
                if (v.Contains("[清除=") && row1 != null)
                {
                    break;
                }
                if (v.Contains("[下载链接]") && row1 != null)
                {
                    break;
                }
                if (v.Contains("[下载图片]") && row1 != null)
                {
                    break;
                }
                if (v.Contains("[下载=") && row1 != null)
                {
                    break;
                }
                if (v.Contains("[下载集合=") && rowst1 != null && rowst1.Count > 0)
                {
                    break;
                }
            }
            return row;

        }

        public static Uri GetNextPage(NSoup.Nodes.Element duan, AcquisitionRule arule)
        {
            Uri uri = null;
            if (arule.NextPageRules == null) { return null; }
            if (string.IsNullOrWhiteSpace(arule.NextPageRules.Value)) { return null; }
            NSoup.Nodes.Element row1 = null;
            NSoup.Select.Elements rowst1 = null;
            var valrs1 = arule.NextPageRules.Value.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var v in valrs1)
            {
                if (v.Contains("[标签名="))
                {
                    var valtemp = v.Substring(1, v.Length - 2).Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
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
                if (v.Contains("[属性名值="))
                {
                    var valtemp = v.Substring(1, v.Length - 2).Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                    if (valtemp.Length >= 2)
                    {
                        valtemp = valtemp[1].Split(new string[] { "&" }, StringSplitOptions.RemoveEmptyEntries);
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
                if (v.Contains("[行索引="))
                {
                    continue;
                }
                if (v.Contains("[索引="))
                {
                    var valtemp = v.Substring(1, v.Length - 2).Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
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
                if (v.Contains("[首索引]"))
                {
                    row1 = rowst1.First;
                }
                if (v.Contains("[尾索引]"))
                {
                    row1 = rowst1.Last;
                }
                if (v.Contains("[上元素]"))
                {
                    row1 = row1.PreviousElementSibling;
                }
                if (v.Contains("[下元素]"))
                {
                    row1 = row1.NextElementSibling;
                }
                if (v.Contains("[父元素]"))
                {
                    row1 = row1.Parent;
                }
                if (v.Contains("[子元素]"))
                {
                    rowst1 = row1.Children;
                }

                if (v.Contains("[内容]") && row1 != null)
                {
                    break;
                }
                if (v.Contains("[属性值=") && row1 != null)
                {
                    break;
                }
                if (v.Contains("[源码]") && row1 != null)
                {
                    break;
                }
                if (v.Contains("[内源码]") && row1 != null)
                {
                    break;
                }
                if (v.Contains("[链接]") && row1 != null)
                {
                    var link = row1.Attr("href");
                    if (!string.IsNullOrWhiteSpace(link))
                    {
                        uri = new Uri(new Uri(arule.Url), link);
                        break;
                    }
                }
                if (v.Contains("[上节点内容]") && row1 != null)
                {
                    break;
                }
                if (v.Contains("[下节点内容]") && row1 != null)
                {
                    break;
                }
                if (v.Contains("[取属性值=") && row1 != null)
                {
                    break;
                }
                if (v.Contains("[清除=") && row1 != null)
                {
                    break;
                }
                if (v.Contains("[下载链接]") && row1 != null)
                {
                    break;
                }
                if (v.Contains("[下载图片]") && row1 != null)
                {
                    break;
                }
                if (v.Contains("[下载=") && row1 != null)
                {
                    break;
                }
                if (v.Contains("[下载集合=") && rowst1 != null && rowst1.Count > 0)
                {
                    break;
                }
            }

            return uri;

        }

        public static string FilterFileNamePath(string filenamepath)
        {
            string illegal = filenamepath;
            string regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            Regex r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
            illegal = r.Replace(illegal, "");
            return illegal;
        }
    }
}
