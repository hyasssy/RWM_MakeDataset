# 使い方

- 一応全てランタイムの GameView で完結するようにしているので、ビルドしても使えると思いますが、テストしていません。

# 手順

## 1 次データ制作

- Downloads に data というフォルダを作り、その中に動画をずらっと格納します。(.mov)
- 画面に沿って必要な情報を埋めます。スペルミスなどには注意してください。
- Downloads/data/result/内に、
  - playarea で入力した値.json
  - videos (Rename された動画と各動画の metadata) が配置されます。
- また、Downloads/data にすでに同名の playarea.json があった場合、それに追記する形で result には json が出力されます。

## 2 次データ制作

- Downloads に playarea.json と videos(metadata だけ使用するが要は全部そのままつっこめば ok)
- 画面で playArea を入力し、Exec を押します
- Downloads/data/search/内に、
  緯度経度で resion(ファイルは Region で分けられます)/district/cell/たくさんのフレーム情報
  という構造で整理された検索用データが生成されます。
