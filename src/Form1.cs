using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Text.RegularExpressions;
using Amazon.S3;
using Amazon.S3.Model;
using SevenZip.SDK;

namespace AperyGenerateTeacherGUI
{
    public partial class Form1 : Form
    {
        private Label labelThreads;
        private Label labelTeacherNodes;
        private Label labelLog;
        private TextBox boxThreads;
        private TextBox boxTeacherNodes;
        private TextBox boxLog;
        private Button btn;
        private Process process = null;
        private Thread thread = null;
        private bool runFrag = false;
        private TextBox textBox1;
        private long threads;
        private long teacherNodes;
        private Random random = null;

        public Form1()
        {
            this.Width = 400;
            this.Height = 300;
            SetupControls();
        }
        public void SetupControls()
        {
            labelThreads = new Label();
            labelThreads.Text = "使用するスレッド数を入力して下さい。";
            labelThreads.Height = 30;
            labelThreads.Width = 300;
            labelThreads.Top = 10;
            this.Controls.Add(labelThreads);
            boxThreads = new TextBox();
            boxThreads.Width = 200;
            boxThreads.Top = labelThreads.Top + 30;
            boxThreads.Left = 10;
            this.Controls.Add(boxThreads);
            labelTeacherNodes = new Label();
            labelTeacherNodes.Text = "作成する教師局面数を入力して下さい。";
            labelTeacherNodes.Height = 30;
            labelTeacherNodes.Width = 200;
            labelTeacherNodes.Top = boxThreads.Top + 30;
            this.Controls.Add(labelTeacherNodes);
            boxTeacherNodes = new TextBox();
            boxTeacherNodes.Width = 225;
            boxTeacherNodes.Top = labelTeacherNodes.Top + 30;
            boxTeacherNodes.Left = 10;
            boxTeacherNodes.Text = "100万以上の値を入力して下さい。";
            this.Controls.Add(boxTeacherNodes);
            btn = new Button();
            btn.Text = "作成開始";
            btn.Height = 30;
            btn.Width = 100;
            btn.Top = boxTeacherNodes.Top + 30;
            btn.Left = 100;
            btn.Click += button1_Click;
            this.Controls.Add(btn);
            labelLog = new Label();
            labelLog.Text = "ログ";
            labelLog.Height = 30;
            labelLog.Width = 300;
            labelLog.Top = btn.Top + 40;
            this.Controls.Add(labelLog);
            boxLog = new TextBox();
            boxLog.Width = 350;
            boxLog.Top = labelLog.Top + 30;
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
        IAmazonS3 client = null;
        private void WriteContent(String file)
        {
            try {
                var request = new PutObjectRequest
                {
                    BucketName = "apery-teacher-v1.0.1",
                    FilePath = file,
                };
                var response = client.PutObject(request);
                this.SetText("サーバーに教師データを送信完了しました。ご協力ありがとうございました。");
                File.Delete(file);
            }
            catch (AmazonS3Exception amazonS3Excetion)
            {
                if (amazonS3Excetion.ErrorCode != null &&
                    (amazonS3Excetion.ErrorCode.Equals("InvalidAccessKeyId") ||
                    amazonS3Excetion.ErrorCode.Equals("InvalidSeculity")))
                {
                    this.SetText("サーバーにアクセス出来ませんでした。");
                }
                else
                {
                    this.SetText("サーバーへの教師データデータ送信に失敗しました。");
                }
            }
        }
        private void ThreadProcSafe()
        {
            process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo("apery.exe");
            startInfo.CreateNoWindow = true;
            startInfo.RedirectStandardInput = true;
            startInfo.RedirectStandardOutput = true;
            startInfo.UseShellExecute = false;

            process.StartInfo = startInfo;
            process.Start();

            String outfile = "out_" + RandomString(20) + ".fspe";
            String line = null;
            String cmd = "make_teacher roots.fsp " + outfile + " " + threads + " " + teacherNodes;
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

            this.SetText("教師データシャッフル中");
            String shufOutfile = "shuf" + outfile;
            startInfo = new ProcessStartInfo("shuffle_fspe.exe");
            startInfo.Arguments = outfile + " " + shufOutfile;
            startInfo.CreateNoWindow = true;
            startInfo.RedirectStandardInput = true;
            startInfo.RedirectStandardOutput = true;
            startInfo.UseShellExecute = false;

            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();
            File.Delete(outfile);
            this.SetText("教師データシャッフル完了");
            
            this.SetText("教師データ圧縮中");
            String outCompressedFile = shufOutfile + ".7z";
            CompressFile(shufOutfile, outCompressedFile);
            this.SetText("教師データ圧縮完了");
            File.Delete(shufOutfile);
            // send aws s3
            Amazon.Runtime.AnonymousAWSCredentials an = new Amazon.Runtime.AnonymousAWSCredentials();
            try {
                using (client = new AmazonS3Client(an, Amazon.RegionEndpoint.USWest1)) // リージョンは今後変わる可能性有り。
                {
                    WriteContent(outCompressedFile);
                };
            }
            catch (Amazon.Runtime.AmazonServiceException ex)
            {
                this.SetText("サーバー接続に失敗しました。");
                SetButton(true);
                return;
            }
            SetButton(true);
        }
        private bool FileIsOK(String filename, long size)
        {
            var fi = new System.IO.FileInfo(filename);
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
            if (!FileIsOK("roots.fsp", 4229347640) ||
                !FileIsOK("apery.exe", 1359360) ||
                !FileIsOK("shuffle_fspe.exe", 864768) ||
                !FileIsOK("20160307\\KPP_synthesized.bin", 776402496) ||
                !FileIsOK("20160307\\KKP_synthesized.bin", 81251424) ||
                !FileIsOK("20160307\\KK_synthesized.bin", 52488))
            {
                return;
            }
            if (!Int64.TryParse(boxThreads.Text, out threads))
            {
                return;
            }
            if (!Int64.TryParse(boxTeacherNodes.Text, out teacherNodes) || teacherNodes < 1000000)
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
