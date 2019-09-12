using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace FilenameRandomizerLB
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("  =====================================");
			Console.WriteLine("    Filename Randomizer");
			Console.WriteLine("    荒木層板小体");
			Console.WriteLine("    Copyright 2017-2018 Genta Ito");
			Console.WriteLine("  =====================================");
			Console.WriteLine("");
			Console.WriteLine("    ◎ファイル名命名規則");
			Console.WriteLine("      グリッド記号+半角スペース+切片番号+半角ハイフン+細胞番号+半角ハイフン+画像番号+半角スペース+拡大倍率");
			Console.WriteLine("      細胞番号は一桁とし、アルファベットの大文字小文字も可");
			Console.WriteLine("      画像番号は最も弱拡大のものを1とし、部分的に強拡大にしたものを2以降とする");
			Console.WriteLine("      フォルダ名にスペースを含まないこと");
			Console.WriteLine("");

			if (args.Length == 0)
			{
				Console.WriteLine("    フォルダーをアイコンにドロップしてください");
				Console.WriteLine("    処理を中断して終了します");
				Console.WriteLine("    何かキーを押してください");
				Console.ReadKey();
				Environment.Exit(0);
			}

			// 全ファイルが同じフォルダになければエラーを出して終了
			// フォルダ名にスペースがあればエラーを出して終了
			string folderName = args[0];
			string[] fileNames = Directory.GetFiles(folderName);

			foreach (string str in fileNames)
			{
				string tmpFolderName = Path.GetDirectoryName(str);

				if (tmpFolderName.Contains(" ") || tmpFolderName.Contains("　"))
				{
					Console.WriteLine("    エラー");
					Console.WriteLine("    フォルダ名にスペースが含まれています");
					Console.WriteLine("    処理を終了します");
					Console.WriteLine("    何かキーを押してください");
					Console.ReadKey();
					Environment.Exit(0);
				}

				if (tmpFolderName != folderName)
				{
					Console.WriteLine("　エラー");
					Console.WriteLine("　全ファイルが同一フォルダにある必要があります");
					Console.WriteLine("　処理を終了します");
					Console.WriteLine("　何かキーを押してください");
					Console.ReadKey();
					Environment.Exit(0);
				}
			}

			// ファイル数が2以下の場合エラーを出して終了
			if (fileNames.Length < 3)
			{
				Console.WriteLine("　エラー");
				Console.WriteLine("　ファイル数が少なすぎます");
				Console.WriteLine("　処理を終了します");
				Console.WriteLine("　何かキーを押してください");
				Console.ReadKey();
				Environment.Exit(0);
			}

			Console.WriteLine("  処理するファイルは以下の" + fileNames.Length.ToString() + "個です ...");
			for (int i = 0; i < fileNames.Length; i++)
			{
				Console.WriteLine("    " + fileNames[i]);
			}
			Console.WriteLine("  続行するには何かキーを押してください ...");
			Console.ReadKey();
			Console.WriteLine("");

			// N-N-Nが1で終わるものを親ファイルとしてリスト化
			// それ以外を子ファイルとして別にリスト化
			List<string> listParentFilePath = new List<string>();
			List<string> listChildFilePath = new List<string>();
			bool errFlag = false;
			for (int i = 0; i < fileNames.Length; i++)
			{
				string[] argSplit = fileNames[i].Split(' ');

				// 命名規則を満たしていないファイルがあればエラーを出して終了
				if (argSplit.Length != 3)
				{
					Console.WriteLine("  エラー");
					Console.WriteLine("  命名規則を満たしていないファイルがあります");
					Console.WriteLine("    " + fileNames[i]);
					errFlag = true;
				}

				string[] argSplit1Split = argSplit[1].Split('-');

				if (argSplit1Split[2] == "1")
				{
					listParentFilePath.Add(fileNames[i]);
				}
				else
				{
					listChildFilePath.Add(fileNames[i]);
				}
			}
			if (errFlag)
			{
				Console.WriteLine("　処理を終了します");
				Console.WriteLine("　何かキーを押してください");
				Console.ReadKey();
				Environment.Exit(0);
			}

			Console.WriteLine("  親ファイルは以下の" + listParentFilePath.Count.ToString() + "個です ...");
			for (int i = 0; i < listParentFilePath.Count; i++)
			{
				Console.WriteLine("    " + listParentFilePath[i]);
			}
			Console.WriteLine("  子ファイルは以下の" + listChildFilePath.Count.ToString() + "個です ...");
			for (int i = 0; i < listChildFilePath.Count; i++)
			{
				Console.WriteLine("    " + listChildFilePath[i]);
			}
			Console.WriteLine("  続行するには何かキーを押してください ...");
			Console.ReadKey();

			// 同一の「グリッド記号+画像番号」をもつ親ファイルがある場合にはエラーを出して終了
			// 親ファイルの「グリッド記号+画像番号」をリスト化
			List<string> listParentImgNum = new List<string>();
			foreach(string str in listParentFilePath)
			{
				string fn = Path.GetFileNameWithoutExtension(str);
				string[] fnsplit = fn.Split(' ');
				listParentImgNum.Add(fnsplit[0] + ' ' + fnsplit[1]);
			}
			int countListParentImgNum = listParentImgNum.Count;
			int distinctCountListParentImgNum = (from x in listParentImgNum select x).Distinct().Count();

			// 重複のある「グリッド記号+画像番号」を格納するリスト
			List<string> diff = new List<string>();
			foreach (string str in listParentImgNum)
			{
				int counter = 0;
				foreach (string str2 in listParentImgNum)
				{
					if (str2 == str)
					{
						counter++;
					}
				}
				if (counter > 1)
				{
					diff.Add(str);
				}
			}
			
			if (diff.Count != 0)
			{
				Console.WriteLine();
				Console.WriteLine("  エラー");
				Console.WriteLine("  親ファイルに重複する画像番号が存在します");
				foreach (string str in diff)
				{
					Console.WriteLine("    " + str);
				}
				Console.WriteLine("  終了します");
				Console.WriteLine("  何かキーを押してください");
				Console.ReadKey();
				Environment.Exit(0);
			}

			DateTime dtNow = DateTime.Now; // タイムスタンプ用

			int[] randNumArray = new int[listParentFilePath.Count]; // 乱数を生成
			for (int i = 0; i < randNumArray.Length; i++)
			{
				randNumArray[i] = i;
			}
			System.Random rnd = new System.Random();
			int n = randNumArray.Length;
			while (n > 1)
			{
				n--;
				int k = rnd.Next(n + 1);
				int tmp = randNumArray[k];
				randNumArray[k] = randNumArray[n];
				randNumArray[n] = tmp;
			}
			// 乱数リストを4桁に整形
			// 親ファイルが10000個を超えるとバグの原因となりうる
			List<string> randNumArrayFormat = new List<string>();
			foreach (int num in randNumArray)
			{
				string output = String.Format("{0:D4}", num);
				randNumArrayFormat.Add(output);
			}

			// 保存用フォルダ作成
			string newFolderName = folderName + "\\r" + dtNow.ToString("yyyyMMddHHmmss");
			try
			{
				DirectoryInfo di = Directory.CreateDirectory(newFolderName);
			}
			catch (Exception e)
			{
				Console.WriteLine("エラー：{0}", e.ToString());
				Console.ReadKey();
			}
			finally { }

			// 新ファイル名をキー、親ファイルパスを値とするディクショナリを生成
			SortedDictionary<string, string> randomizedPathCollection = new SortedDictionary<string, string>();
			for (int i = 0; i < listParentFilePath.Count; i++)
			{
				string newParentFileName = randNumArrayFormat[i] + "-" + "1";
				randomizedPathCollection.Add(newParentFileName, listParentFilePath[i]);
			}

			// 子ファイルが存在するか検索し、存在する場合には別のディクショナリに追加
			Dictionary<string, string> tempChildPathCollection = new Dictionary<string, string>();
			foreach (KeyValuePair<string, string> kv in randomizedPathCollection)
			{
				string cellID = CellIDFromFullPath(kv.Value);
				// 検索
				int subIDCounter = 1;
				foreach (string str in listChildFilePath)
				{
					string cellIDChild = CellIDFromFullPath(str);
					if (cellIDChild == cellID) // 細胞IDが一致するものがある場合
					{
						subIDCounter++;
						// Substringで細胞IDを抽出しているので、ここも
						// 親ファイルが10000個を超えるとバグの原因となりうる
						string randNumSub = kv.Key.Substring(0, 4) + "-" + subIDCounter.ToString();
						string childFileName = folderName + "\\" + Path.GetFileName(str);
						tempChildPathCollection.Add(randNumSub, childFileName); 
					}
				}
			}

			// 親ディクショナリと子ディクショナリを結合
			foreach (KeyValuePair<string, string> kv in tempChildPathCollection)
			{
				randomizedPathCollection.Add(kv.Key, kv.Value);
			}

			// ディクショナリどおりのファイル名をもつファイル群を別フォルダに生成
			foreach (KeyValuePair<string, string> kv in randomizedPathCollection)
			{
				string newFilePath = newFolderName + "//" + kv.Key + Path.GetExtension(kv.Value);
				FileInfo fi = new FileInfo(kv.Value);
				FileInfo finew = fi.CopyTo(newFilePath);
				finew.LastWriteTime = dtNow; // ファイルのタイムスタンプも匿名化
			}
			
			// 対照表をテキスト形式で同じフォルダに出力
			string strListFilePath = newFolderName + "//list" + dtNow.ToString("yyyyMMddHHmmss") + ".csv";
			using (var sw = new StreamWriter(strListFilePath, false))
			{
				foreach (KeyValuePair<string, string> kv in randomizedPathCollection)
				{
					sw.WriteLine("{0}, {1}", kv.Key, kv.Value);
				}
			}
		}

		/// <summary>
		/// 命名規則を満たすフルファイルパスを画像番号に変換する
		/// </summary>
		/// <param name="str">命名規則を満たすフルファイルパス</param>
		/// <returns>画像番号</returns>
		private static string CellIDFromFullPath(string str)
		{
			string filename = Path.GetFileNameWithoutExtension(str);
			// ファイル名をスペースごとに分割
			string[] nameSplit = filename.Split(' ');
			// 画像番号をハイフンごとに分割
			string[] nameSplit2 = nameSplit[1].Split('-');
			// グリッド記号+半角スペース+切片番号+半角ハイフン+細胞番号までをvalueとする
			string value = nameSplit[0] + ' ' + nameSplit2[0] + '-' + nameSplit2[1];
			return value;
		}
	}
}
