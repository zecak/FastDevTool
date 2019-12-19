using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastDevTool.Common
{
    public class Paging : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public Paging()
        {
            PageSize = 20;
            PageIndex = 1;
            count = 0;
            pageCount = 1;
            Where = "";
            MaxPageCount = 9;
        }

        public Paging(int size, int index)
        {
            PageSize = size;
            PageIndex = index;
            count = 0;
            pageCount = 1;
            Where = "";
            MaxPageCount = 9;
        }

        /// <summary>
        /// 每页大小
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// 页码
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// 记录集总数
        /// </summary>
        private int count;
        public int Count
        {
            get { return count; }
            set
            {
                count = value;

                //纠正页码
                if (PageSize == 0) { PageSize = 10; }
                pageCount = count / PageSize;
                if (((count * 1.0) % PageSize) > 0)
                {
                    pageCount++;
                }
                if (PageIndex > pageCount)
                {
                    PageIndex = pageCount;
                }
                if (PageIndex == 0) { PageIndex = 1; }

                if (pageCount < maxPageCount) { maxPageCount = pageCount; }

                SetStartAndEndIndex();
            }
        }

        /// <summary>
        /// 总页数
        /// </summary>
        private int pageCount;
        public int PageCount
        {
            get { return pageCount; }
            set { pageCount = value; }
        }

        private int maxPageCount;
        /// <summary>
        /// 最多显示的页数:最大页数为偶数时有问题
        /// </summary>
        public int MaxPageCount
        {
            get
            {
                if (pageCount < maxPageCount) { return pageCount; }
                return maxPageCount;
            }
            set
            {
                maxPageCount = value; SetStartAndEndIndex();
            }
        }

        public int StartIndex { get; set; }
        public int EndIndex { get; set; }

        void SetStartAndEndIndex()
        {
            if (PageIndex <= MaxPageCount / 2 + 1) { StartIndex = 0; EndIndex = MaxPageCount; }
            else if (PageIndex + MaxPageCount / 2 > PageCount) { StartIndex = PageCount - MaxPageCount; EndIndex = PageCount; }
            else
            {
                StartIndex = PageIndex - MaxPageCount / 2 - 1;
                EndIndex = PageIndex + MaxPageCount / 2;
            }
        }

        /// <summary>
        /// 分页条件
        /// </summary>
        public string Where { get; set; }

        /// <summary>
        /// 第一页页码
        /// </summary>
        public int First
        {
            get
            {
                return 1;
            }
        }

        /// <summary>
        /// 最后一页页码
        /// </summary>
        public int Last
        {
            get
            {
                return pageCount;
            }
        }

        /// <summary>
        /// 上一页页码
        /// </summary>
        public int Previous
        {
            get
            {
                if (PageIndex - 1 <= 0) { return 1; }
                return PageIndex - 1;
            }
        }

        /// <summary>
        /// 下一页页码
        /// </summary>
        public int Next
        {
            get
            {
                if (PageIndex + 1 >= pageCount) { return pageCount; }
                return PageIndex + 1;
            }
        }
    }
}
