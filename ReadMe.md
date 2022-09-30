# 使い方
- 一応全てランタイムのGameViewで完結するようにしているので、ビルドしても使えると思いますが、テストしていません。

## 手順
- Downloadsにdataというフォルダを作り、その中に動画をずらっと格納します。
- 画面に沿って必要な情報を埋めます。スペルミスなどには注意してください。
- Downloads/data/result/内に、
  - playareaで入力した値.json
  - Renameされた動画
  - 各動画のmetadataが配置されます。
- また、Downloads/dataにすでに同名のplayarea.jsonがあった場合、それに追記する形でresultにはjsonが出力されます。
