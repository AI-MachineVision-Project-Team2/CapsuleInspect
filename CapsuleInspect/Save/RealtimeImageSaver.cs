// RealtimeImageSaver.cs
using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using static CapsuleInspect.ImageSaveHelper;

namespace CapsuleInspect
{
    public sealed class RealtimeImageSaver : IDisposable
    {
        private readonly BlockingCollection<SaveJob> _queue;
        private readonly CancellationTokenSource _cts;
        private readonly Task _worker;
        private readonly bool _useDateFolder;

        // 이 이벤트가 꼭 있어야 MainForm 구독 코드가 컴파일됩니다.
        public event Action<string> OnSaved;

        public RealtimeImageSaver(int boundedCapacity = 200, bool useDateFolder = false)
        {
            _queue = new BlockingCollection<SaveJob>(boundedCapacity);
            _cts = new CancellationTokenSource();
            _useDateFolder = useDateFolder;

            _worker = Task.Run(async () =>
            {
                foreach (var job in _queue.GetConsumingEnumerable(_cts.Token))
                {
                    string savedPath = null;
                    try
                    {
                        string name = $"{job.Prefix}_{job.Timestamp:yyyyMMdd_HHmmss_fff}_{job.Id}";
                        savedPath = ImageSaveHelper.SavePng(job.Bitmap, job.Category, name, _useDateFolder);
                    }
                    catch
                    {
                        // 필요시 로깅
                    }
                    finally
                    {
                        job.Bitmap.Dispose();
                    }

                    // 저장 성공 알림
                    if (savedPath != null)
                        OnSaved?.Invoke(savedPath);

                    await Task.Yield();
                }
            }, _cts.Token);
        }

        public void Enqueue(Bitmap srcBitmap, Category category, string prefix = "Capsule")
        {
            if (srcBitmap == null) return;

            // 원본과 분리된 복사본
            Bitmap copy = new Bitmap(srcBitmap.Width, srcBitmap.Height, srcBitmap.PixelFormat);
            using (Graphics g = Graphics.FromImage(copy))
                g.DrawImageUnscaled(srcBitmap, 0, 0);

            var job = new SaveJob
            {
                Bitmap = copy,
                Category = category,
                Prefix = prefix,
                Timestamp = DateTime.Now,
                Id = Guid.NewGuid().ToString("N").Substring(0, 8)
            };

            if (!_queue.TryAdd(job))
            {
                // 큐가 가득 찼으면 오래된 작업 드롭 후 추가
                if (_queue.TryTake(out var dropped))
                {
                    dropped.Bitmap.Dispose();
                    _queue.Add(job);
                }
                else
                {
                    job.Bitmap.Dispose();
                }
            }
        }

        public void Dispose()
        {
            _queue.CompleteAdding();
            _cts.Cancel();
            try { _worker.Wait(2000); } catch { }
            _cts.Dispose();
            while (_queue.TryTake(out var leftover))
                leftover.Bitmap.Dispose();
            _queue.Dispose();
        }

        private class SaveJob
        {
            public Bitmap Bitmap;
            public Category Category;
            public string Prefix;
            public DateTime Timestamp;
            public string Id;
        }
    }
}
