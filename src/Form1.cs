using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Text.RegularExpressions;
using Amazon.S3;
using Amazon.S3.Model;
using SevenZip.SDK;
using System.Net.Http;
using System.Net;
using System.Web;

namespace AperyGenerateTeacherGUI
{
    public partial class Form1 : Form
    {
        private String version = "1.12.0";
        private bool isOlderVersion = false;
        private Label labelThreads;
        private Label labelLoop;
        private Label labelLoopLog;
        private Label labelLog;
        private TextBox boxThreads;
        private TextBox boxLoop;
        private TextBox boxLoopLog;
        private TextBox boxLog;
        private Button btn;
        private Process process = null;
        private Thread thread = null;
        private bool runFrag = false;
        private TextBox textBox1;
        private long threads;
        private long teacherNodes = 1000000;
        private long loops;
        private Random random = null;
        private IAmazonS3 client = null;
        private bool testPassed = false;

        public Form1()
        {
            this.Width = 460;
            this.Height = 400;
            SetupControls();
        }
        public void SetupControls()
        {
            labelThreads = new Label();
            labelThreads.Text = "使用するスレッド数を入力して下さい。\n(最初に入っている値が最も効率的です。数値が大きいほどPCに負荷が掛かります。)";
            labelThreads.Height = 25;
            labelThreads.Width = this.Width;
            labelThreads.Top = 10;
            this.Controls.Add(labelThreads);
            boxThreads = new TextBox();
            boxThreads.Width = 50;
            boxThreads.Top = labelThreads.Bottom + 5;
            boxThreads.Left = 10;
            boxThreads.Text = Environment.ProcessorCount.ToString();
            this.Controls.Add(boxThreads);
            labelLoop = new Label();
            labelLoop.Text = "作成・送信を繰り返す回数を入力して下さい。\n" +
                "(１回で１００万局面分のデータを作成・送信します。\n" +
                " PCの性能によりますが、１回で３０分から２時間程度掛かると思います。\n" +
                " 大きめの数値を入力し、途中で止めたくなった時に\n" +
                " ×ボタンをクリックして終了して頂いても結構です。\n" +
                " その場合は最後の１回以外はデータを送信出来ます。)";
            labelLoop.Height = 75;
            labelLoop.Width = this.Width;
            labelLoop.Top = boxThreads.Bottom + 5;
            this.Controls.Add(labelLoop);
            boxLoop = new TextBox();
            boxLoop.Top = labelLoop.Bottom;
            boxLoop.Left = 10;
            boxLoop.Text = 1.ToString();
            this.Controls.Add(boxLoop);
            btn = new Button();
            btn.Text = "作成開始";
            btn.Height = 30;
            btn.Width = 100;
            btn.Top = boxLoop.Bottom + 5;
            btn.Left = 100;
            btn.Click += button1_Click;
            this.Controls.Add(btn);
            labelLoopLog = new Label();
            labelLoopLog.Text = "現在の作成回数";
            labelLoopLog.Height = 15;
            labelLoopLog.Width = this.Width;
            labelLoopLog.Top = btn.Bottom;
            this.Controls.Add(labelLoopLog);
            boxLoopLog = new TextBox();
            boxLoopLog.Top = labelLoopLog.Bottom;
            boxLoopLog.Left = 10;
            boxLoopLog.Text = "入力不要";
            this.Controls.Add(boxLoopLog);
            labelLog = new Label();
            labelLog.Text = "Info";
            labelLog.Height = 15;
            labelLog.Width = this.Width;
            labelLog.Top = boxLoopLog.Bottom + 5;
            this.Controls.Add(labelLog);
            boxLog = new TextBox();
            boxLog.Width = 350;
            boxLog.Top = labelLog.Bottom;
            boxLog.Left = 10;
            boxLog.Text = "入力不要";
            this.Controls.Add(boxLog);
        }

