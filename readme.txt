﻿aperygenerateteacher は、USI将棋エンジン Apery の評価関数学習の為の教師データを生成するツールです。
このツールのソースコードは GNU General Public License version 3 またはそれ以降のバージョンのもとで配布されます。

使い方は以下の通りです。
・フォルダ bin/ に入り、AperyGenerateTeacherGUI.exe をダブルクリックして下さい。
・次に、教師データ作成に使用するスレッド数、繰り返しの回数を半角英数字で入力して下さい。
  1回のデータ作成で100万局面分のデータを作成します。
  スレッド数は、CPUの論理コア数と同じにすると最も効率良く教師データを作成出来ますが、
  PCへの負荷が大きくなる為、適宜小さい値にして頂いて構いません。
  最初は論理コア数を設定してあります。
  繰り返し回数は、教師局面の作成、シャッフル、サーバーへの送信を1つのまとまりとして、何度行うかを設定できます。
・「作成開始」ボタンを押して下さい。操作は以上になります。
  教師データの作成、シャッフル、サーバーへの送信を自動的に行います。
  全て正常に終了すれば、「サーバーに教師データを送信完了しました。ご協力ありがとうございました。」と表示され、
  ボタンが「作成中」から「作成開始」に戻ります。

Apery を更に強くする為に、皆様、どうか力を貸して下さい。よろしくお願いいたします。

平岡 拓也


更新履歴

2016-10-07  HiraokaTakuya <hiraoka64@gmail.com>

	* v1.15.0
	評価関数ファイルを更新しました。

2016-10-01  HiraokaTakuya <hiraoka64@gmail.com>

	* v1.14.0
	評価関数ファイルを更新しました。

2016-09-17  HiraokaTakuya <hiraoka64@gmail.com>

	* v1.13.0
	評価関数ファイルを更新しました。

2016-09-11  HiraokaTakuya <hiraoka64@gmail.com>

	* v1.12.0
	評価関数ファイルを更新しました。

2016-09-08  HiraokaTakuya <hiraoka64@gmail.com>

	* v1.11.0
	評価関数ファイルを更新しました。

2016-09-04  HiraokaTakuya <hiraoka64@gmail.com>

	* v1.10.0
	評価関数ファイルを更新しました。
	探索深さを変更しました。

2016-08-28  HiraokaTakuya <hiraoka64@gmail.com>

	* v1.9.1
	探索深さを変更しました。

2016-08-24  HiraokaTakuya <hiraoka64@gmail.com>

	* v1.9.0
	評価関数ファイルを更新しました。
	1回で生成・送信する局面数を100万に固定しました。

2016-08-21  HiraokaTakuya <hiraoka64@gmail.com>

	* v1.8.0
	評価関数ファイルを更新しました。

2016-08-16  HiraokaTakuya <hiraoka64@gmail.com>

	* v1.7.0
	評価関数ファイルを更新しました。

2016-08-12  HiraokaTakuya <hiraoka64@gmail.com>

	* v1.6.0
	評価関数ファイルを更新しました。

2016-08-10  HiraokaTakuya <hiraoka64@gmail.com>

	* v1.5.0
	評価関数ファイルを更新しました。

2016-08-04  HiraokaTakuya <hiraoka64@gmail.com>

	* v1.4.0
	評価関数ファイルを更新しました。

2016-07-31  HiraokaTakuya <hiraoka64@gmail.com>

	* v1.3.0
	評価関数ファイルを更新しました。
	最新バージョンであるかチェックする機能を追加しました。

2016-07-28  HiraokaTakuya <hiraoka64@gmail.com>

	* v1.2.0
	評価関数ファイルを更新しました。

2016-07-22  HiraokaTakuya <hiraoka64@gmail.com>

	* v1.1.1
	評価関数ファイルのパスを更新しました。

2016-07-06  HiraokaTakuya <hiraoka64@gmail.com>

	* v1.0.3
	データ作成の繰り返し設定が出来るようになりました。
	作成するデータのフォーマットを変更しました。

2016-07-02  HiraokaTakuya <hiraoka64@gmail.com>

	* v1.0.2
	ファイル破損チェックの不具合を修正しました。

	* v1.0.1
	配布用zipファイルの展開時にファイルが破損した場合は教師データ作成をしないように修正しました。

	* v1.0.0
	最初のバージョンです。
