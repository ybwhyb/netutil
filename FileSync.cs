using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace util
{
    public class FileSync : IDisposable
    {
        private FileSystemWatcher _watcher;
        private string _srcFolder;

        private FileSync(string folderToWatch)
        {
            _srcFolder = folderToWatch;
            InitializeWatcher();
        }

        public static Builder newBuilder(string folderToWatch)
        {
            return new Builder(folderToWatch);
        }

        private void InitializeWatcher()
        {
            _watcher = new FileSystemWatcher();
            _watcher.Path = _srcFolder;
            _watcher.IncludeSubdirectories = true;

            // 변경 이벤트에 대한 핸들러 등록
            _watcher.Created += OnChanged;
            _watcher.Changed += OnChanged;
            _watcher.Deleted += OnChanged;
            _watcher.Renamed += OnChanged;
        }

        public void StartMonitoring()
        {
            _watcher.EnableRaisingEvents = true;
            Console.WriteLine($"폴더 {_srcFolder} 감시를 시작합니다...");
        }

        public void StopMonitoring()
        {
            _watcher.EnableRaisingEvents = false;
            Console.WriteLine($"폴더 {_srcFolder} 감시를 중지합니다.");
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            // 변경된 파일 이름 출력
            Console.WriteLine($"변경된 파일: {e.FullPath}");

            // todo: 여기에 동기화하는 코드를 추가
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _watcher.Dispose();
            }
        }

        public class Builder
        {
            private readonly FileSync _instance;

            public Builder(string folderToWatch)
            {
                _instance = new FileSync(folderToWatch);
            }

            public FileSync Build()
            {
                return _instance;
            }
        }
    }
}
