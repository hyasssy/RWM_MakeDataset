# 使い方
- 一応全てランタイムのGameViewで完結するようにしているので、ビルドしても使えると思いますが、テストしていません。

# 手順
## 1次データ制作
- Downloadsにdataというフォルダを作り、その中に動画をずらっと格納します。
- 画面に沿って必要な情報を埋めます。スペルミスなどには注意してください。
- Downloads/data/result/内に、
  - playareaで入力した値.json
  - Renameされた動画
  - 各動画のmetadataが配置されます。
- また、Downloads/dataにすでに同名のplayarea.jsonがあった場合、それに追記する形でresultにはjsonが出力されます。

## 2次データ制作
- Downloadsにdataというフォルダを作り、その中にplayArea.jsonとそこに記載された動画のmetadata.jsonをずらっと格納します。
- 画面でplayAreaを入力し、Execを押します
- Downloads/data/result/内に、
  緯度経度でresion(ファイルはRegionで分けられます)/district/cell/たくさんのフレーム情報
  という構造で整理された検索用データが生成されます。
