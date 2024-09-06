# UnityProject_README

## バージョン情報
- Unity バージョン: Unity 2022.3.34f1
- 最終更新日: 2024年-月-日

## ダウンロード方法
1. このリポジトリをクローンします。
   ```
   git clone https://github.com/your-repository.git
   ```
2. Unity Hubでプロジェクトを開きます。


##　audioの出力の指定方法
1. output device selectorスクリプトのobjectを開く
![alt text](image.png)
2. Oculus device name （部分検索）
![alt text](image-1.png)
システムのoutput deviceの名前をスクリプトの
oculus device nameのところに書くと、
そのデバイスに出力するようになる

## 片目問題
1.unityのedit-project settings
![alt text](image-2.png)
Stereo rendering mode をmulti passにする