using Amazon.S3;
using Amazon.S3.Model;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace AperyGenerateTeacherGUI
{
	public partial class Form1 : Form
	{
		private Random random = null;
		private Thread thread = null;
		private Process process = null;
		private IAmazonS3 client = null;
		private bool testPassed = false;
		private long loops;
		private long threads;
		private long teacherNodes;

		public Form1()
		{
			InitializeComponent();
			boxThreads.Value = Environment.ProcessorCount;
		}
		private string RandomString(int length)
		{
			if (random == null)
				random = new Random();
			string candidates = "0123456789abcdefghijklmnopqrstuvwxyz";
			char[] list = new char[length];
			for (int i = 0; i < length; ++i)
				list[i] = candidates[random.Next(0, candidates.Length)];
			return new string(list);
		}
		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
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
		}
		delegate void SetButtonEnabledCallback(bool b);
		private void SetButton(bool b)
		{
			if (btn.InvokeRequired)
			{
				SetButtonEnabledCallback d = new SetButtonEnabledCallback(SetButton);
				Invoke(d, new object[] { b });
			}
			else
			{
				if (b)
					btn.Text = "作成開始";
				else
					btn.Text = "作成中";
				btn.Enabled = b;
			}
		}
		delegate void SetTextCallback(string text);
		private void SetText(string text)
		{
			if (boxLog.InvokeRequired)
			{
				SetTextCallback d = new SetTextCallback(SetText);
				Invoke(d, new object[] { text });
			}
			else
			{
				boxLog.Text = text;
			}
		}
		delegate void SetLoopTextCallback(string loopText);
		private void SetLoopText(string loopText)
		{
			if (boxLoopLog.InvokeRequired)
			{
				SetLoopTextCallback d = new SetLoopTextCallback(SetLoopText);
				Invoke(d, new object[] { loopText });
			}
			else
			{
				boxLoopLog.Text = loopText;
			}
		}
		private void WriteContent(string file, bool isTest)
		{
			try
			{
				if (!isTest)
					SetText("サーバーに教師データを送信しています。");
				var request = new PutObjectRequest
				{
					BucketName = "apery-teacher-v1.2.0",
					FilePath = file,
					CannedACL = S3CannedACL.BucketOwnerFullControl,
				};
				var response = client.PutObject(request);
				if (!isTest)
					SetText("サーバーに教師データを送信完了しました。ご協力ありがとうございました。");
				if (isTest)
					testPassed = true;
				File.Delete(file);
			}
			catch (AmazonS3Exception amazonS3Excetion)
			{
				if (amazonS3Excetion.ErrorCode != null &&
					(amazonS3Excetion.ErrorCode.Equals("AccessDenied")))
				{
					SetText("このバージョンでのデータ受け付けを終了しました。");
				}
				else if (amazonS3Excetion.ErrorCode != null &&
						 (amazonS3Excetion.ErrorCode.Equals("InvalidAccessKeyId") ||
						  amazonS3Excetion.ErrorCode.Equals("InvalidSeculity")))
				{
					SetText("サーバーにアクセス出来ませんでした。");
				}
				else
				{
					if (isTest)
						SetText(amazonS3Excetion.ErrorCode.ToString());
					else
						SetText("サーバーへの教師データ送信に失敗しました。");
				}
			}
		}
		private void ThreadProcSafe()
		{
			// send test aws s3
			// テスト用ファイル作成
			string testfile = "test.txt";
			using (FileStream stream = File.Create(testfile))
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
				SetText("サーバー接続に失敗しました。");
				SetButton(true);
				return;
			}
			if (testPassed)
			{
				SetLoopText("0");
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

					string outfile = "out_" + RandomString(20) + ".hcpe";
					string line = null;
					string cmd = "make_teacher roots.hcp " + outfile + " " + threads + " " + teacherNodes;
					process.StandardInput.WriteLine(cmd);
					while ((line = process.StandardOutput.ReadLine()) != null)
					{
						SetText(line);
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

					SetText("教師データをシャッフルしています。");
					string shufOutfile = "shuf" + outfile;
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
					SetText("教師データのシャッフルが完了しました。");
#if false
                    SetText("教師データ圧縮中");
                    String outCompressedFile = shufOutfile + ".7z";
                    CompressFile(shufOutfile, outCompressedFile);
                    SetText("教師データ圧縮完了");
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
						SetText("サーバー接続に失敗しました。");
						SetButton(true);
						return;
					}
					SetLoopText(i.ToString());
				}
			}
			SetButton(true);
		}
		private bool FileIsOK(string filename, long size)
		{
			try
			{
				var fi = new FileInfo(filename);
				long fileSize = fi.Length;
				if (fileSize != size)
				{
					boxLog.Text = filename + " が破損しています。";
					return false;
				}
				return true;
			}
			catch (FileNotFoundException)
			{
				boxLog.Text = filename + " がありません。";
				return false;
			}
		}
		private void btn_Click(object sender, EventArgs e)
		{
			if (!FileIsOK("roots.hcp", 1499386784) ||
				!FileIsOK("apery.exe", 1363968) ||
				!FileIsOK("shuffle_hcpe.exe", 864768) ||
				!FileIsOK("20160727\\KPP_synthesized.bin", 776402496) ||
				!FileIsOK("20160727\\KKP_synthesized.bin", 81251424) ||
				!FileIsOK("20160727\\KK_synthesized.bin", 52488))
			{
				return;
			}
			if (!long.TryParse(boxThreads.Text, out threads))
			{
				return;
			}
			if (!long.TryParse(boxTeacherNodes.Text, out teacherNodes) || teacherNodes < 1000000)
			{
				return;
			}
			if (!long.TryParse(boxLoop.Text, out loops))
			{
				return;
			}

			thread = new Thread(new ThreadStart(ThreadProcSafe));

			thread.Start();
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
