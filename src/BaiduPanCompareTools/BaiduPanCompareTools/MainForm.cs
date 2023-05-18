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
        // ��һ�η���share/list�����ʱ��
        DateTime _LastReqShareListTime = DateTime.Now;
        // �û����õ�����ٶ����̵�ʱ���������룩
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

            // ���Լ���MD5���ļ���С��λ��Ĭ��ѡΪMB
            Cbo.SelectedIndex = 2;
        }

        private void BaiduPanUrlAndAccessCodeTextBoxDragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop) == true)
            {
                Array dragDropFileArray = e.Data.GetData(DataFormats.FileDrop) as Array;
                if (dragDropFileArray.Length != 1)
                {
                    MessageBox.Show("��Ҫͨ����ק��һ�������ļ�����ȡ���Զ��������д洢�İٶ��������Ӻ���ȡ�룬��ֻ����һ�������ļ�", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                string path = dragDropFileArray.GetValue(0).ToString();
                if (Directory.Exists(path) == true)
                {
                    MessageBox.Show("��Ҫͨ����ק��һ�������ļ�����ȡ���Զ��������д洢�İٶ��������Ӻ���ȡ�룬������һ�������ļ��������ļ���", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                            MessageBox.Show(this, $"����ͨ����ק��һ�������ļ�����ȡ���Զ��������д洢�İٶ��������Ӻ���ȡ�룬����ȡ�����ļ�ʧ�ܣ��쳣��ϢΪ��\n{ex.ToString()}", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        snapshootInfo = SnapshootInfoVO.FromJson(snapshootJson, out errorString);
                        if (errorString != null)
                        {
                            MessageBox.Show(this, $"����ͨ����ק��һ�������ļ�����ȡ���Զ��������д洢�İٶ��������Ӻ���ȡ�룬�������ļ�����������ʹ�ñ��������ɵĿ����ļ����쳣��ϢΪ��\n{errorString}", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        TxtBaiduPanUrl.Text = snapshootInfo.baiduPanUrl;
                        TxtBaiduPanAccessCode.Text = snapshootInfo.baiduPanAccessCode;
                    }
                    else
                    {
                        MessageBox.Show("��Ҫͨ����ק��һ�������ļ�����ȡ���Զ��������д洢�İٶ��������Ӻ���ȡ�룬������һ�������ļ�", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    if (Path.GetExtension(path) != $".{AppConsts.SNAPSHOOT_FILE_EXTENSION}")
                    {
                        MessageBox.Show($"��Ҫͨ����ק��һ�������ļ�����ȡ���Զ��������д洢�İٶ��������Ӻ���ȡ�룬������һ����չ��Ϊ{AppConsts.SNAPSHOOT_FILE_EXTENSION}�Ŀ����ļ�", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
            }
            else
            {
                MessageBox.Show($"��Ҫͨ����ק��һ�������ļ�����ȡ���Զ��������д洢�İٶ��������Ӻ���ȡ�룬����ȷ����һ����չ��Ϊ{AppConsts.SNAPSHOOT_FILE_EXTENSION}�Ŀ����ļ�", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            // ���Ӹ�Ŀ¼�ĵ�ַǰ׺��������Ǵ����̸�Ŀ¼��ʼ����ģ��ͻ�����/sharelink��ͷ�ĵ�ַ��
            string rootDirPath;

            SnapshootInfoVO snapShotInfo = new SnapshootInfoVO();

            /**
             * ����û�������Ŀ�Ƿ���ȷ
             */
            // �����������̵�ַ
            baiduPanUrl = TxtBaiduPanUrl.Text.Trim();
            if (string.IsNullOrEmpty(baiduPanUrl))
            {
                MessageBox.Show(this, "������ٶ��������ӵ�ַ", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (baiduPanUrl.StartsWith(AppConsts.BAIDU_PAN_URL_PREFIX, StringComparison.CurrentCultureIgnoreCase) == false)
            {
                MessageBox.Show(this, $"����İٶ��������ӵ�ַ�Ƿ�����������{AppConsts.BAIDU_PAN_URL_PREFIX}��ͷ�ĵ�ַ", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            // ����Ǵ�����ȡ������ӣ����л�ȡ��ȡ��
            int baiduPanUrlAccessCodeParamNameIndex = baiduPanUrl.IndexOf(AppConsts.BAIDU_PAN_URL_ACCESS_CODE_PARAM_NAME, StringComparison.CurrentCultureIgnoreCase);
            if (baiduPanUrlAccessCodeParamNameIndex != -1)
            {
                baiduPanAccessCode = baiduPanUrl.Substring(baiduPanUrlAccessCodeParamNameIndex + AppConsts.BAIDU_PAN_URL_ACCESS_CODE_PARAM_NAME.Length);
                baiduPanUrl = baiduPanUrl.Substring(0, baiduPanUrlAccessCodeParamNameIndex);
                if (AppConsts.CheckBaiduPanAccessCodeFormat(baiduPanAccessCode, out errorString) == false)
                {
                    MessageBox.Show(this, $"�Ӱٶ��������ӵ�ַ�л�ȡ����ȡ��{baiduPanAccessCode}����{errorString}", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            // ����������ȡ��
            string inputBaiduPanAccessCode = TxtBaiduPanAccessCode.Text.Trim();
            if (baiduPanUrlAccessCodeParamNameIndex != -1)
            {
                // �������İٶ��������Ӵ���ȡ�룬���û�������ȡ������������ˣ�Ҫ��֤һ��
                if (string.IsNullOrEmpty(inputBaiduPanAccessCode) == false)
                {
                    if (inputBaiduPanAccessCode.Equals(baiduPanAccessCode, StringComparison.CurrentCultureIgnoreCase) == false)
                    {
                        MessageBox.Show(this, $"�Ӱٶ��������ӵ�ַ�л�ȡ����ȡ��{baiduPanAccessCode}������ȡ��������������{inputBaiduPanAccessCode}��һ��", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
            }
            else
            {
                // �������İٶ��������Ӳ�����ȡ�룬����Ҫ������ȡ��
                if (string.IsNullOrEmpty(inputBaiduPanAccessCode) == true)
                {
                    MessageBox.Show(this, "�������Ӧ����ȡ��", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (AppConsts.CheckBaiduPanAccessCodeFormat(inputBaiduPanAccessCode, out errorString) == false)
                {
                    MessageBox.Show(this, $"�������ȡ��{baiduPanAccessCode}����{errorString}", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                baiduPanAccessCode = inputBaiduPanAccessCode;
            }
            // ��������Ҫ�������յ�Ŀ¼
            targetDir = TxtTargetDir.Text.Trim();
            if (string.IsNullOrEmpty(targetDir) == true)
            {
                MessageBox.Show(this, "������Ҫ�������յ�Ŀ¼·��������ӷ�������Ӷ�Ӧ�ĸ�Ŀ¼���н����������롰/��", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            // ��������������ʱ��
            string inputRequestIntervalString = TxtRequestInterval.Text.Trim();
            if (int.TryParse(inputRequestIntervalString, out _RequestIntervalMillisecond) == false)
            {
                MessageBox.Show(this, "����Ĵ����ļ��м��ʱ�䲻�ǺϷ�����", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (_RequestIntervalMillisecond < 0)
            {
                MessageBox.Show(this, "����Ĵ����ļ��м��ʱ�䲻��С��0", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            // ����������ȴ��ٶ����̷�������Ӧ��ʱ��
            string inputWaitServerString = TxtWaitServerMaxMillisecond.Text.Trim();
            if (int.TryParse(inputWaitServerString, out waitServerMaxMillisecond) == false)
            {
                MessageBox.Show(this, "����ĵȴ��ٶ����̷�������Ӧ���ʱ�䲻�ǺϷ�����", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (waitServerMaxMillisecond <= 0)
            {
                MessageBox.Show(this, "����ĵȴ��ٶ����̷�������Ӧ���ʱ��������0", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            AppendTextToConsole("=====================================================");
            AppendTextToConsole($"�ٶ����̵�ַ��{baiduPanUrl}");
            AppendTextToConsole($"��Ӧ����ȡ�룺{baiduPanAccessCode}");
            AppendTextToConsole($"Ҫ�������յ�Ŀ¼��{targetDir}");

            AppendTextToConsole($"{Environment.NewLine}������ʰٶ��������ӣ�");

            snapShotInfo.baiduPanUrl = baiduPanUrl;
            snapShotInfo.baiduPanAccessCode = baiduPanAccessCode;
            snapShotInfo.forDirPath = targetDir;
            snapShotInfo.saveTimestamp = DateTimeUtil.GetCurrentTimestampSecond();

            /**
             * ��֤����İٶ����̵�ַ�Ƿ���Ч
             */
            CookieContainer cookieContainer = new CookieContainer();
            HttpClientHandler handler = new HttpClientHandler();
            // ������ת��������https://pan.baidu.com/s/1XXXXXʱ�����Զ����ݷ��ص���ת��ַ��https://pan.baidu.com/share/init?surl=XXXXX
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
                MessageBox.Show(this, "����İٶ��������ӵ�ַ��Ч", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else if (reqBaiduUrlResultEnum == HttpRequestResultEnum.Timeout)
            {
                MessageBox.Show(this, "����ٶ����̳�ʱ���������ٽϺ�ʱ���Ի��ߵ�����ȴ�ʱ��", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else if (reqBaiduUrlResultEnum != HttpRequestResultEnum.Ok)
            {
                MessageBox.Show(this, $"����ٶ����̣�{baiduPanUrl}�����������쳣��ϢΪ��\n{errorString}", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (returnContent.Contains(AppConsts.BAIDU_PAN_URL_NOT_FOUND_TIPS))
            {
                MessageBox.Show(this, $"�ðٶ��������ӣ�{baiduPanUrl}���Ѳ�����", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            // ��ҳ���и���errortype�ж������Ƿ���Ч
            {
                Regex regex = new Regex("\"errortype\":(.*?),");
                Match match = regex.Match(returnContent);
                if (match.Success)
                {
                    string errorTypeStriing = match.Groups[1].Value;
                    int errorType;
                    if (int.TryParse(errorTypeStriing, out errorType) == false)
                    {
                        MessageBox.Show(this, $"����ٶ��������ӣ�{baiduPanUrl}�����������޷��ӷ��ص�HTML�У���errortypeתΪ���֣���ֵΪ{errorType}", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    if (errorType != -1)
                    {
                        if (AppConsts.ERROR_TYPE_TO_DESC.ContainsKey(errorType) == true)
                        {
                            MessageBox.Show(this, $"�ðٶ��������ӣ�{baiduPanUrl}����ʧЧ������ԭ��Ϊ��{AppConsts.ERROR_TYPE_TO_DESC[errorType]}", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        else
                        {
                            MessageBox.Show(this, $"�ðٶ��������ӣ�{baiduPanUrl}����ʧЧ����û�ж�Ӧ�Ĵ��������������ش�����Ϊ��{errorType}", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                }
                else
                {
                    MessageBox.Show(this, $"����ٶ��������ӣ�{baiduPanUrl}�����������޷��ӷ��ص�HTML�У��ҵ�errortype��HTML����Ϊ��{returnContent}", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            if (returnContent.Contains(AppConsts.BAIDU_PAN_URL_INPUT_ACCESS_CODE_TIPS) == false)
            {
                MessageBox.Show(this, $"����ٶ����̣�{baiduPanUrl}���������󣬷��ص�ҳ����Ϣ�в�������{AppConsts.BAIDU_PAN_URL_INPUT_ACCESS_CODE_TIPS}�����������޷��ж����Ƿ�Ϊ�ٶ�����ҳ��", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            AppendTextToConsole("������Ч");
            if (AppConsts.REQ_BAIDU_PAN_API_INTERVAL > 0)
                Thread.Sleep(AppConsts.REQ_BAIDU_PAN_API_INTERVAL);
            AppendTextToConsole("������֤��ȡ�룺");
            /**
             * ��֤�������ȡ���Ƿ���ȷ
             */
            // surlΪ����İٶ����̵�ַhttps://pan.baidu.com/s/1XXXXX��ȥ��1֮���ʣ��XXXXX����
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
                MessageBox.Show(this, "��֤��ȡ�볬ʱ���������ٽϺ�ʱ���Ի��ߵ�����ȴ�ʱ��", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else if (reqVerifyResultEnum != HttpRequestResultEnum.Ok)
            {
                MessageBox.Show(this, $"��֤��ȡ�뷢�������쳣��ϢΪ��\n{errorString}", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (returnContent.Contains("\"errno\":0") == false)
            {
                MessageBox.Show(this, $"��ȡ��У��ʧ�ܣ��������ȡ�������\n�ٶ����̷�����������ϢΪ��{returnContent}", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            AppendTextToConsole("��ȡ����ȷ");
            if (AppConsts.REQ_BAIDU_PAN_API_INTERVAL > 0)
                Thread.Sleep(AppConsts.REQ_BAIDU_PAN_API_INTERVAL);
            AppendTextToConsole("�ٴ�������ʰٶ��������ӣ�");
            /**
             * ͨ��У����ٴη��ʰٶ��������ӣ���Ϊ֮ǰУ��ͨ���������������ΪBDCLND��Cookie�����Ŵ�Cookie���·��ʰٶ����̵�ַ���ܿ���������ļ�
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
                MessageBox.Show(this, "ˢ������ٶ����̳�ʱ���������ٽϺ�ʱ���Ի��ߵ�����ȴ�ʱ��", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else if (refreshBaiduUrlResultEnum != HttpRequestResultEnum.Ok)
            {
                MessageBox.Show(this, $"ˢ������ٶ����̣�{baiduPanUrl}�����������쳣��ϢΪ��\n{errorString}", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            // �ӷ��ص�ҳ����������ȡ����֮���ȡ�ļ������Ĳ���
            // ��ȡshareid
            {
                Regex regex = new Regex("\"shareid\":(.*?),");
                Match match = regex.Match(returnContent);
                if (match.Success)
                {
                    shareId = match.Groups[1].Value;
                    AppendTextToConsole($"������shareid={shareId}");
                }
                else
                {
                    AppendTextToConsole($"�޷��Ӱٶ�����ҳ���л�ȡshareid��ҳ������Ϊ��{returnContent}");
                    MessageBox.Show(this, "�޷��Ӱٶ�����ҳ���л�ȡshareid", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            // ��ȡshare_uk
            {
                Regex regex = new Regex("\"share_uk\":\"(.*?)\",");
                Match match = regex.Match(returnContent);
                if (match.Success)
                {
                    shareUk = match.Groups[1].Value;
                    AppendTextToConsole($"������share_uk = {shareUk}");
                }
                else
                {
                    AppendTextToConsole($"�޷��Ӱٶ�����ҳ���л�ȡshare_uk��ҳ������Ϊ��{returnContent}");
                    MessageBox.Show(this, "�޷��Ӱٶ�����ҳ���л�ȡshare_uk", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            // ��ȡ���������ٶ����������и�Ŀ¼�����ļ��к����ļ���Ϣ����locals.mset(����һ���е�json��
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
                    AppendTextToConsole($"�޷��Ӱٶ�����ҳ���л�ȡlocals.mset�е�json��ҳ������Ϊ��{returnContent}");
                    MessageBox.Show(this, "�޷��Ӱٶ�����ҳ���л�ȡlocals.mset�е�json", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            // Ŀǰ���Է�������share/listʱ��������logid����Ҳ����ȷ�õ���������Ӧ
            //// �����е�logid����Cookie�е�BAIDUID����base64���ܺ���ַ���
            //string baiduIdFromCookie = HttpUtil.GetCookieValueByName(cc, "BAIDUID");
            //if (string.IsNullOrEmpty(baiduIdFromCookie) == true)
            //{
            //    errorString = "�޷���Cookie���ҵ�BAIDUID";
            //    return;
            //}

            List<DirOrFileInfoVO> rootDirChilds = new List<DirOrFileInfoVO>();
            try
            {
                JObject jObject = JsonConvert.DeserializeObject(rootPageReturnJson) as JObject;
                JArray fileList = jObject["file_list"] as JArray;
                AppendTextToConsole($"������ҳ����{fileList.Count}���ļ����ļ���");

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

                // ��ȡ���Ӹ�Ŀ¼�ĵ�ַǰ׺
                rootDirPath = (fileList[0] as JObject).Value<string>("parent_path").Replace("%2F", "/");
            }
            catch (Exception ex)
            {
                AppendTextToConsole($"�����Ӱٶ�����ҳ���л�ȡ���ļ��б�json��������json����Ϊ��\n{rootPageReturnJson}\n�쳣��ϢΪ��\n{ex.ToString()}");
                MessageBox.Show(this, "�����Ӱٶ�����ҳ���л�ȡ���ļ��б�json��������", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            // �ж��û��뽨�����յ�Ŀ¼�Ƿ����
            if (targetDir == "/")
            {
                snapShotInfo.childs = rootDirChilds;
                // ��������ҳ��ȡ�����ļ��У���û�б��������������ļ��к����ļ�
                foreach (DirOrFileInfoVO child in snapShotInfo.childs)
                {
                    if (child.isDir == true)
                    {
                        DirInfoVO childDirInfo = child as DirInfoVO;
                        List<DirOrFileInfoVO> allChilds = ReqGetDirInfo(string.Concat(rootDirPath, "/", childDirInfo.name), 1, httpClient, shareId, shareUk, out errorString);
                        if (string.IsNullOrEmpty(errorString) == false)
                        {
                            AppendTextToConsole($"ִ��ʧ�ܣ�����ԭ��\n{errorString}");
                            MessageBox.Show(this, "ִ��ʧ�ܣ�������־�����в鿴����ԭ��", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    AppendTextToConsole($"ִ��ʧ�ܣ�����ԭ��\n{errorString}");
                    MessageBox.Show(this, "ִ��ʧ�ܣ�������־�����в鿴����ԭ��", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            AppendTextToConsole("���ʰٶ��������ӻ�ȡ��ŵ��ļ��л��ļ���Ϣ���");

            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Title = "��ѡ������ļ�����·��";
            dialog.Filter = $"�ٶ����̿����ļ� (*.{AppConsts.SNAPSHOOT_FILE_EXTENSION})|*.{AppConsts.SNAPSHOOT_FILE_EXTENSION}";
            if (dialog.ShowDialog() != DialogResult.OK)
            {
                MessageBox.Show(this, "�����������ɿ���", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string savePath = dialog.FileName;
            try
            {
                File.WriteAllText(savePath, snapShotInfo.ToJson());
                AppendTextToConsole($"�ɹ���������ļ�����{savePath}");
                MessageBox.Show(this, "�ɹ����ɿ����ļ�", "��ϲ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                AppendTextToConsole($"��������ļ�ʱ���������쳣��ϢΪ��\n{ex.ToString()}");
                MessageBox.Show(this, "��������ļ�ʧ��", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// ���ذٶ�������ָ���ļ��а������������ļ��У��������¸��㼶�����ļ��к��ļ������ļ���Ϣ
        /// </summary>
        /// <param name="dirPath">�ٶ����̷���������У�ĳ���ļ��е�����·������/��/sharelink��ͷ</param>
        /// <param name="page">�ڼ�ҳ���ļ��л��ļ���Ϣ����ʼΪ1�����1ҳ���ܰ���ȫ��������Ϊ��ҳ</param>
        /// <param name="httpClient"></param>
        /// <param name="shareId"></param>
        /// <param name="shareUk"></param>
        /// <param name="errorString">����������󣬷��ش�������</param>
        /// <returns></returns>
        private List<DirOrFileInfoVO> ReqGetDirInfo(string dirPath, int page, HttpClient httpClient, string shareId, string shareUk, out string errorString)
        {
            int timespan = (int)((DateTime.Now - _LastReqShareListTime).TotalMilliseconds);
            if (timespan < _RequestIntervalMillisecond)
                Thread.Sleep(timespan);

            AppendTextToConsole($"�����ȡ���ļ��У�{dirPath}����{page}ҳ��Ϣ");

            List<DirOrFileInfoVO> childs = new List<DirOrFileInfoVO>();

            //string shareListUrl = $"https://pan.baidu.com/share/list?uk={shareUk}&shareid={shareId}&order=other&desc=1&showempty=0&web=1&page={page}&num={AppConsts.SHARE_LIST_ONE_PAGE_MAX_NUM}&dir={HttpUtility.UrlEncode(dirPath)}&channel=chunlei&web=1&app_id=250528&bdstoken=&logid={logId}&clienttype=0";
            // ע��dirPath����ΪQueryString������Ҫ����URL���룬��������ļ��������С�&�����ͻ��ԭ�����ֶηָ�������������
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
                errorString = $"�����ȡ���ļ��У�{dirPath}����{page}ҳ��Ϣ��ʱ���������ٽϺ�ʱ���Ի��ߵ�����ȴ�ʱ��";
                return null;
            }
            else if (reqShareListResultEnum != HttpRequestResultEnum.Ok)
            {
                errorString = $"�����ȡ���ļ��У�{dirPath}����{page}ҳ��Ϣʱ���������쳣��ϢΪ��\n{errorString}";
                return null;
            }
            if (returnContent.Contains("\"errno\":2") == true)
            {
                errorString = $"�����ȡ���ļ��У�{dirPath}����{page}ҳ��Ϣʱ�������󣬺ܿ�����Ϊ��·�������ڣ�������������ϢΪ��\n{returnContent}";
                return null;
            }
            if (returnContent.Contains("\"errno\":0") == true)
            {
                //AppendTextToConsole($"����jsonΪ��{returnContent}");
                /**
                 * ������·���µ��������ļ��к��ļ����������ļ��м�������������
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
                            // �����ļ���Ҫ���������������µ����ļ��к����ļ�
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
                    // ����ļ��л��ļ�������100�����п��ܻ��и����ļ�����Ҫ������һҳ
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
                    AppendTextToConsole($"���������ȡ���ļ��У�{dirPath}����{page}ҳ��Ϣ���ص�json��������json����Ϊ��\n{returnContent}\n�쳣��ϢΪ��\n{ex.ToString()}");
                    errorString = $"���������ȡ���ļ��У�{dirPath}����{page}ҳ��Ϣ���ص�json��������";
                    return null;
                }

                errorString = null;
                return childs;
            }
            else
            {
                errorString = $"�����ȡ���ļ��У�{dirPath}����Ϣʱ�������󣬷�����������ϢΪ��\n{returnContent}";
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
                MessageBox.Show(this, "��������ϵĿ����ļ�·��", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (File.Exists(oldSnapshootFilePath) == false)
            {
                MessageBox.Show(this, "����Ľ��ϵĿ����ļ�������", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (Path.GetExtension(oldSnapshootFilePath) != $".{AppConsts.SNAPSHOOT_FILE_EXTENSION}")
            {
                MessageBox.Show(this, $"ָ���Ľ��ϵĿ����ļ����Ϸ��������ļ���չ��ӦΪ{AppConsts.SNAPSHOOT_FILE_EXTENSION}", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                oldSnapshootJson = File.ReadAllText(oldSnapshootFilePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"��ȡ���ϵĿ����ļ�ʧ�ܣ��쳣��ϢΪ��\n{ex.ToString()}", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            oldSnapshootInfo = SnapshootInfoVO.FromJson(oldSnapshootJson, out errorString);
            if (errorString != null)
            {
                MessageBox.Show(this, $"ָ���Ľ��ϵĿ����ļ�����������ʹ�ñ��������ɵĿ����ļ����쳣��ϢΪ��\n{errorString}", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            newSnapshootFilePath = TxtNewSnapshootFilePath.Text.Trim();
            if (string.IsNullOrEmpty(newSnapshootFilePath) == true)
            {
                MessageBox.Show(this, "��������µĿ����ļ�·��", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (File.Exists(newSnapshootFilePath) == false)
            {
                MessageBox.Show(this, "����Ľ��µĿ����ļ�������", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (Path.GetExtension(newSnapshootFilePath) != $".{AppConsts.SNAPSHOOT_FILE_EXTENSION}")
            {
                MessageBox.Show(this, $"ָ���Ľ��µĿ����ļ����Ϸ��������ļ���չ��ӦΪ{AppConsts.SNAPSHOOT_FILE_EXTENSION}", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                newSnapshootJson = File.ReadAllText(newSnapshootFilePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"��ȡ���µĿ����ļ�ʧ�ܣ��쳣��ϢΪ��\n{ex.ToString()}", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            newSnapshootInfo = SnapshootInfoVO.FromJson(newSnapshootJson, out errorString);
            if (errorString != null)
            {
                MessageBox.Show(this, $"ָ���Ľ��µĿ����ļ�����������ʹ�ñ��������ɵĿ����ļ����쳣��ϢΪ��\n{errorString}", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            compareOldSnapshootDir = TxtCompareOldSnapshootDir.Text.Trim();
            if (string.IsNullOrEmpty(compareOldSnapshootDir) == true)
            {
                MessageBox.Show(this, "������Ҫ�ȽϵĽ��Ͽ����е�Ŀ¼·��������Ӹ�Ŀ¼��ʼ�Ƚϣ������롰/��", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            compareNewSnapshootDir = TxtCompareNewSnapshootDir.Text.Trim();
            if (string.IsNullOrEmpty(compareNewSnapshootDir) == true)
            {
                MessageBox.Show(this, "������Ҫ�ȽϵĽ��¿����е�Ŀ¼·��������Ӹ�Ŀ¼��ʼ�Ƚϣ������롰/��", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            AppendTextToConsole("=====================================================");
            AppendTextToConsole($"Ҫ���жԱȵĽ��ϵĿ����ļ�·����{oldSnapshootFilePath}");
            AppendTextToConsole($"Ҫ�Աȵ�·����{compareOldSnapshootDir}");
            AppendTextToConsole($"Ҫ���жԱȵĽ��µĿ����ļ�·����{newSnapshootFilePath}");
            AppendTextToConsole($"Ҫ�Աȵ�·����{compareNewSnapshootDir}");

            if (oldSnapshootFilePath.Equals(newSnapshootFilePath, StringComparison.CurrentCultureIgnoreCase) == true
                && compareOldSnapshootDir == compareNewSnapshootDir)
            {
                MessageBox.Show(this, "��ָ��Ҫ�Աȵ���ͬһ�������ļ����ҶԱȵ�·��Ҳ��ͬ������û�����壬��Ȼ����ͬ��", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (oldSnapshootInfo.baiduPanUrl.Equals(newSnapshootInfo.baiduPanUrl, StringComparison.CurrentCultureIgnoreCase) == false)
            {
                if (MessageBox.Show(this, $"��Ҫ�Աȵ����������ļ��������ڲ�ͬ�İٶ���������\n���ϵĿ��ն�Ӧ������Ϊ{oldSnapshootInfo.baiduPanUrl}\n���µĿ��ն�Ӧ������Ϊ{newSnapshootInfo.baiduPanUrl}\n\n��ȷ��Ҫ�Ա������������ļ���", "ע��", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.Cancel)
                    return;
            }

            oldTargetDirInfo = new DirInfoVO();
            // Ҫ�Աȵĸ�Ŀ¼�����ƣ�����ԭ����ʲô����ͳһȥ��
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

                        MessageBox.Show(this, $"�ڽ��ϵĿ����ļ��У��Ҳ���·����{pathBuilder.ToString()}��", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                oldTargetDirInfo = currentDir;
                // Ҫ�Աȵĸ�Ŀ¼�����ƣ�����ԭ����ʲô����ͳһȥ��
                oldTargetDirInfo.name = "";
            }

            newTargetDirInfo = new DirInfoVO();
            // Ҫ�Աȵĸ�Ŀ¼�����ƣ�����ԭ����ʲô����ͳһȥ��
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

                        MessageBox.Show(this, $"�ڽ��µĿ����ļ��У��Ҳ���·����{pathBuilder.ToString()}��", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                newTargetDirInfo = currentDir;
                // Ҫ�Աȵĸ�Ŀ¼�����ƣ�����ԭ����ʲô����ͳһȥ��
                newTargetDirInfo.name = "";
            }

            DirDiffVO diff = CompareTwoDirInfo(oldTargetDirInfo, newTargetDirInfo, "/");
            diff.CleanNotDiffChildsDir();

            // �ڶԱȽ��������չʾ
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
                 * �Ա������ļ����������ļ��С����ļ��Ĳ���
                 */
                // ���Ƚ����ļ��С����ļ�����������Ϊ�ֵ�ṹ
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

                // �������ԱȲ��洢������Ϣ��ע�����水�����ļ�-ɾ���ļ�-�޸��ļ�-�����ļ���-ɾ���ļ���-��ͬ�ļ��е�˳�����
                // ��˳��Ҳ������֮���������Ŀ¼��ʱ��˳��

                // �Ա��ҵ����¿������������ļ�
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
                // �Ա��ҵ����¿�����ɾ�����ļ�
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
                // �Ա��ҵ����ϡ����¿��ն����ڵ������޸ĵ��ļ����ļ�MD5ֵ�仯��
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
                // �Ա��ҵ����¿������������ļ���
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
                // �Ա��ҵ����¿�����ɾ�����ļ���
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
                // �Խ��ϡ����¿��ն����ڵ��ļ��У��������±����Ƚ����������ļ��С����ļ�
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
            dialog.Title = "��ѡ����ϵĿ����ļ�����·��";
            dialog.Multiselect = false;
            dialog.Filter = $"�ٶ����̿����ļ� (*.{AppConsts.SNAPSHOOT_FILE_EXTENSION})|*.{AppConsts.SNAPSHOOT_FILE_EXTENSION}";
            if (dialog.ShowDialog() == DialogResult.OK)
                TxtOldSnapshootFilePath.Text = dialog.FileName;
        }

        private void BtnChooseNewSnapshootFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "��ѡ����ϵĿ����ļ�����·��";
            dialog.Multiselect = false;
            dialog.Filter = $"�ٶ����̿����ļ� (*.{AppConsts.SNAPSHOOT_FILE_EXTENSION})|*.{AppConsts.SNAPSHOOT_FILE_EXTENSION}";
            if (dialog.ShowDialog() == DialogResult.OK)
                TxtNewSnapshootFilePath.Text = dialog.FileName;
        }

        private void AppendTextToConsole(object text, bool isAppendNewLine = true)
        {
            // �ø��ı����ȡ����
            RtxLog.Focus();
            // ���ù���λ�õ��ı���β
            RtxLog.Select(RtxLog.TextLength, 0);
            // ���������ı����괦
            RtxLog.ScrollToCaret();
            // ׷������
            RtxLog.AppendText(text as string);
            // ����
            if (isAppendNewLine)
                RtxLog.AppendText(Environment.NewLine);
        }

        private void BtnChooseSnapshootFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "��ѡ��ٶ����̿����ļ�����·��";
            dialog.Multiselect = false;
            dialog.Filter = $"�ٶ����̿����ļ� (*.{AppConsts.SNAPSHOOT_FILE_EXTENSION})|*.{AppConsts.SNAPSHOOT_FILE_EXTENSION}";
            if (dialog.ShowDialog() == DialogResult.OK)
                TxtSnapshootFilePath.Text = dialog.FileName;
        }

        private void BtnChooseLocalDir_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "��ѡ��Ҫ�Աȵı����ļ���";
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
                    MessageBox.Show(this, "��ѡ�ˡ����Դﵽ���´�С���ļ������ͱ����趨�ļ���Сֵ", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                double num;
                if (double.TryParse(ignoreBigFileSizeString, out num) == false)
                {
                    MessageBox.Show(this, "������ļ���Сֵ���ǺϷ�����", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (num <= 0)
                {
                    if (DialogResult.Cancel == MessageBox.Show(this, "������ļ���Сֵ��Ϊ���������������Զ������ļ���MD5���㣬�Ӷ����Ա��ļ����죬ȷ��Ҫ��������\n\n�����ȷ�ϡ�����ִ�У������ȡ���������Ա�", "ע��", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning))
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
                    MessageBox.Show(this, "��ѡ�ˡ�����������չ�����ļ������ͱ�������Ҫ������Щ��չ��", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                MessageBox.Show(this, "������ٶ����̿����ļ�·��", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (File.Exists(snapshootFilePath) == false)
            {
                MessageBox.Show(this, "����İٶ����̿����ļ�������", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (Path.GetExtension(snapshootFilePath) != $".{AppConsts.SNAPSHOOT_FILE_EXTENSION}")
            {
                MessageBox.Show(this, $"ָ���İٶ����̿����ļ����Ϸ��������ļ���չ��ӦΪ{AppConsts.SNAPSHOOT_FILE_EXTENSION}", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                snapshootJson = File.ReadAllText(snapshootFilePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"��ȡ�ٶ����̿����ļ�ʧ�ܣ��쳣��ϢΪ��\n{ex.ToString()}", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            snapshootInfo = SnapshootInfoVO.FromJson(snapshootJson, out errorString);
            if (errorString != null)
            {
                MessageBox.Show(this, $"ָ���İٶ����̿����ļ�����������ʹ�ñ��������ɵĿ����ļ����쳣��ϢΪ��\n{errorString}", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            compareSnapshootDir = TxtCompareSnapshootDir.Text.Trim();
            if (string.IsNullOrEmpty(compareSnapshootDir) == true)
            {
                MessageBox.Show(this, "������Ҫ�Ƚϵİٶ����̿����е�Ŀ¼·��������Ӹ�Ŀ¼��ʼ�Ƚϣ������롰/��", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            localDirPath = TxtLocalDirPath.Text.Trim();
            if (string.IsNullOrEmpty(localDirPath) == true)
            {
                MessageBox.Show(this, "������Ҫ�ȶԵı����ļ���·��", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (Directory.Exists(localDirPath) == false)
            {
                MessageBox.Show(this, "�����Ҫ�ȶԵı����ļ��в�����", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            AppendTextToConsole("=====================================================");
            AppendTextToConsole($"Ҫ���жԱȵİٶ����̿����ļ�·����{snapshootFilePath}");
            AppendTextToConsole($"Ҫ�Աȵ�·����{compareSnapshootDir}");
            AppendTextToConsole($"Ҫ���жԱȵı����ļ���·����{localDirPath}");

            targeSnapshootDirInfo = new DirInfoVO();
            // Ҫ�Աȵĸ�Ŀ¼�����ƣ�����ԭ����ʲô����ͳһȥ��
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

                        MessageBox.Show(this, $"�ڰٶ����̿����ļ��У��Ҳ���·����{pathBuilder.ToString()}��", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                targeSnapshootDirInfo = currentDir;
                // Ҫ�Աȵĸ�Ŀ¼�����ƣ�����ԭ����ʲô����ͳһȥ��
                targeSnapshootDirInfo.name = "";
            }

            if (ignoreFileMD5Config.IgnoreFileSizeByte >= 0)
            {
                if (DialogResult.Cancel == MessageBox.Show(this, "������ļ�MD5��ʱ�ϳ����������й����п��ܱ��ֳ����ٻ�������������������벻Ҫǿ����ֹ\n\n�����ȷ������ʼ�Աȱ���Ŀ¼�ļ�������MD5�������ȡ�����������ζԱ�", "ע��", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning))
                    return;
            }

            // Ϊ����Ŀ¼��������
            DirInfoVO localDirInfo = new DirInfoVO();
            localDirInfo.name = "";
            List<string> ignoreCalculateMD5FilePathList;
            localDirInfo.childs = GenerateLocalDirChildsInfo(new DirectoryInfo(localDirPath), ignoreFileMD5Config, out errorString, out ignoreCalculateMD5FilePathList);
            if (errorString != null)
            {
                MessageBox.Show(this, $"��ȡ����Ŀ¼��Ϣʧ�ܣ���������ֹ������ԭ��Ϊ��\n{errorString}", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else if (ignoreCalculateMD5FilePathList.Count > 0)
            {
                AppendTextToConsole("\n�������õġ����㱾���ļ�MD5ѡ��������ļ�������MD5��Ҳ�Ͳ����жԱȣ�");
                AppendTextToConsole(StringUtil.CombineString(ignoreCalculateMD5FilePathList, Environment.NewLine));
            }

            DirDiffVO diff = CompareLocalDirInfoAndSnapshoot(localDirInfo, targeSnapshootDirInfo, "/");
            diff.CleanNotDiffChildsDir();

            // �ڶԱȽ��������չʾ
            SnapshootAndLocalDirDiffResultForm diffResultForm = new SnapshootAndLocalDirDiffResultForm(snapshootFilePath, compareSnapshootDir,
                localDirPath, diff);
            diffResultForm.ShowDialog();
        }

        private List<DirOrFileInfoVO> GenerateLocalDirChildsInfo(DirectoryInfo directoryInfo, IgnoreFileMD5Config ignoreFileMD5Config, out string errorString, out List<string> ignoreCalculateMD5FilePathList)
        {
            List<DirOrFileInfoVO> childsList = new List<DirOrFileInfoVO>();
            ignoreCalculateMD5FilePathList = new List<string>();

            // ���ļ���
            foreach (DirectoryInfo childDir in directoryInfo.GetDirectories())
            {
                DirInfoVO dirInfo = new DirInfoVO();
                dirInfo.name = childDir.Name;
                dirInfo.serverModifyTimestamp = DateTimeUtil.DateTimeToTimestampSecond(childDir.LastWriteTime);
                // ���µݹ�������ļ��к����ļ�
                List<DirOrFileInfoVO> list = GenerateLocalDirChildsInfo(childDir, ignoreFileMD5Config, out errorString, out ignoreCalculateMD5FilePathList);
                if (errorString != null)
                    return null;

                dirInfo.childs.AddRange(list);
                childsList.Add(dirInfo);
            }
            // ���ļ�
            foreach (FileInfo childFile in directoryInfo.GetFiles())
            {
                FileInfoVO fileInfo = new FileInfoVO();
                fileInfo.name = childFile.Name;
                fileInfo.serverModifyTimestamp = DateTimeUtil.DateTimeToTimestampSecond(childFile.LastWriteTime);
                fileInfo.fileSize = childFile.Length;
                // �ļ���С������Լ���MD5��ѡ��
                if (ignoreFileMD5Config.IgnoreFileSizeByte != 0 && fileInfo.fileSize >= ignoreFileMD5Config.IgnoreFileSizeByte)
                {
                    childsList.Add(fileInfo);
                    ignoreCalculateMD5FilePathList.Add(childFile.FullName);
                }
                // �ļ���չ��������Լ���MD5��ѡ��
                else if (ignoreFileMD5Config.IgnoreFileExtensions != null && ignoreFileMD5Config.IgnoreFileExtensions.Contains(childFile.Extension, StringComparer.CurrentCultureIgnoreCase))
                {
                    childsList.Add(fileInfo);
                    ignoreCalculateMD5FilePathList.Add(childFile.FullName);
                }
                else
                {
                    AppendTextToConsole($"��ʼ�����ļ�MD5��{childFile.FullName}");
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
                 * �Ա������ļ����������ļ��С����ļ��Ĳ���
                 */
                // ���Ƚ����ļ��С����ļ�����������Ϊ�ֵ�ṹ
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

                // �������ԱȲ��洢������Ϣ��ע�����水�����ļ�-ɾ���ļ�-�޸��ļ�-�����ļ���-ɾ���ļ���-��ͬ�ļ��е�˳�����
                // ��˳��Ҳ������֮���������Ŀ¼��ʱ��˳��

                // �Ա��ҵ��������������ļ�
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
                // �Ա��ҵ�������ɾ�����ļ�
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
                // �Ա��ҵ����պͱ���Ŀ¼�ж����ڵ������޸ĵ��ļ����ļ�MD5ֵ�仯��
                // ע����������ļ�û��MD5��˵�����ԶԸ��ļ�����MD5��Ҳ�ͷ���������в���Ƚ�
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
                // �Ա��ҵ��������������ļ���
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
                // �Ա��ҵ�������ɾ�����ļ���
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
                // �Կ��պͱ���Ŀ¼�����ڵ��ļ��У��������±����Ƚ����������ļ��С����ļ�
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
                MessageBox.Show(this, "��������ϵĿ����ļ�·��", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (File.Exists(oldSnapshootFilePath) == false)
            {
                MessageBox.Show(this, "����Ľ��ϵĿ����ļ�������", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (Path.GetExtension(oldSnapshootFilePath) != $".{AppConsts.SNAPSHOOT_FILE_EXTENSION}")
            {
                MessageBox.Show(this, $"ָ���Ľ��ϵĿ����ļ����Ϸ��������ļ���չ��ӦΪ{AppConsts.SNAPSHOOT_FILE_EXTENSION}", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                oldSnapshootJson = File.ReadAllText(oldSnapshootFilePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"��ȡ���ϵĿ����ļ�ʧ�ܣ��쳣��ϢΪ��\n{ex.ToString()}", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            oldSnapshootInfo = SnapshootInfoVO.FromJson(oldSnapshootJson, out errorString);
            if (errorString != null)
            {
                MessageBox.Show(this, $"ָ���Ľ��ϵĿ����ļ�����������ʹ�ñ��������ɵĿ����ļ����쳣��ϢΪ��\n{errorString}", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // �ڿ������������չʾ
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
                MessageBox.Show(this, "��������µĿ����ļ�·��", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (File.Exists(newSnapshootFilePath) == false)
            {
                MessageBox.Show(this, "����Ľ��µĿ����ļ�������", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (Path.GetExtension(newSnapshootFilePath) != $".{AppConsts.SNAPSHOOT_FILE_EXTENSION}")
            {
                MessageBox.Show(this, $"ָ���Ľ��µĿ����ļ����Ϸ��������ļ���չ��ӦΪ{AppConsts.SNAPSHOOT_FILE_EXTENSION}", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                newSnapshootJson = File.ReadAllText(newSnapshootFilePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"��ȡ���µĿ����ļ�ʧ�ܣ��쳣��ϢΪ��\n{ex.ToString()}", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            newSnapshootInfo = SnapshootInfoVO.FromJson(newSnapshootJson, out errorString);
            if (errorString != null)
            {
                MessageBox.Show(this, $"ָ���Ľ��µĿ����ļ�����������ʹ�ñ��������ɵĿ����ļ����쳣��ϢΪ��\n{errorString}", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // �ڿ������������չʾ
            SnapshootViewerForm snapshootViewer = new SnapshootViewerForm(newSnapshootFilePath, newSnapshootInfo);
            snapshootViewer.ShowDialog();
        }

        private void BtnViewBaiduPanUrlAndCopyAccessCode_Click(object sender, EventArgs e)
        {
            string baiduPanUrl = null;
            string baiduPanAccessCode = null;
            string errorString = null;
            /**
             * ����û�������Ŀ�Ƿ���ȷ
             */
            // �����������̵�ַ
            baiduPanUrl = TxtBaiduPanUrl.Text.Trim();
            if (string.IsNullOrEmpty(baiduPanUrl))
            {
                MessageBox.Show(this, "������ٶ��������ӵ�ַ", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (baiduPanUrl.StartsWith(AppConsts.BAIDU_PAN_URL_PREFIX, StringComparison.CurrentCultureIgnoreCase) == false)
            {
                MessageBox.Show(this, $"����İٶ��������ӵ�ַ�Ƿ�����������{AppConsts.BAIDU_PAN_URL_PREFIX}��ͷ�ĵ�ַ", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            // ����Ǵ�����ȡ������ӣ����л�ȡ��ȡ��
            int baiduPanUrlAccessCodeParamNameIndex = baiduPanUrl.IndexOf(AppConsts.BAIDU_PAN_URL_ACCESS_CODE_PARAM_NAME, StringComparison.CurrentCultureIgnoreCase);
            if (baiduPanUrlAccessCodeParamNameIndex != -1)
            {
                baiduPanAccessCode = baiduPanUrl.Substring(baiduPanUrlAccessCodeParamNameIndex + AppConsts.BAIDU_PAN_URL_ACCESS_CODE_PARAM_NAME.Length);
                baiduPanUrl = baiduPanUrl.Substring(0, baiduPanUrlAccessCodeParamNameIndex);
                if (AppConsts.CheckBaiduPanAccessCodeFormat(baiduPanAccessCode, out errorString) == false)
                {
                    MessageBox.Show(this, $"�Ӱٶ��������ӵ�ַ�л�ȡ����ȡ��{baiduPanAccessCode}����{errorString}", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            // ����������ȡ��
            string inputBaiduPanAccessCode = TxtBaiduPanAccessCode.Text.Trim();
            if (baiduPanUrlAccessCodeParamNameIndex != -1)
            {
                // �������İٶ��������Ӵ���ȡ�룬���û�������ȡ������������ˣ�Ҫ��֤һ��
                if (string.IsNullOrEmpty(inputBaiduPanAccessCode) == false)
                {
                    if (inputBaiduPanAccessCode.Equals(baiduPanAccessCode, StringComparison.CurrentCultureIgnoreCase) == false)
                    {
                        MessageBox.Show(this, $"�Ӱٶ��������ӵ�ַ�л�ȡ����ȡ��{baiduPanAccessCode}������ȡ��������������{inputBaiduPanAccessCode}��һ��", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
            }
            else
            {
                // �������İٶ��������Ӳ�����ȡ�룬����Ҫ������ȡ��
                if (string.IsNullOrEmpty(inputBaiduPanAccessCode) == true)
                {
                    MessageBox.Show(this, "�������Ӧ����ȡ��", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (AppConsts.CheckBaiduPanAccessCodeFormat(inputBaiduPanAccessCode, out errorString) == false)
                {
                    MessageBox.Show(this, $"�������ȡ��{baiduPanAccessCode}����{errorString}", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                baiduPanAccessCode = inputBaiduPanAccessCode;
            }

            Clipboard.SetText(baiduPanAccessCode);
            Process.Start("explorer.exe", baiduPanUrl);
        }
    }
}