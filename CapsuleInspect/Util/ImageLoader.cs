using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapsuleInspect.Util
{
    public class ImageLoader : IDisposable
    {
        private List<string> _sortedImages;
        private int _grabIndex = -1;

        public bool CyclicMode { get; set; } = true;

        public ImageLoader() { }

        public bool LoadImages(string imageDir)
        {
            if (!Directory.Exists(imageDir))
                return false;

            _sortedImages = ImageFileSorter.GetSortedImages(imageDir);
            if (_sortedImages.Count() <= 0)
                return false;

            _grabIndex = -1;

            return true;
        }

        public bool IsLoadedImages()
        {
            if (_sortedImages is null)
                return false;

            if (_sortedImages.Count() <= 0)
                return false;

            return true;
        }

        public bool Reset()
        {
            _grabIndex = -1;
            return true;
        }

        public string GetImagePath()
        {
            if (_sortedImages is null)
                return "";

            _grabIndex++;

            if (_grabIndex >= _sortedImages.Count)
            {
                if (CyclicMode == false)
                    return "";

                _grabIndex = 0;
            }

            return _sortedImages[_grabIndex];
        }

        public string GetNextImagePath(bool reset = false)
        {
            if (reset)
                Reset();

            return GetImagePath();
        }

        // === ⬇️ 추가: 현재 상태를 외부에 노출하는 읽기 전용 프로퍼티들 ===

        /// <summary>
        /// 현재 그랩된 인덱스 (-1이면 아직 GetImagePath가 호출되지 않음)
        /// </summary>
        public int CurrentIndex
        {
            get { return _grabIndex; }
        }

        /// <summary>
        /// 현재 이미지의 전체 경로 (없으면 빈 문자열)
        /// </summary>
        public string CurrentFilePath
        {
            get
            {
                if (_sortedImages == null) return string.Empty;
                if (_grabIndex < 0 || _grabIndex >= _sortedImages.Count) return string.Empty;
                return _sortedImages[_grabIndex] ?? string.Empty;
            }
        }

        /// <summary>
        /// 정렬된 이미지 목록(읽기 전용)
        /// </summary>
        public IReadOnlyList<string> FileList
        {
            get { return _sortedImages?.AsReadOnly(); }
        }

        #region Dispose

        private bool _disposed = false; // to detect redundant calls

        protected void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            _disposed = true;
        }
        ~ImageLoader()
        {
            Dispose(disposing: false);
        }
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
