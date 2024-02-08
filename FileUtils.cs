using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace util
{
     class FileUtil
    {
        /// <summary>
        /// 폴더의 하위 항목을 가져오는 함수
        /// </summary>
        /// <param name="directoryPath">타겟 디렉토리</param>
        /// <returns></returns>
        public static List<string> GetFileList(string directoryPath) 
        {
            List<string> fileList =  new List<string>();
            try
            {
                // 폴더 내의 파일 목록 가져오기
                string[] files = Directory.GetFiles(directoryPath);
                fileList.AddRange(files);

                // 병렬로 폴더 내의 하위 폴더 목록 가져오기
                string[] directories = Directory.GetDirectories(directoryPath);
                Parallel.ForEach(directories, directory =>
                {
                    try
                    {
                        fileList.Add(directory);
                        fileList.AddRange(GetFileList(directory));
                    }
                    catch (UnauthorizedAccessException){ } // 디렉토리에 액세스할 권한이 없는 경우 무시
                    catch (DirectoryNotFoundException) { } // 디렉토리를 찾을 수 없는 경우 무시
                });
            }
            catch (UnauthorizedAccessException) { }// 디렉토리에 액세스할 권한이 없는 경우 무시
            catch (DirectoryNotFoundException) { }// 디렉토리를 찾을 수 없는 경우 무시
            return fileList;
        }

        /// <summary>
        /// 맵 형태로 디렉토리 경로를 key값으로 파일 항목을 list 로 반환하는 함수
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <returns></returns>
        public static Dictionary<string, List<string>> GetFolderFilesMap(string directoryPath)
        {
            Dictionary<string, List<string>> folderFilesMap = new Dictionary<string, List<string>>();

            try
            {
                // 폴더 내의 파일 목록 가져오기
                string[] files = Directory.GetFiles(directoryPath);
                if (files.Length > 0)
                {
                    folderFilesMap[directoryPath] = files.ToList();
                }

                // 병렬로 폴더 내의 하위 폴더 목록 가져오기
                string[] directories = Directory.GetDirectories(directoryPath);
                Parallel.ForEach(directories, directory =>
                {
                    try
                    {
                        Dictionary<string, List<string>> subFolderFilesMap = GetFolderFilesMap(directory);
                        if (subFolderFilesMap.Any())
                        {
                            lock (folderFilesMap)
                            {
                                folderFilesMap[directory] = subFolderFilesMap.Values.SelectMany(x => x).ToList();
                            }
                        }
                    }
                    catch (UnauthorizedAccessException) { } // 디렉토리에 액세스할 권한이 없는 경우 무시
                    catch (DirectoryNotFoundException) { }  // 디렉토리를 찾을 수 없는 경우 무시
                });
            }
            catch (UnauthorizedAccessException) { }// 디렉토리에 액세스할 권한이 없는 경우 무시
            catch (DirectoryNotFoundException) { }// 디렉토리를 찾을 수 없는 경우 무시
            return folderFilesMap;
        }

    }
}
