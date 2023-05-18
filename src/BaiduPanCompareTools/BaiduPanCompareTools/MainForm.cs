using BaiduPanCompareTools.utils;
using BaiduPanCompareTools.vo.diff;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace BaiduPanCompareTools
{
    public partial class MainForm : Form
    {
        // 上一次发起share/list请求的时间
        DateTime _LastReqShareListTime = DateTime.Now;
        // 用户设置的请求百度网盘的时间间隔（毫秒）
        int _RequestIntervalMillisecond = 0;

        public MainForm()
        {
            InitializeComponent();

            TxtBaiduPanUrl.AllowDrop = true;
            TxtBaiduPanUrl.DragEnter += new DragEventHandler(UiUtil.TextBoxDragEnter);
            TxtBaiduPanUrl.DragDrop += new DragEventHandler(BaiduPanUrlAndAccessCodeTextBoxDragDrop);
            TxtBaiduPanAccessCode.AllowDrop = true;
            TxtBaiduPanAccessCode.DragEnter += new DragEventHandler(UiUtil.TextBoxDragEnter);
            TxtBaiduPanAccessCode.DragDrop += new DragEventHandler(BaiduPanUrlAndAccessCodeTextBoxDragDrop);

            TxtOldSnapshootFilePath.AllowDrop = true;
            TxtOldSnapshootFilePath.DragEnter += new DragEventHandler(UiUtil.TextBoxDragEnter);
            TxtOldSnapshootFilePath.DragDrop += new DragEventHandler(UiUtil.TextBoxOneFileDragDrop);
            TxtNewSnapshootFilePath.AllowDrop = true;
            TxtNewSnapshootFilePath.DragEnter += new DragEventHandler(UiUtil.TextBoxDragEnter);
            TxtNewSnapshootFilePath.DragDrop += new DragEventHandler(UiUtil.TextBoxOneFileDragDrop);
            TxtSnapshootFilePath.AllowDrop = true;
            TxtSnapshootFilePath.DragEnter += new DragEventHandler(UiUtil.TextBoxDragEnter);
            TxtSnapshootFilePath.DragDrop += new DragEventHandler(UiUtil.TextBoxOneFileDragDrop);
            TxtLocalDirPath.AllowDrop = true;
            TxtLocalDirPath.DragEnter += new DragEventHandler(UiUtil.TextBoxDragEnter);
            TxtLocalDirPath.DragDrop += new DragEventHandler(UiUtil.TextBoxOneDirDragDrop);

            // 忽略计算MD5的文件大小单位，默认选为MB
            Cbo.SelectedIndex = 2;
        }

        private void BaiduPanUrlAndAccessCodeTextBoxDragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop) == true)
            {
                Array dragDropFileArray = e.Data.GetData(DataFormats.FileDrop) as Array;
                if (dragDropFileArray.Length != 1)
                {
                    MessageBox.Show("若要通过拖拽进一个快照文件，读取并自动填上其中存储的百度网盘链接和提取码，请只拖入一个快照文件", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                string path = dragDropFileArray.GetValue(0).ToString();
                if (Directory.Exists(path) == true)
                {
                    MessageBox.Show("若要通过拖拽进一个快照文件，读取并自动填上其中存储的百度网盘链接和提取码，请拖入一个快照文件而不是文件夹", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else
                {
                    if (File.Exists(path))
                    {
                        string snapshootJson;
                        SnapshootInfoVO snapshootInfo;
                        string errorString;
                        try
                        {
                            snapshootJson = File.ReadAllText(path);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(this, $"尝试通过拖拽进一个快照文件，读取并自动填上其中存储的百度网盘链接和提取码，但读取快照文件失败，异常信息为：\n{ex.ToString()}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        snapshootInfo = SnapshootInfoVO.FromJson(snapshootJson, out errorString);
                        if (errorString != null)
                        {
                            MessageBox.Show(this, $"尝试通过拖拽进一个快照文件，读取并自动填上其中存储的百度网盘链接和提取码，但快照文件解析错误，请使用本工具生成的快照文件，异常信息为：\n{errorString}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        TxtBaiduPanUrl.Text = snapshootInfo.baiduPanUrl;
                        TxtBaiduPanAccessCode.Text = snapshootInfo.baiduPanAccessCode;
                    }
                    else
                    {
                        MessageBox.Show("若要通过拖拽进一个快照文件，读取并自动填上其中存储的百度网盘链接和提取码，请拖入一个快照文件", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    if (Path.GetExtension(path) != $".{AppConsts.SNAPSHOOT_FILE_EXTENSION}")
                    {
                        MessageBox.Show($"若要通过拖拽进一个快照文件，读取并自动填上其中存储的百度网盘链接和提取码，请拖入一个扩展名为{AppConsts.SNAPSHOOT_FILE_EXTENSION}的快照文件", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
            }
            else
            {
                MessageBox.Show($"若要通过拖拽进一个快照文件，读取并自动填上其中存储的百度网盘链接和提取码，请正确拖入一个扩展名为{AppConsts.SNAPSHOOT_FILE_EXTENSION}的快照文件", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void BtnGenerateSnapshoot_Click(object sender, EventArgs e)
        {
            string baiduPanUrl = null;
            string baiduPanAccessCode = null;
            string targetDir = null;
            int waitServerMaxMillisecond = 0;

            string returnContent;
            string errorString;

            string shareId;
            string shareUk;
            // 链接根目录的地址前缀（如果不是从网盘根目录开始分享的，就会有以/sharelink开头的地址）
            string rootDirPath;

            SnapshootInfoVO snapShotInfo = new SnapshootInfoVO();

            /**
             * 检查用户输入项目是否正确
             */
            // 检查输入的网盘地址
            baiduPanUrl = TxtBaiduPanUrl.Text.Trim();
            if (string.IsNullOrEmpty(baiduPanUrl))
            {
                MessageBox.Show(this, "请输入百度网盘链接地址", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (baiduPanUrl.StartsWith(AppConsts.BAIDU_PAN_URL_PREFIX, StringComparison.CurrentCultureIgnoreCase) == false)
            {
                MessageBox.Show(this, $"输入的百度网盘链接地址非法，请输入以{AppConsts.BAIDU_PAN_URL_PREFIX}开头的地址", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            // 如果是带有提取码的链接，从中获取提取码
            int baiduPanUrlAccessCodeParamNameIndex = baiduPanUrl.IndexOf(AppConsts.BAIDU_PAN_URL_ACCESS_CODE_PARAM_NAME, StringComparison.CurrentCultureIgnoreCase);
            if (baiduPanUrlAccessCodeParamNameIndex != -1)
            {
                baiduPanAccessCode = baiduPanUrl.Substring(baiduPanUrlAccessCodeParamNameIndex + AppConsts.BAIDU_PAN_URL_ACCESS_CODE_PARAM_NAME.Length);
                baiduPanUrl = baiduPanUrl.Substring(0, baiduPanUrlAccessCodeParamNameIndex);
                if (AppConsts.CheckBaiduPanAccessCodeFormat(baiduPanAccessCode, out errorString) == false)
                {
                    MessageBox.Show(this, $"从百度网盘链接地址中获取的提取码{baiduPanAccessCode}错误，{errorString}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            // 检查输入的提取码
            string inputBaiduPanAccessCode = TxtBaiduPanAccessCode.Text.Trim();
            if (baiduPanUrlAccessCodeParamNameIndex != -1)
            {
                // 如果输入的百度网盘链接带提取码，而用户又在提取码输入框输入了，要保证一致
                if (string.IsNullOrEmpty(inputBaiduPanAccessCode) == false)
                {
                    if (inputBaiduPanAccessCode.Equals(baiduPanAccessCode, StringComparison.CurrentCultureIgnoreCase) == false)
                    {
                        MessageBox.Show(this, $"从百度网盘链接地址中获取的提取码{baiduPanAccessCode}与在提取码输入框中输入的{inputBaiduPanAccessCode}不一致", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
            }
            else
            {
                // 如果输入的百度网盘链接不带提取码，则需要输入提取码
                if (string.IsNullOrEmpty(inputBaiduPanAccessCode) == true)
                {
                    MessageBox.Show(this, "请输入对应的提取码", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (AppConsts.CheckBaiduPanAccessCodeFormat(inputBaiduPanAccessCode, out errorString) == false)
                {
                    MessageBox.Show(this, $"输入的提取码{baiduPanAccessCode}错误，{errorString}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                baiduPanAccessCode = inputBaiduPanAccessCode;
            }
            // 检查输入的要建立快照的目录
            targetDir = TxtTargetDir.Text.Trim();
            if (string.IsNullOrEmpty(targetDir) == true)
            {
                MessageBox.Show(this, "请输入要建立快照的目录路径，如果从分享的链接对应的根目录进行建立，请输入“/”", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            // 检查输入的请求间隔时间
            string inputRequestIntervalString = TxtRequestInterval.Text.Trim();
            if (int.TryParse(inputRequestIntervalString, out _RequestIntervalMillisecond) == false)
            {
                MessageBox.Show(this, "输入的打开子文件夹间隔时间不是合法数字", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (_RequestIntervalMillisecond < 0)
            {
                MessageBox.Show(this, "输入的打开子文件夹间隔时间不能小于0", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            // 检查输入的最长等待百度网盘服务器响应的时间
            string inputWaitServerString = TxtWaitServerMaxMillisecond.Text.Trim();
            if (int.TryParse(inputWaitServerString, out waitServerMaxMillisecond) == false)
            {
                MessageBox.Show(this, "输入的等待百度网盘服务器响应的最长时间不是合法数字", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (waitServerMaxMillisecond <= 0)
            {
                MessageBox.Show(this, "输入的等待百度网盘服务器响应的最长时间必须大于0", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            AppendTextToConsole("=====================================================");
            AppendTextToConsole($"百度网盘地址：{baiduPanUrl}");
            AppendTextToConsole($"对应的提取码：{baiduPanAccessCode}");
            AppendTextToConsole($"要建立快照的目录：{targetDir}");

            AppendTextToConsole($"{Environment.NewLine}请求访问百度网盘链接：");

            snapShotInfo.baiduPanUrl = baiduPanUrl;
            snapShotInfo.baiduPanAccessCode = baiduPanAccessCode;
            snapShotInfo.forDirPath = targetDir;
            snapShotInfo.saveTimestamp = DateTimeUtil.GetCurrentTimestampSecond();

            /**
             * 验证输入的百度网盘地址是否有效
             */
            CookieContainer cookieContainer = new CookieContainer();
            HttpClientHandler handler = new HttpClientHandler();
            // 允许跳转，当请求https://pan.baidu.com/s/1XXXXX时，能自动根据返回的跳转地址到https://pan.baidu.com/share/init?surl=XXXXX
            handler.AllowAutoRedirect = true;
            handler.CheckCertificateRevocationList = false;
            handler.CookieContainer = cookieContainer;
            handler.UseCookies = true;

            HttpClient httpClient = new HttpClient(handler);
            httpClient.Timeout = TimeSpan.FromMilliseconds(waitServerMaxMillisecond);

            HttpRequestMessage baiduUrlReq = new HttpRequestMessage();
            baiduUrlReq.Method = HttpMethod.Get;
            baiduUrlReq.RequestUri = new Uri(baiduPanUrl);

            Dictionary<string, string> baiduUrlHeaders = new Dictionary<string, string>();
            baiduUrlHeaders.Add("Host", "pan.baidu.com");
            baiduUrlHeaders.Add("User-Agent", AppConsts.USER_AGENT);

            HttpRequestResultEnum reqBaiduUrlResultEnum = HttpUtil.DoGet(httpClient, baiduUrlReq, baiduUrlHeaders, out returnContent, out errorString);
            if (reqBaiduUrlResultEnum == HttpRequestResultEnum.NotFound)
            {
                MessageBox.Show(this, "输入的百度网盘链接地址无效", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else if (reqBaiduUrlResultEnum == HttpRequestResultEnum.Timeout)
            {
                MessageBox.Show(this, "请求百度网盘超时，请在网速较好时重试或者调大最长等待时间", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else if (reqBaiduUrlResultEnum != HttpRequestResultEnum.Ok)
            {
                MessageBox.Show(this, $"请求百度网盘（{baiduPanUrl}）发生错误，异常信息为：\n{errorString}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (returnContent.Contains(AppConsts.BAIDU_PAN_URL_NOT_FOUND_TIPS))
            {
                MessageBox.Show(this, $"该百度网盘链接（{baiduPanUrl}）已不存在", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            // 从页面中根据errortype判断链接是否有效
            {
                Regex regex = new Regex("\"errortype\":(.*?),");
                Match match = regex.Match(returnContent);
                if (match.Success)
                {
                    string errorTypeStriing = match.Groups[1].Value;
                    int errorType;
                    if (int.TryParse(errorTypeStriing, out errorType) == false)
                    {
                        MessageBox.Show(this, $"请求百度网盘链接（{baiduPanUrl}）发生错误，无法从返回的HTML中，将errortype转为数字，其值为{errorType}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    if (errorType != -1)
                    {
                        if (AppConsts.ERROR_TYPE_TO_DESC.ContainsKey(errorType) == true)
                        {
                            MessageBox.Show(this, $"该百度网盘链接（{baiduPanUrl}）已失效，具体原因为：{AppConsts.ERROR_TYPE_TO_DESC[errorType]}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        else
                        {
                            MessageBox.Show(this, $"该百度网盘链接（{baiduPanUrl}）已失效，但没有对应的错误码描述，返回错误码为：{errorType}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                }
                else
                {
                    MessageBox.Show(this, $"请求百度网盘链接（{baiduPanUrl}）发生错误，无法从返回的HTML中，找到errortype，HTML内容为：{returnContent}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            if (returnContent.Contains(AppConsts.BAIDU_PAN_URL_INPUT_ACCESS_CODE_TIPS) == false)
            {
                MessageBox.Show(this, $"请求百度网盘（{baiduPanUrl}）发生错误，返回的页面信息中不包含“{AppConsts.BAIDU_PAN_URL_INPUT_ACCESS_CODE_TIPS}”，本程序无法判断其是否为百度网盘页面", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            AppendTextToConsole("链接有效");
            if (AppConsts.REQ_BAIDU_PAN_API_INTERVAL > 0)
                Thread.Sleep(AppConsts.REQ_BAIDU_PAN_API_INTERVAL);
            AppendTextToConsole("请求验证提取码：");
            /**
             * 验证输入的提取码是否正确
             */
            // surl为请求的百度网盘地址https://pan.baidu.com/s/1XXXXX，去掉1之后的剩余XXXXX部分
            string surl = baiduPanUrl.Substring(AppConsts.BAIDU_PAN_URL_PREFIX.Length + 1);
            string verifyUrl = $"http://pan.baidu.com/share/verify?channel=chunlei&clienttype=0&web=1&app_id=250528&surl={surl}";
            HttpRequestMessage verifyReq = new HttpRequestMessage();
            verifyReq.Method = HttpMethod.Post;
            verifyReq.RequestUri = new Uri(verifyUrl);

            Dictionary<string, string> verifyHeaders = new Dictionary<string, string>();
            verifyHeaders.Add("Host", "pan.baidu.com");
            verifyHeaders.Add("User-Agent", AppConsts.USER_AGENT);
            verifyHeaders.Add("Referer", "https://pan.baidu.com/disk/home");

            Dictionary<string, string> verifyBodies = new Dictionary<string, string>();
            verifyBodies.Add("pwd", baiduPanAccessCode);

            HttpRequestResultEnum reqVerifyResultEnum = HttpUtil.DoPostForForm(httpClient, verifyReq, verifyHeaders, verifyBodies, out returnContent, out errorString);
            if (reqVerifyResultEnum == HttpRequestResultEnum.Timeout)
            {
                MessageBox.Show(this, "验证提取码超时，请在网速较好时重试或者调大最长等待时间", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else if (reqVerifyResultEnum != HttpRequestResultEnum.Ok)
            {
                MessageBox.Show(this, $"验证提取码发生错误，异常信息为：\n{errorString}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (returnContent.Contains("\"errno\":0") == false)
            {
                MessageBox.Show(this, $"提取码校验失败，请更正提取码后重试\n百度网盘服务器返回信息为：{returnContent}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            AppendTextToConsole("提取码正确");
            if (AppConsts.REQ_BAIDU_PAN_API_INTERVAL > 0)
                Thread.Sleep(AppConsts.REQ_BAIDU_PAN_API_INTERVAL);
            AppendTextToConsole("再次请求访问百度网盘链接：");
            /**
             * 通过校验后，再次访问百度网盘链接，因为之前校验通过后服务器返回名为BDCLND的Cookie，带着此Cookie重新访问百度网盘地址就能看到分享的文件
             */
            HttpRequestMessage refreshBaiduUrlReq = new HttpRequestMessage();
            refreshBaiduUrlReq.Method = HttpMethod.Get;
            refreshBaiduUrlReq.RequestUri = new Uri(baiduPanUrl);

            Dictionary<string, string> refreshBaiduUrlHeaders = new Dictionary<string, string>();
            refreshBaiduUrlHeaders.Add("Host", "pan.baidu.com");
            refreshBaiduUrlHeaders.Add("User-Agent", AppConsts.USER_AGENT);
            refreshBaiduUrlHeaders.Add("Referer", "https://pan.baidu.com/disk/home");

            HttpRequestResultEnum refreshBaiduUrlResultEnum = HttpUtil.DoGet(httpClient, refreshBaiduUrlReq, refreshBaiduUrlHeaders, out returnContent, out errorString);
            if (refreshBaiduUrlResultEnum == HttpRequestResultEnum.Timeout)
            {
                MessageBox.Show(this, "刷新请求百度网盘超时，请在网速较好时重试或者调大最长等待时间", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else if (refreshBaiduUrlResultEnum != HttpRequestResultEnum.Ok)
            {
                MessageBox.Show(this, $"刷新请求百度网盘（{baiduPanUrl}）发生错误，异常信息为：\n{errorString}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            // 从返回的页面内容中提取用于之后获取文件夹树的参数
            // 获取shareid
            {
                Regex regex = new Regex("\"shareid\":(.*?),");
                Match match = regex.Match(returnContent);
                if (match.Success)
                {
                    shareId = match.Groups[1].Value;
                    AppendTextToConsole($"解析出shareid={shareId}");
                }
                else
                {
                    AppendTextToConsole($"无法从百度网盘页面中获取shareid，页面内容为：{returnContent}");
                    MessageBox.Show(this, "无法从百度网盘页面中获取shareid", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            // 获取share_uk
            {
                Regex regex = new Regex("\"share_uk\":\"(.*?)\",");
                Match match = regex.Match(returnContent);
                if (match.Success)
                {
                    shareUk = match.Groups[1].Value;
                    AppendTextToConsole($"解析出share_uk = {shareUk}");
                }
                else
                {
                    AppendTextToConsole($"无法从百度网盘页面中获取share_uk，页面内容为：{returnContent}");
                    MessageBox.Show(this, "无法从百度网盘页面中获取share_uk", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            // 获取分享的这个百度网盘链接中根目录的子文件夹和子文件信息（“locals.mset(”这一行中的json）
            string rootPageReturnJson;
            {
                Regex regex = new Regex("locals\\.mset\\((.*?)\\);");
                Match match = regex.Match(returnContent);
                if (match.Success)
                {
                    rootPageReturnJson = match.Groups[1].Value;
                }
                else
                {
                    AppendTextToConsole($"无法从百度网盘页面中获取locals.mset中的json，页面内容为：{returnContent}");
                    MessageBox.Show(this, "无法从百度网盘页面中获取locals.mset中的json", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            // 目前测试发现请求share/list时，不附带logid参数也能正确得到服务器响应
            //// 请求中的logid就是Cookie中的BAIDUID进行base64加密后的字符串
            //string baiduIdFromCookie = HttpUtil.GetCookieValueByName(cc, "BAIDUID");
            //if (string.IsNullOrEmpty(baiduIdFromCookie) == true)
            //{
            //    errorString = "无法从Cookie中找到BAIDUID";
            //    return;
            //}

            List<DirOrFileInfoVO> rootDirChilds = new List<DirOrFileInfoVO>();
            try
            {
                JObject jObject = JsonConvert.DeserializeObject(rootPageReturnJson) as JObject;
                JArray fileList = jObject["file_list"] as JArray;
                AppendTextToConsole($"链接主页共有{fileList.Count}个文件或文件夹");

                for (int i = 0; i < fileList.Count; i++)
                {
                    JObject oneDirOtFileObject = fileList[i] as JObject;
                    bool isDir = oneDirOtFileObject.Value<bool>("isdir");
                    string name = oneDirOtFileObject.Value<string>("server_filename");
                    long fsId = oneDirOtFileObject.Value<long>("fs_id");
                    int serverModifyTimestamp = oneDirOtFileObject.Value<int>("server_mtime");

                    if (isDir == true)
                    {
                        DirInfoVO childDirInfo = new DirInfoVO();
                        childDirInfo.name = name;
                        childDirInfo.fsId = fsId;
                        childDirInfo.serverModifyTimestamp = serverModifyTimestamp;
                        rootDirChilds.Add(childDirInfo);
                    }
                    else
                    {
                        FileInfoVO childFileInfo = new FileInfoVO();
                        childFileInfo.name = name;
                        childFileInfo.fsId = fsId;
                        childFileInfo.serverModifyTimestamp = serverModifyTimestamp;
                        childFileInfo.baiduMd5 = oneDirOtFileObject.Value<string>("md5");
                        childFileInfo.fileSize = oneDirOtFileObject.Value<long>("size");
                        rootDirChilds.Add(childFileInfo);
                    }
                }

                // 获取链接根目录的地址前缀
                rootDirPath = (fileList[0] as JObject).Value<string>("parent_path").Replace("%2F", "/");
            }
            catch (Exception ex)
            {
                AppendTextToConsole($"解析从百度网盘页面中获取的文件列表json发生错误，json内容为：\n{rootPageReturnJson}\n异常信息为：\n{ex.ToString()}");
                MessageBox.Show(this, "解析从百度网盘页面中获取的文件列表json发生错误", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            // 判断用户想建立快照的目录是否存在
            if (targetDir == "/")
            {
                snapShotInfo.childs = rootDirChilds;
                // 从链接主页获取的子文件夹，还没有遍历其下属的子文件夹和子文件
                foreach (DirOrFileInfoVO child in snapShotInfo.childs)
                {
                    if (child.isDir == true)
                    {
                        DirInfoVO childDirInfo = child as DirInfoVO;
                        List<DirOrFileInfoVO> allChilds = ReqGetDirInfo(string.Concat(rootDirPath, "/", childDirInfo.name), 1, httpClient, shareId, shareUk, out errorString);
                        if (string.IsNullOrEmpty(errorString) == false)
                        {
                            AppendTextToConsole($"执行失败，错误原因：\n{errorString}");
                            MessageBox.Show(this, "执行失败，请在日志窗口中查看具体原因", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        else
                            childDirInfo.childs = allChilds;
                    }
                }
            }
            else
            {
                snapShotInfo.childs = ReqGetDirInfo(string.Concat(rootDirPath, targetDir), 1, httpClient, shareId, shareUk, out errorString);
                if (string.IsNullOrEmpty(errorString) == false)
                {
                    AppendTextToConsole($"执行失败，错误原因：\n{errorString}");
                    MessageBox.Show(this, "执行失败，请在日志窗口中查看具体原因", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            AppendTextToConsole("访问百度网盘链接获取存放的文件夹或文件信息完毕");

            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Title = "请选择快照文件保存路径";
            dialog.Filter = $"百度网盘快照文件 (*.{AppConsts.SNAPSHOOT_FILE_EXTENSION})|*.{AppConsts.SNAPSHOOT_FILE_EXTENSION}";
            if (dialog.ShowDialog() != DialogResult.OK)
            {
                MessageBox.Show(this, "您放弃了生成快照", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string savePath = dialog.FileName;
            try
            {
                File.WriteAllText(savePath, snapShotInfo.ToJson());
                AppendTextToConsole($"成功保存快照文件到：{savePath}");
                MessageBox.Show(this, "成功生成快照文件", "恭喜", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                AppendTextToConsole($"保存快照文件时发生错误，异常信息为：\n{ex.ToString()}");
                MessageBox.Show(this, "保存快照文件失败", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 返回百度网盘中指定文件夹包含的所有子文件夹（包含其下各层级的子文件夹和文件）和文件信息
        /// </summary>
        /// <param name="dirPath">百度网盘分享的链接中，某个文件夹的完整路径，以/或/sharelink开头</param>
        /// <param name="page">第几页的文件夹或文件信息，初始为1，如果1页不能包含全部，则会分为多页</param>
        /// <param name="httpClient"></param>
        /// <param name="shareId"></param>
        /// <param name="shareUk"></param>
        /// <param name="errorString">如果发生错误，返回错误详情</param>
        /// <returns></returns>
        private List<DirOrFileInfoVO> ReqGetDirInfo(string dirPath, int page, HttpClient httpClient, string shareId, string shareUk, out string errorString)
        {
            int timespan = (int)((DateTime.Now - _LastReqShareListTime).TotalMilliseconds);
            if (timespan < _RequestIntervalMillisecond)
                Thread.Sleep(timespan);

            AppendTextToConsole($"请求获取子文件夹（{dirPath}）第{page}页信息");

            List<DirOrFileInfoVO> childs = new List<DirOrFileInfoVO>();

            //string shareListUrl = $"https://pan.baidu.com/share/list?uk={shareUk}&shareid={shareId}&order=other&desc=1&showempty=0&web=1&page={page}&num={AppConsts.SHARE_LIST_ONE_PAGE_MAX_NUM}&dir={HttpUtility.UrlEncode(dirPath)}&channel=chunlei&web=1&app_id=250528&bdstoken=&logid={logId}&clienttype=0";
            // 注意dirPath因作为QueryString，所以要进行URL编码，否则如果文件夹名含有“&”，就会把原来的字段分隔打乱引发错误
            string shareListUrl = $"https://pan.baidu.com/share/list?uk={shareUk}&shareid={shareId}&order=other&desc=1&showempty=0&web=1&page={page}&num={AppConsts.SHARE_LIST_ONE_PAGE_MAX_NUM}&dir={HttpUtility.UrlEncode(dirPath)}&channel=chunlei&web=1&app_id=250528&bdstoken=&clienttype=0";
            HttpRequestMessage shareListReq = new HttpRequestMessage();
            shareListReq.Method = HttpMethod.Get;
            shareListReq.RequestUri = new Uri(shareListUrl);

            Dictionary<string, string> shareListHeaders = new Dictionary<string, string>();
            shareListHeaders.Add("Host", "pan.baidu.com");
            shareListHeaders.Add("User-Agent", AppConsts.USER_AGENT);
            shareListHeaders.Add("Referer", "https://pan.baidu.com/disk/home");

            string returnContent;
            HttpRequestResultEnum reqShareListResultEnum = HttpUtil.DoGet(httpClient, shareListReq, shareListHeaders, out returnContent, out errorString);
            _LastReqShareListTime = DateTime.Now;
            if (reqShareListResultEnum == HttpRequestResultEnum.Timeout)
            {
                errorString = $"请求获取子文件夹（{dirPath}）第{page}页信息超时，请在网速较好时重试或者调大最长等待时间";
                return null;
            }
            else if (reqShareListResultEnum != HttpRequestResultEnum.Ok)
            {
                errorString = $"请求获取子文件夹（{dirPath}）第{page}页信息时发生错误，异常信息为：\n{errorString}";
                return null;
            }
            if (returnContent.Contains("\"errno\":2") == true)
            {
                errorString = $"请求获取子文件夹（{dirPath}）第{page}页信息时发生错误，很可能因为该路径不存在，服务器返回信息为：\n{returnContent}";
                return null;
            }
            if (returnContent.Contains("\"errno\":0") == true)
            {
                //AppendTextToConsole($"返回json为：{returnContent}");
                /**
                 * 解析该路径下的所有子文件夹和文件，对于子文件夹继续向下逐层解析
                 */
                try
                {
                    JObject jObject = JsonConvert.DeserializeObject(returnContent) as JObject;
                    JArray fileList = jObject["list"] as JArray;
                    int count = fileList.Count;
                    for (int i = 0; i < count; i++)
                    {
                        JObject oneDirOtFileObject = fileList[i] as JObject;
                        bool isDir = oneDirOtFileObject.Value<bool>("isdir");
                        string name = oneDirOtFileObject.Value<string>("server_filename");
                        long fsId = oneDirOtFileObject.Value<long>("fs_id");
                        int serverModifyTimestamp = oneDirOtFileObject.Value<int>("server_mtime");

                        if (isDir == true)
                        {
                            DirInfoVO childDirInfo = new DirInfoVO();
                            childDirInfo.name = name;
                            childDirInfo.fsId = fsId;
                            childDirInfo.serverModifyTimestamp = serverModifyTimestamp;
                            // 对于文件夹要继续遍历查找其下的子文件夹和子文件
                            List<DirOrFileInfoVO> dirChilds = ReqGetDirInfo($"{dirPath}/{name}", 1, httpClient, shareId, shareUk, out errorString);
                            if (string.IsNullOrEmpty(errorString) == false)
                                return null;
                            else
                                childDirInfo.childs = dirChilds;

                            childs.Add(childDirInfo);
                        }
                        else
                        {
                            FileInfoVO childFileInfo = new FileInfoVO();
                            childFileInfo.name = name;
                            childFileInfo.fsId = fsId;
                            childFileInfo.serverModifyTimestamp = serverModifyTimestamp;
                            childFileInfo.baiduMd5 = oneDirOtFileObject.Value<string>("md5");
                            childFileInfo.fileSize = oneDirOtFileObject.Value<long>("size");
                            childs.Add(childFileInfo);
                        }
                    }
                    // 如果文件夹或文件数等于100，就有可能还有更多文件，需要请求下一页
                    if (count == AppConsts.SHARE_LIST_ONE_PAGE_MAX_NUM)
                    {
                        List<DirOrFileInfoVO> nextPageFiles = ReqGetDirInfo(dirPath, page + 1, httpClient, shareId, shareUk, out errorString);
                        if (string.IsNullOrEmpty(errorString) == false)
                            return null;
                        else
                            childs.AddRange(nextPageFiles);
                    }
                }
                catch (Exception ex)
                {
                    AppendTextToConsole($"解析请求获取子文件夹（{dirPath}）第{page}页信息返回的json发生错误，json内容为：\n{returnContent}\n异常信息为：\n{ex.ToString()}");
                    errorString = $"解析请求获取子文件夹（{dirPath}）第{page}页信息返回的json发生错误";
                    return null;
                }

                errorString = null;
                return childs;
            }
            else
            {
                errorString = $"请求获取子文件夹（{dirPath}）信息时发生错误，服务器返回信息为：\n{returnContent}";
                return null;
            }
        }

        private void BtnCompareSnapshoot_Click(object sender, EventArgs e)
        {
            string oldSnapshootFilePath;
            string newSnapshootFilePath;
            string oldSnapshootJson;
            string newSnapshootJson;
            string compareOldSnapshootDir;
            string compareNewSnapshootDir;

            SnapshootInfoVO oldSnapshootInfo;
            SnapshootInfoVO newSnapshootInfo;

            DirInfoVO oldTargetDirInfo;
            DirInfoVO newTargetDirInfo;

            string errorString = null;

            oldSnapshootFilePath = TxtOldSnapshootFilePath.Text.Trim();
            if (string.IsNullOrEmpty(oldSnapshootFilePath) == true)
            {
                MessageBox.Show(this, "请输入较老的快照文件路径", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (File.Exists(oldSnapshootFilePath) == false)
            {
                MessageBox.Show(this, "输入的较老的快照文件不存在", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (Path.GetExtension(oldSnapshootFilePath) != $".{AppConsts.SNAPSHOOT_FILE_EXTENSION}")
            {
                MessageBox.Show(this, $"指定的较老的快照文件不合法，快照文件扩展名应为{AppConsts.SNAPSHOOT_FILE_EXTENSION}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                oldSnapshootJson = File.ReadAllText(oldSnapshootFilePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"读取较老的快照文件失败，异常信息为：\n{ex.ToString()}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            oldSnapshootInfo = SnapshootInfoVO.FromJson(oldSnapshootJson, out errorString);
            if (errorString != null)
            {
                MessageBox.Show(this, $"指定的较老的快照文件解析错误，请使用本工具生成的快照文件，异常信息为：\n{errorString}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            newSnapshootFilePath = TxtNewSnapshootFilePath.Text.Trim();
            if (string.IsNullOrEmpty(newSnapshootFilePath) == true)
            {
                MessageBox.Show(this, "请输入较新的快照文件路径", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (File.Exists(newSnapshootFilePath) == false)
            {
                MessageBox.Show(this, "输入的较新的快照文件不存在", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (Path.GetExtension(newSnapshootFilePath) != $".{AppConsts.SNAPSHOOT_FILE_EXTENSION}")
            {
                MessageBox.Show(this, $"指定的较新的快照文件不合法，快照文件扩展名应为{AppConsts.SNAPSHOOT_FILE_EXTENSION}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                newSnapshootJson = File.ReadAllText(newSnapshootFilePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"读取较新的快照文件失败，异常信息为：\n{ex.ToString()}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            newSnapshootInfo = SnapshootInfoVO.FromJson(newSnapshootJson, out errorString);
            if (errorString != null)
            {
                MessageBox.Show(this, $"指定的较新的快照文件解析错误，请使用本工具生成的快照文件，异常信息为：\n{errorString}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            compareOldSnapshootDir = TxtCompareOldSnapshootDir.Text.Trim();
            if (string.IsNullOrEmpty(compareOldSnapshootDir) == true)
            {
                MessageBox.Show(this, "请输入要比较的较老快照中的目录路径，如果从根目录开始比较，请输入“/”", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            compareNewSnapshootDir = TxtCompareNewSnapshootDir.Text.Trim();
            if (string.IsNullOrEmpty(compareNewSnapshootDir) == true)
            {
                MessageBox.Show(this, "请输入要比较的较新快照中的目录路径，如果从根目录开始比较，请输入“/”", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            AppendTextToConsole("=====================================================");
            AppendTextToConsole($"要进行对比的较老的快照文件路径：{oldSnapshootFilePath}");
            AppendTextToConsole($"要对比的路径：{compareOldSnapshootDir}");
            AppendTextToConsole($"要进行对比的较新的快照文件路径：{newSnapshootFilePath}");
            AppendTextToConsole($"要对比的路径：{compareNewSnapshootDir}");

            if (oldSnapshootFilePath.Equals(newSnapshootFilePath, StringComparison.CurrentCultureIgnoreCase) == true
                && compareOldSnapshootDir == compareNewSnapshootDir)
            {
                MessageBox.Show(this, "您指定要对比的是同一个快照文件，且对比的路径也相同，这样没有意义，显然是相同的", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (oldSnapshootInfo.baiduPanUrl.Equals(newSnapshootInfo.baiduPanUrl, StringComparison.CurrentCultureIgnoreCase) == false)
            {
                if (MessageBox.Show(this, $"您要对比的两个快照文件，从属于不同的百度网盘链接\n较老的快照对应的链接为{oldSnapshootInfo.baiduPanUrl}\n较新的快照对应的链接为{newSnapshootInfo.baiduPanUrl}\n\n您确定要对比这两个快照文件吗？", "注意", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.Cancel)
                    return;
            }

            oldTargetDirInfo = new DirInfoVO();
            // 要对比的根目录的名称，不管原本叫什么，都统一去除
            oldTargetDirInfo.name = "";
            oldTargetDirInfo.childs = oldSnapshootInfo.childs;
            if (compareOldSnapshootDir != "/")
            {
                string[] childDirNames = compareOldSnapshootDir.Split("/", StringSplitOptions.RemoveEmptyEntries);
                DirInfoVO currentDir = oldTargetDirInfo;
                for (int i = 0; i < childDirNames.Length; i++)
                {
                    string thisDirName = childDirNames[i];
                    bool isFind = false;
                    foreach (DirOrFileInfoVO oneDirOrFile in currentDir.childs)
                    {
                        if (oneDirOrFile.isDir == true && oneDirOrFile.name == thisDirName)
                        {
                            currentDir = oneDirOrFile as DirInfoVO;
                            isFind = true;
                            break;
                        }
                    }
                    if (isFind == false)
                    {
                        StringBuilder pathBuilder = new StringBuilder();
                        for (int j = 0; j <= i; j++)
                            pathBuilder.Append("/").Append(childDirNames[j]);

                        MessageBox.Show(this, $"在较老的快照文件中，找不到路径“{pathBuilder.ToString()}”", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                oldTargetDirInfo = currentDir;
                // 要对比的根目录的名称，不管原本叫什么，都统一去除
                oldTargetDirInfo.name = "";
            }

            newTargetDirInfo = new DirInfoVO();
            // 要对比的根目录的名称，不管原本叫什么，都统一去除
            newTargetDirInfo.name = "";
            newTargetDirInfo.childs = newSnapshootInfo.childs;
            if (compareNewSnapshootDir != "/")
            {
                string[] childDirNames = compareNewSnapshootDir.Split("/", StringSplitOptions.RemoveEmptyEntries);
                DirInfoVO currentDir = newTargetDirInfo;
                for (int i = 0; i < childDirNames.Length; i++)
                {
                    string thisDirName = childDirNames[i];
                    bool isFind = false;
                    foreach (DirOrFileInfoVO oneDirOrFile in currentDir.childs)
                    {
                        if (oneDirOrFile.isDir == true && oneDirOrFile.name == thisDirName)
                        {
                            currentDir = oneDirOrFile as DirInfoVO;
                            isFind = true;
                            break;
                        }
                    }
                    if (isFind == false)
                    {
                        StringBuilder pathBuilder = new StringBuilder();
                        for (int j = 0; j <= i; j++)
                            pathBuilder.Append("/").Append(childDirNames[j]);

                        MessageBox.Show(this, $"在较新的快照文件中，找不到路径“{pathBuilder.ToString()}”", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                newTargetDirInfo = currentDir;
                // 要对比的根目录的名称，不管原本叫什么，都统一去除
                newTargetDirInfo.name = "";
            }

            DirDiffVO diff = CompareTwoDirInfo(oldTargetDirInfo, newTargetDirInfo, "/");
            diff.CleanNotDiffChildsDir();

            // 在对比结果窗口中展示
            SnapshootDiffResultForm diffResultForm = new SnapshootDiffResultForm(oldSnapshootFilePath, compareOldSnapshootDir,
                newSnapshootFilePath, compareNewSnapshootDir, diff);
            diffResultForm.ShowDialog();
        }

        private DirDiffVO CompareTwoDirInfo(DirInfoVO oldDir, DirInfoVO newDir, string currentDirPath)
        {
            DirDiffVO dirDiff = new DirDiffVO();
            dirDiff.name = oldDir.name;
            dirDiff.path = currentDirPath;

            if (oldDir == null)
            {
                dirDiff.diffState = DiffStateEnum.Add;
                dirDiff.addOrDeleteDirInfo = newDir;
            }
            else if (newDir == null)
            {
                dirDiff.diffState = DiffStateEnum.Delete;
                dirDiff.addOrDeleteDirInfo = oldDir;
            }
            else
            {
                dirDiff.diffState = DiffStateEnum.None;
                dirDiff.childsDiff = new List<DirOrFileDiffVO>();

                /**
                 * 对比两个文件夹下属子文件夹、子文件的差异
                 */
                // 首先将子文件夹、子文件按名称整理为字典结构
                dirDiff.filesInOldSnapshoot = new Dictionary<string, FileInfoVO>();
                dirDiff.filesInNewSnapshoot = new Dictionary<string, FileInfoVO>();
                dirDiff.dirsInOldSnapshoot = new Dictionary<string, DirInfoVO>();
                dirDiff.dirsInNewSnapshoot = new Dictionary<string, DirInfoVO>();

                foreach (DirOrFileInfoVO oldDirOrFile in oldDir.childs)
                {
                    if (oldDirOrFile.isDir == true)
                        dirDiff.dirsInOldSnapshoot.Add(oldDirOrFile.name, oldDirOrFile as DirInfoVO);
                    else
                        dirDiff.filesInOldSnapshoot.Add(oldDirOrFile.name, oldDirOrFile as FileInfoVO);
                }
                foreach (DirOrFileInfoVO newDirOrFile in newDir.childs)
                {
                    if (newDirOrFile.isDir == true)
                        dirDiff.dirsInNewSnapshoot.Add(newDirOrFile.name, newDirOrFile as DirInfoVO);
                    else
                        dirDiff.filesInNewSnapshoot.Add(newDirOrFile.name, newDirOrFile as FileInfoVO);
                }

                // 接下来对比并存储差异信息，注意下面按新增文件-删除文件-修改文件-新增文件夹-删除文件夹-相同文件夹的顺序进行
                // 此顺序也决定了之后输出差异目录树时的顺序

                // 对比找到较新快照中新增的文件
                foreach (string fileName in dirDiff.filesInNewSnapshoot.Keys)
                {
                    if (dirDiff.filesInOldSnapshoot.ContainsKey(fileName) == false)
                    {

                        FileDiffVO childFileDiff = new FileDiffVO();
                        childFileDiff.name = fileName;
                        childFileDiff.path = CombineChildPath(currentDirPath, fileName);
                        childFileDiff.diffState = DiffStateEnum.Add;
                        childFileDiff.newFileInfo = dirDiff.filesInNewSnapshoot[fileName];
                        dirDiff.childsDiff.Add(childFileDiff);
                    }
                }
                // 对比找到较新快照中删除的文件
                foreach (string fileName in dirDiff.filesInOldSnapshoot.Keys)
                {
                    if (dirDiff.filesInNewSnapshoot.ContainsKey(fileName) == false)
                    {

                        FileDiffVO childFileDiff = new FileDiffVO();
                        childFileDiff.name = fileName;
                        childFileDiff.path = CombineChildPath(currentDirPath, fileName);
                        childFileDiff.diffState = DiffStateEnum.Delete;
                        childFileDiff.oldFileInfo = dirDiff.filesInOldSnapshoot[fileName];
                        dirDiff.childsDiff.Add(childFileDiff);
                    }
                }
                // 对比找到较老、较新快照都存在但发生修改的文件（文件MD5值变化）
                foreach (string fileName in dirDiff.filesInNewSnapshoot.Keys)
                {
                    if (dirDiff.filesInOldSnapshoot.ContainsKey(fileName) == true)
                    {
                        FileInfoVO oldFile = dirDiff.filesInOldSnapshoot[fileName];
                        FileInfoVO newFile = dirDiff.filesInNewSnapshoot[fileName];
                        if (oldFile.baiduMd5 != newFile.baiduMd5)
                        {
                            FileDiffVO childFileDiff = new FileDiffVO();
                            childFileDiff.name = fileName;
                            childFileDiff.path = CombineChildPath(currentDirPath, fileName);
                            childFileDiff.diffState = DiffStateEnum.Modity;
                            childFileDiff.oldFileInfo = oldFile;
                            childFileDiff.newFileInfo = newFile;
                            dirDiff.childsDiff.Add(childFileDiff);
                        }
                    }
                }
                // 对比找到较新快照中新增的文件夹
                foreach (string dirName in dirDiff.dirsInNewSnapshoot.Keys)
                {
                    if (dirDiff.dirsInOldSnapshoot.ContainsKey(dirName) == false)
                    {
                        DirDiffVO childDirDiff = new DirDiffVO();
                        childDirDiff.name = dirName;
                        childDirDiff.path = CombineChildPath(currentDirPath, dirName);
                        childDirDiff.diffState = DiffStateEnum.Add;
                        childDirDiff.addOrDeleteDirInfo = dirDiff.dirsInNewSnapshoot[dirName];
                        dirDiff.childsDiff.Add(childDirDiff);
                    }
                }
                // 对比找到较新快照中删除的文件夹
                foreach (string dirName in dirDiff.dirsInOldSnapshoot.Keys)
                {
                    if (dirDiff.dirsInNewSnapshoot.ContainsKey(dirName) == false)
                    {
                        DirDiffVO childDirDiff = new DirDiffVO();
                        childDirDiff.name = dirName;
                        childDirDiff.path = CombineChildPath(currentDirPath, dirName);
                        childDirDiff.diffState = DiffStateEnum.Delete;
                        childDirDiff.addOrDeleteDirInfo = dirDiff.dirsInOldSnapshoot[dirName];
                        dirDiff.childsDiff.Add(childDirDiff);
                    }
                }
                // 对较老、较新快照都存在的文件夹，继续向下遍历比较其下属子文件夹、子文件
                foreach (string dirName in dirDiff.dirsInNewSnapshoot.Keys)
                {
                    if (dirDiff.dirsInOldSnapshoot.ContainsKey(dirName) == true)
                    {
                        dirDiff.childsDiff.Add(CompareTwoDirInfo(dirDiff.dirsInOldSnapshoot[dirName],
                            dirDiff.dirsInNewSnapshoot[dirName], CombineChildPath(currentDirPath, dirName)));
                    }
                }
            }

            return dirDiff;
        }

        private string CombineChildPath(string parentPath, string childName)
        {
            if (parentPath == "/")
                return string.Concat(parentPath, childName);
            else
                return string.Concat(parentPath, "/", childName);
        }

        private void BtnChooseOldSnapshootFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "请选择较老的快照文件所在路径";
            dialog.Multiselect = false;
            dialog.Filter = $"百度网盘快照文件 (*.{AppConsts.SNAPSHOOT_FILE_EXTENSION})|*.{AppConsts.SNAPSHOOT_FILE_EXTENSION}";
            if (dialog.ShowDialog() == DialogResult.OK)
                TxtOldSnapshootFilePath.Text = dialog.FileName;
        }

        private void BtnChooseNewSnapshootFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "请选择较老的快照文件所在路径";
            dialog.Multiselect = false;
            dialog.Filter = $"百度网盘快照文件 (*.{AppConsts.SNAPSHOOT_FILE_EXTENSION})|*.{AppConsts.SNAPSHOOT_FILE_EXTENSION}";
            if (dialog.ShowDialog() == DialogResult.OK)
                TxtNewSnapshootFilePath.Text = dialog.FileName;
        }

        private void AppendTextToConsole(object text, bool isAppendNewLine = true)
        {
            // 让富文本框获取焦点
            RtxLog.Focus();
            // 设置光标的位置到文本结尾
            RtxLog.Select(RtxLog.TextLength, 0);
            // 滚动到富文本框光标处
            RtxLog.ScrollToCaret();
            // 追加内容
            RtxLog.AppendText(text as string);
            // 换行
            if (isAppendNewLine)
                RtxLog.AppendText(Environment.NewLine);
        }

        private void BtnChooseSnapshootFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "请选择百度网盘快照文件所在路径";
            dialog.Multiselect = false;
            dialog.Filter = $"百度网盘快照文件 (*.{AppConsts.SNAPSHOOT_FILE_EXTENSION})|*.{AppConsts.SNAPSHOOT_FILE_EXTENSION}";
            if (dialog.ShowDialog() == DialogResult.OK)
                TxtSnapshootFilePath.Text = dialog.FileName;
        }

        private void BtnChooseLocalDir_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "请选择要对比的本地文件夹";
            dialog.ShowNewFolderButton = false;
            if (dialog.ShowDialog() == DialogResult.OK)
                TxtLocalDirPath.Text = dialog.SelectedPath;
        }

        private void BtnCompareSnapshootAndLocalDir_Click(object sender, EventArgs e)
        {
            string snapshootFilePath;
            string snapshootJson;
            string compareSnapshootDir;
            SnapshootInfoVO snapshootInfo;
            DirInfoVO targeSnapshootDirInfo;

            string localDirPath;

            string errorString = null;

            IgnoreFileMD5Config ignoreFileMD5Config = new IgnoreFileMD5Config();
            if (ChkIgnoreBigFile.Checked == true)
            {
                string ignoreBigFileSizeString = TxtIgnoreBigFileSize.Text.Trim();
                if (string.IsNullOrEmpty(ignoreBigFileSizeString) == true)
                {
                    MessageBox.Show(this, "勾选了“忽略达到以下大小的文件”，就必须设定文件大小值", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                double num;
                if (double.TryParse(ignoreBigFileSizeString, out num) == false)
                {
                    MessageBox.Show(this, "输入的文件大小值不是合法数字", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (num <= 0)
                {
                    if (DialogResult.Cancel == MessageBox.Show(this, "输入的文件大小值不为正数，这样将忽略对所有文件的MD5计算，从而不对比文件差异，确定要这样做吗？\n\n点击“确认”继续执行，点击“取消”放弃对比", "注意", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning))
                        return;
                    else
                        ignoreFileMD5Config.IgnoreFileSizeByte = -1;
                }
                else
                {
                    if (Cbo.SelectedText == "B")
                        ignoreFileMD5Config.IgnoreFileSizeByte = (long)num;
                    else if (Cbo.SelectedText == "KB")
                        ignoreFileMD5Config.IgnoreFileSizeByte = (long)(1024d * num);
                    else if (Cbo.SelectedText == "MB")
                        ignoreFileMD5Config.IgnoreFileSizeByte = (long)(1024d * 1024d * num);
                    else if (Cbo.SelectedText == "GB")
                        ignoreFileMD5Config.IgnoreFileSizeByte = (long)(1024d * 1024d * 1024d * num);
                }
            }
            else
                ignoreFileMD5Config.IgnoreFileSizeByte = 0;

            if (ChkIgnoreFileExtension.Checked == true)
            {
                string ignoreFileExtensionString = TxtIgnoreFileExtension.Text.Trim();
                if (string.IsNullOrEmpty(ignoreFileExtensionString) == true)
                {
                    MessageBox.Show(this, "勾选了“忽略以下扩展名的文件”，就必须输入要忽略哪些扩展名", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                string[] extensions = ignoreFileExtensionString.Split('|', StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < extensions.Length; i++)
                {
                    extensions[i] = extensions[i].Trim();
                    if (extensions[i].StartsWith('.') == false)
                        extensions[i] = "." + extensions[i];
                }
                ignoreFileMD5Config.IgnoreFileExtensions = extensions;
            }

            snapshootFilePath = TxtSnapshootFilePath.Text.Trim();
            if (string.IsNullOrEmpty(snapshootFilePath) == true)
            {
                MessageBox.Show(this, "请输入百度网盘快照文件路径", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (File.Exists(snapshootFilePath) == false)
            {
                MessageBox.Show(this, "输入的百度网盘快照文件不存在", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (Path.GetExtension(snapshootFilePath) != $".{AppConsts.SNAPSHOOT_FILE_EXTENSION}")
            {
                MessageBox.Show(this, $"指定的百度网盘快照文件不合法，快照文件扩展名应为{AppConsts.SNAPSHOOT_FILE_EXTENSION}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                snapshootJson = File.ReadAllText(snapshootFilePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"读取百度网盘快照文件失败，异常信息为：\n{ex.ToString()}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            snapshootInfo = SnapshootInfoVO.FromJson(snapshootJson, out errorString);
            if (errorString != null)
            {
                MessageBox.Show(this, $"指定的百度网盘快照文件解析错误，请使用本工具生成的快照文件，异常信息为：\n{errorString}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            compareSnapshootDir = TxtCompareSnapshootDir.Text.Trim();
            if (string.IsNullOrEmpty(compareSnapshootDir) == true)
            {
                MessageBox.Show(this, "请输入要比较的百度网盘快照中的目录路径，如果从根目录开始比较，请输入“/”", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            localDirPath = TxtLocalDirPath.Text.Trim();
            if (string.IsNullOrEmpty(localDirPath) == true)
            {
                MessageBox.Show(this, "请输入要比对的本地文件夹路径", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (Directory.Exists(localDirPath) == false)
            {
                MessageBox.Show(this, "输入的要比对的本地文件夹不存在", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            AppendTextToConsole("=====================================================");
            AppendTextToConsole($"要进行对比的百度网盘快照文件路径：{snapshootFilePath}");
            AppendTextToConsole($"要对比的路径：{compareSnapshootDir}");
            AppendTextToConsole($"要进行对比的本地文件夹路径：{localDirPath}");

            targeSnapshootDirInfo = new DirInfoVO();
            // 要对比的根目录的名称，不管原本叫什么，都统一去除
            targeSnapshootDirInfo.name = "";
            targeSnapshootDirInfo.childs = snapshootInfo.childs;
            if (compareSnapshootDir != "/")
            {
                string[] childDirNames = compareSnapshootDir.Split("/", StringSplitOptions.RemoveEmptyEntries);
                DirInfoVO currentDir = targeSnapshootDirInfo;
                for (int i = 0; i < childDirNames.Length; i++)
                {
                    string thisDirName = childDirNames[i];
                    bool isFind = false;
                    foreach (DirOrFileInfoVO oneDirOrFile in currentDir.childs)
                    {
                        if (oneDirOrFile.isDir == true && oneDirOrFile.name == thisDirName)
                        {
                            currentDir = oneDirOrFile as DirInfoVO;
                            isFind = true;
                            break;
                        }
                    }
                    if (isFind == false)
                    {
                        StringBuilder pathBuilder = new StringBuilder();
                        for (int j = 0; j <= i; j++)
                            pathBuilder.Append("/").Append(childDirNames[j]);

                        MessageBox.Show(this, $"在百度网盘快照文件中，找不到路径“{pathBuilder.ToString()}”", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                targeSnapshootDirInfo = currentDir;
                // 要对比的根目录的名称，不管原本叫什么，都统一去除
                targeSnapshootDirInfo.name = "";
            }

            if (ignoreFileMD5Config.IgnoreFileSizeByte >= 0)
            {
                if (DialogResult.Cancel == MessageBox.Show(this, "因计算文件MD5耗时较长，程序运行过程中可能表现出卡顿或假死，这是正常现象，请不要强制中止\n\n点击“确定”开始对比本地目录文件并计算MD5，点击“取消”放弃本次对比", "注意", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning))
                    return;
            }

            // 为本地目录建立快照
            DirInfoVO localDirInfo = new DirInfoVO();
            localDirInfo.name = "";
            List<string> ignoreCalculateMD5FilePathList;
            localDirInfo.childs = GenerateLocalDirChildsInfo(new DirectoryInfo(localDirPath), ignoreFileMD5Config, out errorString, out ignoreCalculateMD5FilePathList);
            if (errorString != null)
            {
                MessageBox.Show(this, $"获取本地目录信息失败，程序被迫中止，具体原因为：\n{errorString}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else if (ignoreCalculateMD5FilePathList.Count > 0)
            {
                AppendTextToConsole("\n根据设置的“计算本地文件MD5选项”，以下文件不计算MD5，也就不进行对比：");
                AppendTextToConsole(StringUtil.CombineString(ignoreCalculateMD5FilePathList, Environment.NewLine));
            }

            DirDiffVO diff = CompareLocalDirInfoAndSnapshoot(localDirInfo, targeSnapshootDirInfo, "/");
            diff.CleanNotDiffChildsDir();

            // 在对比结果窗口中展示
            SnapshootAndLocalDirDiffResultForm diffResultForm = new SnapshootAndLocalDirDiffResultForm(snapshootFilePath, compareSnapshootDir,
                localDirPath, diff);
            diffResultForm.ShowDialog();
        }

        private List<DirOrFileInfoVO> GenerateLocalDirChildsInfo(DirectoryInfo directoryInfo, IgnoreFileMD5Config ignoreFileMD5Config, out string errorString, out List<string> ignoreCalculateMD5FilePathList)
        {
            List<DirOrFileInfoVO> childsList = new List<DirOrFileInfoVO>();
            ignoreCalculateMD5FilePathList = new List<string>();

            // 子文件夹
            foreach (DirectoryInfo childDir in directoryInfo.GetDirectories())
            {
                DirInfoVO dirInfo = new DirInfoVO();
                dirInfo.name = childDir.Name;
                dirInfo.serverModifyTimestamp = DateTimeUtil.DateTimeToTimestampSecond(childDir.LastWriteTime);
                // 向下递归遍历子文件夹和子文件
                List<DirOrFileInfoVO> list = GenerateLocalDirChildsInfo(childDir, ignoreFileMD5Config, out errorString, out ignoreCalculateMD5FilePathList);
                if (errorString != null)
                    return null;

                dirInfo.childs.AddRange(list);
                childsList.Add(dirInfo);
            }
            // 子文件
            foreach (FileInfo childFile in directoryInfo.GetFiles())
            {
                FileInfoVO fileInfo = new FileInfoVO();
                fileInfo.name = childFile.Name;
                fileInfo.serverModifyTimestamp = DateTimeUtil.DateTimeToTimestampSecond(childFile.LastWriteTime);
                fileInfo.fileSize = childFile.Length;
                // 文件大小满足忽略计算MD5的选项
                if (ignoreFileMD5Config.IgnoreFileSizeByte != 0 && fileInfo.fileSize >= ignoreFileMD5Config.IgnoreFileSizeByte)
                {
                    childsList.Add(fileInfo);
                    ignoreCalculateMD5FilePathList.Add(childFile.FullName);
                }
                // 文件扩展名满足忽略计算MD5的选项
                else if (ignoreFileMD5Config.IgnoreFileExtensions != null && ignoreFileMD5Config.IgnoreFileExtensions.Contains(childFile.Extension, StringComparer.CurrentCultureIgnoreCase))
                {
                    childsList.Add(fileInfo);
                    ignoreCalculateMD5FilePathList.Add(childFile.FullName);
                }
                else
                {
                    AppendTextToConsole($"开始计算文件MD5：{childFile.FullName}");
                    string fileMD5 = IoUtil.CalculateFileMd5(childFile.FullName, out errorString);
                    if (errorString != null)
                        return null;

                    fileInfo.localMd5 = fileMD5;
                    fileInfo.baiduMd5 = AppConsts.Md5ToBaiduMd5(fileMD5);
                    childsList.Add(fileInfo);
                }
            }

            errorString = null;
            return childsList;
        }

        private DirDiffVO CompareLocalDirInfoAndSnapshoot(DirInfoVO localDir, DirInfoVO snapshootDir, string currentDirPath)
        {
            DirDiffVO dirDiff = new DirDiffVO();
            dirDiff.name = localDir.name;
            dirDiff.path = currentDirPath;

            if (localDir == null)
            {
                dirDiff.diffState = DiffStateEnum.Add;
                dirDiff.addOrDeleteDirInfo = snapshootDir;
            }
            else if (snapshootDir == null)
            {
                dirDiff.diffState = DiffStateEnum.Delete;
                dirDiff.addOrDeleteDirInfo = localDir;
            }
            else
            {
                dirDiff.diffState = DiffStateEnum.None;
                dirDiff.childsDiff = new List<DirOrFileDiffVO>();

                /**
                 * 对比两个文件夹下属子文件夹、子文件的差异
                 */
                // 首先将子文件夹、子文件按名称整理为字典结构
                dirDiff.filesInOldSnapshoot = new Dictionary<string, FileInfoVO>();
                dirDiff.filesInNewSnapshoot = new Dictionary<string, FileInfoVO>();
                dirDiff.dirsInOldSnapshoot = new Dictionary<string, DirInfoVO>();
                dirDiff.dirsInNewSnapshoot = new Dictionary<string, DirInfoVO>();

                foreach (DirOrFileInfoVO oldDirOrFile in localDir.childs)
                {
                    if (oldDirOrFile.isDir == true)
                        dirDiff.dirsInOldSnapshoot.Add(oldDirOrFile.name, oldDirOrFile as DirInfoVO);
                    else
                        dirDiff.filesInOldSnapshoot.Add(oldDirOrFile.name, oldDirOrFile as FileInfoVO);
                }
                foreach (DirOrFileInfoVO newDirOrFile in snapshootDir.childs)
                {
                    if (newDirOrFile.isDir == true)
                        dirDiff.dirsInNewSnapshoot.Add(newDirOrFile.name, newDirOrFile as DirInfoVO);
                    else
                        dirDiff.filesInNewSnapshoot.Add(newDirOrFile.name, newDirOrFile as FileInfoVO);
                }

                // 接下来对比并存储差异信息，注意下面按新增文件-删除文件-修改文件-新增文件夹-删除文件夹-相同文件夹的顺序进行
                // 此顺序也决定了之后输出差异目录树时的顺序

                // 对比找到快照中新增的文件
                foreach (string fileName in dirDiff.filesInNewSnapshoot.Keys)
                {
                    if (dirDiff.filesInOldSnapshoot.ContainsKey(fileName) == false)
                    {

                        FileDiffVO childFileDiff = new FileDiffVO();
                        childFileDiff.name = fileName;
                        childFileDiff.path = CombineChildPath(currentDirPath, fileName);
                        childFileDiff.diffState = DiffStateEnum.Add;
                        childFileDiff.newFileInfo = dirDiff.filesInNewSnapshoot[fileName];
                        dirDiff.childsDiff.Add(childFileDiff);
                    }
                }
                // 对比找到快照中删除的文件
                foreach (string fileName in dirDiff.filesInOldSnapshoot.Keys)
                {
                    if (dirDiff.filesInNewSnapshoot.ContainsKey(fileName) == false)
                    {

                        FileDiffVO childFileDiff = new FileDiffVO();
                        childFileDiff.name = fileName;
                        childFileDiff.path = CombineChildPath(currentDirPath, fileName);
                        childFileDiff.diffState = DiffStateEnum.Delete;
                        childFileDiff.oldFileInfo = dirDiff.filesInOldSnapshoot[fileName];
                        dirDiff.childsDiff.Add(childFileDiff);
                    }
                }
                // 对比找到快照和本地目录中都存在但发生修改的文件（文件MD5值变化）
                // 注意如果本地文件没有MD5，说明忽略对该文件计算MD5，也就放弃对其进行差异比较
                foreach (string fileName in dirDiff.filesInNewSnapshoot.Keys)
                {
                    if (dirDiff.filesInOldSnapshoot.ContainsKey(fileName) == true)
                    {
                        FileInfoVO oldFile = dirDiff.filesInOldSnapshoot[fileName];
                        FileInfoVO newFile = dirDiff.filesInNewSnapshoot[fileName];
                        if (oldFile.baiduMd5 != null && oldFile.baiduMd5 != newFile.baiduMd5)
                        {
                            FileDiffVO childFileDiff = new FileDiffVO();
                            childFileDiff.name = fileName;
                            childFileDiff.path = CombineChildPath(currentDirPath, fileName);
                            childFileDiff.diffState = DiffStateEnum.Modity;
                            childFileDiff.oldFileInfo = oldFile;
                            childFileDiff.newFileInfo = newFile;
                            dirDiff.childsDiff.Add(childFileDiff);
                        }
                    }
                }
                // 对比找到快照中新增的文件夹
                foreach (string dirName in dirDiff.dirsInNewSnapshoot.Keys)
                {
                    if (dirDiff.dirsInOldSnapshoot.ContainsKey(dirName) == false)
                    {
                        DirDiffVO childDirDiff = new DirDiffVO();
                        childDirDiff.name = dirName;
                        childDirDiff.path = CombineChildPath(currentDirPath, dirName);
                        childDirDiff.diffState = DiffStateEnum.Add;
                        childDirDiff.addOrDeleteDirInfo = dirDiff.dirsInNewSnapshoot[dirName];
                        dirDiff.childsDiff.Add(childDirDiff);
                    }
                }
                // 对比找到快照中删除的文件夹
                foreach (string dirName in dirDiff.dirsInOldSnapshoot.Keys)
                {
                    if (dirDiff.dirsInNewSnapshoot.ContainsKey(dirName) == false)
                    {
                        DirDiffVO childDirDiff = new DirDiffVO();
                        childDirDiff.name = dirName;
                        childDirDiff.path = CombineChildPath(currentDirPath, dirName);
                        childDirDiff.diffState = DiffStateEnum.Delete;
                        childDirDiff.addOrDeleteDirInfo = dirDiff.dirsInOldSnapshoot[dirName];
                        dirDiff.childsDiff.Add(childDirDiff);
                    }
                }
                // 对快照和本地目录都存在的文件夹，继续向下遍历比较其下属子文件夹、子文件
                foreach (string dirName in dirDiff.dirsInNewSnapshoot.Keys)
                {
                    if (dirDiff.dirsInOldSnapshoot.ContainsKey(dirName) == true)
                    {
                        dirDiff.childsDiff.Add(CompareTwoDirInfo(dirDiff.dirsInOldSnapshoot[dirName],
                            dirDiff.dirsInNewSnapshoot[dirName], CombineChildPath(currentDirPath, dirName)));
                    }
                }
            }

            return dirDiff;
        }

        private void BtnViewOldSnapshoot_Click(object sender, EventArgs e)
        {
            string oldSnapshootFilePath;
            string oldSnapshootJson;
            SnapshootInfoVO oldSnapshootInfo;

            string errorString = null;

            oldSnapshootFilePath = TxtOldSnapshootFilePath.Text.Trim();
            if (string.IsNullOrEmpty(oldSnapshootFilePath) == true)
            {
                MessageBox.Show(this, "请输入较老的快照文件路径", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (File.Exists(oldSnapshootFilePath) == false)
            {
                MessageBox.Show(this, "输入的较老的快照文件不存在", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (Path.GetExtension(oldSnapshootFilePath) != $".{AppConsts.SNAPSHOOT_FILE_EXTENSION}")
            {
                MessageBox.Show(this, $"指定的较老的快照文件不合法，快照文件扩展名应为{AppConsts.SNAPSHOOT_FILE_EXTENSION}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                oldSnapshootJson = File.ReadAllText(oldSnapshootFilePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"读取较老的快照文件失败，异常信息为：\n{ex.ToString()}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            oldSnapshootInfo = SnapshootInfoVO.FromJson(oldSnapshootJson, out errorString);
            if (errorString != null)
            {
                MessageBox.Show(this, $"指定的较老的快照文件解析错误，请使用本工具生成的快照文件，异常信息为：\n{errorString}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 在快照浏览窗口中展示
            SnapshootViewerForm snapshootViewer = new SnapshootViewerForm(oldSnapshootFilePath, oldSnapshootInfo);
            snapshootViewer.ShowDialog();
        }

        private void BtnViewNewSnapshoot_Click(object sender, EventArgs e)
        {
            string newSnapshootFilePath;
            string newSnapshootJson;
            SnapshootInfoVO newSnapshootInfo;

            string errorString = null;

            newSnapshootFilePath = TxtNewSnapshootFilePath.Text.Trim();
            if (string.IsNullOrEmpty(newSnapshootFilePath) == true)
            {
                MessageBox.Show(this, "请输入较新的快照文件路径", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (File.Exists(newSnapshootFilePath) == false)
            {
                MessageBox.Show(this, "输入的较新的快照文件不存在", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (Path.GetExtension(newSnapshootFilePath) != $".{AppConsts.SNAPSHOOT_FILE_EXTENSION}")
            {
                MessageBox.Show(this, $"指定的较新的快照文件不合法，快照文件扩展名应为{AppConsts.SNAPSHOOT_FILE_EXTENSION}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                newSnapshootJson = File.ReadAllText(newSnapshootFilePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"读取较新的快照文件失败，异常信息为：\n{ex.ToString()}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            newSnapshootInfo = SnapshootInfoVO.FromJson(newSnapshootJson, out errorString);
            if (errorString != null)
            {
                MessageBox.Show(this, $"指定的较新的快照文件解析错误，请使用本工具生成的快照文件，异常信息为：\n{errorString}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 在快照浏览窗口中展示
            SnapshootViewerForm snapshootViewer = new SnapshootViewerForm(newSnapshootFilePath, newSnapshootInfo);
            snapshootViewer.ShowDialog();
        }

        private void BtnViewBaiduPanUrlAndCopyAccessCode_Click(object sender, EventArgs e)
        {
            string baiduPanUrl = null;
            string baiduPanAccessCode = null;
            string errorString = null;
            /**
             * 检查用户输入项目是否正确
             */
            // 检查输入的网盘地址
            baiduPanUrl = TxtBaiduPanUrl.Text.Trim();
            if (string.IsNullOrEmpty(baiduPanUrl))
            {
                MessageBox.Show(this, "请输入百度网盘链接地址", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (baiduPanUrl.StartsWith(AppConsts.BAIDU_PAN_URL_PREFIX, StringComparison.CurrentCultureIgnoreCase) == false)
            {
                MessageBox.Show(this, $"输入的百度网盘链接地址非法，请输入以{AppConsts.BAIDU_PAN_URL_PREFIX}开头的地址", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            // 如果是带有提取码的链接，从中获取提取码
            int baiduPanUrlAccessCodeParamNameIndex = baiduPanUrl.IndexOf(AppConsts.BAIDU_PAN_URL_ACCESS_CODE_PARAM_NAME, StringComparison.CurrentCultureIgnoreCase);
            if (baiduPanUrlAccessCodeParamNameIndex != -1)
            {
                baiduPanAccessCode = baiduPanUrl.Substring(baiduPanUrlAccessCodeParamNameIndex + AppConsts.BAIDU_PAN_URL_ACCESS_CODE_PARAM_NAME.Length);
                baiduPanUrl = baiduPanUrl.Substring(0, baiduPanUrlAccessCodeParamNameIndex);
                if (AppConsts.CheckBaiduPanAccessCodeFormat(baiduPanAccessCode, out errorString) == false)
                {
                    MessageBox.Show(this, $"从百度网盘链接地址中获取的提取码{baiduPanAccessCode}错误，{errorString}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            // 检查输入的提取码
            string inputBaiduPanAccessCode = TxtBaiduPanAccessCode.Text.Trim();
            if (baiduPanUrlAccessCodeParamNameIndex != -1)
            {
                // 如果输入的百度网盘链接带提取码，而用户又在提取码输入框输入了，要保证一致
                if (string.IsNullOrEmpty(inputBaiduPanAccessCode) == false)
                {
                    if (inputBaiduPanAccessCode.Equals(baiduPanAccessCode, StringComparison.CurrentCultureIgnoreCase) == false)
                    {
                        MessageBox.Show(this, $"从百度网盘链接地址中获取的提取码{baiduPanAccessCode}与在提取码输入框中输入的{inputBaiduPanAccessCode}不一致", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
            }
            else
            {
                // 如果输入的百度网盘链接不带提取码，则需要输入提取码
                if (string.IsNullOrEmpty(inputBaiduPanAccessCode) == true)
                {
                    MessageBox.Show(this, "请输入对应的提取码", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (AppConsts.CheckBaiduPanAccessCodeFormat(inputBaiduPanAccessCode, out errorString) == false)
                {
                    MessageBox.Show(this, $"输入的提取码{baiduPanAccessCode}错误，{errorString}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                baiduPanAccessCode = inputBaiduPanAccessCode;
            }

            Clipboard.SetText(baiduPanAccessCode);
            Process.Start("explorer.exe", baiduPanUrl);
        }
    }
}