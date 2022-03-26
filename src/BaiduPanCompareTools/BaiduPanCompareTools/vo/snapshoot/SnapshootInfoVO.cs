using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BaiduPanCompareTools
{
    /// <summary>
    /// 一次用户保存的快照信息
    /// </summary>
    internal class SnapshootInfoVO
    {
        // 网盘地址
        public string baiduPanUrl { get; set; }
        // 网盘提取码
        public string baiduPanAccessCode { get; set; }
        // 保存的哪个目录的快照（如果是根目录则为“/”）
        public string forDirPath { get; set; }
        // 保存时的时间戳（秒）
        public int saveTimestamp { get; set; }
        // 该路径下包含的子文件夹或子文件
        public List<DirOrFileInfoVO> childs { get; set; }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        /// <summary>
        /// 反序列化json为Snapshoot类，不能使用JsonConvert.DeserializeObject<SnapshootInfo>(json)，
        /// 因为表示一个文件夹下的子文件FileInfoVO或子文件夹DirInfoVO，需要用其父类List<DirOrFileInfoVO>，
        /// 如果直接用json库的反序列化，FileInfoVO和DirInfoVO中特有的字段将丢失
        /// </summary>
        public static SnapshootInfoVO FromJson(string json, out string errorString)
        {
            SnapshootInfoVO snapshoot = new SnapshootInfoVO();

            try
            {
                JObject jObject = JsonConvert.DeserializeObject(json) as JObject;

                snapshoot.baiduPanUrl = jObject.Value<string>("baiduPanUrl");
                snapshoot.baiduPanAccessCode = jObject.Value<string>("baiduPanAccessCode");
                snapshoot.forDirPath = jObject.Value<string>("forDirPath");
                snapshoot.saveTimestamp = jObject.Value<int>("saveTimestamp");

                JArray childsArray = jObject["childs"] as JArray;
                snapshoot.childs = GetAllChilds(childsArray);

                errorString = null;
                return snapshoot;
            }
            catch (Exception ex)
            {
                errorString = ex.ToString();
                return null;
            }
        }

        private static List<DirOrFileInfoVO> GetAllChilds(JArray childsArray)
        {
            List<DirOrFileInfoVO> resultList = new List<DirOrFileInfoVO>();

            foreach (JObject oneDirOrFileObject in childsArray)
            {
                bool isDir = oneDirOrFileObject.Value<bool>("isDir");
                string name = oneDirOrFileObject.Value<string>("name");
                long fsId = oneDirOrFileObject.Value<long>("fsId");
                int serverModifyTimestamp = oneDirOrFileObject.Value<int>("serverModifyTimestamp");
                if (isDir == true)
                {
                    DirInfoVO childDir = new DirInfoVO();
                    childDir.name = name;
                    childDir.fsId = fsId;
                    childDir.serverModifyTimestamp = serverModifyTimestamp;
                    JArray childs = oneDirOrFileObject["childs"] as JArray;
                    childDir.childs = GetAllChilds(childs);
                    resultList.Add(childDir);
                }
                else
                {
                    FileInfoVO fileInfo = new FileInfoVO();
                    fileInfo.name = name;
                    fileInfo.fsId = fsId;
                    fileInfo.serverModifyTimestamp = serverModifyTimestamp;
                    fileInfo.baiduMd5 = oneDirOrFileObject.Value<string>("baiduMd5");
                    if (oneDirOrFileObject.ContainsKey("localMd5"))
                        fileInfo.localMd5 = oneDirOrFileObject.Value<string>("localMd5");

                    fileInfo.fileSize = oneDirOrFileObject.Value<long>("fileSize");
                    resultList.Add(fileInfo);
                }
            }

            return resultList;
        }
    }
}