        private String RandomString(int length)
        {
            if (random == null)
                random = new Random();
            String candidates = "0123456789abcdefghijklmnopqrstuvwxyz";
            char[] list = new char[length];
            for (int i = 0; i < length; ++i)
                list[i] = candidates[random.Next(0, candidates.Length)];
            return new String(list);
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            // スレッド・プロセス終了
            if (thread != null)
            {
                if (!process.HasExited)
                {
                    process.Kill();
                }

                if (thread.IsAlive)
                {
                    thread.Abort();
                }

                thread = null;
            }

            base.OnClosing(e);
        }
        delegate void SetButtonEnabledCallback(bool b);
        private void SetButton(bool b)
        {
            if (this.btn.InvokeRequired)
            {
                SetButtonEnabledCallback d = new SetButtonEnabledCallback(SetButton);
                this.Invoke(d, new object[] { b });
            }
            else
            {
                if (b)
                    this.btn.Text = "作成開始";
                else
                    this.btn.Text = "作成中";
                this.btn.Enabled = b;
            }
        }
        delegate void SetTextCallback(string text);
        private void SetText(string text)
        {
            if (this.boxLog.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.boxLog.Text = text;
            }
        }
        delegate void SetLoopTextCallback(string loopText);
        private void SetLoopText(string loopText)
        {
            if (this.boxLoopLog.InvokeRequired)
            {
                SetLoopTextCallback d = new SetLoopTextCallback(SetLoopText);
                this.Invoke(d, new object[] { loopText });
            }
            else
            {
                this.boxLoopLog.Text = loopText;
            }
        }
        private void WriteContent(String file, bool isTest)
        {
            try {
                if (!isTest)
                    this.SetText("サーバーに教師データを送信しています。");
                var request = new PutObjectRequest
                {
                    BucketName = "apery-teacher-v" + version,
                    FilePath = file,
                    CannedACL = S3CannedACL.BucketOwnerFullControl,
                };
                var response = client.PutObject(request);
                if (!isTest)
                    this.SetText("サーバーに教師データを送信完了しました。ご協力ありがとうございました。");
                if (isTest)
                    testPassed = true;
                File.Delete(file);
            }
            catch (AmazonS3Exception amazonS3Excetion)
            {
                if (amazonS3Excetion.ErrorCode != null &&
                    (amazonS3Excetion.ErrorCode.Equals("AccessDenied")))
                {
                    this.SetText("このバージョンでのデータ受け付けを終了しました。");
                }
                else if (amazonS3Excetion.ErrorCode != null &&
                         (amazonS3Excetion.ErrorCode.Equals("InvalidAccessKeyId") ||
                          amazonS3Excetion.ErrorCode.Equals("InvalidSeculity")))
                {
                    this.SetText("サーバーにアクセス出来ませんでした。");
                }
                else
                {
                    if (isTest)
                        this.SetText(amazonS3Excetion.ErrorCode.ToString());
                    else
                        this.SetText("サーバーへの教師データ送信に失敗しました。");
                }
            }
        }
        private void ThreadProcSafe()
        {
            try {
                var webRequest = WebRequest.Create("https://hiraokatakuya.github.io/aperygenerateteacher/version.txt");
                var webResponse = webRequest.GetResponse();
                var webStream = webResponse.GetResponseStream();
                var streamReader = new StreamReader(webStream, Encoding.UTF8);
                var htmlSource = streamReader.ReadToEnd();
                streamReader.Close();
                webStream.Close();
                webResponse.Close();
                if (htmlSource == version + "\n")
                {
                    // 最新のバージョン
                    // nop
                }
                else
                {
                    // バージョンが更新されている。
                    this.SetText("最新バージョンをダウンロードしてからご協力頂けますと幸いです。");
                    isOlderVersion = true;
                    SetButton(true);
                    return;
                }
            }
            catch (WebException ex)
            {
                // バージョン確認にアクセス出来なかった。
                // 取りあえず、そのまま通常通りデータ生成を行う。
                // nop
            }
            // send test aws s3
            // テスト用ファイル作成
            String testfile = "test.txt";
            using (System.IO.FileStream stream = System.IO.File.Create(testfile))
            {
                if (stream != null)
                {
                    stream.Close();
                }
            }
            testPassed = false;
            Amazon.Runtime.AnonymousAWSCredentials testan = new Amazon.Runtime.AnonymousAWSCredentials();
            try
            {
                using (client = new AmazonS3Client(testan, Amazon.RegionEndpoint.EUWest1)) // リージョンは今後変わる可能性有り。
                {
                    WriteContent(testfile, true);
                };
            }
            catch (Amazon.Runtime.AmazonServiceException ex)
            {
                this.SetText("サーバー接続に失敗しました。");
                SetButton(true);
                return;
            }
            if (testPassed)
            {
                this.SetLoopText("0");
                for (long i = 1; i <= loops; ++i)
                {
                    process = new Process();
                    ProcessStartInfo startInfo = new ProcessStartInfo("apery.exe");
                    startInfo.CreateNoWindow = true;
                    startInfo.RedirectStandardInput = true;
                    startInfo.RedirectStandardOutput = true;
                    startInfo.UseShellExecute = false;

                    process.StartInfo = startInfo;
                    process.Start();

                    String outfile = "out_" + RandomString(20) + ".hcpe";
                    String line = null;
                    String cmd = "make_teacher roots.hcp " + outfile + " " + threads + " " + teacherNodes;
                    process.StandardInput.WriteLine(cmd);
                    while ((line = process.StandardOutput.ReadLine()) != null)
                    {
                        this.SetText(line);
                        if (System.Text.RegularExpressions.Regex.IsMatch(line, "^Made"))
                        {
                            break;
                        }
                    }
                    process.StandardInput.WriteLine("quit");
                    while ((line = process.StandardOutput.ReadLine()) != null)
                        ;
                    if (!process.HasExited)
                        process.Kill();
                    process.Close();

                    this.SetText("教師データをシャッフルしています。");
                    String shufOutfile = "shuf" + outfile;
                    startInfo = new ProcessStartInfo("shuffle_hcpe.exe");
                    startInfo.Arguments = outfile + " " + shufOutfile;
                    startInfo.CreateNoWindow = true;
                    startInfo.RedirectStandardInput = true;
                    startInfo.RedirectStandardOutput = true;
                    startInfo.UseShellExecute = false;

                    process.StartInfo = startInfo;
                    process.Start();
                    process.WaitForExit();
                    File.Delete(outfile);
                    this.SetText("教師データのシャッフルが完了しました。");
#if false
                    this.SetText("教師データ圧縮中");
                    String outCompressedFile = shufOutfile + ".7z";
                    CompressFile(shufOutfile, outCompressedFile);
                    this.SetText("教師データ圧縮完了");
                    File.Delete(shufOutfile);
#else
                    String outCompressedFile = shufOutfile;
#endif
                    // send aws s3
                    Amazon.Runtime.AnonymousAWSCredentials an = new Amazon.Runtime.AnonymousAWSCredentials();
                    try
                    {
                        using (client = new AmazonS3Client(an, Amazon.RegionEndpoint.EUWest1)) // リージョンは今後変わる可能性有り。
                        {
                            WriteContent(outCompressedFile, false);
                        };
                    }
                    catch (Amazon.Runtime.AmazonServiceException ex)
                    {
                        this.SetText("サーバー接続に失敗しました。");
                        SetButton(true);
                        return;
                    }
                    this.SetLoopText(i.ToString());
                }
            }
            SetButton(true);
        }
        private bool FileIsOK(String filename, long size)
        {
            var fi = new System.IO.FileInfo(filename);
            if (!fi.Exists)
            {
                boxLog.Text = filename + " がありません。";
                return false;
            }
            long fileSize = fi.Length;
            if (fileSize != size)
            {
                boxLog.Text = filename + " が破損しています。";
                return false;
            }
            return true;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (!FileIsOK("roots.hcp", 1499386784) ||
                !FileIsOK("apery.exe", 1363968) ||
                !FileIsOK("shuffle_hcpe.exe", 864768) ||
                !FileIsOK("20160911\\KPP_synthesized.bin", 776402496) ||
                !FileIsOK("20160911\\KKP_synthesized.bin", 81251424) ||
                !FileIsOK("20160911\\KK_synthesized.bin", 52488))
            {
                return;
            }
            if (!Int64.TryParse(boxThreads.Text, out threads))
            {
                return;
            }
            if (!Int64.TryParse(boxLoop.Text, out loops))
            {
                return;
            }

            this.thread = new Thread(new ThreadStart(this.ThreadProcSafe));

            this.thread.Start();
            SetButton(false);
        }
        private static void CompressFile(string inFile, string outFile)
        {
            var coder = new SevenZip.SDK.Compress.LZMA.Encoder();
            var input = new FileStream(inFile, FileMode.Open);
            var output = new FileStream(outFile, FileMode.Create);

            coder.WriteCoderProperties(output);
            output.Write(BitConverter.GetBytes(input.Length), 0, 8);

            coder.Code(input, output, input.Length, -1, null);
            output.Flush();
            output.Close();
            input.Close();
        }
    }
}
